using System;
using System.Collections.Generic;

namespace Urfu.Its.Integration.Models
{
    public class ModuleAgreementDto
    {
        public string ModuleUUID { get; set; }
        public string DisciplineUUID { get; set; }
        public string ID { get; set; }
        public string CourseTitle { get; set; }
        public string CourseType { get; set; }
        public int EduYear { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int[] Terms { get; set; }
        public string URFUInfoURL { get; set; }
        public string CourseURL { get; set; }
        public string UniversityTitle { get; set; }
        public string UniversityShortTitle { get; set; }
    }
}