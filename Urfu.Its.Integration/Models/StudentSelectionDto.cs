using System.Collections.Generic;

namespace Urfu.Its.Integration.Models
{
    public class StudentSelectionDto
    {
        public string studentPersonId { get; set; }
        public List<VariantSelectionDto> variants { get; set; }
    }

    public class VariantSelectionDto
    {
        public int selectedVariantId { get; set; }
        public int selectedVariantPriority { get; set; }
        public List<StudentSelectionPriorityDto> priorities { get; set; }
        public List<StudentSelectionTeacherDto> teachers { get; set; }

    }

    public class StudentSelectionTeacherDto
    {
        public string pkey { get; set; }
        public string control { get; set; }
        public string disciplineUUID { get; set; }
    }

    public class StudentSelectionPriorityDto
    {
        public int variantContentId { get; set; }
        public int proprity { get; set; }
    }
}