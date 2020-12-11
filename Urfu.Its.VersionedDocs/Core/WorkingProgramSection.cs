using System.Collections.Generic;

namespace Urfu.Its.VersionedDocs.Core
{
    public class WorkingProgramSection
    {
        public WorkingProgramSection(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;           
        }

        public string Name { get; }
        public string DisplayName { get; }
        public IEnumerable<WorkingProgramSection> Sections { get; set; } = new List<WorkingProgramSection>();
    }
}