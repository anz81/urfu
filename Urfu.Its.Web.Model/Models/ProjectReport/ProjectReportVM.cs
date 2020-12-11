using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Web.Model.Models.ProjectReport
{
    public class ProjectReportVM
    {
        public string Title { get; set; }
        public string Level { get; set; }
        public int Year { get; set; }
        public int SemesterId { get; set; }
        public string Semester { get; set; }
        public int Course { get; set; }
        public string Company { get; set; }
        public string CompetitionGroup { get; set; }
        public List<string> Rops { get; set; }
        public List<string> Curators { get; set; }
        public int Limit { get; set; }
        public int Selection { get; set; }
        public int Admission { get; set; }
        public int Vacancy { get; set; }
    }
}
