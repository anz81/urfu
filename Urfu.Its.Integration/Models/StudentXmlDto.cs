using System.Collections.Generic;
using System.Xml.Serialization;

namespace Urfu.Its.Integration.Models
{
    public class StudentXmlDto
    {
        [XmlElement(ElementName = "ID")]
        public string Id { get; set; }
        [XmlElement(ElementName = "PERSON_ID")]
        public string PersonId { get; set; }
        [XmlElement(ElementName = "PERSONAL_NUMBER")]
        public string PersonalNumber { get; set; }
        [XmlElement(ElementName = "STATUS")]
        public string Status { get; set; }
        [XmlElement(ElementName = "GROUP_ID")]
        public string GroupId { get; set; }
        [XmlElement(ElementName = "PHONE_HOME")]
        public string PhoneHome { get; set; }
        [XmlElement(ElementName = "IS_TARGET")]
        public bool IsTarget { get; set; }
        [XmlElement(ElementName = "IS_INTERNATIONAL")]
        public bool IsInternational { get; set; }
        [XmlElement(ElementName = "COMPENSATION")]
        public string Compensation { get; set; }
        [XmlElement(ElementName = "PHONE_MOBILE")]
        public string PhoneMobile { get; set; }
        [XmlElement(ElementName = "PHONE_WORK")]
        public string PhoneWork { get; set; }
        [XmlElement(ElementName = "EMAIL")]
        public string Email { get; set; }
        [XmlElement(ElementName = "ICQ")]
        public string Icq { get; set; }
        [XmlElement(ElementName = "SEX")]
        public int Sex { get; set; }
        [XmlElement(ElementName = "CITIZENSHIP")]
        public string Citizenship { get; set; }
    }

    [XmlRoot(ElementName = "STUDENTS")]
    public class StudentsXmlDto
    {
        [XmlElement(ElementName = "STUDENT")]
        public List<StudentXmlDto> Students { get; set; }
    }
}