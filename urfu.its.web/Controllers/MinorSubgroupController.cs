using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.EntityFrameworkCore;
using Ext.Utilities;
using Ext.Utilities.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
//using Microsoft.Ajax.Utilities;
using Urfu.Its.Common;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Pubs;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Text;
using PagedList.Core;
using Urfu.Its.Web.Controllers.Api;
using Urfu.Its.Web.Model.Models;
using Newtonsoft.Json;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.MinorView)]
    public class MinorSubgroupController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MinorSubgroup
        public ActionResult Index(int? page, int? limit, string sort, string filter, string message = null, int? focus = null)
        {
            ViewBag.Message = message;
            ViewBag.Focus = focus;

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var subgroups = GetMinorSubgroups(sort, filter);

                var paginated = subgroups;//.ToPagedList(page ?? 1, limit ?? 25);
                
                return JsonNet(new
                {
                    data = paginated,
                    total = subgroups.Count(),
                });
            }

            return View();
        }
        [HttpPost]
        public ActionResult UpdateMinorTeacher(int minorSubgroupId, string teacherId)
        {
            var minorSubgroup = db.MinorSubgroups.FirstOrDefault(_ => _.Id == minorSubgroupId);
            if (minorSubgroup != null)
            {
                minorSubgroup.TeacherId = String.IsNullOrEmpty(teacherId) ? null : teacherId;
            }
            db.SaveChanges();
            return Json(new {status = "ok"});
        }
        public ActionResult MinorTeacher(int? page, string sort, string filter, int? limit, int minorId)
        {
            var minor = db.MinorSubgroups.FirstOrDefault(_ => _.Id == minorId);
            if (minor == null)
                return NotFound("property not found");
            var teachers = db.Teachers.Select(t => new
            {
                selected = t.pkey == minor.TeacherId,
                selSort = t.pkey == minor.TeacherId ? 0 : 1,
                teacherId = t.pkey,
                t.firstName,
                t.middleName,
                t.lastName,
                t.workPlace
            });
            SortRules sortRules = SortRules.Deserialize(sort);

            teachers = teachers.OrderByThenBy(sortRules.FirstOrDefault(), a => a.selSort.ToString() ,a => a.lastName);

            teachers = teachers.Where(FilterRules.Deserialize(filter));

            var paginated = teachers.ToPagedList(page ?? 1, limit ?? 25);

            return JsonNet(new
            {
                data = paginated,
                total = teachers.Count()
            });
        }
        public ActionResult StudentsCount(string filter)
        {
            //var studentsCount = db.MinorSubgroups.Include(s => s.Meta.Period.Minor.Module).Include(s => s.Parent).Select(v => 
            //    db.MinorSubgroupMemberships.Where(sm => sm.SubgroupId == v.Id || sm.Subgroup.ParentId == v.Id || sm.Subgroup.Parent.ParentId == v.Id).DistinctBy(_=>_.studentId).Count()
            //);

            var subgroups =
                GetMinorSubgroups(null, filter).Select(v => v.GetPropertyValue<int>("Id")).SelectMany(v =>
                        db.MinorSubgroupMemberships.Where(
                            sm => sm.SubgroupId == v || sm.Subgroup.ParentId == v || sm.Subgroup.Parent.ParentId == v)
                ).Distinct().Count(); //GroupBy(_=>_.studentId).Count();
            return JsonNet(subgroups);
            //return JsonNet(90);

        }
        private IEnumerable<Object> GetMinorSubgroups(string sort, string filter)
        {
            //var rules = FilterRules.Deserialize(filter);

            //rules.FirstOrDefault(r => r.Property == "Year")
            //int? filter_year = null;
            //string filter_semesterName = null;
            //string filter_moduleTitile = null;
            //string filter_name = null;

            SortRules sortRules = SortRules.Deserialize(sort);
            var minorSubgroupMemberships = db.MinorSubgroupMemberships;//.ToList();

            var subgroups = db.MinorSubgroupsForUser(User).Include(s => s.Meta.Period.Minor.Module).Include(s => s.Parent)                
                .Select(v => new
            {
                v.Id,
                v.Name,
                v.MarksFrozen,
                HasScores = v.Meta.Tmer.Tmer.kgmer == 3,
                minorId = v.Meta.Period.Minor.Module.uuid,
                ModuleTitle = v.Meta.Period.Minor.Module.title,
                v.Meta.Period.Year,
                Semester = v.Meta.Period.Semester.Name.ToString(),
                semesterId = v.Meta.Period.Semester.Id,
                subgroupType = v.Meta.Tmer.Tmer.rmer,
                kgmer = v.Meta.Tmer.Tmer.kgmer.ToString(),
                v.Limit,
                teacher = v.Teacher.initials,
                count = minorSubgroupMemberships.Count(
                //db.MinorSubgroupMemberships.Count(
                    sm => sm.SubgroupId == v.Id || sm.Subgroup.ParentId == v.Id || sm.Subgroup.Parent.ParentId == v.Id)
            });
            
            
            subgroups = subgroups.OrderByThenBy(sortRules.FirstOrDefault(), v => v.ModuleTitle, v => v.Name, v => v.Year.ToString(),
                v => v.Semester, v => v.kgmer, v => v.subgroupType);

            var _subgroups = subgroups.Where(FilterRules.Deserialize(filter)).ToList();
            
            return _subgroups;
        }

        [Authorize(Roles = ItsRoles.MinorCreateGroup)]
        public ActionResult DownloadMinorSubGroups(string filter)
        {
            var subgroups = GetMinorSubgroups(null, filter);
            var stream = new VariantExport().Export(new { Rows = subgroups }, "minorSubgroupsTemplate.xlsx");
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Список подгрупп на майноры.xlsx".ToDownloadFileName());

        }

        // GET: Subgroup/Create
        [Authorize(Roles = ItsRoles.MinorCreateGroup)]
        public ActionResult Create()
        {
            var metaSubgroups = db.MinorTmerPeriods
                .OrderByDescending(x => x.Tmer.Tmer.kmer == "tlekc")
                .ThenBy(x => x.Tmer.Tmer.kmer == "tlab")
                .ThenBy(x => x.Tmer.Tmer.kmer == "tprak")
                .ToList();


            foreach (var meta in metaSubgroups)
            {
                var distribution = meta.ExtractDistribution();
                var busyNumbers = new HashSet<int>(db.MinorSubgroups.Where(s => s.MetaSubgroupId == meta.Id).Select(s => s.InnerNumber));

                var exists = busyNumbers.Count();
                for (; exists < meta.GroupCount; exists++)
                {
                    var innerNumber = FindInnerNumber(busyNumbers);
                    busyNumbers.Add(innerNumber);

                    //if (meta.Tmer == null)
                    //    continue;

                    var grp = new MinorSubgroup
                    {
                        Name = meta.Tmer.Discipline.Discipline.title + "\\" + meta.Tmer.Tmer.rmer + "\\" + innerNumber,
                        Limit = (int)Math.Ceiling(meta.Period.MaxStudentsCount / (double)meta.GroupCount),
                        MetaSubgroupId = meta.Id,
                        InnerNumber = innerNumber,
                        ExpectedChildCount = distribution[(innerNumber - 1) % distribution.Length]
                    };

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tlekc")
                    {
                        grp.Name = meta.Tmer.Discipline.Discipline.title + "\\л" + innerNumber;
                    }

                    if (meta.Tmer.Tmer.kmer.ToLower() == "tprak")
                    {
                        grp.Limit = 25;
                        var parents = db.MinorSubgroups.Where(s =>
                           s.Meta.Tmer.Tmer.kmer.ToLower() == "tlekc"
                        && s.Meta.MinorPeriodId == meta.MinorPeriodId
                        && s.Meta.Tmer.MinorDisciplineId == meta.Tmer.MinorDisciplineId)
                        .Select(p => new
                        {
                            p.Id,
                            p.Name,
                            p.ExpectedChildCount,
                            Count = db.MinorSubgroups.Count(sx => sx.ParentId == p.Id)
                        })
                        .OrderBy(p => p.Name)
                        .ToList();

                        var parent = parents.Where(p => (p.ExpectedChildCount ?? 1) > p.Count).FirstOrDefault();
                        if (parent == null)
                        {
                            parent = parents.OrderBy(p => (p.Count / p.ExpectedChildCount ?? 1)).FirstOrDefault();
                        }

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
                        var parent = db.MinorSubgroups.Where(
                            s =>
                                s.Meta.Tmer.Tmer.kmer.ToLower() == "tprak"
                                && s.Meta.MinorPeriodId == meta.MinorPeriodId
                                && s.Meta.Tmer.MinorDisciplineId == meta.Tmer.MinorDisciplineId
                                ).OrderBy(s => db.MinorSubgroups.Count(sx => sx.ParentId == s.Id)).ThenBy(s => s.Name).FirstOrDefault()

                            ??

                        db.MinorSubgroups.Where(
                            s =>
                                s.Meta.Tmer.Tmer.kmer.ToLower() == "tlekc"
                                && s.Meta.MinorPeriodId == meta.MinorPeriodId
                                && s.Meta.Tmer.MinorDisciplineId == meta.Tmer.MinorDisciplineId
                                ).OrderBy(s => db.MinorSubgroups.Count(sx => sx.ParentId == s.Id)).ThenBy(s => s.Name).FirstOrDefault();

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

                    db.MinorSubgroups.Add(grp);
                    //db.Entry(grp).State == EntityState.Added;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        private int FindInnerNumber(HashSet<int> busyNumbers)
        {
            int candidate = 1;
            while (busyNumbers.Contains(candidate))
                candidate++;
            return candidate;
        }

        [Authorize(Roles = ItsRoles.MinorCreateGroup)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            var subgroup = db.MinorSubgroups.Include(s => s.Teacher).FirstOrDefault(s=>s.Id == id);
            if (subgroup == null)
            {
                return NotFound();
            }
            subgroup.Name = subgroup.Name.Replace(@"\", @"\\");

            return View(subgroup);
        }

        [Authorize(Roles = ItsRoles.MinorFreezeMarks)]
        public ActionResult Freeze(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            var subgroup = db.MinorSubgroupsForUser(User).First(m=>m.Id==id);
            subgroup.MarksFrozen = true;
            db.SaveChanges();

            return RedirectToAction("Index",new {focus = id});
        }


        [Authorize(Roles = ItsRoles.MinorFreezeMarks)]
        public ActionResult Unfreeze(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            var subgroup = db.MinorSubgroupsForUser(User).First(m => m.Id == id);
            subgroup.MarksFrozen = false;
            db.SaveChanges();

            return RedirectToAction("Index", new { focus = id });
        }

        // POST: Subgroup/Edit/5
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Authorize(Roles = ItsRoles.MinorCreateGroup)]
        public ActionResult Edit(MinorSubgroup subgroup)
        {
            if (ModelState.IsValid)
            {
                var dbEntry = db.MinorSubgroups.Find(subgroup.Id);
                dbEntry.Limit = subgroup.Limit;
                //dbEntry.Name = subgroup.Name;
                //dbEntry.TeacherId = subgroup.TeacherId;
                db.SaveChanges();
                return JsonNet(new { success = true, status = "OK" });
            }


            return JsonNet(new { success = false, status = "Not Modified" });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Authorize(Roles = ItsRoles.MinorCreateGroup)]
        public ActionResult Delete([FromBody]int[] ids)
        {
            db.MinorSubgroups.RemoveRange(db.MinorSubgroups.Where(s => ids.Contains(s.Id)));
            db.SaveChanges();
            return JsonNet("Ok");
        }


        public ActionResult Students(int id, int? page, int? limit, string sort, string filter)
        {
            var subgroup = db.MinorSubgroups.Find(id);
            ViewBag.Subgroup = subgroup;
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var students = GetMinorSubgroupsStudents(id, sort, filter, subgroup);

                var paginated = students;//.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = students.Count()
                });
            }

            return View(new MinorSubgroupVM
            {
                Subgroup = subgroup,
                Id = id,
                HasScores = HasScores(id)
            });
        }

        private bool HasScores(int id)
        {
            //used also in Index
            return db.MinorSubgroups.Any(s => s.Id == id && s.Meta.Tmer.Tmer.kgmer == 3);
        }

        private IQueryable<Object> GetMinorSubgroupsStudents(int id, string sort, string filter, MinorSubgroup subgroup)
        {
            var query = db.Students
                .Where(s => db.MinorAdmissions.Any(ma => ma.studentId == s.Id
                                                         && ma.Status == AdmissionStatus.Admitted
                                                         && ma.minorPeriodId == subgroup.Meta.MinorPeriodId))
                .Select(s => new
                {
                    Student = s,
                    s.Person,
                    Included = db.MinorSubgroupMemberships.Any(m => m.studentId == s.Id
                                                                    &&
                                                                    (m.SubgroupId == id || m.Subgroup.ParentId == id ||
                                                                     m.Subgroup.Parent.ParentId == id)),
                    AnotherGroup =
                    db.MinorSubgroupMemberships.FirstOrDefault(m => m.studentId == s.Id
                                                                    && (
                                                                        m.Subgroup.MetaSubgroupId == subgroup.MetaSubgroupId
                                                                        //или та же нагрузка 
                                                                        || (
                                                                            m.Subgroup.Meta.MinorPeriodId ==
                                                                            subgroup.Meta.MinorPeriodId
                                                                            //или я на том же майноре но зачислен не в эту группу, а куда то на л, п, или пр
                                                                            &&
                                                                            (m.Subgroup.Meta.Tmer.Tmer.kmer == "tlekc" ||
                                                                             m.Subgroup.Meta.Tmer.Tmer.kmer == "tprak" ||
                                                                             m.Subgroup.Meta.Tmer.Tmer.kmer == "tlab")
                                                                            &&
                                                                            (subgroup.Meta.Tmer.Tmer.kmer == "tlekc" ||
                                                                             subgroup.Meta.Tmer.Tmer.kmer == "tprak" ||
                                                                             subgroup.Meta.Tmer.Tmer.kmer == "tlab")
                                                                        )
                                                                    )
                                                                    && m.SubgroupId != id) //и я точно в другой группе
                        .Subgroup.Name,
                        Membership = db.MinorSubgroupMemberships.FirstOrDefault(m => m.studentId == s.Id
                                                                    &&
                                                                    (m.SubgroupId == id || m.Subgroup.ParentId == id ||
                                                                     m.Subgroup.Parent.ParentId == id))
                });

            var students = query.Select(r => new
            {
                Id = r.Student.Id,
                GroupName = r.Student.Group.Name,
                r.Person.Surname,
                r.Person.Name,
                r.Person.PatronymicName,
                Included = r.Included,
                r.AnotherGroup,
                r.Membership.Score,
                r.Membership.Mark,
            });

            SortRules sortRules = SortRules.Deserialize(sort);
            students = students.OrderByThenBy(sortRules.FirstOrDefault(), v => v.Surname, v => v.Name, v => v.PatronymicName);

            students = students.Where(FilterRules.Deserialize(filter));
            return students;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult StudentsMembership(bool include, int subgroupId, string filter)
        {
            var subgroup = db.MinorSubgroups.FirstOrDefault(_ => _.Id == subgroupId);
            if (subgroup == null) return NotFound("MinorSubgroup not found");

            var subgroupStudents = GetMinorSubgroupsStudents(subgroupId, null, filter, subgroup).ToList().Where(s =>
            {
                var anotherGroup = s.GetPropertyValue<String>("AnotherGroup");
                return String.IsNullOrEmpty(anotherGroup);
            }).ToList();

            subgroupStudents.ForEach(s =>
            {
                StudentMembershipInternal(include, s.GetPropertyValue<string>("Id"), subgroupId);
            });
            var curStudentsCount = db.MinorSubgroupMemberships.Count(m => m.SubgroupId == subgroupId);

            string msg = "";
            if (subgroup?.Limit < curStudentsCount && include)
            {
                msg = "Превышен лимит";
            }

            return Json(new { msg });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult StudentMark(decimal? score, string studentId, int subgroupId)
        {
            var mark = ScoreTransformer.ScoreToMark(score);
            var minorsForUser = db.MinorsForUser(User);
            var subgroup = db.MinorSubgroupMemberships.Include(m=>m.Subgroup).FirstOrDefault(m=>m.studentId== studentId && m.SubgroupId == subgroupId && minorsForUser.Any(mx=>mx.uuid==m.Subgroup.Meta.Period.Minor.ModuleId));
            if(subgroup==null)
                return Json(new { ok = false, msg = "Ошибка поиска зачисления студента в подгруппу" });
            if (subgroup.Subgroup.MarksFrozen)
                return Json(new { ok = false, msg = "Ведомость закрыта" });
            if (!User.IsInRole(ItsRoles.MinorEditMarks))
            {
                return Json(new { ok = false, msg = "У вас нет прав на редактирвоание оценок" });
            }
            
            subgroup.Mark = mark;
            subgroup.Score = score;
            Logger.Info($"Оценка по майнору студент {studentId} подгруппа {subgroupId} балл {score} оценка {mark}");
            db.SaveChanges();

            return Json(new { ok = true, mark });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Authorize(Roles = ItsRoles.MinorCreateGroup)]
        public ActionResult StudentMembership(bool include, string studentId, int subgroupId)
        {
            StudentMembershipInternal(include, studentId, subgroupId);

            var msg = "";
            var subgroup = db.MinorSubgroups.FirstOrDefault(m => m.Id == subgroupId);
            var curStudentsCount = db.MinorSubgroupMemberships.Count(m => m.SubgroupId == subgroupId);
            if (subgroup?.Limit < curStudentsCount && include)
            {
                msg = "Превышен лимит";
            }
            return Json(new { msg });
        }

        private void StudentMembershipInternal(bool include, string studentId, int subgroupId)
        {
            var subgroup = db.MinorSubgroups.Include(s => s.Meta.Tmer.Tmer).FirstOrDefault(s => s.Id == subgroupId);
            
            var existsSubgroups =
                db.MinorSubgroupMemberships.FirstOrDefault(
                    m => (m.studentId == studentId)
                        && m.Subgroup.Meta.Tmer.Tmer.kmer == subgroup.Meta.Tmer.Tmer.kmer
                        && m.Subgroup.MetaSubgroupId == subgroup.MetaSubgroupId);

            if (existsSubgroups == null) // если студент не содержится ни в одной подгруппе
            {
                if (include)
                {
                    AddMembership(new MinorSubgroupMembership
                    {
                        studentId = studentId,
                        SubgroupId = subgroupId
                    });
                    SubgroupPublication.PublishMinorMember(subgroupId, studentId, include);
                }
            }
            else
            {
                if (existsSubgroups.SubgroupId != subgroupId) // если содержится в другой подгруппе
                {
                    if (include)
                    {
                        RemoveMembership(existsSubgroups);

                        AddMembership(new MinorSubgroupMembership
                        {
                            studentId = studentId,
                            SubgroupId = subgroupId
                        });
                        SubgroupPublication.PublishMinorMember(subgroupId, studentId, include);
                    }
                }
                else // если содержится в текущей подгруппе
                {
                    if (!include)
                    {
                        RemoveMembership(existsSubgroups);
                        SubgroupPublication.PublishMinorMember(subgroupId, studentId, include);
                    }
                }
            }
        }

        private void RemoveMembership(MinorSubgroupMembership membership)
        {
            db.MinorSubgroupMemberships.Remove(membership);
            Logger.Info($"Студент {membership.studentId} удален из подгруппы {membership.SubgroupId} Майноры");

            db.SaveChanges();
        }

        private void AddMembership(MinorSubgroupMembership membership)
        {
            db.MinorSubgroupMemberships.Add(membership);
            Logger.Info($"Студент {membership.studentId} добавлен в подгруппу {membership.SubgroupId} Майноры");

            db.SaveChanges();
        }

        [Authorize(Roles = ItsRoles.MinorCreateGroup)]
        public ActionResult FillGroupsWithStudents()
        {
            Logger.Info($"Автоматическое распределение студентов по подгруппам Майноры");
            var metas = db.MinorTmerPeriods.Where(m => m.Subgroups.Count() > 0).Where(m => !db.MinorSubgroups.Any(s => s.Parent.MetaSubgroupId == m.Id)).ToList();
            foreach (var meta in metas)
            {
                var toAdmitt = db.Students.Where(
                s => db.MinorAdmissions.Any(ma => ma.studentId == s.Id && ma.minorPeriodId == meta.MinorPeriodId && ma.Status == AdmissionStatus.Admitted)
                )
                .Where(s => !db.MinorSubgroupMemberships.Any(m => m.Subgroup.MetaSubgroupId == meta.Id && m.studentId == s.Id))
                .Select(s => s.Id).ToList();

                var subgroups =
                    db.MinorSubgroups.Where(s => s.MetaSubgroupId == meta.Id)
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
                    var existedSubgroup = db.MinorSubgroupMemberships.Include(s => s.Subgroup).FirstOrDefault(m => m.studentId == studentId
                            && m.Subgroup.Meta.Period.ModuleId == meta.Period.ModuleId);

                    // порядковый номер группы, на которую надо зачислить студента 
                    var subgroup = subgroups.OrderBy(x => x.Admitted).ThenBy(x => x.InnerNumber)
                        .Where(s => s.InnerNumber == existedSubgroup?.Subgroup?.InnerNumber || existedSubgroup == null)
                        .FirstOrDefault()
                        ?? subgroups.OrderBy(x => x.Admitted).ThenBy(x => x.InnerNumber).First();
                    
                    db.MinorSubgroupMemberships.Add(new MinorSubgroupMembership
                    {
                        studentId = studentId,
                        SubgroupId = subgroup.Id,
                    });
                    SubgroupPublication.PublishMinorMember(subgroup.Id, studentId, true);
                    subgroup.Admitted += 1;
                    Logger.Info($"Студент {studentId} распределен в подгруппу {subgroup.Id} Майноры");
                }
                
                db.SaveChanges();
            }


            return RedirectToAction("Index");
        }

        [Authorize(Roles = ItsRoles.MinorCreateGroup)]
        public ActionResult SimpleCopyMembership(int srcId, int dstId, bool withTeacher)
        {
            var src = db.MinorSubgroups.Include(s => s.Meta.Tmer.Tmer).FirstOrDefault(_ => _.Id == srcId);
            var dst = db.MinorSubgroups.Include(s => s.Meta.Tmer.Tmer).FirstOrDefault(s => s.Id == dstId);

            int cnt = 0;
            if (src.Id != dst.Id)
            {
                if (src.Meta.Tmer.Tmer.rmer ==
                    dst.Meta.Tmer.Tmer.rmer)
                {
                    return RedirectToAction("Index", new { competitionGroupId = src.MetaSubgroupId });
                }

                var srcStudents = src.Students.Select(_ => _.studentId).ToList();

                var students = db.MinorSubgroups.Where(_ => _.Meta.Id == dst.Meta.Id &&
                                                                    _.Meta.Tmer.Tmer.rmer ==
                                                                    dst.Meta.Tmer.Tmer.rmer)
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
                    dst.Students.Add(new MinorSubgroupMembership
                    {
                        SubgroupId = dst.Id,
                        studentId = student.studentId
                    });
                    Logger.Info($"Студент {student.studentId} скопирован в подгруппу {dst.Id} Майоноры");
                    SubgroupPublication.PublishMinorMember(dst.Id, student.studentId, true);
                }
                db.SaveChanges();
                Logger.Info($"Скопированы студенты из подгруппы {src.Name} в подгруппу {dst.Name}");

            }
            return Json(new { success = true, message = "Скопировано " + cnt + " зачислений", focus = dstId });//, "text/html", Encoding.Unicode);
        }

        [Authorize(Roles = ItsRoles.MinorCreateGroup)]
        public FileResult MassPrint(string filter)
        {
            var subgroups = GetMinorSubgroups(null, filter).ToList();

            using (var zipArchiveStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var subgroup in subgroups)
                    {
                        using (var reportStream = PrepareSubgroupReportStream(subgroup.GetPropertyValue<int>("Id")))
                        {
                            var entry = zipArchive.CreateEntry((subgroup.GetPropertyValue<String>("Name") + ".xlsx").CleanFileName(), CompressionLevel.Fastest);
                            using (var zipEntryStream = entry.Open())
                            {
                                reportStream.Position = 0;
                                reportStream.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }

                zipArchiveStream.Position = 0;

                return new FileContentResult(zipArchiveStream.ToArray(), "application/zip") { FileDownloadName = "Списочный состав подгрупп.zip".ToDownloadFileName() };
            }

        }

        public ActionResult DownloadSubgroupReport(int subgroupId)
        {
            var subgroup = db.MinorSubgroups.Find(subgroupId);
            var stream = PrepareSubgroupReportStream(subgroupId);
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ("Отчёт по подгруппе " + subgroup.Name + ".xlsx").ToDownloadFileName());
        }
        public ActionResult MinorPeriodYears()
        {            
            var years = db.MinorPeriods.Distinct().Select(_ => new { Id = _.Year, Name = _.Year }).ToList();
            var json = Json(
                new
                {
                    data = years
                },
                new JsonSerializerSettings()
            );

            return json;           
        }

        public ActionResult Modules()
        {            
            var modules = db.MinorSubgroups.Include(s => s.Meta.Period.Minor.Module).Distinct().Select(_ => new
            {
                Id = _.Meta.Period.Minor.Module.uuid,
                Name = _.Meta.Period.Minor.Module.numberAndTitle,
                Year = _.Meta.Period.Year,
                SemesterId = _.Meta.Period.SemesterId
            }).ToList();
            var json = Json(
                new
                {
                    data = modules
                },
                new JsonSerializerSettings()
            );

            return json;            
        }

        private Stream PrepareSubgroupReportStream(int subgroupId)
        {
            var hasScores = HasScores(subgroupId);
            var reportVms =
                db.MinorSubgroupMemberships.Where(
                        m =>
                            m.SubgroupId == subgroupId || m.Subgroup.ParentId == subgroupId ||
                            m.Subgroup.Parent.ParentId == subgroupId)
                    .Distinct()
                    .OrderBy(s => s.Student.Person.Surname).ThenBy(s => s.Student.Person.Name).Include(s => s.Student.Person).Include(s => s.Student.Group)
                    .ToList();
            var Title = db.MinorSubgroups.Where(s=>s.Id==subgroupId).Select(s=>s.Meta.Tmer.Discipline.Minor.Module.title).FirstOrDefault();
            var Subgroup = db.MinorSubgroups.Where(s => s.Id == subgroupId).Select(s=>s.Name).FirstOrDefault();
            var Passed = reportVms.Count(vm=>ScoreTransformer.IsPassScore(vm.Score) == true);
            var Failed = reportVms.Count(vm => ScoreTransformer.IsPassScore(vm.Score) == false);
            var stream = new VariantExport().Export(new { Rows = reportVms, Title, Subgroup, Passed,Failed},
                hasScores? "subgroupReportWithMarksTemplate.xlsx" : "subgroupReportTemplate.xlsx");
            return stream;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class MinorSubgroupVM
    {
        [Key]
        public int Id { get; set; }
        public MinorSubgroup Subgroup { get; set; }
        public bool HasScores { get; set; }
    }
}