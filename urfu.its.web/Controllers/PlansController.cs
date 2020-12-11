using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class PlansController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Plans/
        public ActionResult Index(int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var plans = db.PlansForUser(User).Select(p => new { id = p.eduplanUUID, p.eduplanNumber, p.versionNumber,
                            p.disciplineTitle, DirectionOkso = db.Directions.FirstOrDefault(d => d.uid == p.directionId).okso, p.controls, p.loads, p.terms });

                plans = plans.OrderBy(SortRules.Deserialize(sort).FirstOrDefault(), p => p.versionNumber);
                plans = plans.Where(FilterRules.Deserialize(filter));

                var paginated = plans.ToPagedList(page ?? 1, limit ?? 25);

                return Json(
                    new
                    {
                        data = paginated,
                        total = plans.Count()
                    },
                    new JsonSerializerSettings());
            }
            else
            { return View(); }

        }



        // GET: /Plans/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Plan plan = db.Plans.Find(id);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }

        // GET: /Plans/Create
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Plans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create(/*[Bind(Include = "moduleUUID,eduplanUUID,disciplineUUID,eduplanNumber,versionNumber,disciplineTitle,directionId,controls,loads,terms,allTermsExtracted")]*/ Plan plan)
        {
            if (ModelState.IsValid)
            {
                db.Plans.Add(plan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(plan);
        }

        // GET: /Plans/Edit/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Plan plan = db.Plans.Find(id);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }

        // POST: /Plans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(/*[Bind(Include = "moduleUUID,eduplanUUID,disciplineUUID,eduplanNumber,versionNumber,disciplineTitle,directionId,controls,loads,terms,allTermsExtracted")]*/ Plan plan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(plan);
        }

        // GET: /Plans/Delete/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Plan plan = db.Plans.Find(id);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }

        // POST: /Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult DeleteConfirmed(string id)
        {
            Plan plan = db.Plans.Find(id);
            db.Plans.Remove(plan);
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
