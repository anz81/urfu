using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Documents.Module;

namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class PlannedResultItemInfo
    {
        public string ProfileId { get; set; }
        public string ProfileCode { get; set; }
        public ICollection<CompetenceInfo> UniversalCompetences { get; set; } = new List<CompetenceInfo>();
        public ICollection<EduResultCompetencesInfo> Results { get; set; } = new List<EduResultCompetencesInfo>();
    }
}