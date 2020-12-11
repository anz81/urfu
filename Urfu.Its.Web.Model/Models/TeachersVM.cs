using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class TeachersVM
    {
        public string pkey { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string workPlace { get; set; }
        public string post { get; set; }
        public string initials { get; set; }
        public string email { get; set; }

        public string BigName => $"{lastName} {firstName} {middleName} ({post}, {workPlace}), {email}";
        
        public string FullName => $"{lastName} {firstName} {middleName}";
    }
}
