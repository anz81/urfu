using Ext.Utilities;
using Ext.Utilities.Linq;
using Newtonsoft.Json;
using PagedList.Core;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Common;
using Urfu.Its.Integration.Queues;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;
//using WebGrease.Css.Extensions;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.SectionFKManager)]
    public class SectionFKAdmissionController : BaseController
    {
        private static StudentDesicionList _sdl;
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: SectionFKAdmission
        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var modules = GetFilteredMinorModules(competitionGroupId, sort, filter);

                var paginated = modules.ToPagedList(page ?? 1, limit ?? 25);

                return JsonNet(new
                {
                    data = paginated,
                    total = modules.Count()
                });
            }
            var competitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);

            ViewBag.IsInMassPublishRole = User.IsInRole(ItsRoles.SectionFKMassPublishAdmissions);
            return View(competitionGroup);
        }

        private IQueryable<object> GetFilteredMinorModules(int competitionGroupId, string sort, string filter)
        {
            var filterRules = FilterRules.Deserialize(filter);
            var competitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            var sortRules = SortRules.Deserialize(sort);
            if (sortRules.Count == 0)
                sortRules.Add(new SortRule
                {
                    Property = "shortTitle",
                    Direction = SortDirection.Ascending
                });
            var modules = _db.SectionFKProperties.Include("SectionFK.Module")
                .Where(_ => _.SectionFKCompetitionGroupId == competitionGroup.Id &&
                _.SectionFk.Periods.Any(d=>(d.Year == competitionGroup.Year) && (d.SemesterId == competitionGroup.SemesterId) && 
                (d.Course ==null || d.Course == competitionGroup.StudentCourse)))
                .Select(p => new
                {
                    p.Id,
                    number = p.SectionFk.Module.number.ToString(),
                    p.SectionFk.Module.shortTitle,
                    p.SectionFk.Module.testUnits,
                    limit =
                    _db.SectionFKProperties.FirstOrDefault(
                            _ => (_.SectionFKId == p.SectionFKId) && (_.SectionFKCompetitionGroupId == competitionGroupId))
                        .Limit,

                    selection =
                        _db.SectionFKStudentSelectionPriorities.Count(
                            s => (s.Section.ModuleId == p.SectionFKId) && (s.competitionGroupId == competitionGroupId) && s.priority > 0 && !s.Student.Sportsman),

                    changeSelection =
                        _db.SectionFKStudentSelectionPriorities.Count(
                            s => (s.Section.ModuleId == p.SectionFKId) && (s.competitionGroupId == competitionGroupId) && s.changePriority > 0 && !s.Student.Sportsman),

                    addmission =
                        _db.SectionFKAdmissions.Count(
                            a =>
                                (a.SectionFKId == p.SectionFKId) &&
                                (a.SectionFKCompetitionGroupId == competitionGroupId) &&
                                (a.Status == AdmissionStatus.Admitted)),

                    addmissionWOS = _db.SectionFKAdmissions.Count(
                        a =>
                            (a.SectionFKId == p.SectionFKId) &&
                            (a.SectionFKCompetitionGroupId == competitionGroupId) &&
                            (a.Status == AdmissionStatus.Admitted) &&
                            (!a.Student.Sportsman) &&
                            (a.Student.Status == "Активный" || a.Student.Status == "Отп.с.посещ.")),

                    addmissionSportsmans = _db.SectionFKAdmissions.Count(
                        a =>
                            (a.SectionFKId == p.SectionFKId) &&
                            (a.SectionFKCompetitionGroupId == competitionGroupId) &&
                            (a.Status == AdmissionStatus.Admitted) &&
                            (a.Student.Sportsman)),

                    vacancy = p.Limit - _db.SectionFKAdmissions.Count(
                        a =>
                            (a.SectionFKId == p.SectionFKId) &&
                            (a.SectionFKCompetitionGroupId == competitionGroupId) &&
                            (a.Status == AdmissionStatus.Admitted) &&
                            (!a.Student.Sportsman) &&
                            (a.Student.Status == "Активный" || a.Student.Status == "Отп.с.посещ.")),

                })
                .Where(filterRules)
                .OrderBy(sortRules.FirstOrDefault(), m => m.number);
            return modules;
        }

        public ActionResult CompetitionGroupStudents(int id, int? page, int? limit, string sort, string filter)
        {
            //TODO: amir roles
            ViewBag.CanEdit = "true";
            //User.IsInRole(ItsRoles.MinorAutoAdmission) && db.IsMinorAccessible(User, competitionGroup.ModuleId) ? "true" : "false";

            ViewBag.Id = id;

            var property = _db.SectionFKProperties.Find(id);
            ViewBag.Title =
                $"Год {property.SectionFkCompetitionGroup.Year}/Семестр {property.SectionFkCompetitionGroup.Semester.Name} - Зачисление на секции ФК {property.SectionFk.Module.shortTitle} \"{property.SectionFkCompetitionGroup.Name}\"";
            ViewBag.MinorPeriodId = id;
            ViewBag.AdmittedCount = _db.SectionFKAdmissions.Count(
                                    _ =>
                                        _.SectionFKId == property.SectionFKId &&
                                        _.SectionFKCompetitionGroupId == property.SectionFKCompetitionGroupId &&
                                        _.Status == AdmissionStatus.Admitted);
            ViewBag.AdmittedCountWOSportsmens = _db.SectionFKAdmissions.Count(
                                                _ =>
                                                    _.SectionFKId == property.SectionFKId &&
                                                    _.SectionFKCompetitionGroupId ==
                                                    property.SectionFKCompetitionGroupId &&
                                                    _.Status == AdmissionStatus.Admitted && !_.Student.Sportsman);
            return View(property);
        }

        public ActionResult CompetitionGroupStudentsAjax(int id, bool hideStudents, int? page, int? limit, string sort, string filter)
        {
            var sortRules = SortRules.Deserialize(sort);
            var filterRules = FilterRules.Deserialize(filter);

            var students = GetFilteredCompetitionGroupStudents(id, sortRules, filterRules, hideStudents);

            var paginated = students.ToPagedList(page ?? 1, limit ?? 25);
            return JsonNet(new
            {
                data = paginated,
                total = students.Count()
            });
        }

        public ActionResult DownloadCompetitionGroupStudents(int id, string filter)
        {
            var property = _db.SectionFKProperties.Find(id);
            var fileName = $"{property.SectionFkCompetitionGroup.ShortName} {property.SectionFk.Module.title}";

            var filterRules = FilterRules.Deserialize(filter);
            var reportVms = GetFilteredCompetitionGroupStudents(id, null, filterRules);
            var stream = new VariantExport().Export(new { Rows = reportVms }, "sectionFKCGStudentsReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{fileName}.xlsx".ToDownloadFileName());

        }
        private IQueryable<object> GetFilteredCompetitionGroupStudents(int id, SortRules sortRules, FilterRules filterRules, bool hideStudents = false)
        {
            var property = _db.SectionFKProperties.Include(x => x.SectionFkCompetitionGroup).FirstOrDefault(_ => _.Id == id);


            var includedGroups = property.SectionFkCompetitionGroup.Groups.Select(_ => _.Id).AsQueryable();
            var students = _db.Students.Include(m => m.SectionFKSelections).Include(m => m.SectionFKAdmissions)
                .Where(s => includedGroups.Any(d => d == s.Group.Id) 
                    && (!hideStudents || hideStudents && (s.Status == "Активный" || s.Status == "Отп.с.посещ." || s.Status == "Отп.дород.послерод."))
                )
                .Select(s => new
                {
                    s.Id,
                    GroupName = _db.GroupsHistories.FirstOrDefault(g => g.GroupId == s.GroupId && g.YearHistory == property.SectionFkCompetitionGroup.Year).Name,
                    s.Person.Surname,
                    s.Person.Name,
                    s.Person.PatronymicName,
                    s.Rating,
                    s.IsTarget,
                    s.IsInternational,
                    s.Compensation,
                    s.PersonalNumber,
                    s.sectionFKDebtTerms,
                    HasDebtTerms = s.sectionFKDebtTerms != null & s.sectionFKDebtTerms.Length > 2 ? "Да" : "Нет",
                    Modified =
                    _db.SectionFKStudentSelectionPriorities
                        .Where(p => (p.studentId == s.Id) && (p.sectionId == property.SectionFKId))
                        .Select(p => (DateTime?)p.modified)
                        .FirstOrDefault(),
                    ModifiedString =
                    _db.SectionFKStudentSelectionPriorities
                        .Where(p => (p.studentId == s.Id) && (p.sectionId == property.SectionFKId))
                        .Select(p => p.modified.ToString())
                        .FirstOrDefault(),

                    Priority =
                        _db.SectionFKStudentSelectionPriorities
                            .Where(p => (p.studentId == s.Id) && (p.sectionId == property.SectionFKId))
                            .Select(p => p.priority)
                            .FirstOrDefault(),
                    ChangePriority = _db.SectionFKStudentSelectionPriorities
                            .Where(p => (p.studentId == s.Id) && (p.sectionId == property.SectionFKId))
                            .Select(p => p.changePriority)
                            .FirstOrDefault(),

                    StudentStatus = s.Status,
                    Teacher = _db.SectionFKSubgroupMemberships
                        .Where(
                            a =>
                                (a.studentId == s.Id) &&
                                (a.Subgroup.Meta.CompetitionGroupId == property.SectionFKCompetitionGroupId)
                                )
                        .Select(a => a.Subgroup.Teacher.initials)
                        .FirstOrDefault(),
                    Status = _db.SectionFKAdmissions
                        .Where(
                            a =>
                                (a.studentId == s.Id) && (a.SectionFKId == property.SectionFKId) &&
                                (a.SectionFKCompetitionGroupId == property.SectionFKCompetitionGroupId))
                        .Select(a => a.Status)
                        .FirstOrDefault(),
                    OtherAdmission = _db.SectionFKAdmissions
                        .Where(
                            a =>
                                (a.studentId == s.Id) && (a.SectionFKId != property.SectionFKId) &&
                                (a.SectionFKCompetitionGroupId == property.SectionFKCompetitionGroupId) &&
                                (a.Status == AdmissionStatus.Admitted))
                        .Select(a => a.SectionFK.Module.shortTitle)
                        .FirstOrDefault(),

                    IsPriority = _db.SectionFKStudentSelectionPriorities
                        .FirstOrDefault(p => (p.studentId == s.Id) && (p.sectionId == property.SectionFKId) && (p.priority != null || p.changePriority != null))
                        != null,

                    Published =
                    s.SectionFKAdmissions.Any(
                        a =>
                            (a.studentId == s.Id) && (a.SectionFKId == property.SectionFKId) &&
                            (a.SectionFKCompetitionGroupId == property.SectionFKCompetitionGroupId)) &&
                    s.SectionFKAdmissions.FirstOrDefault(
                        a =>
                            (a.studentId == s.Id) && (a.SectionFKId == property.SectionFKId) &&
                            (a.SectionFKCompetitionGroupId == property.SectionFKCompetitionGroupId)).Published,
                    Sportsman = s.Sportsman

                });
            if (sortRules != null)
            { students = students.OrderBy(sortRules.FirstOrDefault(), v => v.GroupName); }

            students = students.Where(filterRules);
            return students;
        }


        public ActionResult SetCompetitionGroupAdmissionStatus(string[] studentIds, int propertyId, AdmissionStatus status)
        {
            var property = _db.SectionFKProperties.FirstOrDefault(p => p.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");

            var overlimit = false;

            if (_db.IsSectionFKAccessible(User, property.SectionFKId))
            {
                overlimit = studentIds.Select(_ => SetCompetitionGroupAdmissionStatusInternal(_, status, property, overlimit)).ToList().Any(_ => _);
            }
            else
            {
                return Json(new { reload = false, msg = "У вас нет прав на изменение зачислений секций ФК" });
            }

            return Json(new { reload = false, msg = overlimit ? "Превышен лимит" : null });
        }

        private bool SetCompetitionGroupAdmissionStatusInternal(string studentId, AdmissionStatus status,
            SectionFKProperty property, bool overlimit)
        {
            Action<AdmissionStatus> dismissOtherAdmissionStatus = (admStatus) =>
            {
                if (admStatus == AdmissionStatus.Admitted)
                {
                    var admissions = _db.SectionFKAdmissions.Where(
                        a =>
                            (a.studentId == studentId) &&
                            (a.SectionFKCompetitionGroupId == property.SectionFKCompetitionGroupId) &&
                            (a.SectionFKCompetitionGroup.SemesterId == property.SectionFkCompetitionGroup.SemesterId) &&
                            a.SectionFKId != property.SectionFKId).ToList();
                    admissions.ForEach(a =>
                    {
                        if (a.Status == AdmissionStatus.Admitted)
                        {
                            CleanSubgroupMembership(a);
                        }
                        a.Status = AdmissionStatus.Indeterminate;
                    });
                    _db.SaveChanges();

                }
            };
            var admission =
                  _db.SectionFKAdmissions.FirstOrDefault(
                      a =>
                          (a.studentId == studentId) &&
                          (a.SectionFKCompetitionGroupId == property.SectionFKCompetitionGroupId) &&
                          (a.SectionFKCompetitionGroup.Year == property.SectionFkCompetitionGroup.Year) &&
                          (a.SectionFKCompetitionGroup.SemesterId == property.SectionFkCompetitionGroup.SemesterId) &&
                          a.SectionFKId == property.SectionFKId);

            if (admission == null)
            {
                admission = new SectionFKAdmission
                {
                    studentId = studentId,
                    SectionFKCompetitionGroupId = property.SectionFKCompetitionGroupId,
                    Status = status,
                    SectionFKId = property.SectionFKId,
                    Published = false
                };
                dismissOtherAdmissionStatus(status);
                _db.SectionFKAdmissions.Add(admission);
            }
            else
            {
                if (admission.Status == AdmissionStatus.Admitted)
                {
                    CleanSubgroupMembership(admission);
                }
                admission.Status = status;
                admission.Published = false;

                dismissOtherAdmissionStatus(status);

            }
            _db.SaveChanges();


            var admissionCount = _db.SectionFKAdmissions.Count(
                a =>
                    (a.SectionFKCompetitionGroupId == property.SectionFKCompetitionGroupId) &&
                    (a.Status == AdmissionStatus.Admitted) && (a.SectionFKId == property.SectionFKId));
            if ((status == AdmissionStatus.Admitted) && (admissionCount > property.Limit))
                overlimit = true;


            return overlimit;


        }


        void CleanSubgroupMembership(SectionFKAdmission sectionFKadmission)
        {
            var subgrougMemberships =
                  _db.SectionFKSubgroupMemberships.Where(
                      _ =>
                          _.studentId == sectionFKadmission.studentId &&
                          _.Subgroup.Meta.CompetitionGroupId == sectionFKadmission.SectionFKCompetitionGroupId);

            if (subgrougMemberships != null)
            {
                foreach (var sfsm in subgrougMemberships) _db.SectionFKSubgroupMemberships.Remove(sfsm);
                //subgrougMemberships.ForEach(sfsm => _db.SectionFKSubgroupMemberships.Remove(sfsm));
            }

            _db.SaveChanges();
        }
        public void PublishCompetitionGroupAdmission(string[] studentId, int propertyId)
        {
            foreach (var id in studentId)
            {
                var property = _db.SectionFKProperties.FirstOrDefault(p => p.Id == propertyId);

                if (property != null && _db.IsSectionFKAccessible(User, property.SectionFKId))
                {
                    var admission =
                        _db.SectionFKAdmissions.Where(
                            a =>
                                (a.studentId == id) &&
                                (a.SectionFKCompetitionGroupId == property.SectionFKCompetitionGroupId) &&
                                a.SectionFKId == property.SectionFKId).FirstOrDefault();

                    if (admission == null)
                        continue;

                    admission.Published = true;
                    _db.SaveChanges();
                }

            }
        }

        [Authorize(Roles = ItsRoles.SectionFKMassPublishAdmissions)]
        public ActionResult PublishCompetitionGroupAdmissions(int competitionGroupId)
        {
            foreach(var m in _db.SectionFKAdmissions.Where(m => m.SectionFKCompetitionGroupId == competitionGroupId)) m.Published = true;
/*            _db.SectionFKAdmissions.Where(m => m.SectionFKCompetitionGroupId == competitionGroupId).ForEach(m =>
            {
                m.Published = true;
            });*/

            _db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult PrepareAuto(int competitionGroupId)
        {
            var group = _db.SectionFKCompetitionGroups.Find(competitionGroupId);

            ViewBag.AnyPrepared =
                _db.SectionFKAdmissions.Any(
                    va =>
                        (va.SectionFKCompetitionGroupId == competitionGroupId) &&
                        (va.Status != AdmissionStatus.Indeterminate));
            ViewBag.AnyPublished =
                _db.SectionFKAdmissions.Any(
                    va =>
                        (va.SectionFKCompetitionGroupId == competitionGroupId) &&
                        (va.Status != AdmissionStatus.Indeterminate) && va.Published);
            ViewBag.AnyWithoutRating =
                _db.SectionFKStudentSelectionPriorities.Any(
                    s => (s.competitionGroupId == competitionGroupId) && (s.Student.Rating == null));
            return
                View(new MinorAutoVM
                {
                    CompetitionGroup = group.Name,
                    Semester = group.Semester.Name,
                    SemesterId = group.Id,
                    Year = group.Year,
                    StudentCount =
                        _db.SectionFKStudentSelectionPriorities.Where(
                                s => s.competitionGroupId == competitionGroupId && s.priority != null)
                            .Select(s => s.Student)
                            .Distinct()
                            .Count(),
                    AdmittedCount = _db.SectionFKAdmissions.Where(
                            s =>
                                (s.SectionFKCompetitionGroupId == competitionGroupId) &&
                                (s.Status == AdmissionStatus.Admitted))
                        .Select(s => s.studentId)
                        .Distinct()
                        .Count(),
                    MinorCount = _db.SectionFKProperties.Count(m => m.SectionFKCompetitionGroupId == competitionGroupId)
                });
        }

        [HttpPost]
        public ActionResult CalculateAutoAdmissions(int competitionGroupId)
        {
            var vm = AutoAdmitt(competitionGroupId);
            return View(vm);
        }
        
        private MinorAutoVM AutoAdmitt(int competitionGroupId)
        {
            _sdl = new StudentDesicionList();

            var group = _db.SectionFKCompetitionGroups.Find(competitionGroupId);

            var year = group.Year;
            var semesterId = group.SemesterId;

            var vm = new MinorAutoVM { Semester = group.Name, Year = year, SemesterId = semesterId };

            Logger.Info("Расчёт зачислений ФК {0} {1} {2}", year, semesterId, group.Name);

            //Поиск студентов, приоритеты которых рассматривать не нужно, потому что они уже зачислены в ФК
            var alreadyAdmitted = new HashSet<string>(_db.SectionFKAdmissions.Where(
                    s => (s.SectionFKCompetitionGroupId == competitionGroupId) && (s.Status == AdmissionStatus.Admitted))
                .Select(s => s.studentId).Distinct());

            //Выбор всех секций этого периода
            var fkProperties = _db.SectionFKProperties.Where(m => m.SectionFKCompetitionGroupId == competitionGroupId).ToList();

            //Колчество мест в каждом из секции в текущем периоде
            var placesLeft = new Dictionary<string, int>();
            foreach (var fkProperty in fkProperties)
            {
                placesLeft[fkProperty.SectionFKId] = fkProperty.Limit;
                _sdl.AddNew("всего мест", fkProperty.SectionFKId, fkProperty.Limit.ToString(), null);
            }

            //Существующие зачисления вычитаются из доступных для зачисления мест
            var existingAdmissions =
                _db.SectionFKAdmissions.Where(
                        s =>
                            (s.SectionFKCompetitionGroupId == competitionGroupId) &&
                            (s.Status == AdmissionStatus.Admitted))
                    .ToList();
            foreach (var ea in existingAdmissions)
                if (placesLeft.ContainsKey(ea.SectionFKId))
                {
                    placesLeft[ea.SectionFKId]--;
                    _sdl.AddNew("место уже занято студентом", ea.SectionFKId, ea.studentId, null);
                }

            //Рассматриваются пожелания всех студентов по текущему периоду
            var wishes = _db.SectionFKStudentSelectionPriorities
                .Where(s =>
                    (s.competitionGroupId == competitionGroupId)
                    && !s.Student.Sportsman
                    && s.priority != null && s.priority != 0)
                .Select(s => new
                {
                    date =
                        _db.SectionFKStudentSelectionPriorities.Where(
                            sx =>
                                (sx.studentId == s.studentId)
                                && (sx.competitionGroupId == competitionGroupId)
                                && sx.priority <= s.priority
                                ).Max(sx => sx.modified),
                    priority = s.priority,
                    s.Student.Rating,
                    s
                })
                .OrderBy(s => s.date)
                .ThenBy(s => s.priority)
                .ThenByDescending(s => s.Rating)
                .Select(s => s.s)
                .ToList();

            foreach (var wish in wishes)
            {
                var studentId = wish.studentId;
                var sectionId = wish.sectionId;
                var priority = wish.priority;

                if (alreadyAdmitted.Contains(studentId))
                    continue;
                if (!placesLeft.ContainsKey(sectionId))
                {
                    _sdl.AddNew("у модуля нет периода", sectionId, studentId, priority);
                    continue;
                }
                if (placesLeft[sectionId] <= 0)
                {
                    _sdl.AddNew("на модуле нет мест", sectionId, studentId, priority);
                    continue;
                }

                TryAutoAdmitt(competitionGroupId, studentId, sectionId, priority, vm, placesLeft, alreadyAdmitted);
            }

            _db.SaveChanges();

            placesLeft.Remove("pstcim18hc2jg0000li0lelganc4f93s");
            var unadmitted =
                _db.Students.OnlyActive()
                    .Where(
                        s => s.Group.SectionFkCompetitionGroups.Any(cg => cg.Id == competitionGroupId) && !s.Sportsman)
                    .Where(
                        s =>
                            !_db.SectionFKAdmissions.Any(
                                a =>
                                    (a.studentId == s.Id) && (a.Status == AdmissionStatus.Admitted) &&
                                    (a.SectionFKCompetitionGroupId == competitionGroupId))).
                    OrderByDescending(s => s.Rating)
                    .Select(s => new { s.Id, s.Male }).ToList();

            var periods = _db.SectionFKPeriods.Where(p =>
                p.Course == group.StudentCourse
                && placesLeft.Keys.Contains(p.SectionFKId)
                && p.SemesterId == semesterId
                && p.Year == year
                ).Select(p => new { p.SectionFKId, p.Male }).ToList();

            foreach (var student in unadmitted)
            {
                if (placesLeft.All(p => p.Value <= 0))
                    break;

                var studentSectionIds = periods.Where(p => p.Male == student.Male || p.Male == null).Select(p => p.SectionFKId); // секции для студента, отобранные по полу
                var sectionId = placesLeft.FirstOrDefault(p => p.Value > 0 && studentSectionIds.Contains(p.Key)).Key;
                if (sectionId == null)
                    continue;

                _sdl.AddNew("у студента нет выполнимых приоритетов", sectionId, student.Id, -1);
                TryAutoAdmitt(competitionGroupId, student.Id, sectionId, -1, vm, placesLeft, alreadyAdmitted);
            }

            Logger.Info("Расчёт зачислений на секции ФК окончен {0} {1}", year, group.Name);

            _db.SaveChanges();

            _db.MinorAutoAdmissionReports.Add(new MinorAutoAdmissionReport
            {
                Content = _sdl.GetStringBuilder().ToString(),
                Date = DateTime.Now,
                ModuleType = ModuleType.SectionFk
            });

            _db.SaveChanges();
            return vm;
        }

        private void TryAutoAdmitt(int competitionGroupId, string studentId, string sectionId, int? priority,
            MinorAutoVM vm,
            Dictionary<string, int> placesLeft, HashSet<string> alreadyAdmitted)
        {
            // поиск зачислений для текущего студента
            var adm = _db.SectionFKAdmissions
                .FirstOrDefault(
                    ma => (ma.studentId == studentId) && (ma.SectionFKCompetitionGroupId == competitionGroupId));

            if (adm == null)
            {
                _sdl.AddNew("создаётся зачисление", sectionId, studentId, priority);
                //зачисления ещё не было - создаём его
                _db.SectionFKAdmissions.Add(new SectionFKAdmission
                {
                    studentId = studentId,
                    SectionFKCompetitionGroupId = competitionGroupId,
                    SectionFKId = sectionId,
                    Published = false,
                    Status = AdmissionStatus.Admitted
                });
                vm.AdmittedCount++;
            }
            else
            {
                //если в зачислении было отказано ранее - не рассматриваем более заявку студента на этот майнор
                _sdl.AddNew("перевод неопределённого статуса в зачисленный", sectionId, studentId, priority);
                if (adm.Status == AdmissionStatus.Indeterminate)
                {
                    adm.Status = AdmissionStatus.Admitted;
                    adm.Published = false;
                    vm.AdmittedCount++;
                }
                else
                {
                    _sdl.AddNew("у студента уже есть отрицательное решение", sectionId, studentId, priority);
                    return;
                }
            }

            alreadyAdmitted.Add(studentId);
            placesLeft[sectionId]--;
        }


        public ActionResult DownloadAutoAdmissionReport()
        {
            Response.Headers.Add("Content-Disposition",
                "attachment; filename=Отчёт_по_автозачислениям_на_секции_ФК.txt");
            if (_sdl == null)
                _sdl = new StudentDesicionList();
            return File(_sdl.FormatStream(), "application/text");
        }


        private IQueryable<object> PrepareReport(string sort, string filter,
            Expression<Func<SectionFKAdmission, bool>> admissionFilter, bool allWithPriority = false)
        {
            _db.Database.SetCommandTimeout(1200000);

            var expression = admissionFilter ?? (x => true);
            //((IObjectContextAdapter)_db).ObjectContext.CommandTimeout = 90;

            FilterRules rules = FilterRules.Deserialize(filter);
            var yearstr = rules?.FirstOrDefault(r => r.Property == "Year")?.Value;
            int year;
            bool isYear = Int32.TryParse(yearstr, out year);

            var semesterstr = rules?.FirstOrDefault(r => r.Property == "semesterId")?.Value;
            int semester;
            bool isSemester = Int32.TryParse(semesterstr, out semester);
            
            var periods = !allWithPriority ?
                _db.SectionFKProperties
                .Where(p => (!isYear || isYear && p.SectionFkCompetitionGroup.Year == year)
                && (!isSemester || isSemester && p.SectionFkCompetitionGroup.SemesterId == semester))
                    .Select(
                        p =>
                            new
                            {
                                p,
                                c = _db.SectionFKAdmissions
                                    .Where(expression)
                                    .Where(
                                        ma =>
                                            ma.SectionFKCompetitionGroupId == p.SectionFKCompetitionGroupId &&
                                            ma.SectionFKId == p.SectionFKId && ma.Status == AdmissionStatus.Admitted)
                                    .Select(ma => ma.Student),
                            })
                    .Where(r => r.c.Any())
                    :
                    _db.SectionFKProperties
                    .Where(p => (!isYear || isYear && p.SectionFkCompetitionGroup.Year == year)
                && (!isSemester || isSemester && p.SectionFkCompetitionGroup.SemesterId == semester))
                    .Select(
                        p =>
                            new
                            {
                                p,
                                c = _db.SectionFKStudentSelectionPriorities.Where(_ =>
                                    (_.priority != null && _.priority > 0 || _.changePriority != null && _.changePriority > 0)
                                    && _.competitionGroupId == p.SectionFKCompetitionGroupId
                                    && _.sectionId == p.SectionFKId
                                    ).Select(_ => _.Student)
                            })
                    .Where(r => r.c.Any());
            
            var reportVms = periods
                .SelectMany(p => p.c
                    .Select(s => new
                    {
                        moduleName = p.p.SectionFk.Module.title,
                        CompetitionGroupName = p.p.SectionFkCompetitionGroup.Name,
                        Status = (int?)_db.SectionFKAdmissions.Where(_ => _.SectionFKCompetitionGroupId == p.p.SectionFKCompetitionGroupId && _.studentId == s.Id && p.p.SectionFKId == _.SectionFKId).Select(_ => _.Status).FirstOrDefault() == 0 ? "Нет решения" :
                        ((int?)_db.SectionFKAdmissions.Where(_ => _.SectionFKCompetitionGroupId == p.p.SectionFKCompetitionGroupId && _.studentId == s.Id && p.p.SectionFKId == _.SectionFKId).Select(_ => _.Status).FirstOrDefault() == 1 ? "Зачислен" : "Не зачислен"),
                        p.p.SectionFkCompetitionGroup.Year,
                        Semester = p.p.SectionFkCompetitionGroup.Semester.Name,
                        semesterId = p.p.SectionFkCompetitionGroup.SemesterId,
                        MinStudentsCount = 0,
                        MaxStudentsCount = p.p.Limit,
                        s.Id,
                        GroupName = _db.GroupsHistories.FirstOrDefault(g => g.GroupId == s.GroupId && g.YearHistory == p.p.SectionFkCompetitionGroup.Year).Name,
                        s.Person.Name,
                        s.Person.Surname,
                        s.Person.PatronymicName,
                        male = s.Male ? "м" : "ж",
                        StudentStatus=s.Status,
                        s.Sportsman,
                        s.Rating,
                        s.IsTarget,
                        s.IsInternational,
                        s.Compensation,
                        priority =
                        _db.SectionFKStudentSelectionPriorities.FirstOrDefault(
                            _ =>
                                _.studentId == s.Id && _.competitionGroupId == p.p.SectionFKCompetitionGroupId &&
                                _.sectionId == p.p.SectionFKId).priority,
                        changePriority =
                        _db.SectionFKStudentSelectionPriorities.FirstOrDefault(
                            _ =>
                                _.studentId == s.Id && _.competitionGroupId == p.p.SectionFKCompetitionGroupId &&
                                _.sectionId == p.p.SectionFKId).changePriority,
                        SubgroupTeachers = _db.SectionFKSubgroupMemberships.Where(_ => _.Subgroup.Meta.CompetitionGroupId == p.p.SectionFKCompetitionGroupId && _.studentId == s.Id && _.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId == p.p.SectionFKId).Select(_ => _.Subgroup.Teacher.initials).Distinct(),
                        Teachers = p.p.Teachers.Select(t => t.initials),
                        Places = p.p.TrainingPlaces.Select(t => t.Description + ", " + t.Address)
                    }));
            //.Where(_ => (!allWithPriority) || (_.priority != null && _.priority > 0));

            if (sort != null)
                reportVms = reportVms.OrderBy(SortRules.Deserialize(sort).FirstOrDefault());
            else
                reportVms =
                    reportVms.OrderBy(r => r.CompetitionGroupName)
                        .ThenBy(r => r.moduleName)
                        .ThenBy(r => r.Surname)
                        .ThenBy(r => r.Name);


            reportVms = reportVms.Where(rules);

            return reportVms;
        }

        private IEnumerable<Tuple<string, int, string>> PrepareReportFormativeDivisions(string filter)
        {
            var periods =
                _db.SectionFKProperties
                    .Select(
                        p =>
                            new
                            {
                                p,
                                c =
                                _db.SectionFKAdmissions
                                    .Where(
                                        ma =>
                                            (ma.SectionFKCompetitionGroupId == p.SectionFKCompetitionGroupId) &&
                                            (ma.SectionFKId == p.SectionFKId) && (ma.Status == AdmissionStatus.Admitted))
                            })
                    .Where(r => r.c.Any());

            var reportVms = periods.SelectMany(p => p.c
                .Select(s => new
                {
                    moduleName = p.p.SectionFk.Module.title,
                    CompetitionGroupName = p.p.SectionFkCompetitionGroup.Name,
                    p.p.SectionFkCompetitionGroup.Year,
                    Semester = p.p.SectionFkCompetitionGroup.Semester.Name,
                    semesterId = p.p.SectionFkCompetitionGroup.SemesterId,
                    MinStudentsCount = 0,
                    MaxStudentsCount = p.p.Limit,
                    s.Student.Id,
                    GroupName = s.Student.Group.Name,
                    s.Student.Person.Name,
                    s.Student.Person.Surname,
                    s.Student.Status,
                    male = s.Student.Male ? "м" : "ж",
                    s.Student.Sportsman,
                    s.Student.Person.PatronymicName,
                    s.Student.Rating,
                    s.Student.IsTarget,
                    s.Student.IsInternational,
                    s.Student.Compensation,
                    Teachers = p.p.Teachers.Select(t => t.initials),
                    Places = p.p.TrainingPlaces.Select(t => t.Description + ", " + t.Address),
                    s.Student.Group.FormativeDivisionId
                }));

            reportVms = reportVms.Where(FilterRules.Deserialize(filter));
            var otherVMs = reportVms.Select(x => new { x.Semester, x.Year, x.FormativeDivisionId }).Distinct();

            return otherVMs.ToList().Select(x => Tuple.Create(x.FormativeDivisionId, x.Year, x.Semester));
        }


        public ActionResult Report(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var reportVms = PrepareReport(sort, filter, null).ToList();
                return JsonNet(reportVms);
            }
            ViewBag.Semesters = JsonConvert.SerializeObject(_db.Semesters);

            var years = _db.SectionFKCompetitionGroups.GroupBy(_ => _.Year).Select(s => new { Id = s.Key, Name = s.Key });
            ViewBag.Years = JsonConvert.SerializeObject(years);
            return View();
        }
        public ActionResult SynopticReport(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var reportVms = PrepareReport(sort, filter, null, true).ToList();
                return JsonNet(reportVms);
            }
            ViewBag.Semesters = JsonConvert.SerializeObject(_db.Semesters);

            var years = _db.SectionFKCompetitionGroups.GroupBy(_ => _.Year).Select(s => new { Id = s.Key, Name = s.Key });
            ViewBag.Years = JsonConvert.SerializeObject(years);
            return View();
        }
        public ActionResult DownloadSynopticReport(string filter)
        {
            var reportVms = PrepareReport(null, filter, null, true);
            var stream = new VariantExport().Export(new { Rows = reportVms }, "sectionFKReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Отчёт_по_секциям_ФК.xlsx".ToDownloadFileName());
        }
        public ActionResult DownloadReport(string filter)
        {
            var reportVms = PrepareReport(null, filter, null);
            var stream = new VariantExport().Export(new { Rows = reportVms }, "sectionFKReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Отчёт_по_секциям_ФК.xlsx".ToDownloadFileName());
        }


        private Stream PrepareDivReportStream(string formativeDivisionId, int year, string semesterName, string filter,
            // ReSharper disable once UnusedParameter.Local
            string sort)
        {
            var reportVms = PrepareReport(null, filter,
                x =>
                    (x.SectionFKCompetitionGroup.Year == year) &&
                    (x.SectionFKCompetitionGroup.Semester.Name == semesterName) &&
                    (x.Student.Group.FormativeDivisionId == formativeDivisionId));
            var stream = new VariantExport().Export(new { Rows = reportVms.ToList() },
                "sectionFKDivisionsReportTemplate.xlsx");
            return stream;
        }


        public FileResult DownloadDivisionReport(string sort, string filter)
        {
            using (var zipArchiveStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var tuple in PrepareReportFormativeDivisions(filter).ToList())
                    {
                        var formativedivisionId = tuple.Item1;
                        var year = tuple.Item2;
                        var semesterName = tuple.Item3;

                        var divisionName =
                            _db.Divisions.Where(d => d.uuid == formativedivisionId)
                                .Select(d => d.shortTitle)
                                .FirstOrDefault() ?? formativedivisionId;

                        using (
                            var reportStream = PrepareDivReportStream(formativedivisionId, year, semesterName, filter,
                                sort))
                        {
                            var entry =
                                zipArchive.CreateEntry(
                                    ($"{divisionName} {year} {semesterName}" + ".xlsx").CleanFileName(),
                                    CompressionLevel.Fastest);
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
                    FileDownloadName = "Секции ФК по подразделениям.zip".ToDownloadFileName()
                };
            }


        }

        public ActionResult Years()
        {
            var years = _db.SectionFKCompetitionGroups.GroupBy(_ => _.Year).Select(s => new { Id = s.Key, Name = s.Key });

            var json = Json(
                new
                {
                    data = years
                },
                new JsonSerializerSettings()
            );

            return json;
        }

    }
}