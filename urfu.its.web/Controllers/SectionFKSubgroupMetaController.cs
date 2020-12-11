using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList.Core;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.SectionFKManager)]
    public class SectionFKSubgroupMetaController : BaseController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: SectionFKAdmission
        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter, int? focus)
        {
            ViewBag.Focus = focus;
            var competitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("competitionGroup not found");
            var sections = competitionGroup.SectionFkProperties.Select(_ => _.SectionFKId).AsQueryable();
            var metas = _db.SectionFKTmerPeriods
                .Where(
                    m =>
                        (m.Period.Year == competitionGroup.Year) &&
                        (m.Period.SemesterId == competitionGroup.SemesterId) && 
                        (m.Period.Course == competitionGroup.StudentCourse || m.Period.Course == null) &&
                        sections.Any(_ => _ == m.Period.SectionFKId));
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var data = metas.Select(m => new
                {
                    m.Id,
                    m.Period.SectionFK.Module.title,
                    discipline = m.Tmer.Discipline.Discipline.title,
                    tmer = m.Tmer.Tmer.rmer,
                    count =
                    _db.SectionFKSubgroupCounts.FirstOrDefault(
                            _ => (_.SectionFKDisciplineTmerPeriodId == m.Id) && (_.CompetitionGroupId == competitionGroupId))
                        .GroupCount,
                    admission =
                    _db.SectionFKAdmissions.Count(
                        a =>
                            (a.Status == AdmissionStatus.Admitted) &&
                            (a.SectionFKCompetitionGroupId == competitionGroupId) &&
                            (a.SectionFKId == m.Period.SectionFKId))
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
                var sectionFkSubgroupCount =
                    _db.SectionFKSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.CompetitionGroupId == competitionGroupId) &&
                            (_.SectionFKDisciplineTmerPeriodId == meta.Id));
                if (sectionFkSubgroupCount == null)
                {
                    var newSubgroupCount = new SectionFKSubgroupCount
                    {
                        CompetitionGroupId = competitionGroupId,
                        SectionFKDisciplineTmerPeriodId = meta.Id,
                        GroupCount = 0
                    };
                    _db.SectionFKSubgroupCounts.Add(newSubgroupCount);
                }
            }
            _db.SaveChanges();
            return View(competitionGroup);
        }

        public ActionResult Edit(int? id, int competitionGroupId)
        {
            ViewBag.competitionGroupId = competitionGroupId;
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var meta = _db.SectionFKTmerPeriods.Find(id);
            if (meta == null)
                return NotFound();

            var expectedChildCount =
                _db.SectionFKSubgroupCounts.Where(
                        _ =>
                            (_.SectionFKDisciplineTmerPeriodId == meta.Id) &&
                            (_.CompetitionGroupId == competitionGroupId))
                    .Select(_ => _.GroupCount)
                    .FirstOrDefault();

            ViewBag.ExpectedChildCount = expectedChildCount;


            ViewBag.competitionGroupId = competitionGroupId;
            return View(meta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SectionFKDisciplineTmerPeriod meta, int groupCount, int competitionGroupId)
        {
            ViewBag.competitionGroupId = competitionGroupId;
            if (ModelState.IsValid)
            {
                var msg = _db.SectionFKTmerPeriods.Find(meta.Id);
                var subgroupCount =
                    _db.SectionFKSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.SectionFKDisciplineTmerPeriodId == meta.Id) &&
                            (_.CompetitionGroupId == competitionGroupId));
                if (subgroupCount != null) subgroupCount.GroupCount = groupCount;

                msg.Distribution = meta.Distribution;
                msg.CleanDistribution();

                _db.SaveChanges();
                return RedirectToAction("Index", new {focus = meta.Id, competitionGroupId});
            }

            meta = _db.SectionFKTmerPeriods.Find(meta.Id);
            if (meta == null)
                return NotFound();

            return View(meta);
        }
    }
}