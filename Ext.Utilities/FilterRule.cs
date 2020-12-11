using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Utilities
{
    public class FilterRule
    {
        private string _value;
        public string Property { get; set; }

        public string Value
        {
            get { return _value; }
            set { _value = value?.Trim(); }
        }

        public string Verb { get; set; }
    }

    public class ObjectableFilterRule
    {
        private object _value;
        public string Property { get; set; }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Verb { get; set; }
    }
}
