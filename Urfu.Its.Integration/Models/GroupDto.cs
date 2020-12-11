using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Urfu.Its.Integration.Models
{
    public class GroupXmlDto
    {
        [XmlElement(ElementName = "ID")]
        public string Id { get; set; }
        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }
        [XmlElement(ElementName = "PROFILE_ID")]
        public string ProfileId { get; set; }
        [XmlElement(ElementName = "YEAR")]
        public string Year { get; set; }
        [XmlElement(ElementName = "COURSE")]
        public int Course { get; set; }
        [XmlElement(ElementName = "CHAIR_ID")]
        public string ChairId { get; set; }
        [XmlElement(ElementName = "FORMATIVE_DIVISION_ID")]
        public string FormativeDivisionId { get; set; }
        [XmlElement(ElementName = "FORMATIVE_DIVISION_PARENT_ID")]
        public string FormativeDivisionParentId { get; set; }
        [XmlElement(ElementName = "MANAGING_DIVISION_ID")]
        public string ManagingDivisionId { get; set; }
        [XmlElement(ElementName = "MANAGING_DIVISION_PARENT_ID")]
        public string ManagingDivisionParentId { get; set; }

        [XmlElement(ElementName = "FAM_TYPE")]
        public string FamType { get; set; }
        [XmlElement(ElementName = "FAM_TECH")]
        public string FamTech { get; set; }
        [XmlElement(ElementName = "FAM_COND")]
        public string FamCond { get; set; }
        [XmlElement(ElementName = "FAM_PERIOD")]
        public string FamPeriod { get; set; }
        [XmlElement(ElementName = "QUAL")]
        public string Qual { get; set; }
    }

    [XmlRoot(ElementName = "GROUPS")]
    public class GroupsXmlDto
    {
        [XmlElement(ElementName = "GROUP")]
        public List<GroupXmlDto> Groups { get; set; }
    }
}
