using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.MUPManager)]
    public class MUPSubgroupController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int competitionGroupId, int? page, int? limit, string sort, string filter,
            string message = null, int? focus = null)
        {
            ViewBag.Message = message;
            ViewBag.Focus = focus;
            var competitionGroup = db.MUPCompetitionGroups.FirstOrDefault(g => g.Id == competitionGroupId);
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
            var subgroups =
                GetSectionSubgroups(null, filter, competitionGroupId)
                    .ToList()
                    .Select(v => v.GetPropertyValue<int>("Id"))
                    .SelectMany(v =>
                            db.MUPSubgroupMemberships.Where(
                                sm =>
                                    (sm.SubgroupId == v) || (sm.Subgroup.ParentId == v) ||
                                    (sm.Subgroup.Parent.ParentId == v))
                    ).Distinct().Count();
            return JsonNet(subgroups);
        }

        private IQueryable<object> GetSectionSubgroups(string sort, string filter, int competitionGroupId)
        {
            var competitionGroup = db.MUPCompetitionGroups.FirstOrDefault(_ => _.Id == competitionGroupId);

            var subgroups = db.MUPSubgroups
                .Where(s => s.Meta.CompetitionGroupId == competitionGroupId)
                .Select(v => new
                {
                    v.Id,
                    Name = v.Removed ? "(удалено) " + v.Name : v.Name,
                    v.InnerNumber,
                    HasScores = v.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.kgmer == 3,
                    moduleId = v.Meta.MUPDisciplineTmerPeriod.Period.MUPId,
                    ModuleTitle = v.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.title,
                    Year = v.Meta.MUPDisciplineTmerPeriod.Period.Year.ToString(),
                    Semester = v.Meta.MUPDisciplineTmerPeriod.Period.Semester.Name.ToString(),
                    semesterId = v.Meta.MUPDisciplineTmerPeriod.Period.Semester.Id,
                    subgroupType = v.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    kgmer = v.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.kgmer.ToString(),
                    v.Limit,
                    count =
                    db.MUPSubgroupMemberships.Where(sm => (sm.SubgroupId == v.Id))
                        .Concat(db.MUPSubgroupMemberships.Where(sm => (sm.Subgroup.ParentId == v.Id)))
                        .Concat(db.MUPSubgroupMemberships.Where(sm => (sm.Subgroup.Parent.ParentId == v.Id)))
                        .Count(),
                    teacher = v.Teacher.initials,
                    teachers = v.Teachers.Select(t => t.Teacher.initials),
                    description = v.Description
                });

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

            subgroups = subgroups.Where(FilterRules.Deserialize(filter));
            return subgroups;
        }

        public ActionResult Modules(int competitionGroupId)
        {
            var modules = db.MUPSubgroups
                .Where(
                    s =>
                        s.Meta.MUPDisciplineTmerPeriod.MUPSubgroupCounts.Any(
                            _ => _.CompetitionGroupId == competitionGroupId))
                .Select(_ => new
                {
                    Id = _.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.uuid,
                    Name = _.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.title,
                    _.Meta.MUPDisciplineTmerPeriod.Period.Year,
                    _.Meta.MUPDisciplineTmerPeriod.Period.SemesterId
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

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var subgroup = db.MUPSubgroups.Find(id);
            if (subgroup == null)
                return NotFound();

            var teachers =
                db.MUPProperties.Where(
                        p =>
                            (p.MUPCompetitionGroupId == subgroup.Meta.CompetitionGroupId) &&
                            (p.MUPId ==
                             db.MUPSubgroups.FirstOrDefault(g => g.Id == id)
                                 .Meta.MUPDisciplineTmerPeriod.Period.MUPId))
                    .SelectMany(p => p.Teachers)
                    .ToList();

            ViewBag.TeacherId = new SelectList(teachers, "pkey", "BigName", subgroup.TeacherId);
            return View(subgroup);
        }

        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult Edit(MUPSubgroup subgroup)
        {
            if (ModelState.IsValid)
            {
                var dbEntry = db.MUPSubgroups.Find(subgroup.Id);
                dbEntry.Limit = subgroup.Limit;
                if (User.IsInRole(ItsRoles.Admin))
                    dbEntry.Name = subgroup.Name;
                dbEntry.TeacherId = subgroup.TeacherId;
                dbEntry.Description = subgroup.Description;
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
                db.MUPProperties.Where(
                        p =>
                            (p.MUPCompetitionGroupId == subgroup.Meta.CompetitionGroupId) &&
                            (p.MUPId ==
                             db.MUPSubgroups.FirstOrDefault(g => g.Id == subgroup.Id)
                                 .Meta.MUPDisciplineTmerPeriod.Period.MUPId))
                    .SelectMany(p => p.Teachers)
                    .ToList();

            ViewBag.TeacherId = new SelectList(teachers, "pkey", "BigName", subgroup.TeacherId);


            return View(subgroup);
        }

        public ActionResult Students(int id, int? page, int? limit, string sort, string filter)
        {
            var subgroup =
                db.MUPSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.MUPDisciplineTmerPeriod)
                    .Include(x => x.Meta.MUPDisciplineTmerPeriod.Period)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == id);
            ViewBag.Subgroup = subgroup;
            return View(subgroup);
        }

        public ActionResult StudentsAjax(int id, bool hideStudents, int? page, int? limit, string sort, string filter)
        {
            var subgroup =
                db.MUPSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.MUPDisciplineTmerPeriod)
                    .Include(x => x.Meta.MUPDisciplineTmerPeriod.Period)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == id);

            var students = GetMUPSubgroupsStudents(id, sort, filter, subgroup, hideStudents);

            var paginated = students;
            return JsonNet(new
            {
                data = paginated,
                total = students.Count()
            });
        }

        private IQueryable<object> GetMUPSubgroupsStudents(int id, string sort, string filter,
            MUPSubgroup subgroup, bool hideStudents = false)
        {
            var query = db.Students
                .Where(s => db.MUPAdmissions.Any(ma => (ma.studentId == s.Id) && (ma.Status == AdmissionStatus.Admitted)
                        && (ma.MUPCompetitionGroupId == subgroup.Meta.CompetitionGroupId)
                        && (ma.MUPId == subgroup.Meta.MUPDisciplineTmerPeriod.Period.MUPId))
                        && (!hideStudents || hideStudents && (s.Status == "Активный" || s.Status == "Отп.с.посещ." || s.Status == "Отп.дород.послерод."))
                    )
                .Select(s => new
                {
                    Student = s,
                    s.Person,
                    GroupName = db.GroupsHistories.FirstOrDefault(g => g.GroupId == s.GroupId && g.YearHistory == subgroup.Meta.CompetitionGroup.Year 
                            && (g.Course == subgroup.Meta.CompetitionGroup.StudentCourse || subgroup.Meta.CompetitionGroup.StudentCourse == 0)).Name,
                    Included = db.MUPSubgroupMemberships.Any(m => (m.studentId == s.Id)
                                                                              &&
                                                                              ((m.SubgroupId == id) ||
                                                                               (m.Subgroup.ParentId == id) ||
                                                                               (m.Subgroup.Parent.ParentId == id))),
                    AnotherGroup = db.MUPSubgroupMemberships.FirstOrDefault(m => (m.studentId == s.Id)
                                                                                             && (
                                                                                                 ((m.Subgroup.Meta.Id ==
                                                                                                   subgroup.Meta.Id) &&
                                                                                                  (m.Subgroup.Id !=
                                                                                                   subgroup.Id))
                                                                                                 //или та же нагрузка 
                                                                                                 || (
                                                                                                     (m.Subgroup.Meta.Id ==
                                                                                                      subgroup.Meta.Id)
                                                                                                     //или я на том же мупе, но зачислен не в эту группу, а куда то на л, п, или пр
                                                                                                     &&
                                                                                                     ((m.Subgroup.Meta
                                                                                                           .MUPDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tlekc") ||
                                                                                                      (m.Subgroup.Meta
                                                                                                           .MUPDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tprak") ||
                                                                                                      (m.Subgroup.Meta
                                                                                                           .MUPDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "prz") ||
                                                                                                      (m.Subgroup.Meta
                                                                                                           .MUPDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tlab"))
                                                                                                     &&
                                                                                                     ((subgroup.Meta
                                                                                                           .MUPDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tlekc") ||
                                                                                                      (subgroup.Meta
                                                                                                           .MUPDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "tprak") ||
                                                                                                      (subgroup.Meta
                                                                                                           .MUPDisciplineTmerPeriod
                                                                                                           .Tmer.Tmer
                                                                                                           .kmer ==
                                                                                                       "prz") ||
                                                                                                      (subgroup.Meta
                                                                                                           .MUPDisciplineTmerPeriod
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
                db.MUPSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.MUPDisciplineTmerPeriod)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == subgroupId);
            if (subgroup == null) return NotFound("MUPSubgroup not found");
            var similarsubgroups = FindSubgroups(subgroupId, subgroup);

            var msg = "";
            if (similarsubgroups.Count != 0)
            {
                msg = subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.TmerId == "tprak" ?
                   "зачету" : "практическим занятиям";
            }
            return Json(new { msg });
        }

        private List<MUPSubgroupMembership> FindSubgroups(int subgroupId, MUPSubgroup subgroup)
        {
            var subgroupStudents = GetMUPSubgroupsStudents(subgroupId, null, null, subgroup).ToList()
                .Where(s => s.GetPropertyValue<bool>("Included") == true);

            List<MUPSubgroupMembership> similarsubgroups = new List<MUPSubgroupMembership>();
            foreach (var s in subgroupStudents)
            {
                var studentId = s.GetPropertyValue<string>("Id");
                var ssubgroup = db.MUPSubgroupMemberships.Include(t => t.Subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer)
                  .FirstOrDefault(m => m.studentId == studentId
                    && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId
                    && m.Subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.TmerId != subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.TmerId);
                if (ssubgroup != null)
                    similarsubgroups.Add(ssubgroup);
            }
            return similarsubgroups;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult StudentsMembership(bool include, int subgroupId, string filter, bool similarsubgroup)
        {
            var subgroup =
                db.MUPSubgroups.Include(x => x.Meta)
                    .Include(x => x.Meta.MUPDisciplineTmerPeriod)
                    .Include(x => x.Meta.CompetitionGroup)
                    .FirstOrDefault(_ => _.Id == subgroupId);
            if (subgroup == null) return NotFound("MUPSubgroup not found");

            var subgroupStudents =
                GetMUPSubgroupsStudents(subgroupId, null, filter, subgroup).ToList().Where(s =>
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
            bool reload = false;
            string msg = "";
            var curStudentsCount = db.MUPSubgroupMemberships.Count(m => m.SubgroupId == subgroupId);
            if ((subgroup?.Limit < curStudentsCount) && include)
                msg = "Превышен лимит";
            return Json(new { msg, reload });
        }

        private void StudentMembershipInternal(bool include, string studentId, int subgroupId)
        {
            var subgroup = db.MUPSubgroups.Include(s => s.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(s => s.Id == subgroupId);

            var existsSubgroups =
                db.MUPSubgroupMemberships.FirstOrDefault(
                    m => (m.studentId == studentId)
                        && m.Subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.kmer == subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.kmer
                        && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId);

            if (existsSubgroups == null) // если студент не содержится ни в одной подгруппе
            {
                if (include)
                {
                    AddMembership(new MUPSubgroupMembership
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

                        AddMembership(new MUPSubgroupMembership
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

        private void RemoveMembership(MUPSubgroupMembership membership)
        {
            db.MUPSubgroupMemberships.Remove(membership);
            Logger.Info($"Студент {membership.studentId} удален из подгруппы {membership.SubgroupId} МУПа");
            db.SaveChanges();
        }

        private void AddMembership(MUPSubgroupMembership membership)
        {
            db.MUPSubgroupMemberships.Add(membership);
            Logger.Info($"Студент {membership.studentId} добавлен в подгруппу {membership.SubgroupId} МУПа");

            db.SaveChanges();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult StudentMembership(bool include, string studentId, int subgroupId)
        {
            StudentMembershipInternal(include, studentId, subgroupId);
            var msg = "";
            var subgroup = db.MUPSubgroups.FirstOrDefault(m => m.Id == subgroupId);

            if (!include)
            {
                var similarsubgroup = db.MUPSubgroupMemberships.Include(s => s.Subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer)
                                        .FirstOrDefault(m => (m.studentId == studentId)
                                       && m.Subgroup.Meta.CompetitionGroupId == subgroup.Meta.CompetitionGroupId
                                       && m.Subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.TmerId
                                       != subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.TmerId);

                if (similarsubgroup != null)
                {
                    var rmer = similarsubgroup.Subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.TmerId == "tprak" ?
                       "практическим занятиям" : "зачету";
                    return Json(new { rmer, subgroupId = similarsubgroup.SubgroupId });
                }

            }

            var curStudentsCount = db.MUPSubgroupMemberships.Count(m => m.SubgroupId == subgroupId);
            if ((subgroup?.Limit < curStudentsCount) && include)
                msg = "Превышен лимит";
            return Json(new { msg });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult Delete([FromBody] int[] ids)
        {
            db.MUPSubgroups.RemoveRange(db.MUPSubgroups.Where(s => ids.Contains(s.Id)));
            db.SaveChanges();
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
                db.MUPSubgroupMemberships.Where(
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
                db.MUPSubgroups.Where(s => s.Id == subgroupId)
                    .Select(s => s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.MUP.Module.title)
                    .FirstOrDefault();
            var Subgroup =
                db.MUPSubgroups.Where(s => s.Id == subgroupId).Select(s => s.Name).FirstOrDefault();
            var stream = new VariantExport().Export(new { Rows = reportVms, Title, Subgroup },
                "subgroupReportTemplate.xlsx");
            return stream;
        }


        public ActionResult SimpleCopyMembership(int srcId, int dstId, bool withTeacher)
        {
            var src = db.MUPSubgroups.Include(s => s.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(_ => _.Id == srcId);
            var dst = db.MUPSubgroups.Include(s => s.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer).FirstOrDefault(s => s.Id == dstId);

            int cnt = 0;
            if (src.Id != dst.Id)
            {
                if (src.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.rmer ==
                    dst.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.rmer)
                {
                    return RedirectToAction("Index", new { competitionGroupId = src.Meta.CompetitionGroupId });
                }
                var srcStudents = src.Students.Select(_ => _.studentId).ToList();

                var students = db.MUPSubgroups.Where(_ => _.SubgroupCountId == dst.SubgroupCountId &&
                                                                    _.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.rmer ==
                                                                    dst.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer
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
                    dst.Students.Add(new MUPSubgroupMembership
                    {
                        SubgroupId = dst.Id,
                        studentId = student.studentId
                    });
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

        public ActionResult DownloadSubgroupReport(int subgroupId)
        {
            var subgroup = db.MUPSubgroups.Find(subgroupId);
            var stream = PrepareSubgroupReportStream(subgroupId);

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ("Отчёт по подгруппе " + subgroup.Name + ".xlsx").ToDownloadFileName());
        }
    }
}