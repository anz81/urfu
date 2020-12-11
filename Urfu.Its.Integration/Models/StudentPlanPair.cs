namespace Urfu.Its.Integration.Models
{
    public class StudentPlanPair
    {
        public string StudentId { get; set; }
        public string planId { get; set; }
        public int planNumber { get; set; }
        public string versionId { get; set; }
        public int versionNumber { get; set; }
    }
    public class StudentPlansPair
    {
        public string student { get; set; }
        public PlanVersionDto[] vs { get; set; }

    }
}