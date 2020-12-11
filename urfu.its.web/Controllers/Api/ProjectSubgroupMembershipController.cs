using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Controllers
{
    [IdentityBasicAuthentication]
    public class ProjectSubgroupMembershipController : BaseController
    {
        public IEnumerable<ProjectSubgroupMembershipApiDto> Get(int year, int term, int? subgroupId = null)
        {
            using (var db = new ApplicationDbContext())
            {
                var projectSubgroupMembership = db.ProjectSubgroups
                    .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == term
                        && (subgroupId == null || s.Id == subgroupId)
                        && s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer == "prex"  // отправляем только подгруппы по экзамену
                        && s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Source == Source.Project
                        )
                    .Select(s => new ProjectSubgroupMembershipApiDto
                    {
                        moduleId = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.uuid,
                        moduleName = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title,
                        moduleType = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type,
                        competences = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Competences
                            .OrderBy(c => c.Competence.DirectionId).ThenBy(c => c.Competence.ProfileId).ThenBy(c => c.Competence.Order)
                            .GroupBy(c => c.ProfileId).Select(g => new ProjectCompetenceListApiDto()
                            {
                                programUuid = g.Key,
                                competences = g.Select(c => new ProjectCompetenceApiDto()
                                {
                                    code = c.Competence.Code,
                                    name = c.Competence.Content,
                                    type = c.Competence.Type
                                })
                            }),
                        emoloyersId = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.EmployersId.HasValue ?
                                s.Meta.ProjectDisciplineTmerPeriod.Period.Project.EmployersId.Value : -1,
                        disciplineId = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.DisciplineUid,
                        disciplineName = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                        loadTypeId = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer,
                        loadTypeName = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer,
                        eduyear = s.Meta.CompetitionGroup.Year,
                        term = s.Meta.CompetitionGroup.SemesterId,
                        id = s.Id,
                        name = s.Name,
                        students = db.ProjectSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                            .Union(db.ProjectSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                            .Union(db.ProjectSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                            .Select(m => new StudentInfoApiDto()
                            {
                                studentId = m.studentId,
                                groupId = m.Student.GroupId,
                                groupName = m.Student.Group.Name,
                                runpUid = db.Apploads
                                    .FirstOrDefault(appload => appload.year == year && appload.term == term && appload.grp == m.Student.GroupId
                                            && appload.DisciplineType == DisciplineType.Project && !appload.removed
                                            // сравниваем уровень дисциплины в appload и в проекте, на который зачислен студент. 
                                            // в appload и из modules соответственно: А = А, B = B, ВС = или В, или С, или BC
                                            && appload.Level.Contains(s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Level)
                                            && appload.action == s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer
                                            )
                                    .dckey,
                                roleId = db.ProjectAdmissions.FirstOrDefault(a => 
                                        a.ProjectId == s.Meta.ProjectDisciplineTmerPeriod.Period.Project.ModuleId
                                        && a.ProjectCompetitionGroupId == s.Meta.CompetitionGroupId
                                        && a.studentId == m.studentId
                                        && a.Published 
                                        && a.Status == AdmissionStatus.Admitted
                                        ).Role.EmployersId
                            }).ToList(),
                        teacherId = s.TeacherId,
                        studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                        competitionGroupName = s.Meta.CompetitionGroup.Name
                    }).ToList();
                
                return projectSubgroupMembership;
            }
        }
    }
}