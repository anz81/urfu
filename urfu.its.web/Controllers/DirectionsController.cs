using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class DirectionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? page, int? limit)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var directions = db.DirectionsForUser(User).ToList().Select(d => new {
                    id = d.uid, okso = d.okso,
                    title = d.title, ministerialCode = d.ministerialCode,
                    d.ugnTitle, d.standard, d.qualifications
                });
                
                //var directionsPaginated = directions.ToPagedList(page ?? 1, limit ?? 25);

                var result = new { data = directions, total = directions.Count()};
                return Json(result, new JsonSerializerSettings());
            }
            else
            {
                return View();
            }
        }

        // GET: Directions/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Direction direction = db.Directions.Find(id);
            if (direction == null)
            {
                return NotFound();
            }
            return View(direction);
        }

        // GET: Directions/Create
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Directions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create(/*[Bind(Include = "uid,okso,title,ministerialCode,ugnTitle,standard,qualifications")]*/ Direction direction)
        {
            if (ModelState.IsValid)
            {
                db.Directions.Add(direction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(direction);
        }

        // GET: Directions/Edit/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Direction direction = db.Directions.Find(id);
            if (direction == null)
            {
                return NotFound();
            }
            return View(direction);
        }

        // POST: Directions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        //[Bind(Include = "uid,okso,title,ministerialCode,ugnTitle,standard,qualifications")] 
        public ActionResult Edit(Direction direction)
        {
            
            if (ModelState.IsValid)
            {
                db.Entry(direction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(direction);
        }

        // GET: Directions/Delete/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Direction direction = db.Directions.Find(id);
            if (direction == null)
            {
                return NotFound();
            }
            return View(direction);
        }

        // POST: Directions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult DeleteConfirmed(string id)
        {
            Direction direction = db.Directions.Find(id);
            db.Directions.Remove(direction);
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
