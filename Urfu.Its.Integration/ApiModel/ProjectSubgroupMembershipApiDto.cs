using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Integration.ApiModel
{
    public class StudentInfoApiDto
    {
        public string studentId { get; set; }
        public string runpUid { get; set; }
        public string groupId { get; set; }
        public string groupName { get; set; }

        /// <summary>
        /// поле EmployersId из таблицы ProjectRoles
        /// </summary>
        public int? roleId { get; set; }
    }

    public class ProjectCompetenceListApiDto
    {
        public string programUuid { get; set; }
        public IEnumerable<ProjectCompetenceApiDto> competences { get; set; }
    }
    public class ProjectCompetenceApiDto
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class ProjectSubgroupMembershipApiDto
    {
        public string moduleId { get; set; }
        public string moduleName { get; set; }
        public string moduleType { get; set; }
        public IEnumerable<ProjectCompetenceListApiDto> competences { get; set; }
        public int emoloyersId { get; set; }
        public string disciplineId { get; set; }
        public string disciplineName { get; set; }
        public string loadTypeId { get; set; }
        public string loadTypeName { get; set; }
        public int eduyear { get; set; }
        public int term { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int studentCount => students.Count();
        public IEnumerable<StudentInfoApiDto> students { get; set; }
        public string teacherId { get; set; }
        public int? studentCourse { get; set; }
        public string competitionGroupName { get; set; }
    }
}
