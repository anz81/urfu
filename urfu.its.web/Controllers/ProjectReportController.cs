//using Ext.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using PagedList;
//using Ext.Utilities.Linq;
using Urfu.Its.Web.Model.Models.Practice;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using Urfu.Its.Web.Excel;
using Urfu.Its.Common;
//using Microsoft.Ajax.Utilities;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Urfu.Its.Web.Model.Models.ModulesVM;
using Microsoft.AspNetCore.Identity;
using Urfu.Its.Web.Model.Models.ProjectReport;
using Ext.Utilities;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.ProjectView)]
    public class ProjectReportController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private readonly int currentYear = DateTime.Now.Month < 7 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
        private readonly int currentSemester = DateTime.Now.Month < 7 ? 2 : 1;

        public ActionResult Projects(int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                var projects = GetFilteredProjects(filter);

                return Json(
                    new
                    {
                        data = projects,
                        total = projects.Count()
                    },
                    new JsonSerializerSettings()
                );
            }

            ViewBag.CurrentYear = currentYear;
            ViewBag.CurrentSemester = currentSemester;

            ViewBag.Years = JsonConvert.SerializeObject(db.ProjectPeriods.Select(p => new { Year = p.Year }).Distinct().OrderBy(y => y.Year).ToList());
            ViewBag.Semesters = JsonConvert.SerializeObject(db.Semesters);
            
            return View();
        }

        public ActionResult DownloadProjectsReport(string filter)
        {
            var projects = GetFilteredProjects(filter);

            var filterRules = ObjectableFilterRules.Deserialize(filter);
            var years = GetListOfInt(filterRules, "Years");
            var semesters = GetListOfInt(filterRules, "Semesters").OrderBy(s => s);

            var stream = new VariantExport().Export(new
            {
                Rows = projects
            }, "projectsReportTemplate2.xlsx");
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Отчёт по проектам {string.Join(", ", years)} учебный год {string.Join(", ", semesters)} семестр.xlsx".ToDownloadFileName());
        }

        public ActionResult Students(string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                var students = GetFilteredStudents(filter);

                return Json(
                    new
                    {
                        data = students,
                        total = students.Count()
                    },
                    new JsonSerializerSettings()
                );
            }

            ViewBag.CurrentYear = currentYear;
            ViewBag.CurrentSemester = currentSemester;

            ViewBag.Years = JsonConvert.SerializeObject(db.ProjectPeriods.Select(p => new { Year = p.Year }).Distinct().OrderBy(y => y.Year).ToList());
            ViewBag.Semesters = JsonConvert.SerializeObject(db.Semesters);

            return View();
        }

        public ActionResult DownloadStudentsReport(string filter)
        {
            var students = GetFilteredStudents(filter);

            var filterRules = ObjectableFilterRules.Deserialize(filter);
            var years = GetListOfInt(filterRules, "Years");
            var semesters = GetListOfInt(filterRules, "Semesters").OrderBy(s => s);

            var stream = new VariantExport().Export(new
            {
                Rows = students
            }, "projectStudentsReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Отчёт по студентам на проектном обучении {string.Join(", ", years)} учебный год {string.Join(", ", semesters)} семестр.xlsx".ToDownloadFileName());
        }

        public IEnumerable<int> GetListOfInt (ObjectableFilterRules rules, string property)
        {
            var rule = rules?.FirstOrDefault(f => f.Property == property);
            if (rule != null)
            {
                var value = rule.Value;
                var list = value == null 
                    ? new List<int>()
                    : JsonConvert.DeserializeObject<List<int>>(value.ToString());
                //rules.Remove(rule);
                return list;
            }
            return new List<int>();
        }

        private IEnumerable<ProjectReportVM> GetFilteredProjects(string filter)
        {
            var projectsForUser = db.ProjectsForUser(User).Select(p => p.ModuleId).ToList();

            var rops = db.ProjectUsers.Include(u => u.Teacher).Where(u => u.Type == ProjectUserType.ROP).ToList();
            var curators = db.ProjectUsers.Include(u => u.Teacher).Where(u => u.Type == ProjectUserType.Curator).ToList();

            var filterRules = ObjectableFilterRules.Deserialize(filter);
            var years = GetListOfInt(filterRules, "Years");
            var semesters = GetListOfInt(filterRules, "Semesters");

            if (years.Count() == 0 || semesters.Count() == 0)
            {
                return new List<ProjectReportVM>();
            }
            
            var directionRule = filterRules?.FirstOrDefault(f => f.Property == "Direction");
            var directionFilter = directionRule != null
                ? directionRule.Value.ToString().ToLower()
                : "";
            // направление сравнивать с направлением из лимитов !!!
            
            var titleRule = filterRules?.FirstOrDefault(f => f.Property == "Title");
            var titleFilter = titleRule != null
                ? titleRule.Value.ToString().ToLower()
                : "";


            var projects = db.ProjectProperties.Include(p => p.Project.Module)
                .Include(p => p.Project.Contract.Company)
                .Include(p => p.ProjectCompetitionGroup)
                .Where(p => projectsForUser.Contains(p.ProjectId) && years.Contains(p.ProjectCompetitionGroup.Year) && semesters.Contains(p.ProjectCompetitionGroup.SemesterId)
                        && (titleFilter == "" || p.Project.Module.title.ToLower().Contains(titleFilter)))
                .Select(p => new
                {
                    limits = db.ContractLimits.Where(
                        l => l.Period.ContractId == p.Project.ContractId
                            && l.Period.SemesterId == p.ProjectCompetitionGroup.SemesterId && l.Period.Year == p.ProjectCompetitionGroup.Year
                            && (p.ProjectCompetitionGroup.StudentCourse == 0 || l.Course == p.ProjectCompetitionGroup.StudentCourse || l.Course == 0)
                            
                            ).Select(l => new
                            {
                                direction = l.Direction.okso + " " + l.Direction.title,
                                limit = l.Limit
                            }),
                    property = p,
                    rops = db.ProjectUsers.Where(u => u.Type == ProjectUserType.ROP && u.ProjectId == p.ProjectId)
                                    .Select(r => r.IsChief ? r.Teacher.initials + "*" : r.Teacher.initials).OrderBy(r => r),
                    curators = db.ProjectUsers.Where(u => u.Type == ProjectUserType.Curator && u.ProjectPropertyId == p.Id)
                                    .Select(c => c.Teacher.initials).OrderBy(c => c)
                })
                .Where(p => directionFilter == "" || p.limits.Any(l => l.direction.ToLower().Contains(directionFilter)))
                .Select(p => new
                {
                    limit = p.limits.Count() > 0 ? p.limits.Select(l => l.limit).Sum() : 0,
                    p.property,
                    p.rops,
                    p.curators
                })
                .Select(p => new ProjectReportVM()
                {
                    Title = p.property.Project.Module.title,
                    Level = p.property.Project.Module.Level,

                    Year = p.property.ProjectCompetitionGroup.Year,
                    Semester = p.property.ProjectCompetitionGroup.Semester.Name,
                    SemesterId = p.property.ProjectCompetitionGroup.SemesterId,
                    Course = p.property.ProjectCompetitionGroup.StudentCourse,
                    
                    Company = p.property.Project.Contract.Company.Name,
                    CompetitionGroup = p.property.ProjectCompetitionGroup.Name,

                    Rops = p.rops.ToList(),
                    Curators = p.curators.ToList(),

                    Limit = p.limit,

                    Selection =
                        db.ProjectStudentSelectionPriorities.Count(
                            s => (s.Project.ModuleId == p.property.ProjectId) && (s.competitionGroupId == p.property.ProjectCompetitionGroupId) && s.priority > 0),

                    Admission =
                        db.ProjectAdmissions.Count(
                            a =>
                                (a.ProjectId == p.property.ProjectId) &&
                                (a.ProjectCompetitionGroupId == p.property.ProjectCompetitionGroupId) &&
                                (a.Status == AdmissionStatus.Admitted)),

                    Vacancy = p.limit - db.ProjectAdmissions.Count(
                        a =>
                            (a.ProjectId == p.property.ProjectId) &&
                            (a.ProjectCompetitionGroupId == p.property.ProjectCompetitionGroupId) &&
                            (a.Status == AdmissionStatus.Admitted) &&
                            (a.Student.Status == "Активный" || a.Student.Status == "Отп.с.посещ."))
                })
                .ToList();

            return projects;
        }

        public IEnumerable<StudentsReportVM> GetFilteredStudents(string filter)
        {
            var projectsForUser = db.ProjectsForUser(User).Select(p => p.ModuleId).ToList();
            

            var filterRules = ObjectableFilterRules.Deserialize(filter);
            var years = GetListOfInt(filterRules, "Years");
            var semesters = GetListOfInt(filterRules, "Semesters");

            if (years.Count() == 0 || semesters.Count() == 0)
            {
                return new List<StudentsReportVM>();
            }

            var directionRule = filterRules?.FirstOrDefault(f => f.Property == "Direction");
            var directionFilter = directionRule != null
                ? directionRule.Value.ToString().ToLower()
                : "";
            // направление сравнивать с направлением из лимитов !!!

            var titleRule = filterRules?.FirstOrDefault(f => f.Property == "Title");
            var titleFilter = titleRule != null
                ? titleRule.Value.ToString().ToLower()
                : "";

            var studentRule = filterRules?.FirstOrDefault(f => f.Property == "Student");
            var studentFilter = studentRule != null
                ? studentRule.Value.ToString().ToLower()
                : "";

            var groupsForUser = db.GroupsForUser(User)
                .Where(g => directionFilter == "" || (g.Profile.Direction.okso + " " + g.Profile.Direction.title).ToLower().Contains(directionFilter))
                .Select(g => g.Id).ToList();

            var students = db.ProjectAdmissions
                .Include(a => a.Student.Person)
                .Include(a => a.ProjectCompetitionGroup)
                .Where(a => projectsForUser.Contains(a.ProjectId) && groupsForUser.Contains(a.Student.GroupId) 
                            && years.Contains(a.ProjectCompetitionGroup.Year) && semesters.Contains(a.ProjectCompetitionGroup.SemesterId)
                            && (titleFilter == "" || a.Project.Module.title.ToLower().Contains(titleFilter))
                            && (studentFilter == "" || (a.Student.Person.Surname + " " + a.Student.Person.Name + " " + a.Student.Person.PatronymicName).ToLower().Contains(studentFilter)))
                .Select(a => new
                {
                    admission = a,
                    priority = db.ProjectStudentSelectionPriorities
                        .Where(p => p.studentId == a.studentId && p.projectId == a.ProjectId && p.competitionGroupId == a.ProjectCompetitionGroupId)
                        .FirstOrDefault(),
                    subgroups = db.ProjectSubgroupMemberships.Where(s => s.studentId == a.studentId && s.Subgroup.Meta.CompetitionGroupId == a.ProjectCompetitionGroupId
                                    && s.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.ProjectId == a.ProjectId).Select(s => s.Subgroup.Name)
                })
                .Select(a => new StudentsReportVM()
                {
                    LastName = a.admission.Student.Person.Surname,
                    FirstName = a.admission.Student.Person.Name,
                    MiddleName = a.admission.Student.Person.PatronymicName,

                    Year = a.admission.ProjectCompetitionGroup.Year,
                    Semester = a.admission.ProjectCompetitionGroup.Semester.Name,
                    SemesterId = a.admission.ProjectCompetitionGroup.SemesterId,

                    Group = a.admission.Student.Group.Name,
                    Status = a.admission.Student.Status,
                    Compensation = a.admission.Student.Compensation,
                    CompetitionGroupShortName = a.admission.ProjectCompetitionGroup.ShortName,
                    Project = a.admission.Project.Module.title,
                    Level = a.admission.Project.Module.Level,

                    Priority = a.priority.priority,

                    AdmissionStatus = a.admission.Status,
                    Role = a.admission.Role.Title,

                    Subgroups =a.subgroups.ToList()
                })
                .ToList();

            return students;
        }
    }
}