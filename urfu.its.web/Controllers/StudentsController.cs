using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
//using Microsoft.Ajax.Utilities;
using PagedList;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using PagedList.Core;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class StudentsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Students/
        public ActionResult Index(int? page, string sort, string filter, int? limit)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var query = User.IsInRole(ItsRoles.NsiView) ? db.Students : db.StudentsForUser(User);
                var students = query.Include(s => s.Person).Include(s => s.Group).Select(s => new
                {
                    id = s.Id,
                    name = s.Person.Surname + " " + s.Person.Name + " " + s.Person.PatronymicName,
                    status = s.Status,
                    groupName = s.Group.Name,
                    rating = s.Rating,
                    s.Sportsman
                });
                SortRules sortRules = SortRules.Deserialize(sort);
                students = students.OrderBy(sortRules.FirstOrDefault(), m => m.name);

                students = students.Where(FilterRules.Deserialize(filter));

                var paginated = students.ToPagedList(page ?? 1, limit ?? 25);

                return JsonNet(new
                {
                    data = paginated,
                    total = students.Count()
                });

            }
            var statusTypes = db.Students.Select(s => s.Status).Distinct().Select(s => new { status = s }).ToList();

            ViewBag.statusTypes = statusTypes;
            return View();
        }

        [Authorize(Roles = ItsRoles.SportsmanSetting)]
        [HttpPost]
        public ActionResult SetSportsman(string studentId, bool sportsman)
        {
            var student = db.Students.Find(studentId);
            student.Sportsman = sportsman;
            db.SaveChanges();
            return JsonNet("OK");
        }

        // GET: /Students/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // GET: /Students/Create
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create(/*[Bind(Include = "Id,PersonId,Status,GroupId")] */Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: /Students/Edit/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: /Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(/*[Bind(Include = "Id,PersonId,Status,GroupId")] */Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: /Students/Delete/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: /Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult DeleteConfirmed(string id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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
