using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using PagedList.Core;
using Ext.Utilities;
using Ext.Utilities.Linq;
using System.Linq.Expressions;
//using Microsoft.Ajax.Utilities;
using Urfu.Its.Web.Model.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class ModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Modules
        public ActionResult Index(int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var sortRules = SortRules.Deserialize(sort);

                var filterRules = FilterRules.Deserialize(filter);

                var moduleDocKindTypes = new string[]
                {
                    "Безопасность жизнедеятельности", "Физическая культура", "По выбору", "Майноры",
                    "Факультативные дисциплины", "Стандартный"
                };

                var giaDocKindTypes = new[] { "Дипломирование", "Итоговая государственная аттестация" };

                var practicesDocKindType = new[] { "Учебная и производственная практики" };

                var modulesQuery = db.ModulesForUser(User);

                if (!User.IsInRole(ItsRoles.Admin))
                {
                    if (!User.IsInRole(ItsRoles.AllDirections))
                    {
                        //var divisionFullNames = db.FlattenedHierarchicalDivisionsForUser(User)
                        //.Select(d => d.typeTitle + " «" + d.title + "»");
                        var userName = User.Identity.Name;
                        var divisions = (from ud in db.UserDivisions
                            join d in db.Divisions
                                on ud.DivisionId equals d.uuid
                            where ud.UserName == userName
                            select d).ToList();

                        var divisionFullNames = divisions.SelectMany(d => new []
                        {
                            d.typeTitle + " «" + d.title + "»",
                            "«" + d.title + "»"
                        }).ToList();
                        modulesQuery = modulesQuery.Where(m => divisionFullNames.Contains(m.coordinator));
                    }
                }

                var disciplineResponsiblePersonsModules = db.WorkingProgramResponsiblePersons
                    .Where(u => u.UserId == User.Identity.Name).Select(p => p.Module);
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
                        documentKind = moduleDocKindTypes.Contains(m.type) ? DocumentKind.Module : giaDocKindTypes.Contains(m.type) ? DocumentKind.Gia : practicesDocKindType.Contains(m.type) ? DocumentKind.Practics : (DocumentKind?)null,
                        m.number,
                        m.annotation
                    });

                if (sortRules == null || sortRules.Count == 0)
                {
                    modules = modules.OrderBy(d => d.title);
                }
                else
                {
                    var sortRule = sortRules[0];
                    modules = modules.OrderBy(sortRule);
                }

                modules = modules.Where(filterRules);
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
                return View();
            }
        }



        public ActionResult DisciplinesByModule(string moduleId)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var module = db.UniModules().Include(_ => _.disciplines).FirstOrDefault(_ => _.uuid == moduleId);
                db.Users.Load();
                var disciplines = module.disciplines.Select(d => new
                {
                    d.title,
                    d.file,
                    d.number,
                    d.section,
                    d.pkey,
                    d.uid,
                    d.testUnits,
                    users = string.Join(",",
                        d.WorkingProgramResponsiblePersons.Select(_ => String.Format("{0}.{1}.{2}", _.User.LastName,
                            _.User.FirstName.FirstOrDefault(), _.User.Patronymic.FirstOrDefault())))
                });

                return Json(disciplines, new JsonSerializerSettings());
            }
            var module1 = db.UniModules().Include(_ => _.disciplines).FirstOrDefault(_ => _.uuid == moduleId);

            return View(module1);
        }
        public ActionResult EduPlanVersion(string speciality, string qualification, string familirization, int eduPlanNumber)
        {
            var eduPlanNumbers =
                db.Directions.Where(_ => _.okso == speciality)
                    .SelectMany(d => db.Plans.Where(p => p.directionId == d.uid && p.qualification == qualification && familirization == p.familirizationType && p.eduplanNumber == eduPlanNumber))
                    .Distinct()
                    .Select(p => new { Id = p.versionNumber, Name = p.versionNumber }).ToList();
            return Json(eduPlanNumbers, new JsonSerializerSettings());
        }
        public ActionResult EduPlanNumbers(string speciality, string qualification, string familirization)
        {
            var eduPlanNumbers =
                db.Directions.Where(_ => _.okso == speciality)
                    .SelectMany(d => db.Plans.Where(p => p.directionId == d.uid && p.qualification == qualification && familirization == p.familirizationType))
                    .Where(p => p.eduplanNumber != null)
                    .Distinct()
                    .Select(p => new { Id = p.eduplanNumber, Name = p.eduplanNumber }).ToList();
            return Json(eduPlanNumbers, new JsonSerializerSettings());
        }
        public ActionResult FamilirizationTypesByOksoAndQual(string speciality, string qualification)
        {
            var familirizations =
                db.Directions.Where(_ => _.okso == speciality)
                    .SelectMany(d => db.Plans.Where(p => p.directionId == d.uid && p.qualification == qualification))
                    .Where(p => p.eduplanNumber != null)
                   .Distinct()
                    .Select(p => new { Id = p.familirizationType, Name = p.familirizationType }).ToList();
            return Json(familirizations, new JsonSerializerSettings());
        }
        public ActionResult QualificationsByOkso(string speciality)
        {
            var qualifications =
               db.Directions.Where(_ => _.okso == speciality)
                   .SelectMany(d => db.Plans.Where(p => p.directionId == d.uid))
                   .Where(p => p.eduplanNumber != null)
                   .Distinct()
                   .Select(p => new { Id = p.qualification, Name = p.qualification }).ToList();
            return Json(qualifications, new JsonSerializerSettings());
        }

        public ActionResult DownloadRP(string disciplineUUID, string moduleUUID)
        {
            var discipline = db.Disciplines.FirstOrDefault(_ => _.uid == disciplineUUID);
            var module = db.UniModules().FirstOrDefault(_ => _.uuid == moduleUUID);
            var catalogUUID = discipline.uid.Substring(6, 32);
            var plans = module.Plans.Where(_ => _.catalogDisciplineUUID == catalogUUID);
            throw new NotImplementedException();
        }
        // GET: Modules/Details/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return NotFound();
            }
            return View(module);
        }

        // GET: Modules/Create
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create(/*[Bind(Include = "uuid,title,shortTitle,coordinator,type,competence,testUnits,priority,state,approvedDate,comment,file,specialities")] */Module module)
        {
            if (ModelState.IsValid)
            {
                db.Modules.Add(module);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(module);
        }

        // GET: Modules/Edit/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return NotFound();
            }
            return View(module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(/*[Bind(Include = "uuid,title,shortTitle,coordinator,type,competence,testUnits,priority,state,approvedDate,comment,file,specialities")]*/ Module module)
        {
            if (ModelState.IsValid)
            {
                db.Entry(module).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(module);
        }

        // GET: Modules/Delete/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return NotFound();
            }
            return View(module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult DeleteConfirmed(string id)
        {
            Module module = db.Modules.Find(id);
            db.Modules.Remove(module);
            db.SaveChanges();
            return RedirectToAction("Index");
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
