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
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.Admin)]
    public class RoleSetsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RoleSets
        public ActionResult Index()
        {
            return View(db.RoleSets.ToList());
        }

        public ActionResult ForUser(string id)
        {
            return
                JsonNet(
                    db.RoleSets.Include(r => r.Contents)
                        .Select(r => new
                        {
                            r.Id,
                            r.Name,
                            set = r.Contents.All(c=> db.Users.FirstOrDefault(u=>u.UserName==id).Roles.Any(rx=>rx.RoleId==c.RoleId))
                        }));
        }

        // GET: RoleSets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            RoleSet roleSet = db.RoleSets.Find(id);
            if (roleSet == null)
            {
                return NotFound();
            }
            return View(roleSet);
        }

        // GET: RoleSets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoleSets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*[Bind(Include = "Id,Name")] */RoleSet roleSet)
        {
            if (ModelState.IsValid)
            {
                db.RoleSets.Add(roleSet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roleSet);
        }

        // GET: RoleSets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            RoleSet roleSet = db.RoleSets.Find(id);
            if (roleSet == null)
            {
                return NotFound();
            }
            return View(new SelectUserRolesViewModel(roleSet));
        }

        // POST: RoleSets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SelectUserRolesViewModel roleSet)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(roleSet).State = EntityState.Modified;
                var rs = db.RoleSets.Find(int.Parse(roleSet.FirstName));
                foreach (var r in rs.Contents.ToList())
                {
                    if(roleSet.Roles.Any(rx=>rx.Id==r.RoleId && rx.Selected))
                        continue;
                    rs.Contents.Remove(r);
                }

                foreach (var r in roleSet.Roles)
                {
                    if (!r.Selected || rs.Contents.Any(rx => rx.RoleId == r.Id))
                        continue;
                    rs.Contents.Add(new RoleSetContent
                    {
                        RoleId = r.Id,
                        RoleSetId = rs.Id
                    });
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roleSet);
        }

        // GET: RoleSets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            RoleSet roleSet = db.RoleSets.Find(id);
            if (roleSet == null)
            {
                return NotFound();
            }
            return View(roleSet);
        }

        // POST: RoleSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoleSet roleSet = db.RoleSets.Find(id);
            db.RoleSets.Remove(roleSet);
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
