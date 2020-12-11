using Ext.Utilities;
using Ext.Utilities.Linq;
using Microsoft.AspNetCore.Identity;
using PagedList.Core;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Security.Principal;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Ajax.Utilities;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Urfu.Its.Web.Controllers
{

    [Authorize(Roles = ItsRoles.MinorView)]
    public class MinorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = ItsRoles.MinorReport)]
        public ActionResult DownloadMinorsReport(string filter)
        {
            var minors = GetFilteredModules(filter);

            var stream = new VariantExport().Export(new
            {
                Rows = minors.Select(m => new
                {
                    m.title,
                    m.coordinator,
                    m.testUnits,
                    m.specialities,
                    m.state
                })
            }, "minors2ReportTemplate.xlsx");

            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Отчёт майноры.xlsx".ToDownloadFileName());

        }
        public ActionResult Index(int? page, int? limit, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var sortRules = SortRules.Deserialize(sort);

                var modules = GetFilteredModules(filter).Select(m => new
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
                    approvedDate = m.approvedDate.HasValue
                        ? (m.approvedDate.Value.Day.ToString() + (m.approvedDate.Value.Month < 10 ? ".0" : ".") +
                           m.approvedDate.Value.Month.ToString() + "." + m.approvedDate.Value.Year.ToString())
                        : "", //АБ: это не я, это LINQ
                    m.comment,
                    m.file,
                    m.specialities
                });

                //if (sortRules == null || sortRules.Count == 0)
                //{
                //    modules = modules.OrderBy(d => d.title);
                //}
                //else
                if (sortRules?.Count > 0)
                {
                    var sortRule = sortRules[0];
                    modules = modules.OrderBy(sortRule);
                }
                var paginated = modules.ToPagedList(page ?? 1, limit ?? 25);
                return Json(
                    new
                    {
                        data = paginated,
                        total = modules.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            else
            {
                ViewBag.Focus = focus;
                return View();
            }

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

        private IQueryable<Module> FilterPeriod(FilterRule filterYear, FilterRule filterSemester)
        {
            int year;
            int semester;

            var haveYear = int.TryParse(filterYear?.Value, out year);
            var haveSemester = int.TryParse(filterSemester?.Value, out semester);

            if (haveYear && haveSemester)
                return MinorsForUser().Where(m => m.Minor.Periods.Any(p => p.Year == year && p.SemesterId == semester));

            if (haveYear)
                return MinorsForUser().Where(m => m.Minor.Periods.Any(p => p.Year == year));

            if (haveSemester)
                return MinorsForUser().Where(m => m.Minor.Periods.Any(p => p.SemesterId == semester));

            return MinorsForUser();
        }

        private IQueryable<Module> MinorsForUser()
        {
            //return db.MinorsForUser(User);
            return db.UniModules().Where(m => m.type.Contains("Майнор"));
        }

        private bool IsAccessible(string minorID)
        {
            if (User.IsInRole(ItsRoles.AllMinors)) return true;

            var userName = (User as IPrincipal).Identity.Name;

            var access = db.Users.Single(u => u.UserName == userName).Minors.Any(m => m.ModuleId == minorID);

            return access;
        }
        // GET: Получаем Ajax-запросом семестры для комбобокса
        //[System.Web.Mvc.Authorize(Roles = ItsRoles.MinorCreateGroup)] // использую эту action в Массов печати списка подгрупп по майнорам
        [AllowAnonymous] //TODO: amir: надо на отдельную контроллер вывести
        public ActionResult Semesters()
        {
            var semesters = db.Semesters.Select(s => new { Id = s.Id.ToString(), Name = s.Name });

            var json = Json(
                new
                {
                    data = semesters
                },
                new JsonSerializerSettings()
            );

            return json;            
        }

        [AllowAnonymous] //amir: справочник для фильтра
        public ActionResult States()
        {
            var states = db.UniModules().Distinct().Select(s => new { Id = s.state, Name = s.state});

            var json = Json(
                new
                {
                    data = states
                },
                new JsonSerializerSettings()
            );

            return json;            
        }
        // GET: Modules/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            var module = db.UniModules().Include("Minor.Periods").First(m => m.uuid == id);

            if (module == null)
            {
                return NotFound();
            }

            var techSelector = new SelectList(db.ModuleTeches, "Id", "Name", module.Minor?.MinorTechId);

            var semesters = db.Semesters.ToList();
            var semesterSelector = new SelectList(semesters, "Id", "Name");

            var model = new MinorEditViewModel
            {
                Module = module,
                moduleUUId = module.Minor == null ? string.Empty : module.uuid,
                showInLc = module.Minor?.ShowInLC ?? false,
                techid = module.Minor?.MinorTechId.ToString() ?? string.Empty,
                tech = module.Minor?.Tech.Name ?? string.Empty,
                TechSelector = techSelector,
                SemesterSelector = semesterSelector
            };

            if (module.Minor == null)
                model.periods = new List<MinorPeriodEditViewModel>();
            else
                model.periods = module.Minor.Periods.Select(p =>
                    new MinorPeriodEditViewModel
                    {
                        id = p.Id,
                        year = p.Year,
                        semesterId = p.SemesterId,
                        semesterName = p.Semester.Name,
                        Selector = new SelectList(semesters, "Id", "Name", p.SemesterId),
                        selectionDeadline = p.SelectionDeadline,
                        min = p.MinStudentsCount,
                        max = p.MaxStudentsCount
                    }
                ).ToList();


            var accessible = IsAccessible(id);

            ViewBag.CanEdit = accessible && User.IsInRole(ItsRoles.MinorEdit);
            ViewBag.CanLimitEdit = accessible && User.IsInRole(ItsRoles.MinorLimitEdit);

            return View(model);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //public ActionResult Edit([Bind(Include = "uuid,title,shortTitle,coordinator,type,competence,testUnits,priority,state,approvedDate,comment,file,specialities")] Module module)

        // POST: Modules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.MinorEdit + "," + ItsRoles.MinorLimitEdit)]
        public ActionResult Edit(MinorEditViewModel model)
        {

            if (ModelState.IsValid)
            {
                var minor = new Minor
                {
                    ModuleId = model.Module.uuid,
                    Periods = new List<MinorPeriod>()
                };

                if (User.IsInRole(ItsRoles.MinorLimitEdit))
                {
                    foreach (var p in model.periods)
                    {
                        if (p.id < 0)
                            p.id = 0;

                        //добавили и тут же удалили
                        if (p.isDeleted && p.id == 0) continue;

                        var period = new MinorPeriod
                        {
                            Id = p.id,
                            ModuleId = minor.ModuleId,
                            Year = p.year,
                            SemesterId = p.semesterId,
                            SelectionDeadline = p.selectionDeadline,
                            MinStudentsCount = p.min,
                            MaxStudentsCount = p.max
                        };

                        if (p.isDeleted)
                        {
                            db.Entry(period).State = EntityState.Deleted;
                        }
                        else
                        {
                            minor.Periods.Add(period);
                            db.Entry(period).State = p.id == 0 ? EntityState.Added : EntityState.Modified;
                        }

                    }
                }

                if (User.IsInRole(ItsRoles.MinorEdit))
                {
                    minor.ShowInLC = model.showInLc;
                    minor.MinorTechId = int.Parse(model.techid);

                    if (string.IsNullOrEmpty(model.moduleUUId))
                    {
                        db.Entry(minor).State = EntityState.Added;
                    }
                    else
                    {
                        db.Entry(minor).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index", new { focus = model.moduleUUId });
            }
            else
            {
                //щас по новой будем делать форму с ошибками, нужны справочники
                var techSelector = new SelectList(db.ModuleTeches, "Id", "Name", model.techid);

                var semesters = db.Semesters.ToList();
                var semesterSelector = new SelectList(semesters, "Id", "Name");

                model.TechSelector = techSelector;
                model.SemesterSelector = semesterSelector;

                foreach (var p in model.periods)
                {
                    p.Selector = new SelectList(semesters, "Id", "Name");
                }
            }

            return View(model);
        }

        public ActionResult Disciplines(string moduleId)
        {
            var module = db.UniModules().Include("Minor.Disciplines").Where(m => m.uuid == moduleId).Single();

            ViewBag.Title = string.Format(@"Дисциплины для майнора ""{0}""", module.title);
            ViewBag.MinorId = moduleId;
            ViewBag.CanEdit = IsAccessible(moduleId) && User.IsInRole(ItsRoles.MinorEdit);

            var model = new List<MinorDisciplineViewModel>(module.disciplines.Count);
            foreach (var d in module.disciplines)
            {
                var r = new MinorDisciplineViewModel
                {
                    Discipline = d,
                    MinorDiscipline = module.Minor.Disciplines.FirstOrDefault(f => f.DisciplineUid == d.uid)
                };

                model.Add(r);
            }

            return View(model);
        }

        public ActionResult EditRequirments(string moduleId)
        {
            var model = db.Minors.Include(m => m.Requirments).Where(m => m.ModuleId == moduleId).Single();
            if (model.Requirments.Count > 0)
                model.RequirmentId = model.Requirments.First().uuid;

            var selector = new List<SelectListItem>(
                db.MinorsForUser(User)
                .Select(m => new SelectListItem { Value = m.uuid, Text = m.title, Selected = model.RequirmentId == m.uuid })
                );

            selector.Insert(0, new SelectListItem { Value = null, Text = null });

            ViewBag.MinorSelector = selector;
            ViewBag.CanEdit = IsAccessible(moduleId) && User.IsInRole(ItsRoles.MinorEdit);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.MinorEdit)]
        public ActionResult EditRequirments(Minor model)
        {
            if (ModelState.IsValid)
            {
                if (IsAccessible(model.ModuleId))
                {
                    var minor = db.Minors.Include(m => m.Requirments).Where(m => m.ModuleId == model.ModuleId).Single();

                    var requirment = minor.Requirments.Count > 0 ? minor.Requirments.First() : null;

                    var requirmentId = requirment == null ? null : requirment.uuid;

                    if (!string.Equals(requirmentId, model.RequirmentId))
                    {
                        if (requirment != null)
                        {
                            minor.Requirments.Remove(requirment);
                        }

                        if (model.RequirmentId != null)
                        {
                            var newRequirment = new Module() { uuid = model.RequirmentId };

                            minor.Requirments.Add(newRequirment);

                            db.Entry(newRequirment).State = EntityState.Unchanged;

                            UpdateStudentSelections(minor, newRequirment);
                        }
                    }

                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { focus = model.ModuleId });
            }
            else
            {
                return RedirectToAction("Index", new { focus = model.ModuleId });
            }
        }

        private void UpdateStudentSelections(Minor minor, Module newRequirment)
        {
            //При проставлении пререквизита проверять наличие выбора у студентов, у которых нет зачислений за ранние периоды на этот переквизит. 
            //Если такие выборы есть, то скидывать их
            var selections = db.StudentSelectionMinorPriority
                .Where(s => s.MinorPeriod.ModuleId == minor.ModuleId
                && !db.MinorAdmissions.Where(a => a.studentId == s.studentId
                                               && a.MinorPeriod.ModuleId == newRequirment.uuid
                                               && (a.MinorPeriod.Year < s.MinorPeriod.Year
                                                 || (a.MinorPeriod.Year == s.MinorPeriod.Year && a.MinorPeriod.SemesterId < s.MinorPeriod.SemesterId))
                                            ).Any());

            db.StudentSelectionMinorPriority.RemoveRange(selections);
        }

        //Get: Окошко для показа информации о нагрузке с кнопками к переходу редактирования 
        public ActionResult Tmers(int id,string message)
        {
            ViewBag.Message = message;
            var divisions = db.Divisions.ToDictionary(d => d.uuid);
            CreateTree(divisions);

            var discipline = db.MinorDisciplines.Include("Tmers.Periods").Include("Minor").Include("Discipline").Where(d => d.Id == id).Single();

            ViewData.Add("Tmers1", discipline.Tmers.Where(t => t.Tmer?.kgmer == 1).OrderBy(t => t.Tmer.kmer).ToList());
            ViewData.Add("Tmers2", discipline.Tmers.Where(t => t.Tmer?.kgmer == 2).OrderBy(t => t.Tmer.kmer).ToList());
            ViewData.Add("Tmers3", discipline.Tmers.Where(t => t.Tmer?.kgmer == 3).OrderBy(t => t.Tmer.kmer).ToList());

            ViewBag.CanEdit = IsAccessible(discipline.MinorId) && User.IsInRole(ItsRoles.MinorEdit);

            return View(discipline);
        }

        private void CreateTree(Dictionary<string, Division> divisions)
        {
            foreach (var d in divisions.Values)
            {
                Division parent;
                if (d.parent!=null && divisions.TryGetValue(d.parent, out parent))
                    d.ParentDivision = parent;
            }
        }

        //Get: Редактирование нагрузки
        [Authorize(Roles = ItsRoles.MinorEdit)]
        public ActionResult EditTmers(string minorId, string disciplineId)
        {
            var keys = new List<int?>(3) { 1, 2, 3 };
            var tmers = db.Tmers.Where(t => keys.Contains(t.kgmer)
                                         && t.kmer != "U039"
                                         && t.kmer != "U032"
                                         && t.kmer != "U033")
                .ToList();

            var minor = db.Minors.Include("Disciplines.Tmers").Where(m => m.ModuleId == minorId).Single();
            var discipline = db.Disciplines.Where(d => d.uid == disciplineId).Single();

            var model = new MinorTmersViewModel(minor, discipline, tmers);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.MinorEdit)]
        public ActionResult EditTmers(MinorTmersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var minor = db.Minors.Include("Disciplines.Tmers").Where(m => m.ModuleId == model.Minor.ModuleId).Single();
                var minorDiscipline = minor.Disciplines.FirstOrDefault(d => d.DisciplineUid == model.Discipline.uid);
                if (minorDiscipline == null)
                {
                    minorDiscipline = new MinorDiscipline
                    {
                        DisciplineUid = model.Discipline.uid,
                        MinorId = minor.ModuleId,
                        Tmers = new List<MinorDisciplineTmer>()
                    };
                    minor.Disciplines.Add(minorDiscipline);
                }

                var tmers = new List<MinorTmersRowViewModel>();
                tmers.AddRange(model.Tmers1);
                tmers.AddRange(model.Tmers2);
                tmers.AddRange(model.Tmers3);

                foreach (var t in tmers)
                {
                    var dt = minorDiscipline.Tmers.FirstOrDefault(f => f.TmerId == t.TmerId);
                    if (dt == null && t.Checked)
                    {
                        dt = new MinorDisciplineTmer { TmerId = t.TmerId };
                        minorDiscipline.Tmers.Add(dt);
                        //db.Entry(dt).State = EntityState.Added;
                    }
                    if (dt != null && !t.Checked)
                    {
                        minorDiscipline.Tmers.Remove(dt);
                        db.Entry(dt).State = EntityState.Deleted;
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Tmers", new { id = minorDiscipline.Id });
            }
            else
            {
                return RedirectToAction("Disciplines", new { moduleId = model.Minor.ModuleId });
            }
        }

        //Get: Редактирование периодов тут нагрузки уже заданы
        [Authorize(Roles = ItsRoles.MinorEdit)]
        public ActionResult EditPeriods(string minorId, string disciplineId)
        {
            var keys = new List<int?>(3) { 1, 2, 3 };
            var tmers = db.Tmers.Where(t => keys.Contains(t.kgmer)).ToList();
            var minor = db.Minors.Include(m => m.Periods).Where(m => m.ModuleId == minorId).Single();
            var discipline = db.MinorDisciplines.Include("Tmers.Periods").Where(d => d.MinorId == minorId && d.DisciplineUid == disciplineId).Single();

            var model = new MinorTmersPeriodViewModel(minor, discipline);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.MinorEdit)]
        public ActionResult EditPeriods(MinorTmersPeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var minorDiscipline = db.MinorDisciplines.Include("Tmers.Periods").Where(d => d.Id == model.Discipline.Id).Single();

                foreach (var t in model.Rows)
                {
                    var dt = minorDiscipline.Tmers.FirstOrDefault(f => f.Id == t.Tmer.Id);
                    var tp = dt.Periods.FirstOrDefault(f => f.MinorPeriodId == t.Period.Id);
                    if (tp == null && t.Checked)
                    {
                        tp = new MinorDisciplineTmerPeriod { MinorDisciplineTmerId = t.Tmer.Id, MinorPeriodId = t.Period.Id };
                        dt.Periods.Add(tp);
                        //db.Entry(dt).State = EntityState.Added;
                    }
                    if (tp != null && !t.Checked)
                    {
                        if (tp.Subgroups.Any())
                        {
                            return RedirectToAction("Tmers", new { id = minorDiscipline.Id, message = "Невозможно удалить периоды, т.к. на них есть подгруппы" });
                        }
                        dt.Periods.Remove(tp);
                        db.Entry(tp).State = EntityState.Deleted;
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Tmers", new { id = minorDiscipline.Id });
            }
            else
            {
                return RedirectToAction("Disciplines", new { moduleId = model.Minor.ModuleId });
            }
        }

        //Get: Редактирование кафедр
        [Authorize(Roles = ItsRoles.MinorEdit)]
        public ActionResult EditDivisions(int minorDisciplineTmerPeriodId)
        {
            var period = db.MinorTmerPeriods.Where(t => t.Id == minorDisciplineTmerPeriodId).Single();

            var divisions = db.Divisions.ToList();

            var model = new MinorDivisionViewModel(divisions, period,period.Tmer.MinorDisciplineId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.MinorEdit)]
        public ActionResult EditDivisions(MinorDivisionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var period = db.MinorTmerPeriods.Include(p => p.Divisions).Include(p => p.Tmer).Where(p => p.Id == model.PeriodId).Single();

                var rows = model.GetAllRows();
                foreach (var d in rows)
                {
                    var pd = period.Divisions.FirstOrDefault(f => f.uuid == d.DivisionID);

                    if (pd == null && d.Selected)
                    {
                        var division = new Division { uuid = d.DivisionID };
                        period.Divisions.Add(division);
                        db.Entry(division).State = EntityState.Unchanged;
                    }
                    if (pd != null && !d.Selected)
                    {
                        period.Divisions.Remove(pd);
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Tmers", new { id = period.Tmer.MinorDisciplineId });
            }
            else
            {
                return View(model);
            }
        }


        public ActionResult GetMinorAgreement(string moduleUuid)
        {
            var module = db.Modules.FirstOrDefault(m => m.uuid == moduleUuid);
            var disciplineUuids = module.disciplines.Select(d => d.pkey);
            var agreements = db.ModuleAgreements.Where(a => a.ModuleUUID == module.uuid && disciplineUuids.Contains(a.DisciplineUUID)
                && db.Modules.FirstOrDefault(m => m.uuid == moduleUuid).Minor.Periods.Any(p => p.Year == a.EduYear && p.SemesterId == a.SemesterId))
                .OrderBy(a => a.EduYear).ThenBy(a => a.SemesterId).ToList().Select(a => new
                {
                    yearAndSemester = $"{a.EduYear} учебный год, {a.Semester.Name} семестр",
                    dates = $"Обучение с {a.StartDate.Value.ToShortDateString()} по {a.EndDate.Value.ToShortDateString()}",
                    courseTitle = $"\"{a.CourseTitle}\"",
                    courseType = $"{a.CourseType}",
                    courseURL = $"{a.CourseURL}",
                    urfuInfoUrl = $"{a.URFUInfoURL}"
                });

            return Json(
                new
                {
                    data = agreements,
                    total = agreements.Count()
                },
                new JsonSerializerSettings()
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
