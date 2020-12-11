using System.Linq;
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
    [Authorize(Roles = ItsRoles.ForeignLanguageManager)]
    public class ForeignLanguageSubgroupMetaController : BaseController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: ForeignLanguageAdmission
        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter, int? focus)
        {
            ViewBag.Focus = focus;
            var competitionGroup = _db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("competitionGroup not found");
            var sections = competitionGroup.ForeignLanguageProperties.Select(_ => _.ForeignLanguageId).AsQueryable();
            var metas = _db.ForeignLanguageTmerPeriods
                .Where(
                    m =>
                        (m.Period.Year == competitionGroup.Year) &&
                        (m.Period.SemesterId == competitionGroup.SemesterId) &&
                        (m.Period.Course ==null || m.Period.Course == competitionGroup.StudentCourse) &&
                        sections.Any(_ => _ == m.Period.ForeignLanguageId));
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var data = metas.Select(m => new
                {
                    m.Id,
                    m.Period.ForeignLanguage.Module.title,
                    discipline = m.Tmer.Discipline.Discipline.title,
                    tmer = m.Tmer.Tmer.rmer,
                    count =
                    _db.ForeignLanguageSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.ForeignLanguageDisciplineTmerPeriodId == m.Id) &&
                            (_.CompetitionGroupId == competitionGroupId)).GroupCount,
                    admission =
                    _db.ForeignLanguageAdmissions.Count(
                        a =>
                            (a.Status == AdmissionStatus.Admitted) &&
                            (a.ForeignLanguageCompetitionGroupId == competitionGroupId) &&
                            (a.ForeignLanguageId == m.Period.ForeignLanguageId))
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
                var foreignLanguageSubgroupCount =
                    _db.ForeignLanguageSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.CompetitionGroupId == competitionGroupId) &&
                            (_.ForeignLanguageDisciplineTmerPeriodId == meta.Id));
                if (foreignLanguageSubgroupCount == null)
                {
                    var newSubgroupCount = new ForeignLanguageSubgroupCount
                    {
                        CompetitionGroupId = competitionGroupId,
                        ForeignLanguageDisciplineTmerPeriodId = meta.Id,
                        GroupCount = 0
                    };
                    _db.ForeignLanguageSubgroupCounts.Add(newSubgroupCount);
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

            var meta = _db.ForeignLanguageTmerPeriods.Find(id);
            if (meta == null)
                return NotFound();
            var expectedChildCount =
                _db.ForeignLanguageSubgroupCounts.Where(
                        _ =>
                            (_.ForeignLanguageDisciplineTmerPeriodId == meta.Id) &&
                            (_.CompetitionGroupId == competitionGroupId))
                    .Select(_ => _.GroupCount)
                    .FirstOrDefault();

            ViewBag.ExpectedChildCount = expectedChildCount;

            ViewBag.competitionGroupId = competitionGroupId;
            return View(meta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ForeignLanguageDisciplineTmerPeriod meta, int groupCount, int competitionGroupId)
        {
            ViewBag.competitionGroupId = competitionGroupId;
            if (ModelState.IsValid)
            {
                var msg = _db.ForeignLanguageTmerPeriods.Find(meta.Id);
                var subgroupCount =
                    _db.ForeignLanguageSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.ForeignLanguageDisciplineTmerPeriodId == meta.Id) &&
                            (_.CompetitionGroupId == competitionGroupId));
                if (subgroupCount != null) subgroupCount.GroupCount = groupCount;

                msg.Distribution = meta.Distribution;
                msg.CleanDistribution();

                _db.SaveChanges();
                return RedirectToAction("Index", new {focus = meta.Id, competitionGroupId});
            }

            meta = _db.ForeignLanguageTmerPeriods.Find(meta.Id);
            if (meta == null)
                return NotFound();

            return View(meta);
        }
    }
}