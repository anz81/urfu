using Ext.Utilities;
using Ext.Utilities.Linq;
using Newtonsoft.Json;
using PagedList;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using PagedList.Core;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class DisciplinesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Disciplines
        public ActionResult Index(int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var disciplines = db.DisciplinesForUser(User).Select(d => new { id = d.uid, d.title, d.section, d.testUnits, d.file });

                var sortRules = SortRules.Deserialize(sort);

                disciplines = disciplines.OrderBy(sortRules.FirstOrDefault(), d => d.title);

                disciplines = disciplines.Where(FilterRules.Deserialize(filter));

                var paginated = disciplines.ToPagedList(page ?? 1, limit ?? 25);
                
                return JsonNet(
                    new
                    {
                        data = paginated,
                        total = disciplines.Count()
                    }
                );
            }
            else
                return View();
        }

        public ActionResult ModuleByDiscipline(string disciplineId)
        {
            var discipline = db.Disciplines.Find(disciplineId);
            ViewBag.Title = "Модули дисциплины "+discipline.title;
            return View(discipline.Modules.ToList());
        }

        // GET: Disciplines/Create
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Disciplines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create(/*[Bind(Include = "uid,title,section,testUnits,file")] */Discipline discipline)
        {
            if (ModelState.IsValid)
            {
                db.Disciplines.Add(discipline);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(discipline);
        }

        // GET: Disciplines/Edit/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Discipline discipline = db.Disciplines.Find(id);
            if (discipline == null)
            {
                return NotFound();
            }
            return View(discipline);
        }

        // POST: Disciplines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(/*[Bind(Include = "uid,title,section,testUnits,file")] */Discipline discipline)
        {
            if (ModelState.IsValid)
            {
                db.Entry(discipline).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(discipline);
        }

        // GET: Disciplines/Delete/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Discipline discipline = db.Disciplines.Find(id);
            if (discipline == null)
            {
                return NotFound();
            }
            return View(discipline);
        }

        // POST: Disciplines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult DeleteConfirmed(string id)
        {
            Discipline discipline = db.Disciplines.Find(id);
            db.Disciplines.Remove(discipline);
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
