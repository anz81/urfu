using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

namespace Urfu.Its.Web.Controllers.Api
{

    [IdentityBasicAuthentication]
    public class ModuleSubgroupsController : BaseController
    {
        internal class Subgroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int InnerNumber { get; set; }
            public int? ParentId { get; set; }
            public string moduleId { get; set; }
            public string moduleName { get; set; }
            public string moduleType { get; set; }
            public int? moduleNumber { get; set; }
            public string disciplineId { get; set; }
            public string disciplineName { get; set; }
            public string disciplineKey { get; set; }
            public int eduyear { get; set; }
            public int term { get; set; }
            public string kmer { get; set; }
            public string rmer { get; set; }
            public int groupCount { get; set; }
            public int studentsCount { get; set; }
            public int limit { get; set; }
            public IEnumerable<string> divisions { get; set; }
            public string teacherId { get; set; }
            public string CompetitionGroupName { get; set; }
            public string CompetitionGroupShortName { get; set; }
            public string chairId { get; set; }
            public int? Course { get; set; }
            public string Description { get; set; }
            public ModuleTypeParam type { get; set; }
            public int Removed { get; set; }
        }

        internal static readonly Func<Subgroup, MinorSubgroupApiDto> Selector = g =>
            new MinorSubgroupApiDto
            {
                id = g.Id,
                name = g.Name,
                parentId = g.ParentId,
                competitionGroupName = g.CompetitionGroupName,
                competitionGroupShortName = g.CompetitionGroupShortName,
                course = g.Course,
                eduyear = g.eduyear,
                term = g.term,
                moduleId = g.moduleId,
                moduleType = g.moduleType,
                moduleName = g.moduleName,
                moduleNumber = g.moduleNumber,
                disciplineId = g.disciplineId,
                disciplineName = g.disciplineName,
                loadTypeId = g.kmer,
                loadTypeName = g.rmer,
                groupCount = g.groupCount,
                limit = g.limit,
                studentCount = g.studentsCount,
                divisions = g.divisions.ToArray(),
                combinedKey = ApiDtoFunctions.ToSubgroupKey(g.InnerNumber, g.moduleId, g.disciplineKey, g.kmer, g.term, g.eduyear),
                combinedKey2 = g.type == ModuleTypeParam.Minor
                                    ? ApiDtoFunctions.ToSubgroupKey(g.InnerNumber, g.moduleId, g.disciplineKey, g.kmer, g.term, g.eduyear)
                                    : ApiDtoFunctions.ToSubgroupKey(g.InnerNumber, g.moduleId, g.disciplineKey, g.kmer, g.term, g.eduyear, g.Course, g.CompetitionGroupShortName),
                teacherId = g.teacherId,
                chairId = g.chairId,
                description = g.Description,
                removed = g.Removed
            };

        internal static int MapSemester(int term)
        {
            switch (term)
            {
                case 1: return 1;
                case 2: return 2;
                case 3: return 0;
            }
            throw new ArgumentOutOfRangeException("Допустимые значения параметра term: 1-осенний, 2-весенний, 3-прочий");
        }

        public IEnumerable<MinorSubgroupApiDto> Get(int year, int term, ModuleTypeParam moduleType = ModuleTypeParam.All)
        {
            var semester = MapSemester(term);

            using (var db = new ApplicationDbContext())
            {
                db.Database.SetCommandTimeout(300);
                var minorsQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.Minor ? MinorsQuery(db, year, semester) : Enumerable.Empty<Subgroup>();
                var sectionfkQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.SectionFk ? SectionFKQuery(db, year, semester) : Enumerable.Empty<Subgroup>();
                var flQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.ForeignLanguage ? ForeignLanguageQuery(db, year, semester) : Enumerable.Empty<Subgroup>();
                var projectQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.Project ? ProjectQuery(db, year, semester) : Enumerable.Empty<Subgroup>();
                var pairedModuleQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.PairedModule ? PairedModuleQuery(db, year, semester) : Enumerable.Empty<Subgroup>();
                var mupQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.MUP ? MUPQuery(db, year, semester) : Enumerable.Empty<Subgroup>();
                return minorsQuery.ToList().Concat(sectionfkQuery.ToList()).Concat(flQuery.ToList())
                    .Concat(projectQuery.ToList()).Concat(pairedModuleQuery.ToList()).Concat(mupQuery.ToList()).Select(Selector);
                    
            }
        }

