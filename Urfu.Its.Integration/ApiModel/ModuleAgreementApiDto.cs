using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration.ApiModel
{
    public class ModuleAgreementApiDto
    {
        public string courseTitle { get; set; }
        public string courseType { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string URFUInfoURL { get; set; }
        public string courseURL { get; set; }
        public string universityTitle { get; set; }
        public string universityShortTitle { get; set; }
    }
}
