using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml.Core.ExcelPackage;
    //FormulaParsing.Excel.Functions.DateTime;
using PagedList;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Models;
using Ext.Utilities;
using Ext.Utilities.Linq;
using Newtonsoft.Json;
using Urfu.Its.Common;
using PagedList.Core;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.StudentAdmissionRead)]
    public class AdmissionController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admission
        public ActionResult Variants(int? page, int? limit, string sort, string filter, string focus)
        {
            //var variants = db.VariantsForUser(User)
            //    .Include(v => v.Program.Direction)
            //    .Include(v => v.Limits)
            //    .Where(v => v.State == VariantState.Approved && !v.IsBase)
            //    .Select(v=>new{v,c=db.VariantAdmissions.Count(va => va.variantId==v.Id && va.Status==AdmissionStatus.Admitted)})
            //    .ToList();

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)   
            {
                var variants =
                    db.VariantsForUser(User).Where(v => v.State == VariantState.Approved && !v.IsBase).Select(v => new
                    {
                        Id = v.Id,
                        DirectionOkso = v.Program.Direction.okso,
                        Name = v.Name,
                        CreateDate = v.CreateDate,
                        familirizationType = v.Program.familirizationType,
                        familirizationCondition = v.Program.familirizationCondition,
                        qualification = v.Program.qualification,
                        Year = v.Program.Year,
                        StudentsLimit = v.StudentsLimit,
                        Count =
                        db.VariantAdmissions.Count(va => va.variantId == v.Id && va.Status == AdmissionStatus.Admitted),
                        SelectionDeadline = v.SelectionDeadline
                    });
                SortRules sortRules = SortRules.Deserialize(sort);
                variants = variants.OrderBy(sortRules.FirstOrDefault(), v => v.DirectionOkso);

                variants = variants.Where(FilterRules.Deserialize(filter));

                var paginated = variants.ToPagedList(page ?? 1, limit ?? 25);

                return JsonNet(new
                {
                    data = paginated,
                    total = variants.Count()
                });
            }
            ViewBag.Focus = focus;
            return View();

            //return View(variants.Select(v=>new VariantAdmissionVM(v.v,v.c)));
        }

        public ActionResult VariantsAuto()
        {
            return
                View(db.EduProgramsForUser(User)
                    .Include(e => e.Direction)
                    .Include(e => e.Division)
                    .Include(e => e.Profile)
                    .Include(e => e.Division)
                    .Include(e => e.Chair)
                    .Where(p => p.State == VariantState.Approved && p.Variant.State == VariantState.Approved))
                ;
        }

        // GET: Admission

        public ActionResult Modules(int? page, int? limit, string filter, string sort)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                var dx = db.DirectionsForUser(User);
                var eduprograms =
                    db.ModulesForUser(User)
                        .Where(
                            m =>
                                m.UsedInVariantContents.Any(
                                    vc =>
                                        vc.Group.Variant.State == VariantState.Approved && vc.Selected &&
                                        (vc.Selectable || vc.VariantSelectionGroupId.HasValue)))
                        //.Where(m => m.UsedInVariantContents.Any(vc => vc.Group.Variant.State == VariantState.Approved && (vc.Selectable && vc.Selected)))
                        .Select(
                            m =>
                                new
                                {
                                    module = m,
                                    directions =
                                    dx.Where(
                                        d =>
                                            d.Modules.Any(mx => mx.uuid == m.uuid) &&
                                            d.Programs.SelectMany(p => p.Variants)
                                                .Any(v => v.State == VariantState.Approved))
                                })
                        .SelectMany(
                            m =>
                                m.directions.SelectMany(d => d.Programs)
                                    .Where(p => p.Variant.State == VariantState.Approved)
                                    .Select(v => new
                                    {
                                        v.Id,
                                        moduleId = m.module.uuid,
                                        moduleName = m.module.title,
                                        directionId = v.Direction.uid,
                                        DirectionOkso = v.Direction.okso,
                                        DirectionTitle = v.Direction.title,
                                        v.Name,
                                        v.HeadFullName,
                                        v.qualification,
                                        DivisionTitle = v.Division.shortTitle,
                                        ChairTitle = v.Chair.shortTitle,
                                        Profile = v.Profile.NAME,
                                        v.familirizationType,
                                        v.familirizationCondition,
                                        v.Year,
                                    }));

                SortRules sortRules = SortRules.Deserialize(sort);
                eduprograms = eduprograms.OrderBy(sortRules.FirstOrDefault(), v => v.moduleName);

                eduprograms = eduprograms.Where(FilterRules.Deserialize(filter));

                var paginated = eduprograms.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = eduprograms.Count()
                });
            }
            else
            {
                return View();
            }
        }

        public ActionResult DownloadModuleStudents(string directionId, Int32 programId, string moduleId, string filter)
        {
            var module = db.Modules.Find(moduleId);
            var direction = db.Directions.Find(directionId);
            var program = db.EduPrograms.Find(programId);
            var students = FilteredModuleAdmissions(directionId, programId, moduleId, null, filter);
            var stream = new VariantExport().Export(new {Rows = students}, "moduleStudentsTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ("Отчёт по модулю " + module.shortTitle + " в направлении " + direction.OksoAndTitle +
             $" версия - {program.Name}" + ".xlsx").ToDownloadFileName());
        }

        public ActionResult ModuleStudents(string directionId, Int32 programId, string moduleId, string sort,
            string filter)
        {
            var module = db.Modules.Find(moduleId);
            var direction = db.Directions.Find(directionId);
            var program = db.EduPrograms.Find(programId);
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                var students = FilteredModuleAdmissions(directionId, programId, moduleId, sort, filter);
                return Json(students, new Newtonsoft.Json.JsonSerializerSettings());
            }

            ViewBag.Title = "Зачисление на модуль " + module.numberAndTitle + " направления " + direction.OksoAndTitle +
                            $" версия - {program.Name}";

            ViewBag.directionId = directionId;
            ViewBag.moduleId = moduleId;
            ViewBag.programId = programId;

            return View();
        }

        private IEnumerable<Object> FilteredModuleAdmissions(string directionId, Int32 programId, string moduleId,
            string sort, string filter)
        {
            var userdivision = db.DivisionsForUser(User);
            var students =
                db.Students.Include("ModuleAdmissions")
                    .Where(student => db.VariantAdmissions.Any(
                        va =>
                            va.Variant.Program.Id == programId && va.Variant.Program.Direction.uid == directionId &&
                            va.studentId == student.Id))
                    .Select(
                        student =>
                            new
                            {
                                student,
                                @group = db.Groups.FirstOrDefault(g => g.Id == student.GroupId),
                                va =
                                db.VariantAdmissions.OrderByDescending(va => va.Variant.Program.Year)
                                    .FirstOrDefault(
                                        va =>
                                            va.Variant.Program.Id == programId &&
                                            va.Variant.Program.Direction.uid == directionId &&
                                            va.studentId == student.Id &&
                                            va.Status == AdmissionStatus.Admitted),
                                status =
                                (int?)
                                db.ModuleAdmissions.FirstOrDefault(
                                    va => va.moduleId == moduleId && va.studentId == student.Id).Status,
                                //admission =
                                //db.ModuleAdmissions.FirstOrDefault(va => va.moduleId == moduleId && va.studentId == student.Id),
                                person = db.Persons.FirstOrDefault(p => p.Id == student.PersonId),
                                alreadyAdmitted =
                                db.ModuleAdmissions.Any(
                                    va =>
                                        va.studentId == student.Id && va.moduleId == moduleId &&
                                        va.Status == AdmissionStatus.Admitted)
                            })
                    .Select(r => new
                    {
                        r.student,
                        r.@group,
                        r.va,
                        r.status,
                        //r.admission,
                        r.alreadyAdmitted,
                        StudentStatus =
                        (r.va == null && r.alreadyAdmitted &&
                         StudentsExtension.ActiveStatuses.Any(s => s == r.student.Status))
                            ? r.student.Status + " (Переведён в другую ОП)"
                            : r.student.Status,
                        r.person,
                        r.student.PersonalNumber,
                        //limit = db.Limits.Where(l => l.ModuleId == moduleId && l.directionId == directionId && l.Year == r.va.Variant.Program.Year).Select(l => (int?)l.StudentsCount).FirstOrDefault(),
                        admitted =
                        db.ModuleAdmissions.Count(
                            ma =>
                                ma.moduleId == moduleId && ma.Status == AdmissionStatus.Admitted &&
                                ma.Student.Group.Profile.DIRECTION_ID == directionId &&
                                ma.Student.Group.Year == r.student.Group.Year),
                        priority =
                        db.StudentSelectionPriority.Where(
                                ssp =>
                                    ssp.studentId == r.student.Id &&
                                    (ssp.variantId == r.va.variantId || ssp.variantId == r.va.Variant.Program.VariantId) &&
                                    ssp.VariantContent.moduleId == moduleId)
                            .Select(ssp => (int?) ssp.proprity)
                            .FirstOrDefault(),
                        otherAdmissions =
                        db.ModuleAdmissions.Where(
                                ma =>
                                    ma.moduleId != moduleId && ma.studentId == r.student.Id &&
                                    ma.Status == AdmissionStatus.Admitted &&
                                    ma.Module.UsedInVariantContents.Any(
                                        vc => vc.SelectionGroup.Contents.Any(sgc => sgc.moduleId == moduleId)))
                            .Select(ma => ma.Module.number + " " + ma.Module.title)
                    })
                    .Where(
                        r =>
                            (r.va != null || r.alreadyAdmitted) && r.@group != null && r.person != null &&
                            StudentsExtension.ActiveStatuses.Any(s => s == r.student.Status))
                    .Where(
                        r =>
                            userdivision.Any(
                                d =>
                                    d.uuid == r.student.Group.FormativeDivisionId ||
                                    d.uuid == r.student.Group.FormativeDivisionParentId))
                    .Select(r => new
                    {
                        r.student.Id,
                        GroupName = r.student.Group.Name,
                        r.student.Person.Surname,
                        r.student.Person.Name,
                        r.student.Person.PatronymicName,
                        r.StudentStatus,
                        r.student.PersonalNumber,
                        r.student.Rating,
                        r.student.IsTarget,
                        r.student.IsInternational,
                        r.student.Compensation,
                        Priority = r.priority,
                        Admitted = r.admitted,
                        r.otherAdmissions,
                        Status = r.status // r.admission != null ? r.admission.Status : AdmissionStatus.Indeterminate,
                        //VariantId = (r.va != null ? r.va.variantId : 0)
                        ,
                        Published =
                        r.student.ModuleAdmissions.Any(_ => _.moduleId == moduleId) &&
                        r.student.ModuleAdmissions.FirstOrDefault(_ => _.moduleId == moduleId).Published,
                        VariantName = r.va.Variant.Name
                    });

            //var list = students.ToList();
            SortRules sortRules = SortRules.Deserialize(sort);
            students = students.OrderBy(sortRules.FirstOrDefault(), v => v.GroupName);
            students = students.Where(FilterRules.Deserialize(filter));
            var result = students.AsEnumerable().Select(s => new
            {
                s.Id,
                s.GroupName,
                s.Surname,
                s.Name,
                s.PatronymicName,
                s.StudentStatus,
                s.PersonalNumber,
                s.Rating,
                s.IsTarget,
                s.IsInternational,
                s.Compensation,
                s.Priority,
                s.Admitted,
                OtherAdmissions = string.Join(",", s.otherAdmissions),
                s.otherAdmissions,
                s.Status,
                s.Published,
                s.VariantName
                //s.VariantId
            });

            return result;
        }

        public ActionResult VariantStudents(int id, int? page, int? limit, string sort, string filter)
        {
            var variant = db.Variants.Find(id);
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                ViewBag.Title = "Зачисление на траекторию \"" + variant.Name + "\", " + variant.Program.FullName;
                ViewBag.VariantId = id;
                ViewBag.Statuses = JsonConvert.SerializeObject(new List<object>() {
                    new { Id = AdmissionStatus.Admitted, Name = "Зачислен" },
                    new { Id = AdmissionStatus.Denied, Name = "Не зачислен" },
                    new { Id = AdmissionStatus.Indeterminate, Name = "На рассмотрении" }
                });

                ViewBag.StudentStatuses = JsonConvert.SerializeObject(new List<object>() {
                    new { Id = 1, Name = "Активный" },
                    new { Id = 0, Name = "Отчислен" }
                });

                return View();
            }

            var userdivision = db.DivisionsForUser(User);

            var filterRules = FilterRules.Deserialize(filter);
            var filterStudentStatus = filterRules?.Find(f => f.Property == "StudentStatus");
            if (filterRules != null)
            {
                filterRules.Remove(filterStudentStatus);
            }
            int studentStatus;
            var haveStudentStatus = int.TryParse(filterStudentStatus?.Value, out studentStatus);

            var studentsQuery = db.Students.Include("VariantAdmissions").AsQueryable();

            if (haveStudentStatus && studentStatus == 1)
            {
                studentsQuery = studentsQuery.Where(StudentsExtension.ActivityPredicate);
            }
            if (haveStudentStatus && studentStatus == 0)
            {
                studentsQuery = studentsQuery.Where(s => s.Status == "Отчислен");
            }
             var students = studentsQuery
                .Select(student => new
                {
                    student.Id,
                    GroupName = student.Group.Name,
                    Surname = student.Person.Surname,
                    Name = student.Person.Name,
                    PatronymicName = student.Person.PatronymicName,
                    Rating = student.Rating,
                    IsTarget = student.IsTarget,
                    IsInternational = student.IsInternational,
                    Compensation = student.Compensation,
                    Published =
                    student.VariantAdmissions.Any(_ => _.studentId == student.Id) &&
                    student.VariantAdmissions.FirstOrDefault(_ => _.studentId == student.Id).Published,
                    Priority =
                    db.StudentVariantSelections.Where(
                            svs => svs.studentId == student.Id && svs.selectedVariantId == id)
                        .Select(svs => (int?) svs.selectedVariantPriority)
                        .FirstOrDefault(),
                    Status =
                    (int?)
                    db.VariantAdmissions.OrderByDescending(va => va.Variant.Program.Year)
                        .FirstOrDefault(va => va.variantId == id && va.studentId == student.Id)
                        .Status,
                    AnotherAdmission =
                    db.VariantAdmissions.Where(
                            va =>
                                va.variantId != id && va.studentId == student.Id &&
                                va.Status == AdmissionStatus.Admitted && va.Variant.EduProgramId == variant.EduProgramId)
                        .Select(v => v.Variant.Name)
                        .FirstOrDefault(),
                    @group = db.Groups.FirstOrDefault(g => g.Id == student.GroupId),
                    person = db.Persons.FirstOrDefault(p => p.Id == student.PersonId),
                    student,
                    alreadyAdmitted =
                    db.VariantAdmissions.Any(
                        va =>
                            va.studentId == student.Id && va.variantId == id && va.Status == AdmissionStatus.Admitted &&
                            va.Variant.EduProgramId == variant.EduProgramId)
                })
                .Where(
                    r =>
                        r.student.planVerion == variant.Program.PlanNumber &&
                        r.student.versionNumber == variant.Program.PlanVersionNumber)
                .Where(
                    r =>
                        userdivision.Any(
                            d =>
                                d.uuid == r.student.Group.FormativeDivisionId ||
                                d.uuid == r.student.Group.FormativeDivisionParentId))
                .Where(
                    r =>
                        variant.Program.divisionId == r.student.Group.FormativeDivisionId ||
                        variant.Program.divisionId == r.student.Group.FormativeDivisionParentId)
                .Select(s => new
                {
                    s.Id,
                    s.GroupName,
                    s.Surname,
                    s.Name,
                    s.PatronymicName,
                    s.Rating,
                    s.IsTarget,
                    s.IsInternational,
                    s.Compensation,
                    s.Priority,
                    s.Status,
                    StudentStatus = s.student.Status,
                    s.student,
                    s.AnotherAdmission,
                    s.alreadyAdmitted,
                    VariantId = id,
                    qualified = (s.group.Profile.DIRECTION_ID == variant.Program.directionId && s.group != null &&
                                 s.person != null && StudentsExtension.ActiveStatuses.Any(sx => sx == s.student.Status) &&
                                 s.group.FamType == variant.Program.familirizationType &&
                                 s.group.FamCond == variant.Program.familirizationCondition &&
                                 s.group.Qual == variant.Program.qualification),
                    s.Published
                }).Select(s => new
                {
                    s.Id,
                    s.GroupName,
                    s.Surname,
                    s.Name,
                    s.PatronymicName,
                    s.Rating,
                    s.IsTarget,
                    s.IsInternational,
                    s.Compensation,
                    s.Priority,
                    s.Status,
                    s.student.PersonalNumber,
                    StudentStatus =
                    (!s.qualified && s.alreadyAdmitted &&
                     StudentsExtension.ActiveStatuses.Any(sx => sx == s.student.Status))
                        ? s.student.Status + " (Переведён в другую ОП)"
                        : s.student.Status,
                    s.AnotherAdmission,
                    s.alreadyAdmitted,
                    s.VariantId,
                    s.qualified,
                    s.Published
                })
                .Where(s => s.qualified || s.alreadyAdmitted);

            SortRules sortRules = SortRules.Deserialize(sort);
            students = students.OrderBy(sortRules.FirstOrDefault(), v => v.GroupName);

            students = students.Where(filterRules);

            var paginated = students.ToPagedList(page ?? 1, limit ?? 25);
            return JsonNet(new
            {
                data = paginated,
                total = students.Count()
            });
        }

        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public void PublishVariantAdmission(string[] studentId, int variantId)
        {
            foreach (var id in studentId)
            {
                PublishVariantAdmissionInternal(id, variantId);
            }
        }

        private void PublishVariantAdmissionInternal(string studentId, int variantId)
        {
            var admissions =
                db.VariantAdmissions.Where(
                    va => va.studentId == studentId && va.Variant.Program.Variants.Any(v => v.Id == variantId)).ToList();
            foreach (var a in admissions)
            {
                a.Published = true;
            }
            db.SaveChanges();
            AdmissionsController.QueuePublishedAdmissions(studentId);
        }

        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult SetVariantAdmissionStatus(string[] studentId, int variantId, AdmissionStatus status)
        {
            var errorList = new List<string>();
            var notChangedIds = new List<string>();
            return
                Json(
                    new
                    {
                        reload =
                        studentId.Select(
                                id => VariantAdmissionStatusInternal(id, variantId, status, errorList, notChangedIds))
                            .Any(x => x),
                        errors = errorList.Distinct(),
                        notChangedIds
                    });
        }

        private bool VariantAdmissionStatusInternal(string studentId, int variantId, AdmissionStatus status,
            List<string> errorList, List<string> notChangedIds)
        {
            bool isAdmin = User.IsInRole(Urfu.Its.Web.Models.ItsRoles.Admin);

            if (status == AdmissionStatus.Admitted)
            {
                if (
                    db.Variants.Any(
                        v =>
                            v.Id == variantId &&
                            v.StudentsLimit <=
                            (db.VariantAdmissions.Count(
                                va => va.variantId == variantId && va.Status == AdmissionStatus.Admitted))))
                {
                    errorList.Add("У вас превышен лимит зачисления");
                    notChangedIds.Add(studentId);
                    return false;
                }

                var others = db.VariantAdmissions.Where(
                    va =>
                        va.studentId == studentId && va.Variant.Program.Variants.Any(v => v.Id == variantId) &&
                        va.variantId != variantId);

                db.VariantAdmissions.RemoveRange(others);

                var otherVariants =
                    db.Variants.Where(
                        v => !v.IsBase && v.Id != variantId && v.Program.Variants.Any(vx => vx.Id == variantId));
                foreach (var v in otherVariants)
                {
                    var va = new VariantAdmission
                    {
                        Status =
                            status == AdmissionStatus.Admitted ? AdmissionStatus.Denied : AdmissionStatus.Indeterminate,
                        variantId = v.Id,
                        studentId = studentId
                    };

                    db.VariantAdmissions.Add(va);
                }
            }
            else
            {
                //Не даём отчислить из программы, только перезачислить на другую траекторию
                if (
                    db.VariantAdmissions.Any(
                        va =>
                            va.studentId == studentId && va.variantId == variantId &&
                            va.Status == AdmissionStatus.Admitted))
                    if (!isAdmin)
                        return true;
            }


            var admissions =
                db.VariantAdmissions.Where(va => va.studentId == studentId && va.variantId == variantId).ToList();

            List<VariantAdmission> toDelete = new List<VariantAdmission>();
            bool wasdeleted = false;

            foreach (var a in admissions)
            {
                if (status == AdmissionStatus.Indeterminate && isAdmin && !a.Published)
                {
                    toDelete.Add(a);
                }
                else
                {
                    a.Published = false;
                    a.Status = status;
                }
            }

            foreach (var a in toDelete)
            {
                wasdeleted = true;
                db.VariantAdmissions.Remove(a);
            }

            if (admissions.Count == 0)
            {
                var va = new VariantAdmission
                {
                    Status = status,
                    variantId = variantId,
                    studentId = studentId
                };

                db.VariantAdmissions.Add(va);
            }
            db.SaveChanges();
            Task.Run(() => RunpAdmissionsController.QueueStudentAdmission(studentId));
            return wasdeleted;
        }

        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public void PublishModuleAdmission(string[] studentId, string[] moduleId)
        {
            foreach (var st in studentId) PublishModuleAdmissionInternal(st, moduleId);
            //studentId.ForEach(s => PublishModuleAdmissionInternal(s, moduleId));
        }

        private void PublishModuleAdmissionInternal(string studentId, string[] moduleIds)
        {
            foreach (var moduleId in moduleIds)
            {
                var admission =
                    db.ModuleAdmissions.FirstOrDefault(va => va.studentId == studentId && va.moduleId == moduleId);

                if (admission != null)
                {
                    admission.Published = true;
                }
                else
                {
                    admission = new ModuleAdmission()
                    {
                        moduleId = moduleId,
                        studentId = studentId,
                        Status = AdmissionStatus.Indeterminate,
                        Published = true
                    };
                    db.ModuleAdmissions.Add(admission);
                }
            }
            db.SaveChanges();
            AdmissionsController.QueuePublishedAdmissions(studentId);
        }

        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult SetModuleAdmissionStatus(string[] studentId, string moduleId, AdmissionStatus status)
        {
            return
                Json(
                    new {reload = studentId.Select(s => ModuleAdmissionStatusInternal(s, moduleId, status)).Any(_ => _)});
        }

        private bool ModuleAdmissionStatusInternal(string studentId, string moduleId, AdmissionStatus status)
        {
            if (status == AdmissionStatus.Admitted)
            {
                int eduProgramId =
                    db.VariantAdmissions.OrderByDescending(va => va.Variant.Program.Year)
                        .Where(va => va.studentId == studentId && va.Status == AdmissionStatus.Admitted)
                        .Select(va => va.Variant.EduProgramId)
                        .First();
                int alreadyAdmittedCount =
                    db.ModuleAdmissions.Count(
                        ma =>
                            ma.studentId == studentId && ma.Status == AdmissionStatus.Admitted &&
                            ma.Student.VariantAdmissions.Any(
                                va => va.Status == AdmissionStatus.Admitted && va.Variant.EduProgramId == eduProgramId));
                if (
                    db.VariantAdmissions.Any(
                        va =>
                            va.studentId == studentId && va.Status == AdmissionStatus.Admitted &&
                            va.Variant.Program.Variant.ProgramLimits.Any(
                                pl => pl.ModuleId == moduleId && pl.StudentsCount <= alreadyAdmittedCount)))
                    return true;
            }

            var admissions =
                db.ModuleAdmissions.Where(va => va.studentId == studentId && va.moduleId == moduleId).ToList();
            foreach (var a in admissions)
            {
                a.Published = false;
                a.Status = status;
            }
            if (admissions.Count == 0)
            {
                var va = new ModuleAdmission
                {
                    Status = status,
                    moduleId = moduleId,
                    studentId = studentId
                };

                db.ModuleAdmissions.Add(va);                
            }
            if (status == AdmissionStatus.Admitted)
                Logger.Info($"Студент {studentId} зачислен на модуль {moduleId}");
            db.SaveChanges();
            Task.Run(() => RunpAdmissionsController.QueueStudentAdmission(studentId));
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult CalculateVariants(int programId)
        {
            ViewBag.AnyPrepared =
                db.VariantAdmissions.Any(
                    va => va.Variant.EduProgramId == programId && va.Status != AdmissionStatus.Indeterminate);
            ViewBag.AnyPublished =
                db.VariantAdmissions.Any(
                    va =>
                        va.Variant.EduProgramId == programId && va.Status != AdmissionStatus.Indeterminate &&
                        va.Published);
            ViewBag.AnyWithoutRating =
                db.StudentVariantSelections.Any(s => s.Variant.EduProgramId == programId && s.Student.Rating == null);
            ViewBag.VariantsWithoutLimits =
                db.Variants.Any(
                    v =>
                        v.EduProgramId == programId && !v.IsBase && v.StudentsLimit <= 0 &&
                        v.State == VariantState.Approved);

            return View(db.EduPrograms.Find(programId));
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.StudentAdmission)]
        public ActionResult CalculateVariantAdmissions(int programId, string[] classes, bool admitIgnorers)
        {
            if (classes == null || classes.Length == 0)
                return RedirectToAction("Variants");

            db.ChangeTracker.AutoDetectChangesEnabled = false;

            classes = classes.Select(c => "-" + c).ToArray();

            var deleteQuery =
                db.VariantAdmissions.Where(v => v.Variant.EduProgramId == programId)
                    .Where(s => classes.Any(c => s.Student.Group.Name.Contains(c)));
            db.VariantAdmissions.RemoveRange(deleteQuery);
            var variants = db.Variants.Where(v => v.EduProgramId == programId && !v.IsBase).ToList();

            var places = variants.ToDictionary(v => v.Id, v => v.StudentsLimit);


            var wishes = db.StudentVariantSelections
                .Where(s => classes.Any(c => s.Student.Group.Name.Contains(c)))
                .Where(s => s.Variant.EduProgramId == programId && s.selectedVariantId != s.Variant.Program.VariantId)
                //зачислени в основной вариант не требуется
                .OrderByDescending(s => s.Student.IsTarget)
                .ThenByDescending(s => s.Student.IsInternational)
                .ThenByDescending(s => s.Student.Compensation == "контракт")
                .ThenByDescending(s => s.Student.Rating)
                .ThenBy(s => s.selectedVariantPriority)
                .ToList();

            HashSet<string> admittedStudents = new HashSet<string>();

            foreach (var wish in wishes)
            {
                if (!admittedStudents.Add(wish.studentId))
                {
                    MakeDecision(wish, AdmissionStatus.Denied);
                    continue;
                }
                int placesLeft = places[wish.selectedVariantId];
                if (placesLeft > 0)
                {
                    placesLeft--;
                    places[wish.selectedVariantId] = placesLeft;
                    MakeDecision(wish, AdmissionStatus.Admitted);
                }
                else
                {
                    MakeDecision(wish, AdmissionStatus.Denied);
                }
            }

            db.SaveChanges();


            if (admitIgnorers)
            {
                var p = db.EduPrograms.Find(programId);
                var stids = db.Students
                    .Where(s => s.Group.ChairId == p.chairId &&
                                p.familirizationCondition == s.Group.FamCond &&
                                p.familirizationType == s.Group.FamType &&
                                p.qualification == s.Group.Qual)
                    .Where(s => classes.Any(c => s.Group.Name.Contains(c)))
                    .OnlyActive()
                    .Where(
                        s =>
                            !db.VariantAdmissions.Any(va => va.studentId == s.Id && va.Variant.EduProgramId == programId))
                    .Select(s => s.Id)
                    //.OrderBy(s => db.Random())
                    .ToList();

                var unadmited =
                    places.SelectMany(kvp => Enumerable.Repeat(kvp.Key, kvp.Value))
                        .OrderBy(x => Guid.NewGuid())
                        .ToList();
                int idx = 0;

                foreach (var id in stids)
                {
                    if (idx >= unadmited.Count)
                        break;

                    MakeDecision(new StudentVariantSelection
                    {
                        selectedVariantId = unadmited[idx],
                        studentId = id
                    }, AdmissionStatus.Admitted);

                    idx++;
                }


                db.SaveChanges();
            }

            return RedirectToAction("Variants");
        }

        private void MakeDecision(StudentVariantSelection whish, AdmissionStatus status)
        {
            var va = new VariantAdmission
            {
                Status = status,
                variantId = whish.selectedVariantId,
                studentId = whish.studentId
            };

            db.VariantAdmissions.Add(va);
        }


        public ActionResult VariantsReport(int? programId, String filter, String sort)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                var reportVms = PrepareVariantReport(programId, filter, sort).ToList();
                return JsonNet(reportVms);
            }
            ViewBag.programId = programId;
            return View();
        }

        private IQueryable<VariantsReportModel> PrepareVariantReport(int? programId, string filter, string sort)
        {
            IQueryable<Variant> source = db.VariantsForUser(User);
            if (programId != null)
                source = source.Where(v => v.EduProgramId == programId.Value);
            var variants = source
                .Include(v => v.Program.Direction)
                .Include(v => v.Limits)
                .Where(v => v.State == VariantState.Approved && !v.IsBase)
                .Select(
                    v =>
                        new
                        {
                            v,
                            students =
                            db.VariantAdmissions.Where(
                                    va => va.variantId == v.Id && va.Status == AdmissionStatus.Admitted)
                                .Select(va => new {va.Student, va.Student.Group, va.Student.Person}),
                            admittedCount =
                            db.VariantAdmissions.Count(
                                va => va.variantId == v.Id && va.Status == AdmissionStatus.Admitted)
                        })
                .OrderBy(r => r.v.Name);
            var reportVms =
                variants.SelectMany(v => v.students
                    .OrderByDescending(s => s.Student.Rating)
                    .ThenBy(s => s.Student.Person.Surname)
                    .Select(c =>
                        new VariantsReportModel()
                        {
                            Variant = v.v,
                            Group = c.Group,
                            Person = c.Person,
                            Title = v.v.Name + " Лимит: " + v.v.StudentsLimit, // + " Зачислено: " + v.c.Count(),
                            GroupName = c.Group.Name,
                            Id = c.Student.Id,
                            Name = c.Person.Name,
                            Surname = c.Person.Surname,
                            PatronymicName = c.Person.PatronymicName,
                            Status = c.Student.Status,
                            Rating = c.Student.Rating,
                            IsTarget = c.Student.IsTarget,
                            IsInternational = c.Student.IsInternational,
                            Compensation = c.Student.Compensation
                        }
                    ));

            var filterRules = FilterRules.Deserialize(filter);
            reportVms = reportVms.Where(filterRules);

            var sortRules = SortRules.Deserialize(sort);
            reportVms = reportVms.OrderBy(sortRules.FirstOrDefault(), _ => _.Title);
            //var reportVms =
            //    variants.Select(v =>
            //        new VariantReportVM(
            //            v.v,
            //            v.students.ToList().OrderByDescending(s => s.Student.Rating).ThenBy(s => s.Student.Person.Surname).Select(c => c.Student).ToList(),
            //            v.admittedCount
            //        )).ToList();
            return reportVms;
        }

        public ActionResult DownloadVariantsReport(int? programId, string filter)
        {
            var reportVms =
                PrepareVariantReport(programId, filter, null).ToList().GroupBy(v => v.Variant).Select(v => new
                {
                    Variant = v.Key,
                    Students = v,
                    AdmittedCount =
                    db.VariantAdmissions.Count(va => va.variantId == v.Key.Id && va.Status == AdmissionStatus.Admitted)
                }).ToList();
            foreach (var vm in reportVms)
            {
                foreach (var s in vm.Students)
                {
                    s.SemesterTestUnitses = s.SemesterTestUnitses = db.GetSemesterTestUnitsesForStudent(s.Id);
                    s.SemesterTestUnitses.Add(new SemesterTestUnits()
                    {
                        Semester = 11, // Total column
                        TestUnits = s.SemesterTestUnitses.Sum(_ => _.TestUnits)
                    });
                }
            }
            var semesters = Enumerable.Range(1, 11);
            var dynCols = semesters
                .Select(s => new ReportDynamicColumn
                {
                    GetName = () => s != 11 ? s.ToString() + " сем" : "Итого",
                    GetValue = (obj) =>
                    {
                        if (((VariantsReportModel) obj).SemesterTestUnitses.Any(_ => _.Semester == s))
                            return
                                ((VariantsReportModel) obj).SemesterTestUnitses.FirstOrDefault(_ => _.Semester == s)
                                .TestUnits; // : ((VariantsReportModel)obj).SemesterTestUnitses.Sum(_=>_.TestUnits);
                        return null;
                    }
                })
                .ToList();

            //var reportVms = //reportModel.Select(v=> new VariantReportVM(v.Variant,,))
            var stream =
                new VariantExport().Export(
                    new
                    {
                        Rows =
                        reportVms.SelectMany(
                            vm =>
                                vm.Students.Select(s => new {vm.Variant, Student = s, vm.AdmittedCount, dynColumn = s}))
                    }, "variantsReportTemplate.xlsx", dynCols);

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Отчёт по траекториям.xlsx".ToDownloadFileName());
        }

        public ActionResult ModulesReport(int? programId, String filter, String sort)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                var reportVms = GetModulesStudents(programId, filter, sort).ToList();

                return JsonNet(reportVms);
            }
            ViewBag.programId = programId;
            return View();
        }

        private IQueryable<Object> GetModulesStudents(int? programId, String filter, String sort)
        {
            var variants = db.ModulesForUser(User)
                    .Select(
                        v =>
                            new
                            {
                                v,
                                limit =
                                (int?)
                                db.EduPrograms.FirstOrDefault(p => p.Id == programId)
                                    .Variant.ProgramLimits.FirstOrDefault(l => l.ModuleId == v.uuid)
                                    .StudentsCount,
                                c =
                                db.ModuleAdmissions.Where(
                                        va => va.moduleId == v.uuid && va.Status == AdmissionStatus.Admitted)
                                    .Select(
                                        ma =>
                                            new
                                            {
                                                ma.Student,
                                                ma.Student.Group,
                                                ma.Student.Person,
                                                variant =
                                                db.VariantAdmissions.OrderByDescending(va => va.Variant.Program.Year)
                                                    //найти зачисления в варинаты
                                                    .FirstOrDefault(
                                                        va =>
                                                            va.Status == AdmissionStatus.Admitted &&
                                                            va.studentId == ma.studentId && !va.Variant.IsBase)
                                                    .Variant.Name,
                                                limit =
                                                db.VariantAdmissions.OrderByDescending(va => va.Variant.Program.Year)
                                                    //найти зачисления в варинаты
                                                    .Where(
                                                        va =>
                                                            va.Status == AdmissionStatus.Admitted &&
                                                            va.studentId == ma.studentId) //для текущего студента
                                                    .SelectMany(
                                                        va =>
                                                            va.Variant.ProgramLimits.Union(
                                                                va.Variant.Program.Variant.ProgramLimits))
                                                    //взять лимиты из траектории и основной траектории
                                                    .FirstOrDefault(l => l.ModuleId == ma.moduleId),
                                                // взять лимит по нужному модулю
                                                //admittedCount = db.ModuleAdmissions.Count(mx => mx.moduleId == ma.moduleId && mx.Status == AdmissionStatus.Admitted && mx.Student)//тут может будет подсчёт количества студентов из той же траектории на этом модуле
                                            }),
                            })
                    .Where(r => r.c.Any())
                ;
            if (programId != null)
                variants =
                    variants.Where(
                        r =>
                            r.c.Any(
                                x =>
                                    x.Student.VariantAdmissions.Any(
                                        va =>
                                            va.Status == AdmissionStatus.Admitted &&
                                            va.Variant.EduProgramId == programId)));
            var reportVms =
                variants.SelectMany(
                    v => v.c.OrderByDescending(s => s.Student.Rating).ThenBy(s => s.Person.Surname).Select(c => new
                    {
                        numberAndTitle = v.v.number + " " + v.v.title,
                        moduleTitle = v.v.number + " " + v.v.title + (v.limit.HasValue ? " Лимит: " + v.limit : ""),
                        // + " Зачислено: " + v.c.Count(),
                        GroupName = c.Group.Name,
                        v.v.number,
                        c.Student.Id,
                        c.Person.Name,
                        c.Person.Surname,
                        c.Person.PatronymicName,
                        c.Student.Status,
                        c.Student.Rating,
                        c.Student.IsTarget,
                        c.Student.IsInternational,
                        c.Student.Compensation,
                        StudentsCount = c.limit != null ? c.limit.StudentsCount : 0,
                        c.variant
                    }));

            var filterRules = FilterRules.Deserialize(filter);
            reportVms = reportVms.Where(filterRules);

            var sortRules = SortRules.Deserialize(sort);
            reportVms = reportVms.OrderByThenBy(sortRules.FirstOrDefault(), _ => _.moduleTitle);
            
            return reportVms;
        }

        public ActionResult DownloadModulesReport(int? programId, string filter)
        {
            var reportVms = GetModulesStudents(programId, filter, null);
            var stream = new VariantExport().Export(new {Rows = reportVms}, "modulesReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Отчёт по модулям.xlsx".ToDownloadFileName());
        }


        public ActionResult DownloadPrioritiesReport(int? programId)
        {
            IQueryable<Variant> source = db.VariantsForUser(User);
            if (programId != null)
                source = source.Where(v => v.EduProgramId == programId.Value);
            var variants = source
                .Include(v => v.Program.Direction)
                .Include(v => v.Limits)
                .Where(v => v.State == VariantState.Approved && !v.IsBase)
                .Select(
                    v =>
                        new
                        {
                            v,
                            students =
                            db.VariantAdmissions.Where(
                                    va => va.variantId == v.Id && va.Status == AdmissionStatus.Admitted)
                                .Select(
                                    va =>
                                        new
                                        {
                                            va.Student,
                                            va.Student.Group,
                                            va.Student.Person,
                                            priorities =
                                            db.StudentSelectionPriority.Where(
                                                p => p.studentId == va.studentId && (p.Variant.IsBase ? p.Variant.State == VariantState.Approved && p.Variant.EduProgramId == programId.Value : p.variantId == va.variantId))
                                        }),
                            admittedCount =
                            db.VariantAdmissions.Count(
                                va => va.variantId == v.Id && va.Status == AdmissionStatus.Admitted)
                        })
                .OrderBy(r => r.v.Name)
                .ToList();


            var vc_dictionary = db.VariantContents
                .Where(vc => source.Any(v => v.Id == vc.Group.VariantId))
                .Where(vc => db.StudentSelectionPriority.Any(ssp => ssp.variantContentId == vc.Id))
                .Include(vc => vc.Module)
                .ToDictionary(vc => vc.Id);

            Dictionary<string, string> moduleNames = new Dictionary<string, string>();
            foreach (var vcd in vc_dictionary)
            {
                moduleNames[vcd.Value.moduleId] = vcd.Value.Module.shortTitle;
            }

            var dynCols =
                vc_dictionary.GroupBy(kvp => kvp.Value.Module.uuid).OrderBy(kvp => moduleNames[kvp.Key])
                    .Select(kvp => new ReportDynamicColumn
                    {
                        GetName = () => moduleNames[kvp.Key],
                        GetValue = (obj) =>
                        {
                            int res;
                            if (((StudentWithPriorities) obj).Priorities.TryGetValue(kvp.Key, out res))
                                return res;
                            return null;
                        }
                    })
                    .ToList();


            var reportVms =
                variants.Select(v =>
                    new PrioritiesReportVM(
                        v.v,
                        v.students.ToList()
                            .OrderByDescending(s => s.Student.Rating)
                            .ThenBy(s => s.Student.Person.Surname)
                            .ToList()
                            .Select(c => new StudentWithPriorities(c.Student, c.priorities.ToList(), vc_dictionary))
                            .ToList(),
                        v.admittedCount
                    )).ToList();

            var stream =
                new VariantExport().Export(
                    new
                    {
                        Rows =
                        reportVms.SelectMany(
                            vm =>
                                vm.Students.Select(s => new {vm.Variant, Student = s, vm.AdmittedCount, dynColumn = s}))
                    }, "variantsReportTemplate.xlsx", dynCols);


            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Отчёт по приоритетам.xlsx".ToDownloadFileName());
        }

        public ActionResult ProgramsForReports(int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)                
            {
                var eduprograms =
                    db.EduProgramsForUser(User)
                        .Include(e => e.Direction)
                        .Include(e => e.Division)
                        .Include(e => e.Profile)
                        .Select(e => new
                        {
                            e.Id,
                            DirectionOkso = e.Direction.okso,
                            DirectionTitle = e.Direction.title,
                            e.Name,
                            e.HeadFullName,
                            e.qualification,
                            DivisionTitle = e.Division.shortTitle,
                            ChairTitle = e.Chair.shortTitle,
                            Profile = e.Profile.NAME,
                            e.familirizationType,
                            e.familirizationCondition,
                            e.Year
                        });
                SortRules sortRules = SortRules.Deserialize(sort);
                eduprograms = eduprograms.OrderBy(sortRules.FirstOrDefault(), v => v.DirectionOkso);

                eduprograms = eduprograms.Where(FilterRules.Deserialize(filter));

                var paginated = eduprograms.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = eduprograms.Count()
                });
            }


            return View();
        }
    }

    public class VariantsReportModel
    {
        public VariantsReportModel()
        {
        }

        public string Compensation { get; set; }
        public string GroupName { get; set; }
        public string Id { get; set; }
        public bool IsInternational { get; set; }
        public bool IsTarget { get; set; }
        public string Name { get; set; }
        public string PatronymicName { get; set; }
        public decimal? Rating { get; set; }
        public string Status { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }

        [JsonIgnore]
        public Variant Variant { get; set; }

        [JsonIgnore]
        public Person Person { get; set; }

        [JsonIgnore]
        public Group Group { get; set; }

        [JsonIgnore]
        public List<SemesterTestUnits> SemesterTestUnitses { get; set; }
    }


    public class VariantReportVM
    {
        private readonly Variant _variant;
        private readonly List<Student> _students;
        private readonly int _admittedCount;

        public VariantReportVM(Variant variant, List<Student> students, int admittedCount)
        {
            _variant = variant;
            _students = students;
            _admittedCount = admittedCount;
        }

        public int AdmittedCount
        {
            get { return _admittedCount; }
        }

        public Variant Variant
        {
            get { return _variant; }
        }

        public List<Student> Students
        {
            get { return _students; }
        }
    }

    class PrioritiesReportVM
    {
        private readonly Variant _variant;
        private readonly List<StudentWithPriorities> _students;
        private readonly int _admittedCount;

        public PrioritiesReportVM(Variant variant, List<StudentWithPriorities> students, int admittedCount)
        {
            _variant = variant;
            _students = students;
            _admittedCount = admittedCount;
        }

        public int AdmittedCount
        {
            get { return _admittedCount; }
        }

        public Variant Variant
        {
            get { return _variant; }
        }

        public List<StudentWithPriorities> Students
        {
            get { return _students; }
        }
    }

    class StudentWithPriorities
    {
        public string Id { get; set; }

        public string PersonId { get; set; }

        public string Status { get; set; }

        public string PersonalNumber { get; set; }

        public string GroupId { get; set; }

        public string PhoneHome { get; set; }

        public string PhoneMobile { get; set; }

        public string PhoneWork { get; set; }

        public string Email { get; set; }
        public string Icq { get; set; }

        public decimal? Rating { get; set; }

        public string SelectionJson { get; set; }

        public virtual Person Person { get; set; }

        public virtual Group Group { get; set; }

        public virtual ICollection<StudentVariantSelection> Selections { get; set; }

        public StudentRatingType? RatingType { get; set; }

        public bool IsTarget { get; set; }
        public bool IsInternational { get; set; }
        public string Compensation { get; set; }

        public Dictionary<string, int> Priorities { get; set; }

        public StudentWithPriorities(Student s, List<StudentSelectionPriority> priorities,
            Dictionary<int, VariantContent> vcDictionary)
        {
            this.Group = s.Group;
            this.Compensation = s.Compensation;
            this.Email = s.Email;
            GroupId = s.GroupId;
            Icq = s.Icq;
            IsTarget = s.IsTarget;
            IsInternational = s.IsInternational;
            Person = s.Person;
            Rating = s.Rating;
            RatingType = s.RatingType;
            PersonalNumber = s.PersonalNumber;
            Status = s.Status;
            Priorities = new Dictionary<string, int>();
            foreach (var p in priorities)
            {
                var moduleId = vcDictionary[p.variantContentId].moduleId;
                Priorities[moduleId] = p.proprity;
            }
        }
    }

    public class ModuleReportVM
    {
        public int? Limit { get; set; }
        private readonly Module _module;
        private readonly List<StudentWithLimitVM> _students;

        public ModuleReportVM(Module module, List<StudentWithLimitVM> students, int? limit)
        {
            Limit = limit;
            _module = module;
            _students = students;
        }

        public Module Module
        {
            get { return _module; }
        }

        public List<StudentWithLimitVM> Students
        {
            get { return _students; }
        }
    }

    public class StudentWithLimitVM
    {
        private readonly EduProgramLimit _limit;

        public EduProgramLimit Limit
        {
            get { return _limit; }
        }

        public StudentWithLimitVM(Student s, EduProgramLimit limit)
        {
            _limit = limit;
            Group = s.Group;
            Person = s.Person;
            Rating = s.Rating;
            Status = s.Status;
            Id = s.Id;
            IsTarget = s.IsTarget;
            IsInternational = s.IsInternational;
            Compensation = s.Compensation;
        }


        public string Id { get; set; }

        public string Compensation { get; set; }

        public bool IsInternational { get; set; }

        public bool IsTarget { get; set; }

        public decimal? Rating { get; set; }
        public string Status { get; set; }

        public Person Person { get; set; }

        public Group Group { get; set; }
    }

    public class ModuleViewModel
    {
        private readonly Module _module;
        private readonly IQueryable<Direction> _directions;

        public ModuleViewModel(Module module, IQueryable<Direction> directions)
        {
            _module = module;
            _directions = directions;
        }

        public Module Module
        {
            get { return _module; }
        }

        public IQueryable<Direction> Directions
        {
            get { return _directions; }
        }
    }

    public class VariantAdmissionVM
    {
        private readonly Variant _variant;
        private readonly int _count;

        public Variant Variant
        {
            get { return _variant; }
        }

        public int Count
        {
            get { return _count; }
        }

        public VariantAdmissionVM(Variant variant, int count)
        {
            _variant = variant;
            _count = count;
        }
    }
}