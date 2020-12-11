using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Integration.Models
{
    public class TmerDto
    {
        public int? kgmer { get; set; }

        public string kmer { get; set; }

        public string rmer { get; set; }
        public int kunr { get; set; }

        public int? ktur { get; set; }
        public int? kediz { get; set; }
        public int npp { get; set; }

        public int lnormzd { get; set; }
        public bool techLoad { get; set; }

        public bool techControl { get; set; }

        public bool techSingle { get; set; }
    }
}
