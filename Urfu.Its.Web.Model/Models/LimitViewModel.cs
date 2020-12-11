using System.Collections.Generic;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class LimitViewModel
    {
        public string ModuleNumberAndTitle { get; set; }
        public string ModuleId { get; set; }
        public int? StudentsCount { get; set; }
        public string ModuleTitle { get; set; }
        public VariantGroupType GroupType { get; set; }
        public int StudentsCountAll { get; set; }
        public int? ProgramStudentsCount { get; set; }
        public IList<EduProgramLimit> VariantLimits { get; set; }
        public string Comment { get; set; }
    }
}