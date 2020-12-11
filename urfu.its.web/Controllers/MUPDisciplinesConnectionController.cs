using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
//using System.Web.Script.Serialization;
using Ext.Utilities;
using Ext.Utilities.Linq;
using Newtonsoft.Json;
using PagedList.Core;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.MUPManager)]
    public class MUPDisciplinesConnectionController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? page, int? limit, string sort, string filter, int? focus)
        {
            Expression<Func<Module, bool>> ExcludeModuleTypesExpression = m =>
                !(m.type == "Проектное обучение" || m.type == "Парный модуль" || m.type == "Секции ФК" || m.type == "Физическая культура"
                || m.type == "Иностранный язык" || m.type == "Майноры");

            if (Request.IsAjaxRequest())
            {
                var sortRules = SortRules.Deserialize(sort);
                var filterRules = ObjectableFilterRules.Deserialize(filter);

                var modulesQuery = db.ModulesForUser(User).Where(ExcludeModuleTypesExpression);

                if (!User.IsInRole(ItsRoles.Admin))
                {
                    if (!User.IsInRole(ItsRoles.AllDirections))
                    {
                        var userName = User.Identity.Name;
                        var divisions = (from ud in db.UserDivisions
                                         join d in db.Divisions
                                             on ud.DivisionId equals d.uuid
                                         where ud.UserName == userName
                                         select d).ToList();

                        var divisionFullNames = divisions.SelectMany(d => new[]
                        {
                            d.typeTitle + " «" + d.title + "»",
                            "«" + d.title + "»"
                        }).ToList();
                        modulesQuery = modulesQuery.Where(m => divisionFullNames.Contains(m.coordinator));
                    }
                }

                var disciplineResponsiblePersonsModules = db.WorkingProgramResponsiblePersons
                    .Where(u => u.UserId == User.Identity.Name).Select(p => p.Module).Where(ExcludeModuleTypesExpression);

                modulesQuery = modulesQuery.Concat(disciplineResponsiblePersonsModules).Distinct();

                var modules = modulesQuery
                    .Select(m => new
                    {
                        id = m.uuid,
                        m.title,
                        m.shortTitle,
                        m.coordinator,
                        m.type,
                        m.competence,
                        m.testUnits,
                        m.priority,
                        m.state,
                        approvedDate = m.approvedDate.HasValue ? (m.approvedDate.Value.Day.ToString() + (m.approvedDate.Value.Month < 10 ? ".0" : ".") + m.approvedDate.Value.Month.ToString() + "." + m.approvedDate.Value.Year.ToString()) : "", //АБ: это не я, это LINQ
                        m.comment,
                        m.file,
                        m.specialities,
                        m.number,
                        numberStr = m.number.Value.ToString(),
                        mups = db.MUPDisciplineConnections.Where(c => !c.MUPModeus.Removed && c.ModuleId == m.uuid).Select(c => new { Id = c.MUPModeus.Id, Name = c.MUPModeus.Name }).ToList(),
                    });

                if (sortRules == null || sortRules.Count == 0)
                {
                    modules = modules.OrderBy(d => d.title);
                }
                else
                {
                    var sortRule = sortRules.FirstOrDefault();
                    switch (sortRule?.Property)
                    {
                        case "mups":
                            if (sortRule.Direction == SortDirection.Ascending)
                                modules = modules.OrderBy(v => v.mups.FirstOrDefault());
                            else
                                modules = modules.OrderByDescending(v => v.mups.FirstOrDefault());
                            break;
                        case "specialities":
                            if (sortRule.Direction == SortDirection.Ascending)
                                modules = modules.OrderBy(v => v.specialities);
                            else
                                modules = modules.OrderByDescending(v => v.specialities);
                            break;
                        default:
                            modules = modules.OrderBy(sortRule);
                            break;
                    }
                }

                    var filteringModuls = modules
                        .Select(m => new
                        {
                            m.id,
                            m.title,
                            m.shortTitle,
                            m.coordinator,
                            m.type,
                            m.competence,
                            m.testUnits,
                            m.priority,
                            m.state,
                            m.approvedDate,
                            m.comment,
                            m.file,
                            m.specialities,
                            m.number,
                            m.numberStr,
                            mups = m.mups.Select(mup => mup.Name),
                            mupIds = m.mups.Select(mup => mup.Id).ToList(),
                            hasMup = m.mups.Any()
                        })
                        .Where(filterRules)
                        .ToList();
                
                var paginated = filteringModuls.ToPagedList(page ?? 1, limit ?? 25);

                return Json(
                    new
                    {
                        data = paginated,
                        total = filteringModuls.Count()
                    },
                    new JsonSerializerSettings()
                );
            }

            var types = db.Modules.Where(ExcludeModuleTypesExpression).Select(m => new
                {
                    Type = m.type
                }).Distinct().OrderBy(t => t.Type);

                ViewBag.Types = JsonConvert.SerializeObject(types);

                var mups = db.MUPModeuses.Select(m => new
                    {
                        m.Id,
                        m.Name
                    }).ToList()
                    .Select(m => new {
                        MUPId = m.Id,
                        // замена непрерывного пробела на обычный для корректного отображения ITS-1647
                        MUPName = m.Name.Replace(char.ConvertFromUtf32(160), char.ConvertFromUtf32(32))
                                        .Replace("\"", "'")
                                        .Replace("\\", "\\\\")
                    })
                    .OrderBy(m=>m.MUPName);
                ViewBag.MUPs= JsonConvert.SerializeObject(mups);
                
                ViewBag.CanEdit = User.IsInRole(ItsRoles.MUPManager);
                return View();
            
        }

        
        public ActionResult DisciplinesByModule(string moduleId)
        {
            var module = db.UniModules().Include(_ => _.disciplines).FirstOrDefault(_ => _.uuid == moduleId);
            var disciplines = module.disciplines.Select(d => new
            {
                d.title,
                d.uid,
                mups = db.MUPDisciplineConnections.Where(c => c.DisciplineId == d.uid && c.ModuleId == moduleId).Select(c => c.MUPModeusId)
            });

            return Json(disciplines, new JsonSerializerSettings());
        }

        public ActionResult MUPs(string moduleId)
        {
            var moduleDirections = db.UniModules().FirstOrDefault(_ => _.uuid == moduleId).Directions.Select(d => d.uid).ToList();
            var connectedMups = db.MUPDisciplineConnections.Where(m => m.ModuleId != moduleId).Select(m => m.MUPModeusId).ToList();
            var mups = db.MUPModeusDirections.Where(m => moduleDirections.Contains(m.DirectionId) && !connectedMups.Contains(m.MUPModeusId) && !m.MUPModeus.Removed).Select(m => new
            {
                m.MUPModeus.Id,
                m.MUPModeus.Name
            }).Distinct().OrderBy(m => m.Name);

            return Json(mups, new JsonSerializerSettings());
        }
        
        public ActionResult ConnectDisciplineToMUP(string moduleId, string mups)
        {
            var mupConnections = JsonConvert.DeserializeObject<List<MUPDisciplineConnectionVM>>(mups);
           
            MUPHelper helper = new MUPHelper();
            string message;
            var success = helper.ConnectDisciplineToMUP(moduleId, mupConnections, out message);
            helper.FillMUPTables(moduleId);

            var mupsList = db.MUPDisciplineConnections.Where(m => m.ModuleId == moduleId).Select(m => m.MUPModeus.Name).ToList();

            return Json(new { success, mupsList, message });//, "text/html", Encoding.Unicode);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var module = db.UniModules().Include("MUP.Periods").First(m => m.uuid == id);

            if (module == null)
                return NotFound();

            var techSelector = new SelectList(db.ModuleTeches, "Id", "Name", module.MUP?.ModuleTechId);

            //var semesters = db.Semesters.ToList();
            //var semesterSelector = new SelectList(semesters, "Id", "Name");
            //var courses = new List<object>
            //{
            //    new {Id = "", Name = "Все"},
            //    new {Id = 1, Name = "1"},
            //    new {Id = 2, Name = "2"},
            //    new {Id = 3, Name = "3"},
            //    new {Id = 4, Name = "4"},
            //    new {Id = 5, Name = "5"},
            //};
            var model = new MUPEditViewModel
            {
                Module = module,
                moduleUUId = module.MUP == null ? string.Empty : module.uuid,
                showInLc = module.MUP?.ShowInLC ?? false,
                techid = module.MUP?.ModuleTechId.ToString() ?? string.Empty,
                tech = module.MUP?.Tech.Name ?? string.Empty,
                TechSelector = techSelector,
                //SemesterSelector = semesterSelector
            };

            var moduleMUPs = db.MUPDisciplineConnections.Where(m => m.ModuleId == id).Select(m => m.ModuleMUP).Select(m => m.MUP).ToList();

            if (moduleMUPs.Count == 0)
                model.periods = new List<MUPPeriodEditViewModel>();
            else
                model.periods = moduleMUPs.SelectMany(m => m.Periods).Where(p => !p.Removed).Select(p =>
                        new MUPPeriodEditViewModel
                        {
                            //id = p.Id,
                            year = p.Year,
                            semesterId = p.SemesterId,
                            semesterName = p.Semester.Name,
                            //Selector = new SelectList(semesters, "Id", "Name", p.SemesterId),
                            //CourseSelector = new SelectList(courses, "Id", "Name", p.Course),
                            selectionDeadline = p.SelectionDeadline,
                            Course = p.Course
                        }
                ).GroupBy(p => new { p.Course, p.semesterId, p.year, p.selectionDeadline }).Select(g => g.First()).ToList();

            ViewBag.CanEdit = false; //User.IsInRole(ItsRoles.MUPManager);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MUPEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mup = new MUP
                {
                    ModuleId = model.Module.uuid,
                    Periods = new List<MUPPeriod>()
                };

                if (User.IsInRole(ItsRoles.MUPManager))
                {
                    foreach (var p in model.periods)
                    {
                        if (p.id < 0)
                            p.id = 0;

                        //добавили и тут же удалили
                        if (p.isDeleted && (p.id == 0)) continue;

                        var period = new MUPPeriod
                        {
                            Id = p.id,
                            MUPId = mup.ModuleId,
                            Year = p.year,
                            SemesterId = p.semesterId,
                            Course = p.Course,
                            SelectionDeadline = p.selectionDeadline
                        };

                        if (p.isDeleted)
                        {
                            db.Entry(period).State = EntityState.Deleted;
                        }
                        else
                        {
                            mup.Periods.Add(period);
                            db.Entry(period).State = p.id == 0 ? EntityState.Added : EntityState.Modified;
                        }
                    }
                    mup.ShowInLC = model.showInLc;
                    mup.ModuleTechId = int.Parse(model.techid);

                    db.Entry(mup).State = string.IsNullOrEmpty(model.moduleUUId) ? EntityState.Added : EntityState.Modified;
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
            var module = db.UniModules().Where(m => m.uuid == moduleId).Single();

            ViewBag.Title = string.Format(@"Дисциплины для модуля МУП ""{0}""", module.title);
            ViewBag.MUPId = moduleId;
            ViewBag.CanEdit = User.IsInRole(ItsRoles.MUPManager);

            var moduleMUPs = db.MUPDisciplineConnections.Where(c => c.ModuleId == moduleId).Select(m => m.ModuleMUP).ToList();

            var model = new List<MUPDisciplineViewModel>();// (module.disciplines.Count);
            foreach (var d in module.disciplines)
            {
                var r = new MUPDisciplineViewModel
                {
                    Discipline = d,
                    MUPDiscipline =
                        module.MUP.Disciplines.FirstOrDefault(f => f.DisciplineUid == d.uid)
                };

                model.Add(r);
            }

            return View(model);
        }

        public ActionResult Tmers(int id, string message)
        {
            ViewBag.Message = message;
            var divisions = db.Divisions.ToDictionary(d => d.uuid);
            CreateTree(divisions);

            var discipline =
                db.MUPDisciplines
                    .Include("Tmers.Periods")
                    .Include("MUP")
                    .Include("Discipline")
                    .Single(d => d.Id == id);
            
            ViewData.Add("Tmers1", discipline.Tmers.Where(t => t.Tmer.kgmer == 1).OrderBy(t => t.Tmer.kmer).ToList());
            ViewData.Add("Tmers2", discipline.Tmers.Where(t => t.Tmer.kgmer == 2).OrderBy(t => t.Tmer.kmer).ToList());
            ViewData.Add("Tmers3", discipline.Tmers.Where(t => t.Tmer.kgmer == 3).OrderBy(t => t.Tmer.kmer).ToList());

            ViewBag.CanEdit = User.IsInRole(ItsRoles.MUPManager);

            return View(discipline);
        }

        private void CreateTree(Dictionary<string, Division> divisions)
        {
            foreach (var d in divisions.Values)
            {
                Division parent;
                if ((d.parent != null) && divisions.TryGetValue(d.parent, out parent))
                    d.ParentDivision = parent;
            }
        }

        [Authorize(Roles = ItsRoles.MUPManager)]
        public ActionResult EditTmers(string mupId, string disciplineId)
        {
            var keys = new List<int?>(3) { 1, 2, 3 };
            var tmers = db.Tmers.Where(t => keys.Contains(t.kgmer)
                                            && (t.kmer != "U039")
                                            && (t.kmer != "U032")
                                            && (t.kmer != "U033"))
                .ToList();

            var mup = db.MUPs.Include("Disciplines.Tmers").Single(m => m.ModuleId == mupId);
            var discipline = db.Disciplines.Single(d => d.uid == disciplineId);

            var model = new MUPTmersViewModel(mup, discipline, tmers);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.MUPManager)]
        public ActionResult EditTmers(MUPTmersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mup = db.MUPs
                        .Include("Disciplines.Tmers")
                        .Single(m => m.ModuleId == model.MUP.ModuleId);
                var mupDiscipline = mup.Disciplines.FirstOrDefault(d => d.DisciplineUid == model.Discipline.uid);
                if (mupDiscipline == null)
                {
                    mupDiscipline = new MUPDiscipline
                    {
                        DisciplineUid = model.Discipline.uid,
                        MUPId = mup.ModuleId,
                        Tmers = new List<MUPDisciplineTmer>()
                    };
                    mup.Disciplines.Add(mupDiscipline);
                }

                var tmers = new List<MUPTmersRowViewModel>();
                tmers.AddRange(model.Tmers1);
                tmers.AddRange(model.Tmers2);
                tmers.AddRange(model.Tmers3);

                foreach (var t in tmers)
                {
                    var dt = mupDiscipline.Tmers.FirstOrDefault(f => f.TmerId == t.TmerId);
                    if ((dt == null) && t.Checked)
                    {
                        dt = new MUPDisciplineTmer { TmerId = t.TmerId };
                        mupDiscipline.Tmers.Add(dt);
                    }
                    if ((dt != null) && !t.Checked)
                    {
                        mupDiscipline.Tmers.Remove(dt);
                        db.Entry(dt).State = EntityState.Deleted;
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Tmers", new { id = mupDiscipline.Id });
            }
            return RedirectToAction("Disciplines", new { moduleId = model.MUP.ModuleId });
        }

        [Authorize(Roles = ItsRoles.MUPManager)]
        public ActionResult EditDivisions(int mupDisciplineTmerPeriodId)
        {
            var period = db.MUPDisciplineTmerPeriods.Where(t => t.Id == mupDisciplineTmerPeriodId).Single();

            var divisions = db.Divisions.ToList();

            var model = new MinorDivisionViewModel(divisions, period, period.Tmer.MUPDisciplineId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.MUPManager)]
        public ActionResult EditDivisions(MinorDivisionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var period =
                    db.MUPDisciplineTmerPeriods
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

                return RedirectToAction("Tmers", new { id = period.Tmer.MUPDisciplineId });
            }
            return View(model);
        }

        [Authorize(Roles = ItsRoles.MUPManager)]
        public ActionResult EditPeriods(string mupId, string disciplineId)
        {
            var mup = db.MUPs.Include(m => m.Periods).Single(m => m.ModuleId == mupId);
            var discipline = db.MUPDisciplines
                    .Include("Tmers.Periods")
                    .Single(d => (d.MUPId == mupId) && (d.DisciplineUid == disciplineId));

            var model = new MUPTmersPeriodViewModel(mup, discipline);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.MUPManager)]
        public ActionResult EditPeriods(MUPTmersPeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mupDiscipline = db.MUPDisciplines
                    .Include("Tmers.Periods")
                    .Single(d => d.Id == model.Discipline.Id);

                foreach (var t in model.Rows)
                {
                    var dt = mupDiscipline.Tmers.FirstOrDefault(f => f.Id == t.Tmer.Id);
                    var tp = dt.Periods.FirstOrDefault(f => f.MUPPeriodId == t.Period.Id);
                    if ((tp == null) && t.Checked)
                    {
                        tp = new MUPDisciplineTmerPeriod
                        {
                            MUPDisciplineTmerId = t.Tmer.Id,
                            MUPPeriodId = t.Period.Id
                        };
                        dt.Periods.Add(tp);
                    }
                    if ((tp != null) && !t.Checked)
                    {
                        if (tp.MUPSubgroupCounts.Any(c => c.Subgroups.Any()))
                            return RedirectToAction("Tmers",
                                new
                                {
                                    id = mupDiscipline.Id,
                                    message = "Невозможно удалить периоды, т.к. на них есть подгруппы"
                                });

                        if (tp.MUPSubgroupCounts.Any())
                            tp.MUPSubgroupCounts.Clear();

                        dt.Periods.Remove(tp);
                        db.Entry(tp).State = EntityState.Deleted;
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Tmers", new { id = mupDiscipline.Id });
            }
            return RedirectToAction("Disciplines", new { moduleId = model.MUP.ModuleId });
        }

        public ActionResult GetCoordinators()
        {
            var coordinatorsList = db.Modules.Select(m =>new{ m.coordinator}).Distinct().ToList().Where(m => !string.IsNullOrWhiteSpace(m.coordinator));
            return Json(new { data = coordinatorsList}, new JsonSerializerSettings());
        }

        public ActionResult GetModuleNumbers(string q)
        {
            var numberList = db.Modules.Where(m => m.number != null && m.number.Value.ToString().StartsWith(q.Trim())).OrderBy(m=>m.number)
                .Select(m => new {number= m.number.Value.ToString()}).ToList();
            return Json(new {data = numberList}, new JsonSerializerSettings());
        }


    }
}