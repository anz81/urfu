using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Integration.Models
{
    public class WorkingProgramDocumentDto
    {
        public int id { get; set; }
        public string  module_uuid { get; set; }
        public string type { get; set; }
        public int version { get; set; }
        public List<WPPlanVersionDto> edu_plans { get; set; }
        public List<string> profiles { get; set; }
        public string file_name { get; set; }
        public int file_size { get; set; }
        public byte[] file { get; set; }
        public string started_by { get; set; }
        public string status { get; set; }
        public List<DisciplineWorkingProgramDto> disciplines { get; set; }

        public class DisciplineWorkingProgramDto
        {
            public int id { get; set; }
            public string discipline_uuid { get; set; }
            public string module_uuid { get; set; }
            public string type { get; set; }
            public int version { get; set; }
            public string file_name { get; set; }
            public int file_size { get; set; }
            public byte[] file { get; set; }

        }

        public class WPPlanVersionDto
        {
            public string eduplan_uuid { get; set; }
            public string version_uuid { get; set; }
        }

    }
    
    public class StatusWorkingProgramDocument
    {
        public string Title { get; set; }
        public string UniId { get; set; }
        public string Status { get; set; }
        public string Number { get; set; }
    }
}
