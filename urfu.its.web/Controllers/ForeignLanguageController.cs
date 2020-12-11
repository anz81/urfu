using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
//using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using PagedList.Core;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Model.Models.ModulesVM;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.ForeignLanguageManager)]
    public class ForeignLanguageController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: ForeignLanguage
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


        public ActionResult DownloadForeignLanguageReport(string filter)
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
                "Отчёт модулям ИЯ.xlsx".ToDownloadFileName());
        }

        private IQueryable<Module> FilterPeriod(FilterRule filterYear, FilterRule filterSemester)
        {
            int year;
            int semester;

            var haveYear = int.TryParse(filterYear?.Value, out year);
            var haveSemester = int.TryParse(filterSemester?.Value, out semester);

            if (haveYear && haveSemester)
                return
                    ForeignLanguageForUser()
                        .Where(m => m.ForeignLanguage.Periods.Any(p => (p.Year == year) && (p.SemesterId == semester)));

            if (haveYear)
                return ForeignLanguageForUser().Where(m => m.ForeignLanguage.Periods.Any(p => p.Year == year));

            if (haveSemester)
                return ForeignLanguageForUser().Where(m => m.ForeignLanguage.Periods.Any(p => p.SemesterId == semester));

            return ForeignLanguageForUser();
        }

        private IQueryable<Module> ForeignLanguageForUser()
        {
            return db.UniModules().Where(m => m.type == ModuleTypes.ForeignLanguage);
        }

        public ActionResult CompetitionGroups(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var competitionGroups = db.ForeignLanguageCompetitionGroups.Select(c => new
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
                    db.ForeignLanguageAdmissions.Any(
                        a => (a.ForeignLanguageCompetitionGroupId == c.Id) && (a.Status == AdmissionStatus.Admitted)),
                    AdmissionCount =
                    db.ForeignLanguageAdmissions.Count(
                        a => (a.ForeignLanguageCompetitionGroupId == c.Id) && (a.Status == AdmissionStatus.Admitted))
                });


                SortRules sortRules = SortRules.Deserialize(sort);
                competitionGroups = competitionGroups.OrderBy(sortRules.FirstOrDefault());

                competitionGroups = competitionGroups.Where(FilterRules.Deserialize(filter));

                return Json(
                    new
                    {
                        data = competitionGroups
                    },
                    new JsonSerializerSettings()
                );
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrUpdateCompetitionGroup(ForeignLanguageCompetitionGroup competitionGroup)
        {
            if (ModelState.IsValid)
            {
                var competitionGroupFromBase =
                    db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroup.Id);
                if (competitionGroupFromBase == null)
                {
                    db.ForeignLanguageCompetitionGroups.Add(competitionGroup);
                }
                else
                {
                    var hasAdmissions =
                        db.ForeignLanguageAdmissions.Any(
                            _ =>
                                (_.ForeignLanguageCompetitionGroupId == competitionGroup.Id) &&
                                (_.Status == AdmissionStatus.Admitted));
                    if (hasAdmissions)
                    {
                        competitionGroup.StudentCourse = competitionGroupFromBase.StudentCourse;
                    }
                    else if (competitionGroup.StudentCourse != competitionGroupFromBase.StudentCourse)
                    {
                        var admissions =
                            db.ForeignLanguageAdmissions.Where(
                                _ => _.ForeignLanguageCompetitionGroupId == competitionGroup.Id);
                        db.ForeignLanguageAdmissions.RemoveRange(admissions);
                        competitionGroupFromBase.Groups.Clear();
                    }
                    try
                    {
                        db.ForeignLanguageCompetitionGroups.Update(competitionGroup);
                    }
                    catch
                    {
                        db.ForeignLanguageCompetitionGroups.Add(competitionGroup);
                    }
                }
                db.SaveChanges();
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            return NotFound("Validation error");
        }

        public ActionResult CompetitionGroupContentsTree(int? competitionGroupId, int? course, string filter)
        {
            if (competitionGroupId == null) return new StatusCodeResult(StatusCodes.Status404NotFound);

            var competitionGroup = db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
                //Where(t => t.Id == minorDisciplineTmerPeriodId).Single();

            var divisions = db.Divisions.ToList();

            List<GroupRow> groups;
            if (course != null)
                groups =
                    db.GroupsHistories.Where(_ => _.YearHistory == competitionGroup.Year && (_.Course == course) 
                            && (_.Qual != "Магистр" && _.Qual != "Аспирант") && (_.FamType == "Очная")).Where(FilterRules.Deserialize(filter))
                          .Select(_ => new GroupRow()
                          {
                              Id = _.GroupId,
                              Name = _.Name,
                              ChairId = _.ChairId
                          }).ToList();
            else
                groups = db.Groups.Select(_ => new GroupRow()
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
            var competitionGroup = db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            IEnumerable<object> badGroups = Enumerable.Empty<object>();

            if (competitionGroup != null)
            {
                IEnumerable<Group> groups = Enumerable.Empty<Group>();

                if (groupsIds != null)
                {
                    groups =
                        db.Groups.Where(
                                _ =>
                                    groupsIds.Contains(_.Id) &&
                                    (!db.ForeignLanguageCompetitionGroups.Any(
                                        c =>
                                            c.Id != competitionGroupId && c.Year == competitionGroup.Year &&
                                            c.SemesterId == competitionGroup.SemesterId && c.Groups.Any(g => g.Id == _.Id))))
                            .ToList();
                    var badGroupIds = groupsIds.Where(_ => !groups.Any(g => g.Id == _));
                    badGroups = db.Groups.Where(_ => badGroupIds.Contains(_.Id)).Select(_ =>
                        new
                        {
                            competitionGroupName = db.ForeignLanguageCompetitionGroups.FirstOrDefault(
                                c =>
                                    competitionGroup.Year == c.Year && c.SemesterId == competitionGroup.SemesterId &&
                                    c.Groups.Any(g => g.Id == _.Id)).Name,
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
                    foreach (var group  in deletegroups)
                    {
                        competitionGroup.Groups.Remove(group);

                    }
                }
                else if (deselectedGroups != null)
                {
                    var toDelete = competitionGroup.Groups.Where(_ => deselectedGroups.Contains(_.Id)).ToList();
                    var toDeleteIds = toDelete.Select(_ => _.Id);
                    RemoveAdmissionsForGroup(competitionGroupId, toDeleteIds);
                    LogCompetitionGroupChanges("удалена {0} {1} из {2} ИЯ", toDeleteIds, competitionGroup);
                    foreach (var @group in toDelete)
                    {
                        competitionGroup.Groups.Remove(@group);

                    }
                }

                foreach (var group in groups)
                {
                    competitionGroup.Groups.Add(group);
                    Logger.Info("добавлен {0} {1} в {2} ИЯ", group.Name, group.Id, competitionGroup.Name);

                }
                db.SaveChanges();
                return Json(badGroups);
            }
            return NotFound("competition group not found");
        }
        private void LogCompetitionGroupChanges(string msg, IEnumerable<string> groupsIds, ForeignLanguageCompetitionGroup competitionGroup)
        {
            var groups = db.Groups.Where(_ => groupsIds.Contains(_.Id)).ToList();
            foreach (var @group in groups)
            {
                Logger.Info(msg, @group.Name, @group.Name, @group.Id, competitionGroup.Name);
            }
        }
        private void RemoveAdmissionsForGroup(int competitionGroupId, IEnumerable<string> toDeleteIds)
        {
            var admissions =
                db.ForeignLanguageAdmissions.Where(
                    a =>
            a.ForeignLanguageCompetitionGroupId == competitionGroupId &&
            toDeleteIds.Contains(a.Student.GroupId)).ToList();
            var subgroupAdmissions =
                db.ForeignLanguageSubgroupMemberships.Where(
                    m =>
                        m.Subgroup.Meta.CompetitionGroupId == competitionGroupId &&
                        toDeleteIds.Contains(m.Student.GroupId)).ToList();
            db.ForeignLanguageAdmissions.RemoveRange(admissions);
            db.ForeignLanguageSubgroupMemberships.RemoveRange(subgroupAdmissions);
            db.SaveChanges();
        }

        public ActionResult DeleteCompetitionGroup(int id)
        {
            //TODO: amir Надо до делать 
            db.ForeignLanguageProperties.RemoveRange(
                db.ForeignLanguageProperties.Where(
                    x => (x.ForeignLanguageCompetitionGroupId == id) && (x.Limit == 0) && !x.Teachers.Any()));

            db.ForeignLanguageSubgroups.RemoveRange(
                db.ForeignLanguageSubgroups.Where(_ => _.Meta.CompetitionGroupId == id));

            db.ForeignLanguageSubgroupCounts.RemoveRange(
                db.ForeignLanguageSubgroupCounts.Where(_ => _.CompetitionGroupId == id));

            var competitionGroup = db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == id);
            if (competitionGroup != null)
                db.ForeignLanguageCompetitionGroups.Remove(competitionGroup);
            db.SaveChanges();

            Logger.Info($"Удаление конкурсной группы по ИЯ Id: {competitionGroup?.Id} Name: {competitionGroup?.Name}");
            return new StatusCodeResult(StatusCodes.Status200OK);//, $"{nameof(competitionGroup)} deleted");
        }

        public ActionResult Properties(int competitionGroupId)
        {
            var competitionGroup = db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("CompetitionGroup not found");

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var properties = db.ForeignLanguageProperties.Where(
                        _ => _.ForeignLanguageCompetitionGroupId == competitionGroupId &&                       
                        _.ForeignLanguage.Periods.Any(p=> (p.Year == competitionGroup.Year) && (p.SemesterId == competitionGroup.SemesterId)
                            && (p.Course ==null|| p.Course == competitionGroup.StudentCourse)))                 
                    .ToList()
                    .Select(_ => new
                    {
                        _.Id,
                        ForeignLanguageName=_.ForeignLanguage.Module.shortTitle,
                        _.Limit,
                        Teachers= string.Join(", ", _.Teachers.Select(t => t.initials).ToList())
                    });
                return JsonNet(new {data = properties});
  
            }
            var foreignLanguages =
                db.ForeignLanguages.Where(
                        _ =>
                            _.Periods.Any(
                                p => (p.Year == competitionGroup.Year) && (p.SemesterId == competitionGroup.SemesterId) && 
                                (p.Course ==null || p.Course == competitionGroup.StudentCourse)))
                    .ToList();

            foreach (var foreignLanguage in foreignLanguages)
            {
                if (
                    competitionGroup.ForeignLanguageProperties.Any(
                        _ =>
                            (_.ForeignLanguageId == foreignLanguage.ModuleId) &&
                            (_.ForeignLanguageCompetitionGroupId == competitionGroupId)))
                    continue;

                competitionGroup.ForeignLanguageProperties.Add(new ForeignLanguageProperty
                {
                    ForeignLanguageId = foreignLanguage.ModuleId,
                    ForeignLanguageCompetitionGroupId = competitionGroupId
                });
            }
            db.SaveChanges();
            return View(competitionGroup);
        }

        public ActionResult PropertyTeachers(int? page, string sort, string filter, int? limit, int propertyId)
        {
            var property = db.ForeignLanguageProperties.Include("Teachers").FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("property not found");
            var teachers = db.Teachers.Include("ForeignLanguageProperties").Select(t => new
            {
                selected = t.ForeignLanguageProperties.Any(_ => _.Id == propertyId),
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
        public ActionResult UpdateForeignLanguagePropertyTeachers(int propertyId, string teacherRows)
        {
            var property = db.ForeignLanguageProperties.FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");
            var teachers =
                (List<ModulePropertyRow<string>>)
                JsonConvert.DeserializeObject(teacherRows, typeof(List<ModulePropertyRow<string>>));
            foreach (var teacherRow in teachers)
            {
                var teacher = db.Teachers.FirstOrDefault(_ => _.pkey == teacherRow.id);
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
            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult SetForeignLanguagePropertyLimit(int propertyId, int limit)
        {
            var property = db.ForeignLanguageProperties.FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");
            property.Limit = limit;

            db.SaveChanges();

            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult CopyMembershipPrepare(int a, int b)
        {
            var commonSections =
                db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == a)
                    .Select(p => p.ForeignLanguage)
                    .Intersect(
                        db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == b)
                            .Select(p => p.ForeignLanguage))
                    .ToList();

            var exceptSections =
                db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == a)
                    .Select(p => p.ForeignLanguage)
                    .Except(
                        db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == b)
                            .Select(p => p.ForeignLanguage))
                    .ToList();


            var newSections =
                db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == b)
                    .Select(p => p.ForeignLanguage)
                    .Except(
                        db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == a)
                            .Select(p => p.ForeignLanguage))
                    .ToList();

            var aCount =
                db.ForeignLanguageAdmissions.Count(
                    m => (m.ForeignLanguageCompetitionGroupId == a) && (m.Status == AdmissionStatus.Admitted));
            var bCount =
                db.ForeignLanguageAdmissions.Count(
                    m => (m.ForeignLanguageCompetitionGroupId == b) && (m.Status == AdmissionStatus.Admitted));

            var commongroups = db.ForeignLanguageCompetitionGroups.Where(cg => cg.Id == a)
                .SelectMany(g => g.Groups)
                .Intersect(db.ForeignLanguageCompetitionGroups.Where(cg => cg.Id == b).SelectMany(g => g.Groups))
                .ToList();


            var exceptgroups = db.ForeignLanguageCompetitionGroups.Where(cg => cg.Id == a)
                .SelectMany(g => g.Groups)
                .Except(db.ForeignLanguageCompetitionGroups.Where(cg => cg.Id == b).SelectMany(g => g.Groups))
                .ToList();


            var newgroups = db.ForeignLanguageCompetitionGroups.Where(cg => cg.Id == b)
                .SelectMany(g => g.Groups)
                .Except(db.ForeignLanguageCompetitionGroups.Where(cg => cg.Id == a).SelectMany(g => g.Groups))
                .ToList();

            var vm = new ForeignLanguageCompetitionGroupMembershipsCopyVm
            {
                ACount = aCount,
                BCount = bCount,
                A = db.ForeignLanguageCompetitionGroups.Find(a),
                B = db.ForeignLanguageCompetitionGroups.Find(b),
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
                    db.ForeignLanguageAdmissions.Count(
                        m =>
                            (m.ForeignLanguageCompetitionGroupId == a) && (m.Status == AdmissionStatus.Admitted) &&
                            (m.ForeignLanguageId == section.ModuleId));
                vm.BAdmissions[section] =
                    db.ForeignLanguageAdmissions.Count(
                        m =>
                            (m.ForeignLanguageCompetitionGroupId == b) && (m.Status == AdmissionStatus.Admitted) &&
                            (m.ForeignLanguageId == section.ModuleId));
            }

            foreach (var section in newSections)
                vm.BAdmissions[section] =
                    db.ForeignLanguageAdmissions.Count(
                        m =>
                            (m.ForeignLanguageCompetitionGroupId == b) && (m.Status == AdmissionStatus.Admitted) &&
                            (m.ForeignLanguageId == section.ModuleId));

            foreach (var section in exceptSections)
                vm.AAdmissions[section] =
                    db.ForeignLanguageAdmissions.Count(
                        m =>
                            (m.ForeignLanguageCompetitionGroupId == a) && (m.Status == AdmissionStatus.Admitted) &&
                            (m.ForeignLanguageId == section.ModuleId));
            return View(vm);
        }

        [HttpPost]
        public ActionResult CopyMembership(int src, int dst)
        {
            var destination = db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == dst && p.ForeignLanguage.Periods.Any(_ => _.Course == 2 && _.SemesterId == 2)).ToList();

            if (destination.Count()!=0)
            {
                var source = db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == src).ToList();
                
                foreach(var sr in source)
                {
                    foreach(var ds in destination)
                    {
                        int index = ds.ForeignLanguage.Module.title.IndexOf(sr.ForeignLanguage.Module.title);
                        if (index!= -1)
                        {
                            var todelete =
                                    db.ForeignLanguageAdmissions.Where(
                            m => (m.ForeignLanguageCompetitionGroupId == dst) && (m.ForeignLanguageId == ds.ForeignLanguage.ModuleId));
                            db.ForeignLanguageAdmissions.RemoveRange(todelete);

                            var newStudents = db.ForeignLanguageAdmissions.Where(m =>
                        (m.ForeignLanguageCompetitionGroupId == src) &&
                        (m.ForeignLanguageId == sr.ForeignLanguage.ModuleId) &&
                        (m.Status == AdmissionStatus.Admitted) &&
                        m.Student.Group.ForeignLanguageCompetitionGroups.Any(cg => cg.Id == dst) &&
                        (m.Student.Status == "Активный" || m.Student.Status == "Отп.с.посещ." || m.Student.Status == "Отп.дород.послерод."));

                            foreach (var adm in newStudents)
                                db.ForeignLanguageAdmissions.Add(new ForeignLanguageAdmission
                                {
                                    studentId = adm.studentId,
                                    Published = false,
                                    ForeignLanguageCompetitionGroupId = dst,
                                    ForeignLanguageId = ds.ForeignLanguageId,
                                    Status = AdmissionStatus.Admitted
                                });
                        }

                    }

                }
            }
             else
            {

                var commonSections =
               db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == src)
                   .Select(p => p.ForeignLanguage)
                   .Intersect(
                       db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == dst)
                           .Select(p => p.ForeignLanguage))
                   .Select(s => s.ModuleId)
                   .ToList();
                foreach (var sectionId in commonSections)
                {
                    var todelete =
                        db.ForeignLanguageAdmissions.Where(
                            m => (m.ForeignLanguageCompetitionGroupId == dst) && (m.ForeignLanguageId == sectionId));
                   
                    db.ForeignLanguageAdmissions.RemoveRange(todelete);
                    var newStudents = db.ForeignLanguageAdmissions.Where(m =>
                        (m.ForeignLanguageCompetitionGroupId == src) &&
                        (m.ForeignLanguageId == sectionId) &&
                        (m.Status == AdmissionStatus.Admitted) &&
                        m.Student.Group.ForeignLanguageCompetitionGroups.Any(cg => cg.Id == dst) &&
                        (m.Student.Status == "Активный" || m.Student.Status == "Отп.с.посещ." || m.Student.Status == "Отп.дород.послерод."));

                    foreach (var adm in newStudents)
                        db.ForeignLanguageAdmissions.Add(new ForeignLanguageAdmission
                        {
                            studentId = adm.studentId,
                            Published = false,
                            ForeignLanguageCompetitionGroupId = dst,
                            ForeignLanguageId = adm.ForeignLanguageId,
                            Status = AdmissionStatus.Admitted
                        });
                }

            }

           
            db.SaveChanges();
            return RedirectToAction("Index", "ForeignLanguageAdmission", new {competitionGroupId = dst});
        }
    
        public ActionResult CopyProperties(int src, int dst, bool limits, bool teachers)
        {
           var destination = db.ForeignLanguageProperties.Where(p=>p.ForeignLanguageCompetitionGroupId == dst && p.ForeignLanguage.Periods.Any(_ => _.Course == 2 && _.SemesterId == 2)).ToList();

            if (destination.Count() != 0)
            {
                var source = db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == src).ToList();

                if (source.Count() != 0 && source.Count() == destination.Count())
                {
                    foreach (var sr in source)
                    {
                        foreach (var ds in destination)
                        {
                            int index = ds.ForeignLanguage.Module.title.IndexOf(sr.ForeignLanguage.Module.title);
                            if (index != -1)
                            {

                                var s = db.ForeignLanguageProperties
                                       .Include(x => x.Teachers)
                                       .Single(p => (p.ForeignLanguageCompetitionGroupId == src) && (p.ForeignLanguageId == sr.ForeignLanguage.ModuleId));
                                var d = db.ForeignLanguageProperties
                                        .Include(x => x.Teachers)
                                        .Single(p => (p.ForeignLanguageCompetitionGroupId == dst) && (p.ForeignLanguageId == ds.ForeignLanguage.ModuleId));
                                if (limits)
                                    d.Limit = s.Limit;
                                if (teachers)
                                    foreach (var t in s.Teachers)
                                        if (d.Teachers.All(x => x.pkey != t.pkey))
                                            d.Teachers.Add(t);

                            }
                        }

                    }

                }
            }
            else
            {                           
                var commonSections =
                    db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == src)
                        .Select(p => p.ForeignLanguage)
                        .Intersect(
                            db.ForeignLanguageProperties.Where(p => p.ForeignLanguageCompetitionGroupId == dst)
                                .Select(p => p.ForeignLanguage))
                        .Select(s => s.ModuleId)
                        .ToList();

                    foreach (var sectionId in commonSections)
                    {
                        var s = db.ForeignLanguageProperties
                            .Include(x => x.Teachers)
                            .Single(p => (p.ForeignLanguageCompetitionGroupId == src) && (p.ForeignLanguageId == sectionId));
                        var d = db.ForeignLanguageProperties
                            .Include(x => x.Teachers)
                            .Single(p => (p.ForeignLanguageCompetitionGroupId == dst) && (p.ForeignLanguageId == sectionId));

                        if (limits)
                            d.Limit = s.Limit;

                        if (teachers)
                            foreach (var t in s.Teachers)
                                if (d.Teachers.All(x => x.pkey != t.pkey))
                                    d.Teachers.Add(t);
                    }
            }
            
            db.SaveChanges();
            return RedirectToAction("Properties", "ForeignLanguage", new { competitionGroupId = dst });
        }

        
        public ActionResult CopyGroups(int src, int dst)
        {
            var srcCG = db.ForeignLanguageCompetitionGroups.Include(_ => _.Groups).FirstOrDefault(_ => _.Id == src);
            var dstCG = db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == dst);

            if (srcCG == null || dstCG == null)
                return NotFound("Не найдена конкурсная группа");

            if (dstCG.Groups.Any()) { 
                    Response.StatusCode = (Int32)HttpStatusCode.InternalServerError;
                 return Content("Состав одной из КГ должен быть пустым!");
            }

            if (srcCG.ShortName.Equals(dstCG.ShortName) && srcCG.SemesterId != dstCG.SemesterId)
            {
                var groups = srcCG.Groups.Where(g => !db.ForeignLanguageCompetitionGroups.Any(c =>
                    c.Year == dstCG.Year && c.SemesterId == dstCG.SemesterId &&
                    c.Groups.Any(_ => _.Id == g.Id)));

                var includedinAnotherCG = srcCG.Groups.Except(groups)
                    .Select(exg =>
                        new
                        {
                            group = exg.Name,
                            competitionGroupName = db.ForeignLanguageCompetitionGroups.FirstOrDefault(c =>
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
                                    db.GroupsHistories.Any(_ =>
                                        _.GroupId == g.Id && _.YearHistory == dstCG.Year && _.Course == dstCG.StudentCourse));
                        notGroupHistory = srcCG.Groups.Except(groups).Select(g=>g.Name).ToList();
                    }

                    foreach (var g in groups)
                    {
                        dstCG.Groups.Add(g);
                        Logger.Info("скопирован {0} {1} в {2} ИЯ", g.Name, g.Id, dstCG.Name);
                    }

                    db.SaveChanges();

                    var genericResult = new { message1 ="Группы включены в другие конкурсные группы" , includedinAnotherCG,
                        message2 = "Не найдены исторические группы, обратитесь в тех. поддержку!",
                        notGroupHistory
                    };

                    return Json(genericResult,
                        new JsonSerializerSettings());
                }
                  Response.StatusCode = (Int32)HttpStatusCode.InternalServerError;
                    return Content("Неверно выбран курс!");

               }

            Response.StatusCode = (Int32)HttpStatusCode.InternalServerError;
            return Content("Параметры конкурсных групп заданы неверно"); 
        }

        static StudentDesicionList _sdl;
        public ActionResult CopyComposition(int src, int dst, bool subgroups, bool composition, bool subgroupTeachers)
        {
            using (var dbtran = db.Database.BeginTransaction())
            {
                bool success = true;
                var msg = "";

                var srcCG = db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == src);
                var dstCG = db.ForeignLanguageCompetitionGroups.Include(_ => _.Groups).FirstOrDefault(_ => _.Id == dst);
            
                var vm = new ForeignLanguageCompositionCVM(srcCG, dstCG, subgroups, composition);
                
                try
                {
                    _sdl = new StudentDesicionList();

                    
                    CreateSubgroupCounts(src, dst, subgroups);

                    var moduleswithExam = dstCG.SemesterId == 2 && dstCG.StudentCourse == 2;
                    if (moduleswithExam)
                    {
                        var srcModules = db.ForeignLanguageSubgroupCounts.Where(d => d.CompetitionGroupId == src).Include(s => s.ForeignLanguageDisciplineTmerPeriod.Tmer).ToList();
                        var dstModules = db.ForeignLanguageSubgroupCounts.Where(s => s.CompetitionGroupId == dst).Include(d => d.ForeignLanguageDisciplineTmerPeriod.Tmer).ToList();
                        vm.ExceptedSubgroupCounts = srcModules.Except(dstModules, new ForeignLanguageSubgroupCountComparer()).Select(s => s.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.ForeignLanguage.Module.title + "/" + s.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.rmer).ToList();
                    }
                    else
                    {
                        vm.ExceptedSubgroupCounts =
                                db.ForeignLanguageSubgroupCounts.Where(_ => _.CompetitionGroupId == src).Select(s => s.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.ForeignLanguage.Module.title + "/" + s.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.rmer)
                                    .Except(db.ForeignLanguageSubgroupCounts.Where(s => s.CompetitionGroupId == dst).Select(s => s.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.ForeignLanguage.Module.title + "/" + s.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.rmer)).ToList();
                    }

                    foreach (var vmExceptedSubgroupCount in vm.ExceptedSubgroupCounts)
                    {
                        _sdl.AddNew("подгруппы не скопированы", vmExceptedSubgroupCount, "", null);
                    }

                    if (subgroups)
                    {
                        var subgroupController = new ForeignLanguageSubgroupController();

                        if (subgroupController.CreateSubgroups(dst, dstCG))
                        {
                            if (moduleswithExam)
                            {
                                CopyCompositionModulesWithExam(src, dst, composition, subgroupTeachers);

                            }
                            else
                            {
                                var srcSubgroups = db.ForeignLanguageSubgroups.Include(t => t.Meta.ForeignLanguageDisciplineTmerPeriod)
                                                              .Where(s => s.Meta.CompetitionGroupId == src).AsEnumerable();

                                var intersectedSubgroups = srcSubgroups.Intersect(db.ForeignLanguageSubgroups.Where(d => d.Meta.CompetitionGroupId == dst).AsEnumerable(), new ForeignLanguageSubgroupsComparer()).ToList();

                                intersectedSubgroups.ForEach(s =>
                                            {
                                                var dstSubgroup = db.ForeignLanguageSubgroups.FirstOrDefault(d => d.Meta.CompetitionGroupId == dst && d.Name == s.Name);
                                                if (dstSubgroup != null)
                                                {
                                                    if (composition)
                                                        CopyMemberships(s, dstSubgroup, subgroupTeachers);
                                                    else if (subgroupTeachers)
                                                        CopyTeachers(s, dstSubgroup);
                                                }

                                            });

                                var tst = srcSubgroups.Except(intersectedSubgroups).ToList();

                                srcSubgroups.Except(intersectedSubgroups).ToList()
                                                                     .ForEach(s =>
                                                                     {


                                                                         var dstSubgroup = db.ForeignLanguageSubgroups.FirstOrDefault(
                                                                             d => d.Meta.CompetitionGroupId == dst &&
                                                                                  !d.Students.Any() &&
                                                                                  d.Meta.ForeignLanguageDisciplineTmerPeriod
                                                                                      .Tmer.TmerId ==
                                                                                  s.Meta.ForeignLanguageDisciplineTmerPeriod
                                                                                      .Tmer.TmerId &&
                                                                                  s.Meta.ForeignLanguageDisciplineTmerPeriod
                                                                                      .Tmer.Discipline.ForeignLanguageId ==
                                                                                  d.Meta.ForeignLanguageDisciplineTmerPeriod
                                                                                      .Tmer.Discipline.ForeignLanguageId);
                                                                         if (dstSubgroup != null)
                                                                         {
                                                                             if (composition)
                                                                                 CopyMemberships(s, dstSubgroup, subgroupTeachers);
                                                                             else if (subgroupTeachers)
                                                                                 CopyTeachers(s, dstSubgroup);

                                                                         }

                                                                     });
                            }
                            var dstGroupsIds = dstCG.Groups.Select(_ => _.Id).ToList();
                            var exceptedStudentIds = db.ForeignLanguageSubgroupMemberships
                                .Where(adm => adm.Subgroup.Meta.CompetitionGroupId == src
                                              && db.ForeignLanguageSubgroupCounts.Any(sc => sc.CompetitionGroupId == dst
                                                                                        && sc.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguageId == adm.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguageId)
                                              && (adm.Student.Status == "Активный" || adm.Student.Status == "Отп.с.посещ." || adm.Student.Status == "Отп.дород.послерод."))
                                .Select(_ => _.studentId)
                                .Except(
                                        db.ForeignLanguageSubgroupMemberships
                                        .Where(adm => adm.Subgroup.Meta.CompetitionGroupId == dst
                                                      && (adm.Student.Status == "Активный" || adm.Student.Status == "Отп.с.посещ." || adm.Student.Status == "Отп.дород.послерод."))
                                        //.DistinctBy(_ => _.studentId)
                                        .Select(_ => _.studentId))
                                        .Distinct();
                            var exceptedStudents = db.Students.Where(_ => exceptedStudentIds.Contains(_.Id)).ToList();
                            foreach (var exceptedStudent in exceptedStudents)
                            {

                                if (!dstGroupsIds.Contains(exceptedStudent.GroupId))
                                {
                                    _sdl.AddNew("Академическая группа не включена в состав конкурсной группы", "", exceptedStudent.Id, null);
                                }
                                else if (!db.ForeignLanguageAdmissions.Any(
                                        adm => adm.ForeignLanguageCompetitionGroupId == src && adm.studentId == exceptedStudent.Id
                                               && db.ForeignLanguageAdmissions.FirstOrDefault(_ => _.studentId == adm.studentId
                                                                                              &&
                                                                                              _.ForeignLanguageCompetitionGroupId ==
                                                                                              dstCG.Id
                                                                                              &&
                                                                                              _.Status ==
                                                                                              AdmissionStatus.Admitted)
                                                   .ForeignLanguageId == adm.ForeignLanguageId))
                                {
                                    _sdl.AddNew("Студент отчислен или переведен из данной секции", "", exceptedStudent.Id, null);

                                }
                            }
                            _sdl.AddStudentsList(exceptedStudents);

                        }

                    }
                    db.MinorAutoAdmissionReports.Add(new MinorAutoAdmissionReport
                    {
                        Content = _sdl.GetStringBuilder().ToString(),
                        Date = DateTime.Now,
                        ModuleType = ModuleType.ForeignLanguage
                    });

                    db.SaveChanges();
                    dbtran.Commit();
                }
                catch (DbUpdateException updateException)
                {
                    Logger.Info($"Ошибка при копировании cостава подгрупп конкурсной группы ИЯ. Копирование группы {srcCG.Id} {srcCG.Name} в группу {dstCG.Id} {dstCG.Name}");
                    Logger.Error(updateException);
                    msg = "Студент уже зачислен в подгруппу в текущий год и семестр. Если Вы видите эту ошибку, то необходимо оформить запрос в тех. поддержку";
                    success = false;
                }
                catch (Exception e)
                {
                    Logger.Info($"Ошибка при копировании состава подгрупп конкурсной группы ИЯ. Копирование группы {srcCG.Id} {srcCG.Name} в группу {dstCG.Id} {dstCG.Name}");
                    Logger.Error(e);
                    dbtran.Rollback();
                    msg = "Неизвестная ошибка";
                    success = false;
                }
                vm.Message = msg;
                vm.DownloadReport = success;

                return View(vm);
            }
        }

        private void CopyCompositionModulesWithExam(int src, int dst, bool composition,bool subgroupTeachers)
        {
            var dstCG = db.ForeignLanguageCompetitionGroups.Include(_ => _.Groups).FirstOrDefault(_ => _.Id == dst);
            var dstGroupsIds = dstCG.Groups.Select(_ => _.Id).ToList();
            var intersectedSubgroups = db.ForeignLanguageSubgroups.Where(s => s.Meta.CompetitionGroupId == src).AsEnumerable().Intersect(db.ForeignLanguageSubgroups.Where(s => s.Meta.CompetitionGroupId == dst).AsEnumerable(), new ForeignLanguageSubgroupsComparerForModulsWithExam()).ToList();

            intersectedSubgroups.ForEach(s=>
                                {
                                    var dstSubgroup = db.ForeignLanguageSubgroups.FirstOrDefault(ds => ds.Meta.CompetitionGroupId == dst && (ds.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId && ds.Name.Replace(" (экзамен)", string.Empty) == s.Name || (ds.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "prex" && s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "prz" && ds.Name.Replace(" (экзамен)", string.Empty).Replace("экзамен", "зачет") == s.Name)));
                                    if (dstSubgroup != null)
                                    {
                                        if (composition)
                                            CopyMemberships(s, dstSubgroup, subgroupTeachers);
                                        else if (subgroupTeachers)
                                            CopyTeachers(s, dstSubgroup);

                                    }

                                });            

            // Подгруппы, для которых нет подгрупп с таким же названием в новой конкурсной группе, но зачисления из них (новые) должны быть скопированы
            var exeptedSubgroups = db.ForeignLanguageSubgroups.Where(s => s.Meta.CompetitionGroupId == src).ToList().Except(intersectedSubgroups).ToList();

            exeptedSubgroups.ForEach(s=>{

                var dstSubgroup = db.ForeignLanguageSubgroups.FirstOrDefault(ds => ds.Meta.CompetitionGroupId == dst && ds.Name.Contains(s.Name.Substring(0, s.Name.IndexOf("\\"))) && (ds.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId || ds.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "prex" && s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "prz") && ds.Students.Count() == 0);
                if(dstSubgroup!=null)
                {
                    if (composition)
                        CopyMemberships(s, dstSubgroup, subgroupTeachers);
                    else if (subgroupTeachers)
                        CopyTeachers(s, dstSubgroup);

                }
            });           

         
        }

        private void CopyMemberships(ForeignLanguageSubgroup SourceSubgroup, ForeignLanguageSubgroup FinalSubgroup, bool subgroupTeachers)
        {
            var dstGroupsIds = FinalSubgroup.Meta.CompetitionGroup.Groups.Select(_ => _.Id).ToList();
            var subgroupAdmission = new List<ForeignLanguageSubgroupMembership>();
            if (FinalSubgroup.Meta.CompetitionGroup.SemesterId == 2 && FinalSubgroup.Meta.CompetitionGroup.StudentCourse==2)
            {
                subgroupAdmission = db.ForeignLanguageSubgroupMemberships.Where(adm => adm.SubgroupId == SourceSubgroup.Id
                            && adm.Subgroup.Meta.CompetitionGroupId == SourceSubgroup.Meta.CompetitionGroupId
                            && dstGroupsIds.Contains(adm.Student.GroupId)
                            && db.ForeignLanguageAdmissions.FirstOrDefault(_ => _.studentId == adm.studentId
                             && _.ForeignLanguageCompetitionGroupId == FinalSubgroup.Meta.CompetitionGroupId
                             && _.Status == AdmissionStatus.Admitted).ForeignLanguage.Module.title.Contains(adm.Subgroup.Name.Substring(0, SourceSubgroup.Name.IndexOf("\\")))
                             && (adm.Student.Status == "Активный" || adm.Student.Status == "Отп.с.посещ." || adm.Student.Status == "Отп.дород.послерод.")).ToList(); 
            }          
              else  
            subgroupAdmission = db.ForeignLanguageSubgroupMemberships.Where(adm => adm.SubgroupId == SourceSubgroup.Id
                && adm.Subgroup.Meta.CompetitionGroupId == SourceSubgroup.Meta.CompetitionGroupId
                && dstGroupsIds.Contains(adm.Student.GroupId)
                && db.ForeignLanguageAdmissions.FirstOrDefault(_ => _.studentId == adm.studentId
                            && _.ForeignLanguageCompetitionGroupId == FinalSubgroup.Meta.CompetitionGroupId
                            && _.Status == AdmissionStatus.Admitted).ForeignLanguageId == adm.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.ForeignLanguageId
                           && (adm.Student.Status == "Активный" || adm.Student.Status == "Отп.с.посещ." || adm.Student.Status == "Отп.дород.послерод.")).ToList();

            foreach (var flSubgroupMembership in subgroupAdmission)
            {
                var existsMembership =
                    db.ForeignLanguageSubgroupMemberships.FirstOrDefault(
                        m => (m.studentId == flSubgroupMembership.studentId)
                             && m.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer == FinalSubgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer
                             && m.Subgroup.Meta.CompetitionGroupId == FinalSubgroup.Meta.CompetitionGroupId);

                if (existsMembership == null)
                {
                    var membership = new ForeignLanguageSubgroupMembership()
                    {
                        studentId = flSubgroupMembership.studentId,
                        SubgroupId = FinalSubgroup.Id
                    };
                    db.ForeignLanguageSubgroupMemberships.Add(membership);
                    db.SaveChanges();
                    Logger.Info($"Студент {membership.studentId} скопирован в подгруппу {membership.SubgroupId} ИЯ");
                }
                 else if(existsMembership.SubgroupId != FinalSubgroup.Id) continue;

                if (subgroupTeachers)
                {
                   FinalSubgroup.TeacherId = SourceSubgroup.TeacherId;
                   FinalSubgroup.Description = SourceSubgroup.Description;
                }

                db.SaveChanges();
            }
            
        }

        private void CopyTeachers(ForeignLanguageSubgroup SourceSubgroup, ForeignLanguageSubgroup FinalSubgroup)
        {
            var dstGroupsIds = FinalSubgroup.Meta.CompetitionGroup.Groups.Select(_ => _.Id).ToList();
            var subgroupAdmission = new List<ForeignLanguageSubgroupMembership>();
            if (FinalSubgroup.Meta.CompetitionGroup.SemesterId == 2 && FinalSubgroup.Meta.CompetitionGroup.StudentCourse == 2)
            {
                subgroupAdmission = db.ForeignLanguageSubgroupMemberships.Where(adm => adm.SubgroupId == SourceSubgroup.Id
                            && adm.Subgroup.Meta.CompetitionGroupId == SourceSubgroup.Meta.CompetitionGroupId
                            && dstGroupsIds.Contains(adm.Student.GroupId)
                            && db.ForeignLanguageAdmissions.FirstOrDefault(_ => _.studentId == adm.studentId
                             && _.ForeignLanguageCompetitionGroupId == FinalSubgroup.Meta.CompetitionGroupId
                             && _.Status == AdmissionStatus.Admitted).ForeignLanguage.Module.title.Contains(adm.Subgroup.Name.Substring(0, SourceSubgroup.Name.IndexOf("\\")))
                             && (adm.Student.Status == "Активный" || adm.Student.Status == "Отп.с.посещ." || adm.Student.Status == "Отп.дород.послерод.")).ToList();
            }
            else
                subgroupAdmission = db.ForeignLanguageSubgroupMemberships.Where(adm => adm.SubgroupId == SourceSubgroup.Id
                    && adm.Subgroup.Meta.CompetitionGroupId == SourceSubgroup.Meta.CompetitionGroupId
                    && dstGroupsIds.Contains(adm.Student.GroupId)
                    && db.ForeignLanguageAdmissions.FirstOrDefault(_ => _.studentId == adm.studentId
                                && _.ForeignLanguageCompetitionGroupId == FinalSubgroup.Meta.CompetitionGroupId
                                && _.Status == AdmissionStatus.Admitted).ForeignLanguageId == adm.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.ForeignLanguageId
                               && (adm.Student.Status == "Активный" || adm.Student.Status == "Отп.с.посещ." || adm.Student.Status == "Отп.дород.послерод.")).ToList();

            foreach (var flSubgroupMembership in subgroupAdmission)
            {
                FinalSubgroup.TeacherId = SourceSubgroup.TeacherId;
                FinalSubgroup.Description = SourceSubgroup.Description;

            }

        }

        
        public ActionResult DownloadAutoAdmissionReport()
        {
            Response.Headers.Add("Content-Disposition",
                "attachment; filename=Отчёт_по_копированиям_состав_подгрупп_ИЯ.txt");
            if (_sdl == null)
                _sdl = new StudentDesicionList();
            return File(_sdl.FormatStream(), "application/text");
        }
        private void CreateSubgroupCounts(int src, int dst, bool copyCount)
        {
            var destCompetitionGroup = db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == dst);
            var fls = destCompetitionGroup.ForeignLanguageProperties.Select(_ => _.ForeignLanguageId).AsQueryable();
            var metas = db.ForeignLanguageTmerPeriods
                .Where(
                    m =>
                        (m.Period.Year == destCompetitionGroup.Year) &&
                        (m.Period.SemesterId == destCompetitionGroup.SemesterId) &&
                        fls.Any(_ => _ == m.Period.ForeignLanguageId));

            foreach (var meta in metas.ToList())
            {
                var fkSubgroupCount =
                    db.ForeignLanguageSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.CompetitionGroupId == dst) &&
                            (_.ForeignLanguageDisciplineTmerPeriodId == meta.Id));

                var sourcSubgroupCount = db.ForeignLanguageSubgroupCounts.FirstOrDefault(_ => _.CompetitionGroupId == src && meta.Tmer.TmerId == _.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId && meta.Period.ForeignLanguageId == _.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguageId);

                if (destCompetitionGroup.SemesterId ==2 && destCompetitionGroup.StudentCourse ==2)
                {
                    sourcSubgroupCount = db.ForeignLanguageSubgroupCounts.FirstOrDefault(_ => _.CompetitionGroupId == src && (meta.Tmer.TmerId == _.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId || meta.Tmer.TmerId== "prex" && _.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "prz") && meta.Period.ForeignLanguage.Module.title.IndexOf(_.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.title) !=-1);

                }           

                if (fkSubgroupCount == null)
                {
                    var newSubgroupCount = new ForeignLanguageSubgroupCount
                    {
                        CompetitionGroupId = dst,
                        ForeignLanguageDisciplineTmerPeriodId = meta.Id,
                        GroupCount = 0
                    };
                    if (sourcSubgroupCount != null && copyCount)
                    {
                        newSubgroupCount.GroupCount = sourcSubgroupCount.GroupCount;
                    }
                    db.ForeignLanguageSubgroupCounts.Add(newSubgroupCount);
                }
                else
                {
                    if (sourcSubgroupCount != null && copyCount)
                    {
                        fkSubgroupCount.GroupCount = sourcSubgroupCount.GroupCount;
                    }
                }
            }
            db.SaveChanges();
        }

       
        #region Edit

        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var module = db.UniModules().Include("ForeignLanguage.Periods").First(m => m.uuid == id);

            if (module == null)
                return HttpNotFound();

            var techSelector = new SelectList(db.ModuleTeches, "Id", "Name", module.ForeignLanguage?.ModuleTechId);

            var semesters = db.Semesters.ToList();
            var semesterSelector = new SelectList(semesters, "Id", "Name");
            var courses = new List<object>
            {
                new {Id = "", Name = "Все"},
                new {Id = 1, Name = "1"},
                new {Id = 2, Name = "2"},           
            };
            var model = new ForeignLanguageEditViewModel
            {
                Module = module,
                moduleUUId = module.ForeignLanguage == null ? string.Empty : module.uuid,
                showInLc = module.ForeignLanguage?.ShowInLC ?? false,
                techid = module.ForeignLanguage?.ModuleTechId.ToString() ?? string.Empty,
                tech = module.ForeignLanguage?.Tech.Name ?? string.Empty,
                TechSelector = techSelector,
                SemesterSelector = semesterSelector
            };

            if (module.ForeignLanguage == null)
                model.periods = new List<ForeignLanguagePeriodEditViewModel>();
            else
                model.periods = module.ForeignLanguage.Periods.Select(p =>
                        new ForeignLanguagePeriodEditViewModel
                        {
                            id = p.Id,
                            year = p.Year,
                            semesterId = p.SemesterId,
                            semesterName = p.Semester.Name,
                            Selector = new SelectList(semesters, "Id", "Name", p.SemesterId),
                            CourseSelector = new SelectList(courses, "Id", "Name", p.Course),
                            selectionDeadline = p.SelectionDeadline,
                            Course = p.Course
                        }
                ).ToList();

            ViewBag.CanEdit = User.IsInRole(ItsRoles.ForeignLanguageManager);
            ViewBag.CanLimitEdit = User.IsInRole(ItsRoles.ForeignLanguageManager);

            return View(model);
        }

        // TODO: amir Roles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ForeignLanguageEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var foreignLanguage = new ForeignLanguage
                {
                    ModuleId = model.Module.uuid,
                    Periods = new List<ForeignLanguagePeriod>()
                };

                if (User.IsInRole(ItsRoles.ForeignLanguageManager))
                {
                    foreach (var p in model.periods)
                    {
                        if (p.id < 0)
                            p.id = 0;

                        //добавили и тут же удалили
                        if (p.isDeleted && (p.id == 0)) continue;

                        var period = new ForeignLanguagePeriod
                        {
                            Id = p.id,
                            ForeignLanguageId = foreignLanguage.ModuleId,
                            Year = p.year,
                            SemesterId = p.semesterId,
                            Course = p.Course,
                            SelectionDeadline = p.selectionDeadline
                        };

                        if (p.isDeleted)
                        {
                            db.Entry(period).State = EntityState.Deleted;
                        }
                        else
                        {
                            foreignLanguage.Periods.Add(period);
                            db.Entry(period).State = p.id == 0 ? EntityState.Added : EntityState.Modified;
                        }
                    }
                    foreignLanguage.ShowInLC = model.showInLc;
                    foreignLanguage.ModuleTechId = int.Parse(model.techid);

                    db.Entry(foreignLanguage).State = string.IsNullOrEmpty(model.moduleUUId) ? EntityState.Added : EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index", new {focus = model.moduleUUId});
            }
            //щас по новой будем делать форму с ошибками, нужны справочники
            var techSelector = new SelectList(db.ModuleTeches, "Id", "Name", model.techid);

            var semesters = db.Semesters.ToList();
            var semesterSelector = new SelectList(semesters, "Id", "Name");

            model.TechSelector = techSelector;
            model.SemesterSelector = semesterSelector;

            foreach (var p in model.periods)
                p.Selector = new SelectList(semesters, "Id", "Name");

            return View(model);
        }

        public ActionResult Disciplines(string moduleId)
        {
            var module = db.UniModules().Include("ForeignLanguage.Disciplines").Where(m => m.uuid == moduleId).Single();

            ViewBag.Title = string.Format(@"Дисциплины для модуля ИЯ ""{0}""", module.title);
            ViewBag.ForeignLanguageId = moduleId;
            ViewBag.CanEdit = User.IsInRole(ItsRoles.ForeignLanguageManager);

            var model = new List<ForeignLanguageDisciplineViewModel>(module.disciplines.Count);
            foreach (var d in module.disciplines)
            {
                var r = new ForeignLanguageDisciplineViewModel
                {
                    Discipline = d,
                    ForeignLanguageDiscipline =
                        module.ForeignLanguage.Disciplines.FirstOrDefault(f => f.DisciplineUid == d.uid)
                };

                model.Add(r);
            }

            return View(model);
        }

        [Authorize(Roles = ItsRoles.ForeignLanguageManager)]
        public ActionResult EditTmers(string foreignLanguageId, string disciplineId)
        {
            var keys = new List<int?>(3) {1, 2, 3};
            var tmers = db.Tmers.Where(t => keys.Contains(t.kgmer)
                                            && (t.kmer != "U039")
                                            && (t.kmer != "U032")
                                            && (t.kmer != "U033"))
                .ToList();

            var foreignLanguage =
                db.ForeignLanguages.Include("Disciplines.Tmers").Single(m => m.ModuleId == foreignLanguageId);
            var discipline = db.Disciplines.Single(d => d.uid == disciplineId);

            var model = new ForeignLanguageTmersViewModel(foreignLanguage, discipline, tmers);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.ForeignLanguageManager)]
        public ActionResult EditTmers(ForeignLanguageTmersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var foreignLanguage =
                    db.ForeignLanguages
                        .Include("Disciplines.Tmers")
                        .Single(m => m.ModuleId == model.ForeignLanguage.ModuleId);
                var foreignLanguageDiscipline =
                    foreignLanguage.Disciplines.FirstOrDefault(d => d.DisciplineUid == model.Discipline.uid);
                if (foreignLanguageDiscipline == null)
                {
                    foreignLanguageDiscipline = new ForeignLanguageDiscipline
                    {
                        DisciplineUid = model.Discipline.uid,
                        ForeignLanguageId = foreignLanguage.ModuleId,
                        Tmers = new List<ForeignLanguageDisciplineTmer>()
                    };
                    foreignLanguage.Disciplines.Add(foreignLanguageDiscipline);
                }

                var tmers = new List<ForeignLanguageTmersRowViewModel>();
                tmers.AddRange(model.Tmers1);
                tmers.AddRange(model.Tmers2);
                tmers.AddRange(model.Tmers3);

                foreach (var t in tmers)
                {
                    var dt = foreignLanguageDiscipline.Tmers.FirstOrDefault(f => f.TmerId == t.TmerId);
                    if ((dt == null) && t.Checked)
                    {
                        dt = new ForeignLanguageDisciplineTmer {TmerId = t.TmerId};
                        foreignLanguageDiscipline.Tmers.Add(dt);
                        //db.Entry(dt).State = EntityState.Added;
                    }
                    if ((dt != null) && !t.Checked)
                    {
                        foreignLanguageDiscipline.Tmers.Remove(dt);
                        db.Entry(dt).State = EntityState.Deleted;
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Tmers", new {id = foreignLanguageDiscipline.Id});
            }
            return RedirectToAction("Disciplines", new {moduleId = model.ForeignLanguage.ModuleId});
        }

        public ActionResult Tmers(int id, string message)
        {
            ViewBag.Message = message;
            var divisions = db.Divisions.ToDictionary(d => d.uuid);
            CreateTree(divisions);

            var discipline =
                db.ForeignLanguageDisciplines
                    .Include("Tmers.Periods")
                    .Include("ForeignLanguage")
                    .Include("Discipline")
                    .Single(d => d.Id == id);

            ViewData.Add("Tmers1", discipline.Tmers.Where(t => t.Tmer.kgmer == 1).OrderBy(t => t.Tmer.kmer).ToList());
            ViewData.Add("Tmers2", discipline.Tmers.Where(t => t.Tmer.kgmer == 2).OrderBy(t => t.Tmer.kmer).ToList());
            ViewData.Add("Tmers3", discipline.Tmers.Where(t => t.Tmer.kgmer == 3).OrderBy(t => t.Tmer.kmer).ToList());

            ViewBag.CanEdit = User.IsInRole(ItsRoles.ForeignLanguageManager);

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
        [Authorize(Roles = ItsRoles.ForeignLanguageManager)]
        public ActionResult EditPeriods(string foreignLanguageId, string disciplineId)
        {
            var foreignLanguage =
                db.ForeignLanguages.Include(m => m.Periods).Single(m => m.ModuleId == foreignLanguageId);
            var discipline =
                db.ForeignLanguageDisciplines
                    .Include("Tmers.Periods")
                    .Single(d => (d.ForeignLanguageId == foreignLanguageId) && (d.DisciplineUid == disciplineId));

            var model = new ForeignLanguageTmersPeriodViewModel(foreignLanguage, discipline);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.ForeignLanguageManager)]
        public ActionResult EditPeriods(ForeignLanguageTmersPeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var foreignLanguageDiscipline =
                    db.ForeignLanguageDisciplines
                        .Include("Tmers.Periods")
                        .Single(d => d.Id == model.Discipline.Id);

                foreach (var t in model.Rows)
                {
                    var dt = foreignLanguageDiscipline.Tmers.FirstOrDefault(f => f.Id == t.Tmer.Id);
                    var tp = dt.Periods.FirstOrDefault(f => f.ForeignLanguagePeriodId == t.Period.Id);
                    if ((tp == null) && t.Checked)
                    {
                        tp = new ForeignLanguageDisciplineTmerPeriod
                        {
                            ForeignLanguageDisciplineTmerId = t.Tmer.Id,
                            ForeignLanguagePeriodId = t.Period.Id
                        };
                        dt.Periods.Add(tp);
                        //db.Entry(dt).State = EntityState.Added;
                    }
                    if ((tp != null) && !t.Checked)
                    {
                        if (tp.ForeignLanguageSubgroupCounts.Any(c => c.Subgroups.Any()))
                            return RedirectToAction("Tmers",
                                new
                                {
                                    id = foreignLanguageDiscipline.Id,
                                    message = "Невозможно удалить периоды, т.к. на них есть подгруппы"
                                });

                        if (tp.ForeignLanguageSubgroupCounts.Any())
                            tp.ForeignLanguageSubgroupCounts.Clear();

                        dt.Periods.Remove(tp);
                        db.Entry(tp).State = EntityState.Deleted;
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Tmers", new {id = foreignLanguageDiscipline.Id});
            }
            return RedirectToAction("Disciplines", new {moduleId = model.ForeignLanguage.ModuleId});
        }

        //Get: Редактирование кафедр
        [Authorize(Roles = ItsRoles.ForeignLanguageManager)]
        public ActionResult EditDivisions(int foreignLanguageDisciplineTmerPeriodId)
        {
            var period =
                db.ForeignLanguageTmerPeriods.Where(t => t.Id == foreignLanguageDisciplineTmerPeriodId).Single();

            var divisions = db.Divisions.ToList();

            var model = new MinorDivisionViewModel(divisions, period, period.Tmer.ForeignLanguageDisciplineId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.ForeignLanguageManager)]
        public ActionResult EditDivisions(MinorDivisionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var period =
                    db.ForeignLanguageTmerPeriods
                        .Include(p => p.Divisions)
                        .Include(p => p.Tmer)
                        .Single(p => p.Id == model.PeriodId);

                var rows = model.GetAllRows();
                foreach (var d in rows)
                {
                    var pd = period.Divisions.FirstOrDefault(f => f.uuid == d.DivisionID);

                    if ((pd == null) && d.Selected)
                    {
                        var division = new Division {uuid = d.DivisionID};
                        period.Divisions.Add(division);
                        db.Entry(division).State = EntityState.Unchanged;
                    }
                    if ((pd != null) && !d.Selected)
                        period.Divisions.Remove(pd);
                }

                db.SaveChanges();

                return RedirectToAction("Tmers", new {id = period.Tmer.ForeignLanguageDisciplineId});
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.SectionFKManager)]
        public ActionResult SetDeadlines(string modules, string deadlines)
        {
            List<ForeignLanguagePeriod> newperiods = JsonConvert.DeserializeObject<List<ForeignLanguagePeriod>>(deadlines);

        var selectedmodules = JsonConvert.DeserializeObject<List<string>>(modules);

        var selectedFLs = db.ForeignLanguages.Where(s => selectedmodules.Contains(s.ModuleId)).ToList();

        string excludedsection = System.String.Empty;

            foreach (var module in selectedFLs)
            {
                foreach (var period in newperiods)
                {
                    if (module.Periods.Any(p =>
                        p.Year == period.Year && p.SemesterId == period.SemesterId &&
                        p.Course == period.Course))
                    {
                        excludedsection += module.Module.title + "  Год:" + period.Year + " Курс " + period.Course +
                                           " Семестр  " + period.SemesterId + "<br>";
                        continue;
                    }

                    var newperiod = new ForeignLanguagePeriod
                    {
                        ForeignLanguageId = module.ModuleId,
                        Year = period.Year,
                        SemesterId = period.SemesterId,
                        SelectionDeadline = period.SelectionDeadline,
                        Course = period.Course,
                    }; 
                    db.ForeignLanguagePeriods.Add(newperiod);
                }
            }
                            db.SaveChanges();
                            string message = string.IsNullOrEmpty(excludedsection) ? null : excludedsection;
                            return JsonNet(message);
        }


        #endregion 
    }

    internal class ForeignLanguageSubgroupCountComparer:IEqualityComparer<ForeignLanguageSubgroupCount>
    {
        public bool Equals(ForeignLanguageSubgroupCount dst, ForeignLanguageSubgroupCount src)
        {
             return dst.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.ForeignLanguage.Module.title.EndsWith("(экзамен)") && src.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.ForeignLanguage.Module.title == dst.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.ForeignLanguage.Module.title.Replace("(экзамен)", "").Trim() && (
                       src.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "tprak" && src.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == dst.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId || src.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "prz" && dst.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "prex") == true ? true : false;           
        }
        public int GetHashCode(ForeignLanguageSubgroupCount obj)
        {
            return 0;
        }

    }

    internal class ForeignLanguageSubgroupsComparerForModulsWithExam : IEqualityComparer<ForeignLanguageSubgroup>
    {
         public bool Equals (ForeignLanguageSubgroup dst, ForeignLanguageSubgroup src)
        {
            return dst.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == src.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId  && dst.Name.Replace(" (экзамен)", string.Empty) == src.Name || src.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "prz" &&
               dst.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "prex" && dst.Name.Replace(" (экзамен)", string.Empty).Replace("экзамен", "зачет") == src.Name ? true : false;                
        }

        public int GetHashCode(ForeignLanguageSubgroup obj)
        {
            return 0;
        }
            
    }
    
    internal class ForeignLanguageSubgroupsComparer: IEqualityComparer<ForeignLanguageSubgroup>
    {
        public bool Equals(ForeignLanguageSubgroup src, ForeignLanguageSubgroup dst)
        {
            if (Object.ReferenceEquals(dst, null) || Object.ReferenceEquals(src, null))
                return false;
            return dst.Name == src.Name && dst.InnerNumber == src.InnerNumber;
        }

        public int GetHashCode(ForeignLanguageSubgroup  flsubgroup)
        {
            if (Object.ReferenceEquals(flsubgroup, null)) return 0;
            int hashFLsubgroupName = flsubgroup.Name == null ? 0 : flsubgroup.Name.GetHashCode();
            int hashFLsubgroupInnerNumber = flsubgroup.InnerNumber.GetHashCode();
            return hashFLsubgroupName ^ hashFLsubgroupInnerNumber;

        }

    }

}