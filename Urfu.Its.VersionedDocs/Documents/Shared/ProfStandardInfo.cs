using System;
using System.Collections.Generic;

namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class ProfStandardInfo 
    {        
        public string ProfStandardCode { get; set; }
        public string ProfStandardTitle { get; set; }

        public ProfOrderInfo ProfOrderInfo { get; set; }

        public string Status { get; set; }

        public ICollection<ProfOrderInfo> ProfOrderChanges { get; set; }
    }

    public class ProfOrderInfo
    {
        public string NumberOfMintrud { get; set; }

        public string DateOfMintrud { get; set; }

        public string RegNumberOfMinust { get; set; }

        public string RegNumberDateOfMinust { get; set; }
    }
}