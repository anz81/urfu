using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Web.Model.Models.OHOPModels
{
    public class ProfOrderViewModel
    {
        
        public int? ProfOrderId { get; set; }
        public string ProfStandardCode {get;set;}
        public string NumberOfMintrud { get; set; }
        public DateTime? DateOfMintrud { get; set; }
              
        public string RegNumberOfMinust { get; set; }
        public DateTime? RegNumberDateOfMinust { get; set; }

        public string Status { get; set; }      

    }

    public class ProfOrderChangeViewModel : ProfOrderViewModel
    {       
        public int? ProfOrderChangeID { get; set; }
    }
}
