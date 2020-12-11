using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Utilities
{
    public class FilterRules : List<FilterRule>
    {
        public static FilterRules Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject<FilterRules>(json);
        }
    }

    public class ObjectableFilterRules : List<ObjectableFilterRule>
    {
        public static ObjectableFilterRules Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject<ObjectableFilterRules>(json);
        }
    }
}
