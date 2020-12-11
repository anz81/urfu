using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Web.DataContext;
using AutoMapper.Configuration;
//using WebGrease.Css.Extensions;

namespace Urfu.Its.Web.Controllers.Api
{
    [IdentityBasicAuthentication]
    public class SubgroupsController : BaseController
    {
        // GET api/<controller>
        public IEnumerable<SubgroupApiDto> Get(int? year = null, int? term = null, string division = null, string groupId = null, string detailDiscipline = null)
        {
            using (var db = new ApplicationDbContext())
            {
                ((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 300;
                var query = db.Subgroups.Select(s => new
                {
                    s,
                    load = db.Apploads.Where(a =>
                            a.action == s.Meta.kmer
                            && a.grp == s.Meta.groupId
                            && (a.eduDiscipline == s.Meta.disciplineUUID || a.eduDiscipline == s.Meta.additionalUUID)
                            && a.term == s.Meta.Term
                            && a.year == s.Meta.Year
                        )
                        .Select(a => new { a.dckey, a.detailDiscipline })
                        .FirstOrDefault(),
                    s.Meta.Year,
                    s.Meta,
                    moduleName = s.Meta.Module.title,
                    moduleNumber = s.Meta.Module.number,
                    count1 = db.SubgroupMemberships.Count(m => m.SubgroupId == s.Id),
                    count2 = db.SubgroupMemberships.Count(m => m.Subgroup.ParentId == s.Id),
                    count3 = db.SubgroupMemberships.Count(m => m.Subgroup.Parent.ParentId == s.Id),
                    s.Removed
                });

                if (year.HasValue)
                    query = query.Where(q => q.Year == year);
                if (term.HasValue)
                    query = query.Where(q => q.Meta.Term == term);
                if (!string.IsNullOrWhiteSpace(division))
                    query = query.Where(q => q.Meta.Group.FormativeDivisionId == division);
                if (!string.IsNullOrWhiteSpace(groupId))
                    query = query.Where(q => q.Meta.groupId == groupId);
                if (!string.IsNullOrWhiteSpace(detailDiscipline))
                    query = query.Where(q => q.load.detailDiscipline == detailDiscipline);
                return query.ToList().Select(
                    arg =>
                    {
                        var mce = new MapperConfigurationExpression();
                        var mc = new MapperConfiguration(mce);
                        var mapper = new Mapper(mc);
                        var dto = mapper.Map<SubgroupApiDto>(arg.s);
                        dto.studentCount = arg.count1 + arg.count2 + arg.count3;
                        dto.dckey = arg.load?.dckey;
                        dto.detailDiscipline = arg.load?.detailDiscipline;
                        dto.year = arg.Year;
                        dto.moduleName = arg.moduleName;
                        dto.moduleNumber = arg.moduleNumber;
                        dto.combinedKey = ApiDtoFunctions.ToSubgroupKey(dto.innerNumber, dto.groupId, dto.catalogDisciplineUuid, dto.kmer, dto.term, dto.year);
                        dto.combinedKey2 = ApiDtoFunctions.ToSubgroupKey(dto.innerNumber, dto.groupId, dto.catalogDisciplineUuid, dto.kmer, dto.term, dto.year);
                        return dto;
                    });
            }
        }
    }

    [IdentityBasicAuthentication]
    public class SubgroupMembershipsController : BaseController
    {
        // GET api/<controller>
        public IEnumerable<SubgroupWithMemebersApiDto> Get(int? year = null, int? term = null, string division = null,
            string groupId = null, string detailDiscipline = null)
        {
            using (var db = new ApplicationDbContext())
            {
                ((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 300;
                var query = db.Subgroups.Where(s => !s.Removed).Select(s => new
                {
                    s,
                    db.Apploads.FirstOrDefault(
                        a =>
                            a.action == s.Meta.kmer && a.grp == s.Meta.groupId &&
                            (a.eduDiscipline == s.Meta.disciplineUUID || a.eduDiscipline == s.Meta.additionalUUID) &&
                            a.term == s.Meta.Term && a.year == s.Meta.Year).dckey,
                    db.Apploads.FirstOrDefault(
                        a =>
                            a.action == s.Meta.kmer && a.grp == s.Meta.groupId &&
                            (a.eduDiscipline == s.Meta.disciplineUUID || a.eduDiscipline == s.Meta.additionalUUID) &&
                            a.term == s.Meta.Term && a.year == s.Meta.Year).detailDiscipline,
                    s.Meta.Year,
                    s.Meta,
                    moduleName = s.Meta.Module.title,

                    students1 = db.SubgroupMemberships.Where(m => m.SubgroupId == s.Id).Select(m => m.studentId),
                    students2 = db.SubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id).Select(m => m.studentId),
                    students3 =
                    db.SubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id).Select(m => m.studentId),

                    count1 = db.SubgroupMemberships.Count(m => m.SubgroupId == s.Id),
                    count2 = db.SubgroupMemberships.Count(m => m.Subgroup.ParentId == s.Id),
                    count3 = db.SubgroupMemberships.Count(m => m.Subgroup.Parent.ParentId == s.Id)
                });
                if (year.HasValue)
                    query = query.Where(q => q.Year == year);
                if (term.HasValue)
                    query = query.Where(q => q.Meta.Term == term);
                if (!string.IsNullOrWhiteSpace(division))
                    query = query.Where(q => q.Meta.Group.FormativeDivisionId == division);
                if (!string.IsNullOrWhiteSpace(groupId))
                    query = query.Where(q => q.Meta.groupId == groupId);
                if (!string.IsNullOrWhiteSpace(detailDiscipline))
                    query = query.Where(q => q.detailDiscipline == detailDiscipline);
                return query.ToList().Select(
                    arg =>
                    {
                        var mce = new MapperConfigurationExpression();
                        var mc = new MapperConfiguration(mce);
                        var mapper = new Mapper(mc);
                        var dto = mapper.Map<SubgroupWithMemebersApiDto>(arg.s);
                        dto.studentCount = arg.count1 + arg.count2 + arg.count3;
                        dto.dckey = arg.dckey;
                        dto.year = arg.Year;
                        dto.moduleName = arg.moduleName;
                        dto.detailDiscipline = arg.detailDiscipline;
                        dto.students =
                            arg.students1.ToList().Union(arg.students2.ToList().Union(arg.students3.ToList())).ToList();
                        dto.combinedKey = ApiDtoFunctions.ToSubgroupKey(dto.innerNumber, dto.groupId,
                            dto.catalogDisciplineUuid, dto.kmer, dto.term, dto.year);
                        dto.combinedKey2 = ApiDtoFunctions.ToSubgroupKey(dto.innerNumber, dto.groupId,
                            dto.catalogDisciplineUuid, dto.kmer, dto.term, dto.year);
                        return dto;
                    });
            }
        }

        public IEnumerable<StudentSubgroupMember> Get(string studentId)
        {
            using (var db = new ApplicationDbContext())
            {
                var sectionFks1 = db.SectionFKSubgroupMemberships.Include(_=>_.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline).Include(_=>_.Subgroup.Meta.CompetitionGroup).Where(_ => _.studentId == studentId).ToList();
                //var b = sectionFks1.Where(_ => _.Subgroup.Meta.SectionFKDisciplineTmerPeriod == null);

                var sectionFks = sectionFks1.ToList()
                    .Select(_ => new StudentSubgroupMember()
                    {
                        groupName = _.Subgroup.Name,
                        groupId = _.SubgroupId,
                        groupTypeId = SubgroupType.SectionFK,
                        groupKey = ApiDtoFunctions.ToSubgroupKey(_.Subgroup.InnerNumber, _.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.ModuleId, _.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey, _.Subgroup.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer, _.Subgroup.Meta.CompetitionGroup.SemesterId, _.Subgroup.Meta.CompetitionGroup.Year)
                    }).ToList();
                var fl = db.ForeignLanguageSubgroupMemberships.Include(_ => _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline).Include(_ => _.Subgroup.Meta.CompetitionGroup).Where(_ => _.studentId == studentId).ToList()
                    .Select(_ => new StudentSubgroupMember()
                    {
                        groupName = _.Subgroup.Name,
                        groupId = _.SubgroupId,
                        groupTypeId = SubgroupType.ForeignLanguage,
                        groupKey = ApiDtoFunctions.ToSubgroupKey(_.Subgroup.InnerNumber, _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.ModuleId, _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey, _.Subgroup.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer, _.Subgroup.Meta.CompetitionGroup.SemesterId, _.Subgroup.Meta.CompetitionGroup.Year)
                    }).ToList();
                var minor = db.MinorSubgroupMemberships.Include(_ => _.Subgroup.Meta.Period.Minor).Include(_ => _.Subgroup.Meta.Tmer.Discipline.Discipline).Where(_ => _.studentId == studentId).ToList()
                    .Select(_ => new StudentSubgroupMember()
                    {
                        groupName = _.Subgroup.Name,
                        groupId = _.SubgroupId,
                        groupTypeId = SubgroupType.Minor,
                        groupKey = ApiDtoFunctions.ToSubgroupKey(_.Subgroup.InnerNumber, _.Subgroup.Meta.Period.Minor.ModuleId, _.Subgroup.Meta.Tmer.Discipline.Discipline.pkey, _.Subgroup.Meta.Tmer.Tmer.kmer, _.Subgroup.Meta.Period.SemesterId, _.Subgroup.Meta.Period.Year)

                    }).ToList();
                var itsSubgroups = db.SubgroupMemberships.Include(_=>_.Subgroup.Meta).Where(_ => _.studentId == studentId).ToList()
                    .Select(_ => new StudentSubgroupMember()
                    {
                        groupName = _.Subgroup.Name,
                        groupId = _.SubgroupId,
                        groupTypeId = SubgroupType.ITSSubgroup,
                        groupKey = ApiDtoFunctions.ToSubgroupKey(_.Subgroup.InnerNumber, _.Subgroup.Meta.groupId, _.Subgroup.Meta.catalogDisciplineUuid, _.Subgroup.Meta.kmer, _.Subgroup.Meta.Term, _.Subgroup.Meta.Year)
                    }).ToList();
                var projectSubgroups = db.ProjectSubgroupMemberships.Include(_ => _.Subgroup.Meta.ProjectDisciplineTmerPeriod).Where(_ => _.studentId == studentId && _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Source == Urfu.Its.Web.Models.Source.Project).ToList()
                    .Select(_ => new StudentSubgroupMember()
                    {
                        groupName = _.Subgroup.Name,
                        groupId = _.SubgroupId,
                        groupTypeId = SubgroupType.Project,
                        groupKey = ApiDtoFunctions.ToSubgroupKey(_.Subgroup.InnerNumber, _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.ProjectId, _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey, _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer, _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.SemesterId, _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.Year)
                    }).ToList();
                var pairedModuleSubgroups = db.ProjectSubgroupMemberships.Include(_ => _.Subgroup.Meta.ProjectDisciplineTmerPeriod).Where(_ => _.studentId == studentId && _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type == "Парный модуль").ToList()
                    .Select(_ => new StudentSubgroupMember()
                    {
                        groupName = _.Subgroup.Name,
                        groupId = _.SubgroupId,
                        groupTypeId = SubgroupType.PairedModule,
                        groupKey = ApiDtoFunctions.ToSubgroupKey(_.Subgroup.InnerNumber, _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.ProjectId, _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey, _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer, _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.SemesterId, _.Subgroup.Meta.ProjectDisciplineTmerPeriod.Period.Year)
                    }).ToList();
                var mupSubgroups = db.MUPSubgroupMemberships.Include(_=>_.Subgroup.Meta.MUPDisciplineTmerPeriod).Where(_ => _.studentId == studentId && !_.Subgroup.Removed).ToList()
                    .Select(_ => new StudentSubgroupMember()
                    {
                        groupName = _.Subgroup.Name,
                        groupId = _.SubgroupId,
                        groupTypeId = SubgroupType.MUP,
                        groupKey = ApiDtoFunctions.ToSubgroupKey(_.Subgroup.InnerNumber, _.Subgroup.Meta.MUPDisciplineTmerPeriod.Period.MUPId, _.Subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey, _.Subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.kmer, _.Subgroup.Meta.MUPDisciplineTmerPeriod.Period.SemesterId, _.Subgroup.Meta.MUPDisciplineTmerPeriod.Period.Year)
                    }).ToList();
                var studentSubgroupMembers = sectionFks.Concat(fl).Concat(minor).Concat(itsSubgroups).Concat(projectSubgroups).Concat(pairedModuleSubgroups).Concat(mupSubgroups).ToList();
                studentSubgroupMembers.ForEach(_=>_.groupType = _.groupTypeId.ConvertToName());

                return studentSubgroupMembers;
            }
        }
    }

    public class StudentSubgroupMember
    {
        public SubgroupType groupTypeId { get; set; }
        public string groupType { get; set; }

        public int groupId { get; set; }
        public string groupName { get; set; }
        public string groupKey { get; set; }
    }
    public enum SubgroupType
    {
        [Display(Name = "физическая культура")]
        SectionFK = 1,
        [Display(Name = "группа ИЯ")]
        ForeignLanguage = 2,
        [Display(Name = "группа майноры")]
        Minor = 3,
        [Display(Name = "группы ИТС")]
        ITSSubgroup = 4,
        [Display(Name = "МУП")]
        MUP = 5,
        [Display(Name = "парный модуль")]
        PairedModule = 6,
        [Display(Name = "проектное обучение")]
        Project = 7
    }
}