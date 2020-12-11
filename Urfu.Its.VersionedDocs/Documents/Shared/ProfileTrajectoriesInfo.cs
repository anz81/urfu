using System.Collections.Generic;

namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class ProfileTrajectoriesInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DirectionId { get; set; }

        public ICollection<string> Trajectories { get; set; } = new List<string>();
    }
}