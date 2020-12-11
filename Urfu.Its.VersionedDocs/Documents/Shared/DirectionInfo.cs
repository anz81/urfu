namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class DirectionInfo
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Qualifications { get; set; }
        public string Standard { get; set; }
    }

    public class DirectionInfo2 : DirectionInfo
    {
        public string AreaEducationCode { get; set; }
        public string AreaEducationTitle { get; set; }
    }

    public class DirectionViewModel: DirectionInfo
    {
        public string DisplayName => Code + " - " + Title;
    }
}