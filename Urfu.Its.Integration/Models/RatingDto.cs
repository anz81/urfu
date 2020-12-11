using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Urfu.Its.Integration.Models
{
    [XmlRoot(ElementName = "response")]
    public class RatingDto
    {
        [XmlElement("data")]
        public RatingDataDto data { get; set; }
    }

    public class RatingDataDto
    {
        [XmlElement(ElementName = "record")]
        public List<StudentRatingDto> list { get; set; }
    }
    
    public class StudentRatingDto
    {
        public string id { get; set; }
        public decimal rate { get; set; }
    }
    public class StudentRatingUniAvgDto
    {
        public string student { get; set; }
        public decimal avgScore { get; set; }
    }

    public class ForeignLanguageRatingDtp
    {
        public int attempt_id { get; set; }
        public string quiz_id { get; set; }
        public string quiz_name  { get; set; }
        public string user_id { get; set; }
        public string sam { get; set; }
        public string uni { get; set; }
        public string dcode { get; set; }
        public decimal? grade { get; set; }
        public int attempt_number { get; set; }
        public string date { get; set; }
        public string grade_grammar { get; set; }
        public string grade_reading { get; set; }
        public string grade_audition { get; set; }
        public string grade_writing { get; set; } 
        public string grade_level { get; set; }
        public string test_specification { get; set; }
    }
}
