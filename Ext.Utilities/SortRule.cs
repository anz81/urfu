using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Ext.Utilities
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    // [{"property":"title","direction":"ASC"}]
    public class SortRule
    {
        public string Property { get; set; }

        [JsonConverter(typeof(SortDirectionTypeEnumConverter))]
        public SortDirection Direction { get; set; }
    }
}
