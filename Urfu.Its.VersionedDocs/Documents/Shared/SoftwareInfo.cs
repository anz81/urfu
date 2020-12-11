using System.Collections.Generic;

namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    /// <summary>
    /// пункт 9.3 в соответствии с http://projects.it.ustu/browse/ITS-723
    /// </summary>
    public class SoftwareInfo
    {
        public bool NotUsed { get; set; }

        public ICollection<SoftwareItemInfo> SystemOrOffice { get; set; } = new List<SoftwareItemInfo>();

        public string Hardware { get; set; }
        public string Free { get; set; }
        public string Trial { get; set; }

        public ICollection<SoftwareItemInfo> Application { get; set; } = new List<SoftwareItemInfo>();

        public string AdditionalApplication { get; set; }
    }
    public class SoftwareItemInfo
    {
        public string Name { get; set; }
        public string Class { get; set; }
    }
}