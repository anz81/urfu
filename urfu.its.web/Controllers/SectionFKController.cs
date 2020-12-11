using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Ext.Utilities;
using Ext.Utilities.Linq;
//using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using PagedList;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Model.Models.ModulesVM;
using Urfu.Its.Web.Models;
using PagedList.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.SectionFKManager)]
    public class SectionFKController : BaseController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: SectionFK
        public ActionResult Index(int? page, int? limit, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var sortRules = SortRules.Deserialize(sort);

                var modules = GetFilteredModules(filter).Select(m => new
                {
                    id = m.uuid,
                    m.title,
                    m.shortTitle,
                    m.coordinator,
                    m.type,
                    m.competence,
                    m.testUnits,
                    m.priority,
                    m.state,
                    approvedDate = m.approvedDate.HasValue
                        ? m.approvedDate.Value.Day.ToString() + (m.approvedDate.Value.Month < 10 ? ".0" : ".") +
                          m.approvedDate.Value.Month.ToString() + "." + m.approvedDate.Value.Year.ToString()
                        : "", //АБ: это не я, это LINQ
                    m.comment,
                    m.file,
                    m.specialities
                });

                if (sortRules?.Count > 0)
                {
                    var sortRule = sortRules[0];
                    modules = modules.OrderBy(sortRule);
                }
                var paginated = modules.ToPagedList(page ?? 1, limit ?? 25);
                return Json(
                    new
                    {
                        data = paginated,
                        total = modules.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            ViewBag.Focus = focus;
            return View();
        }

        private IQueryable<Module> GetFilteredModules(string filter)
        {
            var filterRules = FilterRules.Deserialize(filter);

            var filterYear = filterRules?.Find(f => f.Property == "year");
            var filterSemester = filterRules?.Find(f => f.Property == "semester");

            if (filterRules != null)
            {
                filterRules.Remove(filterSemester);
                filterRules.Remove(filterYear);
            }

            var query = FilterPeriod(filterYear, filterSemester);

            var modules = query.OrderBy(m => m.title).AsQueryable();
            modules = modules.Where(filterRules);
            return modules;
        }


        public ActionResult DownloadSectionFKReport(string filter)
        {
            var minors = GetFilteredModules(filter);

            var stream = new VariantExport().Export(new
            {
                Rows = minors.Select(m => new
                {
                    m.title,
                    m.coordinator,
                    m.testUnits,
                    m.specialities,
                    m.state
                })
            }, "minors2ReportTemplate.xlsx");


            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Отчёт секциям ФК.xlsx".ToDownloadFileName());
        }

        public ActionResult DownloadProperties(int competitionGroupId, string filter)
        {
            var competitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("CompetitionGroup not found");

            var vms = GetPropertiesVMs(competitionGroupId, filter, competitionGroup);

            var stream = new VariantExport().Export(new
            {
                Rows = vms
            }, "sectionFKPropertiesReportTemplate.xlsx");


            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Отчёт секциям ФК в конкурсной группе {competitionGroup.Year} {competitionGroup.Semester.Name} {competitionGroup.ShortName}.xlsx"
                    .ToDownloadFileName());
        }

        private IQueryable<Module> FilterPeriod(FilterRule filterYear, FilterRule filterSemester)
        {
            int year;
            int semester;

            var haveYear = int.TryParse(filterYear?.Value, out year);
            var haveSemester = int.TryParse(filterSemester?.Value, out semester);

            if (haveYear && haveSemester)
                return
                    SectionFKForUser()
                        .Where(m => m.SectionFk.Periods.Any(p => (p.Year == year) && (p.SemesterId == semester)));

            if (haveYear)
                return SectionFKForUser().Where(m => m.SectionFk.Periods.Any(p => p.Year == year));

            if (haveSemester)
                return SectionFKForUser().Where(m => m.SectionFk.Periods.Any(p => p.SemesterId == semester));

            return SectionFKForUser();
        }

        private IQueryable<Module> SectionFKForUser()
        {
            return _db.UniModules().Where(m => m.type == ModuleTypes.SectionFK);
        }

        public ActionResult CompetitionGroups(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var competitionGroups = _db.SectionFKCompetitionGroups.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.ShortName,
                    c.StudentCourse,
                    c.Year,
                    c.SemesterId,
                    Semester = c.Semester.Name,
                    SelectedGroupsCount = c.Groups.Count,
                    HasAdmissions =
                    _db.SectionFKAdmissions.Any(
                        a => (a.SectionFKCompetitionGroupId == c.Id) && (a.Status == AdmissionStatus.Admitted)),
                    AdmissionCount =
                    _db.SectionFKAdmissions.Count(
                        a => (a.SectionFKCompetitionGroupId == c.Id) && (a.Status == AdmissionStatus.Admitted))
                });


                SortRules sortRules = SortRules.Deserialize(sort);
                competitionGroups = competitionGroups.OrderBy(sortRules.FirstOrDefault());

                ObjectableFilterRules rules = ObjectableFilterRules.Deserialize(filter);

                if (rules != null)
                {
                    foreach (var rule in rules)
                    {
                        switch (rule.Property)
                        {
                            case "Name":
                                string name = rule.Value.ToString();
                                competitionGroups = competitionGroups.Where(cg => !string.IsNullOrEmpty(name) &&
                                    cg.Name.Contains(name) ||
                                    cg.ShortName.Contains(name));
                                break;
                            case "StudentCourse":
                                int course;
                                if (int.TryParse(rule.Value.ToString(), out course))
                                {
                                    competitionGroups = competitionGroups.Where(cg => cg.StudentCourse == course);
                                }
                                break;
                            case "Year":
                                var years = GetListOfInt(rules, "Year");
                                if(years.Any())
                                 competitionGroups = competitionGroups.Where(cg => years.Contains(cg.Year));
                                break;
                            case "SemesterId":
                                var semesters = GetListOfInt(rules, "SemesterId");
                                 if(semesters.Any())
                                competitionGroups = competitionGroups.Where(cg => semesters.Any() && semesters.Contains(cg.SemesterId));
                                break;
                        }

                    }
                }

                return Json(
                    new
                    {
                        data = competitionGroups.OrderByDescending(g => g.Year)
                    },
                    new JsonSerializerSettings()
                );
            }


            return View(SectionFKForUser().ToList());
        }

        public IEnumerable<int> GetListOfInt(ObjectableFilterRules rules, string property)
        {
            var rule = rules?.FirstOrDefault(f => f.Property == property);
            if (rule != null)
            {
                var value = rule.Value;
                var list = value == null
                    ? new List<int>()
                    : JsonConvert.DeserializeObject<List<int>>(value.ToString());
                return list;
            }
            return new List<int>();
        }

        [HttpPost]
        public ActionResult PrepareAutoMove(int year, int semester,int? course, string modules)
        {
            var autoMoveData = CreateAutoMoveData(year, semester, course, modules);
            return View(autoMoveData);
        }

        private AutoMoveData CreateAutoMoveData(int year, int semester, int? course, string modules)
        {
            Logger.Info($"Предварительный расчёт автоперевода физкультуры {year} Семестр: {semester} Курс: {course} {modules}");

            ViewBag.year = year;
            ViewBag.semester = semester;
            ViewBag.course = course;
            ViewBag.modules = modules;

            var mids = JsonConvert.DeserializeObject<List<string>>(modules);

            var cg = course != null
                ? _db.SectionFKCompetitionGroups.Where(
                        c => c.Year == year && c.SemesterId == semester && c.StudentCourse == course)
                    .Include(s => s.Semester)
                    .ToList()
                : _db.SectionFKCompetitionGroups.Where(
                        c => c.Year == year && c.SemesterId == semester)
                    .Include(s => s.Semester)
                    .ToList();

            var autoMoveData = new AutoMoveData(cg, mids, _db);
            return autoMoveData;
        }

        public ActionResult PrepareAutoMoveDownload(int year, int semester, int? course, string modules)
        {
            var autoMoveData = CreateAutoMoveData(year, semester, course, modules);
            return autoMoveData.MassPrint();
        }

        public ActionResult ExecuteAutoMove(int year, int semester, int? course,string modules)
        {
            var autoMoveData = CreateAutoMoveData(year, semester,course, modules);
            Logger.Info($"Выполнение автоперевода физкультуры {year} {semester} {course} {modules}");
            autoMoveData.Execute();
            Logger.Info($"Автоперевод физкультуры выполнен");

            return RedirectToAction("Index");
        }

        public ActionResult GetAutoMoveReport(int id)
        {
            var report = _db.SectionFKAutoMoveReports.Find(id);
            return new FileContentResult(report.Content, "application/zip") { FileDownloadName = report.FileName };
        }

        [HttpPost]
        public ActionResult CreateOrUpdateCompetitionGroup(SectionFKCompetitionGroup competitionGroup)
        {
            if (ModelState.IsValid)
            {
                var competitionGroupFromBase =
                    _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroup.Id);
                if (competitionGroupFromBase == null)
                {
                    _db.SectionFKCompetitionGroups.Add(competitionGroup);
                }
                else
                {
                    var hasAdmissions =
                        _db.SectionFKAdmissions.Any(
                            _ =>
                                (_.SectionFKCompetitionGroupId == competitionGroup.Id) &&
                                (_.Status == AdmissionStatus.Admitted));
                    if (hasAdmissions)
                    {
                        competitionGroup.StudentCourse = competitionGroupFromBase.StudentCourse;
                    }
                    else if (competitionGroup.StudentCourse != competitionGroupFromBase.StudentCourse)
                    {
                        var admissions =
                            _db.SectionFKAdmissions.Where(_ => _.SectionFKCompetitionGroupId == competitionGroup.Id);
                        _db.SectionFKAdmissions.RemoveRange(admissions);
                        competitionGroupFromBase.Groups.Clear();
                    }
                    try
                    {
                        _db.SectionFKCompetitionGroups.Update(competitionGroup);
                    }
                    catch
                    {
                        _db.SectionFKCompetitionGroups.Add(competitionGroup);
                    }
                }
                _db.SaveChanges();
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            return NotFound("Validation error");
        }

        public ActionResult CompetitionGroupContentsTree(int? competitionGroupId, int? course, string filter)
        {
            if (competitionGroupId == null) return new StatusCodeResult(StatusCodes.Status404NotFound);
            var competitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);

            var divisions = _db.Divisions.ToList();

            List<GroupRow> groups;
            if (course != null)
                groups =
                    _db.GroupsHistories.Where(_ => _.YearHistory == competitionGroup.Year && (_.Course == course) 
                            && (_.Qual != "Магистр" && _.Qual != "Аспирант") && (_.FamType == "Очная")).Where(FilterRules.Deserialize(filter))
                        .Select(_=> new GroupRow()
                        {
                            Id = _.GroupId,
                            Name = _.Name,
                            ChairId = _.ChairId
                        }).ToList();
            else
                groups = _db.Groups.Select(_ => new GroupRow()
                {
                    Id = _.Id,
                    Name = _.Name,
                    ChairId = _.ChairId
                }).ToList();
            var model = new CompetitionGroupContentsViewModel(divisions, groups, competitionGroup);

            return JsonNet(model.Roots);
        }

        [HttpPost]
        public ActionResult DownloadBadGroups(List<BadGroups> badGroups)
        {
            var stream = new VariantExport().Export(new
            {
                Rows = badGroups
            }, "BadGroups.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
             $"Несоответствие состава конкурсных групп.xlsx"
                 .ToDownloadFileName());

        }
        [HttpPost]
        public ActionResult UpdateCompetitionGroupContents(int competitionGroupId, List<string> groupsIds, List<string> deselectedGroups, bool isFiltered)
        {
            var competitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);

            IEnumerable<object> badGroups = Enumerable.Empty<object>();
            if (competitionGroup != null)
            {
                IEnumerable<Group> groups = Enumerable.Empty<Group>();
                if (groupsIds != null)
                {

                    groups = _db.Groups.Where(_ => groupsIds.Contains(_.Id) && (!_db.SectionFKCompetitionGroups.Any(c => c.Id != competitionGroupId && c.Year == competitionGroup.Year && c.SemesterId == competitionGroup.SemesterId && c.Groups.Any(g => g.Id == _.Id)))).ToList();
                    var badGroupIds = groupsIds.Where(_ => !groups.Any(g => g.Id == _));
                    badGroups = _db.Groups.Where(_ => badGroupIds.Contains(_.Id)).Select(_ =>
                        new
                        {
                            competitionGroupName = _db.SectionFKCompetitionGroups.FirstOrDefault(c => competitionGroup.Year == c.Year && c.SemesterId == competitionGroup.SemesterId && c.Groups.Any(g => g.Id == _.Id)).ShortName,
                            group = _.Name,
                            groupId = _.Id
                        }
                    ).ToList();
                }

                if (!isFiltered)
                {
                    var deleteGroupIds = groupsIds != null
                        ? competitionGroup.Groups.Select(_ => _.Id).Except(groupsIds)
                        : competitionGroup.Groups.Select(g => g.Id);
                    LogCompetitionGroupChanges("удалена {0} {1} из {2}", deleteGroupIds, competitionGroup);
                    RemoveAdmissionsForGroup(competitionGroupId, deleteGroupIds);
                    var deletegroups = competitionGroup.Groups.Where(_ => deleteGroupIds.Contains(_.Id)).ToList();
                    foreach (var group in deletegroups)
                    {
                        competitionGroup.Groups.Remove(group);

                    }
                }
                else if (deselectedGroups != null)
                {
                    var toDelete = competitionGroup.Groups.Where(_ => deselectedGroups.Contains(_.Id)).ToList();
                    var toDeleteIds = toDelete.Select(_ => _.Id);
                    RemoveAdmissionsForGroup(competitionGroupId, toDeleteIds);
                    LogCompetitionGroupChanges("удалена {0} {1} из {2}", toDeleteIds, competitionGroup);

                    foreach (var @group in toDelete)
                    {
                        competitionGroup.Groups.Remove(@group);
                    }
                }
                
                foreach (var group in groups)
                { 
                    competitionGroup.Groups.Add(group);
                    Logger.Info("добавлен {0} {1} в {2}",group.Name,group.Id,competitionGroup.Name);
                }

                _db.SaveChanges();
                return Json(badGroups);
            }
            return NotFound("competition group not found");
        }

        private void LogCompetitionGroupChanges(string msg, IEnumerable<string> groupsIds,SectionFKCompetitionGroup competitionGroup)
        {
            var groups = _db.Groups.Where(_ => groupsIds.Contains(_.Id)).ToList();
            foreach (var @group in groups)
            {
                Logger.Info(msg,@group.Name,@group.Name,@group.Id,competitionGroup.Name);
            }
        }
        private void RemoveAdmissionsForGroup(int competitionGroupId, IEnumerable<string> toDeleteIds)
        {
            var admissions =
                _db.SectionFKAdmissions.Where(
                    a =>
                        a.SectionFKCompetitionGroupId == competitionGroupId &&
                        toDeleteIds.Contains(a.Student.GroupId)).ToList();
            var subgroupAdmissions =
                _db.SectionFKSubgroupMemberships.Where(
                    m =>
                        m.Subgroup.Meta.CompetitionGroupId == competitionGroupId &&
                        toDeleteIds.Contains(m.Student.GroupId)).ToList();
            _db.SectionFKAdmissions.RemoveRange(admissions);
            _db.SectionFKSubgroupMemberships.RemoveRange(subgroupAdmissions);
            _db.SaveChanges();

        }

        public ActionResult DeleteCompetitionGroup(int id)
        {
            //TODO: amir Надо до делать 
            _db.SectionFKProperties.RemoveRange(
                _db.SectionFKProperties.Where(
                    x => (x.SectionFKCompetitionGroupId == id) && (x.Limit == 0) && !x.Teachers.Any()));

            _db.SectionFKSubgroups.RemoveRange(
                _db.SectionFKSubgroups.Where(_ => _.Meta.CompetitionGroupId == id));

            _db.SectionFKSubgroupCounts.RemoveRange(
                _db.SectionFKSubgroupCounts.Where(_ => _.CompetitionGroupId == id));

            var competitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == id);
            if (competitionGroup != null)
                _db.SectionFKCompetitionGroups.Remove(competitionGroup);
            _db.SaveChanges();

            Logger.Info($"Удаление конкурсной группы по секциям ФК Id: {competitionGroup?.Id} Name: {competitionGroup?.Name}");
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult Properties(int competitionGroupId, string filter)
        {
            var competitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("CompetitionGroup not found");

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var properties = GetPropertiesVMs(competitionGroupId, filter, competitionGroup);


                return JsonNet(new { data = properties });
            }

            var sectionFks =
                _db.SectionFKs.Where(
                        _ =>
                            _.Periods.Any(
                                p => (p.Year == competitionGroup.Year) && (p.SemesterId == competitionGroup.SemesterId) 
                                && (p.Course == null || p.Course == competitionGroup.StudentCourse)))
                    .ToList();

            foreach (var sectionFk in sectionFks)
            {
                if (
                    competitionGroup.SectionFkProperties.Any(
                        _ =>
                            (_.SectionFKId == sectionFk.ModuleId) &&
                            (_.SectionFKCompetitionGroupId == competitionGroupId)))
                    continue;
                competitionGroup.SectionFkProperties.Add(new SectionFKProperty
                {
                    SectionFKId = sectionFk.ModuleId,
                    SectionFKCompetitionGroupId = competitionGroupId
                });
            }
            _db.SaveChanges();
            return View(competitionGroup);
        }

        private IEnumerable<object> GetPropertiesVMs(int competitionGroupId, string filter,
            SectionFKCompetitionGroup competitionGroup)
        {
            var filterRules = FilterRules.Deserialize(filter);
            string maleFilter = null;
            string sectionFkNameFilter = null;
            if (filterRules != null && filterRules.Count > 0)
            {
                maleFilter = filterRules.FirstOrDefault(f => f.Property == "Male").Value;
                sectionFkNameFilter = filterRules.FirstOrDefault(f => f.Property == "SectionFKName").Value;
            }
            var properties = _db.SectionFKProperties.Where(_ => _.SectionFKCompetitionGroupId == competitionGroupId)
                .Select(_ => new
                {
                    _.Id,
                    _.Limit,
                    _.TrainingPlaces,
                    _.Teachers,
                    _.SectionFKId,
                    SectionFKName = _.SectionFk.Module.shortTitle,
                    Male = _.SectionFk.Periods.FirstOrDefault(
                        p => (p.Year == competitionGroup.Year) && (p.SemesterId == competitionGroup.SemesterId)).Male
                })
                .Where(
                    _ =>
                        (sectionFkNameFilter == null || _.SectionFKName.Contains(sectionFkNameFilter)) &&
                        (maleFilter == null || (maleFilter == "-1" || _.Male.ToString() == maleFilter)))
                .ToList()
                .Select(_ => new
                {
                    _.Id,
                    _.SectionFKName,
                    _.Limit,
                    Admitted =
                    _db.SectionFKAdmissions.Count(
                        a => a.SectionFKCompetitionGroupId == competitionGroupId && a.SectionFKId == _.SectionFKId && a.Status == AdmissionStatus.Admitted),
                    AdmittedAWoSportsmens =
                    _db.SectionFKAdmissions.Count(
                        a =>
                            a.SectionFKCompetitionGroupId == competitionGroupId && a.SectionFKId == _.SectionFKId &&
                            !a.Student.Sportsman && a.Status == AdmissionStatus.Admitted &&
                            (a.Student.Status == "Активный" || a.Student.Status == "Отп.с.посещ.")),
                    admittedSportsmens = _db.SectionFKAdmissions.Count(
                        a =>
                            (a.SectionFKId == _.SectionFKId) &&
                            (a.SectionFKCompetitionGroupId == competitionGroupId) &&
                            (a.Status == AdmissionStatus.Admitted) &&
                            (a.Student.Sportsman)),
                    PlacesAvailable =
                    _.Limit -
                    _db.SectionFKAdmissions.Count(
                        a =>
                            a.SectionFKCompetitionGroupId == competitionGroupId && a.SectionFKId == _.SectionFKId &&
                            !a.Student.Sportsman && a.Status == AdmissionStatus.Admitted &&
                            (a.Student.Status == "Активный" || a.Student.Status == "Отп.с.посещ.")),
                    Teachers = string.Join(", ", _.Teachers.Select(t => t.initials).ToList()),

                    TrainingPlaces =
                    string.Join(", ",
                        _.TrainingPlaces.Select(t => $"{t.Description} ({t.Address})").ToList()),
                    Male = _.Male == null ? "Все" : _.Male == false ? "Женский" : "Мужской"
                });
            return properties;
        }

        public ActionResult PropertyTeachers(int? page, string sort, string filter, int? limit, int propertyId)
        {
            var property = _db.SectionFKProperties.Include("Teachers").FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("property not found");
            var teachers = _db.Teachers.Include("SectionFKProperties").Select(t => new
            {
                selected = t.SectionFKProperties.Any(_ => _.Id == propertyId),
                teacherId = t.pkey,
                t.firstName,
                t.middleName,
                t.lastName,
                t.workPlace
            });
            SortRules sortRules = SortRules.Deserialize(sort);
            teachers = teachers.OrderBy(sortRules.FirstOrDefault(), m => m.lastName);

            teachers = teachers.Where(FilterRules.Deserialize(filter));

            var paginated = teachers.ToPagedList(page ?? 1, limit ?? 25);

            return JsonNet(new
            {
                data = paginated,
                total = teachers.Count()
            });
        }

        [HttpPost]
        public ActionResult UpdateSectionFKPropertyTeachers(int propertyId, string teacherRows)
        {
            var property = _db.SectionFKProperties.FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");
            var teachers =
                (List<ModulePropertyRow<string>>)
                JsonConvert.DeserializeObject(teacherRows, typeof(List<ModulePropertyRow<string>>));
            foreach (var teacherRow in teachers)
            {
                var teacher = _db.Teachers.FirstOrDefault(_ => _.pkey == teacherRow.id);
                if (teacher != null)
                    if (property.Teachers.Any(_ => _.pkey == teacherRow.id))
                    {
                        if (!teacherRow.selected)
                            property.Teachers.Remove(teacher);
                    }
                    else if (teacherRow.selected)
                    {
                        property.Teachers.Add(teacher);
                    }
            }
            _db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult PropertyTrainingPlace(string sort, string filter, int propertyId)
        {
            var property = _db.SectionFKProperties.Include("TrainingPlaces").FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("property not found");
            var trainingPlaces = _db.FirstTrainingPlaceFKs.Include("SectionFKProperties").Select(f => new
            {
                selected = f.SectionFKProperties.Any(_ => _.Id == propertyId),
                id = f.Id,
                description = f.Description,
                address = f.Address
            });
            SortRules sortRules = SortRules.Deserialize(sort);
            trainingPlaces = trainingPlaces.OrderBy(sortRules.FirstOrDefault(), m => m.description);

            trainingPlaces = trainingPlaces.Where(FilterRules.Deserialize(filter));


            return JsonNet(new
            {
                data = trainingPlaces,
                total = trainingPlaces.Count()
            });
        }

        [HttpPost]
        public ActionResult UpdateSectionFKPropertyTrainingPlaces(int propertyId, string trainingPlaceRows)
        {
            var property = _db.SectionFKProperties.FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");
            var trainingPlaces =
                (List<ModulePropertyRow<int>>)
                JsonConvert.DeserializeObject(trainingPlaceRows, typeof(List<ModulePropertyRow<int>>));
            foreach (var row in trainingPlaces)
            {
                var trainingPlaceFK = _db.FirstTrainingPlaceFKs.FirstOrDefault(_ => _.Id == row.id);
                if (trainingPlaceFK != null)
                    if (property.TrainingPlaces.Any(_ => _.Id == row.id))
                    {
                        if (!row.selected)
                            property.TrainingPlaces.Remove(trainingPlaceFK);
                    }
                    else if (row.selected)
                    {
                        property.TrainingPlaces.Add(trainingPlaceFK);
                    }
            }
            _db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult SetSectionFKPropertyLimit(int propertyId, int limit)
        {
            var property = _db.SectionFKProperties.FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");
            property.Limit = limit;

            _db.SaveChanges();

            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult CopyMembershipPrepare(int a, int b)
        {
            var commonSections =
                _db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == a)
                    .Select(p => p.SectionFk)
                    .Intersect(
                        _db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == b).Select(p => p.SectionFk))
                    .ToList();

            var exceptSections =
                _db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == a)
                    .Select(p => p.SectionFk)
                    .Except(
                        _db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == b).Select(p => p.SectionFk))
                    .ToList();


            var newSections =
                _db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == b)
                    .Select(p => p.SectionFk)
                    .Except(
                        _db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == a).Select(p => p.SectionFk))
                    .ToList();

            var aCount =
                _db.SectionFKAdmissions.Count(
                    m => (m.SectionFKCompetitionGroupId == a) && (m.Status == AdmissionStatus.Admitted));
            var bCount =
                _db.SectionFKAdmissions.Count(
                    m => (m.SectionFKCompetitionGroupId == b) && (m.Status == AdmissionStatus.Admitted));

            var commongroups = _db.SectionFKCompetitionGroups.Where(cg => cg.Id == a)
                .SelectMany(g => g.Groups)
                .Intersect(_db.SectionFKCompetitionGroups.Where(cg => cg.Id == b).SelectMany(g => g.Groups))
                .ToList();


            var exceptgroups = _db.SectionFKCompetitionGroups.Where(cg => cg.Id == a)
                .SelectMany(g => g.Groups)
                .Except(_db.SectionFKCompetitionGroups.Where(cg => cg.Id == b).SelectMany(g => g.Groups))
                .ToList();


            var newgroups = _db.SectionFKCompetitionGroups.Where(cg => cg.Id == b)
                .SelectMany(g => g.Groups)
                .Except(_db.SectionFKCompetitionGroups.Where(cg => cg.Id == a).SelectMany(g => g.Groups))
                .ToList();

            var vm = new SectionFKCompetitionGroupMembershipsCopyVm
            {
                ACount = aCount,
                BCount = bCount,
                A = _db.SectionFKCompetitionGroups.Find(a),
                B = _db.SectionFKCompetitionGroups.Find(b),
                CommonSections = commonSections,
                ExceptSections = exceptSections,
                NewSections = newSections,
                CommonGroups = commongroups,
                NewGroups = newgroups,
                ExceptGroups = exceptgroups
            };

            foreach (var section in commonSections)
            {
                vm.AAdmissions[section] =
                    _db.SectionFKAdmissions.Count(
                        m =>
                            (m.SectionFKCompetitionGroupId == a) && (m.Status == AdmissionStatus.Admitted) &&
                            (m.SectionFKId == section.ModuleId));
                vm.BAdmissions[section] =
                    _db.SectionFKAdmissions.Count(
                        m =>
                            (m.SectionFKCompetitionGroupId == b) && (m.Status == AdmissionStatus.Admitted) &&
                            (m.SectionFKId == section.ModuleId));
            }

            foreach (var section in newSections)
                vm.BAdmissions[section] =
                    _db.SectionFKAdmissions.Count(
                        m =>
                            (m.SectionFKCompetitionGroupId == b) && (m.Status == AdmissionStatus.Admitted) &&
                            (m.SectionFKId == section.ModuleId));

            foreach (var section in exceptSections)
                vm.AAdmissions[section] =
                    _db.SectionFKAdmissions.Count(
                        m =>
                            (m.SectionFKCompetitionGroupId == a) && (m.Status == AdmissionStatus.Admitted) &&
                            (m.SectionFKId == section.ModuleId));
            return View(vm);
        }

        [HttpPost]
        public ActionResult CopyMembership(int src, int dst)
        {
            var commonSections =
                _db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == src)
                    .Select(p => p.SectionFk)
                    .Intersect(
                        _db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == dst)
                            .Select(p => p.SectionFk))
                    .Select(s => s.ModuleId)
                    .ToList();
            foreach (var sectionId in commonSections)
            {
                var todelete =
                    _db.SectionFKAdmissions.Where(
                        m => (m.SectionFKCompetitionGroupId == dst) && (m.SectionFKId == sectionId));
                _db.SectionFKAdmissions.RemoveRange(todelete);
                var newStudents = _db.SectionFKAdmissions.Where(m =>
                    (m.SectionFKCompetitionGroupId == src) &&
                    (m.SectionFKId == sectionId) &&
                    (m.Status == AdmissionStatus.Admitted) &&
                    m.Student.Group.SectionFkCompetitionGroups.Any(cg => cg.Id == dst) &&
                    (m.Student.Status == "Активный" || m.Student.Status == "Отп.с.посещ."));

                foreach (var adm in newStudents)
                    _db.SectionFKAdmissions.Add(new SectionFKAdmission
                    {
                        studentId = adm.studentId,
                        Published = false,
                        SectionFKCompetitionGroupId = dst,
                        SectionFKId = adm.SectionFKId,
                        Status = AdmissionStatus.Admitted
                    });
            }
            _db.SaveChanges();
            return RedirectToAction("Index", "SectionFKAdmission", new { competitionGroupId = dst });
        }

        static StudentDesicionList _sdl;
        public ActionResult CopyComposition(int src, int dst, bool subgroups, bool composition, bool subgroupTeachers)
        {
            ((IObjectContextAdapter)_db).ObjectContext.CommandTimeout = 1200000;

            _sdl = new StudentDesicionList();

            var srcCG = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == src);
            var dstCG = _db.SectionFKCompetitionGroups.Include(_ => _.Groups).FirstOrDefault(_ => _.Id == dst);
            var vm = new SectionFKCompositionCVM(srcCG, dstCG, subgroups, composition);
            CreateSubgroupCounts(src, dst, subgroups);
            vm.ExceptedSubgroupCounts =
                    _db.SectionFKSubgroupCounts.Where(_ => _.CompetitionGroupId == src).Select(s => s.SectionFKDisciplineTmerPeriod.Tmer.Discipline.SectionFK.Module.title + "/" + s.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer)
                        .Except(_db.SectionFKSubgroupCounts.Where(s => s.CompetitionGroupId == dst).Select(s => s.SectionFKDisciplineTmerPeriod.Tmer.Discipline.SectionFK.Module.title + "/" + s.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer)).ToList();

            foreach (var vmExceptedSubgroupCount in vm.ExceptedSubgroupCounts)
            {
                _sdl.AddNew("подгруппы не скопированы", vmExceptedSubgroupCount, "", null);
            }
            if (subgroups)
            {
                var subgroupController = new SectionFKSubgroupController(_db);


                if (subgroupController.CreateSubgroups(dst, dstCG))
                {
                
                    var intersectedSectionsFK = _db.SectionFKSubgroups
                    .Where(s => s.Meta.CompetitionGroupId == src)
                    .Select(s => s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId)
                    .Intersect(_db.SectionFKSubgroups
                    .Where(s => s.Meta.CompetitionGroupId == dst)
                    .Select(s => s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId)).ToList();

                    var srcSectionsFK = _db.SectionFKSubgroups.Where(s => s.Meta.CompetitionGroupId == src && intersectedSectionsFK.Contains(s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId)).Select(s => new { SectionFKId = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId, SubgroupId = s.Id, rmer = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer, TeacherId = s.TeacherId }).ToList();

                    var dstSectionsFK = _db.SectionFKSubgroups.Where(s => s.Meta.CompetitionGroupId == dst && intersectedSectionsFK.Contains(s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId)).Select(s => new { SectionFKId = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId, SubgroupId = s.Id, s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer}).ToList();

                    var dstGroupsIds = dstCG.Groups.Select(_ => _.Id).ToList();

                    foreach (var srcSectionFK in srcSectionsFK)
                    {
                        var subgroupAdmissions = _db.SectionFKSubgroupMemberships
                                .Where(adm => adm.SubgroupId == srcSectionFK.SubgroupId
                                    && adm.Subgroup.Meta.CompetitionGroupId == src
                                    && dstGroupsIds.Contains(adm.Student.GroupId)
                                    && _db.SectionFKAdmissions.FirstOrDefault(_ => _.studentId == adm.studentId
                                        && _.SectionFKCompetitionGroupId == dstCG.Id
                                        && _.Status == AdmissionStatus.Admitted).SectionFKId == adm.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.SectionFKId
                                    && (adm.Student.Status == "Активный" || adm.Student.Status == "Отп.с.посещ.")).ToList();

                        var dstSubgroup = dstSectionsFK.FirstOrDefault(ds => ds.SectionFKId == srcSectionFK.SectionFKId && ds.rmer == srcSectionFK.rmer);

                        if (dstSubgroup != null)
                        {
                            foreach (var sectionFKSubgroupMembership in subgroupAdmissions)
                            {
                                var membership = _db.SectionFKSubgroupMemberships.FirstOrDefault(
                                    _ =>
                                        _.studentId == sectionFKSubgroupMembership.studentId &&
                                        dstSubgroup.SubgroupId == _.SubgroupId);

                                if (membership == null)
                                {
                                    membership = new SectionFKSubgroupMembership()
                                    {
                                        studentId = sectionFKSubgroupMembership.studentId,
                                        SubgroupId = dstSubgroup.SubgroupId
                                    };
                                    _db.SectionFKSubgroupMemberships.Add(membership);
                                    Logger.Info($"Студент {membership.studentId} скопирован в подгруппу {membership.SubgroupId} ФК");
                                }

                            }
                             if (subgroupTeachers)
                            {

                                var dstGroup = _db.SectionFKSubgroups.FirstOrDefault(gr => gr.Id == dstSubgroup.SubgroupId);
                                dstGroup.TeacherId = srcSectionFK?.TeacherId;   

                            }
                            dstSectionsFK.Remove(dstSubgroup);
                        }

                    }

                    _db.SaveChanges();
                
                    var exceptedStudentIds = _db.SectionFKSubgroupMemberships
                        .Where(adm => adm.Subgroup.Meta.CompetitionGroupId == src
                                      && _db.SectionFKSubgroupCounts.Any(sc => sc.CompetitionGroupId == dst
                                                                                && sc.SectionFKDisciplineTmerPeriod.Period.SectionFKId == adm.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId)
                                      && (adm.Student.Status == "Активный" || adm.Student.Status == "Отп.с.посещ."))
                        .Select(_ => _.studentId)
                        .Except(
                                _db.SectionFKSubgroupMemberships
                                .Where(adm => adm.Subgroup.Meta.CompetitionGroupId == dst
                                              && (adm.Student.Status == "Активный" || adm.Student.Status == "Отп.с.посещ."))
                                //.DistinctBy(_ => _.studentId)
                                .Select(_ => _.studentId))
                                .Distinct();
                    var exceptedStudents = _db.Students.Where(_ => exceptedStudentIds.Contains(_.Id)).ToList();
                    foreach (var exceptedStudent in exceptedStudents)
                    {

                        if (!dstGroupsIds.Contains(exceptedStudent.GroupId))
                        {
                            _sdl.AddNew("Академическая группа не включена в состав конкурсной группы", "", exceptedStudent.Id, null);
                        }
                        else if (!_db.SectionFKAdmissions.Any(
                                adm => adm.SectionFKCompetitionGroupId == src && adm.studentId == exceptedStudent.Id
                                       && _db.SectionFKAdmissions.FirstOrDefault(_ => _.studentId == adm.studentId
                                                                                      &&
                                                                                      _.SectionFKCompetitionGroupId ==
                                                                                      dstCG.Id
                                                                                      &&
                                                                                      _.Status ==
                                                                                      AdmissionStatus.Admitted)
                                           .SectionFKId == adm.SectionFKId))
                        {
                            _sdl.AddNew("Студент отчислен или переведен из данной секции", "", exceptedStudent.Id, null);

                        }
                    }
                    _sdl.AddStudentsList(exceptedStudents);

                }

            }
          
            _db.MinorAutoAdmissionReports.Add(new MinorAutoAdmissionReport
            {
                Content = _sdl.GetStringBuilder().ToString(),
                Date = DateTime.Now,
                ModuleType = ModuleType.SectionFk
            });

            _db.SaveChanges();
            return View(vm);

        }
        public ActionResult DownloadAutoAdmissionReport()
        {
            Response.Headers.Add("Content-Disposition",
                "attachment; filename=Отчёт_по_копированиям_состав_подгрупп.txt");
            if (_sdl == null)
                _sdl = new StudentDesicionList();
            return File(_sdl.FormatStream(), "application/text");
        }
        private void CreateSubgroupCounts(int src, int dst, bool copyCount)
        {
            var destCompetitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == dst);
            var sections = destCompetitionGroup.SectionFkProperties.Select(_ => _.SectionFKId).AsQueryable(); 

            var metas = _db.SectionFKTmerPeriods
                .Where(
                    m =>
                        (m.Period.Year == destCompetitionGroup.Year) &&
                        (m.Period.SemesterId == destCompetitionGroup.SemesterId) &&  
                        (m.Period.Course == destCompetitionGroup.StudentCourse || m.Period.Course == null) &&
                        sections.Any(_ => _ == m.Period.SectionFKId)
                );

            foreach (var meta in metas.ToList())
            {
                var sectionFkSubgroupCount =
                    _db.SectionFKSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.CompetitionGroupId == dst) &&
                            (_.SectionFKDisciplineTmerPeriodId == meta.Id));

                var sourcSubgroupCount = _db.SectionFKSubgroupCounts.FirstOrDefault(_ => _.CompetitionGroupId == src && meta.Tmer.TmerId == _.SectionFKDisciplineTmerPeriod.Tmer.TmerId && meta.Period.SectionFKId == _.SectionFKDisciplineTmerPeriod.Period.SectionFKId);

                if (sectionFkSubgroupCount == null)
                {
                    var newSubgroupCount = new SectionFKSubgroupCount
                    {
                        CompetitionGroupId = dst,
                        SectionFKDisciplineTmerPeriodId = meta.Id,
                        GroupCount = 0
                    };
                    if (sourcSubgroupCount != null && copyCount)
                    {
                        newSubgroupCount.GroupCount = sourcSubgroupCount.GroupCount;
                    }
                    
                    _db.SectionFKSubgroupCounts.Add(newSubgroupCount);
                }
                else
                {
                    if (sourcSubgroupCount != null && copyCount)
                    {
                        sectionFkSubgroupCount.GroupCount = sourcSubgroupCount.GroupCount;
                    }
                }
            }
            _db.SaveChanges();
        }

        public ActionResult CopyProperties(int src, int dst, bool limits, bool places, bool teachers)
        {
            var commonSections =
                _db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == src)
                    .Select(p => p.SectionFk)
                    .Intersect(
                        _db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == dst)
                            .Select(p => p.SectionFk))
                    .Select(s => s.ModuleId)
                    .ToList();
            foreach (var sectionId in commonSections)
            {
                var s = _db.SectionFKProperties
                    .Include(x => x.TrainingPlaces)
                    .Include(x => x.Teachers)
                    .Single(p => (p.SectionFKCompetitionGroupId == src) && (p.SectionFKId == sectionId));
                var d = _db.SectionFKProperties
                    .Include(x => x.TrainingPlaces)
                    .Include(x => x.Teachers)
                    .Single(p => (p.SectionFKCompetitionGroupId == dst) && (p.SectionFKId == sectionId));

                if (limits)
                    d.Limit = s.Limit;

                if (places)
                    foreach (var place in s.TrainingPlaces)
                        if (!d.TrainingPlaces.Any(x => x.Id == place.Id))
                            d.TrainingPlaces.Add(place);

                if (teachers)
                    foreach (var t in s.Teachers)
                        if (!d.Teachers.Any(x => x.pkey == t.pkey))
                            d.Teachers.Add(t);
            }
            _db.SaveChanges();
            return RedirectToAction("Properties", "SectionFK", new { competitionGroupId = dst });
        }

        public ActionResult GetYears()
        {
            var years = _db.SectionFKCompetitionGroups.Select(g => new {Year = g.Year}).Distinct();

            var yearsjson = Json(
                new
                {
                    data = years
                },
                new JsonSerializerSettings()
            );
            return yearsjson;
        }

        [HttpGet]
        public ActionResult CopyGroups(int src, int dst)
        {
            var srcCG = _db.SectionFKCompetitionGroups.Include(_ => _.Groups).FirstOrDefault(_ => _.Id == src);
            var dstCG = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == dst);

            if (srcCG== null || dstCG== null)
                 return NotFound("Не найдена конкурсная группа");

            if(dstCG.Groups.Any())
                {
                    Response.StatusCode = (Int32)HttpStatusCode.InternalServerError;
                    return Content("Состав одной из КГ должен быть пустым!");
                }

            if (srcCG.ShortName.Equals(dstCG.ShortName) && srcCG.SemesterId != dstCG.SemesterId)
            {
                var groups = srcCG.Groups.Where(g => !_db.SectionFKCompetitionGroups.Any(c =>
                    c.Year == dstCG.Year && c.SemesterId == dstCG.SemesterId &&
                    c.Groups.Any(_ => _.Id == g.Id)));

                var includedinAnotherCG = srcCG.Groups.Except(groups)
                    .Select(exg =>
                        new
                        {
                            group = exg.Name,
                            competitionGroupName = _db.SectionFKCompetitionGroups.FirstOrDefault(c =>
                                c.Year == dstCG.Year && c.SemesterId == dstCG.SemesterId &&
                                c.Groups.Any(g => g.Id == exg.Id))?.Name
                        }).GroupBy(g => g.competitionGroupName)
                    .Select(g => new
                    {
                        competitionGroupName = g.Key,
                        groups = g.Select(gr => gr.group)
                    }).ToList();

                List<string> notGroupHistory = new List<string>();
                bool newcourse = srcCG.Year != dstCG.Year && Math.Abs(srcCG.StudentCourse - dstCG.StudentCourse) == 1;

                if (newcourse || srcCG.Year == dstCG.Year && srcCG.StudentCourse == dstCG.StudentCourse)
                {
                    if (newcourse)
                    {
                        groups = groups.Where(g =>
                            _db.GroupsHistories.Any(_ =>
                                _.GroupId == g.Id && _.YearHistory == dstCG.Year && _.Course == dstCG.StudentCourse));
                        notGroupHistory = srcCG.Groups.Except(groups).Select(g => g.Name).ToList();
                    }

                    foreach (var g in groups)
                    {
                        dstCG.Groups.Add(g);
                        Logger.Info("скопирован {0} {1} в {2} ИЯ", g.Name, g.Id, dstCG.Name);
                    }

                    _db.SaveChanges();

                    var genericResult = new
                    {
                        message1 = "Группы включены в другие конкурсные группы",
                        includedinAnotherCG,
                        message2 = "Не найдены исторические группы, обратитесь в тех. поддержку!",
                        notGroupHistory
                    };

                    return Json(genericResult,
                        new JsonSerializerSettings());
                }

                Response.StatusCode = (Int32) HttpStatusCode.InternalServerError;
                return Content("Неверно выбран курс!");
            }

            Response.StatusCode = (Int32)HttpStatusCode.InternalServerError;
            return Content("Параметры конкурсных групп заданы неверно");
        }

        #region Edit

        public ActionResult Edit(string id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var module = _db.UniModules().Include("SectionFk.Periods").First(m => m.uuid == id);

            if (module == null)
                return NotFound();

            var techSelector = new SelectList(_db.ModuleTeches, "Id", "Name", module.SectionFk?.ModuleTechId);

            var semesters = _db.Semesters.ToList();
            var semesterSelector = new SelectList(semesters, "Id", "Name");
            var males = new List<object>
            {
                new {Id = "", Name = "Все"},
                new {Id = false, Name = "Женский"},
                new {Id = true, Name = "Мужской"}
            };
            var courses = new List<object>
            {
                new {Id = "", Name = "Все"},
                new {Id = 1, Name = "1"},
                new {Id = 2, Name = "2"},
                new {Id = 3, Name = "3"}
            };
            var model = new SectionFKEditViewModel
            {
                Module = module,
                moduleUUId = module.SectionFk == null ? string.Empty : module.uuid,
                showInLc = module.SectionFk?.ShowInLC ?? false,
                withoutPriorities = module.SectionFk?.WithoutPriorities ?? false,
                techid = module.SectionFk?.ModuleTechId.ToString() ?? string.Empty,
                tech = module.SectionFk?.Tech.Name ?? string.Empty,
                TechSelector = techSelector,
                SemesterSelector = semesterSelector
            };

            if (module.SectionFk == null)
                model.periods = new List<SectionFKPeriodEditViewModel>();
            else
                model.periods = module.SectionFk.Periods.Select(p =>
                        new SectionFKPeriodEditViewModel
                        {
                            id = p.Id,
                            year = p.Year,
                            semesterId = p.SemesterId,
                            semesterName = p.Semester.Name,
                            Selector = new SelectList(semesters, "Id", "Name", p.SemesterId),
                            MaleSelector = new SelectList(males, "Id", "Name", p.Male),
                            CourseSelector = new SelectList(courses, "Id", "Name", p.Course),
                            selectionDeadline = p.SelectionDeadline,
                            SelectionBegin = p.SelectionBegin,
                            Course = p.Course
                        }
                ).ToList();

            ViewBag.CanEdit = User.IsInRole(ItsRoles.SectionFKManager);
            ViewBag.CanLimitEdit = User.IsInRole(ItsRoles.SectionFKManager);
            ViewBag.Messages = TempData["Messages"];

            return View(model);
        }

        // TODO: amir Roles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SectionFKEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sectionFK = new SectionFK
                {
                    ModuleId = model.Module.uuid,
                    Periods = new List<SectionFKPeriod>()
                };

                List<string> ErrorMessages =  new List<string>();
                if (User.IsInRole(ItsRoles.SectionFKManager))
                {
                    Logger.Info("Редактирование секции: {0}", JsonConvert.SerializeObject(model));
                    foreach (var p in model.periods)
                    {
                        if (p.id < 0)
                            p.id = 0;

                        //добавили и тут же удалили
                        if (p.isDeleted && (p.id == 0)) continue;

                        var period = new SectionFKPeriod
                        {
                            Id = p.id,
                            SectionFKId = sectionFK.ModuleId,
                            Year = p.year,
                            SemesterId = p.semesterId,
                            SelectionDeadline = p.selectionDeadline,
                            SelectionBegin = p.SelectionBegin,
                            Course = p.Course,
                            Male = p.Male
                        };
                        
                        if (p.isDeleted)
                        {
                            if (_db.SectionFKTmerPeriods.Any(tp => tp.SectionFKPeriodId == p.id))
                            {
                                ErrorMessages.Add($"Невозможно удалить период: {p.year} {_db.Semesters.Single(s => s.Id == p.semesterId).Name} Курс: {(p.Course == null ? "Все" : p.Course.ToString())}. Период задан на дисциплине"); 
                                continue;
                            }
                                
                            _db.Entry(period).State = EntityState.Deleted;
                        }
                        else 
                        {
                            if (p.id == 0 && sectionFK.Periods.Any(_=> _.Year == period.Year && _.SemesterId == period.SemesterId && (_.Course == null && period.Course!= null || _.Course != null && period.Course == null)))
                            {
                                ErrorMessages.Add($"Невозможно задать период: {p.year} {_db.Semesters.Single(s => s.Id == p.semesterId).Name} Курс: {(p.Course == null ? "Все" : p.Course.ToString())}.Курс задан неверно");
                                continue;
                            }
                            sectionFK.Periods.Add(period);
                            _db.Entry(period).State = p.id == 0 ? EntityState.Added : EntityState.Modified;
                        }
                    }
                    sectionFK.ShowInLC = model.showInLc;
                    sectionFK.WithoutPriorities = model.withoutPriorities;
                    sectionFK.ModuleTechId = int.Parse(model.techid);

                    if (string.IsNullOrEmpty(model.moduleUUId))
                        _db.Entry(sectionFK).State = EntityState.Added;
                    else
                        _db.Entry(sectionFK).State = EntityState.Modified;
                }
                
                _db.SaveChanges();
                if (ErrorMessages.Any())
                {
                    TempData["Messages"] = ErrorMessages;
                    return RedirectToAction("Edit", new { id = model.Module.uuid});
                }
                  
                return RedirectToAction("Index", new { focus = model.moduleUUId });
            }

            //по новой будем делать форму с ошибками, нужны справочники
            var techSelector = new SelectList(_db.ModuleTeches, "Id", "Name", model.techid);

            var semesters = _db.Semesters.ToList();
            var semesterSelector = new SelectList(semesters, "Id", "Name");
            var males = new List<object>
            {
                new {Id = "", Name = "Все"},
                new {Id = false, Name = "Женский"},
                new {Id = true, Name = "Мужской"}
            };
            var courses = new List<object>
            {
                new {Id = "", Name = "Все"},
                new {Id = 1, Name = "1"},
                new {Id = 2, Name = "2"},
                new {Id = 3, Name = "3"}
            };
            model.TechSelector = techSelector;
            model.SemesterSelector = semesterSelector;

            foreach (var p in model.periods)
            {
                p.Selector = new SelectList(semesters, "Id", "Name");
                p.MaleSelector = new SelectList(males, "Id", "Name", p.Male);
                p.CourseSelector = new SelectList(courses, "Id", "Name", p.Course);
            }

            return View(model);
        }

        public ActionResult Disciplines(string moduleId)
        {
            var module = _db.UniModules().Include("SectionFK.Disciplines").Where(m => m.uuid == moduleId).Single();

            ViewBag.Title = string.Format(@"Дисциплины для секции ФК ""{0}""", module.title);
            ViewBag.SectionFKId = moduleId;
            ViewBag.CanEdit = User.IsInRole(ItsRoles.SectionFKManager);

            var model = new List<SectionFKDisciplineViewModel>(module.disciplines.Count);
            foreach (var d in module.disciplines)
            {
                var r = new SectionFKDisciplineViewModel
                {
                    Discipline = d,
                    SectionFKDiscipline = module.SectionFk.Disciplines.FirstOrDefault(f => f.DisciplineUid == d.uid)
                };

                model.Add(r);
            }

            return View(model);
        }

        [Authorize(Roles = ItsRoles.SectionFKManager)]
        public ActionResult EditTmers(string sectionFKId, string disciplineId)
        {
            var keys = new List<int?>(3) { 1, 2, 3 };
            var tmers = _db.Tmers.Where(t => keys.Contains(t.kgmer)
                                             && (t.kmer != "U039")
                                             && (t.kmer != "U032")
                                             && (t.kmer != "U033"))
                .ToList();

            var sectionFK = _db.SectionFKs.Include("Disciplines.Tmers").Where(m => m.ModuleId == sectionFKId).Single();
            var discipline = _db.Disciplines.Where(d => d.uid == disciplineId).Single();

            var model = new SectionFKTmersViewModel(sectionFK, discipline, tmers);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.SectionFKManager)]
        public ActionResult EditTmers(SectionFKTmersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sectionFK =
                    _db.SectionFKs.Include("Disciplines.Tmers")
                        .Where(m => m.ModuleId == model.SectionFK.ModuleId)
                        .Single();
                var sectionFKDiscipline =
                    sectionFK.Disciplines.FirstOrDefault(d => d.DisciplineUid == model.Discipline.uid);
                if (sectionFKDiscipline == null)
                {
                    sectionFKDiscipline = new SectionFKDiscipline
                    {
                        DisciplineUid = model.Discipline.uid,
                        SectionFKId = sectionFK.ModuleId,
                        Tmers = new List<SectionFKDisciplineTmer>()
                    };
                    sectionFK.Disciplines.Add(sectionFKDiscipline);
                }

                var tmers = new List<SectionFKTmersRowViewModel>();
                tmers.AddRange(model.Tmers1);
                tmers.AddRange(model.Tmers2);
                tmers.AddRange(model.Tmers3);

                foreach (var t in tmers)
                {
                    var dt = sectionFKDiscipline.Tmers.FirstOrDefault(f => f.TmerId == t.TmerId);
                    if ((dt == null) && t.Checked)
                    {
                        dt = new SectionFKDisciplineTmer { TmerId = t.TmerId };
                        sectionFKDiscipline.Tmers.Add(dt);
                        //db.Entry(dt).State = EntityState.Added;
                    }
                    if ((dt != null) && !t.Checked)
                    {
                        sectionFKDiscipline.Tmers.Remove(dt);
                        _db.Entry(dt).State = EntityState.Deleted;
                    }
                }

                _db.SaveChanges();

                return RedirectToAction("Tmers", new { id = sectionFKDiscipline.Id });
            }
            return RedirectToAction("Disciplines", new { moduleId = model.SectionFK.ModuleId });
        }

        public ActionResult Tmers(int id, string message)
        {
            ViewBag.Message = message;
            var divisions = _db.Divisions.ToDictionary(d => d.uuid);
            CreateTree(divisions);

            var discipline =
                _db.SectionFKDisciplines.Include("Tmers.Periods")
                    .Include("SectionFK")
                    .Include("Discipline")
                    .Where(d => d.Id == id)
                    .Single();

            ViewData.Add("Tmers1", discipline.Tmers.Where(t => t.Tmer.kgmer == 1).OrderBy(t => t.Tmer.kmer).ToList());
            ViewData.Add("Tmers2", discipline.Tmers.Where(t => t.Tmer.kgmer == 2).OrderBy(t => t.Tmer.kmer).ToList());
            ViewData.Add("Tmers3", discipline.Tmers.Where(t => t.Tmer.kgmer == 3).OrderBy(t => t.Tmer.kmer).ToList());

            ViewBag.CanEdit = User.IsInRole(ItsRoles.SectionFKManager);

            return View(discipline);
        }

        private void CreateTree(Dictionary<string, Division> divisions)
        {
            foreach (var d in divisions.Values)
            {
                Division parent;
                if ((d.parent != null) && divisions.TryGetValue(d.parent, out parent))
                    d.ParentDivision = parent;
            }
        }

        //Get: Редактирование периодов тут нагрузки уже заданы
        [Authorize(Roles = ItsRoles.SectionFKManager)]
        public ActionResult EditPeriods(string sectionFKId, string disciplineId)
        {
            var sectionFK = _db.SectionFKs.Include(m => m.Periods).Where(m => m.ModuleId == sectionFKId).Single();
            var discipline =
                _db.SectionFKDisciplines.Include("Tmers.Periods")
                    .Where(d => (d.SectionFKId == sectionFKId) && (d.DisciplineUid == disciplineId))
                    .Single();

            var model = new SectionFKTmersPeriodViewModel(sectionFK, discipline);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.SectionFKManager)]
        public ActionResult EditPeriods(SectionFKTmersPeriodViewModel model)
        {
            if (ModelState.IsValid)
            {

                var sectionFKDisciplines =
                    _db.SectionFKDisciplines.Include("Tmers.Periods").Where(d=>d.SectionFKId==model.SectionFK.ModuleId);
                var currentDiscipline = sectionFKDisciplines.Single(d=>d.Id== model.Discipline.Id);
                bool existsperiod = false;

                foreach (var t in model.Rows)
                {
                    var dt = currentDiscipline.Tmers.FirstOrDefault(f => f.Id == t.Tmer.Id);
                    if (dt != null)
                    {
                        var tp = dt.Periods.FirstOrDefault(f => f.SectionFKPeriodId == t.Period.Id);
                        if ((tp == null) && t.Checked)
                        {
                            tp = new SectionFKDisciplineTmerPeriod
                            {
                                SectionFKDisciplineTmerId = t.Tmer.Id,
                                SectionFKPeriodId = t.Period.Id
                            };

                            var period = _db.SectionFKPeriods.FirstOrDefault(p=>p.Id == t.Period.Id);
                            sectionFKDisciplines.Where(d => d.Id != model.Discipline.Id).ToList().ForEach(d =>
                            {
                                foreach (var tmer in d.Tmers)
                                {
                                    if  (
                                        tmer.Periods.Any(p=> p.Tmer.TmerId == dt.TmerId && (p.SectionFKPeriodId == tp.SectionFKPeriodId || period!= null && p.Period.Year== period.Year && p.Period.SemesterId == period.SemesterId &&(p.Period.Course==null || period.Course==null) ) ))
                                        existsperiod = true;
                                }
                             });
                             if(existsperiod)
                                 return RedirectToAction("Tmers",
                                     new
                                     {
                                         id = currentDiscipline.Id,
                                         message = "Невозможно установить период. Период задан для другой дисциплины"
                                     });
                            dt.Periods.Add(tp);
                            //db.Entry(dt).State = EntityState.Added;
                        }
                        if ((tp != null) && !t.Checked)
                        {
                            if (tp.SectionFKSubgroupCounts.Any(c => c.Subgroups.Any()))
                                return RedirectToAction("Tmers",
                                    new
                                    {
                                        id = currentDiscipline.Id,
                                        message = "Невозможно удалить периоды, т.к. на них есть подгруппы"
                                    });

                            if (tp.SectionFKSubgroupCounts.Any())
                                tp.SectionFKSubgroupCounts.Clear();

                            dt.Periods.Remove(tp);
                            _db.Entry(tp).State = EntityState.Deleted;
                        }
                    }
                }

                _db.SaveChanges();

                return RedirectToAction("Tmers", new { id = currentDiscipline.Id });
            }
            return RedirectToAction("Disciplines", new { moduleId = model.SectionFK.ModuleId });
        }

        //Get: Редактирование кафедр
        [Authorize(Roles = ItsRoles.SectionFKManager)]
        public ActionResult EditDivisions(int sectionFKDisciplineTmerPeriodId)
        {
            var period = _db.SectionFKTmerPeriods.Where(t => t.Id == sectionFKDisciplineTmerPeriodId).Single();

            var divisions = _db.Divisions.ToList();

            var model = new MinorDivisionViewModel(divisions, period, period.Tmer.SectionFKDisciplineId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.SectionFKManager)]
        public ActionResult EditDivisions(MinorDivisionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var period =
                    _db.SectionFKTmerPeriods.Include(p => p.Divisions)
                        .Include(p => p.Tmer)
                        .Where(p => p.Id == model.PeriodId)
                        .Single();

                var rows = model.GetAllRows();
                foreach (var d in rows)
                {
                    var pd = period.Divisions.FirstOrDefault(f => f.uuid == d.DivisionID);

                    if ((pd == null) && d.Selected)
                    {
                        var division = new Division { uuid = d.DivisionID };
                        period.Divisions.Add(division);
                        _db.Entry(division).State = EntityState.Unchanged;
                    }
                    if ((pd != null) && !d.Selected)
                        period.Divisions.Remove(pd);
                }

                _db.SaveChanges();

                return RedirectToAction("Tmers", new { id = period.Tmer.SectionFKDisciplineId });
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.SectionFKManager)]
        public ActionResult SetDeadlines(string modules, string deadlines)
        {
            List<SectionFKPeriod> newperiods = JsonConvert.DeserializeObject<List<SectionFKPeriod>>(deadlines);

            var selectedmodules = JsonConvert.DeserializeObject<List<string>>(modules);

            var selectedFks = _db.SectionFKs.Where(s => selectedmodules.Contains(s.ModuleId)).ToList();

            string excludedsection = System.String.Empty;

            foreach (var section in selectedFks)
                {
                    foreach (var period in newperiods)
                    {
                        if (section.Periods.Any(p =>
                            p.Year == period.Year && p.SemesterId == period.SemesterId &&
                            p.Course == period.Course))
                        {
                            excludedsection += section.Module.title + "  Год:" + period.Year + " Курс " + period.Course +
                                           " Семестр  " + period.SemesterId + Environment.NewLine;
                            continue;
                        }
                        
                        var newperiod = new SectionFKPeriod
                        {
                            SectionFKId = section.ModuleId,
                            Year = period.Year,
                            SemesterId = period.SemesterId,
                            SelectionDeadline = period.SelectionDeadline,
                            SelectionBegin = period.SelectionBegin,
                            Course = period.Course,
                        };
                        _db.SectionFKPeriods.Add(newperiod);
                    }
                }
                _db.SaveChanges();
                string message = string.IsNullOrEmpty(excludedsection) ? null : excludedsection;
                return JsonNet(message);
        }


        #endregion
    }

    public class BadGroups
    {
        public string Group { get; set; }
        public string GroupId { get; set; }
        public string CompetitionGroupName { get; set; }
    }

    public class AutoMoveData
    {
        private readonly List<string> _modules;
        private readonly ApplicationDbContext _db;

        public AutoMoveData(List<SectionFKCompetitionGroup> cg, List<string> modules, ApplicationDbContext db)
        {
            _modules = modules;
            _db = db;
            CompetitionGroups = cg.Select(InitCompGroup).ToList();
        }

        private AutoMoveCG InitCompGroup(SectionFKCompetitionGroup cg)
        {
            return new AutoMoveCG(cg, _modules, _db);
        }

        public List<AutoMoveCG> CompetitionGroups { get; set; }

        public FileContentResult MassPrint()
        {

            using (var zipArchiveStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var group in CompetitionGroups)
                    {
                        using (var reportStream = group.PrepareReportStream())
                        {
                            var entry =
                                zipArchive.CreateEntry((group.Description.NoLonger(150) + ".xlsx").CleanFileName(), CompressionLevel.Fastest);
                            using (var zipEntryStream = entry.Open())
                            {
                                reportStream.Position = 0;
                                reportStream.CopyTo(zipEntryStream);
                            }
                        }
                        using (var reportStream = group.PrepareUngrantedStream())
                        {
                            var entry =
                                zipArchive.CreateEntry(("Не переведённые " + group.Description.NoLonger(150) + ".xlsx").CleanFileName(), CompressionLevel.Fastest);
                            using (var zipEntryStream = entry.Open())
                            {
                                reportStream.Position = 0;
                                reportStream.CopyTo(zipEntryStream);
                            }
                        }
                    }

                }

                zipArchiveStream.Position = 0;

                return new FileContentResult(zipArchiveStream.ToArray(), "application/zip")
                {
                    FileDownloadName = "Автоматический перевод.zip".ToDownloadFileName()
                };
            }
        }

        public void Execute()
        {
            var result = MassPrint();
            _db.SectionFKAutoMoveReports.Add(new SectionFKAutoMoveReport
            {
                Date = DateTime.Now,
                Content = result.FileContents,
                FileName = result.FileDownloadName
            });

            foreach (var competitionGroup in CompetitionGroups)
            {
                Logger.Info("Запись автопереводов для " + competitionGroup.Description);
                competitionGroup.Execute();
            }

            foreach (var competitionGroup in CompetitionGroups)
            {
                Logger.Info("Запись отказов для " + competitionGroup.Description);
                competitionGroup.ExecutePostUngranted();
            }
            _db.SaveChanges();
        }
    }

    public class AutoMoveCG
    {
        private SectionFKCompetitionGroup cg;
        private ApplicationDbContext _db;
        private List<string> _modules;

        public SectionFKCompetitionGroup Cg
        {
            get { return cg; }
        }

        public AutoMoveCG(SectionFKCompetitionGroup cg, List<string> modules, ApplicationDbContext db)
        {
            this.cg = cg;
            this._modules = modules;
            this._db = db;
            Sections =
                db.SectionFKProperties
                .Where(p => p.SectionFKCompetitionGroupId == cg.Id)
                .Include(p => p.SectionFk.Module)
                .ToList()
                .Select(prop => new AutoMoveSection(prop, db, this))
                .ToList();
            
            var wishes = db.SectionFKStudentSelectionPriorities
                .Where(p => p.changePriority > 0
                    && p.competitionGroupId == cg.Id 
                    && modules.Contains(p.sectionId) 
                    && !p.Student.Sportsman)
                .OrderBy(p => p.modified)
                .Include(w => w.Student.Group)
                .Include(w => w.Student.Person)
                .Include(w => w.Section.Module)
                .ToList();

            var granted = new HashSet<SectionFKStudentSelectionPriority>();

            bool somethingWasChanged;
            do
            {
                somethingWasChanged = false;
                foreach (var wish in wishes)
                {
                    if (!String.IsNullOrEmpty(wish.Student.sectionFKDebtTerms))
                        continue;
                    if (granted.Contains(wish))
                        continue;
                    var acceptor = Sections.FirstOrDefault(s => s.Prop.SectionFKId == wish.sectionId && !s.Full);
                    if (acceptor != null)
                    {
                        granted.Add(wish);
                        somethingWasChanged = true;

                        var donator = Sections.FirstOrDefault(s => s.HasStudent(wish.studentId));
                        if (donator == null)
                        {
                            acceptor.Appended.Add(db.Students.Find(wish.studentId));
                        }
                        else
                        {
                            donator.Transfer(wish.studentId, acceptor);
                        }
                    }
                }
            } while (somethingWasChanged);

            Ungranted = wishes.Where(w => !granted.Contains(w) && !WishIsPointless(w)).ToList();
        }

        private bool WishIsPointless(SectionFKStudentSelectionPriority w)
        {
            if (From.ContainsKey(w.Student))
                return w.sectionId == From[w.Student].Prop.SectionFKId;
            return false;
        }

        public List<SectionFKStudentSelectionPriority> Ungranted { get; set; }
        public List<AutoMoveSection> Sections { get; set; }
        public Dictionary<Student, AutoMoveSection> From { get; set; } = new Dictionary<Student, AutoMoveSection>();
        public Dictionary<Student, AutoMoveSection> To { get; set; } = new Dictionary<Student, AutoMoveSection>();

        public string Description => $"Семестр: {Cg.Semester.Name} Год: {Cg.Year} Конкурсная группа: {Cg.ShortName} Курс: {Cg.StudentCourse}";

        public string LookupFrom(Student student)
        {
            AutoMoveSection value;
            if (From.TryGetValue(student, out value))
                return value.Prop.SectionFk.Module.shortTitle;
            return "";
        }

        public string LookupTo(Student student)
        {
            AutoMoveSection value;
            if (To.TryGetValue(student, out value))
                return value.Prop.SectionFk.Module.shortTitle;
            return "";
        }

        public Stream PrepareReportStream()
        {
            var stream = new VariantExport().Export(new { Rows = To.Keys.Select(s => new { Student = s, From = LookupFrom(s), To = LookupTo(s) }), FromName = "Было", ToName = "Стало" },
                "sectionFkAutoMoveReportTemplate.xlsx");
            return stream;
        }


        public Stream PrepareUngrantedStream()
        {
            var stream = new VariantExport().Export(new { Rows = Ungranted.Select(s => new { Student = s.Student, From = LookupFrom(s.Student), To = s.Section.Module.shortTitle }), FromName = "Сейчас в", ToName = "Хотел в" },
                "sectionFkAutoMoveReportTemplate.xlsx");
            return stream;
        }

        public void Execute()
        {
            foreach (var section in Sections)
            {
                section.Execute();
            }
        }

        public void ExecutePostUngranted()
        {
            //отправляем статусы "не зачислен" уже после отправки зачисленных, чтобы избежать конфликтов, если в системе вдруг будут множественные приоритеты по каким-то причинам
            foreach (var wish in Ungranted)
            {
                var nAdmission =
                    _db.SectionFKAdmissions.FirstOrDefault(
                        s =>
                            s.studentId == wish.studentId &&
                            s.SectionFKCompetitionGroupId == wish.competitionGroupId &&
                            s.SectionFKId == wish.sectionId);

                if (nAdmission == null)
                {
                    nAdmission = new SectionFKAdmission
                    {
                        studentId = wish.studentId,
                        SectionFKId = wish.sectionId,
                        SectionFKCompetitionGroupId = wish.competitionGroupId,
                        Status = AdmissionStatus.Denied,
                        Published = false
                    };
                    _db.SectionFKAdmissions.Add(nAdmission);
                }
            }
        }
    }

    public class AutoMoveSection
    {
        private readonly SectionFKProperty _prop;
        private readonly ApplicationDbContext _db;
        private readonly AutoMoveCG _owner;

        public SectionFKProperty Prop
        {
            get { return _prop; }
        }

        public List<Student> Students { get; set; }
        public AutoMoveSection(SectionFKProperty prop, ApplicationDbContext db, AutoMoveCG owner)
        {
            _prop = prop;
            _db = db;
            _owner = owner;
            Students = db.SectionFKAdmissions.Where(
                a =>
                    a.Status == AdmissionStatus.Admitted &&
                    a.SectionFKCompetitionGroupId == prop.SectionFKCompetitionGroupId &&
                    a.SectionFKId == prop.SectionFKId && !a.Student.Sportsman).Select(s => s.Student).Include(s => s.Person).Include(s => s.Group).ToList();

            foreach (var student in Students)
            {
                _owner.From[student] = this;
            }
        }

        public List<Student> Appended { get; set; } = new List<Student>();
        public List<Student> Removed { get; set; } = new List<Student>();

        public bool Full
        {
            get { return Students.Count + Appended.Count - Removed.Count >= _prop.Limit; }
        }

        public bool HasStudent(string sid)
        {
            return (Students.Any(s => s.Id == sid) || Appended.Any(s => s.Id == sid)) && Removed.All(s => s.Id != sid);
        }

        public void Transfer(string studentId, AutoMoveSection acceptor)
        {
            if (acceptor._prop.SectionFKId == this._prop.SectionFKId)
                return;

            var student = Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
            {
                student = Appended.First(s => s.Id == studentId);
                Appended.Remove(student);
            }
            else
            {
                Removed.Add(student);
                _owner.From[student] = this;
            }

            _owner.To[student] = acceptor;

            acceptor.Appended.Add(student);
        }

        public bool Display { get { return Appended.Any() || Removed.Any(); } }

        public void Execute()
        {
            foreach (var student in Appended)
            {
                var currentAdmissions = _db.SectionFKAdmissions.Where(
                        s => s.studentId == student.Id && s.SectionFKCompetitionGroupId == _prop.SectionFKCompetitionGroupId)
                    .ToList();
                foreach (var ca in currentAdmissions)
                {
                    if (ca.SectionFKId == _prop.SectionFKId)
                        continue;
                    ca.Status = AdmissionStatus.Denied;
                    ca.Published = false;
                }

                var subgroupMemberships = _db.SectionFKSubgroupMemberships.Where(s => s.studentId == student.Id && s.Subgroup.Meta.CompetitionGroupId == _prop.SectionFKCompetitionGroupId);
                _db.SectionFKSubgroupMemberships.RemoveRange(subgroupMemberships);

                var nAdmission =
                    _db.SectionFKAdmissions.FirstOrDefault(
                        s =>
                            s.studentId == student.Id &&
                            s.SectionFKCompetitionGroupId == _prop.SectionFKCompetitionGroupId &&
                            s.SectionFKId == _prop.SectionFKId);
                if (nAdmission == null)
                {
                    nAdmission = new SectionFKAdmission
                    {
                        studentId = student.Id,
                        SectionFKId = _prop.SectionFKId,
                        SectionFKCompetitionGroupId = _prop.SectionFKCompetitionGroupId,
                        Status = AdmissionStatus.Admitted,
                        Published = false
                    };
                    _db.SectionFKAdmissions.Add(nAdmission);
                }
                else
                {
                    nAdmission.Status = AdmissionStatus.Admitted;
                    nAdmission.Published = false;
                }
            }
        }
    }
}