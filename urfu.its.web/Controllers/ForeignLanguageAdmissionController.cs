using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
using Newtonsoft.Json;
using PagedList.Core;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Models;
//using WebGrease.Css.Extensions;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.ForeignLanguageManager)]
    public class ForeignLanguageAdmissionController : BaseController
    {
        private static StudentDesicionList _sdl;
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: ForeignLanguageAdmission
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
            var competitionGroup = _db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);

            ViewBag.IsInMassPublishRole = User.IsInRole(ItsRoles.ForeignLanguageMassPublishAdmissions);
            return View(competitionGroup);
        }

        private IQueryable<object> GetFilteredMinorModules(int competitionGroupId, string sort, string filter)
        {
            var filterRules = FilterRules.Deserialize(filter);
            var competitionGroup = _db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            var sortRules = SortRules.Deserialize(sort);
            if (sortRules.Count == 0)
                sortRules.Add(new SortRule
                {
                    Property = "shortTitle",
                    Direction = SortDirection.Ascending
                });
            var modules = _db.ForeignLanguageProperties.Include("ForeignLanguage.Module")
                .Where(_ => _.ForeignLanguageCompetitionGroupId == competitionGroup.Id && _.ForeignLanguage.Periods.Any(d=>d.Year == competitionGroup.Year && d.SemesterId == competitionGroup.SemesterId && ((d.Course== null)||(d.Course == competitionGroup.StudentCourse))))               
                .Select(p => new
                {
                    p.Id,
                    number = p.ForeignLanguage.Module.number.ToString(),
                    p.ForeignLanguage.Module.shortTitle,
                    p.ForeignLanguage.Module.testUnits,
                    limit =
                    _db.ForeignLanguageProperties.FirstOrDefault(
                        _ =>
                            (_.ForeignLanguageId == p.ForeignLanguageId) &&
                            (_.ForeignLanguageCompetitionGroupId == competitionGroupId)).Limit,
                    selection =
                    _db.ForeignLanguageStudentSelectionPriorities.Count(
                        s => (s.ForeignLanguage.ModuleId == p.ForeignLanguageId) && (s.competitionGroupId == competitionGroupId)),
                    addmission =
                    _db.ForeignLanguageAdmissions.Count(
                        a =>
                            (a.ForeignLanguageId == p.ForeignLanguageId) &&
                            (a.ForeignLanguageCompetitionGroupId == competitionGroupId) &&
                            (a.Status == AdmissionStatus.Admitted))
                            
                })
                .Where(filterRules)
                .OrderBy(sortRules.FirstOrDefault(), m => m.number);
            return modules;
        }

        public ActionResult CompetitionGroupStudents(int id, int? page, int? limit, string sort, string filter)
        {
            //TODO: amir roles
            ViewBag.CanEdit = "true";
            
            var property = _db.ForeignLanguageProperties.Find(id);
            ViewBag.Title =
                $"Год {property.ForeignLanguageCompetitionGroup.Year}/Семестр {property.ForeignLanguageCompetitionGroup.Semester.Name} - Зачисление на модули ИЯ \"{property.ForeignLanguageCompetitionGroup.Name}\" - Модуль \"{property.ForeignLanguage.Module.shortTitle}\"";
            ViewBag.MinorPeriodId = id;
            return View(property);
        }

        public ActionResult CompetitionGroupStudentsAjax(int id, bool hideStudents, int? page, int? limit, string sort, string filter)
        {
            var sortRules = SortRules.Deserialize(sort);
            var filterRules = FilterRules.Deserialize(filter);

            var students = GetFilteredMinorStudents(id, hideStudents, sortRules, filterRules);

            var paginated = students.ToPagedList(page ?? 1, limit ?? 25);
            return JsonNet(new
            {
                data = paginated,
                total = students.Count()
            });
        }

        private IQueryable<object> GetFilteredMinorStudents(int id, bool hideStudents, SortRules sortRules, FilterRules filterRules)
        {
            var property = _db.ForeignLanguageProperties.Include(x=>x.ForeignLanguageCompetitionGroup).FirstOrDefault(_=>_.Id==id);

            var includedGroups = property.ForeignLanguageCompetitionGroup.Groups.Select(_ => _.Id).AsQueryable();
            var students = _db.Students.Include(m => m.ForeignLanguageSelections)
                .Include(m => m.ForeignLanguageAdmissions)
                .Where(s => includedGroups.Any(d => d == s.Group.Id)
                            && (!hideStudents || hideStudents &&
                                (s.Status == "Активный" || s.Status == "Отп.с.посещ." ||
                                 s.Status == "Отп.дород.послерод."))
                )
                .Select(s => new
                {
                    s.Id,
                    GroupName = _db.GroupsHistories.FirstOrDefault(g => g.GroupId == s.GroupId && g.YearHistory == property.ForeignLanguageCompetitionGroup.Year).Name,
                    s.Person.Surname,
                    s.Person.Name,
                    s.Person.PatronymicName,
                    s.Rating,
                    s.IsTarget,
                    s.IsInternational,
                    s.Compensation,
                    s.ForeignLanguageRating,
                    s.ForeignLanguageLevel,
                    coefficient = _db.RatingCoefficients.Any(c => c.ModuleId == property.ForeignLanguageId && c.Year == property.ForeignLanguageCompetitionGroup.Year && s.ForeignLanguageLevel.Contains(c.Level) && c.ModuleType == (int)ModuleTypeParam.ForeignLanguage)?
                        (decimal?)_db.RatingCoefficients.FirstOrDefault(c => c.ModuleId == property.ForeignLanguageId && c.Year == property.ForeignLanguageCompetitionGroup.Year && s.ForeignLanguageLevel.Contains(c.Level) && c.ModuleType == (int)ModuleTypeParam.ForeignLanguage).Coefficient
                        : null,
                    s.ForeignLanguageTargetLevel,
                    s.PersonalNumber,
                    StudentStatus = s.Status,
                    Status = _db.ForeignLanguageAdmissions
                        .Where(
                            a =>
                                (a.studentId == s.Id) && (a.ForeignLanguageId == property.ForeignLanguageId) &&
                                (a.ForeignLanguageCompetitionGroupId == property.ForeignLanguageCompetitionGroupId))
                        .Select(a => a.Status)
                        .FirstOrDefault(),
                    OtherAdmission = _db.ForeignLanguageAdmissions
                        .Where(
                            a =>
                                (a.studentId == s.Id) && (a.ForeignLanguageId != property.ForeignLanguageId) &&
                                (a.ForeignLanguageCompetitionGroupId == property.ForeignLanguageCompetitionGroupId) &&
                                (a.Status == AdmissionStatus.Admitted))
                        .Select(a => a.ForeignLanguage.Module.shortTitle)
                        .FirstOrDefault(),
                    IsPriority = _db.ForeignLanguageStudentSelectionPriorities
                        .Any(
                            p =>
                                (p.studentId == s.Id) && (p.sectionId == property.ForeignLanguageId) &&
                                (p.competitionGroupId == property.ForeignLanguageCompetitionGroupId)),
                    Published =
                    s.ForeignLanguageAdmissions.Any(
                        a =>
                            (a.studentId == s.Id) && (a.ForeignLanguageId == property.ForeignLanguageId) &&
                            (a.ForeignLanguageCompetitionGroupId == property.ForeignLanguageCompetitionGroupId)) &&
                    s.ForeignLanguageAdmissions.FirstOrDefault(
                            a =>
                                (a.studentId == s.Id) && (a.ForeignLanguageId == property.ForeignLanguageId) &&
                                (a.ForeignLanguageCompetitionGroupId == property.ForeignLanguageCompetitionGroupId))
                        .Published,
                    studentSelection = _db.ForeignLanguageStudentSelectionPriorities.Where(_ => _.studentId == s.Id).Select(_ => _.ForeignLanguage.Module.shortTitle).FirstOrDefault()
                });

            students = students.OrderBy(sortRules.FirstOrDefault(), v => v.GroupName);

            students = students.Where(filterRules);
            return students;
        }

        public ActionResult SetCompetitionGroupAdmissionStatus(string[] studentIds, int propertyId, AdmissionStatus status)
        {
            var property = _db.ForeignLanguageProperties.FirstOrDefault(p => p.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");

            var overlimit = false;

            if (_db.IsForeignLanguageAccessible(User, property.ForeignLanguageId))
            {
                overlimit =  studentIds.Select(_=> SetCompetitionGroupAdmissionStatusInternal(_, status, property, overlimit)).ToList().Any(_=>_);
            }
            else
            {
                return Json(new {reload = false, msg = "У вас нет прав на изменение зачислений модулей ИЯ"});
            }

            return Json(new {reload = false, msg = overlimit ? "Превышен лимит" : null});
        }

        private bool SetCompetitionGroupAdmissionStatusInternal(string studentId, AdmissionStatus status,
            ForeignLanguageProperty property, bool overlimit)
        {
            Action<AdmissionStatus> dismissOtherAdmissionStatus = (admStatus) =>
            {
                if (admStatus == AdmissionStatus.Admitted)
                {
                    var admissions = _db.ForeignLanguageAdmissions.Where(
                        a =>
                            (a.studentId == studentId) &&
                            (a.ForeignLanguageCompetitionGroupId == property.ForeignLanguageCompetitionGroupId) &&
                            (a.ForeignLanguageCompetitionGroup.SemesterId == property.ForeignLanguageCompetitionGroup.SemesterId) &&
                            a.ForeignLanguageId != property.ForeignLanguageId).ToList();
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
                _db.ForeignLanguageAdmissions.FirstOrDefault(
                    a =>
                        (a.studentId == studentId) &&
                        (a.ForeignLanguageCompetitionGroupId == property.ForeignLanguageCompetitionGroupId) &&
                        (a.ForeignLanguageCompetitionGroup.Year == property.ForeignLanguageCompetitionGroup.Year) &&
                        (a.ForeignLanguageCompetitionGroup.SemesterId ==
                         property.ForeignLanguageCompetitionGroup.SemesterId) &&
                         a.ForeignLanguageId == property.ForeignLanguageId);
            if (admission == null)
            {
                admission = new ForeignLanguageAdmission
                {
                    studentId = studentId,
                    ForeignLanguageCompetitionGroupId = property.ForeignLanguageCompetitionGroupId,
                    Status = status,
                    ForeignLanguageId = property.ForeignLanguageId,
                    Published = false
                };
                dismissOtherAdmissionStatus(status);

                _db.ForeignLanguageAdmissions.Add(admission);
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
  
            var admissionCount = _db.ForeignLanguageAdmissions.Count(
                a =>
                    (a.ForeignLanguageCompetitionGroupId == property.ForeignLanguageCompetitionGroupId) &&
                    (a.Status == AdmissionStatus.Admitted) && (a.ForeignLanguageId == property.ForeignLanguageId));
            if ((status == AdmissionStatus.Admitted) && (admissionCount > property.Limit))
                overlimit = true;
            return overlimit;
        }
        void CleanSubgroupMembership(ForeignLanguageAdmission foreignLanguageAdmission)
        {
            var subgrougMemberships =
                  _db.ForeignLanguageSubgroupMemberships.Where(
                      _ =>
                          _.studentId == foreignLanguageAdmission.studentId &&
                          _.Subgroup.Meta.CompetitionGroupId == foreignLanguageAdmission.ForeignLanguageCompetitionGroupId);

            if (subgrougMemberships != null)
            {
                foreach (var s in subgrougMemberships) _db.ForeignLanguageSubgroupMemberships.Remove(s);
                //subgrougMemberships.ForEach(flsm => _db.ForeignLanguageSubgroupMemberships.Remove(flsm));
            }

            _db.SaveChanges();
        }

        public void PublishCompetitionGroupAdmission(string[] studentId, int propertyId)
        {
            foreach (var Id in studentId)
            {
                var property = _db.ForeignLanguageProperties.FirstOrDefault(p => p.Id == propertyId);

                if (property != null && _db.IsForeignLanguageAccessible(User, property.ForeignLanguageId))
                {
                    var admission =
                        _db.ForeignLanguageAdmissions.Where(
                                a =>
                                    (a.studentId == Id) &&
                                    (a.ForeignLanguageCompetitionGroupId == property.ForeignLanguageCompetitionGroupId) &&
                                    a.ForeignLanguageId == property.ForeignLanguageId)
                            .FirstOrDefault();

                    if (admission == null)
                        continue;

                    admission.Published = true;
                    _db.SaveChanges();
                }

            }
        }

       [Authorize(Roles = ItsRoles.ForeignLanguageMassPublishAdmissions)]
        public ActionResult PublishCompetitionGroupAdmissions(int competitionGroupId)
        {
            foreach (var s in _db.ForeignLanguageAdmissions.Where(m => m.ForeignLanguageCompetitionGroupId == competitionGroupId)) s.Published = true;
/*            _db.ForeignLanguageAdmissions.Where(m => m.ForeignLanguageCompetitionGroupId == competitionGroupId).ForEach(m =>
            {
                m.Published = true;
            });*/

            _db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult PrepareAuto(int competitionGroupId)
        {
            var group = _db.ForeignLanguageCompetitionGroups.Find(competitionGroupId);

            ViewBag.AnyPrepared =
                _db.ForeignLanguageAdmissions.Any(
                    va =>
                        (va.ForeignLanguageCompetitionGroupId == competitionGroupId) &&
                        (va.Status != AdmissionStatus.Indeterminate));
            ViewBag.AnyPublished =
                _db.ForeignLanguageAdmissions.Any(
                    va =>
                        (va.ForeignLanguageCompetitionGroupId == competitionGroupId) &&
                        (va.Status != AdmissionStatus.Indeterminate) && va.Published);
            ViewBag.AnyWithoutRating =
                _db.ForeignLanguageStudentSelectionPriorities.Any(
                    s => (s.competitionGroupId == competitionGroupId) && (s.Student.Rating == null));
            return
                View(new MinorAutoVM
                {
                    CompetitionGroup = group.Name,
                    Semester = group.Semester.Name,
                    SemesterId = group.Id,
                    Year = group.Year,
                    StudentCount =
                        _db.ForeignLanguageStudentSelectionPriorities.Where(
                                s => s.competitionGroupId == competitionGroupId)
                            .Select(s => s.Student)
                            .Distinct()
                            .Count(),
                    AdmittedCount = _db.ForeignLanguageAdmissions.Where(
                            s =>
                                (s.ForeignLanguageCompetitionGroupId == competitionGroupId) &&
                                (s.Status == AdmissionStatus.Admitted))
                        .Select(s => s.studentId)
                        .Distinct()
                        .Count(),
                    MinorCount =
                        _db.ForeignLanguageProperties.Count(
                            m => m.ForeignLanguageCompetitionGroupId == competitionGroupId)
                });
        }

        [HttpPost]
        public ActionResult CalculateAutoAdmissions(int competitionGroupId)
        {
            _sdl = new StudentDesicionList();

            var group = _db.ForeignLanguageCompetitionGroups.Find(competitionGroupId);

            var year = group.Year;
            var semesterId = group.SemesterId;

            var vm = new MinorAutoVM {Semester = group.Name, Year = year, SemesterId = semesterId};

            Logger.Info("Расчёт зачислений ИЯ {0} {1} {2}", year, semesterId, group.Name);

            //Поиск студентов, приоритеты которых рассматривать не нужно, потому что они уже зачислены в ИЯ
            var alreadyAdmitted = new HashSet<string>(_db.ForeignLanguageAdmissions.Where(
                    s =>
                        (s.ForeignLanguageCompetitionGroupId == competitionGroupId) &&
                        (s.Status == AdmissionStatus.Admitted))
                .Select(s => s.studentId).Distinct());

            //Выбор всех секций этого периода
            //var minors = db.ForeignLanguageProperties.Where(m => m.ForeignLanguageCompetitionGroupId==competitionGroupId).ToList();

            //Колчество мест в каждом из майноров в текущем периоде
            //Dictionary<string, int> placesLeft = new Dictionary<string, int>();
            //foreach (var minor in minors)
            //{
            //    placesLeft[minor.ForeignLanguageId] = minor.Limit;
            //    _sdl.AddNew("всего мест", minor.ForeignLanguageId, minor.Limit.ToString(), null);
            //}

            //Существующие зачисления вычитаются из доступных для зачисления мест
            //var existingAdmissions =
            //    db.ForeignLanguageAdmissions.Where(s => s.ForeignLanguageCompetitionGroupId==competitionGroupId && s.Status == AdmissionStatus.Admitted)
            //        .ToList();
            //foreach (var ea in existingAdmissions)
            //{
            //    if (placesLeft.ContainsKey(ea.ForeignLanguageId))
            //    {
            //        placesLeft[ea.ForeignLanguageId]--;
            //        _sdl.AddNew("место уже занято студентом", ea.ForeignLanguageId, ea.studentId, null);
            //    }
            //}

            //Рассматриваются пожелания всех студентов по текущему периоду
            var wishes = _db.ForeignLanguageStudentSelectionPriorities
                .Where(s => s.competitionGroupId == competitionGroupId)
                .Select(s => new
                {
                    date =
                    _db.ForeignLanguageStudentSelectionPriorities.Where(
                            sx => (sx.studentId == s.studentId) && (sx.competitionGroupId == competitionGroupId))
                        .Max(sx => sx.modified),
                    s.Student.Rating,
                    s
                })
                .OrderBy(s => s.date)
                .ThenByDescending(s => s.Rating)
                .Select(s => s.s)
                .ToList();

            foreach (var wish in wishes)
            {
                var studentId = wish.studentId;
                var sectionId = wish.sectionId;

                if (alreadyAdmitted.Contains(studentId))
                    continue;
                //if (!placesLeft.ContainsKey(sectionId))
                //{
                //    _sdl.AddNew("у модуля нет периода", sectionId, studentId, priority);
                //    continue;
                //}
                //if (placesLeft[sectionId] <= 0)
                //{
                //    _sdl.AddNew("на модуле нет мест", sectionId, studentId, priority);
                //    continue;
                //}

                TryAutoAdmitt(competitionGroupId, studentId, sectionId, vm, alreadyAdmitted);
            }

            _db.SaveChanges();

            //var unadmitted =
            //    db.Students.OnlyActive().Where(s => s.Group.ForeignLanguageCompetitionGroups.Any(cg => cg.Id == competitionGroupId) && ! s.Sportsman)
            //    .Where(s=>!db.ForeignLanguageAdmissions.Any(a=>a.studentId==s.Id && a.Status == AdmissionStatus.Admitted && a.ForeignLanguageCompetitionGroupId == competitionGroupId)).
            //    OrderByDescending(s=>s.Rating)
            //    .Select(s=>s.Id).ToList();

            //foreach (var studentId in unadmitted)
            //{
            //    if(placesLeft.All(p=>p.Value<=0))
            //        break;
            //    var sectionId = placesLeft.Where(p=>p.Value>0).Select(p=>p.Key).First();

            //    _sdl.AddNew("у студента нет выполнимых приоритетов", sectionId, studentId, -1);
            //    TryAutoAdmitt(competitionGroupId, studentId, sectionId, -1, vm, placesLeft, alreadyAdmitted);
            //}


            Logger.Info("Расчёт зачислений ИЯ окончен {0} {1}", year, group.Name);

            _db.SaveChanges();

            _db.MinorAutoAdmissionReports.Add(new MinorAutoAdmissionReport
            {
                Content = _sdl.GetStringBuilder().ToString(),
                Date = DateTime.Now,
                ModuleType = ModuleType.ForeignLanguage
            });

            _db.SaveChanges();
            return View(vm);
        }

        private void TryAutoAdmitt(int competitionGroupId, string studentId, string sectionId, MinorAutoVM vm,
            HashSet<string> alreadyAdmitted)
        {
// поиск зачислений для текущего студента
            var adm = _db.ForeignLanguageAdmissions
                .FirstOrDefault(
                    ma => (ma.studentId == studentId) && (ma.ForeignLanguageCompetitionGroupId == competitionGroupId));

            if (adm == null)
            {
                _sdl.AddNew("создаётся зачисление", sectionId, studentId, null);
                //зачисления ещё не было - создаём его
                _db.ForeignLanguageAdmissions.Add(new ForeignLanguageAdmission
                {
                    studentId = studentId,
                    ForeignLanguageCompetitionGroupId = competitionGroupId,
                    ForeignLanguageId = sectionId,
                    Published = false,
                    Status = AdmissionStatus.Admitted
                });
                vm.AdmittedCount++;
                alreadyAdmitted.Add(studentId);
            }
            else
            {
                // если модуль ИЯ в заявке и в зачислении разный, то игнорируем этого студента
                if (adm.ForeignLanguageId == sectionId)
                {
                    //если в зачислении было отказано ранее - не рассматриваем более заявку студента на этот ИЯ
                    _sdl.AddNew("перевод неопределённого статуса в зачисленный", sectionId, studentId, null);
                    if (adm.Status == AdmissionStatus.Indeterminate)
                    {
                        adm.Status = AdmissionStatus.Admitted;
                        adm.Published = false;
                        adm.ForeignLanguageId = sectionId;
                        vm.AdmittedCount++;
                    }
                    else
                    {
                        _sdl.AddNew("у студента уже есть отрицательное решение", sectionId, studentId, null);
                        return;
                    }
                    alreadyAdmitted.Add(studentId);
                }
                else
                {
                    _sdl.AddNew("у студента выбран другой модуль в ЛК", sectionId, studentId, null);
                }
            }

            //alreadyAdmitted.Add(studentId);
            //placesLeft[sectionId]--;
        }


        public ActionResult DownloadAutoAdmissionReport()
        {
            Response.Headers.Add("Content-Disposition",
                "attachment; filename=Отчёт_по_автозачислениям_на_модули_ИЯ.txt");
            if (_sdl == null)
                _sdl = new StudentDesicionList();
            return File(_sdl.FormatStream(), "application/text");
        }


        private IQueryable<object> PrepareReport(string sort, string filter,
            Expression<Func<ForeignLanguageAdmission, bool>> admissionFilter)
        {
            var expression = admissionFilter ?? (x => true);
            _db.Database.SetCommandTimeout(90);
            
            var periods =
                _db.ForeignLanguageProperties
                    .Select(
                        p =>
                            new
                            {
                                p,
                                c =
                                _db.ForeignLanguageAdmissions
                                    .Where(expression)
                                    .Where(
                                        ma =>
                                            (ma.ForeignLanguageCompetitionGroupId ==
                                             p.ForeignLanguageCompetitionGroupId) &&
                                            (ma.ForeignLanguageId == p.ForeignLanguageId) &&
                                            (ma.Status == AdmissionStatus.Admitted))
                                .Select(ma => ma.Student)
                            })
                    .Where(r => r.c.Any());

            var reportVms = periods.SelectMany(p => p.c
                .Select(s => new
                {
                    moduleName = p.p.ForeignLanguage.Module.title,
                    CompetitionGroupName = p.p.ForeignLanguageCompetitionGroup.Name,
                    p.p.ForeignLanguageCompetitionGroup.Year,
                    Semester = p.p.ForeignLanguageCompetitionGroup.Semester.Name,
                    semesterId = p.p.ForeignLanguageCompetitionGroup.SemesterId,
                    MinStudentsCount = 0,
                    MaxStudentsCount = p.p.Limit,
                    s.Id,
                    GroupName = _db.GroupsHistories.FirstOrDefault(g=> g.GroupId == s.GroupId && g.YearHistory == p.p.ForeignLanguageCompetitionGroup.Year ).Name,
                    s.Person.Name,
                    s.Person.Surname,
                    s.Person.PatronymicName,
                    s.Rating,
                    s.IsTarget,
                    s.IsInternational,
                    s.Compensation,
                    s.ForeignLanguageLevel,
                    SubgroupTeachers = _db.ForeignLanguageSubgroupMemberships.Where(_ => _.Subgroup.Meta.CompetitionGroupId == p.p.ForeignLanguageCompetitionGroupId && _.studentId == s.Id && _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguageId == p.p.ForeignLanguageId).Select(_ => _.Subgroup.Teacher.initials).Distinct(),

                    Teachers = p.p.Teachers.Select(t => t.initials)
                }));


            if (sort != null)
                reportVms = reportVms.OrderBy(SortRules.Deserialize(sort).FirstOrDefault());
            else
                reportVms =
                    reportVms.OrderBy(r => r.CompetitionGroupName)
                        .ThenBy(r => r.moduleName)
                        .ThenBy(r => r.Surname)
                        .ThenBy(r => r.Name);


            reportVms = reportVms.Where(FilterRules.Deserialize(filter));

            return reportVms;
        }

        private IEnumerable<Tuple<string, int, string>> PrepareReportFormativeDivisions(string filter)
        {
            var periods =
                _db.ForeignLanguageProperties
                    .Select(
                        p =>
                            new
                            {
                                p,
                                c =
                                _db.ForeignLanguageAdmissions
                                    .Where(
                                        ma =>
                                            (ma.ForeignLanguageCompetitionGroupId ==
                                             p.ForeignLanguageCompetitionGroupId) &&
                                            (ma.ForeignLanguageId == p.ForeignLanguageId) &&
                                            (ma.Status == AdmissionStatus.Admitted))
                            })
                    .Where(r => r.c.Any());

            var reportVms = periods.SelectMany(p => p.c
                .Select(s => new
                {
                    moduleName = p.p.ForeignLanguage.Module.title,
                    CompetitionGroupName = p.p.ForeignLanguageCompetitionGroup.Name,
                    p.p.ForeignLanguageCompetitionGroup.Year,
                    Semester = p.p.ForeignLanguageCompetitionGroup.Semester.Name,
                    semesterId = p.p.ForeignLanguageCompetitionGroup.SemesterId,
                    MinStudentsCount = 0,
                    MaxStudentsCount = p.p.Limit,
                    s.Student.Id,
                    GroupName = _db.GroupsHistories.FirstOrDefault(g => g.GroupId == s.Student.GroupId && g.YearHistory == p.p.ForeignLanguageCompetitionGroup.Year).Name,
                    s.Student.Person.Name,
                    s.Student.Person.Surname,
                    s.Student.Person.PatronymicName,
                    s.Student.Rating,
                    s.Student.IsTarget,
                    s.Student.IsInternational,
                    s.Student.Compensation,
                    Teachers = p.p.Teachers.Select(t => t.initials),
                    Places = "",
                    s.Student.Group.FormativeDivisionId
                }));

            reportVms = reportVms.Where(FilterRules.Deserialize(filter));
            var otherVMs = reportVms.Select(x => new {x.Semester, x.Year, x.FormativeDivisionId}).Distinct();

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
            return View();
        }

        public ActionResult DownloadReport(string filter)
        {
            var reportVms = PrepareReport(null, filter, null);
            var stream = new VariantExport().Export(new {Rows = reportVms}, "ForeignLanguageReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Отчёт по ИЯ.xlsx".ToDownloadFileName());
        }


        private Stream PrepareDivReportStream(string formativeDivisionId, int year, string semesterName, string filter,
            // ReSharper disable once UnusedParameter.Local
            string sort)
        {
            var reportVms = PrepareReport(null, filter,
                x =>
                    (x.ForeignLanguageCompetitionGroup.Year == year) &&
                    (x.ForeignLanguageCompetitionGroup.Semester.Name == semesterName) &&
                    (x.Student.Group.FormativeDivisionId == formativeDivisionId));
            var stream = new VariantExport().Export(new {Rows = reportVms.ToList()},
                "ForeignLanguageDivisionsReportTemplate.xlsx");
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
                    FileDownloadName = "ИЯ по подразделениям.zip".ToDownloadFileName()
                };
            }
        }
    }
}