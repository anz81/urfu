using System;
using System.Collections.Generic;


namespace Urfu.Its.Integration.Models
{
    public class StudentSelectionMinorDto
    {
        public string student { get; set; }
        public List<StudentSelectionMinorPeriodDto> request { get; set; }
    }

    public class StudentSelectionMinorPeriodDto
    {
        public int year { get; set; }
        public int semester { get; set; }

        public List<StudentSelectionMinorPriorityDto> minors { get;set;}
    }

    public class StudentSelectionMinorPriorityDto
    {
        /// <summary>
        /// MinorID
        /// </summary>
        public string id { get; set; }
        public int prio { get; set; } 

        /// <summary>
        /// Названние майнора
        /// </summary>
        public string title { get; set; }
        public string requirment { get; set; }
        public DateTime? deadline { get; set; }

        /// <summary>
        /// Статус зачисления
        /// </summary>
        public string admission { get; set; }
        public int admissionid { get; set; }
    }
}
