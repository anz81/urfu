using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Urfu.Its.Common
{
    public static class DoubleExtensions
    {
        public static string WithoutComma(this double value)
        {
            return value.ToString().Replace(",", ".");
        }
    }
}
