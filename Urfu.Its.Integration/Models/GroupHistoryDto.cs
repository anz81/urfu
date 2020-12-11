using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Integration.Models
{
    public class GroupHistoryDto
    {
        public string parent { get; set; }
        public string qualification { get; set; }
        public string code { get; set; }
        public int year { get; set; }
        public string familirizationForm { get; set; }
        public int course { get; set; }
        public bool its { get; set; }
        public string specialization { get; set; }
        public string formativeDivision { get; set; }
        public string title { get; set; }
        public string group { get; set; }
        public string familirizationTech { get; set; }
    }
}
