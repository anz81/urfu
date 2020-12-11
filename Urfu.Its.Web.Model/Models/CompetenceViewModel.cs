using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class CompetenceViewModel
    {
        public int Id { get; set; } 
        public string Code { get; set; }
        public string Type { get; set; }
        public string TypeDesc { get; set; }
        public string Okso { get; set; }
        public string OksoAndTitle { get; set; }
        public string Content { get; set; }
        public string Standard { get; set; }
        public string Division { get; set; }
        public string Profile { get; set; }
        public string ProfileAndTitle { get; set; }
        public string ProfileId { get; set; }
        public string DirectionId { get; set; }
        public string AreaEducation { get; set; }
        public int? AreaEducationId { get; set; }
        public int? CompetenceGroupId { get; set; }
        public string CompetenceGroupName { get; set; }
        public string QualificationName { get; set; }
        public int Order { get; set; }
        public string EduResults { get; set; }
        public CompetenceViewModel(Competence competence)
        {
            Id = competence.Id;
            Code = competence.Code;
            Type = competence.Type;
            TypeDesc = competence.Type + " " + competence.TypeInfo?.Description ?? "";
            Okso = competence.Direction?.okso ?? "";
            OksoAndTitle = competence.Direction?.OksoAndTitle ?? "";
            Content = competence.Content;
            Standard = competence.Standard;
            Division = competence.TypeInfo?.IsStandard == false ? competence.Profile?.Division?.title : null;
            Profile = competence.Profile?.CODE ?? "";
            ProfileAndTitle = competence.Profile?.OksoAndTitle ?? "";
            ProfileId = competence.ProfileId;
            DirectionId = competence.DirectionId;
            AreaEducation = $"{competence.AreaEducation?.Code} {competence.AreaEducation?.Title}";
            AreaEducationId = competence.AreaEducationId;
            CompetenceGroupId = competence.CompetenceGroupId;
            CompetenceGroupName = competence.CompetenceGroup?.Name;
            QualificationName = competence.QualificationName;
            Order = competence.Order;
            EduResults = competence.EduResults == null 
                ? ""
                : string.Join("", competence.EduResults.Select(r => r.EduResultType.ShortName).Distinct());
        }
    }
}
