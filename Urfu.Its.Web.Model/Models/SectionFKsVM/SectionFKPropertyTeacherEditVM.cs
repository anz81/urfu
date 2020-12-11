using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Web.Model.Models.ModulesVM
{
    public class ModulePropertyRow <T>
    {
        public T id { get; set; }
        public bool selected { get; set; }
    }
}
