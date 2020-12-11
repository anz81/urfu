using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Web.Model.Models
{
    public class SubgroupStudentViewModel
    {
        public string Id { get; set; }
        public string StudentGroupId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string PatronymicName { get; set; }
        public string Status { get; set; }
        public bool Included { get; set; }
        public List<string> AnotherGroup { get; set; }
        public List<string> AnotherGroupGroupId { get; set; }
        public string variantName { get; set; }
    }
}
