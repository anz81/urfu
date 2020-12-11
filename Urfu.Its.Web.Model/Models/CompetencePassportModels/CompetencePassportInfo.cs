using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models.CompetencePassportModels
{
    public class EduResultsInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public int KindId { get; set; }
        public string Kind { get; set; }
        public string Description { get; set; }

        public int SerialNumber { get; set; }

        public EduResultsInfo()
        {

        }

        public EduResultsInfo(EduResult2 eduResult)
        {
            Id = eduResult.Id;
            Code = eduResult.Code;
            Description = eduResult.Description;
            Kind = eduResult.EduResultKind.Name;
            KindId = eduResult.EduResultKindId;
            Type = eduResult.EduResultType.Name;
            TypeId = eduResult.EduResultTypeId;
            SerialNumber = eduResult.SerialNumber;
        }
    }
}
