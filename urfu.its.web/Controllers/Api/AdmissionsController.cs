using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using AutoMapper;
//using Microsoft.Ajax.Utilities;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Queues;
using Urfu.Its.Web.Models;
using Urfu.Its.Web.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Urfu.Its.Web.Controllers
{
    [IdentityBasicAuthentication]
    public class AdmissionsController : BaseController
    {
        public List<StudentAdmissionDto> Get()
        {
            var me = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            using (var db = new ApplicationDbContext())
            {
                return
                    db.VariantAdmissions.Select(va => va.studentId)
                        .Union(db.ModuleAdmissions.Select(ma => ma.studentId))
                        .Select(
                            id =>
                                new
                                {
                                    id,
                                    va = db.VariantAdmissions.Where(vax => vax.studentId == id && vax.Published),
                                    progsVariants = db.VariantAdmissions.Where(vax => vax.studentId == id && vax.Published && vax.Status==AdmissionStatus.Admitted).Select(v=>v.Variant.Program.Variant.Id),
                                    ma = db.ModuleAdmissions.Where(ma => ma.studentId == id && ma.Published)
                                })
                        .ToList()
                        .Select(o => new StudentAdmissionDto
                        {
                            studentId = o.id,
                            modules = o.ma.Select(mapper.Map<ModuleAdmissionDto>).ToList(),
                            variants = o.va.Select(mapper.Map<VariantAdmissionDto>).ToList()
                            .Concat(o.progsVariants.ToList().Select(pv => new VariantAdmissionDto
                            {
                                variantId = pv,
                                status = AdmissionStatusDto.Admitted
                            })).ToList()
                        })
                        .ToList();
            }
        }

        public StudentAdmissionDto Get(string studentId)
        {
            using (var db = new ApplicationDbContext())
            {
                var progsVariants =
                    db.VariantAdmissions.Where(
                        vax => vax.studentId == studentId && vax.Published && vax.Status == AdmissionStatus.Admitted)
                        .Select(v => v.Variant.Program.Variant.Id).ToList();
                var me = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<AutoMapperConfig>();
                });
                var mapper = me.CreateMapper();
                var dto = new StudentAdmissionDto
                {
                    studentId = studentId,
                    modules =
                        db.ModuleAdmissions.Where(ma => ma.studentId == studentId && ma.Published)
                            .Select(mapper.Map<ModuleAdmissionDto>)
                            .ToList(),
                    variants =
                        db.VariantAdmissions.Where(ma => ma.studentId == studentId && ma.Published)
                            .Select(mapper.Map<VariantAdmissionDto>).ToList()
                            .Concat(progsVariants.Select(pv => new VariantAdmissionDto
                            {
                                variantId = pv,
                                status = AdmissionStatusDto.Admitted
                            })).ToList()
                };
                return dto;
            }
        }

        public StudentAdmissionDto[] GetAll()
        {
            List<string> studentIds;
            using (var db = new ApplicationDbContext())
            {
                studentIds = db.VariantAdmissions.Where(
                    vax => vax.Published && vax.Status == AdmissionStatus.Admitted)
                    .Select(v => v.studentId).ToList();
            }
            return studentIds.Select(Get).ToArray();
        }


        public static void QueueAllStudentAdmissionToLKS()
        {
            try
            {
                Logger.Info("Отправка зачисления студента в ЛКС все");
                var ac = new AdmissionsController();
                var dto = ac.GetAll();
                Logger.Info("Подготовлен пакет зачисления студента в ЛКС ");
                PersonalCabinetService.PostAdmission( dto );
                Logger.Info("Отправка зачисления студента в ЛКС завершена");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void QueuePublishedAdmissions(string studentId)
        {
            try
            {
                Logger.Info("Отправка зачисления студента в ЛКС " + studentId);
                var ac = new AdmissionsController();
                var dto = ac.Get(studentId);
                Logger.Info("Подготовлен пакет зачисления студента в ЛКС " + studentId);
                PersonalCabinetService.PostAdmission(new[] {dto});
                Logger.Info("Отправка зачисления студента в ЛКС завершена" + studentId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }
    }
    
    [IdentityBasicAuthentication]
    public class RunpAdmissionsController : BaseController
    {
        public List<RunpAdmissionDto> Get(int? year = null, string direction = null, int? course = null)
        {
            using (var db = new ApplicationDbContext())
            {
                var dtos = Query(db,null,year,direction,course);

                return dtos;
            }
        }

        public List<RunpAdmissionDto> Get(string studentId)
        {
            using (var db = new ApplicationDbContext())
            {
                var dtos = Query(db, s => s.Id == studentId);

                return dtos;
            }
        }

        public static void QueueStudentAdmission(string studentId)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var okso = db.Students.Where(_ => _.Id==studentId).Select(_ =>_.Group.Profile.Direction.okso).FirstOrDefault();
                    Logger.Info($"Отправка зачисления студента {studentId} , {okso}");
                    var dtos = Query(db, student => student.Id == studentId);
                    Logger.Info($"Подготовлен пакет зачисления студента {studentId} , {okso}");
                    PersonalCabinetService.PostAdmissionsToRunp(dtos);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void QueueAllStudentAdmissionToRunp()
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    Logger.Info("Отправка зачисления всех студентов");
                    var dtos = Query(db, null);
                    Logger.Info("Подготовлен пакет зачисления всех студентов");
                    PersonalCabinetService.PostAdmissionsToRunp(dtos);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private static List<RunpAdmissionDto> Query(ApplicationDbContext db,Expression<Func<Student,bool>> filter, int? year = null, string direction = null, int? course = null)
        {
            IQueryable<Student> students;

            db.Database.SetCommandTimeout(600);

            if (filter != null)
                students = db.Students.Where(filter);
            else
                students = db.Students;

            if (course.HasValue)
                students = students.Where(s => s.Group.Course == course.Value);
            
            var query = students.Select(s => new
            {
                s.Group,
                Student = s,
                s.Person,
                s.VariantAdmissions.Where(_=>_.Variant.State == VariantState.Approved && _.Variant.Program.PlanNumber == s.planVerion && _.Variant.Program.PlanVersionNumber == s.versionNumber)
                                    .OrderByDescending(va=>va.Variant.Program.Year).FirstOrDefault(va => va.Status == AdmissionStatus.Admitted).Variant,
                admissions = s.ModuleAdmissions.Where(a => a.Status == AdmissionStatus.Admitted).Select(a => a.moduleId)
            }).Where(r => r.Variant != null)
                .Select(s => new
                {
                    s.Student,
                    s.Person,
                    s.Group,
                    s.Variant,
                    s.Variant.Program,
                    Modules =
                            s.Variant.Program.Variant.Groups.SelectMany(g=>g.Contents).Where(vc=> vc.Selected && (!vc.Selectable || vc.Selectable && s.admissions.Contains(vc.moduleId)))
                            .Union(s.Variant.Groups.SelectMany(g => g.Contents).Where(vc => vc.Selected && (!vc.Selectable || vc.Selectable && s.admissions.Contains(vc.moduleId))))
                })
                .Select(s => new
                {
                    s.Student,
                    s.Person,
                    s.Group,
                    s.Variant,
                    s.Program,
                    Modules = s.Modules.Select(vc => new
                    {
                        VariantContent =vc,
                        Plans = vc.Module.Plans.Where(p => p.directionId == s.Program.directionId &&
                                                           p.qualification == s.Program.qualification &&
                                                           p.familirizationType == s.Program.familirizationType &&
                                                           p.familirizationCondition ==
                                                           s.Program.familirizationCondition &&
                                                           (p.faculty == s.Program.divisionId || p.faculty == s.Program.departmentId || p.faculty == s.Program.chairId))
                            .Select(p =>new
                                    {
                                        Plan = p,
                                        teachers =db.StudentSelectionTeachers.Where(sst =>sst.studentId == s.Student.Id &&sst.disciplineUUID == p.disciplineUUID).Select(sst => new {sst.control, sst.pkey})
                                    }),
                        limit = s.Program.Variant.ProgramLimits.Where(pl=>pl.ModuleId==vc.Module.uuid).Select(pl=>(int?)pl.StudentsCount).FirstOrDefault()
                    })

                });
        
            if (filter == null)
                query = query.Where(q => q.Modules.Any());

            if (year.HasValue)
                query = query.Where(q => q.Program.Year == year);

            if (!string.IsNullOrWhiteSpace(direction))
                query = query.Where(q => q.Program.Direction.okso == direction);

            var projection = query.Select(q => new
            {
                studentId = q.Student.Id,
                studentName = q.Person.Surname + " " + q.Person.Name + " " + q.Person.PatronymicName,
                groupId = q.Group.Id,
                groupName = q.Group.Name,
                q.Program.Year,
                modules = q.Modules.Select(m => new
                {
                    m.VariantContent.Module.title, m.VariantContent.Group.Variant.IsBase,m.VariantContent.Selectable,
                    SelectionGroupName= m.VariantContent.SelectionGroup.Name, m.limit, m.VariantContent.Module.uuid, 
                    disciplines = m.Plans.Select(p => new
                    {
                        id = p.Plan.disciplineUUID, 
                        name = p.Plan.disciplineTitle, 
                        t = p.teachers,
                        detailDisciplines = db.Apploads
                        .Where(ax => (ax.eduDiscipline == p.Plan.disciplineUUID ) && ax.grp == q.Student.GroupId && !ax.removed && ax.status==ApploadStatus.Approved).Select(a => a.detailDiscipline)
              .Concat(db.Apploads
                        .Where(ax => (ax.eduDiscipline == p.Plan.additionalUUID) && ax.grp == q.Student.GroupId && !ax.removed && ax.status == ApploadStatus.Approved).Select(a => a.detailDiscipline))
                        .Distinct()
                    }).Where(m0 => m0.detailDisciplines.Any())
                })
            }).ToList();

            var dtos = projection.Select(q => new RunpAdmissionDto
            {
                studentId = q.studentId,
                eduProgYear = q.Year,
                groupId = q.groupId,
                groupName = q.groupName,
                studentName = q.studentName,
                modules = q.modules.ToList().Select(m=>new RunpModuleAdmissionDto
                {
                    moduleId = m.uuid,
                    moduleName = m.title,
                    moduleLimit = m.limit,
                    isBase = m.IsBase,
                    selectable = m.Selectable,
                    selectionGroupName = m.SelectionGroupName,
                    disciplines = m.disciplines.ToList().Select(d => new RunpDisciplineDto
                    {
                        disciplineId = d.id,
                        disciplineName = d.name,
                        teachers = d.t.ToDictionary(kvp=>kvp.control,kvp=>kvp.pkey),
                        detailDisciplines = d.detailDisciplines.ToList()
                    }).ToArray()
                }).ToArray()
            }).ToList();
            return dtos;
        }
    }

    [IdentityBasicAuthentication]
    public class ModuleDisciplinesController : BaseController
    {
        public List<ModulePeriodDto> Get(ModuleTypeParam moduleType = ModuleTypeParam.All,int? year = null, int? term = null, string moduleName = null, string competitionGroup = null)
        {
            using (var db = new ApplicationDbContext())
            {
                return Query(db, year, term, moduleType, moduleName, competitionGroup);
            }
        }

        private static List<ModulePeriodDto> Query(ApplicationDbContext db, int? year, int? term, ModuleTypeParam moduleType, string moduleName, string competitionGroup)
        {
            db.Database.SetCommandTimeout(600);

            var projectionFk = moduleType == ModuleTypeParam.SectionFk || moduleType == ModuleTypeParam.All ? QueryFK(db, year, term, moduleName, competitionGroup) : Enumerable.Empty<ModuleDisciplineAdm>();
            var projectionFl = moduleType == ModuleTypeParam.ForeignLanguage || moduleType == ModuleTypeParam.All ? QueryFL(db, year, term, moduleName, competitionGroup) : Enumerable.Empty<ModuleDisciplineAdm>();
            var projectionMinors = moduleType == ModuleTypeParam.Minor || moduleType == ModuleTypeParam.All ? QueryMinors(db, year, term, moduleName, competitionGroup) : Enumerable.Empty<ModuleDisciplineAdm>();
            var projectionProject = moduleType == ModuleTypeParam.Project || moduleType == ModuleTypeParam.All ? QueryProject(db, year, term, moduleName, competitionGroup) : Enumerable.Empty<ModuleDisciplineAdm>();
            var projectionPairedModule = moduleType == ModuleTypeParam.PairedModule || moduleType == ModuleTypeParam.All ? QueryPairedModule(db, year, term, moduleName, competitionGroup) : Enumerable.Empty<ModuleDisciplineAdm>();
            var projectionMUPs = moduleType == ModuleTypeParam.MUP || moduleType == ModuleTypeParam.All ? QueryMUP(db, year, term, moduleName, competitionGroup) : Enumerable.Empty<ModuleDisciplineAdm>();

            return projectionFk.ToList()
                .Concat(projectionFl.ToList())
                .Concat(projectionMinors.ToList())
                .Concat(projectionProject.ToList())
                .Concat(projectionPairedModule.ToList())
                .Concat(projectionMUPs.ToList()).Select(f => new ModulePeriodDto
            {
                moduleId = f.Module.uuid,
                moduleName = f.Module.title,
                moduleType = f.ModuleType,
                year = f.Year,
                term = f.SemesterId,
                course = f.StudentCourse,
                disciplines = f.Disciplines.Select(d => new ModuleDisciplineDto
                {
                    disciplineId = d.disciplineId,
                    disciplineName = d.disciplineName,
                    tmers = d.tmers
                }).ToArray()
            }).ToList();
        }

        private static IEnumerable<ModuleDisciplineAdm> QueryMinors(ApplicationDbContext db, int? year, int? term, string moduleName, string competitionGroup)
        {
            var minors = db.MinorAdmissions.Where(a => a.Status == AdmissionStatus.Admitted).Select(a => new
            {
                a.MinorPeriod.Year,
                a.MinorPeriod.SemesterId,
                StudentCourse = 0,
                a.MinorPeriod.Minor,
            }).Where(a => (year == null || a.Year == year) &&
                          (term == null || term == a.SemesterId) &&
                          (moduleName == null || a.Minor.Module.title == moduleName)
                          ).Distinct();
            
            var projectionMinors = minors.Select(a => new ModuleDisciplineAdm
            {
                Year = a.Year,
                SemesterId = a.SemesterId,
                StudentCourse = 0,
                Module = a.Minor.Module,
                ModuleType = a.Minor.Module.type,
                Disciplines = a.Minor.Disciplines.Select(d => new ModuleDisciplineDto
                {
                    disciplineId = d.Discipline.uid,
                    disciplineName = d.Discipline.title,
                    tmers =
                    db.MinorTmerPeriods.Where(
                            pp => pp.Tmer.Discipline == d && pp.Period.Year == a.Year && pp.Period.SemesterId == a.SemesterId)
                        .Select(pp => pp.Tmer.Tmer.rmer).ToList()
                }).ToList()
            });
            return projectionMinors;
        }

        private static IEnumerable<ModuleDisciplineAdm> QueryFL(ApplicationDbContext db, int? year, int? term, string moduleName, string competitionGroup)
        {
            var fl = db.ForeignLanguageAdmissions.Where(a => a.Status == AdmissionStatus.Admitted
                                                            && (competitionGroup == null || a.ForeignLanguageCompetitionGroup.ShortName == competitionGroup))
                .Select(a => new
                {
                    a.ForeignLanguageCompetitionGroup.Year,
                    a.ForeignLanguageCompetitionGroup.SemesterId,
                    a.ForeignLanguageCompetitionGroup.StudentCourse,
                    a.ForeignLanguage
                }).Where(a => (year == null || a.Year == year) &&
                        (term == null || term == a.SemesterId) &&
                        (moduleName == null || a.ForeignLanguage.Module.title == moduleName)
                        ).Distinct(); ;
          
            var projectionFl = fl.Select(a => new ModuleDisciplineAdm
            {
                Year = a.Year,
                SemesterId = a.SemesterId,
                StudentCourse = a.StudentCourse,
                Module = a.ForeignLanguage.Module,
                ModuleType = a.ForeignLanguage.Module.type,
                Disciplines = a.ForeignLanguage.Disciplines.Select(d => new ModuleDisciplineDto
                {
                    disciplineId = d.Discipline.uid,
                    disciplineName = d.Discipline.title,
                    tmers =
                    db.ForeignLanguageTmerPeriods.Where(
                            pp => pp.Tmer.Discipline == d && pp.Period.Year == a.Year && pp.Period.SemesterId == a.SemesterId)
                        .Select(pp => pp.Tmer.Tmer.rmer).ToList()
                }).ToList()
            });
            return projectionFl;
        }

        private static IEnumerable<ModuleDisciplineAdm> QueryFK(ApplicationDbContext db, int? year, int? term, string moduleName, string competitionGroup)
        {
            var fk = db.SectionFKAdmissions.Where(a => a.Status == AdmissionStatus.Admitted 
                                                        && (competitionGroup == null || a.SectionFKCompetitionGroup.ShortName == competitionGroup))
                .Select(a => new
                {
                    a.SectionFKCompetitionGroup.Year,
                    a.SectionFKCompetitionGroup.SemesterId,
                    a.SectionFKCompetitionGroup.StudentCourse,
                    a.SectionFK
                }).Where(a => (year == null || a.Year == year) &&
                        (term == null || term == a.SemesterId) &&
                        (moduleName == null || a.SectionFK.Module.title == moduleName)
                        ).Distinct();
           
            var projectionFk = fk.Select(a => new ModuleDisciplineAdm
            {
                Year = a.Year,
                SemesterId = a.SemesterId,
                StudentCourse = a.StudentCourse,
                Module = a.SectionFK.Module,
                ModuleType = a.SectionFK.Module.type,
                Disciplines = a.SectionFK.Disciplines.Select(d => new ModuleDisciplineDto
                {
                    disciplineId = d.Discipline.uid,
                    disciplineName = d.Discipline.title,
                    tmers =
                    db.SectionFKTmerPeriods.Where(
                            pp => pp.Tmer.Discipline == d && pp.Period.Year == a.Year && pp.Period.SemesterId == a.SemesterId)
                        .Select(pp => pp.Tmer.Tmer.rmer).ToList()
                }).ToList()
            });
            return projectionFk;
        }

        private static IEnumerable<ModuleDisciplineAdm> QueryProject(ApplicationDbContext db, int? year, int? term, string moduleName, string competitionGroup)
        {
            var project = db.ProjectAdmissions.Where(a => a.Status == AdmissionStatus.Admitted && a.Project.Module.Source == Urfu.Its.Web.Models.Source.Project
                                                        && (competitionGroup == null || a.ProjectCompetitionGroup.ShortName == competitionGroup))
                .Select(a => new
                {
                    a.ProjectCompetitionGroup.Year,
                    a.ProjectCompetitionGroup.SemesterId,
                    a.ProjectCompetitionGroup.StudentCourse,
                    a.Project
                }).Where(a => (year == null || a.Year == year) &&
                        (term == null || term == a.SemesterId) &&
                        (moduleName == null || a.Project.Module.title == moduleName)
                        ).Distinct();

            var projectionProject = project.Select(a => new ModuleDisciplineAdm
            {
                Year = a.Year,
                SemesterId = a.SemesterId,
                StudentCourse = a.StudentCourse,
                Module = a.Project.Module,
                ModuleType = a.Project.Module.type,
                Disciplines = a.Project.Disciplines.Select(d => new ModuleDisciplineDto
                {
                    disciplineId = d.Discipline.uid,
                    disciplineName = d.Discipline.title,
                    tmers =
                    db.ProjectTmerPeriods.Where(
                            pp => pp.Tmer.Discipline == d && pp.Period.Year == a.Year && pp.Period.SemesterId == a.SemesterId)
                        .Select(pp => pp.Tmer.Tmer.rmer).ToList()
                }).ToList()
            });
            return projectionProject;
        }

        private static IEnumerable<ModuleDisciplineAdm> QueryPairedModule(ApplicationDbContext db, int? year, int? term, string moduleName, string competitionGroup)
        {
            var project = db.ProjectAdmissions.Where(a => a.Status == AdmissionStatus.Admitted && a.Project.Module.type == "Парный модуль"
                                                        && (competitionGroup == null || a.ProjectCompetitionGroup.ShortName == competitionGroup))
                .Select(a => new
                {
                    a.ProjectCompetitionGroup.Year,
                    a.ProjectCompetitionGroup.SemesterId,
                    a.ProjectCompetitionGroup.StudentCourse,
                    a.Project
                }).Where(a => (year == null || a.Year == year) &&
                        (term == null || term == a.SemesterId) &&
                        (moduleName == null || a.Project.Module.title == moduleName)
                        ).Distinct();

            var projectionProject = project.Select(a => new ModuleDisciplineAdm
            {
                Year = a.Year,
                SemesterId = a.SemesterId,
                StudentCourse = a.StudentCourse,
                Module = a.Project.Module,
                ModuleType = a.Project.Module.type,
                Disciplines = a.Project.Disciplines.Select(d => new ModuleDisciplineDto
                {
                    disciplineId = d.Discipline.uid,
                    disciplineName = d.Discipline.title,
                    tmers =
                    db.ProjectTmerPeriods.Where(
                            pp => pp.Tmer.Discipline == d && pp.Period.Year == a.Year && pp.Period.SemesterId == a.SemesterId)
                        .Select(pp => pp.Tmer.Tmer.rmer).ToList()
                }).ToList()
            });
            return projectionProject;
        }

        private static IEnumerable<ModuleDisciplineAdm> QueryMUP(ApplicationDbContext db, int? year, int? term, string moduleName, string competitionGroup)
        {
            var mups = db.MUPAdmissions.Where(a => a.Status == AdmissionStatus.Admitted && !a.MUP.Removed
                                                && (competitionGroup == null || a.MUPCompetitionGroup.ShortName == competitionGroup))
                .Select(a => new
                {
                    a.MUPCompetitionGroup.Year,
                    a.MUPCompetitionGroup.SemesterId,
                    a.MUPCompetitionGroup.StudentCourse,
                    a.MUP
                }).Where(a => (year == null || a.Year == year) &&
                        (term == null || term == a.SemesterId) &&
                        (moduleName == null || a.MUP.Module.title == moduleName)
                        ).Distinct(); ;

            var projectionMUP = mups.Select(a => new ModuleDisciplineAdm
            {
                Year = a.Year,
                SemesterId = a.SemesterId,
                StudentCourse = a.StudentCourse,
                Module = a.MUP.Module,
                ModuleType = "МУП",
                Disciplines = a.MUP.Disciplines.Select(d => new ModuleDisciplineDto
                {
                    disciplineId = d.Discipline.uid,
                    disciplineName = d.Discipline.title,
                    tmers =
                    db.MUPDisciplineTmerPeriods.Where(
                            pp => pp.Tmer.Discipline == d && pp.Period.Year == a.Year && pp.Period.SemesterId == a.SemesterId && !pp.Removed)
                        .Select(pp => pp.Tmer.Tmer.rmer).ToList()
                }).ToList()
            });
            return projectionMUP;
        }

        private class ModuleDisciplineAdm
        {
            public int Year { get; set; }
            public int SemesterId { get; set; }
            public int StudentCourse { get; set; }
            public Module Module { get; set; }
            public string ModuleType { get; set; }
            public List<ModuleDisciplineDto> Disciplines { get; set; }
        }
    }

    [IdentityBasicAuthentication]
    public class ModuleAdmissionsController : BaseController
    {
        public List<SectionFKAdmissionDto> Get()
        {
            using (var db = new ApplicationDbContext())
            {
                var dtos = Query(db, null);

                return dtos;
            }
        }

        public List<SectionFKAdmissionDto> Get(string studentId)
        {
            using (var db = new ApplicationDbContext())
            {
                var dtos = Query(db, s => s.Id == studentId);

                return dtos;
            }
        }

        private static List<SectionFKAdmissionDto> Query(ApplicationDbContext db, Expression<Func<Student, bool>> filter)
        {
            IQueryable<Student> students;

            db.Database.SetCommandTimeout(600);

            if (filter != null)
                students = db.Students.Where(filter);
            else
                students = db.Students;

            var query = students.Select(s => new
                {
                    s.Group,
                    Student = s,
                    s.Person
                })
                .Select(s => new
                {
                    s.Student,
                    s.Person,
                    s.Group,
                    ModulesSFK = s.Student.SectionFKAdmissions.Where(a => a.Status == AdmissionStatus.Admitted),
                    ModulesFL = s.Student.ForeignLanguageAdmissions.Where(a => a.Status == AdmissionStatus.Admitted),
                });

                var query1 = query.Select(s => new
                {
                    s.Student,
                    s.Person,
                    s.Group,
                    Modules = s.ModulesSFK.Select(a => new
                    {
                        Plans = a.SectionFK.Module.Plans.Where(p => p.eduplanNumber==s.Student.planVerion && s.Student.versionNumber==p.versionNumber),
                            a.SectionFK.Module,
                        Admission = a
                    }),
                    Modules3 = s.ModulesFL.Select(a => new
                    {
                        Plans = a.ForeignLanguage.Module.Plans.Where(p => p.eduplanNumber==s.Student.planVerion && s.Student.versionNumber==p.versionNumber),
                            a.ForeignLanguage.Module,
                        Admission = a
                    }),
                    Modules2 = db.UniModules().Where(m=>m.title== "Физическая культура и спорт").Select(m=>new
                    {
                        Plans = m.Plans.Where(p => p.eduplanNumber == s.Student.planVerion && s.Student.versionNumber == p.versionNumber),
                        Module = m,
                    }).Where(m=>m.Plans.Any()),
                    Modules4 = db.UniModules().Where(m => m.disciplines.Any(d => d.title == "Иностранный язык")).Select(m=>new
                    {
                        Plans = m.Plans.Where(p => p.eduplanNumber == s.Student.planVerion && s.Student.versionNumber == p.versionNumber),
                        Module = m,
                    }).Where(m=>m.Plans.Any())
                });

            if (filter == null)
                query1 = query1.Where(s => s.Modules.Any() || s.Modules2.Any());

            var projection1 = query1.Select(q => new
            {
                studentId = q.Student.Id,
                studentName = q.Person.Surname + " " + q.Person.Name + " " + q.Person.PatronymicName,
                groupId = q.Group.Id,
                groupName = q.Group.Name,
                modules = q.Modules.Select(m => new
                {
                    m.Module.title,
                    m.Module.uuid,
                    m.Module.type,
                    Year = (int?) m.Admission.SectionFKCompetitionGroup.Year,
                    SemesterId = (int?) m.Admission.SectionFKCompetitionGroup.SemesterId,
                    disciplines = m.Admission.SectionFK.Disciplines.Select(p => new
                    {
                        id = p.Discipline.uid,
                        name = p.Discipline.title
                    })
                }),
                modules3= q.Modules3.Select(m => new
                {
                    m.Module.title,
                    m.Module.uuid,
                    m.Module.type,
                    Year = (int?)m.Admission.ForeignLanguageCompetitionGroup.Year,
                    SemesterId = (int?)m.Admission.ForeignLanguageCompetitionGroup.SemesterId,
                    disciplines = m.Admission.ForeignLanguage.Disciplines.Select(p => new
                    {
                        id = p.Discipline.uid,
                        name = p.Discipline.title
                    })
                })
            }).ToList();

            var projection2 = query1.Select(q => new
            {
                studentId = q.Student.Id,
                modules = q.Modules2.Select(m => new
                {
                    m.Module.title,
                    m.Module.uuid,
                    m.Module.type,
                    Year = (int?)null,
                    SemesterId = (int?)null,
                    disciplines = m.Plans.Select(p => new
                    {
                        id = p.disciplineUUID,
                        name = p.disciplineTitle,
                        detailDisciplines = db.Apploads.Where(ax => (ax.eduDiscipline == p.disciplineUUID) && ax.grp == q.Student.GroupId && !ax.removed && ax.status == ApploadStatus.Approved)
                        .Concat(db.Apploads.Where(ax => (ax.eduDiscipline == p.additionalUUID) && ax.grp == q.Student.GroupId && !ax.removed && ax.status == ApploadStatus.Approved))
                        .Select(a => a.detailDiscipline).Distinct()
                    })
                }),
                modules4 = q.Modules4.Select(m => new
                {
                    m.Module.title,
                    m.Module.uuid,
                    m.Module.type,
                    Year = (int?)null,
                    SemesterId = (int?)null,
                    disciplines = m.Plans.Select(p => new
                    {
                        id = p.disciplineUUID,
                        name = p.disciplineTitle,
                        detailDisciplines = db.Apploads.Where(ax => (ax.eduDiscipline == p.disciplineUUID) && ax.grp == q.Student.GroupId && !ax.removed && ax.status == ApploadStatus.Approved)
                        .Concat(db.Apploads.Where(ax => (ax.eduDiscipline == p.additionalUUID) && ax.grp == q.Student.GroupId && !ax.removed && ax.status == ApploadStatus.Approved))
                        .Select(a => a.detailDiscipline).Distinct()
                    })
                })
            }).ToDictionary(x=>x.studentId,x=>x.modules.ToList().Concat(x.modules4.ToList()));

            var dtos = projection1.Select(q => new SectionFKAdmissionDto
            {
                studentId = q.studentId,
                groupId = q.groupId,
                groupName = q.groupName,
                studentName = q.studentName,
                modules = q.modules.ToList().Concat(q.modules3).Select(m => new SectionFKModuleAdmissionDto
                {
                    moduleId = m.uuid,
                    moduleName = m.title,
                    moduleType = m.type,
                    year = m.Year,
                    term = m.SemesterId,
                    disciplines = m.disciplines.ToList().Select(d => new RunpDisciplineDto
                    {
                        disciplineId = d.id,
                        disciplineName = d.name,
                    }).ToArray()
                }).Concat(projection2[q.studentId].ToList().Select(m => new SectionFKModuleAdmissionDto
                {
                    moduleId = m.uuid,
                    moduleName = m.title,
                    moduleType = m.type,
                    year = m.Year,
                    term = m.SemesterId,
                    disciplines = m.disciplines.ToList().Select(d => new RunpDisciplineDto
                    {
                        disciplineId = d.id,
                        disciplineName = d.name,
                        detailDisciplines = d.detailDisciplines.ToList()
                    }).ToArray()
                }))
                .ToArray()
            }).ToList();
            return dtos;
        }
    }

    [IdentityBasicAuthentication]
    public class SectionFKAdmissionsController : BaseController
    {
        public List<SectionFKAdmissionDto> Get()
        {
            return new ModuleAdmissionsController().Get();
        }

        public List<SectionFKAdmissionDto> Get(string studentId)
        {
            return new ModuleAdmissionsController().Get(studentId);
        }
    }

    [IdentityBasicAuthentication]
    public class MinorAdmissionsController : BaseController
    {
        public List<SectionFKAdmissionDto> Get(int year)
        {
            using (var db = new ApplicationDbContext())
            {
                var dtos = Query(db, null, year);

                return dtos;
            }
        }

        public List<SectionFKAdmissionDto> Get(string studentId, int year)
        {
            using (var db = new ApplicationDbContext())
            {
                var dtos = Query(db, s => s.Id == studentId, year);

                return dtos;
            }
        }

        private static List<SectionFKAdmissionDto> Query(ApplicationDbContext db, Expression<Func<Student, bool>> filter, int year)
        {
            IQueryable<Student> students;

            db.Database.SetCommandTimeout(600);

            if (filter != null)
                students = db.Students.Where(filter);
            else
                students = db.Students;

            var query = students.Select(s => new
                {
                    s.Group,
                    Student = s,
                    s.Person
                })
                .Select(s => new
                {
                    s.Student,
                    s.Person,
                    s.Group,
                    Modules = s.Student.MinorAdmissions.Where(a => a.Status == AdmissionStatus.Admitted)
                });

           if(filter==null)
                query = query.Where(s => s.Modules.Any());

                var query1 = query.Select(s => new
                {
                    s.Student,
                    s.Person,
                    s.Group,
                    Modules = s.Modules.Select(a => new
                    {
                        Plans = a.MinorPeriod.Minor.Module.Plans.Where(p => p.eduplanNumber==s.Student.planVerion && s.Student.versionNumber==p.versionNumber),
                            a.MinorPeriod.Minor.Module,
                        Admission = a
                    }),
                    Modules2 = db.UniModules().Where(m=>m.type.Contains(ModuleTypes.Minor)).Select(m=>new
                    {
                        Plans = m.Plans.Where(p => p.eduplanNumber == s.Student.planVerion && s.Student.versionNumber == p.versionNumber),
                        Module = m,
                    }).Where(m=>m.Plans.Any())
                });

            if (filter == null)
                query1 = query1.Where(q => q.Modules.Any());

            

            var projection = query1.Select(q => new
            {
                studentId = q.Student.Id,
                studentName = q.Person.Surname + " " + q.Person.Name + " " + q.Person.PatronymicName,
                groupId = q.Group.Id,
                groupName = q.Group.Name,
                modules = q.Modules.Select(m => new
                {
                    m.Module.title,
                    m.Module.uuid,
                    m.Module.type,
                    m.Admission.MinorPeriod.Year,
                    m.Admission.MinorPeriod.SemesterId,
                    disciplines = m.Admission.MinorPeriod.Minor.Disciplines.Select(p => new
                    {
                        id = p.Discipline.uid,
                        name = p.Discipline.title,
                        
                    })//.Where(m0 => m0.detailDisciplines.Any())
                }).Where(r=>r.Year==year),
                modules2 = q.Modules2.Select(m => new
                {
                    m.Module.title,
                    m.Module.uuid,
                    m.Module.type,
                    disciplines = m.Plans.Select(p => new
                    {
                        id = p.disciplineUUID,
                        name = p.disciplineTitle,
                        detailDisciplines = db.Apploads.Where(ax => (ax.eduDiscipline == p.disciplineUUID) && ax.grp == q.Student.GroupId && !ax.removed && ax.status == ApploadStatus.Approved)
                        .Union(db.Apploads.Where(ax => (ax.eduDiscipline == p.additionalUUID) && ax.grp == q.Student.GroupId && !ax.removed && ax.status == ApploadStatus.Approved))
                        .Select(a => a.detailDiscipline).Distinct(),
                        terms = p.terms
                    })//.Where(m0 => m0.detailDisciplines.Any())
                })
            });

            if (filter == null)
                projection = projection.Where(p => p.modules.Any());


            var final = projection.ToList();

            var dtos = final.Select(q => new SectionFKAdmissionDto
            {
                studentId = q.studentId,
                groupId = q.groupId,
                groupName = q.groupName,
                studentName = q.studentName,
                modules = q.modules.ToList().Select(m => new SectionFKModuleAdmissionDto
                {
                    moduleId = m.uuid,
                    moduleName = m.title,
                    moduleType = m.type,
                    year = m.Year,
                    term = m.SemesterId,
                    disciplines = m.disciplines.ToList().Select(d => new RunpDisciplineDto
                    {
                        disciplineId = d.id,
                        disciplineName = d.name,
                        
                    }).ToArray()
                })
                .Union(q.modules2.ToList().Select(m => new SectionFKModuleAdmissionDto
                {
                    moduleId = m.uuid,
                    moduleName = m.title,
                    moduleType = m.type,
                    disciplines = m.disciplines.ToList().Select(d => new RunpDisciplineDto
                    {
                        disciplineId = d.id,
                        disciplineName = d.name,
                        detailDisciplines = d.detailDisciplines.ToList(),
                        terms = d.terms
                    }).ToArray()
                }))
                .ToArray()
            }).ToList();
            return dtos;
        }
    }
}