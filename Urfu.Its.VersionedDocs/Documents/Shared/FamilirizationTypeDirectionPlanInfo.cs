namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class FamilirizationTypeDirectionPlanInfo
    {
        public string ItemId { get; set; }
        public string FamType { get; set; }
        public string DirectionId { get; set; }
        public string DirectionCode { get; set; }

        /// <summary>
        /// [disciplineUUID]
        /// </summary>
        public string DisciplineId { get; set; }

        public string PlanVersionId { get; set; }
        public int? PlanNumber { get; set; }
        public string PlanVersionTitle { get; set; }
    }
}