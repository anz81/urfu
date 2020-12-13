using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Controllers
{
    [IdentityBasicAuthentication]
    public class ForeignLanguagesController : BaseController
    {
        public object Get(string okso, int year, int semester)
        {
            using (var db = new ApplicationDbContext())
            {
                List<Module> modules = db.UniModules()
                    .Include(m=>m.ForeignLanguage.Periods)
                    .Include(m=>m.ForeignLanguage.Disciplines)
                    .Where(m =>
                            m.type.Contains(ModuleTypes.ForeignLanguage)
                            && m.specialities.Contains(okso)
                            && m.ForeignLanguage.ShowInLC
                            && m.ForeignLanguage.Periods.Any(p => p.Year == year && p.SemesterId == semester
                            && db.ForeignLanguageCompetitionGroups.Any(cg=>cg.Year==year && cg.SemesterId==semester && cg.ForeignLanguageProperties.Any(px=>px.ForeignLanguageId==m.uuid))
                            )
                    )
                    .OrderBy(m=>m.title)
                    .ToList();
                var me = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<AutoMapperConfig>();
                });
                var mapper = me.CreateMapper();
                var dtos = modules.Select(mapper.Map<ModuleApiDto>).ToList();
                foreach (var m in dtos)
                {
                    m.period = mapper.Map<PeriodApiDto>(modules.First(f => f.uuid == m.uuid).ForeignLanguage.Periods.First(p => p.Year == year && p.SemesterId == semester));
                    m.file = ChangeExtension(m.file);
                    
                    if (m.disciplines != null)
                    {
                        foreach (var d in m.disciplines)
                        {
                            d.file = ChangeExtension(d.file);
                        }
                    }
                }
                return dtos;
            }
        }

        private string ChangeExtension(string file)
        {
            try
            {
                if (string.IsNullOrEmpty(file)) return file;

                return Path.ChangeExtension(file, "docx");
            }
            catch
            {
                return file;
            }
        }
        
        public object Post(StudentSelectionForeignLanuguageDto selection)
        {
            Logger.Info("Запрос api выбора ИЯ студентов");
            return WriteStudentFLSelections(selection);
        }

        public List<StudentSelectionForeignLanuguageDto> GetSelection(string student)
        {
            using (var db = new ApplicationDbContext())
            {
                var stud = db.Students.FirstOrDefault(s => student == s.Id);
                if (stud == null)
                    return new List<StudentSelectionForeignLanuguageDto>();

                var selections = db.ForeignLanguageStudentSelectionPriorities
                    .Where(s=>s.studentId==student).ToList();

                var admissions = db.ForeignLanguageAdmissions
                    .Where(a => a.studentId == student && a.Published).ToList();

                var dtos = new List<StudentSelectionForeignLanuguageDto>();

                foreach(var admission in admissions)
                {
                    dtos.Add(new StudentSelectionForeignLanuguageDto
                    {
                        student = student,
                        moduleId = admission.ForeignLanguageId,
                        semester = admission.ForeignLanguageCompetitionGroup.SemesterId,
                        year = admission.ForeignLanguageCompetitionGroup.Year,
                        targetLevel = stud.ForeignLanguageTargetLevel,
                        admission = EnumHelper<AdmissionStatus>.GetDisplayValue(admission?.Status ?? AdmissionStatus.Indeterminate),
                        admissionid = (int)(admission?.Status ?? AdmissionStatus.Indeterminate),
                        subgroups = db.ForeignLanguageSubgroupMemberships.Include(_ => _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer)
                            .Where(_ => _.studentId == student 
                                    && _.Subgroup.Meta.CompetitionGroupId == admission.ForeignLanguageCompetitionGroupId
                                    && _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Period.SemesterId == admission.ForeignLanguageCompetitionGroup.SemesterId
                                    && _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Period.Year == admission.ForeignLanguageCompetitionGroup.Year
                            ).ToList().Select(_ =>
                                 new ForeignLanguageSubgroupDto()
                                 {
                                     title = _.Subgroup.Name,
                                     id = _.SubgroupId,
                                     level = String.Join(",", db.ForeignLanguageSubgroupMemberships.Where(sm => (sm.SubgroupId == _.Subgroup.Id))
                                         .Concat(db.ForeignLanguageSubgroupMemberships.Where(sm => (sm.Subgroup.ParentId == _.SubgroupId)))
                                         .Concat(db.ForeignLanguageSubgroupMemberships.Where(sm => (sm.Subgroup.Parent != null && sm.Subgroup.Parent.ParentId == _.SubgroupId)))
                                         .Select(m => m.Student.ForeignLanguageLevel)
                                         .Distinct().OrderBy(x => x).ToList()),
                                     teacher = _.Subgroup.Teacher?.initials,
                                     kmer = _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer

                                 }).ToList()
                    });

                    selections.RemoveAll(s => s.competitionGroupId == admission.ForeignLanguageCompetitionGroupId && s.sectionId == admission.ForeignLanguageId);
                }

                foreach(var selection in selections)
                {
                    dtos.Add(new StudentSelectionForeignLanuguageDto
                    {
                        student = student,
                        moduleId = selection.sectionId,
                        semester = selection.CompetitionGroup.SemesterId,
                        year = selection.CompetitionGroup.Year,
                        targetLevel = stud.ForeignLanguageTargetLevel,
                        admission = EnumHelper<AdmissionStatus>.GetDisplayValue(AdmissionStatus.Indeterminate),
                        admissionid = (int)(AdmissionStatus.Indeterminate),
                        subgroups = db.ForeignLanguageSubgroupMemberships.Include(_ => _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer)
                            .Where(_ => _.studentId == student
                                    && _.Subgroup.Meta.CompetitionGroupId == selection.competitionGroupId
                                    && _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Period.SemesterId == selection.CompetitionGroup.SemesterId
                                    && _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Period.Year == selection.CompetitionGroup.Year
                            ).ToList().Select(_ =>
                                 new ForeignLanguageSubgroupDto()
                                 {
                                     title = _.Subgroup.Name,
                                     id = _.SubgroupId,
                                     level = String.Join(",", db.ForeignLanguageSubgroupMemberships.Where(sm => (sm.SubgroupId == _.Subgroup.Id))
                                         .Concat(db.ForeignLanguageSubgroupMemberships.Where(sm => (sm.Subgroup.ParentId == _.SubgroupId)))
                                         .Concat(db.ForeignLanguageSubgroupMemberships.Where(sm => (sm.Subgroup.Parent != null && sm.Subgroup.Parent.ParentId == _.SubgroupId)))
                                         .Select(m => m.Student.ForeignLanguageLevel)
                                         .Distinct().OrderBy(x => x).ToList()),
                                     teacher = _.Subgroup.Teacher?.initials,
                                     kmer = _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer

                                 }).ToList()
                    });
                }

                dtos = dtos.OrderByDescending(s => s.year).ThenByDescending(s => s.semester).ToList();

                if (dtos.Count == 0)
                { 
                    // если не было ни выбора, ни зачисления, то отдаем null
                    dtos.Add(new StudentSelectionForeignLanuguageDto()
                    {
                        student = student,
                        moduleId = null,
                        semester = null,
                        year = null,
                        targetLevel = null,
                        admission = null,
                        admissionid = null
                    });
                }

                return dtos;
            }
        }

        private object WriteStudentFLSelections(StudentSelectionForeignLanuguageDto selection)
        {
            try
            {
                var affected = SyncEngine.WriteStudentForeignLanguageSelectionsToDatabase(selection);
                if (affected == 0)
                    return new {success = true};
                return new {success = false, code = 1};
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new { success = false, error = ex.ToString() };
            }

        }
  
    }
}