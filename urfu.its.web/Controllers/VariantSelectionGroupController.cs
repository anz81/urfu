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
    [Authorize(Roles = ItsRoles.VariantsEdit)]
    public class VariantSelectionGroupController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /VariantSelectionGroup/
        public ActionResult Index(int variantId)
        {
            Variant variant = db.Variants.Find(variantId);

            ViewBag.Title = "Редактирование групп выбора траектории " + variant.Name + " направление " +
                            variant.Program.Direction.okso;
            ViewBag.BackButtonText = variant.IsBase ? "Редактирование модулей версии ОП" : "Редактирование траектории";

            return
                View(new EditVariantSelectionGroupViewModel
                {
                    Rows =
                        variant.SelectionGroups.Select(m => new EditVariantSelectionGroupRowViewModel(m)).ToList(),
                    VaraintName = variant.Name,
                    VariantId = variant.Id
                });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(EditVariantSelectionGroupViewModel model)
        {
            Variant variant = db.Variants.Find(model.VariantId);

            ViewBag.BackButtonText = variant.IsBase ? "Редактирование модулей версии ОП" : "Редактирование траектории";

            if(variant.State==VariantState.Approved)
                ModelState.AddModelError(string.Empty,"Невозможно изменить утверждённый варианта");

            if (ModelState.IsValid)
            {

                    foreach (var group in variant.SelectionGroups)
                    {
                        var row = model.Rows.First(r=>r.Id == @group.Id);
                        group.Name = row.Name;
                        group.TestUnits = row.TestUnits;
                        group.SelectionDeadline = row.SelectionDeadline;
                    }

                    db.SaveChanges();
                    variant.OnChanged();

                    return RedirectToAction("index", "VariantSelectionGroup", new { model.VariantId });
                
            }
            return View(model);
        }

        public ActionResult Create(int variantId)
        {
            Variant variant = db.Variants.Find(variantId);

            if (variant.State != VariantState.Approved)
            {
                variant.OnChanged();
                variant.SelectionGroups.Add(new VariantSelectionGroup() {Name = "Новая группа выбора"});

                db.SaveChanges();
            }


            return RedirectToAction("Index", new {variantId});
        }

        // GET: /VariantSelectionGroup/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            VariantSelectionGroup variantselectiongroup = db.VariantSelectionGroups.Find(id);
            if (variantselectiongroup.Contents.Any())
                ModelState.AddModelError(string.Empty,"Группа используется в траектории. Её невозможно удалить");
            if (variantselectiongroup == null)
            {
                return NotFound();
            }
            return View(variantselectiongroup);
        }

        // POST: /VariantSelectionGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VariantSelectionGroup variantselectiongroup = db.VariantSelectionGroups.Find(id);
            var variantId = variantselectiongroup.VariantId;
            if (variantselectiongroup.Contents.Any())
                return RedirectToAction("Delete", new {id});
            if (variantselectiongroup.Variant.State != VariantState.Approved)
            {
                db.VariantSelectionGroups.Remove(variantselectiongroup);
                db.SaveChanges();
                if (variantselectiongroup.Variant != null)
                    variantselectiongroup.Variant.OnChanged();
            }
            return RedirectToAction("Index", new { variantId });
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
