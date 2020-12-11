using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration.ApiModel
{    
    public class OrganizationPersonDto
    {
        public string name { get; set; }
        public string post { get; set; }
    }

    /// <summary>
    /// ITS-1263
    /// </summary>
    public class OrganizationApiDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string director { get; set; }
        public string inn { get; set; }
        public OrganizationPersonDto person_in_charge { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string location { get; set; }
        public IEnumerable<DirectionApiDto> directions { get; set; }
    }
}
