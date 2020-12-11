using Ext.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using PagedList.Core;
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
//using System.Data.Entity.Migrations;
using Urfu.Its.Web.Model.Models.ModulesVM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.ProjectView)]
    public class ProjectPairedModuleController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var modules = User.IsInRole(ItsRoles.ProjectManager) || User.IsInRole(ItsRoles.ProjectROP)
                    ? GetFilteredPlans(filter).ToList().Select(p => new
                    {
                        id = p.Module.uuid,
                        p.Module.title,
                        p.Module.coordinator,
                        p.Module.type,
                        p.Module.competence,
                        p.Module.testUnits,
                        p.Module.priority,
                        p.Module.state,
                        approvedDate = p.Module.approvedDate.HasValue
                                        ? p.Module.approvedDate.Value.Day.ToString() + (p.Module.approvedDate.Value.Month < 10 ? ".0" : ".") +
                                          p.Module.approvedDate.Value.Month.ToString() + "." + p.Module.approvedDate.Value.Year.ToString()
                                        : "",
                        p.Module.comment,
                        p.Module.file,
                        p.Module.specialities,
                        p.eduplanNumber,
                        p.versionNumber,
                        planAndVersion = $"{p.eduplanNumber} ({p.versionNumber})",
                        group = string.Join(",", db.ModuleRelations.Where(m => (m.MainModuleUUID == p.moduleUUID || m.PairedModuleUUID == p.moduleUUID)
                                && m.eduplanNumber == p.eduplanNumber && m.versionNumber == p.versionNumber).Select(m => m.Group))
                    })
                    : Enumerable.Empty<object>();

                return Json(
                    new
                    {
                        data = modules,
                        total = modules.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            ViewBag.Focus = focus;
            return View();
        }

        private IQueryable<Plan> GetFilteredPlans(string filter)
        {
            var filterRules = FilterRules.Deserialize(filter);

            var filterYear = filterRules?.Find(f => f.Property == "year");
            var filterSemester = filterRules?.Find(f => f.Property == "semester");

            var filterEduplanNumber = filterRules?.Find(f => f.Property == "eduplanNumber");
            var filterVersionNumber = filterRules?.Find(f => f.Property == "versionNumber");

            if (filterRules != null)
            {
                filterRules.Remove(filterSemester);
                filterRules.Remove(filterYear);

                filterRules.Remove(filterEduplanNumber);
                filterRules.Remove(filterVersionNumber);
            }

            int year;
            int semester;
            var haveYear = int.TryParse(filterYear?.Value, out year);
            var haveSemester = int.TryParse(filterSemester?.Value, out semester);

            var queryModules = db.UniProjectModulesForConnection();

            if (haveYear)
                queryModules = queryModules.Where(m => m.Project.Periods.Any(p => p.Year == year));
            if (haveSemester)
                queryModules = queryModules.Where(m => m.Project.Periods.Any(p => p.SemesterId == semester));

            queryModules = queryModules.Where(filterRules);

            int eduplan;
            int version;
            var haveEduplan = int.TryParse(filterEduplanNumber?.Value, out eduplan);
            var haveVersion = int.TryParse(filterVersionNumber?.Value, out version);

            var queryPlans = queryModules.SelectMany(m => m.Plans); 

            if (haveEduplan)
                queryPlans = queryPlans.Where(p => p.eduplanNumber == eduplan);
            if (haveVersion)
                queryPlans = queryPlans.Where(p => p.versionNumber == version);
            
            queryPlans = queryPlans.Include(p => p.Module);

            return queryPlans;
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            
            var module = db.UniProjectModules().Include("Project.Periods").First(m => m.uuid == id);

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
                SemesterSelector = semesterSelector
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

            ViewBag.CanEdit = User.IsInRole(ItsRoles.ProjectManager) || User.IsInRole(ItsRoles.ProjectROP);
            ViewBag.CanLimitEdit = User.IsInRole(ItsRoles.ProjectManager) || User.IsInRole(ItsRoles.ProjectROP);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var project = new Project
                {
                    ModuleId = model.Module.uuid,
                    Periods = new List<ProjectPeriod>()
                };

                if (User.IsInRole(ItsRoles.ProjectManager) || User.IsInRole(ItsRoles.ProjectROP))
                {
                    Logger.Info("Редактирвоание парного модуля: {0}", JsonConvert.SerializeObject(model));

                    foreach (var p in model.periods)
                    {
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
                            SelectionDeadline = p.selectionDeadline,
                        };

                        if (p.isDeleted)
                        {
                            db.Entry(period).State = EntityState.Deleted;
                        }
                        else
                        {
                            project.Periods.Add(period);
                            db.Entry(period).State = p.id == 0 ? EntityState.Added : EntityState.Modified;
                        }
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
            var module = db.UniProjectModules().Include("Project.Disciplines").Where(m => m.uuid == moduleId).Single();

            ViewBag.Title = string.Format(@"Дисциплины для модуля ""{0}""", module.title);
            ViewBag.ProjectId = moduleId;
            ViewBag.CanEdit = User.IsInRole(ItsRoles.ProjectManager) || User.IsInRole(ItsRoles.ProjectROP);

            var model = new List<ProjectDisciplineViewModel>(module.disciplines.Count);
            foreach (var d in module.disciplines)
            {
                var r = new ProjectDisciplineViewModel
                {
                    Discipline = d,
                    ProjectDiscipline =
                        module.Project?.Disciplines?.FirstOrDefault(f => f.DisciplineUid == d.uid)
                };

                model.Add(r);
            }

            return View(model);
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

            var project = db.Projects.Include("Disciplines.Tmers").FirstOrDefault(m => m.ModuleId == projectId);
            var module = db.Modules.FirstOrDefault(m => m.uuid == projectId);
            if (project == null && module != null)
            {
                project = new Project
                {
                    ModuleId = projectId,
                    Module = module,
                    Disciplines = new List<ProjectDiscipline>(),
                    ModuleTechId = 1,
                    ShowInLC = false
                };
                db.Projects.Add(project);
                db.SaveChanges();
            }
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

            ViewBag.CanEdit = User.IsInRole(ItsRoles.ProjectView);

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

        public ActionResult Connect(string mod1, string mod2, int eduplan, int version, bool connect)
        {
            string projectType = "Проектное обучение";
            string pairedType = "Парный модуль";

            var module1 = db.Modules.FirstOrDefault(m => m.uuid == mod1 && (m.type == projectType || m.type == pairedType));
            var module2 = db.Modules.FirstOrDefault(m => m.uuid == mod2 && (m.type == projectType || m.type == pairedType));

            if (module1 == null || module2 == null)
                return Json(new { success = false, message = "Модули не найдены" });//, "text/html", Encoding.Unicode);

            if (module1.type == module2.type)
                return Json(new { success = false, message = "Модули должны быть разных типов" });//, "text/html", Encoding.Unicode);

            var plan1 = db.Plans.FirstOrDefault(p => p.moduleUUID == module1.uuid && p.eduplanNumber == eduplan && p.versionNumber == version);
            var plan2 = db.Plans.FirstOrDefault(p => p.moduleUUID == module2.uuid && p.eduplanNumber == eduplan && p.versionNumber == version);

            if (plan1 == null || plan2 == null)
                return Json(new { success = false, message = "Модули должны принадлежать одному учебному плану" });//, "text/html", Encoding.Unicode);

            if (plan1.directionId != plan2.directionId)
                return Json(new { success = false, message = "Учебный план должен быть на одном направлении" });//, "text/html", Encoding.Unicode);

            var mainModuleUUID = module1.type == projectType ? module1.uuid : module2.uuid;
            var pairedModuleUUID = module1.type == pairedType ? module1.uuid : module2.uuid;

            var existRelation = db.ModuleRelations
                .FirstOrDefault(m => m.MainModuleUUID == mainModuleUUID && m.PairedModuleUUID == pairedModuleUUID
                        && m.eduplanNumber == eduplan && m.versionNumber == version);

            if (existRelation != null && connect)
                return Json(new { success = false, message = "Модули уже связаны" });//, "text/html", Encoding.Unicode);

            if (existRelation == null && !connect)
                return Json(new { success = false, message = "Модули не связаны" });//, "text/html", Encoding.Unicode);

            if (!connect)
            {
                // Удалять связь, если нет зачисленных студентов на парный модуль и нет студентов в созданных подгруппах на парный модуль. 

                var anyAdmissions = db.ProjectAdmissions.Any(a => a.ProjectId == pairedModuleUUID) 
                    || db.ProjectSubgroupMemberships.Any(s => s.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.ProjectId == pairedModuleUUID);

                if (anyAdmissions)
                    return Json(new { success = false, message = "Сначала необходимо удалить студентов из зачислений и подгрупп на парный модуль!" });//, "text/html", Encoding.Unicode);

                db.ModuleRelations.Remove(existRelation);
                db.SaveChanges();

                return Json(new { success = true, message = "Связь модулей удалена" });//, "text/html", Encoding.Unicode);
            }

            // Добавление связи

            var existPairedModule = db.ModuleRelations.Any(m => m.PairedModuleUUID == pairedModuleUUID
                    && m.eduplanNumber == eduplan && m.versionNumber == version);

            if (existPairedModule && connect)
                return Json(new { success = false, message = "Парный модуль уже связан с другим модулем" });//, "text/html", Encoding.Unicode);

            // нумерация группы происходит внутри направления
            var lastRelations = db.ModuleRelations.Select(m => new
                {
                    m,
                    db.Plans.FirstOrDefault(
                        p => p.moduleUUID == m.MainModuleUUID && p.eduplanNumber == m.eduplanNumber && p.versionNumber == m.versionNumber).directionId
                })
            .Where(m => m.directionId == plan1.directionId).Select(m => m.m.Group).OrderByDescending(group => group).ToList();

            int nextGroup = 1;
            if (lastRelations.Count > 0)
                nextGroup += lastRelations.First();

            var connection = new ModuleRelation()
            {
                MainModuleUUID = mainModuleUUID,
                PairedModuleUUID = pairedModuleUUID,
                Group = nextGroup,
                eduplanNumber = eduplan,
                versionNumber = version
            };

            db.ModuleRelations.Add(connection);
            db.SaveChanges();

            return Json(new { success = true, message = "Связь модулей добавлена" });//, "text/html", Encoding.Unicode);
        }
    }
}