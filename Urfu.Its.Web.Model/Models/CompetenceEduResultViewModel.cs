using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class CompetenceEduResultViewModel
    {
        public int Id { get; set; }
        public int SerialNumber { get; set; }
        public int EduResultTypeId { get; set; }
        public string EduResultTypeName { get; set; }
        public int EduResultKindId { get; set; }
        public string EduResultKindName { get; set; }
        public string Code { get; set; }
        public int CompetenceId { get; set; }
        public string Description { get; set; }

        public CompetenceEduResultViewModel(EduResult2 eduResult)
        {
            Id = eduResult.Id;
            SerialNumber = eduResult.SerialNumber;
            EduResultTypeId = eduResult.EduResultTypeId;
            EduResultTypeName = eduResult.EduResultType.Name;
            EduResultKindId = eduResult.EduResultKindId;
            EduResultKindName = eduResult.EduResultKind.Name;
            Code = eduResult.Code;
            CompetenceId = eduResult.CompetenceId;
            Description = eduResult.Description;
        }
    }
}
