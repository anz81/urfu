using System.Collections.Generic;
using System.Linq;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.MqModel;
using Urfu.Its.Integration.Queues;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Pubs
{
    public class SubgroupPublication
    {

        // Публикация изменения состава миноргруппы
        public static void PublishMinorMember(int subgroupId, string studentId, bool include)
        {
            var members = GetMinorgroupMember(subgroupId, studentId);
            if (null != members)
            {
                PersonalCabinetService.PostSubgroupMember(members, include);
            }
        }

        private static IEnumerable<MinorgroupMemberMqDto> GetMinorgroupMember(int subgroupId, string studentId)
        {
            using (var db = ApplicationDbContext.Create())
            {
                var student = db.Students
                    .Where(s => s.Id == studentId)
                    .Select(s => new
                    {
                        s.Id,
                        s.Person.Surname,
                        s.Person.Name,
                        s.Person.PatronymicName
                    })
                    .SingleOrDefault();

                if (null == student)
                {
                    return null;
                }

                var subgroups = db.MinorSubgroups
                    .Where(s =>
                        // Выбор всей иерархии минорнагрузки
                        db.MinorSubgroups
                            .Where(p => p.Id == subgroupId)
                            .Any(p => new[] { p.Id, p.ParentId, p.Parent.ParentId }.Contains(s.Id))
                    )
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.InnerNumber,
                        s.Meta.Period.Minor.ModuleId,
                        moduleName = s.Meta.Period.Minor.Module.title,
                        disciplineKey = s.Meta.Tmer.Discipline.Discipline.pkey,
                        disciplineId = s.Meta.Tmer.Discipline.Discipline.uid,
                        disciplineName = s.Meta.Tmer.Discipline.Discipline.title,
                        eduyear = s.Meta.Period.Year,
                        term = s.Meta.Period.SemesterId,
                        loadType = s.Meta.Tmer.Tmer.kmer,
                    }).ToList();

                return subgroups.Select(m => new MinorgroupMemberMqDto
                {
                    id = m.Id,
                    name = m.Name,
                    combinedKey = ApiDtoFunctions.ToSubgroupKey(m.InnerNumber, m.ModuleId, m.disciplineKey, m.loadType, m.term, m.eduyear),
                    combinedKey2 = ApiDtoFunctions.ToSubgroupKey(m.InnerNumber, m.ModuleId, m.disciplineKey, m.loadType, m.term, m.eduyear),
                    moduleId = m.ModuleId,
                    moduleName = m.moduleName,

                    disciplineUuid = m.disciplineId,
                    discipline = m.disciplineKey,
                    disciplineName = m.disciplineName,

                    loadType = m.loadType,
                    eduyear = m.eduyear,
                    term = m.term,
                    student = student.Id,
                    studentName = student.Surname + " " + student.Name + " " + student.PatronymicName
                });
            }
        }

        // Публикация изменения состава подгруппы
        public static void PublishMember(int subgroupId, string studentId, bool include)
        {
            var members = GetSubgroupMember(subgroupId, studentId);
            if (null != members)
            {
                PersonalCabinetService.PostSubgroupMember(members, include);
            }
        }

        private static IEnumerable<SubgroupMemberMqDto> GetSubgroupMember(int subgroupId, string studentId)
        {
            using (var db = ApplicationDbContext.Create())
            {
                var student = db.Students
                    .Where(s => s.Id == studentId)
                    .Select(s => new
                    {
                        s.Id,
                        s.Person.Surname,
                        s.Person.Name,
                        s.Person.PatronymicName
                    })
                    .SingleOrDefault();

                if (null == student)
                {
                    return null;
                }

                var subgroups = db.Subgroups
                    .Where(s => !s.Removed &&
                        // Выбор всей иерархии нагрузки
                        db.Subgroups
                        .Where(p => !p.Removed && p.Id == subgroupId)
                        .Any(p => new[] { p.Id, p.ParentId, p.Parent.ParentId }.Contains(s.Id))
                    )
                    .Select(s => new
                    {
                        s.Id,
                        s.InnerNumber,
                        s.Name,
                        s.Meta.groupId,
                        groupName = s.Meta.Group.Name,
                        s.Meta.moduleId,
                        moduleName = s.Meta.Module.title,
                        s.Meta.catalogDisciplineUuid,
                        loadType = s.Meta.kmer,
                        s.Meta.Program.Year,
                        SubgroupYear = s.Meta.Year,
                        s.Meta.Term
                    })
                    .ToList();

                return subgroups.Select(s => new SubgroupMemberMqDto
                {
                    id = s.Id,
                    combinedKey = ApiDtoFunctions.ToSubgroupKey(s.InnerNumber, s.groupId, s.catalogDisciplineUuid, s.loadType, s.Term, s.SubgroupYear),
                    combinedKey2 = ApiDtoFunctions.ToSubgroupKey(s.InnerNumber, s.groupId, s.catalogDisciplineUuid, s.loadType, s.Term, s.SubgroupYear),
                    name = s.Name,
                    groupId = s.groupId,
                    groupName = s.groupName,
                    moduleId = s.moduleId,
                    moduleName = s.moduleName,
                    catalogDisciplineUuid = s.catalogDisciplineUuid,
                    loadType = s.loadType,
                    eduyear = s.Year,
                    term = s.Term,
                    student = student.Id,
                    studentName = student.Surname + " " + student.Name + " " + student.PatronymicName
                });
            }

        }

    }
}