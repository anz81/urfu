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

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.MUPManager)]
    public class MUPAdmissionController : BaseController
    {
        //private static StudentDesicionList _sdl;
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var modules = GetFilteredMUPs(competitionGroupId, sort, filter);
                var paginated = modules.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = modules.Count()
                });
            }
            var competitionGroup = db.MUPCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            
            return View(competitionGroup);
        }

        private IQueryable<object> GetFilteredMUPs(int competitionGroupId, string sort, string filter)
        {
            var filterRules = FilterRules.Deserialize(filter);
            var competitionGroup = db.MUPCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            var sortRules = SortRules.Deserialize(sort);
            if (sortRules.Count == 0)
                sortRules.Add(new SortRule
                {
                    Property = "title",
                    Direction = SortDirection.Ascending
                });
            var modules = db.MUPProperties.Include("MUP.Module")
                .Where(m => m.MUPCompetitionGroupId == competitionGroup.Id && m.MUP.Periods.Any(d=>d.Year == competitionGroup.Year && d.SemesterId == competitionGroup.SemesterId 
                        && (d.Course== null || d.Course == competitionGroup.StudentCourse || competitionGroup.StudentCourse == 0)))               
                .Select(p => new
                {
                    p.Id,
                    number = p.MUP.Module.number.ToString(),
                    p.MUP.Module.title,
                    p.MUP.Module.testUnits,
                    limit = db.MUPProperties.FirstOrDefault(
                            pr => (pr.MUPId == p.MUPId) && (pr.MUPCompetitionGroupId == competitionGroupId)).Limit,
                    addmission = db.MUPAdmissions.Count(
                        a =>
                            (a.MUPId == p.MUPId) &&
                            (a.MUPCompetitionGroupId == competitionGroupId) &&
                            (a.Status == AdmissionStatus.Admitted))       
                })
                .Where(filterRules)
                .OrderBy(sortRules.FirstOrDefault(), m => m.number);
            return modules;
        }

        public ActionResult CompetitionGroupStudents(int id, int? page, int? limit, string sort, string filter)
        {
            ViewBag.CanEdit = User.IsInRole(ItsRoles.MUPManager);
            
            var property = db.MUPProperties.Find(id);
            ViewBag.Title =
                $"Год {property.MUPCompetitionGroup.Year}/Семестр {property.MUPCompetitionGroup.Semester.Name} - Зачисление на модули МУП \"{property.MUPCompetitionGroup.Name}\" - Модуль \"{property.MUP.Module.shortTitle}\"";
            ViewBag.MUPPeriodId = id;
            return View(property);
        }

        public ActionResult CompetitionGroupStudentsAjax(int id, bool hideStudents, int? page, int? limit, string sort, string filter)
        {
            var sortRules = SortRules.Deserialize(sort);
            var filterRules = FilterRules.Deserialize(filter);

            var students = GetFilteredMUPStudents(id, hideStudents, sortRules, filterRules);

            var paginated = students.ToPagedList(page ?? 1, limit ?? 25);
            return JsonNet(new
            {
                data = paginated,
                total = students.Count()
            });
        }

        private IQueryable<object> GetFilteredMUPStudents(int id, bool hideStudents, SortRules sortRules, FilterRules filterRules)
        {
            var property = db.MUPProperties.Include(x => x.MUPCompetitionGroup).FirstOrDefault(_=>_.Id==id);

            var includedGroups = property.MUPCompetitionGroup.Groups.Select(_ => _.Id).AsQueryable();

            var students = db.Students.Include(m => m.MUPAdmissions)
                .Where(s => includedGroups.Any(d => d == s.Group.Id)
                    && (!hideStudents || hideStudents && (s.Status == "Активный" || s.Status == "Отп.с.посещ." || s.Status == "Отп.дород.послерод."))
                    )
                .Select(s => new
                {
                    s.Id,
                    GroupName = db.GroupsHistories.FirstOrDefault(g => g.GroupId == s.GroupId && g.YearHistory == property.MUPCompetitionGroup.Year).Name,
                    s.Person.Surname,
                    s.Person.Name,
                    s.Person.PatronymicName,
                    s.Rating,
                    s.IsTarget,
                    s.IsInternational,
                    s.Compensation,
                    s.PersonalNumber,
                    StudentStatus = s.Status,
                    Status = db.MUPAdmissions
                        .Where(
                            a =>
                                (a.studentId == s.Id) && (a.MUPId == property.MUPId) &&
                                (a.MUPCompetitionGroupId == property.MUPCompetitionGroupId))
                        .Select(a => a.Status)
                        .FirstOrDefault(),
                    OtherAdmission = db.MUPAdmissions
                        .Where(
                            a =>
                                (a.studentId == s.Id) && (a.MUPId != property.MUPId) &&
                                (a.MUPCompetitionGroupId == property.MUPCompetitionGroupId) &&
                                (a.Status == AdmissionStatus.Admitted))
                        .Select(a => a.MUP.Module.shortTitle)
                        .FirstOrDefault(),
                    Published =
                        s.MUPAdmissions.Any(
                            a =>
                                (a.studentId == s.Id) && (a.MUPId == property.MUPId) &&
                                (a.MUPCompetitionGroupId == property.MUPCompetitionGroupId)) &&
                        s.MUPAdmissions.FirstOrDefault(
                                a =>
                                    (a.studentId == s.Id) && (a.MUPId == property.MUPId) &&
                                    (a.MUPCompetitionGroupId == property.MUPCompetitionGroupId))
                        .Published
                });

            students = students.OrderBy(sortRules.FirstOrDefault(), v => v.GroupName);

            students = students.Where(filterRules);
            return students;
        }

        public ActionResult SetCompetitionGroupAdmissionStatus(string[] studentIds, int propertyId, AdmissionStatus status)
        {
            var property = db.MUPProperties.FirstOrDefault(p => p.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");

            var overlimit = false;

            overlimit = studentIds.Select(_=> SetCompetitionGroupAdmissionStatusInternal(_, status, property, overlimit)).ToList().Any(_=>_);

            return Json(new {reload = false, msg = overlimit ? "Превышен лимит" : null});
        }

        private bool SetCompetitionGroupAdmissionStatusInternal(string studentId, AdmissionStatus status,
            MUPProperty property, bool overlimit)
        {
            Action<AdmissionStatus> dismissOtherAdmissionStatus = (admStatus) =>
            {
                if (admStatus == AdmissionStatus.Admitted)
                {
                    var admissions = db.MUPAdmissions.Where(
                        a =>
                            (a.studentId == studentId) &&
                            (a.MUPCompetitionGroupId == property.MUPCompetitionGroupId) &&
                            (a.MUPCompetitionGroup.SemesterId == property.MUPCompetitionGroup.SemesterId) &&
                            a.MUPId != property.MUPId).ToList();
                    admissions.ForEach(a =>
                    {
                        if (a.Status == AdmissionStatus.Admitted)
                        {
                            CleanSubgroupMembership(a);
                        }
                        a.Status = AdmissionStatus.Indeterminate;
                    });
                    db.SaveChanges();

                }
            };

            var admission =
                db.MUPAdmissions.FirstOrDefault(
                    a =>
                        (a.studentId == studentId) &&
                        (a.MUPCompetitionGroupId == property.MUPCompetitionGroupId) &&
                        (a.MUPCompetitionGroup.Year == property.MUPCompetitionGroup.Year) &&
                        (a.MUPCompetitionGroup.SemesterId == property.MUPCompetitionGroup.SemesterId) &&
                         a.MUPId == property.MUPId);
            if (admission == null)
            {
                admission = new MUPAdmission
                {
                    studentId = studentId,
                    MUPCompetitionGroupId = property.MUPCompetitionGroupId,
                    Status = status,
                    MUPId = property.MUPId,
                    Published = false
                };
                dismissOtherAdmissionStatus(status);

                db.MUPAdmissions.Add(admission);
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
            db.SaveChanges();
  
            var admissionCount = db.MUPAdmissions.Count(
                a =>
                    (a.MUPCompetitionGroupId == property.MUPCompetitionGroupId) &&
                    (a.Status == AdmissionStatus.Admitted) && (a.MUPId == property.MUPId));
            if ((status == AdmissionStatus.Admitted) && (admissionCount > property.Limit))
                overlimit = true;
            return overlimit;
        }
        void CleanSubgroupMembership(MUPAdmission mupAdmission)
        {
            var subgrougMemberships =
                  db.MUPSubgroupMemberships.Where(
                      m =>
                          m.studentId == mupAdmission.studentId &&
                          m.Subgroup.Meta.CompetitionGroupId == mupAdmission.MUPCompetitionGroupId);

            if (subgrougMemberships != null)
            {
                foreach (var a in subgrougMemberships) db.MUPSubgroupMemberships.Remove(a);
                //subgrougMemberships.ForEach(flsm => db.MUPSubgroupMemberships.Remove(flsm));
            }

            db.SaveChanges();
        }

        public void PublishCompetitionGroupAdmission(string[] studentId, int propertyId)
        {
            foreach (var Id in studentId)
            {
                var property = db.MUPProperties.FirstOrDefault(p => p.Id == propertyId);

                if (property != null)
                {
                    var admission =
                        db.MUPAdmissions.Where(
                                a =>
                                    (a.studentId == Id) &&
                                    (a.MUPCompetitionGroupId == property.MUPCompetitionGroupId) &&
                                    a.MUPId == property.MUPId)
                            .FirstOrDefault();

                    if (admission == null)
                        continue;

                    admission.Published = true;
                    db.SaveChanges();
                }
            }
        }

        public ActionResult Report(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var reportVms = PrepareReport(sort, filter).ToList();
                return JsonNet(reportVms);
            }
            ViewBag.Semesters = JsonConvert.SerializeObject(db.Semesters);

            var years = db.MUPCompetitionGroups.GroupBy(_ => _.Year).Select(s => new { Id = s.Key, Name = s.Key });
            ViewBag.Years = JsonConvert.SerializeObject(years);
            return View();
        }

        private IQueryable<object> PrepareReport(string sort, string filter)
        {
            db.Database.SetCommandTimeout(1200000);

            //var expression = admissionFilter ?? (x => true);

            FilterRules rules = FilterRules.Deserialize(filter);

            var yearstr = rules?.FirstOrDefault(r => r.Property == "Year")?.Value;
            int year;
            bool isYear = Int32.TryParse(yearstr, out year);

            var semesterstr = rules?.FirstOrDefault(r => r.Property == "semesterId")?.Value;
            int semester;
            bool isSemester = Int32.TryParse(semesterstr, out semester);

            var periods =
                db.MUPProperties
                    .Where(p => (!isYear || isYear && p.MUPCompetitionGroup.Year == year)
                                && (!isSemester || isSemester && p.MUPCompetitionGroup.SemesterId == semester))
                    .Select(
                        p => new
                        {
                            p,
                            c = db.MUPAdmissions
                                .Where(
                                    ma =>
                                        ma.MUPCompetitionGroupId == p.MUPCompetitionGroupId &&
                                        ma.MUPId == p.MUPId && ma.Status == AdmissionStatus.Admitted)
                                .Select(ma =>  ma.Student),
                        })
                    .Where(r => r.c.Any());

            var reportVms = periods
                .SelectMany(p => p.c
                    .SelectMany(student => db.MUPSubgroupMemberships.Where(m => m.studentId == student.Id &&  m.Subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.MUPId==p.p.MUPId &&  !m.Subgroup.Removed),(student, subgroup)=>new{student,subgroup})
                    .Select(c => new
                    {
                        MUP = p.p.MUP.Module.title,
                        CompetitionGroupName = p.p.MUPCompetitionGroup.Name,
                        p.p.MUPCompetitionGroup.Year,
                        Semester = p.p.MUPCompetitionGroup.Semester.Name,
                        semesterId = p.p.MUPCompetitionGroup.SemesterId,
                        GroupName = db.GroupsHistories.FirstOrDefault(g =>
                                         g.GroupId == c.student.GroupId && g.YearHistory == p.p.MUPCompetitionGroup.Year).Name,
                        c.student.Id,
                        c.student.Person.Name,
                        c.student.Person.PatronymicName,
                        c.student.Person.Surname,
                        StudentStatus= c.student.Status,
                        SubgroupName =c.subgroup.Subgroup.Name,
                         c.subgroup.Subgroup.Description,
                        Teacher= c.subgroup.Subgroup.Teacher.initials
                    })
                );


            if (sort != null)
                reportVms = reportVms.OrderBy(SortRules.Deserialize(sort).FirstOrDefault());
            else
                reportVms =
                    reportVms.OrderBy(r => r.CompetitionGroupName)
                        .ThenBy(r => r.MUP)
                        .ThenBy(r => r.Surname)
                        .ThenBy(r => r.Name);

            reportVms = reportVms.Where(rules);

            return reportVms;
        }

        public ActionResult DownloadReport(string filter)
        {
            var reportVms = PrepareReport(null, filter);

            FilterRules rules = FilterRules.Deserialize(filter);

            var yearstr = rules?.FirstOrDefault(r => r.Property == "Year")?.Value;
            int year;
            bool isYear = Int32.TryParse(yearstr, out year);

            var semesterstr = rules?.FirstOrDefault(r => r.Property == "semesterId")?.Value;
            int semester;
            bool isSemester = Int32.TryParse(semesterstr, out semester);

            //db.Semesters.Load();
            string semesterName = db.Semesters.FirstOrDefault(s => s.Id == semester).Name;

            var filename = $"Отчет_по_МУПам_{year}_{semesterName}.xlsx";

            var stream = new VariantExport().Export(new {Rows = reportVms}, "MUPReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename.ToDownloadFileName());
        }


    }

}
