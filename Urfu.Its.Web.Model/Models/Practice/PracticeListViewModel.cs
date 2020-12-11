using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Web.Model.Models.Practice
{

    public class ThemeViewModel
    {
        public int Id { get; set; }
        public string Theme { get; set; }
        public string DisciplineUUID { get; set; }
        public int Year { get; set; }
        public int SemesterId { get; set; }
        public string GroupHistoryId { get; set; }
    }

    public class PracticeListViewModel
    {
        public string DisciplineTitle { get; set; }
        public string Name { get { return DisciplineTitle + (IsRemovedDiscipline ? " (Удаленная дисциплина)" : ""); } }
        public string DisciplineUUID { get; set; }
        public string Department { get; set; }
        public string Institute { get; set; }
        public string InstituteId { get; set; }
        public string Okso { get; set; }
        public string Direction { get; set; }
        public string DirectionId { get; set; }
        public string Group { get; set; }
        public string Teachers { get; set; }
        public string Themes { get; set; }
        public string PracticeType { get; set; }
        public string EduplanUUID { get; set; }
        public string GroupYear { get; set; }

        /// <summary>
        /// Id исторической группы (GroupHistoryId)
        /// </summary>
        public string GroupId { get; set; }
        public string SemesterName { get; set; }
        public int SemesterId { get; set; }
        public int Year { get; set; }
        public string PracticeDates { get; set; }
        public int? TimeId { get; set; }
        public string TimeDescription { get; set; }
        public int? WayId { get; set; }
        public string WayDescription { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string Subdivision { get; set; }

        /// <summary>
        /// Поле для определения фокуса.
        /// DisciplineUid + GroupHistoryId + SemesterID
        /// </summary>
        public string FocusField
        {
            get
            {
                return DisciplineUUID + GroupId + SemesterId;
            }
        }

        public bool IsOldPlanVersion { get; set; }
        public bool IsRemovedDiscipline { get; set; }
        public int PlanNumber { get; set; }
        public int PlanVersion { get; set; }
        public string PlanStatus { get; set; }
        public string PlanNumberAndVersion
        {
            get
            {
                string planNumberAndVersion = $"{PlanNumber} ({PlanVersion})";
                return IsOldPlanVersion ? $"Старая версия ({planNumberAndVersion})" : $"{PlanStatus} {planNumberAndVersion}";
            }
        }
        public string ReportBeginDate { get; set; }
        public string ReportEndDate {get;set;}
        public string ReportDates { get; set; }

        public string Standard { get; set; }

    }
}
