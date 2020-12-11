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
using Ext.Utilities;
using Ext.Utilities.Linq;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using PagedList.Core;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
//using WebGrease.Css.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.ProjectView)]
    public class ProjectAdmissionController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private static StudentDesicionList _sdl;

        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                if (!db.ProjectCompetitionGroupsForUser(User).Select(g => g.Id).Contains(competitionGroupId))
                {
                    return JsonNet(new
                    {
                        data = new List<object>(),
                        total = 0
                    });
                }

                var modules = GetFilteredProjectModules(competitionGroupId, sort, filter);
                var paginated = modules.ToPagedList(page ?? 1, limit ?? 25);
                
                return JsonNet(new
                {
                    data = paginated,
                    total = modules.Count()
                });
            }
            
            var canEdit = db.CanEditProjectCompetitionGroup(User, competitionGroupId);
            ViewBag.IsInMassPublishRole = canEdit;
            ViewBag.CanAutoAdmit = canEdit;

            var competitionGroup = db.ProjectCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            return View(competitionGroup);
        }

        private IQueryable<object> GetFilteredProjectModules(int competitionGroupId, string sort, string filter)
        {
            var filterRules = FilterRules.Deserialize(filter);
            var competitionGroup = db.ProjectCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            var sortRules = SortRules.Deserialize(sort);
            if (sortRules.Count == 0)
                sortRules.Add(new SortRule
                {
                    Property = "title",
                    Direction = SortDirection.Ascending
                });

            var projectsForUser = db.ProjectsForUser(User).Select(p => p.ModuleId).ToList();
            var modules = db.ProjectProperties.Include("Project.Module")
                .Where(_ => _.ProjectCompetitionGroupId == competitionGroup.Id && _.Project.Module.type != "Парный модуль" && projectsForUser.Contains(_.ProjectId))
                .Select(p => new
                {
                    limits = db.ContractLimits.Where(
                        l => l.Period.ContractId == p.Project.ContractId
                            && l.Period.SemesterId == p.ProjectCompetitionGroup.SemesterId && l.Period.Year == p.ProjectCompetitionGroup.Year
                            && (p.ProjectCompetitionGroup.StudentCourse == 0 || l.Course == p.ProjectCompetitionGroup.StudentCourse || l.Course == 0)
                            // проверять группу и направление в лимите ????  && (p.ProjectCompetitionGroup.)
                            ).Select(l => l.Limit),
                    p
                })
                .Select(p => new
                {
                    limit = p.limits.Count() > 0 ? p.limits.Sum() : 0,
                    p
                })
                .Select(p => new
                {
                    p.p.p.Id,
                    p.p.p.Project.Module.title,
                    p.p.p.Project.Module.Level,
                    p.p.p.Project.Module.testUnits,
                    p.limit,
                
                    selection =
                        db.ProjectStudentSelectionPriorities.Count(
                            s => (s.Project.ModuleId == p.p.p.ProjectId) && (s.competitionGroupId == competitionGroupId) && s.priority > 0 && !s.Student.Sportsman),

                    addmission =
                        db.ProjectAdmissions.Count(
                            a =>
                                (a.ProjectId == p.p.p.ProjectId) &&
                                (a.ProjectCompetitionGroupId == competitionGroupId) &&
                                (a.Status == AdmissionStatus.Admitted)),

                    vacancy = p.limit - db.ProjectAdmissions.Count(
                        a =>
                            (a.ProjectId == p.p.p.ProjectId) &&
                            (a.ProjectCompetitionGroupId == competitionGroupId) &&
                            (a.Status == AdmissionStatus.Admitted) &&
                            (a.Student.Status == "Активный" || a.Student.Status == "Отп.с.посещ."))
                })
                .Where(filterRules)
                .OrderBy(sortRules.FirstOrDefault());
            return modules;
        }

        public ActionResult CompetitionGroupStudents(int id, int? page, int? limit, string sort, string filter)
        {
            ViewBag.Id = id;

            var property = db.ProjectProperties.Find(id);
            ViewBag.Title =
                $"Год {property.ProjectCompetitionGroup.Year}/Семестр {property.ProjectCompetitionGroup.Semester.Name} - Зачисление на проект \"{property.Project.Module.shortTitle}\" ({property.ProjectCompetitionGroup.Name})";
            ViewBag.ProjectPeriodId = id;

            var directionsForUser = db.ProjectProfilesForUser(User, property.ProjectId).Select(p => p.DIRECTION_ID).ToList();

            var directions = db.ContractLimits.Where(
                l => l.Period.ContractId == property.Project.ContractId
                    && l.Period.SemesterId == property.ProjectCompetitionGroup.SemesterId
                    && l.Period.Year == property.ProjectCompetitionGroup.Year
                    && (l.Course == property.ProjectCompetitionGroup.StudentCourse || property.ProjectCompetitionGroup.StudentCourse == 0 || l.Course == 0)
                    && l.Direction != null && directionsForUser.Contains(l.DirectionId)
                )
                .ToList()
                .Select(l => new
                {
                    Id = l.DirectionId,
                    Name = l.Direction.OksoAndTitle
                }).OrderBy(l => l.Name);
            ViewBag.Directions = JsonConvert.SerializeObject(directions);

            ViewBag.AdmittedCount = db.ProjectAdmissions.Count(
                                    _ =>
                                        _.ProjectId == property.ProjectId &&
                                        _.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId &&
                                        _.Status == AdmissionStatus.Admitted);
            ViewBag.CanEdit = db.CanEditProject(User, property.ProjectId);
            ViewBag.Roles = JsonConvert.SerializeObject(property.Project.Roles.Select(r => new { Id = r.Id, Name = r.Title.Replace("\\", "\\\\") }));
            ViewBag.PropertyId = property.Id;
            return View();
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

        private IQueryable<object> GetFilteredCompetitionGroupStudents(int id, SortRules sortRules, FilterRules filterRules, bool hideStudents = false)
        {
            var property = db.ProjectProperties.Include(x => x.ProjectCompetitionGroup).FirstOrDefault(_ => _.Id == id);

            var limits = db.ContractLimits.Where(l => l.Period.ContractId == property.Project.ContractId);
            var profiles = db.ProjectProfilesForUser(User, property.ProjectId, includeCurators: true).Select(p => p.ID).ToList();

            var includedGroups = property.ProjectCompetitionGroup.Groups
                .Where(g => limits.Any(l => (l.ProfileId == g.ProfileId || l.ProfileId == null) && (l.DirectionId == g.Profile.DIRECTION_ID || l.DirectionId == null))
                    && profiles.Contains(g.ProfileId))
                .Select(_ => _.Id).AsQueryable();

            var students = db.Students.Include(m => m.ProjectSelections).Include(m => m.ProjectAdmissions)
                .Where(s => includedGroups.Any(d => d == s.Group.Id)
                            && (!hideStudents || hideStudents &&
                                (s.Status == "Активный" || s.Status == "Отп.с.посещ." ||
                                 s.Status == "Отп.дород.послерод."))
                )
                .Select(s => new
                {
                    s,
                    priority = db.ProjectStudentSelectionPriorities
                        .Where(p => (p.studentId == s.Id) && (p.projectId == property.ProjectId))
                        .FirstOrDefault(),
                    admission = db.ProjectAdmissions.FirstOrDefault(a=>
                        a.studentId == s.Id && a.ProjectId == property.ProjectId && a.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId)
                })
                .Select(s => new
                {
                    s.s.Id,
                    GroupName = s.s.Group.Name,
                    Direction = s.s.Group.Profile.Direction.uid,
                    s.s.Group.Profile.Direction.okso,
                    profileCode = s.s.Group.Profile.CODE,
                    s.s.Person.Surname,
                    s.s.Person.Name,
                    s.s.Person.PatronymicName,
                    s.s.Rating,
                    s.s.IsTarget,
                    s.s.IsInternational,
                    s.s.Compensation,
                    s.s.PersonalNumber,
                    Modified = (DateTime?)s.priority.modified,
                    ModifiedString = s.priority.modified.ToString(),
                    Priority = s.priority.priority,
                    Role = s.admission == null ? s.priority.Role.Title : s.admission.Role.Title,
                    RoleId = s.admission == null ? (int?)s.priority.Role.Id : (int?)s.admission.Role.Id,
                    s.priority.Comment,
                    StudentStatus = s.s.Status,
                    Status = db.ProjectAdmissions
                        .Where(
                            a =>
                                (a.studentId == s.s.Id) && (a.ProjectId == property.ProjectId) &&
                                (a.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId))
                        .Select(a => a.Status)
                        .FirstOrDefault(),
                    
                    withoutAdmission = !db.ProjectAdmissions.Any(a =>
                                               (a.studentId == s.s.Id) && (a.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId) &&
                                               (a.Status == AdmissionStatus.Admitted)),

                    OtherAdmission = db.ProjectAdmissions
                        .Where(
                            a =>
                                (a.studentId == s.s.Id) &&
                                (a.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId) &&
                                (a.ProjectId != property.ProjectId) && (a.Project.Module.type != "Парный модуль") &&
                                (a.Status == AdmissionStatus.Admitted))
                        .Select(a => a.Project.Module.shortTitle)
                        .FirstOrDefault(),

                    IsPriority = db.ProjectStudentSelectionPriorities
                        .Any(p => (p.studentId == s.s.Id) && (p.projectId == property.ProjectId) && p.priority != null),

                    Published =
                    s.s.ProjectAdmissions.Any(
                        a =>
                            (a.studentId == s.s.Id) && (a.ProjectId == property.ProjectId) &&
                            (a.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId)) &&
                    s.s.ProjectAdmissions.FirstOrDefault(
                        a =>
                            (a.studentId == s.s.Id) && (a.ProjectId == property.ProjectId) &&
                            (a.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId)).Published

                });

            var filteredstudents = students.Where(filterRules);

            if (filterRules != null)
            {
                var withoutAdmission = filterRules.First(r => r.Property == "withoutAdmission").Value;
                var status = filterRules.First(r => r.Property == "Status").Value;
                
                if (withoutAdmission == "true" && (status == null || status.Equals("1")))
                {
                    var admittedstudents = students.Where(s => s.Status == AdmissionStatus.Admitted);
                    filteredstudents = filteredstudents.Union(admittedstudents);
                }
            }

            if (sortRules != null)
            { filteredstudents = filteredstudents.OrderBy(sortRules.FirstOrDefault(), v => v.GroupName); }

            return filteredstudents;
        }

        public ActionResult GetStudentLastProjectsInfo(string id)
        {
            string msg = SyncEngine.GetProjectStudentInfo(id);
            msg = string.IsNullOrWhiteSpace(msg) ? "Информации о предыдущих проектах студента нет" : msg;
            
            return Json(new { msg = msg });
        }

        public ActionResult Limits(int id)
        {
            var projectProperty = db.ProjectProperties.Find(id);
            var profiles = db.ProjectProfilesForUser(User, projectProperty.ProjectId, includeCurators: true);

            if (projectProperty == null)
                return JsonNet(new
                {
                    data = new List<object>(),
                    total = 0
                });

            var limits = db.ContractLimits.Where(
                l => l.Period.ContractId == projectProperty.Project.ContractId
                    && l.Period.SemesterId == projectProperty.ProjectCompetitionGroup.SemesterId
                    && l.Period.Year == projectProperty.ProjectCompetitionGroup.Year
                    && (l.Course == projectProperty.ProjectCompetitionGroup.StudentCourse || projectProperty.ProjectCompetitionGroup.StudentCourse == 0 || l.Course == 0)
                )
                .Select(l => new
                {
                    l.Limit,
                    l.Profile,
                    l.Direction
                })
                .GroupBy(l => new { l.Direction, l.Profile })
                .Where(l => (l.Key.Direction == null && l.Key.Profile == null) 
                    || (l.Key.Profile != null && profiles.Any(p => p.ID == l.Key.Profile.ID))
                    || (l.Key.Profile == null && l.Key.Direction.uid != null && profiles.Any(p => p.DIRECTION_ID == l.Key.Direction.uid))
                    )
                .Select(l => new
                {
                    l,
                    admitted = db.ProjectAdmissions.Count(
                        a => (a.ProjectCompetitionGroupId == projectProperty.ProjectCompetitionGroupId)
                            && a.Status == AdmissionStatus.Admitted
                            && a.ProjectId == projectProperty.ProjectId
                            && (a.Student.Group.Profile.ID == l.Key.Profile.ID || l.Key.Profile.ID == null)
                            && (a.Student.Group.Profile.DIRECTION_ID == l.Key.Direction.uid || l.Key.Direction.uid == null))
                })
                .Select(l => new
                {
                    profileId = l.l.Key.Profile.ID,
                    okso = l.l.Key.Direction.okso ?? "Не указано",
                    profile = l.l.Key.Profile.CODE ?? "Не указано",
                    limit = l.l.Select(m => m.Limit).Sum(),
                    l.admitted,
                    freePlaces = l.l.Select(m => m.Limit).Sum() - l.admitted,
                })
                .ToList();

            var allLimit = limits.Select(l => l.limit).Sum();
            var allAdmitted = limits.Select(l => l.admitted).Sum();

            var result = limits.Select(l => new
            {
                l.okso,
                l.profile,
                l.limit,
                l.admitted,
                l.freePlaces,
                groupField = $"Всего мест: {allLimit}, Зачислено: {allAdmitted}"
            });
            
            return JsonNet(new
            {
                data = result,
                total = result.Count()
            });
        }

        public ActionResult SetCompetitionGroupAdmissionStatus(string studentIds, int propertyId, AdmissionStatus status)
        {
            var studentRoles = JsonConvert.DeserializeObject<List<StudentRole>>(studentIds);

            var property = db.ProjectProperties.FirstOrDefault(p => p.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");

            var overlimit = studentRoles.Select(s => SetCompetitionGroupAdmissionStatusInternal(s, status, property)).ToList().Any(_ => _);

            return Json(new { reload = false, msg = overlimit ? "Превышен лимит" : null });
        }

        private bool SetCompetitionGroupAdmissionStatusInternal(StudentRole studentRole, AdmissionStatus status, ProjectProperty property)
        {
            Action<AdmissionStatus> dismissOtherAdmissionStatus = (admStatus) =>
            {
                if (admStatus == AdmissionStatus.Admitted)
                {
                    var admissions = db.ProjectAdmissions.Where(
                        a =>
                            (a.studentId == studentRole.Id) &&
                            (a.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId) &&
                            (a.ProjectCompetitionGroup.SemesterId == property.ProjectCompetitionGroup.SemesterId) &&
                            a.ProjectId != property.ProjectId).ToList();
                    admissions.ForEach(a =>
                    {
                        if (a.Status == AdmissionStatus.Admitted)
                        {
                            CleanSubgroupMembership(a);
                        }
                        a.Status = AdmissionStatus.Indeterminate;
                        a.RoleId = studentRole.RoleId;
                    });
                    db.SaveChanges();

                }
            };
            var admission =
                  db.ProjectAdmissions.FirstOrDefault(
                      a =>
                          (a.studentId == studentRole.Id) &&
                          (a.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId) &&
                          (a.ProjectCompetitionGroup.Year == property.ProjectCompetitionGroup.Year) &&
                          (a.ProjectCompetitionGroup.SemesterId == property.ProjectCompetitionGroup.SemesterId) &&
                          a.ProjectId == property.ProjectId);

            if (admission == null)
            {
                admission = new ProjectAdmission
                {
                    studentId = studentRole.Id,
                    ProjectCompetitionGroupId = property.ProjectCompetitionGroupId,
                    Status = status,
                    ProjectId = property.ProjectId,
                    Published = false,
                    RoleId = studentRole.RoleId
                };
                dismissOtherAdmissionStatus(status);
                db.ProjectAdmissions.Add(admission);
            }
            else
            {
                if (admission.Status == AdmissionStatus.Admitted)
                {
                    CleanSubgroupMembership(admission);
                }
                admission.Status = status;
                admission.Published = false;
                admission.RoleId = studentRole.RoleId;

                dismissOtherAdmissionStatus(status);
            }
            db.SaveChanges();

            bool overlimit = false;
            if (status == AdmissionStatus.Admitted)
            {
                var student = db.Students.FirstOrDefault(s => s.Id == studentRole.Id);
                
                // если проект был уровня А, то надо зачислить его на парный модуль
                if (property.Project.Module.Level == "A" || property.Project.Module.Level == "А") // первая А англ., вторая - рус.
                {
                    AdmitToPairedModule(student, property.ProjectCompetitionGroup.SemesterId, property.ProjectCompetitionGroupId);
                }
                
                var limitsQuery = db.ContractLimits.Where(
                    l => l.Period.ContractId == property.Project.ContractId
                        && l.Period.SemesterId == property.ProjectCompetitionGroup.SemesterId
                        && l.Period.Year == property.ProjectCompetitionGroup.Year
                        && (l.Course == property.ProjectCompetitionGroup.StudentCourse || property.ProjectCompetitionGroup.StudentCourse == 0 || l.Course == 0)
                        && (l.ProfileId == null || l.ProfileId == student.Group.ProfileId)
                        && (l.DirectionId == null || l.DirectionId == student.Group.Profile.DIRECTION_ID)
                    );

                var admissionCount = db.ProjectAdmissions.Count(
                    a =>
                        (a.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId) &&
                        (a.Status == AdmissionStatus.Admitted) && (a.ProjectId == property.ProjectId)
                        && limitsQuery.Any(l => (l.ProfileId == null || l.ProfileId == a.Student.Group.ProfileId)
                                    && (l.DirectionId == null || l.DirectionId == a.Student.Group.Profile.DIRECTION_ID))
                                    );
                var limits = limitsQuery.Select(l => l.Limit).ToList();
                overlimit = false;
                if (limits.Count > 0)
                    overlimit = admissionCount > limits.Sum();
            }
            
            return overlimit;
        }

        void CleanSubgroupMembership(ProjectAdmission projectAdmission)
        {
            var subgrougMemberships =
                  db.ProjectSubgroupMemberships.Where(
                      _ =>
                          _.studentId == projectAdmission.studentId &&
                          _.Subgroup.Meta.CompetitionGroupId == projectAdmission.ProjectCompetitionGroupId);

            if (subgrougMemberships != null)
            {
                foreach (var sfsm in subgrougMemberships) db.ProjectSubgroupMemberships.Remove(sfsm);
                //subgrougMemberships.ForEach(sfsm => db.ProjectSubgroupMemberships.Remove(sfsm));
            }

            db.SaveChanges();
        }

        public void PublishCompetitionGroupAdmission(string[] studentId, int propertyId)
        {
            foreach (var id in studentId)
            {
                var property = db.ProjectProperties.FirstOrDefault(p => p.Id == propertyId);

                if (property != null) // TODO проверка на наличие прав вносить изменения
                {
                    var admission =
                        db.ProjectAdmissions.Where(
                            a =>
                                (a.studentId == id) &&
                                (a.ProjectCompetitionGroupId == property.ProjectCompetitionGroupId) &&
                                a.ProjectId == property.ProjectId).FirstOrDefault();

                    if (admission == null)
                        continue;

                    admission.Published = true;
                    db.SaveChanges();
                }

            }
        }

        public ActionResult DownloadCompetitionGroupStudents(int id, string filter)
        {
            try
            {
                Logger.Info("Скачивание отчета excel по зачислениям студентов на проект. ProjectProperty: {0}. Фильтры: {1}", id, filter);
                var property = db.ProjectProperties.Find(id);
                var fileName = $"{property.ProjectCompetitionGroup.ShortName} {property.Project.Module.title}";

                var filterRules = FilterRules.Deserialize(filter);
                var reportVms = GetFilteredCompetitionGroupStudents(id, null, filterRules);
                var stream = new VariantExport().Export(new { Rows = reportVms }, "projectPGStudentsReportTemplate.xlsx");

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{fileName}.xlsx".ToDownloadFileName());

            }
            catch (Exception ex)
            {
                Logger.Info("Ошибка при скачивании отчета excel по зачислениям студентов на проект. ProjectProperty: {0}. Фильтры: {1}", id, filter);
                Logger.Error(ex);
            }
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult DownloadAutoAdmissionReport()
        {
            Response.Headers.Add("Content-Disposition",
                "attachment; filename=Отчёт_по_автозачислениям_на_проекты.txt");
            if (_sdl == null)
                _sdl = new StudentDesicionList();
            return File(_sdl.FormatStream(), "application/text");
        }

        public ActionResult PrepareAuto(int competitionGroupId)
        {
            var group = db.ProjectCompetitionGroups.FirstOrDefault(p => p.Id == competitionGroupId);

            ViewBag.AnyPrepared =
                db.ProjectAdmissions.Any(
                    va =>
                        (va.ProjectCompetitionGroupId == competitionGroupId) &&
                        (va.Status != AdmissionStatus.Indeterminate));
            ViewBag.AnyPublished =
                db.ProjectAdmissions.Any(
                    va =>
                        (va.ProjectCompetitionGroupId == competitionGroupId) &&
                        (va.Status != AdmissionStatus.Indeterminate) && va.Published);
            ViewBag.AnyWithoutRating =
                db.ProjectStudentSelectionPriorities.Any(
                    s => (s.competitionGroupId == competitionGroupId) && (s.Student.Rating == null));
            ViewBag.CanEdit = db.CanEditProjectCompetitionGroup(User, competitionGroupId);
            return
                View(new MinorAutoVM
                {
                    CompetitionGroup = group.Name,
                    CompetitionGroupId = group.Id,
                    Semester = group.Semester.Name,
                    SemesterId = group.Id,
                    Year = group.Year,
                    StudentCount =
                        db.ProjectStudentSelectionPriorities.Where(
                                s => s.competitionGroupId == competitionGroupId && s.priority != null)
                            .Select(s => s.Student)
                            .Distinct()
                            .Count(),
                    AdmittedCount = db.ProjectAdmissions.Where(
                            s =>
                                (s.ProjectCompetitionGroupId == competitionGroupId) &&
                                (s.Status == AdmissionStatus.Admitted))
                        .Select(s => s.studentId)
                        .Distinct()
                        .Count(),
                    MinorCount = db.ProjectProperties.Count(m => m.ProjectCompetitionGroupId == competitionGroupId),
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
            ((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 1200000;

            _sdl = new StudentDesicionList();
            var group = db.ProjectCompetitionGroups.FirstOrDefault(g => g.Id == competitionGroupId);

            var year = group.Year;
            var semesterId = group.SemesterId;

            var vm = new MinorAutoVM { Semester = group.Name, Year = year, SemesterId = semesterId, CompetitionGroupId = competitionGroupId };

            Logger.Info("Расчёт зачислений на проекты {0} {1} {2}", year, semesterId, group.Name);

            //Поиск студентов, приоритеты которых рассматривать не нужно, потому что они уже зачислены на проект
            HashSet<string> alreadyAdmitted = new HashSet<string>(db.ProjectAdmissions.Where(
                s => s.ProjectCompetitionGroupId == competitionGroupId && s.Status == AdmissionStatus.Admitted)
                .Select(s => s.studentId).Distinct());

            //Выбор всех проектов этого периода
            var projects = db.ProjectPropertiesForUser(User).Where(p => p.ProjectCompetitionGroupId == competitionGroupId)
                .SelectMany(p => p.Project.Periods.Where(per => per.Year == year && per.SemesterId == semesterId)).ToList();
                
                //_db.ProjectsForUser(User).SelectMany(m => m.Periods.Where(p => p.Year == year && p.SemesterId == semesterId)).ToList();

            var profiles = db.ProjectProfilesForUser(User).Select(p => new { p.DIRECTION_ID, p.ID }).ToList();
            //Все лимиты на проекты этого периода
            var contractIds = projects.Select(p => p.Project.ContractId);

            var limits = db.ContractLimits.Where(l => contractIds.Contains(l.Period.ContractId))
                .ToList()
                .Where(l => (l.DirectionId != null && l.ProfileId != null && profiles.Any(p => p.DIRECTION_ID == l.DirectionId && p.ID == l.ProfileId))
                    || (l.DirectionId != null && l.ProfileId == null && profiles.Any(p => p.DIRECTION_ID == l.DirectionId))
                    || l.DirectionId == null && l.ProfileId == null)
                .Select(l => new ContractLimitModel()
                {
                    Course = l.Course,
                    Limit = l.Limit,
                    DirectionId = l.DirectionId,
                    ProfileId = l.ProfileId,
                    ContractId = l.Period.ContractId,
                    QualificationName = l.QualificationName,
                    SemesterId = l.Period.SemesterId,
                    Year = l.Period.Year,
                    Level = db.Projects.FirstOrDefault(p => p.ContractId == l.Period.ContractId).Module.Level
                })
                .ToList();

            var projectIds = projects.Select(p => p.ProjectId);
            //Вычитаем уже зачисленных студентов из лимитов
            var existingAdmissions = db.ProjectAdmissions.Where(s => s.ProjectCompetitionGroupId == competitionGroupId && s.Status == AdmissionStatus.Admitted)
                    .Include(a => a.Student.Group.Profile).ToList()
                    .Where(s => projectIds.Contains(s.ProjectId) && profiles.Any(p => p.ID == s.Student.Group.ProfileId));

            foreach(var ea in existingAdmissions)
            {
                var limitIndex = limits.FindIndex(l => (l.ProfileId == null || l.ProfileId == ea.Student.Group.ProfileId)
                        && (l.DirectionId == null || l.DirectionId == ea.Student.Group.Profile.DIRECTION_ID)
                        && (l.QualificationName == null || l.QualificationName == ea.Student.Group.Qual)
                        && (l.Course == 0 || l.Course == ea.Student.Group.Course)
                        && (l.ContractId == ea.Project.ContractId)
                        && l.Limit > 0);
                if (limitIndex != -1)
                {
                    limits[limitIndex].Limit--;
                    _sdl.AddNew("Студент уже зачислен на проект в этом периоде", ea.ProjectId, ea.studentId, null);
                }
                else
                {
                    //TODO сообщение о том, что для студента нет лимита ???
                }
            }
            
            //Рассматриваются пожелания всех студентов по текущей проектной группе
            var wishes = db.ProjectStudentSelectionPriorities
                .Where(s => s.competitionGroupId == competitionGroupId && s.priority.HasValue && projectIds.Contains(s.projectId))
                .Include(s => s.Student.Group.Profile)
                .ToList()
                .Where(s => profiles.Any(p => p.ID == s.Student.Group.ProfileId))
                .OrderByDescending(s => s.Student.Rating)
                .ThenBy(s => s.priority)
                .ToList();

            foreach (var wish in wishes)
            {
                if (alreadyAdmitted.Contains(wish.studentId))
                {
                    continue;
                }
                var limitIndex = limits.FindIndex(l => (l.ProfileId == null || l.ProfileId == wish.Student.Group.ProfileId)
                        && (l.DirectionId == null || l.DirectionId == wish.Student.Group.Profile.DIRECTION_ID)
                        && (l.QualificationName == null || l.QualificationName == wish.Student.Group.Qual)
                        && (l.Course == 0 || l.Course == wish.Student.Group.Course)
                        && (l.ContractId == wish.Project.ContractId)
                        && l.Limit > 0);

                if (limitIndex == -1)
                {
                    _sdl.AddNew("На проекте нет мест для образовательной программы студента", wish.projectId, wish.studentId, wish.priority);
                    continue;
                }

                TryAutoAdmitt(wish.competitionGroupId, wish.studentId, wish.projectId, wish.priority, vm, limits, limitIndex, alreadyAdmitted, wish.roleId);
            }
            db.SaveChanges();

            var unadmitted =
                db.Students.OnlyActive()
                    .Where(s => s.Group.ProjectCompetitionGroups.Any(cg => cg.Id == competitionGroupId))
                    .Include(s => s.Group.Profile)
                    .ToList()
                    .Where(s => profiles.Any(p => p.ID == s.Group.ProfileId)
                        && !db.ProjectAdmissions.Any(
                                a =>
                                    (a.studentId == s.Id) && (a.Status == AdmissionStatus.Admitted) &&
                                    (a.ProjectCompetitionGroupId == competitionGroupId)))
                    .OrderByDescending(s => s.Rating);

            foreach(var student in unadmitted)
            {
                // зачислить студентов без приоритета можно только на проект уровня А
                var limitIndex = limits.FindIndex(l => (l.ProfileId == null || l.ProfileId == student.Group.ProfileId)
                    && (l.DirectionId == null || l.DirectionId == student.Group.Profile.DIRECTION_ID)
                    && (l.QualificationName == null || l.QualificationName == student.Group.Qual)
                    && (l.Course == 0 || l.Course == student.Group.Course)
                    && l.Limit > 0
                    && (l.Level == "A" || l.Level == "А") // первая А - англ., вторая - рус.
                    );
                
                if (limitIndex != -1)
                {
                    var contractId = limits[limitIndex].ContractId;
                    var project = db.Projects.FirstOrDefault(p => p.ContractId == contractId);
                    _sdl.AddNew("у студента нет выполнимых приоритетов", project?.ModuleId, student.Id, -1);

                    TryAutoAdmitt(competitionGroupId, student.Id, projectId: project?.ModuleId, priority: null,
                        vm: vm, limits: limits, limitIndex: limitIndex, alreadyAdmitted: alreadyAdmitted);
                }
                else
                {
                    _sdl.AddNew("Нет проектов для образовательной программы студента или нет мест", null, student.Id, -1);
                }
            }
            db.SaveChanges();
            Logger.Info("Расчёт зачислений на проекты окончен {0} {1}", year, group.Name);

            db.MinorAutoAdmissionReports.Add(new MinorAutoAdmissionReport
            {
                Content = _sdl.GetStringBuilder().ToString(),
                Date = DateTime.Now,
                ModuleType = ModuleType.Project
            });

            db.SaveChanges();
            return vm;
        }


        private void TryAutoAdmitt(int competitionGroupId, string studentId, string projectId, int? priority, 
            MinorAutoVM vm, List<ContractLimitModel> limits, int limitIndex, HashSet<string> alreadyAdmitted, int? roleId = null)
        {
            // поиск зачислений для текущего студента
            var adm = db.ProjectAdmissions
                .FirstOrDefault(a => a.studentId == studentId && a.ProjectCompetitionGroupId == competitionGroupId);

            if (adm == null)
            {
                _sdl.AddNew("создаётся зачисление", projectId, studentId, priority);
                //зачисления ещё не было - создаём его
                db.ProjectAdmissions.Add(new ProjectAdmission
                {
                    studentId = studentId,
                    ProjectCompetitionGroupId = competitionGroupId,
                    ProjectId = projectId,
                    Published = false,
                    Status = AdmissionStatus.Admitted,
                    RoleId = roleId
                });
                vm.AdmittedCount++;
            }
            else
            {
                //если в зачислении было отказано ранее - не рассматриваем более заявку студента на этот майнор
                _sdl.AddNew("перевод неопределённого статуса в зачисленный", projectId, studentId, priority);
                if (adm.Status == AdmissionStatus.Indeterminate)
                {
                    adm.Status = AdmissionStatus.Admitted;
                    adm.Published = false;
                    adm.RoleId = adm.RoleId ?? roleId;
                    vm.AdmittedCount++;
                }
                else
                {
                    _sdl.AddNew("у студента уже есть отрицательное решение", projectId, studentId, priority);
                    return;
                }
            }
            var project = db.Projects.FirstOrDefault(p => p.ModuleId == projectId);
            if (project.Module.Level == "A" || project.Module.Level == "А") // первая А англ., вторая - рус.
            {
                // зачисляем студента на соответствующий парный модуль
                var student = db.Students.FirstOrDefault(s => s.Id == studentId);
                AdmitToPairedModule(student, limits[limitIndex].SemesterId, competitionGroupId);
            }
            
            alreadyAdmitted.Add(studentId);
            limits[limitIndex].Limit--;
        }

        /// <summary>
        /// Зачисление студента на парный модуль. Используется при зачислении студента на проект уровня А
        /// </summary>
        /// <param name="student"></param>
        /// <param name="semesterId"></param>
        /// <param name="competitionGroupId"></param>
        private void AdmitToPairedModule(Student student, int semesterId, int competitionGroupId)
        {
            var planTerms = db.PlanTerms;

            var mainModuleUUID = db.Plans
                .Where(p => p.eduplanNumber == student.planVerion
                    && p.versionNumber == student.versionNumber
                    && p.Module.type == "Проектное обучение"
                    && p.Module.Source == Source.Uni
                    && !p.remove
                    && (p.Module.Level == "A" || p.Module.Level == "А") // первая А англ., вторая - рус.
                    && db.PlanDisciplineTerms.Any(t =>
                                t.Course == student.Group.Course
                                && t.SemesterID == semesterId
                                && t.DisciplineUUID == p.disciplineUUID)
                    )
                .FirstOrDefault()?.moduleUUID;

            var pairedModuleUUID = db.ModuleRelations.FirstOrDefault(r =>
                        r.MainModuleUUID == mainModuleUUID && r.eduplanNumber == student.planVerion && r.versionNumber == student.versionNumber)?.PairedModuleUUID;

            if (db.Projects.Any(p => p.ModuleId == pairedModuleUUID))
            {
                var admission = new ProjectAdmission
                {
                    studentId = student.Id,
                    ProjectCompetitionGroupId = competitionGroupId,
                    Status = AdmissionStatus.Admitted,
                    ProjectId = pairedModuleUUID,
                    Published = false
                };
                try
                {
                    db.ProjectAdmissions.Update(admission);
                }
                catch
                {
                    db.ProjectAdmissions.Add(admission);
                }
                
                db.SaveChanges();
            }
        }
        
        public ActionResult PublishCompetitionGroupAdmissions(int competitionGroupId)
        {
            foreach(var m in db.ProjectAdmissions.Where(m => m.ProjectCompetitionGroupId == competitionGroupId))
            //db.ProjectAdmissions.Where(m => m.ProjectCompetitionGroupId == competitionGroupId).ForEach(m =>
            {
                m.Published = true;
            }

            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }
    }
}