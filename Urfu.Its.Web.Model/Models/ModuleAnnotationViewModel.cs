using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class ModuleAnnotationViewModel
    {
        public string directionId { get; set; }
        public string directionOkso { get; set; }
        public string directionTitle { get; set; }
        public string qualification { get; set; }
        public string profile { get; set; }
        public string profileTitle { get; set; }
        public string profileId { get; set; }
        public int year { get; set; }
        public string status { get; set; }
        public int statusId { get; set; }
        public DateTime statusDateTime { get; set; }
        public string statusChangeTime { get { return $"{statusDateTime.ToShortDateString()} {statusDateTime.ToShortTimeString()}"; } }

        public int planNumber { get; set; }
        public int planVersionNumber { get; set; }
        public string standard { get; set; }
        public int versionedDocumentId { get; set; }
        public string chairTitle { get; set; }
        public string chairId { get; set; }
        public string divisionTitle { get; set; }
        public string divisionId { get; set; }
        public string comment { get; set; }

        public int basicCharacteristicOPId { get; set; }
        public int basicCharacteristicOPVersion { get; set; }

        public ModuleAnnotationViewModel()
        {

        }

        public ModuleAnnotationViewModel(ModuleAnnotation annotation)
        {
            directionId = annotation.BasicCharacteristicOP.Info.Profile.DIRECTION_ID;
            directionOkso = annotation.BasicCharacteristicOP.Info.Profile.Direction.okso;
            directionTitle = annotation.BasicCharacteristicOP.Info.Profile.Direction.title;
            qualification = annotation.BasicCharacteristicOP.Info.Profile.QUALIFICATION;
            profile = annotation.BasicCharacteristicOP.Info.Profile.CODE;
            profileTitle = annotation.BasicCharacteristicOP.Info.Profile.NAME;
            profileId = annotation.BasicCharacteristicOP.Info.ProfileId;
            year = annotation.BasicCharacteristicOP.Info.Year;
            status = annotation.Status == null ? "" : annotation.Status.Name;
            statusId = annotation.Status.Id;
            statusDateTime = annotation.StatusChangeTime;
            standard = annotation.BasicCharacteristicOP.Info.Profile.Direction.standard;
            versionedDocumentId = annotation.VersionedDocumentId;
            chairTitle = annotation.BasicCharacteristicOP.Info.Profile.Division.title;
            chairId = annotation.BasicCharacteristicOP.Info.Profile.Division.uuid;
            divisionId = annotation.BasicCharacteristicOP.Info.Profile.Division.parent;
            comment = annotation.Comment;
            planNumber = annotation.PlanNumber;
            planVersionNumber = annotation.PlanVersionNumber;
            basicCharacteristicOPId = annotation.BasicCharacteristicOPId;
            basicCharacteristicOPVersion = annotation.BasicCharacteristicOP.Version;
        }
    }
}
