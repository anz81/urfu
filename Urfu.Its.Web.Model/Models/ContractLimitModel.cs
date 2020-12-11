using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Web.Model.Models
{
    public class ContractLimitModel
    {
        public string QualificationName { get; set; }

        public int Course { get; set; }

        public int Limit { get; set; }

        public string DirectionId { get; set; }

        public string ProfileId { get; set; }

        public int SemesterId { get; set; }
        public int Year { get; set; }
        public int ContractId { get; set; }

        public string Level { get; set; }
    }
}
