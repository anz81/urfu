using System.Collections.Generic;

namespace Urfu.Its.Integration.Models
{
    /*public class ApploadDto
    {
        public string dckey { get; set; }
        public string kzajvN { get; set; }
        public string dd { get; set; }
        public string edd { get; set; }
        public string modid { get; set; }
        public string modtitle { get; set; }
        public string modnum { get; set; }
        public string elearn { get; set; }
        public string openedu { get; set; }
        public string forgn { get; set; }
        public string itc { get; set; }
        public string kpdrfrod { get; set; }
        public string kgod { get; set; }
        public string god { get; set; }
        public string semes { get; set; }
        public string vsem { get; set; }
        public string kpdr { get; set; }
        public string kzatr { get; set; }
        public string rzatr { get; set; }
        public string kobj { get; set; }
        public string kobjn { get; set; }
        public string kobjs { get; set; }
        public string kzajv { get; set; }
        public string indexd { get; set; }
        public string kdis { get; set; }
        public string rdis { get; set; }
        public string kprblk { get; set; }
        public string prblk { get; set; }
        public string vpoUP { get; set; }
        public string obaudn { get; set; }
        public string ranee { get; set; }
        public string tsem { get; set; }
        public string numgrp { get; set; }
        public string labgrps { get; set; }
        public string prflws { get; set; }
        public string kpot { get; set; }
        public string pot { get; set; }
        public string expgrp { get; set; }
        public string kgrp { get; set; }
        public string rgrp { get; set; }
        public string kpdrf { get; set; }
        public string kpdrg { get; set; }
        public string course { get; set; }
        public string grpstud { get; set; }
        public string kolpgr { get; set; }
        public string brs { get; set; }
        public string kspec { get; set; }
        public string kokso { get; set; }
        public string rspec { get; set; }
        public string strd { get; set; }
        public string kspez { get; set; }
        public string rspez { get; set; }
        public string spezcode { get; set; }
        public string kfos { get; set; }
        public string rfos { get; set; }
        public string ksvpo { get; set; }
        public string rsvpo { get; set; }
        public string ktos { get; set; }
        public string rtos { get; set; }
        public string diskol { get; set; }
        public string kmer { get; set; }
        public string rmer { get; set; }
        public string rediz { get; set; }
        public string tmer { get; set; }
        public string kolstd { get; set; }
        public string ltt { get; set; }
        public string ps1 { get; set; }
        public string ps2 { get; set; }
        public string kfmer { get; set; }
        public string fmer { get; set; }
    }*/

    public class ApploadsDto
    {
        public List<ApploadDto> data { get; set; }
    }

    public class ApploadDto
    {
      public int labSubgroups { get; set; }
      public string actionTitle { get; set; }
      public string eduDiscipline { get; set; }
      public string disciplineTitle { get; set; }
      public string eduModule { get; set; }
      public string lectureFlows { get; set; }
      public string chair { get; set; }
      public string group { get; set; }
      public string action { get; set; }
      public decimal value { get; set; }
      public string dckey { get; set; }
      public int? practiceFlows { get; set; }
      public string uuid { get; set; }
      public string discipline { get; set; }
      public string detailDiscipline { get; set; }

        public bool removed { get; set; }

        public int status { get; set; }

        public string modtypeUNI { get; set; }
        public string TypeProject { get; set; }
    }
}