using System;
using System.Collections.Generic;
using System.Linq;

namespace Urfu.Its.Integration.Models
{
    public class StudentSelectionsForeignLanuguageDto
    {
        public string student { get; set; }
        public List<StudentSelectionForeignLanguagePeriodDto> request { get; set; }
    }

    public class StudentSelectionForeignLanuguageDto
    {
        public string student { get; set; }
        public int? year { get; set; }
        public int? semester { get; set; }
        public string moduleId { get; set; }
        public string targetLevel { get; set; }
        public string admission { get; set; }
        public int? admissionid { get; set; }
        public List<ForeignLanguageSubgroupDto> subgroups { get; set; }
    }

    public class ForeignLanguageSubgroupDto
    {
        public int id { get; set; }
        public string title { get; set; }
        public string teacher { get; set; }
        public string level { get; set; }
        public string kmer { get; set; }
    }
    public class StudentSelectionForeignLanguagePeriodDto
    {
        public int year { get; set; }
        public int semester { get; set; }

        public List<StudentSelectionForeignLanuguagePriorityDto> modules { get; set; }
    }

    public class StudentSelectionForeignLanuguagePriorityDto
    {
        /// <summary>
        /// MinorID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// Названние майнора
        /// </summary>
        public string title { get; set; }
        public string requirment { get; set; }
        public DateTime? deadline { get; set; }
        public string targetLevel { get; set; }
        /// <summary>
        /// Статус зачисления
        /// </summary>
        public string admission { get; set; }
        public int admissionid { get; set; }
    }
}