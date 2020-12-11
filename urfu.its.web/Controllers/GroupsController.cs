using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList.Core;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class GroupsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Groups/
        public ActionResult Index(string sort, string filter, int? page, int? limit)
        {
            ViewBag.UniUrl = ConfigurationManager.AppSettings["UniUrl"] ?? "https://uni.dit.urfu.ru";
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var groups = db.GroupsForUser(User).Include(g=>g.Profile).Select(g => new
                {
                    g.Id,
                    g.Name,
                    ProfileName = g.Profile.NAME,
                    g.Year,
                    g.ChairId,
                    g.FormativeDivisionId,
                    g.FormativeDivisionParentId,
                    g.ManagingDivisionId,
                    g.ManagingDivisionParentId
                });
                SortRules sortRules = SortRules.Deserialize(sort);
                groups = groups.OrderBy(sortRules.FirstOrDefault(),m=>m.Name);

                groups = groups.Where(FilterRules.Deserialize(filter));

                var paginated = groups.ToPagedList(page ?? 1, limit ?? 25);

                return JsonNet(new
                {
                    data = paginated,
                    total = groups.Count()
                });
            }
            return View();
        }

        // GET: /Groups/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        // GET: /Groups/Create
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create(/*[Bind(Include="Id,Name,ProfileId,Year,ChairId,FormativeDivisionId,FormativeDivisionParentId,ManagingDivisionId,ManagingDivisionParentId")] */Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(group);
        }

        // GET: /Groups/Edit/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        // POST: /Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(/*[Bind(Include="Id,Name,ProfileId,Year,ChairId,FormativeDivisionId,FormativeDivisionParentId,ManagingDivisionId,ManagingDivisionParentId")] */Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: /Groups/Delete/5
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        // POST: /Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult DeleteConfirmed(string id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
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
