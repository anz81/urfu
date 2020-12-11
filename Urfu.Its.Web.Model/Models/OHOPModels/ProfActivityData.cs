using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models.OHOPModels
{
    public class ProfActivityAreaData
    {
        public string AreaCode { get; set; }
        public string AreaTitle { get; set; }
        public string AreaFullTitle { get { return $"{AreaCode} - {AreaTitle}"; } }

        public IEnumerable<ProfActivityKindData> Kinds { get; set; }
    }

    public class ProfActivityKindData
    {
        public string KindCode { get; set; }
        public string KindTitle { get; set; }
        public string KindFullTitle { get { return $"{KindCode} - {KindTitle}"; } }

        public string StandardCode { get; set; }
        public string StandardTitle { get; set; }

        public bool IsOldStandard { get; set; }
    }
}
