using Ext.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using PagedList;
using Ext.Utilities.Linq;
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
using PagedList.Core;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
//using System.Web.Script.Serialization;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.ProjectView)]
    public class ProjectController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? page, int? limit, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var sortRules = SortRules.Deserialize(sort);

                var projectRops = db.ProjectUsers.Include(u => u.Teacher).Where(u => u.Type == ProjectUserType.ROP).ToList();
                var userId = User.Identity.GetUserId();

                var modules = GetFilteredModules(filter);
                var total = modules.Count();

                if (sortRules?.Count > 0)
                {
                    var sortRule = sortRules[0];
                    modules = modules.OrderBy(sortRule);
                }

                var paginated = modules.ToPagedList(page ?? 1, limit ?? 25);

                var result =  modules.ToList().Select(m => new
                {
                    id = m.uuid,
                    m.title,
                    m.shortTitle,
                    m.type,
                    competence = GetCompetencesStr(m.uuid),
                    m.testUnits,
                    m.priority,
                    m.state,
                    approvedDate = m.approvedDate.HasValue
                        ? m.approvedDate.Value.Day.ToString() + (m.approvedDate.Value.Month < 10 ? ".0" : ".") +
                          m.approvedDate.Value.Month.ToString() + "." + m.approvedDate.Value.Year.ToString()
                        : "",
                    m.comment,
                    m.file,
                    m.specialities,
                    m.Level,
                    rops = string.Join(", ", projectRops.Where(p => p.ProjectId == m.uuid).Select(p => p.IsChief ? p.Teacher.initials + "*" : p.Teacher.initials).ToList()),
                    summary = m.Project?.Summary,
                    projectroles = string.Join(", ", db.ProjectRoles.Where(r => r.ProjectId == m.uuid).Select(r => r.Title).ToList()),

                    // редактировать может только РОП, назначенный на проект, или project manager
                    canEdit = db.CanEditProject(User, m.uuid)

                });


                return Json(
                    new
                    {
                        data = result,
                        total = total
                    },
                    new Newtonsoft.Json.JsonSerializerSettings()

                );
            }
            ViewBag.Focus = focus;
            ViewBag.CanEdit = User.IsInRole(ItsRoles.ProjectManager) || User.IsInRole(ItsRoles.ProjectROP);
            return View();
        }

        public ActionResult DownloadProjectReport(string filter)
        {
            var projects = GetFilteredModules(filter);

            var stream = new VariantExport().Export(new
            {
                Rows = projects.ToList().Select(m => new
                {
                    m.title,
                    m.shortTitle,
                    level = m.Level ?? "",
                    m.specialities,
                    rops = string.Join(", ", db.ProjectUsers.Where(p => p.ProjectId == m.uuid && p.Type == ProjectUserType.ROP)
                                .Select(p => p.IsChief ? p.Teacher.initials + "*" : p.Teacher.initials).ToList())
                })
            }, "projectsReportTemplate.xlsx");


            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Отчёт по проектам.xlsx".ToDownloadFileName());
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

        public ActionResult GetProjectRops(string projectId)
        {
            var existedRops = db.ProjectUsers.Where(t => t.Type == ProjectUserType.ROP && t.ProjectId == projectId).ToList();

            var ropRoles = db.RoleSets.Include(rs => rs.Contents).FirstOrDefault(rs => rs.Id == 1).Contents.Select(c => c.RoleId);

            var ropTeachers = db.Teachers.Where(t => t.User != null).Where(t => ropRoles.All(r => t.User.Roles.Select(ur => ur.RoleId).Contains(r))).ToList();

            var result = ropTeachers.Select(t => new
            {
                IsSelected = existedRops.Any(p => p.TeacherId == t.pkey),
                IsChief = existedRops.FirstOrDefault(p => p.TeacherId == t.pkey)?.IsChief ?? false,
                Id = t.pkey,
                LastName = t.lastName,
                FirstName = t.firstName,
                Patronymic = t.middleName,
                Fio = t.FullName
            }).OrderBy(u => u.Fio).ToList();

            return Json(result, new Newtonsoft.Json.JsonSerializerSettings());
        }

        public void SetProjectRops(string projectId, string[] teacherIds, string chiefUser)
        {
            var project = db.Projects.FirstOrDefault(p => p.ModuleId == projectId);
            if (project == null)
                return;

            db.ProjectUsers.RemoveRange(db.ProjectUsers.Where(u =>
                u.Type == ProjectUserType.ROP
                && u.ProjectId == projectId
                && !teacherIds.ToList().Contains(u.TeacherId)
                ));

            foreach (var id in teacherIds)
            {
                if (!db.ProjectUsers.Any(p => p.Type == ProjectUserType.ROP && p.TeacherId == id && p.ProjectId == projectId))
                {
                    // добавить нового РОПа
                    db.ProjectUsers.Add(new ProjectUser()
                    {
                        IsChief = id == chiefUser,
                        ProjectId = projectId,
                        Type = ProjectUserType.ROP,
                        TeacherId = id
                    });
                }
                else
                {
                    // изменить существующего
                    db.ProjectUsers.FirstOrDefault(p => p.Type == ProjectUserType.ROP && p.TeacherId == id && p.ProjectId == projectId)
                        .IsChief = id == chiefUser;
                }
            }
            db.SaveChanges();
        }

        public ActionResult Company(string id)
        {
            var project = db.Projects.FirstOrDefault(p => p.ModuleId == id);
            if (project == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            return RedirectToAction("Contracts", "PracticeCompanies", new { id = project.Contract.CompanyId, contractId = project.ContractId });
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var module = db.ProjectModules().Include("Project.Periods").FirstOrDefault(m => m.uuid == id);

            if (module == null)
                return NotFound();

            var techSelector = new SelectList(db.ModuleTeches, "Id", "Name", module.Project?.ModuleTechId);

            var semesters = db.Semesters.ToList();
            var semesterSelector = new SelectList(semesters, "Id", "Name");

            var model = new ProjectEditViewModel
            {
                Module = module,
                moduleUUId = module.Project == null ? string.Empty : module.uuid,
                showInLc = module.Project?.ShowInLC ?? false,
                techid = module.Project?.ModuleTechId.ToString() ?? string.Empty,
                tech = module.Project?.Tech.Name ?? string.Empty,
                TechSelector = techSelector,
                SemesterSelector = semesterSelector,
                Competences = GetCompetencesStr(module.uuid)
            };

            if (module.Project == null)
                model.periods = new List<ProjectPeriodEditViewModel>();
            else
                model.periods = module.Project.Periods.Select(p =>
                        new ProjectPeriodEditViewModel
                        {
                            id = p.Id,
                            year = p.Year,
                            semesterId = p.SemesterId,
                            semesterName = p.Semester.Name,
                            Selector = new SelectList(semesters, "Id", "Name", p.SemesterId),
                            selectionDeadline = p.SelectionDeadline
                        }
                ).ToList();

            bool canEdit = db.CanEditProject(User, id);

            ViewBag.CanEdit = canEdit;
            ViewBag.CanLimitEdit = canEdit;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var project = db.Projects.Include(p => p.Contract.Periods).FirstOrDefault(p => p.ModuleId == model.Module.uuid);
                if (project == null)
                    return NotFound("Validation error");

                if (User.IsInRole(ItsRoles.ProjectManager) || User.IsInRole(ItsRoles.ProjectROP))
                {
                    foreach (var p in model.periods)
                    {
                        var contractPeriods = db.ContractPeriods.Where(c => c.ContractId == project.ContractId).ToList();

                        if (p.id < 0)
                            p.id = 0;

                        //добавили и тут же удалили
                        if (p.isDeleted && (p.id == 0)) continue;

                        var period = new ProjectPeriod
                        {
                            Id = p.id,
                            ProjectId = project.ModuleId,
                            Year = p.year,
                            SemesterId = p.semesterId,
                            SelectionDeadline = p.selectionDeadline
                        };

                        // удалить период
                        if (p.isDeleted)
                        {
                            db.Entry(period).State = EntityState.Deleted;
                            db.ContractPeriods.RemoveRange(contractPeriods.Where(cp => cp.Year == p.year && cp.SemesterId == p.semesterId));
                        }
                        else
                        {
                            if (p.id == 0) // добавить период
                            {
                                project.Periods.Add(period);
                                if (!contractPeriods.Any(cp => cp.Year == period.Year && cp.SemesterId == period.SemesterId))
                                {
                                    db.ContractPeriods.Add(new ContractPeriod()
                                    {
                                        ContractId = project.ContractId.Value,
                                        SemesterId = p.semesterId,
                                        Year = p.year
                                    });
                                }
                            }
                            else // обновить период
                            {
                                var dbPeriod = db.ProjectPeriods.FirstOrDefault(pp => pp.Id == p.id);
                                // обновить лимит в договоре
                                var contractPeriod = contractPeriods.FirstOrDefault(cp => cp.Year == dbPeriod.Year && cp.SemesterId == dbPeriod.SemesterId);
                                contractPeriod.Year = p.year;
                                contractPeriod.SemesterId = p.semesterId;

                                dbPeriod.Year = p.year;
                                dbPeriod.SemesterId = p.semesterId;
                                dbPeriod.SelectionDeadline = p.selectionDeadline;
                            }
                        }

                        db.SaveChanges();
                    }
                    project.ShowInLC = model.showInLc;
                    project.ModuleTechId = int.Parse(model.techid);

                    db.Entry(project).State = string.IsNullOrEmpty(model.moduleUUId) ? EntityState.Added : EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index", new { focus = model.moduleUUId });
            }

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
            var project = db.Projects.FirstOrDefault(p => p.ModuleId == moduleId);

            ViewBag.Title = string.Format(@"Дисциплины для проекта ""{0}""", project.Module.title);
            ViewBag.ProjectId = moduleId;
            ViewBag.CanEdit = db.CanEditProject(User, moduleId);

            var model = new List<ProjectDisciplineViewModel>(project.Disciplines.Count);
            foreach (var d in project.Disciplines)
            {
                var r = new ProjectDisciplineViewModel
                {
                    Discipline = d.Discipline,
                    ProjectDiscipline =
                        project.Disciplines.FirstOrDefault(f => f.DisciplineUid == d.DisciplineUid)
                };

                model.Add(r);
            }

            return View(model);
        }

        private IQueryable<Module> FilterPeriod(FilterRule filterYear, FilterRule filterSemester)
        {
            int year;
            int semester;

            var haveYear = int.TryParse(filterYear?.Value, out year);
            var haveSemester = int.TryParse(filterSemester?.Value, out semester);

            if (haveYear && haveSemester)
                return
                    db.ProjectsForUser(User)
                        .Where(m => m.Periods.Any(p => (p.Year == year) && (p.SemesterId == semester))).Select(p => p.Module);

            if (haveYear)
                return db.ProjectsForUser(User).Where(m => m.Periods.Any(p => p.Year == year)).Select(p => p.Module);

            if (haveSemester)
                return db.ProjectsForUser(User).Where(m => m.Periods.Any(p => p.SemesterId == semester)).Select(p => p.Module);

            return db.ProjectsForUser(User).Select(p => p.Module);
        }

        [Authorize(Roles = ItsRoles.ProjectView)]
        public ActionResult EditTmers(string projectId, string disciplineId)
        {
            var keys = new List<int?>(3) { 1, 2, 3 };
            var tmers = db.Tmers.Where(t => keys.Contains(t.kgmer)
                                            && (t.kmer != "U039")
                                            && (t.kmer != "U032")
                                            && (t.kmer != "U033"))
                .ToList();

            var project =
                db.Projects.Include("Disciplines.Tmers").Single(m => m.ModuleId == projectId);
            var discipline = db.Disciplines.Single(d => d.uid == disciplineId);

            var model = new ProjectTmersViewModel(project, discipline, tmers);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.ProjectView)]
        public ActionResult EditTmers(ProjectTmersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var project =
                    db.Projects
                        .Include("Disciplines.Tmers")
                        .Single(m => m.ModuleId == model.Project.ModuleId);
                var projectDiscipline =
                    project.Disciplines.FirstOrDefault(d => d.DisciplineUid == model.Discipline.uid);
                if (projectDiscipline == null)
                {
                    projectDiscipline = new ProjectDiscipline
                    {
                        DisciplineUid = model.Discipline.uid,
                        ProjectId = project.ModuleId,
                        Tmers = new List<ProjectDisciplineTmer>()
                    };
                    project.Disciplines.Add(projectDiscipline);
                }

                var tmers = new List<ProjectTmersRowViewModel>();
                tmers.AddRange(model.Tmers1);
                tmers.AddRange(model.Tmers2);
                tmers.AddRange(model.Tmers3);

                foreach (var t in tmers)
                {
                    var dt = projectDiscipline.Tmers.FirstOrDefault(f => f.TmerId == t.TmerId);
                    if ((dt == null) && t.Checked)
                    {
                        dt = new ProjectDisciplineTmer { TmerId = t.TmerId };
                        projectDiscipline.Tmers.Add(dt);
                        //db.Entry(dt).State = EntityState.Added;
                    }
                    if ((dt != null) && !t.Checked)
                    {
                        projectDiscipline.Tmers.Remove(dt);
                        db.Entry(dt).State = EntityState.Deleted;
                    }
                }

                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("Tmers",
                        new
                        {
                            id = projectDiscipline.Id,
                            message = "Невозможно удалить нагрузки, т.к. на них есть подгруппы"
                        });
                }

                return RedirectToAction("Tmers", new { id = projectDiscipline.Id });
            }
            return RedirectToAction("Disciplines", new { moduleId = model.Project.ModuleId });
        }

        public ActionResult Tmers(int id, string message)
        {
            ViewBag.Message = message;

            var discipline =
                db.ProjectDisciplines
                    .Include("Tmers.Periods")
                    .Include("Project")
                    .Include("Discipline")
                    .Single(d => d.Id == id);

            ViewData.Add("Tmers1", discipline.Tmers.Where(t => t.Tmer.kgmer == 1).OrderBy(t => t.Tmer.kmer).ToList());
            ViewData.Add("Tmers2", discipline.Tmers.Where(t => t.Tmer.kgmer == 2).OrderBy(t => t.Tmer.kmer).ToList());
            ViewData.Add("Tmers3", discipline.Tmers.Where(t => t.Tmer.kgmer == 3).OrderBy(t => t.Tmer.kmer).ToList());

            ViewBag.CanEdit = db.CanEditProject(User, discipline.ProjectId);

            return View(discipline);
        }

        [Authorize(Roles = ItsRoles.ProjectView)]
        public ActionResult EditPeriods(string projectId, string disciplineId)
        {
            var project =
                db.Projects.Include(m => m.Periods).Single(m => m.ModuleId == projectId);
            var discipline =
                db.ProjectDisciplines
                    .Include("Tmers.Periods")
                    .Single(d => (d.ProjectId == projectId) && (d.DisciplineUid == disciplineId));

            var model = new ProjectTmersPeriodViewModel(project, discipline);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.ProjectView)]
        public ActionResult EditPeriods(ProjectTmersPeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var projectDiscipline =
                    db.ProjectDisciplines
                        .Include("Tmers.Periods")
                        .Single(d => d.Id == model.Discipline.Id);

                foreach (var t in model.Rows)
                {
                    var dt = projectDiscipline.Tmers.FirstOrDefault(f => f.Id == t.Tmer.Id);
                    var tp = dt.Periods.FirstOrDefault(f => f.ProjectPeriodId == t.Period.Id);
                    if ((tp == null) && t.Checked)
                    {
                        tp = new ProjectDisciplineTmerPeriod
                        {
                            ProjectDisciplineTmerId = t.Tmer.Id,
                            ProjectPeriodId = t.Period.Id
                        };
                        dt.Periods.Add(tp);
                        //db.Entry(dt).State = EntityState.Added;
                    }
                    if ((tp != null) && !t.Checked)
                    {
                        if (tp.ProjectSubgroupCounts.Any(c => c.Subgroups.Any()))
                            return RedirectToAction("Tmers",
                                new
                                {
                                    id = projectDiscipline.Id,
                                    message = "Невозможно удалить периоды, т.к. на них есть подгруппы"
                                });

                        if (tp.ProjectSubgroupCounts.Any())
                            tp.ProjectSubgroupCounts.Clear();

                        dt.Periods.Remove(tp);
                        db.Entry(tp).State = EntityState.Deleted;
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Tmers", new { id = projectDiscipline.Id });
            }
            return RedirectToAction("Disciplines", new { moduleId = model.Project.ModuleId });
        }

        [Authorize(Roles = ItsRoles.ProjectView)]
        public ActionResult EditDivisions(int projectDisciplineTmerPeriodId)
        {
            var period =
                db.ProjectTmerPeriods.Where(t => t.Id == projectDisciplineTmerPeriodId).Single();

            var divisions = db.Divisions.ToList();

            var model = new MinorDivisionViewModel(divisions, period, period.Tmer.ProjectDisciplineId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.ProjectView)]
        public ActionResult EditDivisions(MinorDivisionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var period =
                    db.ProjectTmerPeriods
                        .Include(p => p.Divisions)
                        .Include(p => p.Tmer)
                        .Single(p => p.Id == model.PeriodId);

                var rows = model.GetAllRows();
                foreach (var d in rows)
                {
                    var pd = period.Divisions.FirstOrDefault(f => f.uuid == d.DivisionID);

                    if ((pd == null) && d.Selected)
                    {
                        var division = new Division { uuid = d.DivisionID };
                        period.Divisions.Add(division);
                        db.Entry(division).State = EntityState.Unchanged;
                    }
                    if ((pd != null) && !d.Selected)
                        period.Divisions.Remove(pd);
                }

                db.SaveChanges();

                return RedirectToAction("Tmers", new { id = period.Tmer.ProjectDisciplineId });
            }
            return View(model);
        }

        public ActionResult CompetitionGroups(string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var filterRules = FilterRules.Deserialize(filter);

                var filterCourse = filterRules?.Find(f => f.Property == "StudentCourse");
                var filterName = filterRules?.Find(f => f.Property == "Name");

                if (filterRules != null)
                {
                    filterRules.Remove(filterCourse);
                    filterRules.Remove(filterName);
                }
                int course = -1;
                bool isFilterCourse = int.TryParse(filterCourse?.Value, out course);

                string projectGroupName = filterName?.Value?.ToString()?.ToLower();
                bool isFilterName = !string.IsNullOrWhiteSpace(projectGroupName);

                var competitionGroups = db.ProjectCompetitionGroupsForUser(User).Where(c => 
                    (isFilterName && (c.Name.ToLower().Contains(projectGroupName) || c.ShortName.ToLower().Contains(projectGroupName)) || !isFilterName)
                    && (isFilterCourse && (c.StudentCourse == course || course == 0) || !isFilterCourse)).Select(c => new
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
                    db.ProjectAdmissions.Any(
                        a => (a.ProjectCompetitionGroupId == c.Id) && (a.Status == AdmissionStatus.Admitted)),
                    AdmissionCount =
                    db.ProjectAdmissions.Count(
                        a => (a.ProjectCompetitionGroupId == c.Id) && (a.Status == AdmissionStatus.Admitted)),

                    canEdit = db.CanEditProjectCompetitionGroup(User, c.Id)
                });

                competitionGroups = competitionGroups.Where(filterRules);

                return Json(
                    new
                    {
                        data = competitionGroups
                    },
                    new JsonSerializerSettings()
                ); ;
            }
            ViewBag.CanEdit = User.IsInRole(ItsRoles.ProjectManager) || User.IsInRole(ItsRoles.ProjectROP);
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrUpdateCompetitionGroup(ProjectCompetitionGroup competitionGroup)
        {
            if (ModelState.IsValid)
            {
                competitionGroup.Name = competitionGroup.Name?.Replace("\n", " ");
                competitionGroup.ShortName = competitionGroup.ShortName?.Replace("\n", " ");

                var competitionGroupFromBase =
                    db.ProjectCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroup.Id);

                var action = "Редактирование";
                if (competitionGroupFromBase == null)
                {
                    db.ProjectCompetitionGroups.Add(competitionGroup);
                    action = "Создание";
                }
                else
                {
                    var hasAdmissions =
                        db.ProjectAdmissions.Any(
                            _ =>
                                (_.ProjectCompetitionGroupId == competitionGroup.Id) &&
                                (_.Status == AdmissionStatus.Admitted));
                    if (hasAdmissions)
                    {
                        competitionGroup.StudentCourse = competitionGroupFromBase.StudentCourse;
                    }
                    else if (competitionGroup.StudentCourse != competitionGroupFromBase.StudentCourse)
                    {
                        var admissions =
                            db.ProjectAdmissions.Where(
                                _ => _.ProjectCompetitionGroupId == competitionGroup.Id);
                        db.ProjectAdmissions.RemoveRange(admissions);
                        competitionGroupFromBase.Groups.Clear();
                    }
                    try
                    {
                        db.ProjectCompetitionGroups.Update(competitionGroup);
                    }
                    catch
                    {
                        db.ProjectCompetitionGroups.Add(competitionGroup);
                    }
                }
                db.SaveChanges();

                Logger.Info($"{action} проектной группы Id: {competitionGroup.Id} Name: {competitionGroup.Name}");

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            return NotFound("Validation error");
        }

        public ActionResult CompetitionGroupContentsTree(int? competitionGroupId, int? course, string filter)
        {
            if (competitionGroupId == null) return new StatusCodeResult(StatusCodes.Status404NotFound);

            var competitionGroup = db.ProjectCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);

            var divisions = db.Divisions.ToList();
            var directions = db.DirectionsForUser(User).Select(d => d.uid).ToList();
            var profiles = db.Profiles.Where(p => directions.Contains(p.DIRECTION_ID) && !p.remove).Select(p => p.ID);

            List<GroupRow> groups =
                db.GroupsHistories.Where(_ => _.YearHistory == competitionGroup.Year
                            && (_.Course == course || course == null || course == 0)
                            && (_.Qual != "Аспирант") //&& (_.FamType == "Очная")
                            && profiles.Contains(_.ProfileId)
                            ).Where(FilterRules.Deserialize(filter))
                    .Select(_ => new GroupRow()
                    {
                        Id = _.GroupId,
                        Name = _.Name,
                        ChairId = _.ChairId
                    }).OrderBy(g => g.Name).ToList();
            var model = new CompetitionGroupContentsViewModel(divisions, groups, competitionGroup);
            return JsonNet(model.Roots);
        }

        [HttpPost]
        public ActionResult UpdateCompetitionGroupContents(int competitionGroupId, List<string> groupsIds, List<string> deselectedGroups, bool isFiltered)
        {
            var competitionGroup = db.ProjectCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
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
                                    (!db.ProjectCompetitionGroups.Any(
                                        c =>
                                            c.Id != competitionGroupId && c.Year == competitionGroup.Year &&
                                            c.SemesterId == competitionGroup.SemesterId && c.Groups.Any(g => g.Id == _.Id))))
                            .ToList();
                    var badGroupIds = groupsIds.Where(_ => !groups.Any(g => g.Id == _));
                    badGroups = db.Groups.Where(_ => badGroupIds.Contains(_.Id)).Select(_ =>
                        new
                        {
                            competitionGroupName = db.ProjectCompetitionGroups.FirstOrDefault(
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
                    if (groupsIds != null)
                    {
                        var toDeleteIds = competitionGroup.Groups.Select(_ => _.Id).Except(groupsIds);
                        LogCompetitionGroupChanges("удалена {0} {1} из {2}", toDeleteIds, competitionGroup);

                        RemoveAdmissionsForGroup(competitionGroupId, toDeleteIds);
                    }
                    competitionGroup.Groups.Clear();
                }
                else if (deselectedGroups != null)
                {
                    var toDelete = competitionGroup.Groups.Where(_ => deselectedGroups.Contains(_.Id)).ToList();
                    var toDeleteIds = toDelete.Select(_ => _.Id);
                    RemoveAdmissionsForGroup(competitionGroupId, toDeleteIds);
                    LogCompetitionGroupChanges("удалена {0} {1} из {2} Проектной группы", toDeleteIds, competitionGroup);
                    foreach (var @group in toDelete)
                    {
                        competitionGroup.Groups.Remove(@group);

                    }
                }

                foreach (var group in groups)
                {
                    competitionGroup.Groups.Add(group);
                    Logger.Info("добавлен {0} {1} в {2} проектную группу", group.Name, group.Id, competitionGroup.Name);

                }
                db.SaveChanges();
                return Json(badGroups);
            }
            return NotFound("competition group not found");
        }
        private void LogCompetitionGroupChanges(string msg, IEnumerable<string> groupsIds, ProjectCompetitionGroup competitionGroup)
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
                db.ProjectAdmissions.Where(
                    a =>
                        a.ProjectCompetitionGroupId == competitionGroupId &&
                        toDeleteIds.Contains(a.Student.GroupId)).ToList();
            var subgroupAdmissions =
                db.ProjectSubgroupMemberships.Where(
                    m =>
                        m.Subgroup.Meta.CompetitionGroupId == competitionGroupId &&
                        toDeleteIds.Contains(m.Student.GroupId)).ToList();
            db.ProjectAdmissions.RemoveRange(admissions);
            db.ProjectSubgroupMemberships.RemoveRange(subgroupAdmissions);
            db.SaveChanges();
        }

        public ActionResult DeleteCompetitionGroup(int id)
        {
            using (var dbtran = db.Database.BeginTransaction())
            {
                try
                {
                    db.ProjectProperties.RemoveRange(
                        db.ProjectProperties.Where(
                            x => (x.ProjectCompetitionGroupId == id) && !x.ProjectUsers.Any()));

                    db.ProjectSubgroups.RemoveRange(
                        db.ProjectSubgroups.Where(_ => _.Meta.CompetitionGroupId == id));

                    db.ProjectSubgroupCounts.RemoveRange(
                        db.ProjectSubgroupCounts.Where(_ => _.CompetitionGroupId == id));

                    var competitionGroup = db.ProjectCompetitionGroups.FirstOrDefault(_ => _.Id == id);
                    if (competitionGroup != null)
                        db.ProjectCompetitionGroups.Remove(competitionGroup);

                    db.SaveChanges();
                    dbtran.Commit();

                    Logger.Info($"Удаление проектной группы Id: {competitionGroup.Id} Name: {competitionGroup.Name}");

                    return Json(new { success = true });//, "text/html", Encoding.Unicode);
                }
                catch
                {
                    dbtran.Rollback();
                    Logger.Info($"Попытка удаления проектной группы Id: {id}");
                    return Json(new { success = false, message = "Невозможно удалить проектную группу" });//, "text/html", Encoding.Unicode);
                }
            }
        }

        public ActionResult Properties(int competitionGroupId, string filter)
        {
            var competitionGroup = db.ProjectCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("CompetitionGroup not found");

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var projectsForUser = db.ProjectsForUser(User, includePairedModule: true).Select(p => p.ModuleId).ToList();

                var filterRules = FilterRules.Deserialize(filter);

                var filterTitle = filterRules?.Find(f => f.Property == "title");
                if (filterRules != null)
                {
                    filterRules.Remove(filterTitle);
                }
                string title = filterTitle?.Value?.ToLower() ?? "";

                var properties = db.ProjectProperties.Where(
                        _ => _.ProjectCompetitionGroupId == competitionGroupId && projectsForUser.Contains(_.ProjectId))
                    .Select(_ => new
                    {
                        property = _,
                        period =
                        _.Project.Periods.FirstOrDefault(
                            p => (p.Year == competitionGroup.Year || competitionGroup.Year == 0) && (p.SemesterId == competitionGroup.SemesterId))
                    })
                    .ToList()
                    .Select(_ => new
                    {
                        _.property.Id,
                        level = _.property.Project.Module.Level,
                        _.property.Project.Module.title,
                        _.property.Project.Module.type,
                        coefficient = db.RatingCoefficients.SingleOrDefault(c=>c.Year==_.period.Year && c.Level.Equals(_.property.Project.Module.Level) && c.ModuleType ==(int)ModuleTypeParam.Project)?.Coefficient,
                        Curators = string.Join(", ", _.property.ProjectUsers.Select(u => u.Teacher.initials).ToList()),

                        canEdit = db.CanEditProject(User, _.property.ProjectId)
                    })
                    .Where(p => p.title.ToLower().Contains(title))
                    .AsQueryable().Where(filterRules).ToList();

                return JsonNet(new { data = properties });
            }
            
            var directions = competitionGroup.Groups.Select(g => g.Profile.DIRECTION_ID).ToList();
            var profiles = competitionGroup.Groups.Select((g => g.ProfileId)).ToList();
            
            var projects =
                db.Projects.Where(
                        _ =>
                            _.Periods.Any(
                                p => (p.Year == competitionGroup.Year || competitionGroup.Year == 0) && (p.SemesterId == competitionGroup.SemesterId))
                                && (_.Contract.Periods.SelectMany(p => p.Limits).Where(l => l.DirectionId != null
                                            && (l.Course == 0 || competitionGroup.StudentCourse == 0 || l.Course == competitionGroup.StudentCourse))
                                        .Select(l => new{l.DirectionId,l.ProfileId}).Any(d => profiles.Contains(d.ProfileId) && directions.Contains(d.DirectionId))
                                    && _.Module.type != "Парный модуль"
                                    || directions.Any(d => _.Module.Directions.Select(dr => dr.uid).Contains(d)) && _.Module.type == "Парный модуль")
                                )
                    .ToList();

            foreach (var project in projects)
            {
                if (competitionGroup.ProjectProperties.Any(
                        _ =>
                            (_.ProjectId == project.ModuleId) &&
                            (_.ProjectCompetitionGroupId == competitionGroupId)))
                    continue;

                competitionGroup.ProjectProperties.Add(new ProjectProperty
                {
                    ProjectId = project.ModuleId,
                    ProjectCompetitionGroupId = competitionGroupId,

                });
            }
            db.SaveChanges();

            //ViewBag.CanEdit = db.CanEditProjectCompetitionGroup(User, competitionGroupId);

            ViewBag.Types = JsonConvert.SerializeObject(projects.Select(p => new { p.Module.type }).OrderBy(t => t.type).Distinct());
            ViewBag.Levels = JsonConvert.SerializeObject(projects.Select(p => new { p.Module.Level }).OrderBy(t => t.Level).Distinct());

            return View(competitionGroup);
        }

        public ActionResult PropertyCurators(int? page, string sort, string filter, int? limit, int propertyId)
        {
            var property = db.ProjectProperties.FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("property not found");

            var users = db.Teachers./*Include(t => t.Projects).*/Select(t => new
            {
                selected = db.ProjectUsers.Any(p => p.TeacherId == t.pkey && p.Type == ProjectUserType.Curator && p.ProjectPropertyId == propertyId),
                userId = t.pkey,
                FirstName = t.firstName,
                Patronymic = t.middleName,
                LastName = t.lastName
            });

            SortRules sortRules = SortRules.Deserialize(sort);
            users = users.OrderBy(sortRules.FirstOrDefault(), m => m.LastName);

            users = users.Where(FilterRules.Deserialize(filter));

            var paginated = users.ToPagedList(page ?? 1, limit ?? 25);

            return JsonNet(new
            {
                data = paginated,
                total = users.Count()
            });
        }

        [HttpPost]
        public ActionResult UpdateProjectPropertyCurators(int propertyId, string curatorRows)
        {
            var property = db.ProjectProperties.FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");
            var curators =
                (List<ModulePropertyRow<string>>)
                JsonConvert.DeserializeObject(curatorRows, typeof(List<ModulePropertyRow<string>>));
            foreach (var curatorRow in curators)
            {
                var teacher = db.Teachers.FirstOrDefault(_ => _.pkey == curatorRow.id);
                if (teacher != null)
                {
                    var projectCurator = property.ProjectUsers.FirstOrDefault(_ => _.TeacherId == curatorRow.id);
                    if (projectCurator != null)
                    {
                        if (!curatorRow.selected)
                        {
                            property.ProjectUsers.Remove(projectCurator);
                        }
                    }
                    else if (curatorRow.selected)
                    {
                        property.ProjectUsers.Add(new ProjectUser()
                        {
                            IsChief = false,
                            ProjectPropertyId = propertyId,
                            Type = ProjectUserType.Curator,
                            TeacherId = curatorRow.id
                        });
                    }
                }
            }
            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult SetProjectPropertyLimit(int propertyId, int limit)
        {
            var property = db.ProjectProperties.FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");
            //property.Limit.Limit = limit;

            db.SaveChanges();

            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult Levels()
        {
            var levels = db.ProjectModules().Select(m => new { m.Level }).OrderBy(m => m.Level).ToList();

            var json = Json(
                new
                {
                    data = levels
                },
                new JsonSerializerSettings()
            );

            return json;
        }


        public ActionResult ProjectPeriods()
        {
            var periods = db.ProjectPeriods
                .GroupBy(p => new {p.Year, p.SemesterId,p.Semester.Name}, 
                    (key, group) =>new {year = key.Year, semesterId = key.SemesterId, semesterName = key.Name }).ToList();
            var json = Json(
                new
                {
                    data = periods
                },
                new JsonSerializerSettings()
            );

            return json;
        }

        public ActionResult PossibleCompetences(string id)
        {
            var project = db.Projects.FirstOrDefault(p => p.ModuleId == id);

            var directions = project.Contract.Periods.SelectMany(p => p.Limits).Where(l => l.Direction != null).Select(l => l.Direction);
            var directionIds = directions.Select(d => d.uid).Distinct();
            var areaEducationIds = directions.Select(d => d.AreaEducationId).Distinct();
            var qualifications = directions.Select(d => d.qualifications).Distinct();
            var profileIds = project.Contract.Periods.SelectMany(p => p.Limits).Where(l => l.ProfileId != null).Select(l => l.ProfileId).Distinct();


            var competences = db.Competences.Where(c => !c.IsDeleted && areaEducationIds.Contains(c.AreaEducationId) 
                                                        && (c.QualificationName == null || qualifications.Contains(c.QualificationName)))
                .Include(c => c.Profile.Division)
                .ToList()
                .Where(c => ((c.Type == "УК" || c.Type == "ОПК")
                                    && areaEducationIds.Contains(c.AreaEducationId)
                                    && qualifications.Contains(c.QualificationName))
                            ||
                            directions.Any(d => (string.IsNullOrWhiteSpace(c.QualificationName) || d.qualifications == c.QualificationName)
                                                                && d.uid == c.DirectionId
                                                                && d.AreaEducationId == c.AreaEducationId)
                                    && (c.ProfileId == null || profileIds.Contains(c.ProfileId)))
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.Order,
                    Description = c.Content,
                    Standard = c.Standard,
                    Type = c.Type,

                    ProfileId = c.ProfileId,
                    ProfileTitle = c.Profile?.OksoAndTitle,
                    ProfileCode = c.Profile?.CODE,
                    Division = c.ProfileId != null ? $"Кафедра {c.Profile?.Division?.title} ({db.Divisions.FirstOrDefault(d => d.uuid == c.Profile.Division.parent).shortTitle})"
                        : "",

                    DirectionId = c.DirectionId,
                    DirectionTitle = c.Direction?.OksoAndTitle,
                    DirectionCode = c.Direction?.okso,
                    DirectionAreaEducationId = c.Direction?.AreaEducationId,
                    DirectionQualification = c.Direction?.qualifications,

                    AreaEducationId = c.AreaEducationId
                })
                .OrderBy(c => c.Type).ThenBy(c => c.Order);

            var json = Json(
                new
                {
                    data = competences
                },
                new JsonSerializerSettings()
            );

            return json;
        }
        
        public ActionResult SetCompetences(string id, List<int> competences)
        {
            try
            {
                competences = competences ?? new List<int>();
                var project = db.Projects.FirstOrDefault(p => p.ModuleId == id);

                //удаляем компетенции, которых нет в списке для добавления
                var competenceToRemove = db.ProjectCompetences.Where(p => p.ProjectId == id && !competences.Contains(p.CompetenceId)).ToList();
                db.ProjectCompetences.RemoveRange(competenceToRemove);
                db.SaveChanges();

                // добавляем компетенции
                foreach (var competenceId in competences)
                {
                    var competence = db.Competences.FirstOrDefault(c => c.Id == competenceId);
                    var profiles = project.Contract.Periods.SelectMany(p => p.Limits).Where(l => l.ProfileId != null).Where(p =>
                            p.ProfileId == competence.ProfileId && competence.ProfileId != null
                                || competence.ProfileId == null && competence.DirectionId != null && p.DirectionId == competence.DirectionId 
                                || competence.DirectionId == null
                            ).Select(p => p.ProfileId).Distinct();
                    
                    foreach (var profile in profiles) 
                    //profiles.ForEach(delegate (string profile)
                    {
                        if (!db.ProjectCompetences.Any(c => c.CompetenceId == competenceId && c.ProjectId == id && c.ProfileId == profile))
                        {
                            db.ProjectCompetences.Add(new ProjectCompetence
                            {
                                ProjectId = id,
                                CompetenceId = competenceId,
                                ProfileId = profile
                            });
                        }
                    }
                }
                db.SaveChanges();

                return Json(new { success = true, message = GetCompetencesStr(id) });//, "text/html", Encoding.Unicode);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }

        public ActionResult ProjectCompetences(string id)
        {
            var competences = db.ProjectCompetences.Where(p => p.ProjectId == id).ToList().Select(c => new
            {
                c.Competence.Id,
                c.Competence.Code,
                Description = c.Competence.Content,
                Standard = c.Competence.Standard,
                Type = c.Competence.Type,

                ProfileId = c.Competence.ProfileId,
                ProfileTitle = c.Competence.Profile?.OksoAndTitle,
                ProfileCode = c.Competence.Profile?.CODE,

                DirectionId = c.Competence.DirectionId,
                DirectionTitle = c.Competence.Direction?.OksoAndTitle,
                DirectionCode = c.Competence.Direction?.okso,

                Order = c.Competence.Order
            })
            .OrderBy(c => c.DirectionCode)
            .ThenBy(c => c.ProfileCode)
            .ThenBy(c => c.Order)
            .ToList();

            return Json(
                new
                {
                    data = competences,
                    total = competences.Count
                },
                new JsonSerializerSettings());
        }

        public ActionResult SetProjectDeadline(string projects, string deadlines)
        {
            List<ProjectPeriod> periods = JsonConvert.DeserializeObject<List<ProjectPeriod>>(deadlines);
            var selectedprojects = JsonConvert.DeserializeObject<List<string>>(projects);

            foreach (var  p in selectedprojects)
            {
                var project = db.ProjectPeriods.FirstOrDefault(_=>_.ProjectId==p);

                var period = periods.FirstOrDefault(t => t.Year == project.Year && t.SemesterId == project.SemesterId);

                if (period != null && period != null)
                {
                    project.SelectionDeadline = period.SelectionDeadline;

                }
            }
            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult GetProjectCompetitionGroups(string id)
        {
            var groups = db.ProjectProperties.Where(p => p.ProjectId == id).Select(p=>
                new
                {
                    p.ProjectCompetitionGroupId,
                    p.ProjectCompetitionGroup.Name,
                    Semester= p.ProjectCompetitionGroup.Semester.Name,
                    p.ProjectCompetitionGroup.Year,
                    Course = p.ProjectCompetitionGroup.StudentCourse
                }
            ).ToList(); 

            return Json(
                new
                {
                    data = groups,
                },
                new JsonSerializerSettings());
        }


        private string GetCompetencesStr(string projectId)
        {
            var competences = string.Join("; ", db.ProjectCompetences.Include(c => c.Competence.Direction).Where(c => c.ProjectId == projectId)
                .OrderBy(c => c.Competence.DirectionId).ThenBy(c => c.Competence.ProfileId).ThenBy(c => c.Competence.Order)
                .GroupBy(c => c.Competence.Direction.okso).ToList().Select(g => new
                {
                    okso = g.Key,
                    competences = string.Join(", ", g.Select(c => c.Competence.Code).Distinct())
                }).Select(c => $"{c.okso}: {c.competences}"));

            return competences;
        }
    }
}