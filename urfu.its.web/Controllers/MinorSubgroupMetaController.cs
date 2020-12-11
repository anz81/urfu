using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.MinorCreateGroup)]
    public class MinorSubgroupMetaController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MinorMetaSubgroup
        public ActionResult Index(int? page, int? limit, string sort, string filter, int? focus)
        {
            ViewBag.Focus = focus;

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var metas = db.MinorTmerPeriods.Select(m => new
                {
                    m.Id,
                    year = m.Period.Year.ToString(),
                    semester = m.Period.Semester.Name,
                    title = m.Period.Minor.Module.title,
                    discipline = m.Tmer.Discipline.Discipline.title,
                    tmer = m.Tmer.Tmer.rmer,
                    count = m.GroupCount,
                    admission = db.MinorAdmissions.Count(a => a.Status == AdmissionStatus.Admitted && a.minorPeriodId == m.MinorPeriodId)
                });

                var sortRules = SortRules.Deserialize(sort);
                metas = metas.OrderByThenBy(sortRules.FirstOrDefault(), m=>m.title, m=>m.year, m=>m.semester);

                metas = metas.Where(FilterRules.Deserialize(filter));

                var paginated = metas.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = metas.Count()
                });
            }
            
            return View();

        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            var meta = db.MinorTmerPeriods.Find(id);
            if (meta == null)
            {
                return NotFound();
            }
            
           
            string expectedChildKmer = null;
            if (meta.Tmer.Tmer.kmer.ToLower() == "tlekc")
                expectedChildKmer = "tprak";
            if (meta.Tmer.Tmer.kmer.ToLower() == "tprak")
                expectedChildKmer = "tlab";
            if (expectedChildKmer != null)
            {
                var expectedChildCount =
                    db.MinorTmerPeriods
                        .Where(s => s.MinorPeriodId == meta.MinorPeriodId && s.Tmer.Tmer.kmer == expectedChildKmer)
                        .Select(s => (int?)s.GroupCount)
                        .FirstOrDefault();

                ViewBag.ExpectedChildCount = expectedChildCount;
                
            }

             var Readonly = true;
            if (HttpContext.User.IsInRole("AllMinor"))
            {
                Readonly = false;
            }
            else
                Readonly = db.UserMinors.Where(m => m.UserName == HttpContext.User.Identity.Name && m.ModuleId == meta.Period.ModuleId).ToList().Count > 0 ? false : true;

            ViewBag.Readonly = Readonly;

            return View(meta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MinorDisciplineTmerPeriod meta)
        {
          if (ModelState.IsValid)
            {
                var msg = db.MinorTmerPeriods.Find(meta.Id);
                msg.GroupCount = meta.GroupCount;
                msg.Distribution = meta.Distribution;
                msg.CleanDistribution();
                
                db.SaveChanges();
                return RedirectToAction("Index", new {focus = meta.Id });
            }

            meta = db.MinorTmerPeriods.Find(meta.Id);
            if (meta == null)
            {
                return NotFound();
            }

            return View(meta);
        }

        public ActionResult RecalculateMetas()
        {
            RecalcMetaSubgroups();
            return RedirectToAction("Index");
        }

        private void RecalcMetaSubgroups()
        {
            //foreach (var group in groups)
            //    foreach (var plan in plans)
            //    {
            //        var loads =
            //            db.Apploads.Where(
            //                a =>
            //                    (a.eduDiscipline == plan.disciplineUUID || a.eduDiscipline == plan.additionalUUID) &&
            //                    a.year == program.Year && a.grp == @group.Id && !a.removed && a.status == ApploadStatus.Approved)
            //                .GroupBy(a => new { a.term, a.action })
            //                .ToList();

            //        foreach (var load in loads)
            //        {
            //            int count = 0;
            //            switch (load.Key.action)
            //            {
            //                case "tlekc":
            //                    count = load.First().lectureFlows ?? 1;
            //                    break;
            //                case "tprak":
            //                    count = load.First().practiceFlows ?? 0;
            //                    break;
            //                case "tlab":
            //                    count = load.First().labSubgroups;
            //                    break;
            //                default:
            //                    count = 0;
            //                    break;
            //            }

            //            var meta = new MetaSubgroup
            //            {
            //                kmer = load.Key.action,
            //                disciplineUUID = plan.disciplineUUID,
            //                additionalUUID = plan.additionalUUID,
            //                catalogDisciplineUuid = plan.catalogDisciplineUUID,
            //                programId = programId,
            //                groupId = @group.Id,
            //                Term = load.Key.term,
            //                moduleId = plan.moduleUUID,
            //                Count = count
            //            };
            //            if (
            //                !db.MetaSubgroups.Any(
            //                    m =>
            //                        m.kmer == meta.kmer &&
            //                        m.disciplineUUID == meta.disciplineUUID &&
            //                        m.programId == meta.programId &&
            //                        m.groupId == meta.groupId &&
            //                        m.Term == meta.Term &&
            //                        m.moduleId == meta.moduleId))
            //                db.MetaSubgroups.Add(meta);
            //        }
            //    }
            //db.SaveChanges();
        }

    }
}