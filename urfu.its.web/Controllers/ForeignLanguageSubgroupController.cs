using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
//using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.ForeignLanguageManager)]
    public class ForeignLanguageSubgroupController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: ForeignLanguageSubgroup
        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter,
            string message = null, int? focus = null)
        {
            ViewBag.Message = message;
            ViewBag.Focus = focus;
            var competitionGroup = db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("competitionGroup not found");

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var subgroups = GetSectionSubgroups(sort, filter, competitionGroupId);

                return JsonNet(new
                {
                    data = subgroups,
                    total = subgroups.Count()
                });
            }

            return View(competitionGroup);
        }

        public ActionResult StudentsCount(string filter, int competitionGroupId)
        {
            var subgroups =
                GetSectionSubgroups(null, filter, competitionGroupId)
                    .ToList()
                    .Select(v => v.GetPropertyValue<int>("Id"))
                    .SelectMany(v =>
                            db.ForeignLanguageSubgroupMemberships.Where(
                                sm =>
                                    (sm.SubgroupId == v) || (sm.Subgroup.ParentId == v) ||
                                    (sm.Subgroup.Parent.ParentId == v))
                    ).Distinct().Count(); //GroupBy(_=>_.studentId).Count();
            return JsonNet(subgroups);
        }

        private IQueryable<object> GetSectionSubgroups(string sort, string filter, int competitionGroupId)
        {
          
            var subgroupmembershipsCg = db.ForeignLanguageSubgroupMemberships
                .Where(m => m.Subgroup.Meta.CompetitionGroupId == competitionGroupId);


            var subgroups = db.ForeignLanguageSubgroups
                .Where(s => s.Meta.CompetitionGroupId == competitionGroupId)
                .Select(s => new
                {
                    s,
                    memberships = subgroupmembershipsCg.Where(sm => (sm.SubgroupId == s.Id))
                        .Concat(subgroupmembershipsCg.Where(sm => (sm.Subgroup.ParentId == s.Id)))
                        .Concat(subgroupmembershipsCg.Where(sm => (sm.Subgroup.Parent.ParentId == s.Id))).ToList()

                })
                .Select(v => new
                {
                    v.s.Id,
                    v.s.Name,
                    v.s.InnerNumber,
                    HasScores = v.s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kgmer == 3,
                    moduleId = v.s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguageId,
                    ModuleTitle = v.s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.title,
                    Year = v.s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.Year.ToString(),
                    Semester = v.s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.Semester.Name.ToString(),
                    semesterId = v.s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.Semester.Id,
                    subgroupType = v.s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    kgmer = v.s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kgmer.ToString(),
                    v.s.Limit,
                    count =v.memberships.Count(),
                    levels= v.memberships.Where(m=>m.Student.ForeignLanguageLevel!=null).
                        Select(m=>m.Student.ForeignLanguageLevel).Distinct().OrderBy(x=>x),
                    teacher = v.s.Teacher.initials,
                    description = v.s.Description
                });
            subgroups = subgroups.Where(FilterRules.Deserialize(filter));
            SortRules sortRules = SortRules.Deserialize(sort);
            if (sort?.Any() ?? false)
            {
                subgroups = subgroups.OrderByThenBy(sortRules.FirstOrDefault(), v => v.ModuleTitle, v => v.Year,
                    v => v.Semester, v => v.kgmer, v => v.subgroupType, v => v.Name);
            }
            else
            {
                subgroups = subgroups.OrderBy(v => v.ModuleTitle).ThenBy(v => v.Year).ThenBy(
                v => v.Semester).ThenBy(v => v.kgmer).ThenBy(v => v.subgroupType).ThenBy(v => v.InnerNumber).ThenBy(v => v.Name);
            }

            return subgroups;
        }

        public ActionResult Modules(int competitionGroupId)
        {
            var modules = db.ForeignLanguageSubgroups
                .Where(
                    s =>
                        s.Meta.ForeignLanguageDisciplineTmerPeriod.ForeignLanguageSubgroupCounts.Any(
                            _ => _.CompetitionGroupId == competitionGroupId))
                .Select(_ => new
                {
                    Id = _.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.uuid,
                    Name = _.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.title,
                    _.Meta.ForeignLanguageDisciplineTmerPeriod.Period.Year,
                    _.Meta.ForeignLanguageDisciplineTmerPeriod.Period.SemesterId
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

        public ActionResult Create(int competitionGroupId)
        {
            var competitionGroup = db.ForeignLanguageCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("competitionGroup not found");
            if (CreateSubgroups(competitionGroupId, competitionGroup))
                return RedirectToAction("Index", new { competitionGroupId });


            return RedirectToAction("Index", new { competitionGroupId });
        }

        public bool CreateSubgroups(int competitionGroupId, ForeignLanguageCompetitionGroup competitionGroup)
        {
            var metaSubgroups = db.ForeignLanguageTmerPeriods
                .Where(
                    x =>
                        (x.Period.Year == competitionGroup.Year) &&
                        (x.Period.SemesterId == competitionGroup.SemesterId) &&
                        x.ForeignLanguageSubgroupCounts.Any(_ => _.CompetitionGroupId == competitionGroupId))
                //.Where(x=>  x.ForeignLanguageSubgroupCounts.Any(_=>_.CompetitionGroupId == competitionGroupId))
                .OrderByDescending(x => x.Tmer.Tmer.kmer == "tlekc")
                .ThenBy(x => x.Tmer.Tmer.kmer == "tlab")
                .ThenBy(x => x.Tmer.Tmer.kmer == "tprak")
                .ToList();


            foreach (var meta in metaSubgroups)
            {
                var distribution = meta.ExtractDistribution();

                var busyNumbers =
                    new HashSet<int>(
                        db.ForeignLanguageSubgroups.Where(
                            s =>
                                (s.Meta.ForeignLanguageDisciplineTmerPeriodId == meta.Id) &&
                                (s.Meta.CompetitionGroupId == competitionGroupId)).Select(s => s.InnerNumber));
                var metaCount =
                    meta.ForeignLanguageSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.CompetitionGroupId == competitionGroupId) &&
                            (_.ForeignLanguageDisciplineTmerPeriodId == meta.Id));
                var exists = busyNumbers.Count();

                for (; exists < metaCount.GroupCount; exists++)
                {
                    var innerNumber = FindInnerNumber(busyNumbers);
                    busyNumbers.Add(innerNumber);

                    //if (meta.Tmer == null)
                    //    continue;
                    var foreignLanguageProperty =
                        db.ForeignLanguageProperties.FirstOrDefault(
                            _ =>
                                (_.ForeignLanguageId == meta.Period.ForeignLanguageId) &&
                                (_.ForeignLanguageCompetitionGroupId == competitionGroupId));

                    var grp = new ForeignLanguageSubgroup
                    {
                        Name =
                            meta.Tmer.Discipline.ForeignLanguage.Module.shortTitle + "\\" +
                            meta.Tmer.Discipline.Discipline.title + "\\" + meta.Tmer.Tmer.rmer + "\\" + innerNumber,
                        Limit = (int)Math.Ceiling(foreignLanguageProperty.Limit / (double)metaCount.GroupCount),
                        SubgroupCountId = metaCount.Id,
                        InnerNumber = innerNumber,
                        ExpectedChildCount = distribution[(innerNumber - 1) % distribution.Length]
                    };

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tlekc")
                        grp.Name = meta.Tmer.Discipline.ForeignLanguage.Module.shortTitle + "\\" +
                                   meta.Tmer.Discipline.Discipline.title + "\\л" + innerNumber;

                    if (meta.Tmer.Tmer.kmer.ToLower() == "prz")
                        grp.Name = meta.Tmer.Discipline.ForeignLanguage.Module.shortTitle + "\\" +
                                   meta.Tmer.Discipline.Discipline.title + "\\зачет" + innerNumber;

                    if (meta.Tmer.Tmer.kmer.ToLower() == "prex")
                        grp.Name = meta.Tmer.Discipline.ForeignLanguage.Module.shortTitle + "\\" +
                                   meta.Tmer.Discipline.Discipline.title + "\\экзамен" + innerNumber;

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tprak")
                    {
                        grp.Limit = 25;
                        var parents = db.ForeignLanguageSubgroups.Where(s =>
                                (s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer.ToLower() == "tlekc")
                                &&
                                (s.Meta.ForeignLanguageDisciplineTmerPeriod.ForeignLanguagePeriodId ==
                                 meta.ForeignLanguagePeriodId)
                                &&
                                (s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.ForeignLanguageDisciplineId ==
                                 meta.Tmer.ForeignLanguageDisciplineId))
                            .Select(p => new
                            {
                                p.Id,
                                p.Name,
                                p.ExpectedChildCount,
                                Count = db.ForeignLanguageSubgroups.Count(sx => sx.ParentId == p.Id)
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
                            grp.Name = meta.Tmer.Discipline.ForeignLanguage.Module.shortTitle + "\\" +
                                       meta.Tmer.Discipline.Discipline.title + "\\п" + innerNumber;
                            grp.Limit = (int)Math.Ceiling(foreignLanguageProperty.Limit / (double)metaCount.GroupCount);
                        }
                    }

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tlab")
                    {
                        grp.Limit = 12;
                        var parent = db.ForeignLanguageSubgroups.Where(
                                             s =>
                                                 (s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer.ToLower() ==
                                                  "tprak")
                                                 &&
                                                 (s.Meta.ForeignLanguageDisciplineTmerPeriod.ForeignLanguagePeriodId ==
                                                  meta.ForeignLanguagePeriodId)
                                                 &&
                                                 (s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer
                                                      .ForeignLanguageDisciplineId == meta.Tmer.ForeignLanguageDisciplineId)
                                         )
                                         .OrderBy(s => db.ForeignLanguageSubgroups.Count(sx => sx.ParentId == s.Id))
                                         .ThenBy(s => s.Name)
                                         .FirstOrDefault()
                                     ??
                                     db.ForeignLanguageSubgroups.Where(
                                             s =>
                                                 (s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer.ToLower() ==
                                                  "tlekc")
                                                 &&
                                                 (s.Meta.ForeignLanguageDisciplineTmerPeriod.ForeignLanguagePeriodId ==
                                                  meta.ForeignLanguagePeriodId)
                                                 &&
                                                 (s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer
                                                      .ForeignLanguageDisciplineId == meta.Tmer.ForeignLanguageDisciplineId)
                                         )
                                         .OrderBy(s => db.ForeignLanguageSubgroups.Count(sx => sx.ParentId == s.Id))
                                         .ThenBy(s => s.Name)
                                         .FirstOrDefault();

                        if (parent != null)
                        {
                            grp.ParentId = parent.Id;
                            grp.Name = parent.Name + "\\лаб" + innerNumber;
                        }
                        else
                        {
                            grp.Name = meta.Tmer.Discipline.ForeignLanguage.Module.shortTitle + "\\" +
                                       meta.Tmer.Discipline.Discipline.title + "\\лаб" + innerNumber;
                            grp.Limit = (int)Math.Ceiling(foreignLanguageProperty.Limit / (double)metaCount.GroupCount);
                        }
                    }

                    db.ForeignLanguageSubgroups.Add(grp);
                    //db.Entry(grp).State == EntityState.Added;
                    db.SaveChanges();
                    Logger.Info(
                        $"Создана подгруппа {grp.Id} {grp.Name} в конкурсной группе {metaCount.CompetitionGroupId} {competitionGroup.Name} сокращ. название");
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
            using (var dbtran = db.Database.BeginTransaction())
            {
                bool success = true;
                var msg = "";

                try
                { 
                    var metas =
                        db.ForeignLanguageSubgroupCounts.Include(x => x.ForeignLanguageDisciplineTmerPeriod)
                            .Where(m => m.Subgroups.Count() > 0)
                            .Where(m => !db.ForeignLanguageSubgroups.Any(s => s.Parent.SubgroupCountId == m.Id))
                            .ToList();
                    foreach (var meta in metas)
                    {
                        var toAdmitt = db.Students.Where(
                                s =>
                                    db.ForeignLanguageAdmissions.Any(
                                        ma =>
                                            (ma.studentId == s.Id) &&
                                            (ma.ForeignLanguageCompetitionGroupId == competitionGroupId) &&
                                            (ma.ForeignLanguageId ==
                                             meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguageId) &&
                                            (ma.Status == AdmissionStatus.Admitted))
                            )
                            .Where(s => !db.ForeignLanguageSubgroupMemberships.Any(m => m.Subgroup.SubgroupCountId == meta.Id && m.studentId == s.Id))
                            .OrderBy(_ => _.ForeignLanguageLevel)
                            .Select(_ => new { _.Id, _.ForeignLanguageLevel })
                            .GroupBy(_ => _.ForeignLanguageLevel)
                            .OrderBy(_ => _.Key);



                        var subgroups =
                            db.ForeignLanguageSubgroups.Where(s => s.SubgroupCountId == meta.Id)
                                //.OrderBy(_=>_.Name)
                                //.ThenBy(_=>_.Name.Length)
                                .OrderBy(_ => _.InnerNumber)

                                .Select(s => new { s.Id, admCount = db.ForeignLanguageSubgroupMemberships.Count(_ => _.SubgroupId == s.Id), admitted = s.Students.Count(), s.Limit })
                                .ToList().GetEnumerator();


                        foreach (var group in toAdmitt)
                        {
                            var subgroup = subgroups.Current;
                            if (subgroups.MoveNext())
                            {
                                subgroup = subgroups.Current;
                            }
                            else
                            {
                                break;
                            }

                            var admittedCount = subgroup.admCount;//subgroup.admitted;
                            bool end;
                            foreach (var student in group)
                            {
                                end = false;
                                while (subgroup.Limit <= admittedCount) // если лимит превышен то переходим на другую подгруппу
                                {
                                    if (!subgroups.MoveNext())
                                    {
                                        end = true;
                                        break;
                                    }
                                    subgroup = subgroups.Current;
                                    admittedCount = subgroup.admCount;
                                }
                                if (end) break; // если больше нет групп, выходим из цикла
                                if (subgroup.Limit > admittedCount)
                                {
                                    db.ForeignLanguageSubgroupMemberships.Add(new ForeignLanguageSubgroupMembership
                                    {
                                        studentId = student.Id,
                                        SubgroupId = subgroup.Id
                                    });
                                    Logger.Info($"Студент {student.Id} распределен в подгруппу {subgroup.Id} ИЯ");
                                    admittedCount++;
                                }
                            }

                        }

                        db.SaveChanges();
                    }
                    
                    dbtran.Commit();
                }
                catch (DbUpdateException updateException)
                {
                    Logger.Info($"Ошибка при заполнении подгрупп ИЯ конкурсной группы {competitionGroupId}");
                    Logger.Error(updateException);
                    msg = "Студент уже зачислен в подгруппу в текущий год и семестр. Если Вы видите эту ошибку, то необходимо оформить запрос в тех. поддержку";
                    success = false;
                }
                catch (Exception e)
                {
                    Logger.Info($"Ошибка при заполнении подгрупп ИЯ конкурсной группы {competitionGroupId}");
                    Logger.Error(e);
                    dbtran.Rollback();
                    msg = "Неизвестная ошибка";
                    success = false;
                }

                return Json(new { msg, success });
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var subgroup = db.ForeignLanguageSubgroups.Find(id);
            if (subgroup == null)
                return NotFound();

            var teachers =
                db.ForeignLanguageProperties.Where(
                        p =>
                            (p.ForeignLanguageCompetitionGroupId == subgroup.Meta.CompetitionGroupId) &&
                            (p.ForeignLanguageId ==
                             db.ForeignLanguageSubgroups.FirstOrDefault(g => g.Id == id)
                                 .Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguageId))
                    .SelectMany(p => p.Teachers)
                    .ToList();

            ViewBag.TeacherId = new SelectList(teachers, "pkey", "BigName", subgroup.TeacherId);

            return View(subgroup);
        }

        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult Edit(ForeignLanguageSubgroup subgroup)
        {
            if (ModelState.IsValid)
            {
                var dbEntry = db.ForeignLanguageSubgroups.Find(subgroup.Id);
                dbEntry.Limit = subgroup.Limit;
                if (User.IsInRole(ItsRoles.Admin)) 
                    dbEntry.Name = subgroup.Name;
                dbEntry.TeacherId = subgroup.TeacherId;
                dbEntry.Description = subgroup.Description;
                db.SaveChanges();
                Logger.Info($"Отредактирована подгруппа по ИЯ Id {subgroup.Id} TeacherId {subgroup.TeacherId} Description {subgroup.Description} ");
                return RedirectToAction("Index",
                    new
                    {
                        programId = dbEntry.Id,
                        focus = subgroup.Id,
                        competitionGroupId = dbEntry.Meta.CompetitionGroupId
                    });
            }
            var teachers =
                db.ForeignLanguageProperties.Where(
                        p =>
                            (p.ForeignLanguageCompetitionGroupId == subgroup.Meta.CompetitionGroupId) &&
                            (p.ForeignLanguageId ==
                             db.ForeignLanguageSubgroups.FirstOrDefault(g => g.Id == subgroup.Id)
                                 .Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguageId))
                    .SelectMany(p => p.Teachers)
                    .ToList();

            ViewBag.TeacherId = new SelectList(teachers, "pkey", "BigName", subgroup.TeacherId);


            return View(subgroup);
        }

        public ActionResult Students(int id, int? page, int? limit, string sort, string filter)
        {
            var subgroup =
                db.ForeignLanguageSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.ForeignLanguageDisciplineTmerPeriod)
                    .Include(x => x.Meta.ForeignLanguageDisciplineTmerPeriod.Period)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == id);
            ViewBag.Subgroup = subgroup;
            var levels = db.Students.Distinct().Select(_ => new { Level = _.ForeignLanguageLevel }).OrderBy(_ => _.Level).ToList();
            ViewBag.Levels = levels;
            return View(subgroup);
        }

        public ActionResult StudentsAjax(int id, bool hideStudents, int? page, int? limit, string sort, string filter)
        {
            var subgroup =
                db.ForeignLanguageSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.ForeignLanguageDisciplineTmerPeriod)
                    .Include(x => x.Meta.ForeignLanguageDisciplineTmerPeriod.Period)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == id);

            var students = GetForeignLanguageSubgroupsStudents(id, sort, filter, subgroup, hideStudents);

            var paginated = students; //.ToPagedList(page ?? 1, limit ?? 25);
            return JsonNet(new
            {
                data = paginated,
                total = students.Count()
            });
        }

        private IQueryable<object> GetForeignLanguageSubgroupsStudents(int id, string sort, string filter,
            ForeignLanguageSubgroup subgroup, bool hideStudents = false)
        {
            var query = db.Students
                .Where(s => db.ForeignLanguageAdmissions.Any(ma => (ma.studentId == s.Id)
                                                                   && (ma.Status == AdmissionStatus.Admitted)
                                                                   &&
                                                                   (ma.ForeignLanguageCompetitionGroupId ==
                                                                    subgroup.Meta.CompetitionGroupId)
                                                                   &&
                                                                   (ma.ForeignLanguageId ==
                                                                    subgroup.Meta.ForeignLanguageDisciplineTmerPeriod
                                                                        .Period.ForeignLanguageId))
                    && (!hideStudents || hideStudents && (s.Status == "Активный" || s.Status == "Отп.с.посещ." || s.Status == "Отп.дород.послерод."))
                    )
                .Select(s => new
                {
                    Student = s,
                    s.Person,
                    GroupName = db.GroupsHistories.FirstOrDefault(g => g.GroupId == s.GroupId && g.YearHistory == subgroup.Meta.CompetitionGroup.Year && g.Course == subgroup.Meta.CompetitionGroup.StudentCourse).Name,
                    Included = db.ForeignLanguageSubgroupMemberships.Any(m => (m.studentId == s.Id)
                                                                              &&
                                                                              //m.Subgroup.SubgroupCountId == subgroup.SubgroupCountId),
                                                                              ((m.SubgroupId == id) ||
                                                                               (m.Subgroup.ParentId == id) ||
                                                                               (m.Subgroup.Parent.ParentId == id))),
                    AnotherGroup = db.ForeignLanguageSubgroupMemberships.FirstOrDefault(m => (m.studentId == s.Id)
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
                                                                                                           .ForeignLanguageDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tlekc") ||
                                                                                                      (m.Subgroup.Meta
                                                                                                           .ForeignLanguageDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tprak") ||
                                                                                                      (m.Subgroup.Meta
                                                                                                           .ForeignLanguageDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "prz") ||
                                                                                                      (m.Subgroup.Meta
                                                                                                           .ForeignLanguageDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tlab"))
                                                                                                     &&
                                                                                                     ((subgroup.Meta
                                                                                                           .ForeignLanguageDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tlekc") ||
                                                                                                      (subgroup.Meta
                                                                                                           .ForeignLanguageDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tprak") ||
                                                                                                      (subgroup.Meta
                                                                                                           .ForeignLanguageDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "prz") ||
                                                                                                      (subgroup.Meta
                                                                                                           .ForeignLanguageDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tlab"))
                                                                                                 )
                                                                                             )
                                                                                             && (m.SubgroupId != id))
                        //и я точно в другой группе
                        .Subgroup.Name
                });

            var students = query.Select(r => new
            {
                r.Student.Id,
                GroupName = r.GroupName,
                r.Person.Surname,
                r.Person.Name,
                r.Person.PatronymicName,
                r.Included,
                r.AnotherGroup,
                r.Student.ForeignLanguageLevel,
                r.Student.ForeignLanguageRating,
                r.Student.Status
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
                db.ForeignLanguageSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.ForeignLanguageDisciplineTmerPeriod)
                    .Include(x=>x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == subgroupId);
            if (subgroup == null) return NotFound("ForeignLanguageSubgroup not found");
          var  similarsubgroups =  FindSubgroups(subgroupId, subgroup);

            var msg = "";
            if(similarsubgroups.Count !=0)
            {
                 msg= subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "tprak" ?
                    "зачету" : "практическим занятиям";
            }           
            return Json(new { msg });
        }

        private List<ForeignLanguageSubgroupMembership> FindSubgroups(int subgroupId, ForeignLanguageSubgroup subgroup)
        {
            var subgroupStudents = GetForeignLanguageSubgroupsStudents(subgroupId, null, null, subgroup).ToList()
                .Where(s => s.GetPropertyValue<bool>("Included") == true);

            List<ForeignLanguageSubgroupMembership> similarsubgroups = new List<ForeignLanguageSubgroupMembership>();
            foreach (var s in subgroupStudents)
            {
                var studentId = s.GetPropertyValue<string>("Id");
                var ssubgroup= db.ForeignLanguageSubgroupMemberships.Include(t => t.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer)
                  .FirstOrDefault(m => m.studentId == studentId
                    && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId
                    && m.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId != subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId);
                if (ssubgroup != null)
                    similarsubgroups.Add(ssubgroup);
            }
            return similarsubgroups;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult StudentsMembership(bool include, int subgroupId, string filter, bool similarsubgroup)
        {
            using (var dbtran = db.Database.BeginTransaction())
            {
                var subgroup =
                    db.ForeignLanguageSubgroups.Include(x => x.Meta)
                        .Include(x => x.Meta.ForeignLanguageDisciplineTmerPeriod)
                        .Include(x => x.Meta.CompetitionGroup)
                        .FirstOrDefault(_ => _.Id == subgroupId);
                if (subgroup == null) return NotFound("ForeignLanguageSubgroup not found");

                bool reload = false;
                string msg = "";
                bool success = true;

                try
                {
                    var subgroupStudents =
                        GetForeignLanguageSubgroupsStudents(subgroupId, null, filter, subgroup).ToList().Where(s =>
                        {
                            var anotherGroup = s.GetPropertyValue<string>("AnotherGroup");
                            return string.IsNullOrEmpty(anotherGroup);
                        }).ToList();

                    if (similarsubgroup)
                    {
                        FindSubgroups(subgroupId, subgroup).ForEach(s => { StudentMembershipInternal(include, s.studentId, s.SubgroupId); });
                    }
                    subgroupStudents.ForEach(
                        s => { StudentMembershipInternal(include, s.GetPropertyValue<string>("Id"), subgroupId); });

                    var curStudentsCount = db.ForeignLanguageSubgroupMemberships.Count(m => m.SubgroupId == subgroupId);
                    if ((subgroup?.Limit < curStudentsCount) && include)
                        msg = "Превышен лимит";

                    dbtran.Commit();
                }
                catch (DbUpdateException updateException)
                {
                    Logger.Info($"Ошибка при изменении состава подгруппы ИЯ {subgroup.Id} {subgroup.Name}");
                    Logger.Error(updateException);
                    msg = "Студент уже зачислен в подгруппу в текущий год и семестр. Если Вы видите эту ошибку, то необходимо оформить запрос в тех. поддержку";
                    success = false;
                }
                catch (Exception e)
                {
                    Logger.Info($"Ошибка при изменении состава подгруппы ИЯ {subgroup.Id} {subgroup.Name}");
                    Logger.Error(e);
                    dbtran.Rollback();
                    msg = "Неизвестная ошибка";
                    success = false;
                }

                return Json(new { msg, reload, success });
            }
        }

        private void StudentMembershipInternal(bool include, string studentId, int subgroupId)
        {
            var subgroup = db.ForeignLanguageSubgroups.Include(s => s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(s => s.Id == subgroupId);

            var existsSubgroups =
                db.ForeignLanguageSubgroupMemberships.FirstOrDefault(
                    m => (m.studentId == studentId)
                        && m.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer == subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer
                        && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId);

            if (existsSubgroups == null) // если студент не содержится ни в одной подгруппе
            {
                if (include)
                {
                    AddMembership(new ForeignLanguageSubgroupMembership
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

                        AddMembership(new ForeignLanguageSubgroupMembership
                        {
                            studentId = studentId,
                            SubgroupId = subgroupId
                        });
                    }
                }
                else // если содержится в текущей подгруппе
                {
                    if (!include)
                    {
                        RemoveMembership(existsSubgroups);
                    }

                }
            }
        }

        private void RemoveMembership(ForeignLanguageSubgroupMembership membership)
        {
            db.ForeignLanguageSubgroupMemberships.Remove(membership);
            Logger.Info($"Студент {membership.studentId} удален из подгруппы {membership.SubgroupId} ИЯ");
            db.SaveChanges();
        }

        private void AddMembership(ForeignLanguageSubgroupMembership membership)
        {
            db.ForeignLanguageSubgroupMemberships.Add(membership);
            db.SaveChanges();
            Logger.Info($"Студент {membership.studentId} добавлен в подгруппу {membership.SubgroupId} ИЯ");
        }


        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult StudentMembership(bool include, string studentId, int subgroupId)
        {
            using (var dbtran = db.Database.BeginTransaction())
            {
                var msg = "";
                bool success = true;
                var subgroup = db.ForeignLanguageSubgroups.FirstOrDefault(m => m.Id == subgroupId);
                
                try 
                { 
                    StudentMembershipInternal(include, studentId, subgroupId);
                    
                    if (!include)
                    {
                        var similarsubgroup = db.ForeignLanguageSubgroupMemberships.Include(s => s.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer)
                                                .FirstOrDefault(m => (m.studentId == studentId)
                                               && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId
                                               && m.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId
                                               != subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId);

                        if (similarsubgroup != null)
                        {
                            var rmer = similarsubgroup.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.TmerId == "tprak" ?
                               "практическим занятиям" : "зачету";
                            return Json(new { rmer, subgroupId = similarsubgroup.SubgroupId });
                        }

                    }

                    var curStudentsCount = db.ForeignLanguageSubgroupMemberships.Count(m => m.SubgroupId == subgroupId);
                    if ((subgroup?.Limit < curStudentsCount) && include)
                        msg = "Превышен лимит";
                    
                    dbtran.Commit();
                }
                catch (DbUpdateException updateException)
                {
                    Logger.Info($"Ошибка при изменении состава подгруппы ИЯ. Cтудент {studentId}, зачисление {include}, подгруппа ИЯ {subgroup.Id} {subgroup.Name}");
                    Logger.Error(updateException);
                    msg = "Студент уже зачислен в подгруппу в текущий год и семестр. Если Вы видите эту ошибку, то необходимо оформить запрос в тех. поддержку";
                    success = false;
                }
                catch (Exception e)
                {
                    Logger.Info($"Ошибка при изменении состава подгруппы ИЯ. Cтудент {studentId}, зачисление {include}, подгруппа ИЯ {subgroup.Id} {subgroup.Name}");
                    Logger.Error(e);
                    dbtran.Rollback();
                    msg = "Неизвестная ошибка";
                    success = false;
                }

                return Json(new { msg, success });
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult Delete([FromBody] int[] ids)
        {
            db.ForeignLanguageSubgroups.RemoveRange(db.ForeignLanguageSubgroups.Where(s => ids.Contains(s.Id)));
            db.SaveChanges();
            return JsonNet("Ok");
        }


        public ActionResult DownloadSubGroups(string filter, int competitionGroupId)
        {
            var subgroups = GetSectionSubgroups(null, filter, competitionGroupId);
            var stream = new VariantExport().Export(new {Rows = subgroups}, "minorSubgroupsTemplate.xlsx");

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
                                    (subgroup.GetPropertyValue<string>("Name") + ".xlsx").CleanFileName(),
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
                db.ForeignLanguageSubgroupMemberships.Where(
                        m =>
                            (m.SubgroupId == subgroupId) || (m.Subgroup.ParentId == subgroupId) ||
                            (m.Subgroup.Parent.ParentId == subgroupId))
                    .Distinct()
                    .OrderBy(s => s.Student.Person.Surname)
                    .ThenBy(s => s.Student.Person.Name)
                    .Include(s => s.Student.Person)
                    .Include(s => s.Student.Group)
                    .ToList();
            var Title =
                db.ForeignLanguageSubgroups.Where(s => s.Id == subgroupId)
                    .Select(s => s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.ForeignLanguage.Module.title)
                    .FirstOrDefault();
            var Subgroup =
                db.ForeignLanguageSubgroups.Where(s => s.Id == subgroupId).Select(s => s.Name).FirstOrDefault();
            var stream = new VariantExport().Export(new {Rows = reportVms, Title, Subgroup},
                "subgroupReportTemplate.xlsx");
            return stream;
        }


        public ActionResult SimpleCopyMembership(int srcId, int dstId, bool withTeacher)
        {
            using (var dbtran = db.Database.BeginTransaction())
            {
                var src = db.ForeignLanguageSubgroups.Include(s => s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(_ => _.Id == srcId);
                var dst = db.ForeignLanguageSubgroups.Include(s => s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(s => s.Id == dstId);

                var msg = "";
                bool success = true;

                try
                {
                    int cnt = 0;
                    if (src.Id != dst.Id)
                    {
                        if (src.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.rmer ==
                            dst.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.rmer)
                        {
                            return RedirectToAction("Index", new { competitionGroupId = src.Meta.CompetitionGroupId });
                        }
                        var srcStudents = src.Students.Select(_ => _.studentId).ToList();

                        var students = db.ForeignLanguageSubgroups.Where(_ => _.SubgroupCountId == dst.SubgroupCountId &&
                                                                            _.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.rmer ==
                                                                            dst.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer
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
                            dst.Students.Add(new ForeignLanguageSubgroupMembership
                            {
                                SubgroupId = dst.Id,
                                studentId = student.studentId
                            });
                            db.SaveChanges();
                            Logger.Info($"Студент {student.studentId} скопирован в подгруппу {dst.Id} ИЯ");
                        }

                        db.SaveChanges();
                        dbtran.Commit();
                        msg = "Скопировано " + cnt + (cnt < 5 && cnt > 0 ? " зачисления " : " зачислений");
                    }
                }
                catch (DbUpdateException updateException)
                {
                    Logger.Info($"Ошибка при копировании cостава подгруппы ИЯ. Копирование подгруппы {src.Id} {src.Name} в подгруппу {dst.Id} {dst.Name}");
                    Logger.Error(updateException);
                    msg = "Студент уже зачислен в подгруппу в текущий год и семестр. Если Вы видите эту ошибку, то необходимо оформить запрос в тех. поддержку";
                    success = false;
                }
                catch (Exception e)
                {
                    Logger.Info($"Ошибка при копировании состава подгруппы ИЯ. Копирование подгруппы {src.Id} {src.Name} в подгруппу {dst.Id} {dst.Name}");
                    Logger.Error(e);
                    dbtran.Rollback();
                    msg = "Неизвестная ошибка";
                    success = false;
                }

                return Json(new
                {
                    success,
                    message = msg,
                    competitionGroupId = src.Meta.CompetitionGroupId,
                    focus = dstId
                });//, "text/html", Encoding.Unicode);
            }
        }

        public ActionResult DownloadSubgroupReport(int subgroupId)
        {
            var subgroup = db.ForeignLanguageSubgroups.Find(subgroupId);
            var stream = PrepareSubgroupReportStream(subgroupId);

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ("Отчёт по подгруппе " + subgroup.Name + ".xlsx").ToDownloadFileName());
        }
    }
}