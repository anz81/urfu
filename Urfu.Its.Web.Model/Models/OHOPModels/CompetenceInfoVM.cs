using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Web.Model.Models
{
    public class CompetenceInfoVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public int? CompetenceGroupId { get; set; }
        public string CompetenceGroupName { get; set; }

        public int Order
        {
            get
            {
                int order = 0;
                int.TryParse(Code.Split('-').Last(), out order);

                return order;
            }
        }
    }
}
