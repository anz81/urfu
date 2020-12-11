using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Model.Models.OHOPModels
{
    public class ApprovalActTemplateModel : ApprovalAct
    {
        public string Chair { get; set; }
        public string Institute { get; set; }
        public string EducationLevelGenitive { get; set; }
        public string ProfileCode { get; set; }
        public string ProfileName { get; set; }

        public IEnumerable<string> UniqueAreaStrings
        {
            get
            {
                return VariantInfos?.SelectMany(v => v.ProfActivityRows).OrderBy(r => r.AreaCode).Select(r => r.NoProfStandard ? $"{r.AreaTitle}" : $"{r.AreaCode} - {r.AreaTitle}").Distinct();
            }
        }

        public IEnumerable<string> UniqueProfObjectsStrings
        {
            get
            {
                return VariantInfos?.SelectMany(v => v.ProfActivityRows).Select(r => r.ProfObjects).Where(r => r != null).Distinct();
            }
        }
    }
}
