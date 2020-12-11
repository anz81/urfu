using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class VersionOHOPViewModel
    {
        public int version { get; set; }
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
        public string standard { get; set; }
        public int versionedDocumentId { get; set; }
        public string chairTitle { get; set; }
        public string chairId { get; set; }
        public string divisionTitle { get; set; }
        public string divisionId { get; set; }
        public string comment { get; set; }

        public int rowsCountTable { get; set; }
        public int filledRowsCountTable1 { get; set; }
        public string percentTable1 { get { return $"{filledRowsCountTable1} из {rowsCountTable}"; } }


        public int filledRowsCountTable4 { get; set; }
        public string percentTable4 { get { return $"{filledRowsCountTable4} из {rowsCountTable}"; } }

        public VersionOHOPViewModel()
        {

        }

        public VersionOHOPViewModel(BasicCharacteristicOP ohop)
        {
            version = ohop.Version;
            directionId = ohop.Info.Profile.DIRECTION_ID;
            directionOkso = ohop.Info.Profile.Direction.okso;
            directionTitle = ohop.Info.Profile.Direction.title;
            qualification = ohop.Info.Profile.QUALIFICATION;
            profile = ohop.Info.Profile.CODE;
            profileTitle = ohop.Info.Profile.NAME;
            profileId = ohop.Info.ProfileId;
            year = ohop.Info.Year;
            status = ohop.Status == null ? "" : ohop.Status.Name;
            statusId = ohop.Status.Id;
            statusDateTime = ohop.StatusChangeTime;
            standard = ohop.Info.Profile.Direction.standard;
            versionedDocumentId = ohop.VersionedDocumentId;
            chairTitle = ohop.Info.Profile.Division.title;
            chairId = ohop.Info.Profile.Division.uuid;
            divisionId = ohop.Info.Profile.Division.parent;
            comment = ohop.Comment;
        }
    }
}
