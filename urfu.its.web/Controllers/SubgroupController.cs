using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList.Core;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using Urfu.Its.Web.Pubs;
using Newtonsoft.Json;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.StudentAdmissionRead)]
    public class SubgroupController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subgroup

        public ActionResult Index(int programId, int? page, int? limit, string sort, string filter, string message = null, int? focus = null)
        {
            ViewBag.Message = message;
            ViewBag.Focus = focus;

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                db.Database.SetCommandTimeout(60);
                var eduprograms = db.SubgroupsForUser(User).Include(s => s.Meta.Module).Include(s => s.Parent).Include(s => s.Meta.Group).Where(p => p.Meta.programId == programId).Select(v => new
                {
                    v.Id,
                    v.Name,
                    ModuleTitle = v.Meta.Module.title,
                    GroupName = v.Meta.Group.Name,
                    Term = v.Meta.Term.ToString(),
                    Year = v.Meta.Year,
                    subgroupType = v.Meta.Tmer.rmer,
                    kgmer = v.Meta.Tmer.kgmer.ToString(),
                    v.Limit,
                    count = db.SubgroupMemberships.Count(sm => sm.SubgroupId == v.Id )
                    + db.SubgroupMemberships.Count(sm => sm.Subgroup.ParentId == v.Id )
                    + db.SubgroupMemberships.Count(sm => sm.Subgroup.Parent.ParentId == v.Id),
                });

                SortRules sortRules = SortRules.Deserialize(sort);
                eduprograms = eduprograms.OrderByThenBy(sortRules.FirstOrDefault(), v => v.GroupName, v => v.ModuleTitle, v => v.Name, v => v.Term, v => v.kgmer, v => v.subgroupType);

                eduprograms = eduprograms.Where(FilterRules.Deserialize(filter));

                var paginated = eduprograms;//.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = eduprograms.Count()
                });
            }
            ViewBag.programId = programId;
            var program = db.EduPrograms.Find(programId);
            if (program != null)
                ViewBag.programInfo = program.FullName;
            return View();
        }

        public ActionResult Disciplines(int programId)
        {
            var subgroups = db.MetaSubgroups.Where(m => m.programId == programId);
            var titles = subgroups.Select(v => new
            {
                plan = db.Plans.FirstOrDefault(p => p.disciplineUUID == v.disciplineUUID
                                             && p.directionId == v.Program.directionId
                                             && p.qualification == v.Program.qualification
                                             && p.familirizationCondition == v.Program.familirizationCondition
                                             && p.versionNumber == v.Program.PlanVersionNumber
                                             && p.eduplanNumber == v.Program.PlanNumber
                                             && p.familirizationType == v.Program.familirizationType
                                             && p.active)
            }).Select(v => new { id = v.plan.catalogDisciplineUUID, name = v.plan.disciplineTitle }).Distinct().OrderBy(c => c.name);

            return JsonNet(new
            {
                data = titles,
                total = titles.Count()
            });
        }

        // GET: Subgroup/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Subgroup subgroup = db.Subgroups.Find(id);
            if (subgroup == null)
            {
                return NotFound();
            }
            return View(subgroup);
        }

        public static readonly string[] SpectialSubgroupTypes = new[] { "tlekc", "tprak", "tlab" };

        // GET: Subgroup/Create
        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult Create(int programId)
        {
            var program = db.EduPrograms.Find(programId);

            var subgroups = db.MetaSubgroups.Where(m => m.programId == programId);
            var metaSubgroups = subgroups.OrderByDescending(x => x.kmer == "tlekc").ThenBy(x => x.kmer == "tlab").ThenBy(x => x.kmer == "tprak").Include(m => m.Group).ToList();
            var titles = subgroups.Select(v => new
            {
                v.Id,
                db.Plans.FirstOrDefault(p => p.disciplineUUID == v.disciplineUUID
                                             && p.directionId == v.Program.directionId
                                             && p.qualification == v.Program.qualification
                                             && p.familirizationCondition == v.Program.familirizationCondition
                                             && p.versionNumber == v.Program.PlanVersionNumber
                                             && p.eduplanNumber == v.Program.PlanNumber
                                             && p.familirizationType == v.Program.familirizationType
                                             && p.active).disciplineTitle
            }).ToDictionary(v => v.Id, v => v.disciplineTitle);

            foreach (var meta in metaSubgroups)
            {
                var distribution = meta.ExtractDistribution();
                var busyNumbers = new HashSet<int>(db.Subgroups.Where(s => !s.Removed && s.MetaSubgroupId == meta.Id).Select(s => s.InnerNumber));

                var exists = busyNumbers.Count();
                for (; exists < meta.Count; exists++)
                {
                    var innerNumber = FindInnerNumber(busyNumbers);
                    busyNumbers.Add(innerNumber);

                    if (meta.Tmer == null)
                        continue;

                    var grp = new Subgroup
                    {
                        Name = meta.Group.Name + "\\" + titles[meta.Id] + "\\" + meta.Tmer.rmer + "\\" + innerNumber,
                        Limit = (int)Math.Ceiling(program.Variant.StudentsLimit / (double)meta.Count),
                        MetaSubgroupId = meta.Id,
                        InnerNumber = innerNumber,
                        ExpectedChildCount = distribution[(innerNumber - 1) % distribution.Length]
                    };

                    if (meta.kmer == "tlekc")
                    {
                        grp.Name = meta.Group.Name + "\\" + titles[meta.Id] + "\\л" + innerNumber;
                    }

                    if (meta.kmer == "tprak")
                    {
                        grp.Limit = 25;
                        var parent = db.Subgroups.Where(
                            s => !s.Removed &&
                                s.Meta.kmer == "tlekc" && s.Meta.Term == meta.Term && s.Meta.programId == meta.programId &&
                                s.Meta.disciplineUUID == meta.disciplineUUID &&
                                s.Meta.moduleId == meta.moduleId && s.Meta.groupId == meta.groupId)  
                            .OrderBy(s => db.Subgroups.Count(sx => !sx.Removed && sx.ParentId == s.Id) / (s.ExpectedChildCount ?? 1)).ThenBy(s => s.Name).FirstOrDefault();
                        if (parent != null)
                        {
                            grp.ParentId = parent.Id;
                            grp.Name = parent.Name + "\\п" + innerNumber;
                        }
                        else
                        {
                            grp.Name = meta.Group.Name + "\\" + titles[meta.Id] + "\\п" + innerNumber;
                        }
                    }

                    if (meta.kmer == "tlab")
                    {
                        grp.Limit = 12;
                        var parent = db.Subgroups.Where(
                            s => !s.Removed &&
                                s.Meta.kmer == "tprak" && s.Meta.Term == meta.Term && s.Meta.programId == meta.programId &&
                                s.Meta.disciplineUUID == meta.disciplineUUID &&
                                s.Meta.moduleId == meta.moduleId && s.Meta.groupId == meta.groupId)
                            .OrderBy(s => db.Subgroups.Count(sx => !sx.Removed && sx.ParentId == s.Id)).ThenBy(s => s.Name).FirstOrDefault()

                            ??

                        db.Subgroups.Where(
                            s => !s.Removed &&
                                s.Meta.kmer == "tlekc" && s.Meta.Term == meta.Term && s.Meta.programId == meta.programId &&
                                s.Meta.disciplineUUID == meta.disciplineUUID &&
                                s.Meta.moduleId == meta.moduleId && s.Meta.groupId == meta.groupId)
                            .OrderBy(s => db.Subgroups.Count(sx => !sx.Removed && sx.ParentId == s.Id)).ThenBy(s => s.Name).FirstOrDefault();

                        if (parent != null)
                        {
                            grp.ParentId = parent.Id;
                            grp.Name = parent.Name + "\\лаб" + innerNumber;
                        }
                        else
                        {
                            grp.Name = meta.Group.Name + "\\" + titles[meta.Id] + "\\лаб" + innerNumber;
                        }
                    }

                    db.Subgroups.Add(grp);

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", new { programId });
        }

        private int FindInnerNumber(HashSet<int> busyNumbers)
        {
            int candidate = 1;
            while (busyNumbers.Contains(candidate))
                candidate++;
            return candidate;
        }

        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult CopyMembership(int programId, int subgroupId, int targetTerm, string disciplineId)
        {
            var program = db.EduPrograms.Find(programId);

            var subgroup = db.Subgroups.Find(subgroupId);

            string disciplineUUID;
            if (db.Plans.Any(p => p.disciplineUUID == disciplineId))
                disciplineUUID = disciplineId;
            else
                disciplineUUID = subgroup.Meta.disciplineUUID;
            var source = db.MetaSubgroups.Where(
                m =>
                    m.programId == subgroup.Meta.programId && m.Term == subgroup.Meta.Term &&
                    m.disciplineUUID == disciplineUUID && m.moduleId == subgroup.Meta.moduleId &&
                    m.groupId == subgroup.Meta.groupId).ToList();

            foreach (var srcMeta in source)
            {
                if (db.SubgroupMemberships.Count(m => m.Subgroup.MetaSubgroupId == srcMeta.Id) == 0)
                    continue;

                var dstMetas = db.MetaSubgroups.Where(
                    m =>
                        m.programId == subgroup.Meta.programId && m.Term == targetTerm &&
                        m.disciplineUUID == disciplineUUID && m.moduleId == subgroup.Meta.moduleId &&
                        m.groupId == subgroup.Meta.groupId && m.kmer == srcMeta.kmer);
                if (dstMetas.Count() != 1)
                    continue;
                var dstMeta = dstMetas.First();

                if (srcMeta.Id == dstMeta.Id)
                    continue;

                var srcSubroups = db.Subgroups.Where(s => !s.Removed && s.MetaSubgroupId == srcMeta.Id).OrderBy(s => s.Name).Include(s => s.Students).ToList();
                var dstSubroups = db.Subgroups.Where(s => !s.Removed && s.MetaSubgroupId == dstMeta.Id).OrderBy(s => s.Name).Include(s => s.Students).ToList();
                if (srcSubroups.Count != dstSubroups.Count)
                    continue;
                for (int i = 0; i < srcSubroups.Count; i++)
                {
                    dstSubroups[i].Students.Clear();
                    foreach (var student in srcSubroups[i].Students)
                    {
                        dstSubroups[i].Students.Add(new SubgroupMembership
                        {
                            SubgroupId = srcSubroups[i].Id,
                            studentId = student.studentId
                        });
                    }
                }
            }

            db.SaveChanges();

            return RedirectToAction("Index", new { programId, focus = subgroupId });
        }

        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult SimpleCopyMembership(int srcId, int dstId)
        {
            var src = db.Subgroups.Find(srcId);
            var dst = db.SubgroupsForUser(User).First(s => s.Id == dstId);

            int cnt = 0;
            if (src.Id != dst.Id)
            {
                dst.Students.Clear();


                foreach (var student in src.Students.Where(s => !db.SubgroupMemberships.Any(sm => sm.studentId == s.studentId && sm.Subgroup.MetaSubgroupId == dst.MetaSubgroupId)))
                {
                    cnt++;
                    dst.Students.Add(new SubgroupMembership
                    {
                        SubgroupId = dst.Id,
                        studentId = student.studentId
                    });
                    SubgroupPublication.PublishMember(dst.Id, student.studentId, true);

                }
                db.SaveChanges();
                Logger.Info($"Скопированы студенты из подгруппы {src.Name} в подгруппу {dst.Name}");
            }
            return RedirectToAction("Index", new { src.Meta.programId, message = "Скопировано " + cnt + " зачислений", focus = dstId });
        }


        // GET: Subgroup/Edit/5
        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Subgroup subgroup = db.Subgroups.Find(id);
            if (subgroup == null)
            {
                return NotFound();
            }
            return View(subgroup);
        }

        // POST: Subgroup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult Edit(/*[Bind(Include = "Id,Name,EduProgramId,ParentId,VariantId,ModuleId,SubgroupType,Limit")] */Subgroup subgroup)
        {
            if (ModelState.IsValid)
            {
                var dbEntry = db.Subgroups.Find(subgroup.Id);
                dbEntry.Limit = subgroup.Limit;
                dbEntry.Name = subgroup.Name;
                db.SaveChanges();
                return RedirectToAction("Index", new { programId = dbEntry.Meta.programId, focus = subgroup.Id });
            }


            return View(subgroup);
        }


        [HttpPost]
        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult Delete([FromBody]int[] ids)
        {
            var subgroups = db.Subgroups.Where(s => ids.Contains(s.Id)).ToList();
            foreach(var s in subgroups)
            {
                s.Removed = true;
                db.Entry(s).State = EntityState.Modified;
            }
            db.SubgroupMemberships.RemoveRange(subgroups.SelectMany(s => s.Students));
            db.SaveChanges();
            return JsonNet("Ok");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Students(int id, int? page, int? limit, string sort, string filter)
        {
            var subgroup = db.Subgroups.Find(id);
            ViewBag.Subgroup = subgroup;
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {

                var eduprograms = GetSubGroupStudents(id, sort, filter, subgroup);

                var paginated = eduprograms;//.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = eduprograms.Count()
                });
            }

            return View(new SubgroupVM
            {
                Subgroup = subgroup,
                Id = id
            });
        }

        private IQueryable<SubgroupStudentViewModel> GetSubGroupStudents(int id, string sort, string filter, Subgroup subgroup)
        {
            var queryable = db.Students.Where(
                    s => s.GroupId == subgroup.Meta.groupId &&
                         db.VariantAdmissions.Any(
                             va =>
                                 va.Status == AdmissionStatus.Admitted && va.studentId == s.Id &&
                                 va.Variant.EduProgramId == subgroup.Meta.programId))
                .Select(
                    s =>
                        new
                        {
                            Student = s,
                            variantName =
                            db.VariantAdmissions.FirstOrDefault(
                                va =>
                                    va.Status == AdmissionStatus.Admitted && va.studentId == s.Id &&
                                    va.Variant.EduProgramId == subgroup.Meta.programId).Variant.Name,
                            s.Person,
                            Included =
                            db.SubgroupMemberships.Any(
                                m =>
                                    m.studentId == s.Id &&
                                    (m.SubgroupId == id || m.Subgroup.ParentId == id || m.Subgroup.Parent.ParentId == id)),
                            AnotherGroup =
                            db.SubgroupMemberships.Where(
                                    m =>
                                        m.studentId == s.Id && (
                                            m.Subgroup.Meta.Id == subgroup.MetaSubgroupId ||
                                            (m.Subgroup.Meta.programId == subgroup.Meta.programId &&
                                             m.Subgroup.Meta.Term == subgroup.Meta.Term &&
                                             m.Subgroup.Meta.disciplineUUID == subgroup.Meta.disciplineUUID &&
                                             m.Subgroup.Meta.moduleId == subgroup.Meta.moduleId
                                             &&
                                             (m.Subgroup.Meta.kmer == "tlekc" || m.Subgroup.Meta.kmer == "tprak" ||
                                              m.Subgroup.Meta.kmer == "tlab")
                                             &&
                                             (subgroup.Meta.kmer == "tlekc" || subgroup.Meta.kmer == "tprak" ||
                                              subgroup.Meta.kmer == "tlab")))
                                        && m.SubgroupId != id)

                        });

            var eduprograms = queryable
                .Select(r => new SubgroupStudentViewModel()
                {
                    Id = r.Student.Id,
                    StudentGroupId = r.Student.GroupId,
                    Surname = r.Person.Surname,
                    Name = r.Person.Name,
                    Status = r.Student.Status,
                    PatronymicName = r.Person.PatronymicName,
                    Included = r.Included,
                    AnotherGroup = r.AnotherGroup.Select(_=>_.Subgroup.Name).ToList(),
                    AnotherGroupGroupId = r.AnotherGroup.Select(_=>_.Subgroup.Meta.groupId).ToList(),
                    variantName = r.variantName
                });

            SortRules sortRules = SortRules.Deserialize(sort);
            eduprograms = eduprograms.OrderByThenBy(sortRules.FirstOrDefault(), v => v.Surname, v => v.Name,
                v => v.PatronymicName);
            eduprograms = eduprograms.Where(FilterRules.Deserialize(filter));
            return eduprograms;
        }


        public ActionResult DownloadSubgroupReport(int subgroupId)
        {
            var subgroup = db.Subgroups.Find(subgroupId);
            var reportVms = db.SubgroupMemberships.Where(m => m.SubgroupId == subgroupId || m.Subgroup.ParentId == subgroupId || m.Subgroup.Parent.ParentId == subgroupId)
                .Select(m => m.Student)
                .Distinct()
                .OrderBy(s => s.Person.Surname).ThenBy(s => s.Person.Name).Include(s => s.Person)
                .ToList();
            var stream = new VariantExport().Export(new { Rows = reportVms.Select(s => new { Student = s }) }, "subgroupReportTemplate.xlsx");
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ("Отчёт по подгруппе" + subgroup.Name + ".xlsx").ToDownloadFileName());
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult SaveStudents(SubgroupVM vm)
        {
            /*
            var idToInclude = vm.Students.Where(s=>s.Included).Select(m=>m.Id).ToList();
            var memberships = db.SubgroupMemberships.Where(m=>m.SubgroupId==vm.Id).ToList();

            db.SubgroupMemberships.RemoveRange(memberships.Where(m => !idToInclude.Contains(m.studentId)));

            foreach (var addId in idToInclude.Where(id=>memberships.All(m=>m.studentId!=id)))
            {
                db.SubgroupMemberships.Add(new SubgroupMembership
                {
                    studentId = addId,
                    SubgroupId = vm.Id
                });
            }

            db.SaveChanges();*/
            return RedirectToAction("Students", new { vm.Id });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult StudentsMembership(bool include, int subgroupId, string filter)
        {
            var subgroup = db.Subgroups.FirstOrDefault(_ => _.Id == subgroupId);
            if (subgroup == null) return NotFound("Subgroup not found");

            var subgroupStudents = GetSubGroupStudents(subgroupId, null, filter, subgroup).ToList()
                .Where(s =>
                {
                    var forbid = s.AnotherGroup.Count > 0 && include && s.AnotherGroupGroupId.Contains(s.StudentGroupId);
                    return !forbid;
                }).ToList();

            subgroupStudents.ForEach(s =>
            {
                StudentMembershipInternal(include, s.GetPropertyValue<string>("Id"), subgroupId);
            });
            var curStudentsCount = db.SubgroupMemberships.Count(m => m.SubgroupId == subgroupId);

            string msg = "";
            if (subgroup?.Limit < curStudentsCount && include)
            {
                msg = "Превышен лимит";
            }

            return Json(new { msg });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult StudentMembership(bool include, string studentId, int subgroupId)
        {
            StudentMembershipInternal(include, studentId, subgroupId);
            var msg = "";
            var subgroup = db.Subgroups.FirstOrDefault(m => m.Id == subgroupId);
            var curStudentsCount = db.SubgroupMemberships.Count(m => m.SubgroupId == subgroupId);
            if (subgroup?.Limit < curStudentsCount && include)
            {
                msg = "Превышен лимит";
            }
            return Json(new { msg });
        }

        private void StudentMembershipInternal(bool include, string studentId, int subgroupId)
        {
            var dstSubgroup = db.Subgroups.FirstOrDefault(_ => _.Id == subgroupId);
            var existMembership = db.SubgroupMemberships.FirstOrDefault(m => m.studentId == studentId && m.SubgroupId == subgroupId);
            var student = db.Students.FirstOrDefault(_ => _.Id == studentId);
            
            if ((existMembership != null != include) || (existMembership != null &&
                                                       existMembership.Subgroup.Meta.groupId != student.GroupId &&
                                                       student.GroupId == dstSubgroup.Meta.groupId))
            {
                if (include)
                {
                    db.SubgroupMemberships.Add(new SubgroupMembership
                    {
                        studentId = studentId,
                        SubgroupId = subgroupId
                    });
                }
                else
                {
                    db.SubgroupMemberships.Remove(existMembership);
                }
                db.SaveChanges();
                SubgroupPublication.PublishMember(subgroupId, studentId, include);
            }
        }

        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult FillGroupsWithStudents(int programId)
        {
            var metas = db.MetaSubgroups.Where(m => m.programId == programId && m.Subgroups.Count(s => !s.Removed) > 0).Where(m => !db.Subgroups.Any(s => !s.Removed && s.Parent.MetaSubgroupId == m.Id)).ToList();
            foreach (var meta in metas)
            {
                var toAdmitt = db.Students.Where(
                s =>
                    db.VariantAdmissions.Any(
                        va =>
                            va.Status == AdmissionStatus.Admitted &&
                            va.studentId == s.Id &&
                            va.Variant.EduProgramId == programId &&
                            va.Student.GroupId == meta.groupId &&
                            (
                                va.Variant.Program.Variant.Groups.SelectMany(g => g.Contents).Any(vc => vc.moduleId == meta.moduleId && vc.Selected && !vc.VariantSelectionGroupId.HasValue && !vc.Selectable) ||
                                va.Variant.Groups.SelectMany(g => g.Contents).Any(vc => vc.moduleId == meta.moduleId && vc.Selected && !vc.VariantSelectionGroupId.HasValue && !vc.Selectable)
                            )
                            )
                            ||
                   db.ModuleAdmissions.Any(ma => ma.studentId == s.Id && ma.moduleId == meta.moduleId && ma.Status == AdmissionStatus.Admitted
                   ))
                .Where(s => !db.SubgroupMemberships.Any(m => m.Subgroup.MetaSubgroupId == meta.Id) && s.GroupId == meta.groupId)
                .Select(s => s.Id).ToList();

                var subgroups =
                    db.Subgroups.Where(s => !s.Removed && s.MetaSubgroupId == meta.Id)
                        .Select(s => new { s.Id, admitted = s.Students.Count() })
                        .ToDictionary(s => s.Id, s => s.admitted);
                foreach (var studentId in toAdmitt)
                {
                    var kvp = subgroups.OrderBy(x => x.Value).ThenBy(x => x.Key).First();
                    db.SubgroupMemberships.Add(new SubgroupMembership
                    {
                        studentId = studentId,
                        SubgroupId = kvp.Key,
                    });
                    SubgroupPublication.PublishMember(kvp.Key, studentId, true);
                    subgroups[kvp.Key] = kvp.Value + 1;
                }

                db.SaveChanges();
            }
            var program = db.EduPrograms.FirstOrDefault(_ => _.Id == programId);
            Logger.Info($"Автоматическое распределение студентов по подгруппам версии {program.FullName}");

            return RedirectToAction("Index", new { programId });
        }
    }

    public class SubgroupVM
    {
        [Key]
        public int Id { get; set; }
        public Subgroup Subgroup { get; set; }
    }

    public class SubgroupStudent
    {
        [Key]
        public string Id { get; set; }
        public Student Student { get; set; }

        public Person Person { get; set; }

        public bool Included { get; set; }

        public bool IncludedInChild { get; set; }
    }
}
