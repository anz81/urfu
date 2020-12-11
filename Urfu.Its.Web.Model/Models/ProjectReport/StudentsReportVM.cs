using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models.ProjectReport
{
    public class StudentsReportVM
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get { return $"{LastName} {FirstName} {MiddleName}"; } }
        public string Group { get; set; }
        public int Year { get; set; }
        public int SemesterId { get; set; }
        public string Semester { get; set; }
        public string Status { get; set; }
        public string Compensation { get; set; }
        public string CompetitionGroupShortName { get; set; }
        public string Project { get; set; }
        public string Level { get; set; }
        public int? Priority { get; set; }
        public AdmissionStatus AdmissionStatus { get; set; }
        public string AdmissionStatusName { get { return EnumHelper<AdmissionStatus>.GetDisplayValue(AdmissionStatus); } }
        public string Role { get; set; }
        public List<string> Subgroups { get; set; }
    }
}
