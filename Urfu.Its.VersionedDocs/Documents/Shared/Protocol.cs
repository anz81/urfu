using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class Protocol
    {
        public string ProtocolNumber { get; set; }

        public string ProtocolDate { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string MonthName { get; set; }
        public string Year { get; set; }


    }
}
