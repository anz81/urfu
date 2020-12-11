namespace Urfu.Its.Web.Models
{
    public class MinorAutoVM
    {
        public int Year { get; set; }
        public string Semester { get; set; }
        public int SemesterId { get; set; }
        public int StudentCount { get; set; }
        public int AdmittedCount { get; set; }
        public int MinorCount { get; set; }
        public string CompetitionGroup { get; set; }

        public int? CompetitionGroupId { get; set; }
    }
}