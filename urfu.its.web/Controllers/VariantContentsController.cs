using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.VariantsView)]
    public class VariantContentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /VariantContents/
        public ActionResult Index(int variantId)
        {
            var variantcontents = db.VariantContents.Where(vc=>vc.Group.VariantId == variantId && vc.Selected).Include(v => v.Group).Include(v => v.Module).Include(v => v.Group.Variant);
            ViewBag.variantId = variantId;
            var variant = db.Variants.Find(variantId);
            ViewBag.BackButtonText = variant.IsBase ? "Редактирование модулей версии ОП" : "Редактирование траектории";
            ViewBag.Title = string.Format("Модули варианта {0} \n направление {1}", variant.Name, variant.Program.Direction.okso);
            return View(variantcontents.ToList());
        }

        // GET: /VariantContents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            VariantContent variantcontent = db.VariantContents.Find(id);
            if (variantcontent == null)
            {
                return NotFound();
            }
            return View(variantcontent);
        }

        // GET: /VariantContents/Create
        //[Authorize(Roles = ItsRoles.VariantsEdit)]
        //public ActionResult Create(int? variantId)
        //{
        //    ViewBag.VariantGroupId = new SelectList(db.Variants.Find(variantId).Groups, "Id", "Name");
        //    ViewBag.moduleId =
        //        new SelectList(
        //            ModulesForVariant(variantId), "uuid", "title");
        //    ViewBag.VariantId = new SelectList(db.VariantsForUser(User), "Id", "Name", variantId);
        //    return View();
        //}

        private IQueryable<Module> ModulesForVariant(int? variantId)
        {
            return db.UniModules().Where(
                m =>
                    m.Directions.SelectMany(d=>d.Programs.SelectMany(p=>p.Variants)).Any(v => v.Id == variantId) &&
                    m.UsedInVariantContents.All(vc => vc.Group.VariantId != variantId));
        }

        public string Graph(int variantId)
        {
            var variant = db.Variants.Find(variantId);

            var graph = new GraphModel
            {
                nodes = new List<GraphNode>(),
                edges = new List<GraphEdge>()
            };


            Random rnd = new Random();
            foreach (var group in variant.Groups)
            {
                foreach (var vc in group.Contents)
                {
                    graph.nodes.Add(new GraphNode
                    {
                        id = vc.Id.ToString(),
                        label = vc.Module.title,
                        size = 10,
                        x = rnd.Next(800),
                        y = rnd.Next(600),
                    });
                    foreach (var r in vc.Requirments)
                    {
                        graph.edges.Add(new GraphEdge
                        {
                            id = vc.Id.ToString() + " " +r.Id.ToString(),
                            source = r.Id.ToString(),
                            target = vc.Id.ToString()
                        });
                    }
                }
            }

            return JsonConvert.SerializeObject(graph);
        }

        // POST: /VariantContents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Create(/*[Bind(Include="Id,moduleId,GroupName,Selectable,Limits,VariantGroupId,VariantId")] */VariantContent variantcontent)
        {
            if (ModelState.IsValid)
            {
                db.VariantContents.Add(variantcontent);
                db.SaveChanges();

                return RedirectToAction("Index", new { variantId = db.VariantContents.Where(vc => vc.VariantGroupId == variantcontent.VariantGroupId).Select(vc => vc.Group.VariantId).First() });
            }

            ViewBag.VariantGroupId = new SelectList(db.VariantGroups, "Id", "GroupType", variantcontent.VariantGroupId);
            ViewBag.moduleId = new SelectList(db.UniModules(), "uuid", "title", variantcontent.moduleId);
            return View(variantcontent);
        }

        // GET: /VariantContents/Edit/5
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            VariantContent variantcontent = db.VariantContents.Find(id);
            if (variantcontent == null)
            {
                return NotFound();
            }
            ViewBag.VariantGroupId = new SelectList(db.VariantGroups.Where(vg => vg.Variant.Groups.Any(vg1 => vg1.Id == variantcontent.VariantGroupId)), "Id", "Name", variantcontent.VariantGroupId);
            ViewBag.moduleId = new SelectList(db.UniModules(), "uuid", "title", variantcontent.moduleId);
            ViewBag.RequirmentsIds = new MultiSelectList(variantcontent.LoadPossibleRequirements(db), "Id", "Module.numberAndTitle", variantcontent.Requirments.Select(r => r.Id));
            ViewBag.EffectiveRequirments = variantcontent.LoadEffectiveRequirements(db);
            ViewBag.IsBase = db.VariantContents.Where(vc => vc.Id == id).Any(vc => vc.Group.Variant.IsBase);
            return View(variantcontent);
        }

        // POST: /VariantContents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Edit(/*[Bind(Include = "Id,moduleId,GroupName,Selected,Selectable,Limits,VariantGroupId,VariantId,RequirmentsIds,ModuleTypeId")]*/ VariantContent variantcontent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(variantcontent).State = EntityState.Modified;
                db.SaveChanges();

                variantcontent = db.VariantContents.Include(r=>r.Requirments).First(vc=>vc.Id==variantcontent.Id);
                variantcontent.UpdateRequirements(db);
                db.SaveChanges();

                var variant = db.Variants.First(v=>v.Groups.Any(g=>g.Id==variantcontent.VariantGroupId));

                Logger.Info("Изменён варинат '{0}' с направлением '{1}'", variant.Name, variant.Program.Direction.okso);
                return RedirectToAction("Index", new { variantId = db.VariantContents.Where(vc => vc.VariantGroupId == variantcontent.VariantGroupId).Select(vc => vc.Group.VariantId).First() });
            }
            variantcontent = db.VariantContents.Find(variantcontent.Id);
            ViewBag.VariantGroupId = new SelectList(db.VariantGroups.Where(vg => vg.Variant.Groups.Any(vg1 => vg1.Id == variantcontent.VariantGroupId)), "Id", "Name", variantcontent.VariantGroupId);
            ViewBag.moduleId = new SelectList(db.UniModules(), "uuid", "title", variantcontent.moduleId);
            ViewBag.RequirmentsIds = new MultiSelectList(variantcontent.Group.Variant.Groups.SelectMany(g => g.Contents).ToList(), "Id", "Module.numberAndTitle", variantcontent.RequirmentsIds);
            ViewBag.EffectiveRequirments = variantcontent.LoadEffectiveRequirements(db);
            ViewBag.IsBase = db.VariantContents.Where(vc => vc.Id == variantcontent.Id).Any(vc => vc.SelectionGroup.Variant.IsBase);
            return View(variantcontent);
        }

        // GET: /VariantContents/Delete/5
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            VariantContent variantcontent = db.VariantContents.Find(id);
            if (variantcontent == null)
            {
                return NotFound();
            }
            return View(variantcontent);
        }

        // POST: /VariantContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult DeleteConfirmed(int id)
        {
            VariantContent variantcontent = db.VariantContents.Find(id);
            db.VariantContents.Remove(variantcontent);
            db.SaveChanges();
            return RedirectToAction("Index", new { variantId = db.VariantContents.Where(vc => vc.VariantGroupId == variantcontent.VariantGroupId).Select(vc => vc.Group.VariantId).First() });
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
