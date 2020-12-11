using Ext.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using PagedList.Core;
using Ext.Utilities.Linq;
using Urfu.Its.Web.Model.Models.Practice;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using Urfu.Its.Web.Excel;
using Urfu.Its.Common;
//using Microsoft.Ajax.Utilities;
using System.Net;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity.Migrations;
using Urfu.Its.Web.Model.Models.ModulesVM;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.MUPManager)]
    public class MUPCompetitionGroupsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var filterRules = FilterRules.Deserialize(filter);

                var filterCourse = filterRules?.Find(f => f.Property == "StudentCourse");

                if (filterRules != null)
                {
                    filterRules.Remove(filterCourse);
                }
                int course = -1;
                bool isFilterCourse = int.TryParse(filterCourse?.Value, out course);

                var competitionGroups = db.MUPCompetitionGroups.Where(c => isFilterCourse && (c.StudentCourse == course || course == 0) || !isFilterCourse).Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.ShortName,
                    c.StudentCourse,
                    c.Year,
                    c.SemesterId,
                    Semester = c.Semester.Name,
                    SelectedGroupsCount = c.Groups.Count,
                    HasAdmissions =
                    db.MUPAdmissions.Any(
                        a => (a.MUPCompetitionGroupId == c.Id) && (a.Status == AdmissionStatus.Admitted)),
                    AdmissionCount = db.MUPAdmissions.Where(a => (a.MUPCompetitionGroupId == c.Id) && (a.Status == AdmissionStatus.Admitted)).GroupBy(s=>s.studentId).Count()
                });


                SortRules sortRules = SortRules.Deserialize(sort);
                competitionGroups = competitionGroups.OrderBy(sortRules.FirstOrDefault());

                competitionGroups = competitionGroups.Where(filterRules);

                return Json(
                    new
                    {
                        data = competitionGroups
                    },
                    new JsonSerializerSettings()
                );
            }
            ViewBag.CanEdit = User.IsInRole(ItsRoles.MUPManager);
            ViewBag.CanCreateSubgroups = User.IsInRole(ItsRoles.Admin);
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrUpdateCompetitionGroup(MUPCompetitionGroup competitionGroup)
        {
            if (ModelState.IsValid)
            {
                var competitionGroupFromBase =
                    db.MUPCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroup.Id);
                if (competitionGroupFromBase == null)
                {
                    db.MUPCompetitionGroups.Add(competitionGroup);
                }
                else
                {
                    var hasAdmissions =
                        db.MUPAdmissions.Any(
                            a =>
                                (a.MUPCompetitionGroupId == competitionGroup.Id) &&
                                (a.Status == AdmissionStatus.Admitted));
                    if (hasAdmissions)
                    {
                        competitionGroup.StudentCourse = competitionGroupFromBase.StudentCourse;
                    }
                    else if (competitionGroup.StudentCourse != competitionGroupFromBase.StudentCourse)
                    {
                        var admissions =
                            db.MUPAdmissions.Where(
                                a => a.MUPCompetitionGroupId == competitionGroup.Id);
                        db.MUPAdmissions.RemoveRange(admissions);
                        competitionGroupFromBase.Groups.Clear();
                    }
                    try
                    {
                        db.MUPCompetitionGroups.Update(competitionGroup);
                    }
                    catch
                    { db.MUPCompetitionGroups.Add(competitionGroup); }
                    
                }
                db.SaveChanges();
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            return NotFound("Validation error");
        }

        public ActionResult CompetitionGroupContentsTree(int? competitionGroupId, int? course, string filter)
        {
            if (competitionGroupId == null) return new StatusCodeResult(StatusCodes.Status404NotFound);

            var competitionGroup = db.MUPCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);

            var divisions = db.Divisions.ToList();
            var directions = db.DirectionsForUser(User).Select(d => d.uid).ToList();
            var profiles = db.Profiles.Where(p => directions.Contains(p.DIRECTION_ID) && !p.remove).Select(p => p.ID);

            List<GroupRow> groups =
                db.GroupsHistories.Where(_ => _.YearHistory == competitionGroup.Year
                            && (_.Course == course || course == null || course == 0)
                            && (_.Qual != "Аспирант") && (_.FamType == "Очная")
                            && profiles.Contains(_.ProfileId)
                            ).Where(FilterRules.Deserialize(filter))
                    .Select(_ => new GroupRow()
                    {
                        Id = _.GroupId,
                        Name = _.Name,
                        ChairId = _.ChairId
                    }).OrderBy(g => g.Name).ToList();
            var model = new CompetitionGroupContentsViewModel(divisions, groups, competitionGroup);
            return JsonNet(model.Roots);
        }

        [HttpPost]
        public ActionResult UpdateCompetitionGroupContents(int competitionGroupId, List<string> groupsIds, List<string> deselectedGroups, bool isFiltered)
        {
            var competitionGroup = db.MUPCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            IEnumerable<object> badGroups = Enumerable.Empty<object>();

            if (competitionGroup != null)
            {
                IEnumerable<Group> groups = Enumerable.Empty<Group>();

                if (groupsIds != null)
                {
                    groups =
                        db.Groups.Where(
                                _ =>
                                    groupsIds.Contains(_.Id) &&
                                    (!db.MUPCompetitionGroups.Any(
                                        c =>
                                            c.Id != competitionGroupId && c.Year == competitionGroup.Year &&
                                            c.SemesterId == competitionGroup.SemesterId && c.Groups.Any(g => g.Id == _.Id))))
                            .ToList();
                    var badGroupIds = groupsIds.Where(_ => !groups.Any(g => g.Id == _));
                    badGroups = db.Groups.Where(_ => badGroupIds.Contains(_.Id)).Select(_ =>
                        new
                        {
                            competitionGroupName = db.MUPCompetitionGroups.FirstOrDefault(
                                c =>
                                    competitionGroup.Year == c.Year && c.SemesterId == competitionGroup.SemesterId &&
                                    c.Groups.Any(g => g.Id == _.Id)).Name,
                            group = _.Name,
                            groupId = _.Id
                        }
                    ).ToList();

                }
                if (!isFiltered)
                {
                    if (groupsIds != null)
                    {
                        var toDeleteIds = competitionGroup.Groups.Select(_ => _.Id).Except(groupsIds);
                        LogCompetitionGroupChanges("удалена {0} {1} из {2}", toDeleteIds, competitionGroup);

                        RemoveAdmissionsForGroup(competitionGroupId, toDeleteIds);
                    }
                    competitionGroup.Groups.Clear();
                }
                else if (deselectedGroups != null)
                {
                    var toDelete = competitionGroup.Groups.Where(_ => deselectedGroups.Contains(_.Id)).ToList();
                    var toDeleteIds = toDelete.Select(_ => _.Id);
                    RemoveAdmissionsForGroup(competitionGroupId, toDeleteIds);
                    LogCompetitionGroupChanges("удалена {0} {1} из {2} Проектной группы", toDeleteIds, competitionGroup);
                    foreach (var @group in toDelete)
                    {
                        competitionGroup.Groups.Remove(@group);

                    }
                }

                foreach (var group in groups)
                {
                    competitionGroup.Groups.Add(group);
                    Logger.Info("добавлен {0} {1} в {2} проектную группу", group.Name, group.Id, competitionGroup.Name);

                }
                db.SaveChanges();
                return Json(badGroups);
            }
            return NotFound("competition group not found");
        }
        private void LogCompetitionGroupChanges(string msg, IEnumerable<string> groupsIds, MUPCompetitionGroup competitionGroup)
        {
            var groups = db.Groups.Where(_ => groupsIds.Contains(_.Id)).ToList();
            foreach (var @group in groups)
            {
                Logger.Info(msg, @group.Name, @group.Name, @group.Id, competitionGroup.Name);
            }
        }
        private void RemoveAdmissionsForGroup(int competitionGroupId, IEnumerable<string> toDeleteIds)
        {
            var admissions =
                db.MUPAdmissions.Where(
                    a =>
                        a.MUPCompetitionGroupId == competitionGroupId &&
                        toDeleteIds.Contains(a.Student.GroupId)).ToList();
            var subgroupAdmissions =
                db.MUPSubgroupMemberships.Where(
                    m =>
                        m.Subgroup.Meta.CompetitionGroupId == competitionGroupId &&
                        toDeleteIds.Contains(m.Student.GroupId)).ToList();
            db.MUPAdmissions.RemoveRange(admissions);
            db.MUPSubgroupMemberships.RemoveRange(subgroupAdmissions);
            db.SaveChanges();
        }

        public ActionResult DeleteCompetitionGroup(int id)
        {
            db.MUPProperties.RemoveRange(
                db.MUPProperties.Where(x => (x.MUPCompetitionGroupId == id) && x.Limit == 0 && !x.Teachers.Any()));
            db.MUPSubgroups.RemoveRange(
                db.MUPSubgroups.Where(_ => _.Meta.CompetitionGroupId == id));
            db.MUPSubgroupCounts.RemoveRange(
                db.MUPSubgroupCounts.Where(_ => _.CompetitionGroupId == id));

            var competitionGroup = db.MUPCompetitionGroups.FirstOrDefault(_ => _.Id == id);
            if (competitionGroup == null)
                return NotFound();
            else
                db.MUPCompetitionGroups.Remove(competitionGroup);

            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);//, $"{nameof(competitionGroup)} deleted");
        }

        public ActionResult Properties(int competitionGroupId, string filter)
        {
            var competitionGroup = db.MUPCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("CompetitionGroup not found");

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                //var MUPsForUser = db.MUPsForUser(User).Select(p => p.ModuleId).ToList();

                var filterRules = FilterRules.Deserialize(filter);

                var filterTitle = filterRules?.Find(f => f.Property == "title");
                if (filterRules != null)
                {
                    filterRules.Remove(filterTitle);
                }
                string title = filterTitle?.Value?.ToLower() ?? "";

                var properties = db.MUPProperties.Where(
                        p => p.MUPCompetitionGroupId == competitionGroupId && 
                            p.MUP.Periods.Any(pr => (pr.Year == competitionGroup.Year) && (pr.SemesterId == competitionGroup.SemesterId)
                            && (pr.Course == null || pr.Course == competitionGroup.StudentCourse || competitionGroup.StudentCourse == 0)))
                    .Select(p => new
                    {
                        property = p,
                        period = p.MUP.Periods.FirstOrDefault(
                            pr => (pr.Year == competitionGroup.Year || competitionGroup.Year == 0) && (pr.SemesterId == competitionGroup.SemesterId))
                    })
                    .ToList()
                    .Select(p => new
                    {
                        p.property.Id,
                        p.property.MUP.Module.title,
                        teachers = string.Join(", ", p.property.Teachers.Select(u => u.initials).ToList()),
                        coefficient =db.RatingCoefficients.SingleOrDefault(c=>c.ModuleId==p.property.MUPId && c.Level ==p.property.MUP.Module.Level && c.Year == p.period.Year && c.ModuleType == (int)ModuleTypeParam.MUP)?.Coefficient
                    })
                    .Where(p => p.title.ToLower().Contains(title))
                    .AsQueryable().Where(filterRules).ToList();

                return JsonNet(new { data = properties });
            }

            var mups =
                db.MUPs.Where(
                        m => !m.Removed &&
                            m.Periods.Any(
                                p => (p.Year == competitionGroup.Year) && (p.SemesterId == competitionGroup.SemesterId)))
                    .ToList();

            foreach (var mup in mups)
            {
                if (competitionGroup.MUPProperties.Any(
                        p =>
                            (p.MUPId == mup.ModuleId) &&
                            (p.MUPCompetitionGroupId == competitionGroupId)))
                    continue;

                competitionGroup.MUPProperties.Add(new MUPProperty
                {
                    MUPId = mup.ModuleId,
                    MUPCompetitionGroupId = competitionGroupId,

                });
            }
            db.SaveChanges();

            ViewBag.CanEdit = User.IsInRole(ItsRoles.MUPManager);
            
            return View(competitionGroup);
        }

        public ActionResult PropertyTeachers(int? page, string sort, string filter, int? limit, int propertyId)
        {
            var property = db.MUPProperties.FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("property not found");

            var users = db.Teachers.Select(t => new
            {
                selected = t.MUPProperties.Any(_ => _.Id == propertyId),
                userId = t.pkey,
                FirstName = t.firstName,
                Patronymic = t.middleName,
                LastName = t.lastName
            });

            SortRules sortRules = SortRules.Deserialize(sort);
            users = users.OrderBy(sortRules.FirstOrDefault(), m => m.LastName);

            users = users.Where(FilterRules.Deserialize(filter));

            var paginated = users.ToPagedList(page ?? 1, limit ?? 25);

            return JsonNet(new
            {
                data = paginated,
                total = users.Count()
            });
        }

        [HttpPost]
        public ActionResult UpdateMUPPropertyTeachers(int propertyId, string teacherRows)
        {
            var property = db.MUPProperties.FirstOrDefault(_ => _.Id == propertyId);
            if (property == null)
                return NotFound("Property not found");
            var teachers =
                (List<ModulePropertyRow<string>>)
                JsonConvert.DeserializeObject(teacherRows, typeof(List<ModulePropertyRow<string>>));
            foreach (var teacherRow in teachers)
            {
                var teacher = db.Teachers.FirstOrDefault(_ => _.pkey == teacherRow.id);
                if (teacher != null)
                    if (property.Teachers.Any(_ => _.pkey == teacherRow.id))
                    {
                        if (!teacherRow.selected)
                            property.Teachers.Remove(teacher);
                    }
                    else if (teacherRow.selected)
                    {
                        property.Teachers.Add(teacher);
                    }
            }
            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult CreateSubgroups(int id)
        {
            MUPHelper helper = new MUPHelper();
            helper.FillMUPTables();
            helper.FillMUPSubgroupTables(id);
            return new StatusCodeResult(StatusCodes.Status200OK);
        }
    }
}