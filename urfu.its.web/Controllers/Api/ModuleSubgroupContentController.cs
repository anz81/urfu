using System;
using System.Collections.Generic;
//using System.EnterpriseServices;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers.Api
{
    [IdentityBasicAuthentication]
    public class ModuleSubgroupContentController : BaseController
    {
        internal class ModuleSubgroupContent
        {
            public int id { get; set; }
            public int innerNumber { get; set; }
            public string moduleId { get; set; }
            public string disciplineKey { get; set; }
            public string kmer { get; set; }
            public int term { get; set; }
            public int eduyear { get; set; }
            public int? studentCourse { get; set; }
            public string competitionGroupName { get; set; }
            public IEnumerable<string> groups { get; set; }
            public ModuleTypeParam type { get; set; }
        }

        internal static Func<ModuleSubgroupContent, ModuleSubgroupContentApiDto> Selector = g =>
            new ModuleSubgroupContentApiDto
            {
                subgroupId = g.id,
                combinedKey2 = g.type == ModuleTypeParam.Minor || g.type == ModuleTypeParam.ITS
                                    ? ApiDtoFunctions.ToSubgroupKey(g.innerNumber, g.moduleId, g.disciplineKey, g.kmer, g.term, g.eduyear)
                                    : ApiDtoFunctions.ToSubgroupKey(g.innerNumber, g.moduleId, g.disciplineKey, g.kmer, g.term, g.eduyear, g.studentCourse, g.competitionGroupName),
                groups = g.groups
            };

        public IEnumerable<ModuleSubgroupContentApiDto> Get(int year, int term, ModuleTypeParam moduleType = ModuleTypeParam.All)
        {
            var semester = ModuleSubgroupsController.MapSemester(term);

            using (var db = new ApplicationDbContext())
            {
                var sectionfkQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.SectionFk ? SectionFKQuery(db, year, semester) : Enumerable.Empty<ModuleSubgroupContent>();
                var flQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.ForeignLanguage ? ForeignLanguageQuery(db, year, semester) : Enumerable.Empty<ModuleSubgroupContent>();
                var projectQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.Project ? ProjectQuery(db, year, semester) : Enumerable.Empty<ModuleSubgroupContent>();
                var pairedModuleQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.PairedModule ? PairedModuleQuery(db, year, semester) : Enumerable.Empty<ModuleSubgroupContent>();
                var MUPModuleQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.MUP ? MUPQuery(db, year, semester) : Enumerable.Empty<ModuleSubgroupContent>();
                var ITSQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.ITS ? ITSGroupsQuery(db, year, semester) : Enumerable.Empty<ModuleSubgroupContent>();

                var result = 
                    sectionfkQuery.ToList()
                        .Concat(flQuery.ToList())
                        .Concat(projectQuery.ToList())
                        .Concat(pairedModuleQuery.ToList())
                        .Concat(MUPModuleQuery.ToList())
                        .Concat(ITSQuery.ToList())
                    .Select(Selector);

                return result;
            }
        }

        internal static IEnumerable<ModuleSubgroupContent> SectionFKQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.SectionFKSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester)
                .Select(s => new ModuleSubgroupContent()
                {
                    id = s.Id,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.ModuleId,
                    disciplineKey = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    groups = s.Meta.CompetitionGroup.Groups.Select(g => g.Id),
                    type = ModuleTypeParam.SectionFk
                }).ToList();
        }

        internal static IEnumerable<ModuleSubgroupContent> ForeignLanguageQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.ForeignLanguageSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester)
                .Select(s => new ModuleSubgroupContent()
                {
                    id = s.Id,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.ModuleId,
                    disciplineKey = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    groups = s.Meta.CompetitionGroup.Groups.Select(g => g.Id),
                    type = ModuleTypeParam.ForeignLanguage
                }).ToList();
        }

        internal static IEnumerable<ModuleSubgroupContent> ProjectQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.ProjectSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                            && s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Source == Urfu.Its.Web.Models.Source.Project)
                .Select(s => new ModuleSubgroupContent()
                {
                    id = s.Id,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.ModuleId,
                    disciplineKey = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    groups = s.Meta.CompetitionGroup.Groups.Select(g => g.Id),
                    type = ModuleTypeParam.Project
                }).ToList();
        }

        internal static IEnumerable<ModuleSubgroupContent> PairedModuleQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.ProjectSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                             && s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type == "Парный модуль")
                .Select(s => new ModuleSubgroupContent()
                {
                    id = s.Id,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.ModuleId,
                    disciplineKey = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    groups = s.Meta.CompetitionGroup.Groups.Select(g => g.Id),
                    type = ModuleTypeParam.PairedModule
                }).ToList();
        }

        internal static IEnumerable<ModuleSubgroupContent> MUPQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.MUPSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester)
                .Select(s => new ModuleSubgroupContent()
                {
                    id = s.Id,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.MUPDisciplineTmerPeriod.Period.MUP.ModuleId,
                    disciplineKey = s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    groups = s.Meta.CompetitionGroup.Groups.Select(g => g.Id),
                    type = ModuleTypeParam.MUP
                }).ToList();
        }

        internal static IEnumerable<ModuleSubgroupContent> ITSGroupsQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.Subgroups
                .Where(s => s.Meta.Year == year && s.Meta.Term == semester)
                .Select(s => new ModuleSubgroupContent()
                {
                    id = s.Id,
                    innerNumber = s.InnerNumber,
                    studentCourse = s.Meta.Group.Course,
                    moduleId = s.Meta.moduleId,
                    disciplineKey = s.Meta.catalogDisciplineUuid,
                    eduyear = s.Meta.Year,
                    term = s.Meta.Term,
                    kmer = s.Meta.kmer,
                    groups = new List<string>() { s.Meta.groupId },
                    type = ModuleTypeParam.ITS
                }).ToList();
        }
    }
}