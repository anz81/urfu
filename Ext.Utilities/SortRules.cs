using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Utilities
{
    public class SortRules : List<SortRule>
    {
        public static SortRules Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json)) return new SortRules();
            return JsonConvert.DeserializeObject<SortRules>(json);
        }
    }
}
