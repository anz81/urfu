using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList.Core;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.MUPManager)]
    public class MUPSubgroupMetaController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter, int? focus)
        {
            ViewBag.Focus = focus;
            var competitionGroup = db.MUPCompetitionGroups.FirstOrDefault(c => c.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("competitionGroup not found");
            var sections = competitionGroup.MUPProperties.Select(p => p.MUPId).AsQueryable();
            var metas = db.MUPDisciplineTmerPeriods
                .Where(
                    m =>
                        (m.Period.Year == competitionGroup.Year) &&
                        (m.Period.SemesterId == competitionGroup.SemesterId) &&
                        (m.Period.Course ==null || m.Period.Course == competitionGroup.StudentCourse || competitionGroup.StudentCourse == 0) &&
                        sections.Any(s => s == m.Period.MUPId));
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var data = metas.Select(m => new
                {
                    m.Id,
                    m.Period.MUP.Module.title,
                    discipline = m.Tmer.Discipline.Discipline.title,
                    tmer = m.Tmer.Tmer.rmer,
                    count =
                    db.MUPSubgroupCounts.FirstOrDefault(
                        c =>
                            (c.MUPDisciplineTmerPeriodId == m.Id) &&
                            (c.CompetitionGroupId == competitionGroupId)).GroupCount,
                    admission =
                    db.MUPAdmissions.Count(
                        a =>
                            (a.Status == AdmissionStatus.Admitted) &&
                            (a.MUPCompetitionGroupId == competitionGroupId) &&
                            (a.MUPId == m.Period.MUPId))
                });

                var sortRules = SortRules.Deserialize(sort);
                data = data.OrderByThenBy(sortRules.FirstOrDefault(), m => m.title);

                data = data.Where(FilterRules.Deserialize(filter));

                var paginated = data.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = data.Count()
                });
            }
            foreach (var meta in metas.ToList())
            {
                var MUPSubgroupCount =
                    db.MUPSubgroupCounts.FirstOrDefault(
                        c =>
                            (c.CompetitionGroupId == competitionGroupId) &&
                            (c.MUPDisciplineTmerPeriodId == meta.Id));
                if (MUPSubgroupCount == null)
                {
                    var newSubgroupCount = new MUPSubgroupCount
                    {
                        CompetitionGroupId = competitionGroupId,
                        MUPDisciplineTmerPeriodId = meta.Id,
                        GroupCount = 0
                    };
                    db.MUPSubgroupCounts.Add(newSubgroupCount);
                }
            }
            db.SaveChanges();
            return View(competitionGroup);
        }

        public ActionResult Edit(int? id, int competitionGroupId)
        {
            ViewBag.competitionGroupId = competitionGroupId;
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var meta = db.MUPDisciplineTmerPeriods.Find(id);
            if (meta == null)
                return NotFound();
            var expectedChildCount =
                db.MUPSubgroupCounts.Where(
                        c =>
                            (c.MUPDisciplineTmerPeriodId == meta.Id) &&
                            (c.CompetitionGroupId == competitionGroupId))
                    .Select(c => c.GroupCount)
                    .FirstOrDefault();

            ViewBag.ExpectedChildCount = expectedChildCount;

            ViewBag.competitionGroupId = competitionGroupId;
            return View(meta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MUPDisciplineTmerPeriod meta, int groupCount, int competitionGroupId)
        {
            ViewBag.competitionGroupId = competitionGroupId;
            if (ModelState.IsValid)
            {
                var msg = db.MUPDisciplineTmerPeriods.Find(meta.Id);
                var subgroupCount =
                    db.MUPSubgroupCounts.FirstOrDefault(
                        c =>
                            (c.MUPDisciplineTmerPeriodId == meta.Id) &&
                            (c.CompetitionGroupId == competitionGroupId));
                if (subgroupCount != null) subgroupCount.GroupCount = groupCount;

                msg.Distribution = meta.Distribution;
                msg.CleanDistribution();

                db.SaveChanges();
                return RedirectToAction("Index", new {focus = meta.Id, competitionGroupId});
            }

            meta = db.MUPDisciplineTmerPeriods.Find(meta.Id);
            if (meta == null)
                return NotFound();

            return View(meta);
        }
    }
}