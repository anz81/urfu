using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers
{
    [IdentityBasicAuthentication]
    public class LimitsController : BaseController
    {
        public List<RunpProgramLimitDto> Get(string okso = null, int? year = null)
        {
            using (var db = new ApplicationDbContext())
            {
                IQueryable<EduProgram> eduPrograms = db.EduPrograms;
                if (!string.IsNullOrWhiteSpace(okso))
                {
                    eduPrograms = eduPrograms.Where(p => p.Direction.okso == okso);
                }

                if (year.HasValue)
                {
                    eduPrograms = eduPrograms.Where(p => p.Year == year);
                }
                var query = eduPrograms
                    .Where(
                        p =>
                            p.Variant.State == VariantState.Approved &&
                            p.Variants.SelectMany(v => v.ProgramLimits).Any())
                    .Select(p => new
                    {
                        p,
                        Modules = p.Variants
                        .SelectMany(v=>v.Groups.SelectMany(g=>g.Contents).Where(c=>c.Selected).Select(c=>c.Module))
                        .Select(m=>new
                        {
                            m,
                            limits = p.Variant.ProgramLimits.Where(pl=>pl.ModuleId==m.uuid)
                        })
                        .Where(m=>m.limits.Any())
                        .Select(m=>new
                        {
                            m.m,
                            limit = m.limits.Sum(l=>l.StudentsCount),
                            disciplines = m.m.Plans.Where(px => px.directionId == p.directionId &&
                            px.qualification == p.qualification &&
                                px.familirizationType == p.familirizationType &&
                                px.familirizationCondition == p.familirizationCondition &&
                                px.active &&
                                (px.faculty == p.divisionId || px.faculty == p.departmentId || px.faculty == p.chairId)).Select(d => new
                                {
                                    d.catalogDisciplineUUID,
                                    d.disciplineUUID
                                    //dckeys= db.Apploads.Where(a=>a.discipline==d.catalogDisciplineUUID).Select(a=>a.dckey)
                                })
                        })
                    });

                return query.ToList().Select(p => new RunpProgramLimitDto
                {
                    programId = p.p.Id,
                    programName = p.p.Name,
                    programYear = p.p.Year,
                    programDivision = p.p.divisionId,
                    programDirection = p.p.directionId,
                    programQualification = p.p.qualification,
                    programFamCondition = p.p.familirizationCondition,
                    programFamType = p.p.familirizationType,
                    programLimit = p.p.Variant.StudentsLimit,
                    modules = p.Modules.ToList().Select(m=>new RunpModuleLimitDto
                    {
                        moduleId = m.m.uuid,
                        limit = m.limit,
                        disciplines = m.disciplines.ToList().Select(d => d.disciplineUUID).ToList()
                    }).ToList()
                }).ToList();
            }
        }
    }
}