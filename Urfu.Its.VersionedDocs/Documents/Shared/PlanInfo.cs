namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class PlanInfo
    {
        public string DisciplineId { get; set; }
        public string ModuleId { get; set; }
        public string EduPlanId { get; set; }
        public string VersionId { get; set; }
        public int VersionNumber { get; set; }
        public int? EduPlanNumber { get; set; }
    }

    public class PlanViewModel : PlanInfo
    {
        public string DisplayName => $"№{EduPlanNumber} ({VersionNumber})";
    }
    public class PlanShortInfo
    {
        public int Number { get; set; }
        public int Version { get; set; }
    }
}