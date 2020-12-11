using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.ProjectView)]
    public class ProjectSubgroupController : BaseController
    {
        private readonly ApplicationDbContext db;

        public ProjectSubgroupController()
        {
            db = new ApplicationDbContext();
        }

        public ProjectSubgroupController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter,
            string message = null, int? focus = null)
        {
            ViewBag.Message = message;
            ViewBag.Focus = focus;
            var competitionGroup = db.ProjectCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("projectGroup not found");

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

            ViewBag.CanEdit = db.CanEditProjectCompetitionGroup(User, competitionGroupId);
            return View(competitionGroup);
        }
        
        private IQueryable<object> GetSectionSubgroups(string sort, string filter, int competitionGroupId)
        {
            var subgroups = db.ProjectSubgroupsForUser(User)
                .Where(s => s.Meta.CompetitionGroupId == competitionGroupId)
                .Select(v => new
                {
                    v.Id,
                    v.Name,
                    v.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Level,
                    HasScores = v.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kgmer == 3,
                    moduleId = v.Meta.ProjectDisciplineTmerPeriod.Period.ProjectId,
                    ModuleTitle = v.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title,
                    Year = v.Meta.ProjectDisciplineTmerPeriod.Period.Year.ToString(),
                    Semester = v.Meta.ProjectDisciplineTmerPeriod.Period.Semester.Name.ToString(),
                    semesterId = v.Meta.ProjectDisciplineTmerPeriod.Period.Semester.Id,
                    subgroupType = v.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    kgmer = v.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kgmer.ToString(),
                    v.Limit,
                    count =
                    db.ProjectSubgroupMemberships.Count(
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

        public ActionResult Create(int competitionGroupId)
        {
            var competitionGroup = db.ProjectCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);
            if (competitionGroup == null)
                return NotFound("projectGroup not found");

            if (CreateSubgroups(competitionGroupId, competitionGroup))
                return RedirectToAction("Index", new { competitionGroupId });
            return NotFound();
        }

        public bool CreateSubgroups(int competitionGroupId, ProjectCompetitionGroup competitionGroup)
        {
            var metaSubgroups = db.ProjectTmerPeriods
                .Where(
                    x =>
                        (x.Period.Year == competitionGroup.Year) &&
                        (x.Period.SemesterId == competitionGroup.SemesterId) &&
                        x.ProjectSubgroupCounts.Any(_ => _.CompetitionGroupId == competitionGroupId))
                .OrderByDescending(x => x.Tmer.Tmer.kmer == "tlekc")
                .ThenBy(x => x.Tmer.Tmer.kmer == "tlab")
                .ThenBy(x => x.Tmer.Tmer.kmer == "tprak")
                .ToList();


            foreach (var meta in metaSubgroups)
            {
                var distribution = meta.ExtractDistribution();

                var busyNumbers =
                    new HashSet<int>(
                        db.ProjectSubgroups.Where(
                            s =>
                                (s.Meta.ProjectDisciplineTmerPeriodId == meta.Id) &&
                                (s.Meta.CompetitionGroupId == competitionGroupId)).Select(s => s.InnerNumber));
                var metaCount =
                    meta.ProjectSubgroupCounts.FirstOrDefault(
                        _ =>
                            (_.CompetitionGroupId == competitionGroupId) &&
                            (_.ProjectDisciplineTmerPeriodId == meta.Id));

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
                        db.ProjectProperties.FirstOrDefault(
                            _ =>
                                (_.ProjectId == meta.Period.ProjectId) &&
                                (_.ProjectCompetitionGroupId == competitionGroupId));
                    if (sectionProperty == null)
                    {
                        return false;

                    }
                    var grp = new ProjectSubgroup
                    {
                        Name =
                            meta.Tmer.Discipline.Discipline.title + "\\" + meta.Tmer.Tmer.rmer + "\\" + innerNumber,
                        Limit = 0, // TODO посчитать!!!! //(int)Math.Ceiling(sectionProperty.Limit / (double)metaCount.GroupCount),
                        SubgroupCountId = metaCount.Id,
                        InnerNumber = innerNumber,
                        ExpectedChildCount = distribution[(innerNumber - 1) % distribution.Length]
                    };

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tlekc")
                        grp.Name = meta.Tmer.Discipline.Discipline.title + "\\л" + innerNumber;

                    if (meta.Tmer.Tmer.kmer.ToLower() == "prz")
                        grp.Name = meta.Tmer.Discipline.Discipline.title + "\\зачет" + innerNumber;

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tprak")
                    {
                        grp.Limit = 25;
                        var parents = db.ProjectSubgroups.Where(s =>
                                (s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer.ToLower() == "tlekc")
                                && (s.Meta.ProjectDisciplineTmerPeriod.ProjectPeriodId == meta.ProjectPeriodId)
                                &&
                                (s.Meta.ProjectDisciplineTmerPeriod.Tmer.ProjectDisciplineId ==
                                 meta.Tmer.ProjectDisciplineId))
                            .Select(p => new
                            {
                                p.Id,
                                p.Name,
                                p.ExpectedChildCount,
                                Count = db.ProjectSubgroups.Count(sx => sx.ParentId == p.Id)
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
                            grp.Name = meta.Tmer.Discipline.Discipline.title + "\\п" + innerNumber;
                        }
                    }

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tlab")
                    {
                        grp.Limit = 12;
                        var parent = db.ProjectSubgroups.Where(
                                             s =>
                                                 (s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer.ToLower() == "tprak")
                                                 &&
                                                 (s.Meta.ProjectDisciplineTmerPeriod.ProjectPeriodId ==
                                                  meta.ProjectPeriodId)
                                                 &&
                                                 (s.Meta.ProjectDisciplineTmerPeriod.Tmer.ProjectDisciplineId ==
                                                  meta.Tmer.ProjectDisciplineId)
                                         )
                                         .OrderBy(s => db.ProjectSubgroups.Count(sx => sx.ParentId == s.Id))
                                         .ThenBy(s => s.Name)
                                         .FirstOrDefault()
                                     ??
                                     db.ProjectSubgroups.Where(
                                             s =>
                                                 (s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer.ToLower() == "tlekc")
                                                 &&
                                                 (s.Meta.ProjectDisciplineTmerPeriod.ProjectPeriodId ==
                                                  meta.ProjectPeriodId)
                                                 &&
                                                 (s.Meta.ProjectDisciplineTmerPeriod.Tmer.ProjectDisciplineId ==
                                                  meta.Tmer.ProjectDisciplineId)
                                         )
                                         .OrderBy(s => db.ProjectSubgroups.Count(sx => sx.ParentId == s.Id))
                                         .ThenBy(s => s.Name)
                                         .FirstOrDefault();

                        if (parent != null)
                        {
                            grp.ParentId = parent.Id;
                            grp.Name = parent.Name + "\\лаб" + innerNumber;
                        }
                        else
                        {
                            grp.Name = meta.Tmer.Discipline.Discipline.title + "\\лаб" + innerNumber;
                        }
                    }

                    db.ProjectSubgroups.Add(grp);

                    db.SaveChanges();
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

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var subgroup = db.ProjectSubgroups.Find(id);
            if (subgroup == null)
                return NotFound();

            var teachers =
                db.ProjectProperties.Where(
                        p =>
                            (p.ProjectCompetitionGroupId == subgroup.Meta.CompetitionGroupId) &&
                            (p.ProjectId ==
                             db.ProjectSubgroups.FirstOrDefault(g => g.Id == id)
                                 .Meta.ProjectDisciplineTmerPeriod.Period.ProjectId))
                    .SelectMany(p => p.ProjectUsers.Select(u => u.Teacher))
                    .ToList();

            ViewBag.TeacherId = new SelectList(teachers, "pkey", "BigName", subgroup.TeacherId);
            ViewBag.CanEdit = db.ProjectSubgroupsForUser(User).Any(s => s.Id == id);
            return View(subgroup);
        }

        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult Edit(ProjectSubgroup subgroup)
        {
            if (ModelState.IsValid)
            {
                var dbEntry = db.ProjectSubgroups.Find(subgroup.Id);
                dbEntry.Limit = subgroup.Limit;
                if (User.IsInRole(ItsRoles.Admin))
                    dbEntry.Name = subgroup.Name;
                dbEntry.TeacherId = subgroup.TeacherId;
                db.SaveChanges();
                return RedirectToAction("Index",
                    new
                    {
                        programId = dbEntry.Id,
                        focus = subgroup.Id,
                        competitionGroupId = dbEntry.Meta.CompetitionGroupId
                    });
            }
            var teachers =
                db.ProjectProperties.Where(
                        p =>
                            (p.ProjectCompetitionGroupId == subgroup.Meta.CompetitionGroupId) &&
                            (p.ProjectId ==
                             db.ProjectSubgroups.FirstOrDefault(g => g.Id == subgroup.Id)
                                 .Meta.ProjectDisciplineTmerPeriod.Period.ProjectId))
                    .SelectMany(p => p.ProjectUsers) // TODO поправить!!!! доставать кураторов?
                    .ToList();

            ViewBag.TeacherId = new SelectList(teachers, "pkey", "BigName", subgroup.TeacherId);


            return View(subgroup);
        }

        public ActionResult FillGroupsWithStudents(int competitionGroupId)
        {
            var metas =
                db.ProjectSubgroupCounts.Include(x => x.ProjectDisciplineTmerPeriod)
                    .Where(m => m.CompetitionGroupId == competitionGroupId)
                    .Where(m => m.Subgroups.Count() > 0)
                    .Where(m => !db.ProjectSubgroups.Any(s => s.Parent.SubgroupCountId == m.Id))
                    .ToList();
            foreach (var meta in metas)
            {
                var toAdmitt = db.Students.Where(
                        s =>
                            db.ProjectAdmissions.Any(
                                ma =>
                                    (ma.studentId == s.Id) && (ma.ProjectCompetitionGroupId == competitionGroupId) &&
                                    (ma.ProjectId == meta.ProjectDisciplineTmerPeriod.Period.ProjectId) &&
                                    (ma.Status == AdmissionStatus.Admitted))
                    )
                    .Where(s => !db.ProjectSubgroupMemberships.Any(m => m.Subgroup.SubgroupCountId == meta.Id && m.studentId == s.Id))
                    .Select(s => s.Id).ToList();

                var subgroups =
                    db.ProjectSubgroups.Where(s => s.SubgroupCountId == meta.Id)
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
                    var existedSubgroup = db.ProjectSubgroupMemberships.Include(s => s.Subgroup).FirstOrDefault(m => m.studentId == studentId
                            && m.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.ProjectId == meta.ProjectDisciplineTmerPeriod.Period.ProjectId
                            && m.Subgroup.Meta.CompetitionGroupId == meta.CompetitionGroupId);

                    // порядковый номер группы, на которую надо зачислить студента 
                    var subgroup = subgroups.OrderBy(x => x.Admitted).ThenBy(x => x.InnerNumber)
                        .Where(s => s.InnerNumber == existedSubgroup?.Subgroup?.InnerNumber || existedSubgroup == null)
                        .FirstOrDefault()
                        ?? subgroups.OrderBy(x => x.Admitted).ThenBy(x => x.InnerNumber).First();

                    db.ProjectSubgroupMemberships.Add(new ProjectSubgroupMembership
                    {
                        studentId = studentId,
                        SubgroupId = subgroup.Id
                    });
                    Logger.Info($"Студент {studentId} распределен в подгруппу {subgroup.Id}");
                    subgroup.Admitted += 1;
                }

                db.SaveChanges();
            }


            return RedirectToAction("Index", new { competitionGroupId });
        }

        public ActionResult DownloadSubGroups(string filter, int competitionGroupId)
        {
            var subgroups = GetSectionSubgroups(null, filter, competitionGroupId);
            var stream = new VariantExport().Export(new { Rows = subgroups }, "minorSubgroupsTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Список подгрупп на проектах.xlsx".ToDownloadFileName());
        }

        public ActionResult Modules(int competitionGroupId)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var modules = db.ProjectSubgroups
                    .Where(
                        s =>
                            s.Meta.ProjectDisciplineTmerPeriod.ProjectSubgroupCounts.Any(
                                _ => _.CompetitionGroupId == competitionGroupId))
                    .Select(_ => new
                    {
                        Id = _.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.uuid,
                        Name = _.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title,
                        _.Meta.ProjectDisciplineTmerPeriod.Period.Year,
                        _.Meta.ProjectDisciplineTmerPeriod.Period.SemesterId
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

        public ActionResult StudentsCount(string filter, int competitionGroupId)
        {
            var subgroups =
                GetSectionSubgroups(null, filter, competitionGroupId)
                    .ToList()
                    .Select(v => v.GetPropertyValue<int>("Id"))
                    .SelectMany(v =>
                            db.ProjectSubgroupMemberships.Where(
                                sm =>
                                    (sm.SubgroupId == v) || (sm.Subgroup.ParentId == v) ||
                                    (sm.Subgroup.Parent.ParentId == v))
                    ).Distinct().Count(); //GroupBy(_=>_.studentId).Count();
            return JsonNet(subgroups);
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
                db.ProjectSubgroupMemberships.Where(
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
                db.ProjectSubgroups.Where(s => s.Id == subgroupId)
                    .Select(s => s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Project.Module.title)
                    .FirstOrDefault();
            var subgroup = db.ProjectSubgroups.Where(s => s.Id == subgroupId).Select(s => s.Name).FirstOrDefault();
            var stream = new VariantExport().Export(new { Rows = reportVms, Title = title, Subgroup = subgroup },
                "subgroupReportTemplate.xlsx");
            return stream;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult Delete([FromBody] int[] ids)
        {
            var toDelete = db.ProjectSubgroups.Include(_ => _.Meta.CompetitionGroup).Where(s => ids.Contains(s.Id));
            foreach (var subgroup in toDelete)
            {
                Logger.Info($"Удалена подгруппа {subgroup.Id} {subgroup.Name} в конкурсной группе {subgroup.Meta.CompetitionGroupId} {subgroup.Meta.CompetitionGroup.Name} сокращ. название");

                db.ProjectSubgroups.Remove(subgroup);

            }
            db.SaveChanges();


            return JsonNet("Ok");
        }

        public ActionResult SimpleCopyMembership(int srcId, int dstId, bool withTeacher)
        {
            var src = db.ProjectSubgroups.Include(s => s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(_ => _.Id == srcId);
            var dst = db.ProjectSubgroups.Include(s => s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(s => s.Id == dstId);

            int cnt = 0;
            if (src.Id != dst.Id)
            {
                if (src.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer ==
                    dst.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer)
                {
                    return RedirectToAction("Index", new { competitionGroupId = src.Meta.CompetitionGroupId });
                }
                var srcStudents = src.Students.Select(_ => _.studentId).ToList();

                var students = db.ProjectSubgroups.Where(_ => _.SubgroupCountId == dst.SubgroupCountId &&
                                                                    _.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer ==
                                                                    dst.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer
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
                    dst.Students.Add(new ProjectSubgroupMembership
                    {
                        SubgroupId = dst.Id,
                        studentId = student.studentId
                    });
                    Logger.Info($"Студент {student.studentId} скопирован в подгруппу {dst.Id}");
                }
                db.SaveChanges();
            }
            return Json(new
            {
                success = true,
                message = "Скопировано " + cnt + (cnt < 5 && cnt > 0 ? " зачисления " : " зачислений"),
                competitionGroupId = src.Meta.CompetitionGroupId,
                focus = dstId
            });//, "text/html", Encoding.Unicode);
        }

        public ActionResult Students(int id, int? page, int? limit, string sort, string filter)
        {
            var subgroup =
                db.ProjectSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.ProjectDisciplineTmerPeriod)
                    .Include(x => x.Meta.ProjectDisciplineTmerPeriod.Period)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == id);
            ViewBag.Subgroup = subgroup;
            ViewBag.CanEdit = db.ProjectSubgroupsForUser(User).Any(s => s.Id == id);
            return View(subgroup);
        }

        public ActionResult StudentsAjax(int id, bool hideStudents, int? page, int? limit, string sort, string filter)
        {
            var subgroup =
                db.ProjectSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.ProjectDisciplineTmerPeriod)
                    .Include(x => x.Meta.ProjectDisciplineTmerPeriod.Period)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == id);

            var students = GetProjectSubgroupsStudents(id, sort, filter, subgroup, hideStudents);

            var paginated = students; //.ToPagedList(page ?? 1, limit ?? 25);
            return JsonNet(new
            {
                data = paginated,
                total = students.Count()
            });
        }

        private IQueryable<object> GetProjectSubgroupsStudents(int id, string sort, string filter,
            ProjectSubgroup subgroup, bool hideStudents = false)
        {
            var query = db.Students
                .Where(s => db.ProjectAdmissions.Any(ma => (ma.studentId == s.Id)
                                                             && (ma.Status == AdmissionStatus.Admitted)
                                                             &&
                                                             (ma.ProjectCompetitionGroupId ==
                                                              subgroup.Meta.CompetitionGroupId)
                                                             &&
                                                             (ma.ProjectId ==
                                                              subgroup.Meta.ProjectDisciplineTmerPeriod.Period
                                                                  .ProjectId))
                    && (!hideStudents || hideStudents && (s.Status == "Активный" || s.Status == "Отп.с.посещ." || s.Status == "Отп.дород.послерод."))
                    )
                .Select(s => new
                {
                    Student = s,
                    s.Person,
                    GroupName = db.GroupsHistories.FirstOrDefault(g => g.GroupId == s.GroupId && g.YearHistory == subgroup.Meta.CompetitionGroup.Year).Name,
                    SubgroupAdm = db.ProjectSubgroupMemberships.FirstOrDefault(m => (m.studentId == s.Id)
                                                                        &&
                                                                        //m.Subgroup.SubgroupCountId == subgroup.SubgroupCountId),
                                                                        ((m.SubgroupId == id) ||
                                                                         (m.Subgroup.ParentId == id) ||
                                                                         (m.Subgroup.Parent.ParentId == id))),
                    AnotherSubgroupGroup = db.ProjectSubgroupMemberships.FirstOrDefault(m => (m.studentId == s.Id)
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
                                                                                                     .ProjectDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tlekc") ||
                                                                                                (m.Subgroup.Meta
                                                                                                     .ProjectDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tprak") ||
                                                                                                (m.Subgroup.Meta
                                                                                                     .ProjectDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "prz") ||
                                                                                                (m.Subgroup.Meta
                                                                                                     .ProjectDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tlab"))
                                                                                               &&
                                                                                               ((subgroup.Meta
                                                                                                     .ProjectDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tlekc") ||
                                                                                                (subgroup.Meta
                                                                                                     .ProjectDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "tprak") ||
                                                                                                (subgroup.Meta
                                                                                                     .ProjectDisciplineTmerPeriod
                                                                                                     .Tmer.Tmer.kmer ==
                                                                                                 "prz") ||
                                                                                                (subgroup.Meta
                                                                                                     .ProjectDisciplineTmerPeriod
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
                r.Student.Status,
                Included = r.SubgroupAdm != null,
                AnotherGroup = r.AnotherSubgroupGroup.Subgroup.Name,
                //Teacher = r.SubgroupAdm == null ?
                //    r.AnotherSubgroupGroup.Subgroup.Teacher.initials :
                //    r.SubgroupAdm.Subgroup.Teacher.initials

            });

            SortRules sortRules = SortRules.Deserialize(sort);
            students = students.OrderByThenBy(sortRules.FirstOrDefault(), v => v.Surname, v => v.Name,
                v => v.PatronymicName);

            students = students.Where(FilterRules.Deserialize(filter));
            return students;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult StudentsMembership(bool include, int subgroupId, string filter)
        {
            var subgroup =
                db.ProjectSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.ProjectDisciplineTmerPeriod)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == subgroupId);
            if (subgroup == null) return NotFound("ProjectSubgroup not found");

            var subgroupStudents = GetProjectSubgroupsStudents(subgroupId, null, filter, subgroup).ToList().Where(s =>
            {
                var anotherGroup = s.GetPropertyValue<string>("AnotherGroup");
                return string.IsNullOrEmpty(anotherGroup);
            }).ToList();

            subgroupStudents.ForEach(
                s => { StudentMembershipInternal(include, s.GetPropertyValue<string>("Id"), subgroupId); });
            var curStudentsCount = db.ProjectSubgroupMemberships.Count(m => m.SubgroupId == subgroupId);

            string msg = "";
            if ((subgroup.Limit < curStudentsCount) && include)
                msg = "Превышен лимит";
            
            return Json(new { msg, reload = false });
        }

        private void StudentMembershipInternal(bool include, string studentId, int subgroupId)
        {
            var subgroup = db.ProjectSubgroups.Include(s => s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(s => s.Id == subgroupId);

            var subgroupModuleType = subgroup.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type;
            
            var existsSubgroups =
                db.ProjectSubgroupMemberships.FirstOrDefault(
                    m => (m.studentId == studentId)
                        && m.Subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer == subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer
                        && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId
                        && m.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type == subgroupModuleType);
            
            if (existsSubgroups == null) // если студент не содержится ни в одной подгруппе
            {
                if (include)
                {
                    AddMembership(new ProjectSubgroupMembership
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

                        AddMembership(new ProjectSubgroupMembership
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
        private void RemoveMembership(ProjectSubgroupMembership membership)
        {
            db.ProjectSubgroupMemberships.Remove(membership);
            Logger.Info($"Студент {membership.studentId} удален из подгруппы проекта {membership.SubgroupId}");

            db.SaveChanges();
        }

        private void AddMembership(ProjectSubgroupMembership membership)
        {
            db.ProjectSubgroupMemberships.Add(membership);
            Logger.Info($"Студент {membership.studentId} добавлен в подгруппу проекта {membership.SubgroupId}");

            db.SaveChanges();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult StudentMembership(bool include, string studentId, int subgroupId)
        {
            StudentMembershipInternal(include, studentId, subgroupId);
            var msg = "";
            var subgroup = db.ProjectSubgroups.FirstOrDefault(m => m.Id == subgroupId);

            if (!include)
            {
                var similarsubgroup = db.ProjectSubgroupMemberships.Include(s => s.Subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer)
                                        .FirstOrDefault(m => (m.studentId == studentId)
                                       && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId
                                       && m.Subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.TmerId
                                       != subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.TmerId);

                if (similarsubgroup != null)
                {
                    var rmer = similarsubgroup.Subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.TmerId == "tprak" ?
                       "практическим занятиям" : "зачету";
                    return Json(new { rmer, subgroupId = similarsubgroup.SubgroupId });
                }

            }

            var curStudentsCount = db.ProjectSubgroupMemberships.Count(m => m.SubgroupId == subgroupId);
            if ((subgroup?.Limit < curStudentsCount) && include)
                msg = "Превышен лимит";
            return Json(new { msg });
        }

        public ActionResult DownloadSubgroupReport(int subgroupId)
        {
            var stream = PrepareSubgroupReportStream(subgroupId);
            var firstOrDefault = db.ProjectSubgroups.Include(_ => _.Teacher)
                .FirstOrDefault(s => s.Id == subgroupId);
            var title =
                $"{firstOrDefault.Name}"; // {firstOrDefault?.Teacher?.initials}";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ("Отчёт по подгруппе " + title + ".xlsx").ToDownloadFileName());
        }

        public ActionResult GetProjectsTeachers(string moduleIds, int competitionGroupId)
        {
            var selectedprojects = JsonConvert.DeserializeObject<List<string>>(moduleIds).Distinct();

               List<Teacher> finallist = new List<Teacher>();

                 var projects=  db.ProjectProperties.Where(p =>
                       (p.ProjectCompetitionGroupId == competitionGroupId) &&
                       selectedprojects.Contains(p.ProjectId)).ToList();

                 foreach (var p in projects)
                 {
                     var teachers = p.ProjectUsers
                              .Select(pr => pr.Teacher).ToList();
                     finallist = !finallist.Any() ? teachers : teachers.Intersect(finallist).ToList();
                     if(!finallist.Any()) break;
                 }

                 return JsonNet(new
            {
                data = finallist.Select(t=>new{t.pkey,name =t.FullName,t.post,t.workPlace})
            });

        }


        public ActionResult SetCuratorToSubgroups(int[] subgroupIds, string teacherId)
        {
            if (!string.IsNullOrEmpty(teacherId) && !db.Teachers.Any(t => t.pkey == teacherId)) return NotFound("Teacher not found");

            foreach(var sp in db.ProjectSubgroups.Where(s => subgroupIds.Contains(s.Id))) sp.TeacherId = teacherId;
            /*db.ProjectSubgroups.Where(s => subgroupIds.Contains(s.Id)).ForEach(s =>
            {
                s.TeacherId = teacherId;

            });*/
            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }


        public ActionResult SetLimitToSubgroups(int[] subgroupIds, int? limit)
        {
            if (limit != null)
            {
                foreach(var sp in db.ProjectSubgroups.Where(s => subgroupIds.Contains(s.Id))) sp.Limit = (int)limit;
/*                db.ProjectSubgroups.Where(s => subgroupIds.Contains(s.Id)).ForEach(s =>
                {
                    s.Limit = (int)limit;

                });*/
                db.SaveChanges();
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

    }
}