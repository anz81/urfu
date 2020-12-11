

using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Urfu.Its.Integration.Models
{
    public class DirectionXmlDto
    {
        public string ID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string QUALIFICATION { get; set; }
        public string UGN_ID { get; set; }
        public string STANDART_VPO { get; set; }
    }

    [XmlRoot(ElementName = "DIRECTIONS")]
    public class DirectionsXmlDto
    {
        [XmlElement(ElementName = "DIRECTION")]
        public List<DirectionXmlDto> Directions { get; set; }
    }

    public class AreaEducationDto
    {
        public int id { get; set; }
        public string code { get; set; }
        public string title { get; set; }
    }

    public class DirectionDto
    {
        public string uid { get; set; }
        public string okso { get; set; }
        public string title { get; set; }
        public string ministerialCode { get; set; }
        public string ugnTitle { get; set; }
        public string standard { get; set; }
        public string[] qualifications { get; set; }
        public AreaEducationDto areaEducation { get; set; }
        public string diplomaQualification { get; set; }
    }

    public class ProfileXmlDto
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("code")]
        public string CODE { get; set; }
        [JsonProperty("name")]
        public string NAME { get; set; }
        [JsonProperty("chair")]
        public string CHAIR_ID { get; set; }
        [JsonProperty("givedQualificarion")]
        public string QUALIFICATION { get; set; }
        [JsonProperty("direction")]
        public string DIRECTION_ID { get; set; }
        [JsonProperty("foreingContent")]
        public string FOREIGN_CONTENT { get; set; }
    }

    [XmlRoot(ElementName = "PROFILES")]
    public class ProfilesXmlDto
    {
        [XmlElement(ElementName = "PROFILE")]
        public List<ProfileXmlDto> Profiles { get; set; }
    }
}
