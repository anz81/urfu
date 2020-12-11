using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.Its.Web.DataContext;
using Microsoft.EntityFrameworkCore;
//using System.Web.WebPages.Html;
using Microsoft.AspNetCore.Mvc;

namespace Urfu.Its.Frames.Controllers
{
    public class ProjectInfoVM
    {
        public string DisciplineUID { get; set; }
        public string ModuleId { get; set; }
        public string Name { get; set; }
        public int SemesterID { get; set; }
        public string Semester { get; set; }

        public bool IsPairedModule { get; set; }

        public string Level { get; set; }

        public string PairedModuleClass
        {
            get
            {
                // если есть парный модуль, делаем его видимым. Если нет - скрываем.
                return IsPairedModule ? "col-sm-4" : "col-sm-4 collapse";
            }
        }

        public string PairedModuleClassEmpty
        {
            get
            {
                // противоположный класс PairedModuleClass. 
                // Занимает место в строке в случае, если нет парного модуля.
                // hidden не сработал, поэтому делаем так.
                return IsPairedModule ? "col-sm-4 collapse" : "col-sm-4";
            }
        }

        public string PairedModuleName { get; set; }
        public string PairedModuleDocument { get; set; }
    }

    public class GroupProjectVM
    {
        private GroupsHistory _group;

        public string ID => _group.Id;
        public string Name => _group.Name;
        public int Year => _group.YearHistory;
        public int Course => _group.Course;
        public string YearInfo => $"{Year}/{Year + 1} уч.год";

        public List<ProjectInfoVM> Projects;

        public GroupProjectVM(GroupsHistory group)
        {
            _group = group;
            Projects = new List<ProjectInfoVM>();
        }

        public bool HaveProject => Projects.Count > 0;

        internal void Add(PlanVM p, TermVM t, PlanVM secondPlan)
        {
            var project = new ProjectInfoVM
            {
                Name = p.Plan.disciplineTitle,
                SemesterID = t.SemesterID,
                Semester = $"{t.Semester} семестр",
                DisciplineUID = p.Plan.disciplineUUID,
                ModuleId = p.Plan.moduleUUID,

                IsPairedModule = secondPlan != null,
                PairedModuleName = secondPlan?.Plan?.disciplineTitle ?? "1",
                PairedModuleDocument = null, //secondPlan?.File

                Level = p.Level
            };
            Projects.Add(project);
            Projects = Projects.OrderBy(pr => pr.SemesterID).ToList();
        }
    }

    public class ProjectMainList
    {
        public string StudentFIO { get; set; }
        public List<ProjectList> ProjectLists { get; set; }

        public ProjectMainList(ApplicationDbContext db, List<string> studentIds)
        {
            studentIds = studentIds ?? new List<string>();
            ProjectLists = new List<ProjectList>();
            foreach (var studentId in studentIds)
            {
                ProjectLists.Add(new ProjectList(db, studentId));
            }
            ProjectLists = ProjectLists.OrderBy(p => p.StudentId).ToList();
            StudentFIO = db.Students.FirstOrDefault(s => studentIds.Contains(s.Id))?.Person?.FullName();
        }

        public ProjectMainList(ApplicationDbContext db, string studentId)
        {
            ProjectLists = new List<ProjectList>();
            ProjectLists.Add(new ProjectList(db, studentId));
            ProjectLists = ProjectLists.OrderBy(p => p.StudentId).ToList();
            StudentFIO = db.Students.FirstOrDefault(s => s.Id == studentId)?.Person?.FullName();
        }
    }

    public class ProjectList
    {
        private ApplicationDbContext _db;
        public string StudentId { get; set; }

        public string StudentFIO { get; set; }

        public List<GroupProjectVM> Groups { get; set; }
        public List<PlanVM> Plans { get; set; }

        public ProjectList(ApplicationDbContext db, string studentId)
        {
            _db = db;
            StudentId = studentId;

            var student = _db.Students
                .Include(s => s.Person)
                .Where(s => s.Id == StudentId)
                .FirstOrDefault();

            var groups = _db.GroupsHistories
              .Where(h => h.GroupId == student.GroupId)
              .ToList();

            StudentFIO = student.Person.FullName();

            Groups = groups.Select(g => new GroupProjectVM(g)).ToList();

            var semesters = _db.Semesters.ToList();

            var planTerms = _db.PlanTerms;

            var plans = _db.Plans
                .Where(p => p.eduplanNumber == student.planVerion
                    && p.versionNumber == student.versionNumber
                    && p.Module.type == "Проектное обучение"
                    && !p.remove)
                .Include(p => p.Module)
                .Select(p => new
                {
                    p.Module.file,
                    p.Module.Level,
                    Plan = p,
                    PlanTerms = planTerms.Where(pt => pt.eduplanUUID == p.eduplanUUID)
                })
                .ToList();

            var secondPlans = _db.Plans
                .Where(p => p.eduplanNumber == student.planVerion
                    && p.versionNumber == student.versionNumber
                    && p.Module.type == "Парный модуль"
                    && !p.remove)
                .Include(p => p.Module.file)
                .Select(p => new
                {
                    p.Module.file,
                    p.Module.Level,
                    Plan = p,
                    PlanTerms = planTerms.Where(pt => pt.eduplanUUID == p.eduplanUUID)
                })
                .ToList();

            Plans = plans.Select(p => new PlanVM(p.Plan, p.PlanTerms.ToList(), semesters, null, p.Level)).ToList();
            var secondPlansVM = secondPlans.Select(p => new PlanVM(p.Plan, p.PlanTerms.ToList(), semesters, p.file, p.Level)).ToList();

            foreach (var p in Plans)
            {
                var terms = p.GetTerms();
                foreach (var t in terms)
                {
                    if (t == null)
                        continue;

                    var group = Groups.LastOrDefault(g => g.Course == t.Course);
                    if (group != null)
                    {
                        PlanVM secondPlan = null;
                        // только у модулей уровня А может быть парный модуль
                        if (p.Level == "A" || p.Level == "А") // первая А англ., вторая - рус.
                            secondPlan = secondPlansVM.FirstOrDefault(sp => sp.GetTerms().Contains(t));

                        group.Add(p, t, secondPlan);
                    }
                }
            }
        }
    }

}