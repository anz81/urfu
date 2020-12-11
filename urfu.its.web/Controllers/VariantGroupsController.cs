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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.VariantsView)]
    public class VariantGroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /VariantGroups/
        /*public ActionResult Index()
        {
            var variantgroups = db.VariantGroups.Include(v => v.Variant);
            return View(variantgroups.ToList());
        }*/

        public ActionResult Index(int? variantId)
        {
            if (variantId == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            ViewBag.variantId = variantId.Value;

            var variant = db.Variants.Find(variantId);
            ViewBag.Title = string.Format("Группы варианта {0}  \n направление {1}", variant.Name, variant.Program.Direction.okso);
            return View(db.Variants.Find(variantId).Groups.ToList());
        }

        // GET: /VariantGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            VariantGroup variantgroup = db.VariantGroups.Find(id);
            if (variantgroup == null)
            {
                return NotFound();
            }
            return View(variantgroup);
        }

        // GET: /VariantGroups/Create
        /*public ActionResult Create()
        {
            ViewBag.VariantId = new SelectList(db.Variants, "Id", "Name");
            return View();
        }
*/
        // GET: /VariantGroups/Create

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Create(int? variantId)
        {
            ViewBag.VariantId = new SelectList(db.VariantsForUser(User), "Id", "Name", variantId);
            return View();
        }

        // POST: /VariantGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Create(/*[Bind(Include = "Id,Name,TestUnits,VariantId")] */VariantGroup variantgroup)
        {
            var variant = db.Variants.Find(variantgroup.VariantId);
            if (variant.State == VariantState.Approved)
                ModelState.AddModelError(string.Empty, "Невозможно изменить утверждённый варианта");
            if(variant.Groups.Any(g=>g.GroupType==variantgroup.GroupType))
                ModelState.AddModelError(string.Empty, "Группа с таким типом уже существует");
            if (ModelState.IsValid)
            {
                db.VariantGroups.Add(variantgroup);
                db.SaveChanges();
                variant.OnChanged();
                return RedirectToAction("Index", new { variantId = variantgroup.VariantId });
            }

            ViewBag.VariantId = new SelectList(db.VariantsForUser(User), "Id", "Name", variantgroup.VariantId);
            return View(variantgroup);
        }

        // GET: /VariantGroups/Edit/5
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            VariantGroup variantgroup = db.VariantGroups.Find(id);
            if (variantgroup == null)
            {
                return NotFound();
            }
            ViewBag.VariantId = new SelectList(db.VariantsForUser(User), "Id", "Name", variantgroup.VariantId);
            return View(variantgroup);
        }

        // POST: /VariantGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Edit(/*[Bind(Include = "Id,GroupType,TestUnits,VariantId")] */VariantGroup variantgroup)
        {
            var variant = db.Variants.Find(variantgroup.VariantId);
            if (variant.State == VariantState.Approved)
                ModelState.AddModelError(string.Empty, "Невозможно изменить утверждённый варианта");
            if (ModelState.IsValid)
            {
                db.Entry(variantgroup).State = EntityState.Modified;
                db.SaveChanges();
                variant.OnChanged();
                return RedirectToAction("Index", new { variantId = variantgroup.VariantId });
            }
            ViewBag.VariantId = new SelectList(db.VariantsForUser(User), "Id", "Name", variantgroup.VariantId);
            return View(variantgroup);
        }

        // GET: /VariantGroups/Delete/5
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            VariantGroup variantgroup = db.VariantGroups.Find(id);
            if (variantgroup == null)
            {
                return NotFound();
            }
            return View(variantgroup);
        }

        // POST: /VariantGroups/Delete/5
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VariantGroup variantgroup = db.VariantGroups.Find(id);
            if (variantgroup.Variant.State != VariantState.Approved)
            {
                db.VariantGroups.Remove(variantgroup);
                db.SaveChanges();

                variantgroup.Variant.OnChanged();
                db.Variants.Find(variantgroup.VariantId).OnChanged();
            }
            return RedirectToAction("Index", new { variantId = variantgroup.VariantId });
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
