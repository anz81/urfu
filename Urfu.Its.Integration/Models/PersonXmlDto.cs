using System.Collections.Generic;
using System.Xml.Serialization;

namespace Urfu.Its.Integration.Models
{
    public class PersonXmlDto
    {
        [XmlElement(ElementName = "ID")]
        public string Id { get; set; }
        [XmlElement(ElementName = "SURNAME")]
        public string Surname { get; set; }
        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }
        [XmlElement(ElementName = "PATRONYMIC_NAME")]
        public string PatronymicName { get; set; }
        [XmlElement(ElementName = "E_MAIL")]
        public string EMail { get; set; }
        [XmlElement(ElementName = "PHONE")]
        public string Phone { get; set; }
        [XmlElement(ElementName = "DATE_OF_BIRTH")]
        public string DateOfBirth { get; set; }
    }

    [XmlRoot(ElementName = "PERSONS")]
    public class PersonsXmlDto
    {
        [XmlElement(ElementName = "PERSON")]
        public List<PersonXmlDto> Students { get; set; }
    }
}