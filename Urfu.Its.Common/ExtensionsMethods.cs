using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Common
{
    public static class ExtensionsMethods
    {
        public static T GetPropertyValue<T>(this object obj,String propertyName)
        {
            return (T)obj?.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }
    }
}
