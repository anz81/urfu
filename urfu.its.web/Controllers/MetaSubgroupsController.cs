using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
//using Microsoft.Ajax.Utilities;
using PagedList.Core;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.StudentAdmission)]
    public class MetaSubgroupsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MetaSubgroups
        public ActionResult Index(int programId, int? page, int? limit, string sort, string filter,int? focus)
        {
            ViewBag.Focus = focus;
            CheckMetaSubgroups(programId);
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var eduprograms = db.MetaSubgroupsForUser(User).Include(s => s.Module).Include(s => s.Group).Where(p => p.programId == programId).Select(v => new
                {
                    v.Id,
                    Name = db.Plans.FirstOrDefault(p => p.disciplineUUID== v.disciplineUUID
                                            && p.directionId==v.Program.directionId
                                            && p.qualification==v.Program.qualification
                                            && p.familirizationCondition==v.Program.familirizationCondition
                                            && p.familirizationType==v.Program.familirizationType 
                                            && p.versionNumber == v.Program.PlanVersionNumber
                                            && p.eduplanNumber == v.Program.PlanNumber
                                            && p.active).disciplineTitle,
                    ModuleTitle = v.Module.title,
                    year = v.Year,
                    GroupName = v.Group.Name,
                    Term = v.Term.ToString(),
                    subgroupType = v.Tmer.rmer,
                    kgmer = v.Tmer.kgmer.ToString(),
                    count = v.Count,
                    v.Distribution,
                    Selectable = v.Selectable?"Да":"Нет"
                });

                SortRules sortRules = SortRules.Deserialize(sort);
                eduprograms = eduprograms.OrderByThenBy(sortRules.FirstOrDefault(), v => v.GroupName, v => v.ModuleTitle, v => v.Name, v => v.Term, v => v.kgmer, v => v.subgroupType);

                eduprograms = eduprograms.Where(FilterRules.Deserialize(filter));

                var paginated = eduprograms.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = eduprograms.Count()
                });
            }
            ViewBag.programId = programId;
            var program = db.EduPrograms.Find(programId);
            if (program != null)
                ViewBag.programInfo = program.FullName;
            return View();
        }

        private void CheckMetaSubgroups(int programId)
        {
            if(db.MetaSubgroups.Any(m=>m.programId==programId))
                return;

            RecalcMetaSubgroups(programId);
        }

        public ActionResult RecalculateMetas(int programId)
        {
            RecalcMetaSubgroups(programId);
            return RedirectToAction("Index", new {programId});
        }

        private void RecalcMetaSubgroups(int programId)
        {
            var program = db.EduPrograms.Find(programId);

            var plans = db.Plans.Where(p => p.qualification == program.qualification &&
                                            p.familirizationType == program.familirizationType &&
                                            p.familirizationCondition == program.familirizationCondition &&
                                            p.directionId == program.directionId &&
                                            p.versionNumber == program.PlanVersionNumber &&
                                            p.eduplanNumber == program.PlanNumber &&
                                            p.active &&
                                            (p.faculty == program.divisionId || p.faculty == program.departmentId || p.faculty == program.chairId) &&
                                            p.Module.UsedInVariantContents.Any(
                                                vc => vc.Group.Variant.EduProgramId == programId && vc.Selected)).ToList();

            var groups = db.Students.OnlyActive().Where(
                s =>
                    db.VariantAdmissions.Any(
                        va =>
                            va.Status == AdmissionStatus.Admitted && va.studentId == s.Id &&
                            va.Variant.EduProgramId == programId)).Select(s => s.Group).Distinct().ToList();

            var tmers = db.Tmers.Select(t => t.kmer).ToList();

            foreach (var group in groups)
                foreach (var plan in plans)
                {
                    var loads =
                        db.Apploads.Where(
                            a =>
                                (a.eduDiscipline == plan.disciplineUUID || a.eduDiscipline == plan.additionalUUID) &&
                                  a.grp == @group.Id && !a.removed && a.status == ApploadStatus.Approved
                                  && tmers.Contains(a.action) // пока условие на наличие tmer, чтобы не падало. Потом нужно сделать синхронизацию таблицы Tmers
                                  )
                            .GroupBy(a => new {a.term, a.action,a.year})
                            .ToList();

                    foreach (var load in loads)
                    {
                        int count = 0;
                        switch (load.Key.action)
                        {
                            case "tlekc":
                                count = load.Sum(x=>x.lectureFlows ?? 1);
                                break;
                            case "tprak":
                                count = load.Sum(x => x.practiceFlows ?? 0);
                                break;
                            case "tlab":
                                count = load.Sum(x => x.labSubgroups);
                                break;
                            default:
                                count = 0;
                                break;
                        }

                        var meta = new MetaSubgroup
                        {
                            kmer = load.Key.action,
                            disciplineUUID = plan.disciplineUUID,
                            additionalUUID = plan.additionalUUID,
                            catalogDisciplineUuid = plan.catalogDisciplineUUID,
                            programId = programId,
                            groupId = @group.Id,
                            Term = load.Key.term,
                            moduleId = plan.moduleUUID,
                            Count = count,
                            Year = load.Key.year
                        };
                        if (
                            !db.MetaSubgroups.Any(
                                m =>
                                    m.kmer == meta.kmer && 
                                    m.disciplineUUID == meta.disciplineUUID &&
                                    m.programId == meta.programId && 
                                    m.groupId == meta.groupId && 
                                    m.Term == meta.Term &&
                                    m.moduleId == meta.moduleId &&
                                    m.Year == meta.Year))
                            db.MetaSubgroups.Add(meta);
                    }
                }
            db.SaveChanges();
        }


        // GET: MetaSubgroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            MetaSubgroup metaSubgroup = db.MetaSubgroups.Find(id);
            if (metaSubgroup == null)
            {
                return NotFound();
            }
            return View(metaSubgroup);
        }

        // GET: MetaSubgroups/Create
        public ActionResult Create()
        {
            ViewBag.groupId = new SelectList(db.Groups, "Id", "Name");
            ViewBag.moduleId = new SelectList(db.UniModules(), "uuid", "title");
            ViewBag.programId = new SelectList(db.EduPrograms, "Id", "Name");
            ViewBag.kmer = new SelectList(db.Tmers, "kmer", "rmer");
            return View();
        }

        // POST: MetaSubgroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*[Bind(Include = "Id,groupId,moduleId,Term,programId,kmer,catalogDisciplineUuid,Count,Selectable")]*/ MetaSubgroup metaSubgroup)
        {
            if (ModelState.IsValid)
            {
                db.MetaSubgroups.Add(metaSubgroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.groupId = new SelectList(db.Groups, "Id", "Name", metaSubgroup.groupId);
            ViewBag.moduleId = new SelectList(db.UniModules(), "uuid", "title", metaSubgroup.moduleId);
            ViewBag.programId = new SelectList(db.EduPrograms, "Id", "Name", metaSubgroup.programId);
            ViewBag.kmer = new SelectList(db.Tmers, "kmer", "rmer", metaSubgroup.kmer);
            return View(metaSubgroup);
        }

        // GET: MetaSubgroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            MetaSubgroup metaSubgroup = db.MetaSubgroups.Find(id);
            if (metaSubgroup == null)
            {
                return NotFound();
            }

            string expectedChildKmer = null;
            if (metaSubgroup.kmer == "tlekc")
                expectedChildKmer = "tprak";
            if (metaSubgroup.kmer == "tprak")
                expectedChildKmer = "tlab";
            if (expectedChildKmer != null)
            {
                var expectedChildCount =
                    db.MetaSubgroups
                        .Where(s =>  s.programId == metaSubgroup.programId
                                    && s.Term == metaSubgroup.Term
                                    && s.groupId == metaSubgroup.groupId
                                    && s.catalogDisciplineUuid == metaSubgroup.catalogDisciplineUuid
                                    && s.kmer == expectedChildKmer
                        ).Select(s => (int?)s.Count).FirstOrDefault();
                ViewBag.ExpectedChildCount = expectedChildCount;
            }

            ViewBag.discipline = db.Plans.FirstOrDefault(p => p.disciplineUUID == metaSubgroup.disciplineUUID
                                                              && p.directionId == metaSubgroup.Program.directionId
                                                              && p.qualification == metaSubgroup.Program.qualification 
                                                              && p.versionNumber == metaSubgroup.Program.PlanVersionNumber
                                                              && p.eduplanNumber == metaSubgroup.Program.PlanNumber
                                                              &&
                                                              p.familirizationCondition == metaSubgroup.Program.familirizationCondition
                                                              && p.active
                                                              && p.familirizationType == metaSubgroup.Program.familirizationType)
                ?.disciplineTitle;
            return View(metaSubgroup);
        }

        // POST: MetaSubgroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(/*[Bind(Include = "Id,groupId,moduleId,Term,programId,kmer,catalogDisciplineUuid,Count,Selectable,Distribution")]*/ MetaSubgroup metaSubgroup)
        {
            if (ModelState.IsValid)
            {
                var msg = db.MetaSubgroups.Find(metaSubgroup.Id);
                msg.Selectable = metaSubgroup.Selectable;
                msg.Count = metaSubgroup.Count;
                msg.Distribution = metaSubgroup.Distribution;
                msg.CleanDistribution();
                db.SaveChanges();
                return RedirectToAction("Index",new {msg.programId,focus = metaSubgroup.Id});
            }


            metaSubgroup = db.MetaSubgroups.Find(metaSubgroup.Id);
            if (metaSubgroup == null)
            {
                return NotFound();
            }

            ViewBag.discipline = db.Plans.FirstOrDefault(p => p.disciplineUUID == metaSubgroup.disciplineUUID
                                                              && p.directionId == metaSubgroup.Program.directionId
                                                              && p.qualification == metaSubgroup.Program.qualification
                                                              && p.versionNumber == metaSubgroup.Program.PlanVersionNumber
                                                              && p.eduplanNumber == metaSubgroup.Program.PlanNumber
                                                              && p.familirizationCondition == metaSubgroup.Program.familirizationCondition
                                                              && p.active 
                                                              && p.familirizationType == metaSubgroup.Program.familirizationType)
                .disciplineTitle;

            return View(metaSubgroup);
        }

        // GET: MetaSubgroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            MetaSubgroup metaSubgroup = db.MetaSubgroups.Find(id);
            if (metaSubgroup == null)
            {
                return NotFound();
            }
            return View(metaSubgroup);
        }

        // POST: MetaSubgroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MetaSubgroup metaSubgroup = db.MetaSubgroups.Find(id);
            db.MetaSubgroups.Remove(metaSubgroup);
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
