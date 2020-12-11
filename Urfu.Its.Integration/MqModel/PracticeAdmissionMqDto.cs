using System;
using System.Collections.Generic;

namespace Urfu.Its.Integration.ApiModel
{
    public class PracticeAdmissionMqDto
    {
        public string StudentId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string PatronymicName { get; set; }
        public int LimitId { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public int Course { get; set; }
        public List<PracticePeriodModelMqDto> PracticeDates { get; set; }
        public string PracticeType { get; set; }
        public string PracticeTitle { get; set; }
        public int PracticeId { get; set; }
        public int? Units { get; set; }
        public string Theme { get; set; }
        public int Status { get; set; }
        public bool Agreement { get; set; }
    }

    public class PracticePeriodModelMqDto
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PracticePeriodModelMqDto(DateTime? beginDate, DateTime? endDate)
        {
            BeginDate = beginDate;
            EndDate = endDate;
        }
    }



}