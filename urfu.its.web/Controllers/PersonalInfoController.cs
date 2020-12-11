using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.StudentAdmission)]
    public class PersonalInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PersonalInfo
        public ActionResult Index()
        {
            return View(db.StudentsForUser(User).Take(20));
        }

        public ActionResult Student(string studentId)
        {
            var student = db.StudentsForUser(User).FirstOrDefault(s=>s.Id==studentId);
            if(student==null)
                return NotFound();
            return View(new PersonalInfoVM(student, db));
        }

        public ActionResult ResetStudentModuleSelection()
        {
            throw new NotImplementedException();
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

    public class PersonalInfoVM
    {
        private readonly Student _student;

        public Student Student
        {
            get { return _student; }
        }

        private readonly ApplicationDbContext _db;

        public PersonalInfoVM(Student student, ApplicationDbContext db)
        {
            _student = student;
            _db = db;
            var id  = _student.Id;

            var query = db.Variants.Include("EduPrograms").Where(
                    v =>
                        db.VariantAdmissions.Any(
                            va =>
                                va.Variant.EduProgramId == v.EduProgramId && va.studentId == id &&
                                va.Status == AdmissionStatus.Admitted))
                .Select(
                    variant =>
                        new
                        {
                            variant,
                            selectedVariantPriority =
                            (int?)
                            db.StudentVariantSelections.FirstOrDefault(
                                    sxs => sxs.selectedVariantId == variant.Id && sxs.studentId == id)
                                .selectedVariantPriority,
                        })
                .Select(row => new
                {
                    row.variant.Program.Year,
                    row.variant.Name,
                    variantId = row.variant.Id,
                    row.variant.IsBase,
                    row.selectedVariantPriority,
                    row.variant.Program,
                    modules = row.variant.Groups.SelectMany(g => g.Contents).Where(vc => vc.Selected)
                        .Select(vc => new
                        {
                            vc.moduleId,
                            vc.Module.title,
                            proprity =
                            db.StudentSelectionPriority.Where(
                                ssp =>
                                    ssp.studentId == id && ssp.variantId == row.variant.Id &&
                                    ssp.variantContentId == vc.Id).Select(ssp => (int?) ssp.proprity).FirstOrDefault(),
                            admission =
                            db.ModuleAdmissions.Where(ma => ma.moduleId == vc.moduleId && ma.studentId == id)
                                .Select(va => (AdmissionStatus?) va.Status)
                                .FirstOrDefault(),
                            publishPossible =
                            db.ModuleAdmissions.Any(
                                ma => ma.moduleId == vc.moduleId && ma.studentId == id && !ma.Published),
                            shouldHaveStatus = vc.Selectable || vc.SelectionGroup != null,
                            teachers =
                            db.StudentSelectionTeachers.Where(
                                    sst =>
                                        sst.studentId == id &&
                                        sst.selectedVariantPriority == row.selectedVariantPriority &&
                                        sst.Discipline.Modules.Any(m => m.uuid == vc.moduleId))
                                .Select(sst => new {sst.Discipline.title, sst.control, sst.Teacher}),
                            //vc.Module.disciplines,
                            vc.Module.Plans,
                            vc.Module.type
                        })
                        .OrderBy(r => r.proprity),
                    admission =
                    db.VariantAdmissions.OrderByDescending(va => va.Variant.Program.Year)
                        .Where(va => va.variantId == row.variant.Id && va.studentId == id)
                        .Select(va => (AdmissionStatus?) va.Status)
                        .FirstOrDefault(),
                    publishPossible =
                    db.VariantAdmissions.Any(va => va.variantId == row.variant.Id && va.studentId == id && !va.Published)
                });
                

            Selections = query
                .OrderByDescending(svs => svs.Year)
                .ThenBy(svs => svs.IsBase)
                .ThenBy(svs => svs.selectedVariantPriority)
                .ToList()
                .Select(svs => new VariantSelectionVM(svs.Name, svs.selectedVariantPriority, svs.admission, svs.IsBase, svs.variantId, svs.publishPossible)
                {
                    Modules = svs.modules.ToList().Select(m => new ModuleSelectionVM(m.title, m.proprity, m.admission,m.moduleId,m.publishPossible,m.shouldHaveStatus)
                    {
                        Teachers = m.teachers.ToList().Select(t=>new TeacherSelectionVM(t.Teacher,t.title,t.control)).ToList()
                    }).ToList()
                })
                .ToList();

            var minorsSelecetion = db.StudentSelectionMinorPriority.Include("MinorPeriod.Minor.Module").Where(p => p.studentId == id).ToList();
            var minorAdmission = db.MinorAdmissions.Include("MinorPeriod.Minor.Module").Where(a => a.studentId == id).ToList();

            MinorSelections = minorAdmission.Select(a => new MinorSelectionVM(a)).ToList();

            foreach (var s in minorsSelecetion)
            {
                var a = MinorSelections.FirstOrDefault(m => m.PeriodId == s.minorPeriodId);
                if (a == null)
                {
                    MinorSelections.Add(new MinorSelectionVM(s));
                }
                else
                {
                    a.Priority = s.priority;
                }
            }


            SemesterTestUnitses = db.GetSemesterTestUnitsesForStudent(id);

        }
        
        public List<VariantSelectionVM> Selections { get; set; }

        public List<MinorSelectionVM> MinorSelections { get; set; }
        public List<SemesterTestUnits> SemesterTestUnitses { get;set; }
    }


    public class ModuleSelectionVM
    {
        private readonly string _title;
        private readonly int? _proprity;
        private readonly AdmissionStatus? _admission;
        private readonly bool _publishPossible;

        public string Title
        {
            get { return _title; }
        }

        public int? Proprity
        {
            get { return _proprity; }
        }

        public AdmissionStatus Admission
        {
            get { return _admission??AdmissionStatus.Indeterminate; }
        }

        public string ModuleUuid { get; set; }
        public bool ShouldHaveStatus { get; set; }

        public ModuleSelectionVM(string title, int? proprity, AdmissionStatus? admission, string moduleUuid, bool publishPossible, bool shouldHaveStatus)
        {
            _title = title;
            _proprity = proprity;
            _admission = admission;
            _publishPossible = publishPossible;
            ModuleUuid = moduleUuid;
            ShouldHaveStatus = shouldHaveStatus;
        }

        public bool PublishPossible
        {
            get { return _publishPossible; }
        }

        public List<TeacherSelectionVM> Teachers { get; set; }
    }

    public class TeacherSelectionVM
    {
        private readonly Teacher _teacher;
        private readonly string _title;
        private readonly string _control;

        public TeacherSelectionVM(Teacher teacher, string title, string control)
        {
            _teacher = teacher;
            _title = title;
            _control = control;
        }

        public Teacher Teacher
        {
            get { return _teacher; }
        }

        public string Title
        {
            get { return _title; }
        }

        public string Control
        {
            get { return _control; }
        }
    }

    public class VariantSelectionVM
    {
        private readonly string _name;
        private readonly int? _priority;
        private readonly AdmissionStatus? _admission;
        private readonly bool _isBase;
        private readonly bool _publishPossible;
        private readonly int _variantId;

        public string Name
        {
            get { return _name; }
        }

        public AdmissionStatus Admission
        {
            get { return _admission??AdmissionStatus.Indeterminate; }
        }

        public bool PublishPossible
        {
            get { return _publishPossible; }
        }

        public int? Priority
        {
            get { return _priority; }
        }

        public List<ModuleSelectionVM> Modules { get; set; }

        public VariantSelectionVM(string name, int? priority, AdmissionStatus? admission, bool isBase, int variantId, bool publishPossible)
        {
            _name = name;
            _priority = priority;
            _admission = admission;
            _isBase = isBase;
            _publishPossible = publishPossible;
            _variantId = variantId;
        }

        public bool IsBase
        {
            get { return _isBase; }
        }

        public int VariantId
        {
            get { return _variantId; }
        }
    }

    public class MinorSelectionVM
    {
        private readonly int _periodId;
        private readonly string _name;
        private int? _priority;
        private readonly AdmissionStatus? _admission;
        private readonly bool _published;
        private int _year;
        private string _semester;


        public MinorSelectionVM(MinorAdmission a)
        {
            _periodId = a.minorPeriodId;
            _name = a.MinorPeriod.Minor.Module.title;
            _priority = null;
            _admission = a.Status;
            _published = a.Published;
            _year = a.MinorPeriod.Year;
            _semester = a.MinorPeriod.Semester.Name;
        }

        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public string Semester
        {
            get { return _semester; }
            set { _semester = value; }
        }

        public MinorSelectionVM(StudentSelectionMinorPriority s)
        {
            _periodId = s.minorPeriodId;
            _name = s.MinorPeriod.Minor.Module.title;
            _priority = s.priority;
            _admission = AdmissionStatus.Indeterminate;
            _published = false;
            _year = s.MinorPeriod.Year;
            _semester = s.MinorPeriod.Semester.Name;
        }

        public int PeriodId
        {
            get { return _periodId; }
        }

        public string Name
        {
            get { return _name; }
        }

        public AdmissionStatus Admission
        {
            get { return _admission ?? AdmissionStatus.Indeterminate; }
        }

        public bool Published
        {
            get { return _published; }
        }

        public int? Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
    }   
}