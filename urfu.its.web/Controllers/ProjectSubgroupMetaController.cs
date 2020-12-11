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
    [Authorize(Roles = ItsRoles.ProjectView)]
    public class ProjectSubgroupMetaController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        
        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter, int? focus)
        {
            ViewBag.Focus = focus;
            var competitionGroup = db.ProjectCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("projectGroup not found");
            var projectsForUser = db.ProjectPropertiesForUser(User, includePairedModule: true)
                .Where(p => p.ProjectCompetitionGroupId == competitionGroupId).Select(p => p.ProjectId).ToList();
            var metas = db.ProjectTmerPeriods
                .Where(
                    m =>
                        (m.Period.Year == competitionGroup.Year) &&
                        (m.Period.SemesterId == competitionGroup.SemesterId) 
                        && projectsForUser.Contains(m.Period.ProjectId)
                        );
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var data = metas.Select(m => new
                {
                    m.Id,
                    m.Period.Project.Module.Level,
                    m.Period.Project.Module.title,
                    discipline = m.Tmer.Discipline.Discipline.title,
                    tmer = m.Tmer.Tmer.rmer,
                    count =
                    db.ProjectSubgroupCounts.FirstOrDefault(
                            _ => (_.ProjectDisciplineTmerPeriodId == m.Id) && (_.CompetitionGroupId == competitionGroupId))
                        .GroupCount,
                    admission =
                    db.ProjectAdmissions.Count(
                        a =>
                            (a.Status == AdmissionStatus.Admitted) &&
                            (a.ProjectCompetitionGroupId == competitionGroupId) &&
                            (a.ProjectId == m.Period.ProjectId))
                }).AsQueryable();

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
                var projectSubgroupCount =
                    db.ProjectSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.CompetitionGroupId == competitionGroupId) &&
                            (_.ProjectDisciplineTmerPeriodId == meta.Id));
                if (projectSubgroupCount == null)
                {
                    var newSubgroupCount = new ProjectSubgroupCount
                    {
                        CompetitionGroupId = competitionGroupId,
                        ProjectDisciplineTmerPeriodId = meta.Id,
                        GroupCount = 0
                    };
                    db.ProjectSubgroupCounts.Add(newSubgroupCount);
                }
            }
            db.SaveChanges();
            ViewBag.CanEdit = db.CanEditProjectCompetitionGroup(User, competitionGroupId);
            return View(competitionGroup);
        }

        public ActionResult Edit(int? id, int competitionGroupId)
        {
            ViewBag.competitionGroupId = competitionGroupId;
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var meta = db.ProjectTmerPeriods.Find(id);
            if (meta == null)
                return NotFound();

            var expectedChildCount =
                db.ProjectSubgroupCounts.Where(
                        _ =>
                            (_.ProjectDisciplineTmerPeriodId == meta.Id) &&
                            (_.CompetitionGroupId == competitionGroupId))
                    .Select(_ => _.GroupCount)
                    .FirstOrDefault();

            ViewBag.ExpectedChildCount = expectedChildCount;
            ViewBag.competitionGroupId = competitionGroupId;

            // TODO для каких ролей??? 
            ViewBag.CanEdit = User.IsInRole(ItsRoles.ProjectManager) || User.IsInRole(ItsRoles.ProjectROP) || User.IsInRole(ItsRoles.ProjectCurator);

            return View(meta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectDisciplineTmerPeriod meta, int groupCount, int competitionGroupId)
        {
            ViewBag.competitionGroupId = competitionGroupId;
            if (ModelState.IsValid)
            {
                var msg = db.ProjectTmerPeriods.Find(meta.Id);
                var subgroupCount =
                    db.ProjectSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.ProjectDisciplineTmerPeriodId == meta.Id) &&
                            (_.CompetitionGroupId == competitionGroupId));
                if (subgroupCount != null) subgroupCount.GroupCount = groupCount;

                msg.Distribution = meta.Distribution;
                msg.CleanDistribution();

                db.SaveChanges();
                return RedirectToAction("Index", new { focus = meta.Id, competitionGroupId });
            }

            meta = db.ProjectTmerPeriods.Find(meta.Id);
            if (meta == null)
                return NotFound();

            return View(meta);
        }
    }
}