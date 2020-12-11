using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Urfu.Its.Web.DataContext;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Urfu.Its.Practices;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Urfu.Its.Frames.Controllers
{
    public class CompanyProjectVM : CompanyVM
    {
        public string Class { get; set; }
        public CompanyProjectVM(string address, string name, string direction, int periodId, string site, string personInCharge, string email, bool admitted = false)
        {
            Address = address;
            Name = name;
            Direction = direction;
            PeriodID = periodId;
            Site = site;
            PersonInCharge = personInCharge;
            Email = email;
            Class = admitted ? "panel-collapse" : "panel-collapse collapse";
        }
        public List<Project2VM> Projects { get; set; }
    }

    public class Project2VM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Limit { get; set; }
        public bool HasProjectDescription { get; set; }
        public string Curator { get; set; }
        public int? Priority { get; set; }

        public string Description { get; set; }
        public string Target { get; set; }

        public IEnumerable<ProjectRole> Roles { get; set; }

        public ProjectRole Role { get; set; }

        public string RolesInfo { get; set; }

        public string SelectionDeadline { get; set; }

        /// <summary>
        /// проект неактивен в случаях, когда
        /// 1. Дата выбора проекта прошла
        /// 2. Студент уже зачислен на какой-то проект -> неактивны ВСЕ проекты
        /// </summary>
        public bool IsActive { get; set; }
        public AdmissionStatus? Status { get; set; }
        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case AdmissionStatus.Admitted: return "Зачислен";
                    case AdmissionStatus.Denied: return "Не зачислен";
                    case AdmissionStatus.Indeterminate: return "На расcмотрении";
                }

                return "";
            }
        }

        public string Comment { get; set; }

    }

    public class ProjectVM
    {
        private ApplicationDbContext _db;
        private bool _isAdmin;

        private Student _student;

        public string StudentID { get; set; }
        public string GroupHistoryID { get; set; }
        public int Course { get; set; }
        public string DisciplineUUID { get; set; }
        public int CompetitionGroupId { get; set; }

        public string DisciplineTitle { get; set; }

        public string InstituteTitle { get; set; }

        public int Year { get; set; }

        public string YearInfo => $"{Year}/{Year % 100 + 1} уч.год";

        public string Semester { get; set; }
        public int SemesterId { get; set; }

        public int MinPriority { get; set; }
        public int MaxPriority { get; set; }
        public IEnumerable<int> UsedPriorities { get; set; }

        /// <summary>
        /// Выбранный уровень модуля из uni (A, B, C, BC)
        /// </summary>
        public string Level { get; set; }

        public string Search { get; set; }

        public List<CompanyProjectVM> Companies { get; private set; }

        public string[] Modules { get; private set; }
        //{
        //    get
        //    {
        //        return Companies?.SelectMany(c => c.Projects)?.Select(p => p.Id)?.ToArray() ?? new string[0];
        //    }
        //}

        public bool ShowMessage { get; set; }

        public string AdmissionProjectName { get; set; }
        public string TeamProjectAddress { get; set; }

        public ProjectVM()
        {
        }

        public ProjectVM(ApplicationDbContext db, string studentId, string groupId, int year, int semesterId, string disciplineUID, string level, string search, bool isAdmin)
        {
            _db = db;
            _isAdmin = isAdmin;
            _student = _db.Students.FirstOrDefault(s => s.Id == studentId);

            GroupHistoryID = groupId;
            DisciplineUUID = disciplineUID;
            Year = year;
            SemesterId = semesterId;
            Semester = db.Semesters.FirstOrDefault(s => s.Id == semesterId).Name;
            Search = search;

            Level = level;

            Companies = GetCompanyProjects();
            Modules = Companies.SelectMany(c => c.Projects).Select(p => p.Id).ToArray();

            UsedPriorities = Companies.SelectMany(c => c.Projects).Where(c => c.Priority != null).Select(c => c.Priority.Value);
        }

        private List<CompanyProjectVM> GetCompanyProjects()
        {
            StudentID = _student.Id;

            var plan = _db.Plans.FirstOrDefault(p => p.disciplineUUID == DisciplineUUID);

            DisciplineTitle = plan.disciplineTitle;

            var direction = _db.Directions.FirstOrDefault(d => d.uid == plan.directionId);

            var historyGroup = _db.GroupsHistories.FirstOrDefault(g => g.Id == GroupHistoryID);
            Course = historyGroup.Course;

            var compGroup = _db.ProjectCompetitionGroups.Include(p => p.ProjectProperties).FirstOrDefault(
                        pg => pg.SemesterId == SemesterId
                            && (pg.StudentCourse == 0 || pg.StudentCourse == Course)
                            && pg.Year == Year
                            && pg.Groups.Any(g => g.Id == historyGroup.GroupId)
                            );

            if (compGroup == null)
            {
                return new List<CompanyProjectVM>();
            }

            CompetitionGroupId = compGroup.Id;

            var admissionProject = _db.ProjectAdmissions
                .FirstOrDefault(a => a.studentId == StudentID && a.Status == AdmissionStatus.Admitted && a.Published
                        && a.ProjectCompetitionGroupId == CompetitionGroupId);

            FillAdmissionProperties(admissionProject);

            var projects = _db.ProjectPeriods
                .Include(p => p.Project)
                .Include(p => p.Project.Contract.Company)
                .Where(
                    p =>
                        Level.Contains(p.Project.Module.Level) // A = A, B = B, C = C, BC = или В, или C
                        && p.Year == Year && p.SemesterId == SemesterId
                        && (p.Course == null || p.Course == Course)
                        && (p.SelectionBegin == null || p.SelectionBegin <= DateTime.Now)
                        && p.Project.ShowInLC
                        //&& (p.SelectionDeadline == null || p.SelectionDeadline > DateTime.Now)
                        )
                .Select(p => new
                {
                    limit = p.Project.Contract.Periods.FirstOrDefault(
                        pr => pr.SemesterId == SemesterId
                            && pr.Year == Year)
                            .Limits
                            .Where(
                                l => (l.DirectionId == null || l.DirectionId == direction.uid)
                                && (l.Course == 0 || l.Course == Course)
                                && (l.QualificationName == null || l.QualificationName == historyGroup.Qual)
                                && (l.ProfileId == null || l.ProfileId == historyGroup.ProfileId))
                            .Select(l => l.Limit
                                -
                                    _db.ProjectAdmissions.Count(
                                        a => (a.ProjectCompetitionGroupId == CompetitionGroupId)
                                            && a.Status == AdmissionStatus.Admitted
                                            && a.ProjectId == p.ProjectId
                                            && (a.Student.Group.Profile.ID == historyGroup.ProfileId)
                                            && (a.Student.Group.Profile.DIRECTION_ID == direction.uid))),
                    p.Project,
                    p.SelectionDeadline
                })
                .ToList()
                .Where(pl => pl.limit.Count() > 0 || pl.Project.ModuleId == admissionProject?.ProjectId)
                .Select(p => new
                {
                    p.limit,
                    p.Project,
                    curators = compGroup.ProjectProperties.FirstOrDefault(pp => pp.ProjectId == p.Project.ModuleId)?
                            .ProjectUsers?
                            .Where(u => u.Type == ProjectUserType.Curator)?.Select(u => u.Teacher.initials),
                    p.SelectionDeadline,
                    priority = _db.ProjectStudentSelectionPriorities.FirstOrDefault(pr => pr.competitionGroupId == compGroup.Id
                                    && pr.projectId == p.Project.ModuleId && pr.studentId == StudentID)
                })
                .GroupBy(pl => pl.Project.Contract.Company)
                .Select(pl => new CompanyProjectVM(
                    pl.Key.Address,
                    pl.Key.Name,
                    direction.OksoAndTitle,
                    pl.Key.Id,
                    pl.Key.Site,
                    pl.Key.PersonInChargeInitials,
                    pl.Key.Email,
                    admitted: pl.Any(p => p.Project.ModuleId == admissionProject?.ProjectId)
                    )
                {
                    Projects = pl.Select(p => new Project2VM()
                    {
                        Name = p.Project.Module.title,
                        Limit = p.limit.Sum(),
                        HasProjectDescription = p.Project.Module.file != null,
                        Id = p.Project.ModuleId,
                        Curator = p.curators != null ? string.Join(", ", p.curators) : "",
                        Priority = p.priority?.priority,
                        IsActive = _isAdmin && (admissionProject != null ? false : (p.SelectionDeadline == null || p.SelectionDeadline > DateTime.Now)),
                        Status = _db.ProjectAdmissions.FirstOrDefault(
                            a => a.studentId == StudentID && a.Published && a.ProjectCompetitionGroupId == CompetitionGroupId && a.ProjectId == p.Project.ModuleId)?.Status,
                        Description = p.Project.Description,
                        Target = p.Project.Target,
                        Roles = p.Project.Roles ?? new List<ProjectRole>(),
                        Role = p.Project.ModuleId == admissionProject?.ProjectId ? admissionProject.Role : p.priority?.Role,
                        RolesInfo = string.Join("", p.Project.Roles.Select(r => $"<b>{r.Title}</b> - {r.Description}.<br/>")),
                        SelectionDeadline = p.SelectionDeadline.HasValue ? p.SelectionDeadline.Value.ToShortDateString() : "",
                        Comment = p.priority?.Comment
                    }).ToList()
                })
                .ToList();

            MinPriority = 1;
            MaxPriority = Math.Min(projects.SelectMany(c => c.Projects).Where(c => c.IsActive).Count(), 10);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var search = Search.ToLower();
                var companies = projects.Where(p => p.Name.ToLower().Contains(search)).Select(c => c.PeriodID);

                projects.ForEach(p =>
                {
                    if (!companies.Contains(p.PeriodID))
                        p.Projects.RemoveAll(pr => !(pr.Name.ToLower().Contains(search) || pr.Curator.ToLower().Contains(search)));
                });

                projects.RemoveAll(p => p.Projects.Count == 0);
            }

            return projects;
        }

        private void FillAdmissionProperties(ProjectAdmission admissionProject)
        {
            if (admissionProject == null)
                return;

            AdmissionProjectName = admissionProject?.Project?.Module?.title;

            var subgroupMember = _db.ProjectSubgroupMemberships.FirstOrDefault(s => s.studentId == StudentID
                                    && s.Subgroup.Meta.CompetitionGroupId == admissionProject.ProjectCompetitionGroupId
                                    && s.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.ProjectId == admissionProject.ProjectId
                                    && s.Subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.TmerId == "prex");

            if (subgroupMember == null)
                return;

            var teamProjectAddress = ConfigurationManager.AppSettings["TeamProjectAddress"];
            TeamProjectAddress = string.Format(teamProjectAddress, subgroupMember.SubgroupId);
        }
    }
}