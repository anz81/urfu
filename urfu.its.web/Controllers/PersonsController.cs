using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList;
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
using PagedList.Core;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class PersonsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Persons/
        public ActionResult Index(int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var persons = ((IQueryable<Person>)db.Persons).Select(p => new { id = p.Id, p.Name, p.Surname, p.PatronymicName, p.Phone });
                var sortRules = SortRules.Deserialize(sort);
                var filterRules = FilterRules.Deserialize(filter);

                if (sortRules == null || sortRules.Count == 0)
                {
                    persons = persons.OrderBy(p => p.Surname);
                }
                else
                {
                    var sortRule = sortRules[0];
                    persons = persons.OrderBy(sortRule);
                }

                if (!(filter == null || filterRules.Count == 0))
                {
                    foreach (var filterRule in filterRules)
                    {
                        switch (filterRule.Property)
                        {
                            case "Name":
                                persons = persons.Where(d => d.Name.Contains(filterRule.Value));
                                break;
                            case "Surname":
                                persons = persons.Where(d => d.Surname.Contains(filterRule.Value));
                                break;
                            case "PatronymicName":
                                persons = persons.Where(d => d.PatronymicName.Contains(filterRule.Value));
                                break;
                        }
                    }
                }

                var paginated = persons.ToPagedList(page ?? 1, limit ?? 25);

                return Json(
                    new
                    {
                        data = paginated,
                        total = persons.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            else
                return View();
        }

        // GET: /Persons/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // GET: /Persons/Create
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Persons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create(Person person)
        {
            if (ModelState.IsValid)
            {
                db.Persons.Add(person);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(person);
        }

        // GET: /Persons/Edit/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: /Persons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(person);
        }

        // GET: /Persons/Delete/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: /Persons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult DeleteConfirmed(string id)
        {
            Person person = db.Persons.Find(id);
            db.Persons.Remove(person);
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
