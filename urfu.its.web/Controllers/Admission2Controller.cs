using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Ajax.Utilities;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;
using PagedList.Core;

namespace Urfu.Its.Web.Controllers
{
    [Authorize]
    //TODO: Rename to MinorAdmissions or smth
    public class Admission2Controller : BaseController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = ItsRoles.MinorViewAdmission)]
        public ActionResult DownloadMinorModulesReport(string filter)
        {
            var modules = GetFilteredMinorModules(string.Empty, filter);
            var stream = new VariantExport().Export(new
            {
                Rows = modules
            }, "minorModulesReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Отчёт зачисление на майноры.xlsx".ToDownloadFileName());
        }

        [Authorize(Roles = ItsRoles.MinorViewAdmission)]
        public ActionResult Minors(int? page, int? limit, string sort, string filter)
        {

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                var modules = GetFilteredMinorModules(sort, filter);

                //.OrderBy(m => m.coordinator)
                //.ThenBy(m => m.number)
                //.ThenBy(m => m.title)
                //.ToList()
                //.Select(m => new ModuleViewModel(m.module, m.directions))


                var paginated = modules.ToPagedList(page ?? 1, limit ?? 25);

                return JsonNet(new
                {
                    data = paginated,
                    total = modules.Count()
                });
            }
            //ViewBag.IsAdmin = HttpContext.GetOwinContext().Authentication.User.IsInRole(ItsRoles.MinorMassPublishAdmissions) ;
            ViewBag.IsInMassPublishRole = User.IsInRole(ItsRoles.MinorMassPublishAdmissions);
            return View();
        }

        private IQueryable<object> GetFilteredMinorModules(string sort, string filter)
        {
            var filterRules = FilterRules.Deserialize(filter);
            var sortRules = SortRules.Deserialize(sort);
            if (sortRules.Count == 0)
            {
                sortRules.Add(new SortRule()
                {
                    Property = "shortTitle",
                    Direction = SortDirection.Ascending
                });
            }
            // title = p.Minor.Module.shortTitle,
            var modules = db.MinorPeriods.Include("Minor.Module")
                .Select(p => new
                {
                    p.Id,
                    p.Minor.Module.number,
                    p.Minor.Module.shortTitle,
                    typeId = p.Minor.Tech.Id,
                    type = p.Minor.Tech.Name,
                    p.Year,
                    semester = p.Semester.Name,
                    p.SemesterId,
                    p.Minor.Module.coordinator,
                    p.Minor.Module.testUnits,
                    p.MaxStudentsCount,
                    selection = db.StudentSelectionMinorPriority.Where(s => s.minorPeriodId == p.Id).Count(),
                    addmission =
                    db.MinorAdmissions.Where(a => a.minorPeriodId == p.Id && a.Status == AdmissionStatus.Admitted).Count()
                })
                .Where(filterRules)
                .OrderBy(sortRules.FirstOrDefault(), m => m.number);
            return modules;
        }
        // GET: Получаем Ajax-запросом типы майноры для комбобокса
        public ActionResult MinorTypes()
        {
            var minorTypes = db.ModuleTeches.Select(s => new { Id = s.Id.ToString(), Name = s.Name });

            var json = Json(
                new
                {
                    data = minorTypes
                },
                new Newtonsoft.Json.JsonSerializerSettings()
            );

            return json;
        }

        //public ActionResult Minors2(int? page, int? limit, string sort, string filter)
        //{
        //    if (Request.IsAjaxRequest())
        //    {
        //        var filterRules = FilterRules.Deserialize(filter);
        //        var sortRules = SortRules.Deserialize(sort);

        //        var filterYear = filterRules?.Find(f => f.Property == "year");
        //        var filterSemester = filterRules?.Find(f => f.Property == "semester");

        //        if (filterRules != null)
        //        {
        //            filterRules.Remove(filterSemester);
        //            filterRules.Remove(filterYear);
        //        }

        //        var query = FilterPeriod(filterYear, filterSemester);
        //        query = query.Where(filterRules);
        //        query = query.OrderBy(sortRules.FirstOrDefault(), m => m.number);

        //        //var dx = db.DirectionsForUser(User);
        //        var modules = query.Select(m => new { Id = m.uuid, m.number, m.shortTitle, m.coordinator, m.testUnits });
        //        //.OrderBy(m => m.coordinator)
        //        //.ThenBy(m => m.number)
        //        //.ThenBy(m => m.title)
        //        //.ToList()
        //        //.Select(m => new ModuleViewModel(m.module, m.directions))


        //        var paginated = modules.ToPagedList(page ?? 1, limit ?? 25);

        //        return JsonNet(new
        //        {
        //            data = paginated,
        //            total = modules.Count()
        //        });
        //    }

        //    return View();
        //}

        //private IQueryable<Module> FilterPeriod(FilterRule filterYear, FilterRule filterSemester)
        //{
        //    int year;
        //    int semester;

        //    var haveYear = int.TryParse(filterYear?.Value, out year);
        //    var haveSemester = int.TryParse(filterSemester?.Value, out semester);

        //    if (haveYear && haveSemester)
        //        return db.MinorsForUser(User).Where(m => m.Minor.Periods.Any(p => p.Year == year && p.SemesterId == semester));

        //    if (haveYear)
        //        return db.MinorsForUser(User).Where(m => m.Minor.Periods.Any(p => p.Year == year));

        //    if (haveSemester)
        //        return db.MinorsForUser(User).Where(m => m.Minor.Periods.Any(p => p.SemesterId == semester));

        //    return db.MinorsForUser(User);
        //}

        [Authorize(Roles = ItsRoles.MinorViewAdmission)]
        public ActionResult DownloadMinorStudentsReport(int id, string filter)
        {
            var filterRules = FilterRules.Deserialize(filter);
            var priorityRule = filterRules.FirstOrDefault(r => r.Property == "IsPriority");
            if (priorityRule != null)
                priorityRule.Value = "True";

            else
                filterRules.Add(new FilterRule()
                {
                    Property = "IsPriority",
                    Value = "True"
                });

            var period = db.MinorPeriods.Find(id);
            db.Semesters.Load();
            db.Minors.Load();
            db.UniModules().Load();
            var minorStudents = GetFilteredMinorStudents(id, new SortRules(), filterRules);
            var stream = new VariantExport().Export(new
            {
                Rows = minorStudents
            }, "minorStudentsReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Год {period.Year}/Семестр {period.Semester.Name} - Зачисление на майнор \"{period.Minor.Module.shortTitle}\".xlsx".ToDownloadFileName());
        }

        [Authorize(Roles = ItsRoles.MinorViewAdmission)]
        public ActionResult MinorStudents(int id, int? page, int? limit, string sort, string filter)
        {
            var period = db.MinorPeriods.Find(id);

            ViewBag.CanEdit = User.IsInRole(ItsRoles.MinorAutoAdmission) && db.IsMinorAccessible(User, period.ModuleId) ? "true" : "false";

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                ViewBag.Title = $"Год {period.Year}/Семестр {period.Semester.Name} - Зачисление на майнор \"{period.Minor.Module.shortTitle}\"";
                ViewBag.MinorPeriodId = id;
                return View();
            }
            var sortRules = SortRules.Deserialize(sort);
            var filterRules = FilterRules.Deserialize(filter);

            var students = GetFilteredMinorStudents(id, sortRules, filterRules);
            //var first = students.First();

            var paginated = students.ToPagedList(page ?? 1, limit ?? 25);
            return JsonNet(new
            {
                data = paginated,
                total = students.Count()
            });

        }

        private IQueryable<Object> GetFilteredMinorStudents(int id, SortRules sortRules, FilterRules filterRules)
        {
            MinorPeriod period = db.MinorPeriods.Find(id);
            var directions = db.Directions.Where(d => d.Modules.Any(m => m.uuid == period.Minor.Module.uuid));

            var students = db.Students.Include("MinorSelections").Include("MinorAdmissions")
                .Where(s => directions.Any(d => d.uid == s.Group.Profile.Direction.uid))
                .Select(s => new
                {
                    s.Id,
                    GroupName = s.Group.Name,
                    Surname = s.Person.Surname,
                    Name = s.Person.Name,
                    PatronymicName = s.Person.PatronymicName,
                    Rating = s.Rating,
                    IsTarget = s.IsTarget,
                    IsInternational = s.IsInternational,
                    Compensation = s.Compensation,
                    s.PersonalNumber,
                    Priority =
                    db.StudentSelectionMinorPriority
                        .Where(p => p.studentId == s.Id && p.minorPeriodId == id)
                        .Select(p => p.priority)
                        .FirstOrDefault(),
                    StudentStatus = s.Status,
                    Status = db.MinorAdmissions
                        .Where(a => a.studentId == s.Id && a.minorPeriodId == id)
                        .Select(a => a.Status)
                        .FirstOrDefault(),
                    IsPriority = db.StudentSelectionMinorPriority
                        .Where(p => p.studentId == s.Id && p.minorPeriodId == id)
                        .Any(),
                    Published = s.MinorAdmissions.FirstOrDefault(_ => _.minorPeriodId == id) != null && s.MinorAdmissions.FirstOrDefault(_ => _.minorPeriodId == id).Published

                });

            students = students.OrderBy(sortRules.FirstOrDefault(), v => v.GroupName);

            students = students.Where(filterRules);
            return students;
        }

        [Authorize(Roles = ItsRoles.MinorAutoAdmission)]
        public ActionResult SetMinorAdmissionStatus(string[] studentIds, int minorPeriodId, AdmissionStatus status)
        {
            var minorID = db.MinorPeriods.Where(p => p.Id == minorPeriodId).Select(p => p.ModuleId).Single();
            if (db.IsMinorAccessible(User, minorID))
            {
                foreach (var st in studentIds) SetMinorAdmissionStatusInternal(st, minorPeriodId, status);
               // studentIds.ForEach(_ => SetMinorAdmissionStatusInternal(_, minorPeriodId, status));
            }
            //Task.Run(() => RunpAdmissionsController.QueueStudentAdmission(studentId));
            return Json(new { reload = false });
        }

        private void SetMinorAdmissionStatusInternal(string studentId, int minorPeriodId, AdmissionStatus status)
        {
            var admission =
                db.MinorAdmissions.Where(a => a.studentId == studentId && a.minorPeriodId == minorPeriodId).FirstOrDefault();
            if (admission == null)
            {
                admission = new MinorAdmission
                {
                    studentId = studentId,
                    minorPeriodId = minorPeriodId,
                    Status = status,
                    Published = false
                };

                db.MinorAdmissions.Add(admission);
            }
            else
            {
                admission.Status = status;
                admission.Published = false;
                if (status == AdmissionStatus.Indeterminate || status == AdmissionStatus.Denied)
                {
                    var minorSubgroupMembership = db.MinorSubgroupMemberships.Where(_ => _.studentId == studentId);
                    db.MinorSubgroupMemberships.RemoveRange(minorSubgroupMembership);
                }
            }
            db.SaveChanges();
        }

        [Authorize(Roles = ItsRoles.MinorAutoAdmission)]
        public void PublishMinorAdmission(string studentId, int minorPeriodId)
        {
            var minorID = db.MinorPeriods.Where(p => p.Id == minorPeriodId).Select(p => p.ModuleId).Single();
            if (db.IsMinorAccessible(User, minorID))
            {
                var admission = db.MinorAdmissions.Where(a => a.studentId == studentId && a.minorPeriodId == minorPeriodId).FirstOrDefault();
                if (admission == null)
                {
                    return;
                }

                admission.Published = true;
                db.SaveChanges();
            }
            //AdmissionsController.QueuePublishedAdmissions(studentId);
        }

        [Authorize(Roles = ItsRoles.MinorReport)]
        public ActionResult MinorsReport(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                var reportVms = PrepareMinorsReport(sort, filter);
                return JsonNet(reportVms);
            }
            return View();
        }

        [Authorize(Roles = ItsRoles.MinorReport)]
        public ActionResult DownloadMinorsReport(string filter)
        {
            var reportVms = PrepareMinorsReport(null, filter);
            var stream = new VariantExport().Export(new { Rows = reportVms }, "minorsReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Отчёт по майнорам.xlsx".ToDownloadFileName());
        }

        private IQueryable<object> PrepareMinorsReport(string sort, string filter)
        {
            var periods =
                db.MinorPeriods.Include("Minor.Module.Directions")
                  .Select(
                      p =>
                          new
                          {
                              p,
                              c =
                                  db.MinorAdmissions
                                  .Where(ma => ma.minorPeriodId == p.Id && ma.Status == AdmissionStatus.Admitted)
                              //.Select(ma => ma.Student)
                          })
                  .Where(r => r.c.Any());

            var reportVms = periods.SelectMany(p => p.c
                .Select(s => new
                {
                    moduleId = p.p.Minor.Module.uuid,
                    number = p.p.Minor.Module.number,
                    numberAndTitle = p.p.Minor.Module.number + " " + p.p.Minor.Module.title,
                    Year = p.p.Year,
                    Semester = p.p.Semester.Name,
                    semesterId = p.p.Semester.Id,
                    MinStudentsCount = p.p.MinStudentsCount,
                    MaxStudentsCount = p.p.MaxStudentsCount,
                    Id = s.Student.Id,
                    GroupName = s.Student.Group.Name,
                    Name = s.Student.Person.Name,
                    Surname = s.Student.Person.Surname,
                    PatronymicName = s.Student.Person.PatronymicName,
                    Rating = s.Student.Rating,
                    IsTarget = s.Student.IsTarget,
                    IsInternational = s.Student.IsInternational,
                    Compensation = s.Student.Compensation,
                    //p.p.Minor.Module.specialities
                    db.MinorSubgroupMemberships.FirstOrDefault(m => m.studentId == s.studentId && m.Subgroup.Meta.MinorPeriodId == s.minorPeriodId && m.Score.HasValue).Mark,

                    db.MinorSubgroupMemberships.FirstOrDefault(m => m.studentId == s.studentId && m.Subgroup.Meta.MinorPeriodId == s.minorPeriodId && m.Score.HasValue).Score

                }));


            reportVms = reportVms.OrderBy(SortRules.Deserialize(sort).FirstOrDefault());
            reportVms = reportVms.Where(FilterRules.Deserialize(filter));

            return reportVms;
        }
        [Authorize(Roles = ItsRoles.MinorReport)]
        public ActionResult GetSpecialities(string moduleId)
        {
            var spec = db.UniModules().FirstOrDefault(_ => _.uuid == moduleId).specialities.Split(' ').OrderBy(_ => _);
            var specialities = String.Join(" ", spec);
            return JsonNet(new { specialities });
        }

        [Authorize(Roles = ItsRoles.MinorAutoAdmission)]
        public ActionResult PrepareAuto(int year, int semester)
        {
            var sem = db.Semesters.Find(semester);

            ViewBag.AnyPrepared = db.MinorAdmissions.Any(va => va.MinorPeriod.SemesterId == semester && va.MinorPeriod.Year == year && va.Status != AdmissionStatus.Indeterminate);
            ViewBag.AnyPublished = db.MinorAdmissions.Any(va => va.MinorPeriod.SemesterId == semester && va.MinorPeriod.Year == year && va.Status != AdmissionStatus.Indeterminate && va.Published);
            ViewBag.AnyWithoutRating = db.StudentSelectionMinorPriority.Any(s => s.MinorPeriod.SemesterId == semester && s.MinorPeriod.Year == year && s.Student.Rating == null);
            return
                View(new MinorAutoVM
                {
                    Semester = sem.Name,
                    SemesterId = sem.Id,
                    Year = year,
                    StudentCount =
                        db.StudentSelectionMinorPriority.Where(
                            s => s.MinorPeriod.SemesterId == semester && s.MinorPeriod.Year == year)
                            .Select(s => s.Student)
                            .Distinct()
                            .Count(),
                    AdmittedCount = db.MinorAdmissions.Where(
                            s => s.MinorPeriod.SemesterId == semester && s.MinorPeriod.Year == year && s.Status == AdmissionStatus.Admitted)
                            .Select(s => s.studentId)
                            .Distinct()
                            .Count(),
                    MinorCount = db.Minors.Count(m => m.Periods.Any(p => p.Year == year && p.SemesterId == semester))
                });
        }

        static StudentDesicionList _sdl;

        [HttpPost]
        [Authorize(Roles = ItsRoles.MinorAutoAdmission)]
        public ActionResult CalculateAutoAdmissions(int year, int semesterid)
        {

            _sdl = new StudentDesicionList();
            var sem = db.Semesters.Find(semesterid);
            var vm = new MinorAutoVM { Semester = sem.Name, Year = year, SemesterId = semesterid };

            Logger.Info("Расчёт зачислений майноров {0} {1}", year, sem.Name);

            //Поиск студентов, приоритеты которых рассматривать не нужно, потому что они уже зачислены в майнор этого периода
            HashSet<string> alreadyAdmitted = new HashSet<string>(db.MinorAdmissions.Where(
                s => s.MinorPeriod.SemesterId == semesterid && s.MinorPeriod.Year == year && s.Status == AdmissionStatus.Admitted)
                .Select(s => s.studentId).Distinct());

            //Выбор всех майноров этого периода
            var minors = db.Minors.SelectMany(m => m.Periods.Where(p => p.Year == year && p.SemesterId == semesterid)).ToList();

            //Колчество мест в каждом из майноров в текущем периоде
            Dictionary<string, int> placesLeft = new Dictionary<string, int>();
            foreach (var minor in minors)
            {
                placesLeft[minor.ModuleId] = minor.MaxStudentsCount;
                _sdl.AddNew("всего мест", minor.ModuleId, minor.MaxStudentsCount.ToString(), null);
            }

            //Существующие зачисления вычитаются из доступных для зачисления мест
            var existingAdmissions =
                db.MinorAdmissions.Where(s => s.MinorPeriod.SemesterId == semesterid && s.MinorPeriod.Year == year && s.Status == AdmissionStatus.Admitted).Include(ma => ma.MinorPeriod)
                    .ToList();
            foreach (var ea in existingAdmissions)
            {
                if (placesLeft.ContainsKey(ea.MinorPeriod.ModuleId))
                {
                    placesLeft[ea.MinorPeriod.ModuleId]--;
                    _sdl.AddNew("студент уже зачислен на майнор в этом периоде", ea.MinorPeriod.ModuleId, ea.studentId, null);
                }
            }

            //Рассматриваются пожелания всех студентов по текущему периоду
            var wishes = db.StudentSelectionMinorPriority
                .Where(s => s.MinorPeriod.SemesterId == semesterid && s.MinorPeriod.Year == year && s.priority != 0)
                .OrderByDescending(s => s.Student.IsTarget)
                //.ThenByDescending(s => s.Student.IsInternational)
                //.ThenByDescending(s => s.Student.Compensation == "контракт")
                .ThenByDescending(s => s.Student.Rating)
                .ThenBy(s => s.priority)
                .ToList();

            foreach (var wish in wishes)
            {
                if (alreadyAdmitted.Contains(wish.studentId))
                {
                    //_sdl.AddNew("у студента уже есть зачисления", wish.MinorPeriod.ModuleId, wish.studentId, wish.priority);
                    continue;
                }
                if (!placesLeft.ContainsKey(wish.MinorPeriod.ModuleId))
                {
                    _sdl.AddNew("у майнора нет периода", wish.MinorPeriod.ModuleId, wish.studentId, wish.priority);
                    continue;
                }
                if (placesLeft[wish.MinorPeriod.ModuleId] <= 0)
                {
                    _sdl.AddNew("все места на майноре уже заняты или лимит равен 0", wish.MinorPeriod.ModuleId, wish.studentId, wish.priority);
                    continue;
                }

                // поиск зачислений для текущего студента
                var adm = db.MinorAdmissions
                    .FirstOrDefault(ma => ma.studentId == wish.studentId && ma.minorPeriodId == wish.minorPeriodId);
                if (adm == null)
                {
                    // поиск зачислений текущего студента на желаемого майнора в ранние периоды
                    var adm_currentMinor = db.MinorAdmissions.FirstOrDefault(ma => ma.studentId == wish.studentId
                        && ma.MinorPeriod.ModuleId == wish.MinorPeriod.ModuleId && ma.Status == AdmissionStatus.Admitted);

                    if (adm_currentMinor == null)
                    {
                        _sdl.AddNew("студент зачислен на майнор", wish.MinorPeriod.ModuleId, wish.studentId, wish.priority);
                        //зачисления ещё не было - создаём его
                        db.MinorAdmissions.Add(new MinorAdmission()
                        {
                            studentId = wish.studentId,
                            minorPeriodId = wish.minorPeriodId,
                            Published = false,
                            Status = AdmissionStatus.Admitted
                        });
                        vm.AdmittedCount++;
                    }
                    else
                    {
                        _sdl.AddNew("студент уже прошел курс этого майнора", wish.MinorPeriod.ModuleId, wish.studentId, wish.priority);
                        continue;
                    }
                }
                else
                {
                    //если в зачислении было отказано ранее - не рассматриваем более заявку студента на этот майнор
                    if (adm.Status == AdmissionStatus.Indeterminate)
                    {
                        _sdl.AddNew("\"нет решения\" заменено на \"зачислен\"", wish.MinorPeriod.ModuleId, wish.studentId, wish.priority);
                        adm.Status = AdmissionStatus.Admitted;
                        adm.Published = false;
                        //placesLeft[wish.MinorPeriod.ModuleId]--;
                        vm.AdmittedCount++;
                    }
                    if (adm.Status == AdmissionStatus.Denied)
                    {
                        _sdl.AddNew("студент \"не зачиcлен\" на этот майнор ранее", wish.MinorPeriod.ModuleId, wish.studentId, wish.priority);
                        continue;
                    }
                    else
                    {
                        _sdl.AddNew("студент \"зачиcлен\" на этот майнор ранее", wish.MinorPeriod.ModuleId, wish.studentId, wish.priority);
                        continue;
                    }
                }

                alreadyAdmitted.Add(wish.studentId);
                placesLeft[wish.MinorPeriod.ModuleId]--;
            }

            db.SaveChanges();


            //список всех приоритетов, на майноры которых студенты зачислены не были
            var unadmitted = db.StudentSelectionMinorPriority
                .Where(s => s.MinorPeriod.SemesterId == semesterid && s.MinorPeriod.Year == year && s.priority != 0)
                .Where(s => !db.MinorAdmissions.Any(ma => ma.studentId == s.studentId && ma.minorPeriodId == s.minorPeriodId))
                .OrderBy(p => p.Student.Person.Surname)
                .ThenBy(p => p.Student.Person.Name)
                .ThenBy(p => p.studentId)
                .ThenBy(p => p.priority)
                .ToList();
            foreach (var wish in unadmitted)
            {
                _sdl.AddNew("студент не имеет зачислений на майноры в этом периоде", wish.MinorPeriod.ModuleId, wish.studentId, wish.priority);
            }

            _sdl.SetStudents(
                db.StudentSelectionMinorPriority.Select(s => s.Student)
                    .Distinct()
                    .Include(s => s.Person)
                    .Include(s => s.Group)
                    .ToDictionary(s => s.Id));
            _sdl.SetModules(db.Minors.Include(m => m.Module).ToDictionary(m => m.ModuleId));

            /*
            foreach (var wish in decline)
            {
                //_sdl.AddNew("формирование отказа по результату расчёта", wish.MinorPeriod.ModuleId, wish.studentId);
                db.MinorAdmissions.Add(new MinorAdmission()
                {
                    studentId = wish.studentId,
                    minorPeriodId = wish.minorPeriodId,
                    Published = false,
                    Status = AdmissionStatus.Denied
                });
            }*/

            Logger.Info("Расчёт зачислений майноров окончен {0} {1}", year, sem.Name);

            db.SaveChanges();

            db.MinorAutoAdmissionReports.Add(new MinorAutoAdmissionReport
            {
                Content = _sdl.GetStringBuilder().ToString(),
                Date = DateTime.Now,
                ModuleType = ModuleType.Minor
            });

            db.SaveChanges();
            return View(vm);
        }

        public ActionResult AutoAdmissionReports()
        {
            return View(db.MinorAutoAdmissionReports);
        }

        public ActionResult AutoAdmissionReport(int id)
        {
            var report = db.MinorAutoAdmissionReports.Find(id);
            var ms = new MemoryStream();
            using (var sw = new StreamWriter(ms, Encoding.UTF8))
            {
                sw.Write(report.Content);
            }
            return new FileContentResult(ms.ToArray(), "application/text") { FileDownloadName = id + ".txt" };
        }

        [Authorize(Roles = ItsRoles.MinorAutoAdmission)]
        public ActionResult DownloadAutoAdmissionReport()
        {
            Response.Headers.Add("Content-Disposition",
                "attachment; filename=Отчёт_по_автозачислениям_на_майноры.txt");
            if (_sdl == null)
                _sdl = new StudentDesicionList();
            return File(_sdl.FormatStream(), "application/text");
        }

        [Authorize(Roles = ItsRoles.MinorMassPublishAdmissions)]
        public ActionResult PublishMinorAdmissions(int year, int semester)
        {
            var minorPeriodIds = db.MinorPeriods.Where(m => m.Year == year && m.SemesterId == semester).Select(m => m.Id);
            foreach (var mp in db.MinorAdmissions.Where(m => minorPeriodIds.Contains(m.minorPeriodId))) { mp.Published = true; }
            /*db.MinorAdmissions.Where(m => minorPeriodIds.Contains(m.minorPeriodId)).ForEach(m =>
              {
                  m.Published = true;
              });
            */
            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

    }



    public class MinorReportVM
    {
        private readonly MinorPeriod _period;
        private readonly List<Student> _students;

        public MinorReportVM(MinorPeriod period, List<Student> students)
        {
            _period = period;
            _students = students.OrderByDescending(s => s.Rating).ThenBy(s => s.Person.Surname).ToList();
        }

        public int Year { get { return _period.Year; } }
        public string Semester { get { return _period.Semester.Name; } }

        public Module Module
        {
            get { return _period.Minor.Module; }
        }

        public List<Student> Students
        {
            get { return _students; }
        }

        public int LimitMin
        {
            get { return _period.MinStudentsCount; }
        }

        public int LimitMax
        {
            get { return _period.MaxStudentsCount; }
        }

    }

    class StudentDesicionList : List<StudentDecision>
    {
        public void AddNew(string decision, string module, string student, int? priority)
        {
            Add(new StudentDecision { studentId = student, decision = decision, minorId = module, Priority = priority });
        }

        public byte[] FormatStream()
        {
            var sb = GetStringBuilder();


            return GetBytes(sb);
        }

        private static byte[] GetBytes(StringBuilder sb)
        {
            var ms = new MemoryStream();
            using (var sw = new StreamWriter(ms, Encoding.UTF8))
            {
                sw.Write(sb);
                return ms.ToArray();
            }
        }

        StringBuilder sb;
        public StringBuilder GetStringBuilder()
        {

            sb = new StringBuilder();
            foreach (var s in this)
            {
                DumpInfo(s);
                sb.AppendLine();
            }
            return sb;
        }

        private void DumpInfo(StudentDecision s)
        {
            sb.Append(s.studentId);
            sb.Append(",\t");
            sb.Append(s.minorId);
            sb.Append(",\t");
            sb.Append(s.decision);
            sb.Append(",\t");
            sb.Append(s.Priority ?? 0);
            Minor minor = null;
            if (_minors?.TryGetValue(s.minorId, out minor) ?? false)
            {
                sb.Append(",\t");
                sb.Append(minor?.Module.shortTitle);
            }
            Student student = null;
            if (_students?.TryGetValue(s.studentId, out student) ?? false)
            {
                sb.Append(",\t");
                sb.Append(student.Person.Surname);
                sb.Append(",\t");
                sb.Append(student.Person.Name);
                sb.Append(",\t");
                sb.Append(student.Person.PatronymicName);
                sb.Append(",\t");
                sb.Append(student.Group.Name);
            }
        }

        Dictionary<string, Student> _students;
        public void AddStudentsList(IList<Student> students)
        {
            if (_students==null) _students=new Dictionary<string, Student>();
            foreach (var student in students)
            {
                _students.Add(student.Id,student);
            }
        }
        public void SetStudents(Dictionary<string, Student> students)
        {
            _students = students;
        }

        private Dictionary<string, Minor> _minors;
        public void SetModules(Dictionary<string, Minor> minors)
        {
            _minors = minors;
        }
    }

    class StudentDecision
    {
        public string studentId { get; set; }
        public string minorId { get; set; }
        public string decision { get; set; }
        public int? Priority { get; set; }
    }
}