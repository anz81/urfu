using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
//using Microsoft.Ajax.Utilities;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.SectionFKManager)]
    public class SectionFKSubgroupController : BaseController
    {
        private readonly ApplicationDbContext _db;

        public SectionFKSubgroupController()
        {
            _db = new ApplicationDbContext();
        }

        public SectionFKSubgroupController(ApplicationDbContext db)
        {
            _db = db;
        }
        // GET: SectionFKSubgroup
        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter,
            string message = null, int? focus = null)
        {
            ViewBag.Message = message;
            ViewBag.Focus = focus;
            var competitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("competitionGroup not found");

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var subgroups = GetSectionSubgroups(sort, filter, competitionGroupId);

                var paginated = subgroups;
                return JsonNet(new
                {
                    data = paginated,
                    total = subgroups.Count()
                });
            }

            return View(competitionGroup);
        }

        public ActionResult StudentsCount(string filter, int competitionGroupId)
        {
            ((IObjectContextAdapter)_db).ObjectContext.CommandTimeout = 1200000;
            var subgroups =
                GetSectionSubgroups(null, filter, competitionGroupId)
                    .ToList()
                    .Select(v => v.GetPropertyValue<int>("Id"))
                    .SelectMany(v =>
                            _db.SectionFKSubgroupMemberships.Where(
                                sm =>
                                    (sm.SubgroupId == v) || (sm.Subgroup.ParentId == v) ||
                                    (sm.Subgroup.Parent.ParentId == v))
                    ).Distinct().Count(); //GroupBy(_=>_.studentId).Count();
            return JsonNet(subgroups);
        }

        private IQueryable<object> GetSectionSubgroups(string sort, string filter, int competitionGroupId)
        {
            ((IObjectContextAdapter)_db).ObjectContext.CommandTimeout = 1200000;
            var subgroups = _db.SectionFKSubgroups
                .Where(s => s.Meta.CompetitionGroupId == competitionGroupId)
                .Select(v => new
                {
                    v.Id,
                    v.Name,
                    HasScores = v.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kgmer == 3,
                    moduleId = v.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId,
                    ModuleTitle = v.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.title,
                    Year = v.Meta.SectionFKDisciplineTmerPeriod.Period.Year.ToString(),
                    Semester = v.Meta.SectionFKDisciplineTmerPeriod.Period.Semester.Name.ToString(),
                    semesterId = v.Meta.SectionFKDisciplineTmerPeriod.Period.Semester.Id,
                    subgroupType = v.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    kgmer = v.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kgmer.ToString(),
                    v.Limit,
                    count =
                    _db.SectionFKSubgroupMemberships.Count(
                        sm =>
                            (sm.SubgroupId == v.Id) || (sm.Subgroup.ParentId == v.Id) ||
                            (sm.Subgroup.Parent.ParentId == v.Id)),
                    teacher = v.Teacher.initials
                });

            SortRules sortRules = SortRules.Deserialize(sort);
            subgroups = subgroups.OrderByThenBy(sortRules.FirstOrDefault(), v => v.ModuleTitle, v => v.Name, v => v.Year,
                v => v.Semester, v => v.kgmer, v => v.subgroupType);

            subgroups = subgroups.Where(FilterRules.Deserialize(filter));
            return subgroups;
        }

        public ActionResult Modules(int competitionGroupId)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var modules = _db.SectionFKSubgroups
                    .Where(
                        s =>
                            s.Meta.SectionFKDisciplineTmerPeriod.SectionFKSubgroupCounts.Any(
                                _ => _.CompetitionGroupId == competitionGroupId))
                    .Select(_ => new
                    {
                        Id = _.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.uuid,
                        Name = _.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.title,
                        _.Meta.SectionFKDisciplineTmerPeriod.Period.Year,
                        _.Meta.SectionFKDisciplineTmerPeriod.Period.SemesterId
                    }).Distinct().ToList();
                var json = Json(
                    new
                    {
                        data = modules
                    },
                    new JsonSerializerSettings()
                );

                return json;
            }
            return NotFound();
        }

        public ActionResult Create(int competitionGroupId)
        {
            var competitionGroup = _db.SectionFKCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("competitionGroup not found");
            
            if (CreateSubgroups(competitionGroupId, competitionGroup))
                return RedirectToAction("Index", new { competitionGroupId });
            return NotFound();
        }

        public bool CreateSubgroups(int competitionGroupId, SectionFKCompetitionGroup competitionGroup)
        {
            var metaSubgroups = _db.SectionFKTmerPeriods
                .Where(
                    x =>
                        (x.Period.Year == competitionGroup.Year) &&
                        (x.Period.SemesterId == competitionGroup.SemesterId) &&
                        (x.Period.Course == competitionGroup.StudentCourse || x.Period.Course== null) && 
                        x.SectionFKSubgroupCounts.Any(_ => _.CompetitionGroupId == competitionGroupId))
                .OrderByDescending(x => x.Tmer.Tmer.kmer == "tlekc")
                .ThenBy(x => x.Tmer.Tmer.kmer == "tlab")
                .ThenBy(x => x.Tmer.Tmer.kmer == "tprak")
                .ToList();


            foreach (var meta in metaSubgroups)
            {
                var distribution = meta.ExtractDistribution();

                var busyNumbers =
                    new HashSet<int>(
                        _db.SectionFKSubgroups.Where(
                            s =>
                                (s.Meta.SectionFKDisciplineTmerPeriodId == meta.Id) &&
                                (s.Meta.CompetitionGroupId == competitionGroupId)).Select(s => s.InnerNumber));
                var metaCount =
                    meta.SectionFKSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.CompetitionGroupId == competitionGroupId) &&
                            (_.SectionFKDisciplineTmerPeriodId == meta.Id));

                var exists = busyNumbers.Count;
                if (metaCount == null)
                {
                    
                    return false;
                    
                }
                for (; exists < metaCount.GroupCount; exists++)
                {
                    var innerNumber = FindInnerNumber(busyNumbers);
                    busyNumbers.Add(innerNumber);

                    var sectionProperty =
                        _db.SectionFKProperties.FirstOrDefault(
                            _ =>
                                (_.SectionFKId == meta.Period.SectionFKId) &&
                                (_.SectionFKCompetitionGroupId == competitionGroupId));
                    if (sectionProperty == null)
                    {
                         return false;
                       
                    }
                    var grp = new SectionFKSubgroup
                    {
                        Name =
                            meta.Tmer.Discipline.SectionFK.Module.shortTitle + "\\" +
                            meta.Tmer.Discipline.Discipline.title + "\\" + meta.Tmer.Tmer.rmer + "\\" + innerNumber,
                        Limit = (int) Math.Ceiling(sectionProperty.Limit / (double) metaCount.GroupCount),
                        SubgroupCountId = metaCount.Id,
                        InnerNumber = innerNumber,
                        ExpectedChildCount = distribution[(innerNumber - 1) % distribution.Length]
                    };

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tlekc")
                        grp.Name = meta.Tmer.Discipline.SectionFK.Module.shortTitle + "\\" +
                                   meta.Tmer.Discipline.Discipline.title + "\\л" + innerNumber;

                    if (meta.Tmer.Tmer.kmer.ToLower() == "prz")
                        grp.Name = meta.Tmer.Discipline.SectionFK.Module.shortTitle + "\\" +
                                   meta.Tmer.Discipline.Discipline.title + "\\зачет" + innerNumber;

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tprak")
                    {
                        grp.Limit = 25;
                        var parents = _db.SectionFKSubgroups.Where(s =>
                                (s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer.ToLower() == "tlekc")
                                && (s.Meta.SectionFKDisciplineTmerPeriod.SectionFKPeriodId == meta.SectionFKPeriodId)
                                &&
                                (s.Meta.SectionFKDisciplineTmerPeriod.Tmer.SectionFKDisciplineId ==
                                 meta.Tmer.SectionFKDisciplineId))
                            .Select(p => new
                            {
                                p.Id,
                                p.Name,
                                p.ExpectedChildCount,
                                Count = _db.SectionFKSubgroups.Count(sx => sx.ParentId == p.Id)
                            })
                            .OrderBy(p => p.Name)
                            .ToList();

                        var parent = parents.Where(p => (p.ExpectedChildCount ?? 1) > p.Count).FirstOrDefault();
                        if (parent == null)
                            parent = parents.OrderBy(p => p.Count / p.ExpectedChildCount ?? 1).FirstOrDefault();

                        if (parent != null)
                        {
                            grp.ParentId = parent.Id;
                            grp.Name = parent.Name + "\\п" + innerNumber;
                        }
                        else
                        {
                            grp.Name = meta.Tmer.Discipline.SectionFK.Module.shortTitle + "\\" +
                                       meta.Tmer.Discipline.Discipline.title + "\\п" + innerNumber;
                        }
                    }

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tlab")
                    {
                        grp.Limit = 12;
                        var parent = _db.SectionFKSubgroups.Where(
                                             s =>
                                                 (s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer.ToLower() == "tprak")
                                                 &&
                                                 (s.Meta.SectionFKDisciplineTmerPeriod.SectionFKPeriodId ==
                                                  meta.SectionFKPeriodId)
                                                 &&
                                                 (s.Meta.SectionFKDisciplineTmerPeriod.Tmer.SectionFKDisciplineId ==
                                                  meta.Tmer.SectionFKDisciplineId)
                                         )
                                         .OrderBy(s => _db.SectionFKSubgroups.Count(sx => sx.ParentId == s.Id))
                                         .ThenBy(s => s.Name)
                                         .FirstOrDefault()
                                     ??
                                     _db.SectionFKSubgroups.Where(
                                             s =>
                                                 (s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer.ToLower() == "tlekc")
                                                 &&
                                                 (s.Meta.SectionFKDisciplineTmerPeriod.SectionFKPeriodId ==
                                                  meta.SectionFKPeriodId)
                                                 &&
                                                 (s.Meta.SectionFKDisciplineTmerPeriod.Tmer.SectionFKDisciplineId ==
                                                  meta.Tmer.SectionFKDisciplineId)
                                         )
                                         .OrderBy(s => _db.SectionFKSubgroups.Count(sx => sx.ParentId == s.Id))
                                         .ThenBy(s => s.Name)
                                         .FirstOrDefault();

                        if (parent != null)
                        {
                            grp.ParentId = parent.Id;
                            grp.Name = parent.Name + "\\лаб" + innerNumber;
                        }
                        else
                        {
                            grp.Name = meta.Tmer.Discipline.SectionFK.Module.shortTitle + "\\" +
                                       meta.Tmer.Discipline.Discipline.title + "\\лаб" + innerNumber;
                        }
                    }

                    _db.SectionFKSubgroups.Add(grp);
                    //db.Entry(grp).State == EntityState.Added;
                    _db.SaveChanges();
                    Logger.Info(
                        $"Создана подгруппа {grp.Id} {grp.Name} в конкурсной группе {metaCount.CompetitionGroupId} {metaCount.CompetitionGroup.Name} сокращ. название");
                }
            }
            return true;
        }

        private int FindInnerNumber(HashSet<int> busyNumbers)
        {
            int candidate = 1;
            while (busyNumbers.Contains(candidate))
                candidate++;
            return candidate;
        }


        public ActionResult FillGroupsWithStudents(int competitionGroupId)
        {
            var metas =
                _db.SectionFKSubgroupCounts.Include(x => x.SectionFKDisciplineTmerPeriod)
                    .Where(m => m.CompetitionGroupId == competitionGroupId)
                    .Where(m => m.Subgroups.Count() > 0)
                    .Where(m => !_db.SectionFKSubgroups.Any(s => s.Parent.SubgroupCountId == m.Id))
                    .ToList();
            foreach (var meta in metas)
            {
                var toAdmitt = _db.Students.Where(
                        s =>
                            _db.SectionFKAdmissions.Any(
                                ma =>
                                    (ma.studentId == s.Id) && (ma.SectionFKCompetitionGroupId == competitionGroupId) &&
                                    (ma.SectionFKId == meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId) &&
                                    (ma.Status == AdmissionStatus.Admitted))
                    )
                    .Where(s => !_db.SectionFKSubgroupMemberships.Any(m => m.Subgroup.SubgroupCountId == meta.Id && m.studentId == s.Id))
                    .Select(s => s.Id).ToList();

                var subgroups =
                    _db.SectionFKSubgroups.Where(s => s.SubgroupCountId == meta.Id)
                        .Select(s => new SubgroupShortInfo
                        {
                            Id = s.Id,
                            Admitted = s.Students.Count(),
                            InnerNumber = s.InnerNumber
                        })
                        .ToList();

                foreach (var studentId in toAdmitt)
                {
                    // найти нагрузки, на которые студент уже зачислен. 
                    var existedSubgroup = _db.SectionFKSubgroupMemberships.Include(s => s.Subgroup).FirstOrDefault(m => m.studentId == studentId
                            && m.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId == meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId
                            && m.Subgroup.Meta.CompetitionGroupId == meta.CompetitionGroupId);

                    // порядковый номер группы, на которую надо зачислить студента 
                    var subgroup = subgroups.OrderBy(x => x.Admitted).ThenBy(x => x.InnerNumber)
                        .Where(s => s.InnerNumber == existedSubgroup?.Subgroup?.InnerNumber || existedSubgroup == null)
                        .FirstOrDefault() 
                        ?? subgroups.OrderBy(x => x.Admitted).ThenBy(x => x.InnerNumber).First();

                    _db.SectionFKSubgroupMemberships.Add(new SectionFKSubgroupMembership
                    {
                        studentId = studentId,
                        SubgroupId = subgroup.Id
                    });
                    Logger.Info($"Студент {studentId} распределен в подгруппу {subgroup.Id} ФК");
                    subgroup.Admitted += 1;
                }

                _db.SaveChanges();
            }


            return RedirectToAction("Index", new { competitionGroupId });
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var subgroup = _db.SectionFKSubgroups.Include(s => s.Meta.SectionFKDisciplineTmerPeriod.Period).FirstOrDefault(s => s.Id == id);
            if (subgroup == null)
                return NotFound();

            var sectionFKId = subgroup.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId;

            var teachers =
                _db.SectionFKProperties.Where(
                        p =>
                            (p.SectionFKCompetitionGroupId == subgroup.Meta.CompetitionGroupId) &&
                            (p.SectionFKId == sectionFKId))
                    .SelectMany(p => p.Teachers)
                    .ToList();

            ViewBag.TeacherId = new SelectList(teachers, "pkey", "BigName", subgroup.TeacherId);

            return View(subgroup);
        }

        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult Edit(SectionFKSubgroup subgroup)
        {
            if (ModelState.IsValid)
            {
                var dbEntry = _db.SectionFKSubgroups.Find(subgroup.Id);
                dbEntry.Limit = subgroup.Limit;
                if (User.IsInRole(ItsRoles.Admin))
                    dbEntry.Name = subgroup.Name;
                dbEntry.TeacherId = subgroup.TeacherId;
                _db.SaveChanges();
                return RedirectToAction("Index",
                    new
                    {
                        programId = dbEntry.Id,
                        focus = subgroup.Id,
                        competitionGroupId = dbEntry.Meta.CompetitionGroupId
                    });
            }
            var teachers =
                _db.SectionFKProperties.Where(
                        p =>
                            (p.SectionFKCompetitionGroupId == subgroup.Meta.CompetitionGroupId) &&
                            (p.SectionFKId ==
                             _db.SectionFKSubgroups.FirstOrDefault(g => g.Id == subgroup.Id)
                                 .Meta.SectionFKDisciplineTmerPeriod.Period.SectionFKId))
                    .SelectMany(p => p.Teachers)
                    .ToList();

            ViewBag.TeacherId = new SelectList(teachers, "pkey", "BigName", subgroup.TeacherId);


            return View(subgroup);
        }

        public ActionResult Students(int id, int? page, int? limit, string sort, string filter)
        {
            var subgroup =
                _db.SectionFKSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.SectionFKDisciplineTmerPeriod)
                    .Include(x => x.Meta.SectionFKDisciplineTmerPeriod.Period)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == id);
            ViewBag.Subgroup = subgroup;
            return View(subgroup);
        }

        public ActionResult StudentsAjax(int id, bool hideStudents, int? page, int? limit, string sort, string filter)
        {
            var subgroup =
                _db.SectionFKSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.SectionFKDisciplineTmerPeriod)
                    .Include(x => x.Meta.SectionFKDisciplineTmerPeriod.Period)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == id);
            
            var students = GetSectionFKSubgroupsStudents(id, sort, filter, subgroup, hideStudents);

            var paginated = students; //.ToPagedList(page ?? 1, limit ?? 25);
            return JsonNet(new
            {
                data = paginated,
                total = students.Count()
            });
            
            return View(subgroup);
        }

        private IQueryable<object> GetSectionFKSubgroupsStudents(int id, string sort, string filter,
            SectionFKSubgroup subgroup, bool hideStudents = false)
        {
            var query = _db.Students
                .Where(s => _db.SectionFKAdmissions.Any(ma => (ma.studentId == s.Id)
                                                             && (ma.Status == AdmissionStatus.Admitted)
                                                             &&
                                                             (ma.SectionFKCompetitionGroupId ==
                                                              subgroup.Meta.CompetitionGroupId)
                                                             &&
                                                             (ma.SectionFKId ==
                                                              subgroup.Meta.SectionFKDisciplineTmerPeriod.Period
                                                                  .SectionFKId))
                    && (!hideStudents || hideStudents && (s.Status == "Активный" || s.Status == "Отп.с.посещ." || s.Status == "Отп.дород.послерод."))
                    )
                .Select(s => new
                {
                    Student = s,
                    s.Person,
                    GroupName = _db.GroupsHistories.FirstOrDefault(g=> g.GroupId == s.GroupId && g.YearHistory == subgroup.Meta.CompetitionGroup.Year ).Name,
                    SubgroupAdm = _db.SectionFKSubgroupMemberships.FirstOrDefault(m => (m.studentId == s.Id)
                                                                        &&
                                                                        //m.Subgroup.SubgroupCountId == subgroup.SubgroupCountId),
                                                                        ((m.SubgroupId == id) ||
                                                                         (m.Subgroup.ParentId == id) ||
                                                                         (m.Subgroup.Parent.ParentId == id))),
                    AnotherSubgroupGroup = _db.SectionFKSubgroupMemberships.FirstOrDefault(m => (m.studentId == s.Id)
                                                                                       && (
                                                                                           ((m.Subgroup.Meta.Id ==
                                                                                             subgroup.Meta.Id) &&
                                                                                            (m.Subgroup.Id !=
                                                                                             subgroup.Id))
                                                                                           //или та же нагрузка 
                                                                                           || (
                                                                                               (m.Subgroup.Meta.Id ==
                                                                                                subgroup.Meta.Id)
                                                                                               //или я на том же майноре но зачислен не в эту группу, а куда то на л, п, или пр
                                                                                               &&
                                                                                               ((m.Subgroup.Meta
                                                                                                     .SectionFKDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tlekc") ||
                                                                                                (m.Subgroup.Meta
                                                                                                     .SectionFKDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tprak") ||
                                                                                                (m.Subgroup.Meta
                                                                                                     .SectionFKDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "prz") ||
                                                                                                (m.Subgroup.Meta
                                                                                                     .SectionFKDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tlab"))
                                                                                               &&
                                                                                               ((subgroup.Meta
                                                                                                     .SectionFKDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tlekc") ||
                                                                                                (subgroup.Meta
                                                                                                     .SectionFKDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tprak") ||
                                                                                                (subgroup.Meta
                                                                                                     .SectionFKDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "prz") ||
                                                                                                (subgroup.Meta
                                                                                                     .SectionFKDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tlab"))
                                                                                           )
                                                                                       )
                                                                                       && (m.SubgroupId != id))
                    //и я точно в другой группе

                });

            var students = query.Select(r => new
            {
                r.Student.Id,
                GroupName = r.GroupName,
                r.Person.Surname,
                r.Person.Name,
                r.Person.PatronymicName,
                r.Student.Sportsman,
                r.Student.Status,
                Included = r.SubgroupAdm != null,
                AnotherGroup = r.AnotherSubgroupGroup.Subgroup.Name,
                Teacher = r.SubgroupAdm == null ?
                    r.AnotherSubgroupGroup.Subgroup.Teacher.initials :
                    r.SubgroupAdm.Subgroup.Teacher.initials

            });

            SortRules sortRules = SortRules.Deserialize(sort);
            students = students.OrderByThenBy(sortRules.FirstOrDefault(), v => v.Surname, v => v.Name,
                v => v.PatronymicName);

            students = students.Where(FilterRules.Deserialize(filter));
            return students;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult CheckMemberships(int subgroupId)
        {
            var subgroup =
                _db.SectionFKSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.SectionFKDisciplineTmerPeriod)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == subgroupId);
            if (subgroup == null) return NotFound("ForeignLanguageSubgroup not found");
            var similarsubgroups = FindSubgroups(subgroupId, subgroup);

            var msg = "";
            if (similarsubgroups.Count != 0)
            {
                msg = subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.TmerId == "tprak" ?
                   "зачету" : "практическим занятиям";
            }
            return Json(new { msg });
        }
        private List<SectionFKSubgroupMembership> FindSubgroups(int subgroupId, SectionFKSubgroup subgroup)
        {
            var subgroupStudents = GetSectionFKSubgroupsStudents(subgroupId, null, null, subgroup).ToList()
                .Where(s => s.GetPropertyValue<bool>("Included") == true);

            List<SectionFKSubgroupMembership> similarsubgroups = new List<SectionFKSubgroupMembership>();
            foreach (var s in subgroupStudents)
            {
                var studentId = s.GetPropertyValue<string>("Id");
                var ssubgroup=  _db.SectionFKSubgroupMemberships.Include(x => x.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer)
                    .FirstOrDefault(m => m.studentId == studentId
                    && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId
                    && m.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.TmerId != subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.TmerId);
                 if(ssubgroup!= null)               
                    similarsubgroups.Add(ssubgroup);

            }
            return similarsubgroups;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult StudentsMembership(bool include, int subgroupId, string filter, bool similarsubgroup)
        {
            var subgroup =
                _db.SectionFKSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.SectionFKDisciplineTmerPeriod)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == subgroupId);
            if (subgroup == null) return NotFound("SectionFKSubgroup not found");

            var subgroupStudents = GetSectionFKSubgroupsStudents(subgroupId, null, filter, subgroup).ToList().Where(s =>
            {
                var anotherGroup = s.GetPropertyValue<string>("AnotherGroup");
                return string.IsNullOrEmpty(anotherGroup);
            }).ToList();

            if (similarsubgroup)
            {
                FindSubgroups(subgroupId, subgroup).ForEach(s => { StudentMembershipInternal(include, s?.studentId, s.SubgroupId); });
            }
            subgroupStudents.ForEach(
                s => { StudentMembershipInternal(include, s.GetPropertyValue<string>("Id"), subgroupId); });

            var curStudentsCount = _db.SectionFKSubgroupMemberships.Count(m => m.SubgroupId == subgroupId);

            string msg = "";
            if ((subgroup.Limit < curStudentsCount) && include)
                msg = "Превышен лимит";

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return Json(new { msg, reload = false });
        }

        private void StudentMembershipInternal(bool include, string studentId, int subgroupId)
        {
            var subgroup = _db.SectionFKSubgroups.Include(s => s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(s => s.Id == subgroupId);

            var existsSubgroups =
                _db.SectionFKSubgroupMemberships.FirstOrDefault(
                    m => (m.studentId == studentId)
                        && m.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer == subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer
                        && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId);

            if (existsSubgroups == null) // если студент не содержится ни в одной подгруппе
            {
                if (include)
                {
                    AddMembership(new SectionFKSubgroupMembership
                    {
                        studentId = studentId,
                        SubgroupId = subgroupId
                    });
                }
            }
            else
            {
                if (existsSubgroups.SubgroupId != subgroupId) // если содержится в другой подгруппе
                {
                    if (include)
                    {
                        RemoveMembership(existsSubgroups);

                        AddMembership(new SectionFKSubgroupMembership
                        {
                            studentId = studentId,
                            SubgroupId = subgroupId
                        });
                    }
                }
                else // если содержится в текущей подгруппе
                {
                    if (!include)
                        RemoveMembership(existsSubgroups);
                }
            }
        }
        private void RemoveMembership(SectionFKSubgroupMembership membership)
        {
            _db.SectionFKSubgroupMemberships.Remove(membership);
            Logger.Info($"Студент {membership.studentId} удален из подгруппы {membership.SubgroupId} ФК");

            _db.SaveChanges();
        }

        private void AddMembership(SectionFKSubgroupMembership membership)
        {
            _db.SectionFKSubgroupMemberships.Add(membership);
            Logger.Info($"Студент {membership.studentId} добавлен в подгруппу {membership.SubgroupId} ФК");

            _db.SaveChanges();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult StudentMembership(bool include, string studentId, int subgroupId)
        {
            StudentMembershipInternal(include, studentId, subgroupId);

            var msg = "";
            var subgroup = _db.SectionFKSubgroups.FirstOrDefault(m => m.Id == subgroupId);
            if (!include)
            {
                var similarsubgroup = _db.SectionFKSubgroupMemberships.Include(s => s.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer)
                                        .FirstOrDefault(m => (m.studentId == studentId)
                                       && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId
                                       && m.SubgroupId != subgroup.Id);

                if (similarsubgroup != null)
                {
                    var rmer = similarsubgroup.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.TmerId == "tprak" ?
                          "практическим занятиям":"зачету" ;

                    return Json(new { rmer, subgroupId = similarsubgroup.SubgroupId });
                }

            }
            var curStudentsCount = _db.SectionFKSubgroupMemberships.Count(m => m.SubgroupId == subgroupId);
           
            if ((subgroup?.Limit < curStudentsCount) && include)
                msg = "Превышен лимит";
            return Json(new { msg });
        }

        //[Microsoft.AspNetCore.Mvc.HttpPost]
        //public ActionResult RemoveMembership(string studentId, int subgroupId)
        //{
        //    var subgroup = _db.SectionFKSubgroups.Include(s => s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(s => s.Id == subgroupId);
        //    var existsSubgroups =
        //        _db.SectionFKSubgroupMemberships.FirstOrDefault(
        //            m => (m.studentId == studentId)
        //                && m.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer == subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer
        //                && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId);
        //    if (existsSubgroups != null)
        //    {
        //        RemoveMembership(existsSubgroups);
        //        return Json(new { success = true }); ;
        //    }
        //    return Json(new { success = false, message = "Не удалось удалить студента из подгруппы" }, "text/html", Encoding.Unicode);

        //}

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult Delete([FromBody] int[] ids)
        {
            var toDelete = _db.SectionFKSubgroups.Include(_ => _.Meta.CompetitionGroup).Where(s => ids.Contains(s.Id));
            foreach (var sectionFKSubgroup in toDelete)
            {
                Logger.Info($"Удалена подгруппа {sectionFKSubgroup.Id} {sectionFKSubgroup.Name} в конкурсной группе {sectionFKSubgroup.Meta.CompetitionGroupId} {sectionFKSubgroup.Meta.CompetitionGroup.Name} сокращ. название");

                _db.SectionFKSubgroups.Remove(sectionFKSubgroup);

            }
            _db.SaveChanges();


            return JsonNet("Ok");
        }


        public ActionResult DownloadSubGroups(string filter, int competitionGroupId)
        {
            var subgroups = GetSectionSubgroups(null, filter, competitionGroupId);
            var stream = new VariantExport().Export(new { Rows = subgroups }, "minorSubgroupsTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Список подгрупп на секциях.xlsx".ToDownloadFileName());
        }

        public FileResult MassPrint(string filter, int competitionGroupId)
        {
            var subgroups = GetSectionSubgroups(null, filter, competitionGroupId).ToList();

            using (var zipArchiveStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var subgroup in subgroups)
                        using (var reportStream = PrepareSubgroupReportStream(subgroup.GetPropertyValue<int>("Id")))
                        {
                            var entry =
                                zipArchive.CreateEntry(
                                    ($"{subgroup.GetPropertyValue<string>("Name")} {subgroup.GetPropertyValue<string>("teacher")}" + ".xlsx").CleanFileName(),
                                    CompressionLevel.Fastest);
                            using (var zipEntryStream = entry.Open())
                            {
                                reportStream.Position = 0;
                                reportStream.CopyTo(zipEntryStream);
                            }
                        }
                }

                zipArchiveStream.Position = 0;

                return new FileContentResult(zipArchiveStream.ToArray(), "application/zip")
                {
                    FileDownloadName = "Списочный состав подгрупп.zip".ToDownloadFileName()
                };
            }
        }

        private Stream PrepareSubgroupReportStream(int subgroupId)
        {
            var reportVms =
                _db.SectionFKSubgroupMemberships.Where(
                        m =>
                            (m.SubgroupId == subgroupId) || (m.Subgroup.ParentId == subgroupId) ||
                            (m.Subgroup.Parent.ParentId == subgroupId))
                    .Distinct()
                    .OrderBy(s => s.Student.Person.Surname)
                    .ThenBy(s => s.Student.Person.Name)
                    .Include(s => s.Student.Person)
                    .Include(s => s.Student.Group)
                    .ToList();
            var title =
                _db.SectionFKSubgroups.Where(s => s.Id == subgroupId)
                    .Select(s => s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.SectionFK.Module.title)
                    .FirstOrDefault();
            var subgroup = _db.SectionFKSubgroups.Where(s => s.Id == subgroupId).Select(s => s.Name).FirstOrDefault();
            var stream = new VariantExport().Export(new { Rows = reportVms, Title = title, Subgroup = subgroup },
                "subgroupReportTemplate.xlsx");
            return stream;
        }


        public ActionResult SimpleCopyMembership(int srcId, int dstId, bool withTeacher)
        {
            var src = _db.SectionFKSubgroups.Include(s => s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(_ => _.Id == srcId);
            var dst = _db.SectionFKSubgroups.Include(s => s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(s => s.Id == dstId);

            int cnt = 0;
            if (src.Id != dst.Id)
            {
                if (src.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer ==
                    dst.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer)
                {
                    return RedirectToAction("Index", new { competitionGroupId = src.Meta.CompetitionGroupId });
                }
                var srcStudents = src.Students.Select(_ => _.studentId).ToList();

                var students = _db.SectionFKSubgroups.Where(_ => _.SubgroupCountId == dst.SubgroupCountId &&
                                                                    _.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer ==
                                                                    dst.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer
                                                                        .rmer)
                    .SelectMany(_ => _.Students).Where(s => srcStudents.Contains(s.studentId))
                    .Include(s => s.Student)
                    .Include(s => s.Student.Group)
                    .ToList();

                if (students.Count != 0)
                {
                    var studentsInfo = "Существуют студенты, зачисленные в другие подгруппы по выбранному виду нагрузки<br></br>";
                    students = students.Distinct().ToList();
                    foreach (var student in students)
                    {
                        var studentName = student.Student.Person.ShortName();
                        var groupName = student.Student.Group.Name;
                        var subgroupName = student.Subgroup.Name;
                        studentsInfo += $"{studentName}, {groupName}, {subgroupName}<br>";
                    }

                    return Json(new { success = false, message = studentsInfo, focus = dstId });//, "text/html", Encoding.Unicode);
                }

                if (withTeacher)
                    dst.TeacherId = src.TeacherId;
                dst.Students.Clear();

                foreach (var student in src.Students)
                {
                    cnt++;
                    dst.Students.Add(new SectionFKSubgroupMembership
                    {
                        SubgroupId = dst.Id,
                        studentId = student.studentId
                    });
                    Logger.Info($"Студент {student.studentId} скопирован в подгруппу {dst.Id} ФК");
                }
                _db.SaveChanges();
            }
            return Json(new {
                success = true,
                message = "Скопировано " + cnt + (cnt < 5 && cnt > 0 ? " зачисления " : " зачислений"),
                competitionGroupId = src.Meta.CompetitionGroupId,
                focus = dstId
            });//, "text/html", Encoding.Unicode);
        }


        public ActionResult DownloadSubgroupReport(int subgroupId)
        {

            var stream = PrepareSubgroupReportStream(subgroupId);
            var firstOrDefault = _db.SectionFKSubgroups.Include(_ => _.Teacher)
                .FirstOrDefault(s => s.Id == subgroupId);
            var title =
                $"{firstOrDefault.Name} {firstOrDefault?.Teacher?.initials}";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ("Отчёт по подгруппе " + title + ".xlsx").ToDownloadFileName());
        }
    }
}