        internal static IQueryable<Subgroup> MinorsQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.MinorSubgroups
                .Where(s => s.Meta.Period.Year == year && s.Meta.Period.SemesterId == semester)
                .Select(s => new Subgroup
                {
                    Id = s.Id,
                    Name = s.Name,
                    InnerNumber = s.InnerNumber,
                    ParentId = s.ParentId,
                    CompetitionGroupName = null,
                    CompetitionGroupShortName = null,
                    Course = null,
                    moduleId = s.Meta.Period.Minor.ModuleId,
                    moduleName = s.Meta.Period.Minor.Module.title,
                    moduleType = s.Meta.Period.Minor.Module.type,
                    moduleNumber = s.Meta.Period.Minor.Module.number,
                    disciplineId = s.Meta.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.Period.Year,
                    term = s.Meta.Period.SemesterId,
                    kmer = s.Meta.Tmer.Tmer.kmer,
                    rmer = s.Meta.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    studentsCount = db.MinorSubgroupMemberships.Count(m => m.SubgroupId == s.Id)
                                    + db.MinorSubgroupMemberships.Count(m => m.Subgroup.ParentId == s.Id)
                                    + db.MinorSubgroupMemberships.Count(m => m.Subgroup.Parent.ParentId == s.Id),
                    limit = s.Limit,
                    divisions = s.Meta.Divisions.Select(d => d.uuid),
                    teacherId = s.TeacherId,
                    chairId = "undich18hc2jg0000lpc17ajtm581pic",
                    Description = null,
                    type = ModuleTypeParam.Minor
                });
        }

        private static IQueryable<Subgroup> ForeignLanguageQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.ForeignLanguageSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester)
                .Select(s => new Subgroup
                {
                    Id = s.Id,
                    Name = s.Name,
                    InnerNumber = s.InnerNumber,
                    ParentId = s.ParentId,
                    CompetitionGroupName = s.Meta.CompetitionGroup.Name,
                    CompetitionGroupShortName = s.Meta.CompetitionGroup.ShortName,
                    Course = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.ModuleId,
                    moduleName = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.title,
                    moduleType = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.type,
                    moduleNumber = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.number,
                    disciplineId = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    studentsCount = db.ForeignLanguageSubgroupMemberships.Count(m => m.SubgroupId == s.Id)
                                  + db.ForeignLanguageSubgroupMemberships.Count(m => m.Subgroup.ParentId == s.Id)
                                  + db.ForeignLanguageSubgroupMemberships.Count(m => m.Subgroup.Parent.ParentId == s.Id),
                    limit = s.Limit,
                    divisions = s.Meta.ForeignLanguageDisciplineTmerPeriod.Divisions.Select(d => d.uuid),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.ForeignLanguageDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    Description = s.Description,
                    type = ModuleTypeParam.ForeignLanguage
                });
        }

        private static IQueryable<Subgroup> SectionFKQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.SectionFKSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester)
                .Select(s => new Subgroup
                {
                    Id = s.Id,
                    Name = s.Name,
                    InnerNumber = s.InnerNumber,
                    ParentId = s.ParentId,
                    CompetitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    CompetitionGroupShortName = s.Meta.CompetitionGroup.ShortName,
                    Course = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.ModuleId,
                    moduleName = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.title,
                    moduleType = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.type,
                    moduleNumber = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.number,
                    disciplineId = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    studentsCount = db.SectionFKSubgroupMemberships.Count(m => m.SubgroupId == s.Id)
                                    + db.SectionFKSubgroupMemberships.Count(m => m.Subgroup.ParentId == s.Id)
                                    + db.SectionFKSubgroupMemberships.Count(m => m.Subgroup.Parent.ParentId == s.Id),
                    limit = s.Limit,
                    divisions = s.Meta.SectionFKDisciplineTmerPeriod.Divisions.Select(d => d.uuid),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.SectionFKDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    Description = null,
                    type = ModuleTypeParam.SectionFk
                });
        }

        private static IQueryable<Subgroup> ProjectQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.ProjectSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Source == Urfu.Its.Web.Models.Source.Project
                    )
                .Select(s => new Subgroup
                {
                    Id = s.Id,
                    Name = s.Name,
                    InnerNumber = s.InnerNumber,
                    ParentId = s.ParentId,
                    CompetitionGroupName = s.Meta.CompetitionGroup.Name,
                    CompetitionGroupShortName = s.Meta.CompetitionGroup.ShortName,
                    Course = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.ModuleId,
                    moduleName = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title,
                    moduleType = "Проектное обучение",
                    moduleNumber = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.number,
                    disciplineId = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    studentsCount = db.MUPSubgroupMemberships.Count(m => m.SubgroupId == s.Id)
                                  + db.MUPSubgroupMemberships.Count(m => m.Subgroup.ParentId == s.Id)
                                  + db.MUPSubgroupMemberships.Count(m => m.Subgroup.Parent.ParentId == s.Id),
                    limit = s.Limit,
                    divisions = s.Meta.ProjectDisciplineTmerPeriod.Divisions.Select(d => d.uuid),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.ProjectDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    Description = null,
                    type = ModuleTypeParam.Project
                });
        }

        private static IQueryable<Subgroup> PairedModuleQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.ProjectSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type == "Парный модуль"
                    )
                .Select(s => new Subgroup
                {
                    Id = s.Id,
                    Name = s.Name,
                    InnerNumber = s.InnerNumber,
                    ParentId = s.ParentId,
                    CompetitionGroupName = s.Meta.CompetitionGroup.Name,
                    CompetitionGroupShortName = s.Meta.CompetitionGroup.ShortName,
                    Course = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.ModuleId,
                    moduleName = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title,
                    moduleType = "Парный модуль",
                    moduleNumber = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.number,
                    disciplineId = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    studentsCount = db.MUPSubgroupMemberships.Count(m => m.SubgroupId == s.Id)
                                  + db.MUPSubgroupMemberships.Count(m => m.Subgroup.ParentId == s.Id)
                                  + db.MUPSubgroupMemberships.Count(m => m.Subgroup.Parent.ParentId == s.Id),
                    limit = s.Limit,
                    divisions = s.Meta.ProjectDisciplineTmerPeriod.Divisions.Select(d => d.uuid),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.ProjectDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    Description = null,
                    type = ModuleTypeParam.PairedModule
                });
        }

        private static IQueryable<Subgroup> MUPQuery(ApplicationDbContext db, int year, int semester)
        {
            return db.MUPSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester)
                .Select(s => new Subgroup
                {
                    Id = s.Id,
                    Name = s.Name,
                    InnerNumber = s.InnerNumber,
                    ParentId = s.ParentId,
                    CompetitionGroupName = s.Meta.CompetitionGroup.Name,
                    CompetitionGroupShortName = s.Meta.CompetitionGroup.ShortName,
                    Course = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.MUPDisciplineTmerPeriod.Period.MUP.ModuleId,
                    moduleName = s.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.title,
                    moduleType = "МУП",
                    moduleNumber = s.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.number,
                    disciplineId = s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    studentsCount = db.MUPSubgroupMemberships.Count(m => m.SubgroupId == s.Id)
                                  + db.MUPSubgroupMemberships.Count(m => m.Subgroup.ParentId == s.Id)
                                  + db.MUPSubgroupMemberships.Count(m => m.Subgroup.Parent.ParentId == s.Id),
                    limit = s.Limit,
                    divisions = s.Meta.MUPDisciplineTmerPeriod.Divisions.Select(d => d.uuid),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.MUPDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    Description = s.Description,
                    type = ModuleTypeParam.MUP,
                    Removed = s.Removed ? 1:0
                });
        }
    }

    [IdentityBasicAuthentication]
    public class ModuleSubgroupMembershipsController : BaseController
    {
        internal class SubgroupMembers
        {
            public int id { get; set; }
            public string name { get; set; }
            public string moduleLevel { get; set; }
            public int innerNumber { get; set; }
            public string moduleId { get; set; }
            public string moduleName { get; set; }
            public string moduleType { get; set; }
            public string disciplineId { get; set; }
            public string disciplineName { get; set; }
            public string disciplineKey { get; set; }
            public int eduyear { get; set; }
            public int term { get; set; }
            public string kmer { get; set; }
            public string rmer { get; set; }
            public int groupCount { get; set; }
            public int studentCount { get { return students.Count(); } }
            public List<string> students { get; set; }
            public List<Division> divisions { get; set; }
            public string teacherId { get; set; }
            public int? studentCourse { get; set; }
            public string competitionGroupName { get; set; }
            public string chairId { get; set; }
            public List<Student> studentsObj { get; set; }
            public ModuleTypeParam type { get; set; }
            public List<StudentInfo> studentInfo { get; set; }
        }

        internal class StudentInfo
        {
            public string StudentId { get; set; }
            public string GroupId { get; set; }
            public string ForeignLanguageLevel { get; set; }
        }

        Mapper mapper;

        internal static Func<SubgroupMembers, MinorSubgroupWithMemebersApiDto> Selector = g =>
            new MinorSubgroupWithMemebersApiDto
            {
                id = g.id,
                studentCourse = g.studentCourse,
                name = g.name,
                competitionGroupName = g.competitionGroupName,
                eduyear = g.eduyear,
                term = g.term,
                moduleId = g.moduleId,
                moduleName = g.moduleName,
                moduleType = g.moduleType,
                disciplineId = g.disciplineId,
                disciplineName = g.disciplineName,
                loadTypeId = g.kmer,
                loadTypeName = g.rmer,
                groupCount = g.groupCount,
                studentCount = g.studentCount,
                students = g.students.ToArray(),
                divisions = g.divisions.Select(new Mapper(new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<AutoMapperConfig>();
                })).Map<DivisionApiDto>).ToArray(),
                combinedKey = ApiDtoFunctions.ToSubgroupKey(g.innerNumber, g.moduleId, g.disciplineKey, g.kmer, g.term, g.eduyear),
                combinedKey2 = g.type == ModuleTypeParam.Minor
                                    ? ApiDtoFunctions.ToSubgroupKey(g.innerNumber, g.moduleId, g.disciplineKey, g.kmer, g.term, g.eduyear)
                                    : ApiDtoFunctions.ToSubgroupKey(g.innerNumber, g.moduleId, g.disciplineKey, g.kmer, g.term, g.eduyear, g.studentCourse, g.competitionGroupName),
                teacherId = g.teacherId,
                chairId = g.chairId
            };

        public IEnumerable<MinorSubgroupWithMemebersApiDto> Get(int year, int term, ModuleTypeParam moduleType = ModuleTypeParam.All, string moduleName = null, string competitionGroup = null, bool newQuery = true)
        {

            Logger.Info($"Запрос ModuleSubgroupMemberships: moduleType={moduleType} year={year} term={term} moduleName={moduleName} competitionGroup={competitionGroup} newQuery={newQuery}");
            var semester = ModuleSubgroupsController.MapSemester(term);
            
            using (var db = new ApplicationDbContext())
            {
                db.Database.SetCommandTimeout(newQuery ? 900 : 4000);
                var minorsQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.Minor ? (newQuery ? MinorsQuery(db, year, semester, moduleName, competitionGroup) : MinorsOldQuery(db, year, semester, moduleName, competitionGroup)) : Enumerable.Empty<SubgroupMembers>();
                var sectionfkQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.SectionFk ? (newQuery ? SectionFKQuery(db, year, semester, moduleName, competitionGroup) : SectionFKOldQuery(db, year, semester, moduleName, competitionGroup)) : Enumerable.Empty<SubgroupMembers>();
                var flQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.ForeignLanguage ? (newQuery ? ForeignLanguageQuery(db, year, semester, moduleName, competitionGroup) : ForeignLanguageOldQuery(db, year, semester, moduleName, competitionGroup)) : Enumerable.Empty<SubgroupMembers>();
                var projectQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.Project ? (newQuery ? ProjectQuery(db, year, semester, moduleName, competitionGroup) : ProjectOldQuery(db, year, semester, moduleName, competitionGroup)) : Enumerable.Empty<SubgroupMembers>();
                var pairedModuleQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.PairedModule ? (newQuery ? PairedModuleQuery(db, year, semester, moduleName, competitionGroup) : PairedModuleOldQuery(db, year, semester, moduleName, competitionGroup)) : Enumerable.Empty<SubgroupMembers>();
                var MUPModuleQuery = moduleType == ModuleTypeParam.All || moduleType == ModuleTypeParam.MUP ? (newQuery ? MUPQuery(db, year, semester, moduleName, competitionGroup) : MUPOldQuery(db, year, semester, moduleName, competitionGroup)) : Enumerable.Empty<SubgroupMembers>();

                var result = minorsQuery.ToList().Concat(sectionfkQuery.ToList()).Concat(flQuery.ToList()).Concat(projectQuery.ToList()).Concat(pairedModuleQuery.ToList())
                    .Concat(MUPModuleQuery.ToList()).Select(Selector);

                return result;
            }

        }

        public Tuple<bool, IEnumerable<MinorSubgroupWithMemebersApiDto>, IEnumerable<MinorSubgroupWithMemebersApiDto>> Get(bool equal, int year, int term, ModuleTypeParam moduleType)
        {
            var oldList = Get(year, term, moduleType, newQuery: false);
            var newList = Get(year, term, moduleType, newQuery: true);

            var equals = oldList.SequenceEqual(newList);

            return new Tuple<bool, IEnumerable<MinorSubgroupWithMemebersApiDto>, IEnumerable<MinorSubgroupWithMemebersApiDto>>
                (equals, oldList, newList);
        }

        public HttpResponseMessage Get(bool excel, int year, int term, ModuleTypeParam moduleType = ModuleTypeParam.All, string moduleName = null, string competitionGroup = null, bool newQuery = true)
        {
            Logger.Info($"Запрос ModuleSubgroupMemberships: moduleType={moduleType} year={year} term={term} moduleName={moduleName} competitionGroup={competitionGroup}");
            var semester = ModuleSubgroupsController.MapSemester(term);

            using (var db = new ApplicationDbContext())
            {
                var resultList = Get(year, term, moduleType, moduleName, competitionGroup, newQuery);

                var filename = $"ModuleSubgroupMembership newQuery={newQuery} moduleType={(int)moduleType} year={year} term={term}.xlsx";
                var stream = new VariantExport().Export(new { Rows = resultList },
                    "ModuleSubgroupMembership.xlsx") as MemoryStream;

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.ToArray())
                };

                result.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = filename
                    };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                return result;
            }
        }
        
        internal static IEnumerable<SubgroupMembers> SectionFKQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            var apploads = db.Apploads.Where(appload => appload.year == year && appload.term == semester && 
                                appload.DisciplineType == DisciplineType.SectionFK && !appload.removed).ToList();

            var subgroups =  db.SectionFKSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && (competitionGroup == null || s.Meta.CompetitionGroup.ShortName == competitionGroup)
                    && (moduleName == null || s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.title == moduleName))
                .Select(s => new SubgroupMembers
                {
                    id = s.Id,
                    studentInfo = s.Students
                        .Union(s.Subgroups.SelectMany(child => child.Students))
                        .Union(s.Subgroups.SelectMany(child => child.Subgroups).SelectMany(child => child.Students))
                        .Select(st => new StudentInfo() {
                            StudentId = st.studentId,
                            GroupId = st.Student.GroupId }
                        )
                        .ToList(),
                    name = s.Name,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.ModuleId,
                    moduleName = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.title,
                    moduleType = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.type,
                    disciplineId = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    divisions = s.Meta.SectionFKDisciplineTmerPeriod.Divisions.ToList(),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.SectionFKDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    type = ModuleTypeParam.SectionFk
                }).ToList();

            foreach(var s in subgroups)
            {
                s.students = s.studentInfo.Select(sg => string.Concat(sg.StudentId, ",",
                    apploads.Where(appload => appload.grp == sg.GroupId).FirstOrDefault()?.detailDiscipline)).ToList();                
            }
            
            return subgroups;
        }

        internal static IQueryable<SubgroupMembers> SectionFKOldQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            
            return db.SectionFKSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && (competitionGroup == null || s.Meta.CompetitionGroup.ShortName == competitionGroup)
                    && (moduleName == null || s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.title == moduleName))
                .Select(s => new SubgroupMembers
                {
                    id = s.Id,
                    name = s.Name,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.ModuleId,
                    moduleName = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.title,
                    moduleType = s.Meta.SectionFKDisciplineTmerPeriod.Period.SectionFK.Module.type,
                    disciplineId = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.SectionFKDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    //studentCount = db.SectionFKSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                    //    .Union(db.SectionFKSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                    //    .Union(db.SectionFKSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                    //    .Select(m => m.studentId).Count(),
                    students = db.SectionFKSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                        .Union(db.SectionFKSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                        .Union(db.SectionFKSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                        .Select(m => m.studentId + "," +
                                                                 //db.Plans.Where(_ => _.eduplanNumber == m.Student.planVerion && _.versionNumber == m.Student.versionNumber &&
                                                                 //                                _.disciplineTitle == "Прикладная физическая культура" //|| _.disciplineTitle == "Физическая культура"
                                                                 //                                )
                                                                 //                                .Select(_ => db.Apploads.Where(appload => appload.eduDiscipline == _.disciplineUUID && appload.year == year && appload.term == semester)
                                                                 //                                    .Select(appload => appload.detailDiscipline).FirstOrDefault())
                                                                 //                                    .Concat(db.StudentPlans.Where(sp => sp.StudentId == m.Student.Id)
                                                                 //                                    .SelectMany(sp => db.Plans
                                                                 //                                                            .Where(p=>p.eduplanNumber == sp.PlanNumber && p.versionNumber == sp.VersionNumber && p.disciplineTitle == "Прикладная физическая культура" )
                                                                 //                                                            .Select(p=>db.Apploads.Where(appload=> appload.eduDiscipline == p.disciplineUUID && appload.year == year && appload.term == semester)
                                                                 //                                                            .Select(a=>a.detailDiscipline).FirstOrDefault()))
                                                                 //                                    )
                                                                 //.Concat(

                                                                 db.Apploads.Where(appload => appload.year == year && appload.term == semester && appload.grp == m.Student.GroupId && appload.DisciplineType == DisciplineType.SectionFK && !appload.removed)
                                                                .Select(appload => appload.detailDiscipline).FirstOrDefault()).ToList(),
                    divisions = s.Meta.SectionFKDisciplineTmerPeriod.Divisions.ToList(),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.SectionFKDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    type = ModuleTypeParam.SectionFk
                });
        }

        internal static List<SubgroupMembers> MinorsQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            var apploads = db.Apploads.Where(appload => appload.year == year && appload.term == semester
                                    && appload.DisciplineType == DisciplineType.Minor && !appload.removed).ToList();

            var subgroupMemberses = db.MinorSubgroups
                 .Where(s => s.Meta.Period.Year == year && s.Meta.Period.SemesterId == semester && (moduleName == null || s.Meta.Period.Minor.Module.title == moduleName))
                 .Select(s => new SubgroupMembers()
                 {
                     id = s.Id,
                     name = s.Name,
                     innerNumber = s.InnerNumber,
                     competitionGroupName = null,
                     moduleId = s.Meta.Period.Minor.ModuleId,
                     moduleName = s.Meta.Period.Minor.Module.title,
                     moduleType = s.Meta.Period.Minor.Module.type,
                     disciplineId = s.Meta.Tmer.Discipline.Discipline.uid,
                     disciplineName = s.Meta.Tmer.Discipline.Discipline.title,
                     disciplineKey = s.Meta.Tmer.Discipline.Discipline.pkey,
                     eduyear = s.Meta.Period.Year,
                     term = s.Meta.Period.SemesterId,
                     kmer = s.Meta.Tmer.Tmer.kmer,
                     rmer = s.Meta.Tmer.Tmer.rmer,
                     groupCount = s.Meta.GroupCount,
                     studentsObj = s.Students
                        .Union(s.Subgroups.SelectMany(child => child.Students))
                        .Union(s.Subgroups.SelectMany(child => child.Subgroups).SelectMany(child => child.Students))
                        .Select(m => m.Student).ToList(),
                     divisions = s.Meta.Divisions.ToList(),
                     teacherId = s.TeacherId,
                     chairId = "undich18hc2jg0000lpc17ajtm581pic",
                     type = ModuleTypeParam.Minor

                 }).ToList();

            var studentMinorDict = new Dictionary<string, Dictionary<string, string>>();
            foreach (var subgroupMemberse in subgroupMemberses)
            {
                subgroupMemberse.students = new List<string>();
                foreach (var subgroupMemberseStudent in subgroupMemberse.studentsObj)
                {
                    var toSkip = studentMinorDict.ContainsKey(subgroupMemberseStudent.Id) ? studentMinorDict[subgroupMemberseStudent.Id].Count(_ => _.Key != subgroupMemberse.moduleId) : 0;
                    string detailDiscipline = "";
                    if (studentMinorDict.Any(_ => _.Key == subgroupMemberseStudent.Id && _.Value.Any(d => d.Key == subgroupMemberse.moduleId)))
                        detailDiscipline = studentMinorDict[subgroupMemberseStudent.Id][subgroupMemberse.moduleId];
                    else
                        detailDiscipline = apploads.Where(appload => appload.grp == subgroupMemberseStudent.GroupId && appload.action == subgroupMemberse.kmer)
                            .Select(appload => appload.detailDiscipline).OrderBy(s => s).Skip(toSkip).FirstOrDefault();
                    if (!studentMinorDict.ContainsKey(subgroupMemberseStudent.Id) || !studentMinorDict[subgroupMemberseStudent.Id].ContainsKey(subgroupMemberse.moduleId) || studentMinorDict[subgroupMemberseStudent.Id].Any(_ => _.Key == subgroupMemberse.moduleId && studentMinorDict[subgroupMemberseStudent.Id][subgroupMemberse.moduleId] == detailDiscipline))
                    {
                        subgroupMemberse.students.Add(subgroupMemberseStudent.Id + "," + detailDiscipline);
                    }

                    if (!string.IsNullOrEmpty(detailDiscipline) && !studentMinorDict.Any(_ => _.Key == subgroupMemberseStudent.Id && _.Value.Any(d => d.Key == subgroupMemberse.moduleId)))
                    {
                        if (!studentMinorDict.ContainsKey(subgroupMemberseStudent.Id))
                        {
                            studentMinorDict.Add(subgroupMemberseStudent.Id, new Dictionary<string, string>());
                        }
                        studentMinorDict[subgroupMemberseStudent.Id].Add(subgroupMemberse.moduleId, detailDiscipline);
                    }
                }
            }

            return subgroupMemberses;
        }

        internal static List<SubgroupMembers> MinorsOldQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            var subgroupMemberses = db.MinorSubgroups
                 .Where(s => s.Meta.Period.Year == year && s.Meta.Period.SemesterId == semester && (moduleName == null || s.Meta.Period.Minor.Module.title == moduleName))
                 .Select(s => new SubgroupMembers()
                 {
                     id = s.Id,
                     name = s.Name,
                     innerNumber = s.InnerNumber,
                     competitionGroupName = null,
                     moduleId = s.Meta.Period.Minor.ModuleId,
                     moduleName = s.Meta.Period.Minor.Module.title,
                     moduleType = s.Meta.Period.Minor.Module.type,
                     disciplineId = s.Meta.Tmer.Discipline.Discipline.uid,
                     disciplineName = s.Meta.Tmer.Discipline.Discipline.title,
                     disciplineKey = s.Meta.Tmer.Discipline.Discipline.pkey,
                     eduyear = s.Meta.Period.Year,
                     term = s.Meta.Period.SemesterId,
                     kmer = s.Meta.Tmer.Tmer.kmer,
                     rmer = s.Meta.Tmer.Tmer.rmer,
                     groupCount = s.Meta.GroupCount,
                     //studentCount =
                     //db.MinorSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                     //    .Union(db.MinorSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                     //    .Union(db.MinorSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                     //    .Select(m => m.studentId).Count(),
                     studentsObj = db.MinorSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                         .Union(db.MinorSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                         .Union(db.MinorSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                         .Select(m => m.Student).ToList(),//+ db.Apploads.Where(appload => appload.year == year && appload.term == semester && appload.grp == m.Student.GroupId && appload.DisciplineType == DisciplineType.Minor)
                                                          //                                        //.Where(appload=> appload.dckey.Substring(33, 32) == s.Meta.Tmer.Discipline.Discipline.pkey)
                                                          //                                        .Select(appload => appload.detailDiscipline).FirstOrDefault()),

                     divisions = s.Meta.Divisions.ToList(),
                     teacherId = s.TeacherId,
                     chairId = "undich18hc2jg0000lpc17ajtm581pic",
                     type = ModuleTypeParam.Minor

                 }).ToList();

            var studentMinorDict = new Dictionary<string, Dictionary<string, string>>();
            foreach (var subgroupMemberse in subgroupMemberses)
            {
                subgroupMemberse.students = new List<string>();
                foreach (var subgroupMemberseStudent in subgroupMemberse.studentsObj)
                {
                    var toSkip = studentMinorDict.ContainsKey(subgroupMemberseStudent.Id) ? studentMinorDict[subgroupMemberseStudent.Id].Count(_ => _.Key != subgroupMemberse.moduleId) : 0;
                    string detailDiscipline = "";
                    if (studentMinorDict.Any(_ => _.Key == subgroupMemberseStudent.Id && _.Value.Any(d => d.Key == subgroupMemberse.moduleId)))
                        detailDiscipline = studentMinorDict[subgroupMemberseStudent.Id][subgroupMemberse.moduleId];
                    else
                        detailDiscipline = db.Apploads.Where(appload =>
                            appload.year == year && appload.term == semester &&
                            appload.grp == subgroupMemberseStudent.GroupId &&
                            appload.DisciplineType == DisciplineType.Minor && appload.action == subgroupMemberse.kmer && !appload.removed)
                            .Select(appload => appload.detailDiscipline).OrderBy(s => s).Skip(toSkip).FirstOrDefault();
                    if (!studentMinorDict.ContainsKey(subgroupMemberseStudent.Id) || !studentMinorDict[subgroupMemberseStudent.Id].ContainsKey(subgroupMemberse.moduleId) || studentMinorDict[subgroupMemberseStudent.Id].Any(_ => _.Key == subgroupMemberse.moduleId && studentMinorDict[subgroupMemberseStudent.Id][subgroupMemberse.moduleId] == detailDiscipline))
                    {
                        subgroupMemberse.students.Add(subgroupMemberseStudent.Id + "," + detailDiscipline);

                    }

                    if (!string.IsNullOrEmpty(detailDiscipline) && !studentMinorDict.Any(_ => _.Key == subgroupMemberseStudent.Id && _.Value.Any(d => d.Key == subgroupMemberse.moduleId)))
                    {
                        if (!studentMinorDict.ContainsKey(subgroupMemberseStudent.Id))
                        {
                            studentMinorDict.Add(subgroupMemberseStudent.Id, new Dictionary<string, string>());
                            //studentMinorDict[subgroupMemberseStudent.Id].Add(subgroupMemberse.moduleId, detailDiscipline);
                        }
                        studentMinorDict[subgroupMemberseStudent.Id].Add(subgroupMemberse.moduleId, detailDiscipline);
                    }
                }
            }

            return subgroupMemberses;
        }

        
        internal static IEnumerable<SubgroupMembers> ForeignLanguageQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            var apploads = db.Apploads.Where(appload => appload.year == year && appload.term == semester
                        && appload.DisciplineType == DisciplineType.ForeignLanguage && !appload.removed).ToList();

            var ratingCoefficients = db.RatingCoefficients.Where(c => c.ModuleType == (int)ModuleTypeParam.ForeignLanguage && c.Year == year).ToList();

            var subgroups = db.ForeignLanguageSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && (competitionGroup == null || s.Meta.CompetitionGroup.ShortName == competitionGroup)
                    && (moduleName == null || s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.title == moduleName))
                .Select(s => new SubgroupMembers
                {
                    id = s.Id,
                    studentInfo = s.Students
                        .Union(s.Subgroups.SelectMany(child => child.Students))
                        .Union(s.Subgroups.SelectMany(child => child.Subgroups)
                                        .SelectMany(child => child.Students))
                        .Select(m => new StudentInfo()
                        {
                            StudentId = m.studentId,
                            GroupId = m.Student.GroupId,
                            ForeignLanguageLevel = m.Student.ForeignLanguageLevel ?? ""
                        })
                        .ToList(),
                    name = s.Name,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.ModuleId,
                    moduleName = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.title,
                    moduleType = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.type,
                    disciplineId = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    divisions = s.Meta.ForeignLanguageDisciplineTmerPeriod.Divisions.ToList(),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.ForeignLanguageDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    type = ModuleTypeParam.ForeignLanguage

                }).ToList();

            foreach(var subgroup in subgroups)
            {
                subgroup.students = subgroup.studentInfo.Select(s => string.Concat(s.StudentId, ",",
                    apploads.Where(appload => appload.grp == s.GroupId).FirstOrDefault()?.detailDiscipline,
                    ",",
                    ratingCoefficients.FirstOrDefault(c => s.ForeignLanguageLevel.Contains(c.Level))?.Coefficient.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) ?? "1"
                    )).ToList();
            }

            return subgroups;
        }
        
        internal static IEnumerable<SubgroupMembers> ForeignLanguageOldQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {

            return db.ForeignLanguageSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && (competitionGroup == null || s.Meta.CompetitionGroup.ShortName == competitionGroup)
                    && (moduleName == null || s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.title == moduleName))
                .Select(s => new SubgroupMembers
                {
                    id = s.Id,
                    name = s.Name,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.ModuleId,
                    moduleName = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.title,
                    moduleType = s.Meta.ForeignLanguageDisciplineTmerPeriod.Period.ForeignLanguage.Module.type,
                    disciplineId = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.ForeignLanguageDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    //studentCount =
                    //db.ForeignLanguageSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                    //    .Union(db.ForeignLanguageSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                    //    .Union(db.ForeignLanguageSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                    //    .Select(m => m.studentId).Count(),
                    students = db.ForeignLanguageSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                        .Union(db.ForeignLanguageSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                        .Union(db.ForeignLanguageSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                        .Select(m => m.studentId + "," + db.Apploads.Where(appload => appload.year == year && appload.term == semester && appload.grp == m.Student.GroupId && appload.DisciplineType == DisciplineType.ForeignLanguage && !appload.removed)
                                                                .Select(appload => appload.detailDiscipline).FirstOrDefault() + "," +
                                                              (db.RatingCoefficients.Any(c => c.ModuleType == (int)ModuleTypeParam.ForeignLanguage && c.Year == year && db.Students.FirstOrDefault(_ => _.Id == m.studentId).ForeignLanguageLevel.Contains(c.Level)) ?
                                                                db.RatingCoefficients.FirstOrDefault(c => c.ModuleType == (int)ModuleTypeParam.ForeignLanguage && c.Year == year && db.Students.FirstOrDefault(_ => _.Id == m.studentId).ForeignLanguageLevel.Contains(c.Level)).Coefficient
                                                                : 1)
                                                                ).ToList(),
                    divisions = s.Meta.ForeignLanguageDisciplineTmerPeriod.Divisions.ToList(),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.ForeignLanguageDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    type = ModuleTypeParam.ForeignLanguage

                });
        }
        
        internal static IEnumerable<SubgroupMembers> ProjectQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            var apploads = db.Apploads.Where(appload => appload.year == year && appload.term == semester &&
                                appload.DisciplineType == DisciplineType.Project && !appload.removed).ToList();

            var ratingCoefficients = db.RatingCoefficients.Where(c => c.ModuleType == (int)ModuleTypeParam.Project && c.Year == year).ToList();

            var subgroups = db.ProjectSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && (competitionGroup == null || s.Meta.CompetitionGroup.ShortName == competitionGroup)
                    && (moduleName == null || s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title == moduleName)
                    && s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Source == Urfu.Its.Web.Models.Source.Project)
                .Select(s => new SubgroupMembers
                {
                    id = s.Id,
                    studentInfo = s.Students
                        .Union(s.Subgroups.SelectMany(child => child.Students))
                        .Union(s.Subgroups.SelectMany(child => child.Subgroups).SelectMany(child => child.Students))
                        .Select(m => new StudentInfo()
                        {
                            StudentId = m.studentId,
                            GroupId = m.Student.GroupId
                        })
                        .ToList(),
                    name = s.Name,
                    innerNumber = s.InnerNumber,
                    moduleLevel = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Level,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.ModuleId,
                    moduleName = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title,
                    moduleType = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type,
                    disciplineId = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    divisions = s.Meta.ProjectDisciplineTmerPeriod.Divisions.ToList(),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.ProjectDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    type = ModuleTypeParam.Project
                }).ToList();

            foreach (var subgroup in subgroups)
            {
                subgroup.students = subgroup.studentInfo.Select(s => string.Concat(s.StudentId, ",",
                    apploads.Where(appload => appload.grp == s.GroupId && appload.Level.Contains(subgroup.moduleLevel)
                                    ).FirstOrDefault()?.detailDiscipline,
                    ",",
                    ratingCoefficients.FirstOrDefault(c => c.Level == subgroup.moduleLevel)?.Coefficient.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) ?? "1")
                    ) .ToList();
            }

            return subgroups;
        }
        
        internal static IQueryable<SubgroupMembers> ProjectOldQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            return db.ProjectSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && (competitionGroup == null || s.Meta.CompetitionGroup.ShortName == competitionGroup)
                    && (moduleName == null || s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title == moduleName)
                    && s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Source == Urfu.Its.Web.Models.Source.Project
                    )
                .Select(s => new SubgroupMembers
                {
                    id = s.Id,
                    name = s.Name,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.ModuleId,
                    moduleName = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title,
                    moduleType = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type,
                    disciplineId = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    //studentCount =
                    //db.ProjectSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                    //    .Union(db.ProjectSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                    //    .Union(db.ProjectSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                    //    .Select(m => m.studentId).Count(),
                    students = db.ProjectSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                        .Union(db.ProjectSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                        .Union(db.ProjectSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                        .Select(m => m.studentId + "," +
                                db.Apploads
                                .Where(appload => appload.year == year && appload.term == semester && appload.grp == m.Student.GroupId
                                        && appload.DisciplineType == DisciplineType.Project && !appload.removed
                                        // сравниваем уровень дисциплины в appload и в проекте, на который зачислен студент. 
                                        // в appload и из modules соответственно: А = А, B = B, ВС = или В, или С, или BC
                                        && appload.Level.Contains(db.ProjectAdmissions.FirstOrDefault(adm =>
                                                adm.studentId == m.studentId && adm.Status == AdmissionStatus.Admitted
                                                && adm.ProjectCompetitionGroup.Year == year && adm.ProjectCompetitionGroup.SemesterId == semester
                                                ).Project.Module.Level)
                                        )
                                .Select(appload => appload.detailDiscipline).FirstOrDefault()
                                + "," + (db.RatingCoefficients.Any(c => c.ModuleType == (int)ModuleTypeParam.Project && c.Year == year 
                                                                    && c.Level == s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Level
                                                 ) ? db.RatingCoefficients.FirstOrDefault(c => c.ModuleType == (int)ModuleTypeParam.Project && c.Year == year
                                                                    && c.Level == s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.Level).Coefficient : 1)
                                ).ToList(),
                    divisions = s.Meta.ProjectDisciplineTmerPeriod.Divisions.ToList(),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.ProjectDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    type = ModuleTypeParam.Project
                });
        }

        internal static IEnumerable<SubgroupMembers> PairedModuleQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            var apploads = db.Apploads.Where(appload => appload.year == year && appload.term == semester &&
                    appload.DisciplineType == DisciplineType.PairedModule && !appload.removed).ToList();

            var subgroups = db.ProjectSubgroups
               .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                   && (competitionGroup == null || s.Meta.CompetitionGroup.ShortName == competitionGroup)
                   && (moduleName == null || s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title == moduleName)
                   && s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type == "Парный модуль")
               .Select(s => new SubgroupMembers
               {
                   id = s.Id,
                   studentInfo = s.Students
                        .Union(s.Subgroups.SelectMany(child => child.Students))
                        .Union(s.Subgroups.SelectMany(child => child.Subgroups).SelectMany(child => child.Students))
                        .Select(m => new StudentInfo()
                        {
                            StudentId = m.studentId,
                            GroupId = m.Student.GroupId
                        })
                        .ToList(),
                   name = s.Name,
                   innerNumber = s.InnerNumber,
                   competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                   studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                   moduleId = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.ModuleId,
                   moduleName = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title,
                   moduleType = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type,
                   disciplineId = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                   disciplineName = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                   disciplineKey = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                   eduyear = s.Meta.CompetitionGroup.Year,
                   term = s.Meta.CompetitionGroup.SemesterId,
                   kmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer,
                   rmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer,
                   groupCount = s.Meta.GroupCount,
                   divisions = s.Meta.ProjectDisciplineTmerPeriod.Divisions.ToList(),
                   teacherId = s.TeacherId,
                   chairId = s.Meta.ProjectDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                   type = ModuleTypeParam.PairedModule
               }).ToList();
            
            foreach (var subgroup in subgroups)
            {
                subgroup.students = subgroup.studentInfo.Select(st => string.Concat(st.StudentId, ",", 
                        apploads.FirstOrDefault(appload => appload.grp == st.GroupId)?.detailDiscipline))
                        .ToList();
            }
            
            return subgroups;
        }

        internal static IQueryable<SubgroupMembers> PairedModuleOldQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            return db.ProjectSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && (competitionGroup == null || s.Meta.CompetitionGroup.ShortName == competitionGroup)
                    && (moduleName == null || s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title == moduleName)
                    && s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type == "Парный модуль"
                    )
                .Select(s => new SubgroupMembers
                {
                    id = s.Id,
                    name = s.Name,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.ModuleId,
                    moduleName = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.title,
                    moduleType = s.Meta.ProjectDisciplineTmerPeriod.Period.Project.Module.type,
                    disciplineId = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.ProjectDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    //studentCount =
                    //db.ProjectSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                    //    .Union(db.ProjectSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                    //    .Union(db.ProjectSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                    //    .Select(m => m.studentId).Count(),
                    students = db.ProjectSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                        .Union(db.ProjectSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                        .Union(db.ProjectSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                        .Select(m => m.studentId + "," + db.Apploads.Where(appload => appload.year == year && appload.term == semester && appload.grp == m.Student.GroupId && appload.DisciplineType == DisciplineType.PairedModule && !appload.removed)
                                                                .Select(appload => appload.detailDiscipline).FirstOrDefault()).ToList(),
                    divisions = s.Meta.ProjectDisciplineTmerPeriod.Divisions.ToList(),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.ProjectDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    type = ModuleTypeParam.PairedModule

                });
        }
        
        internal static IEnumerable<SubgroupMembers> MUPQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            var apploads = db.Apploads.Where(appload => appload.year == year && appload.term == semester
                                && appload.DisciplineType == DisciplineType.MUP && !appload.removed).ToList();

            var ratingCoefficients = db.RatingCoefficients.Where(c => c.Year == year && c.ModuleType == (int)ModuleTypeParam.MUP).ToList();
            var MUPDisciplineConnections = db.MUPDisciplineConnections.ToList();

            var subgroups = db.MUPSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && (competitionGroup == null || s.Meta.CompetitionGroup.ShortName == competitionGroup)
                    && (moduleName == null || s.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.title == moduleName) && !s.Removed)
                .Select(s => new SubgroupMembers
                {
                    id = s.Id,
                    studentInfo = s.Students
                        .Union(s.Subgroups.SelectMany(child => child.Students))
                        .Union(s.Subgroups.SelectMany(child => child.Subgroups).SelectMany(child => child.Students))
                        .Select(m => new StudentInfo()
                        {
                            StudentId = m.studentId,
                            GroupId = m.Student.GroupId
                        })
                        .ToList(),
                    name = s.Name,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.MUPDisciplineTmerPeriod.Period.MUP.ModuleId,
                    moduleName = s.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.title,
                    moduleType = "МУП",
                    moduleLevel = s.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.Level,
                    disciplineId = s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    divisions = s.Meta.MUPDisciplineTmerPeriod.Divisions.ToList(),
                    teacherId = s.TeacherId,
                    type = ModuleTypeParam.MUP
                }).ToList();

            foreach(var subgroup in subgroups)
            {
                subgroup.students = subgroup.studentInfo.Select(s => string.Concat(s.StudentId, 
                    ",",
                    apploads.FirstOrDefault(appload => appload.grp == s.GroupId && MUPDisciplineConnections.Any(c =>
                            appload.dckey.Contains(c.Discipline.pkey) && c.ModuleMUPId == subgroup.moduleId))
                        ?.detailDiscipline,
                    ",",
                    ratingCoefficients.FirstOrDefault(c => c.Level == subgroup.moduleLevel && c.ModuleId == subgroup.moduleId)?.Coefficient.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) ?? "1")
                    ).ToList();

                subgroup.chairId = subgroup.divisions?.FirstOrDefault()?.uuid;
            }

            return subgroups;
        }

        internal static IEnumerable<SubgroupMembers> MUPOldQuery(ApplicationDbContext db, int year, int semester, string moduleName = null, string competitionGroup = null)
        {
            return db.MUPSubgroups
                .Where(s => s.Meta.CompetitionGroup.Year == year && s.Meta.CompetitionGroup.SemesterId == semester
                    && (competitionGroup == null || s.Meta.CompetitionGroup.ShortName == competitionGroup)
                    && (moduleName == null || s.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.title == moduleName) && !s.Removed)
                .Select(s => new SubgroupMembers
                {
                    id = s.Id,
                    name = s.Name,
                    innerNumber = s.InnerNumber,
                    competitionGroupName = s.Meta.CompetitionGroup.ShortName,
                    studentCourse = s.Meta.CompetitionGroup.StudentCourse,
                    moduleId = s.Meta.MUPDisciplineTmerPeriod.Period.MUP.ModuleId,
                    moduleName = s.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.title,
                    moduleType = "МУП",
                    disciplineId = s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.uid,
                    disciplineName = s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.title,
                    disciplineKey = s.Meta.MUPDisciplineTmerPeriod.Tmer.Discipline.Discipline.pkey,
                    eduyear = s.Meta.CompetitionGroup.Year,
                    term = s.Meta.CompetitionGroup.SemesterId,
                    kmer = s.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.kmer,
                    rmer = s.Meta.MUPDisciplineTmerPeriod.Tmer.Tmer.rmer,
                    groupCount = s.Meta.GroupCount,
                    //studentCount =
                    //db.MUPSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                    //    .Union(db.MUPSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                    //    .Union(db.MUPSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                    //    .Select(m => m.studentId).Count(),
                    students = db.MUPSubgroupMemberships.Where(m => m.SubgroupId == s.Id)
                        .Union(db.MUPSubgroupMemberships.Where(m => m.Subgroup.ParentId == s.Id))
                        .Union(db.MUPSubgroupMemberships.Where(m => m.Subgroup.Parent.ParentId == s.Id))
                        .Select(m => m.studentId + "," + db.Apploads.Where(appload => appload.year == year && appload.term == semester
                                                        && appload.grp == m.Student.GroupId && appload.DisciplineType == DisciplineType.MUP && !appload.removed
                                                        && db.MUPDisciplineConnections.Any(c =>
                                                                    appload.dckey.Contains(c.Discipline.pkey) && c.ModuleMUPId == s.Meta.MUPDisciplineTmerPeriod.Period.MUP.ModuleId))
                                    .Select(appload => appload.detailDiscipline).FirstOrDefault()
                                    + "," + (db.RatingCoefficients.Any(c => c.Year == year && c.ModuleType == (int)ModuleTypeParam.MUP && c.Level == s.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.Level && c.ModuleId == s.Meta.MUPDisciplineTmerPeriod.Period.MUP.ModuleId)
                                              ?
                                              db.RatingCoefficients.FirstOrDefault(c => c.Year == year && c.ModuleType == (int)ModuleTypeParam.MUP && c.Level == s.Meta.MUPDisciplineTmerPeriod.Period.MUP.Module.Level && c.ModuleId == s.Meta.MUPDisciplineTmerPeriod.Period.MUP.ModuleId).Coefficient
                                              : 1
                                              )
                                    ).ToList(),
                    divisions = s.Meta.MUPDisciplineTmerPeriod.Divisions.ToList(),
                    teacherId = s.TeacherId,
                    chairId = s.Meta.MUPDisciplineTmerPeriod.Divisions.FirstOrDefault().uuid,
                    type = ModuleTypeParam.MUP

                }).ToList();
        }
    }

    [IdentityBasicAuthentication]
    public class MinorSubgroupMembershipsController : BaseController
    {
        public IEnumerable<MinorSubgroupWithMemebersApiDto> Get(int year, int term, string moduleName = null)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Database.SetCommandTimeout(300);
                var query = ModuleSubgroupMembershipsController.MinorsOldQuery(db, year, term, moduleName);
                return query.ToList().Select(ModuleSubgroupMembershipsController.Selector);
            }
        }
    }

    [IdentityBasicAuthentication]
    public class MinorSubgroupsController : BaseController
    {
        public IEnumerable<MinorSubgroupApiDto> Get(int year, int term)
        {
            var semester = ModuleSubgroupsController.MapSemester(term);
            using (var db = new ApplicationDbContext())
            {
                db.Database.SetCommandTimeout(300);
                var query = ModuleSubgroupsController.MinorsQuery(db, year, semester);
                return query.ToList().Select(ModuleSubgroupsController.Selector);
            }
        }
    }
}