namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class EducationalMethodicalCouncilInfo
    {
        public string ProtocolNumber { get; set; }

        public string ProtocolDate { get; set; }

        public WorkingProgramPersonInfo Chairman { get; set; } = new WorkingProgramPersonInfo();
    }
}