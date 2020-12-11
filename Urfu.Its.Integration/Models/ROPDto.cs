using System;
using System.Collections.Generic;

namespace Urfu.Its.Integration.Models
{
    public class ROPDto
    {
        public string runpId { get; set; }
        public string userPrincipalName { get; set; }
        public string samAccountName { get; set; }
        public List<string> divisions { get; set; }
        public List<string> directions { get; set; }
    }
}