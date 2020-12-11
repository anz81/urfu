using System.Collections.Generic;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers
{
    public class SectionFKCompetitionGroupMembershipsCopyVm
    {
        public int ACount { get; set; }
        public int BCount { get; set; }
        public SectionFKCompetitionGroup A { get; set; }
        public SectionFKCompetitionGroup B { get; set; }
        public List<SectionFK> CommonSections { get; set; }
        public Dictionary<SectionFK, int> AAdmissions { get; set; } = new Dictionary<SectionFK, int>();
        public Dictionary<SectionFK, int> BAdmissions { get; set; } = new Dictionary<SectionFK, int>();
        public List<Group> CommonGroups { get; set; }
        public List<Group> NewGroups { get; set; }
        public List<Group> ExceptGroups { get; set; }
        public List<SectionFK> ExceptSections { get; set; }
        public List<SectionFK> NewSections { get; set; }
    }

    public class ForeignLanguageCompetitionGroupMembershipsCopyVm
    {
        public int ACount { get; set; }
        public int BCount { get; set; }
        public ForeignLanguageCompetitionGroup A { get; set; }
        public ForeignLanguageCompetitionGroup B { get; set; }
        public List<ForeignLanguage> CommonSections { get; set; }
        public Dictionary<ForeignLanguage, int> AAdmissions { get; set; } = new Dictionary<ForeignLanguage, int>();
        public Dictionary<ForeignLanguage, int> BAdmissions { get; set; } = new Dictionary<ForeignLanguage, int>();
        public List<Group> CommonGroups { get; set; }
        public List<Group> NewGroups { get; set; }
        public List<Group> ExceptGroups { get; set; }
        public List<ForeignLanguage> ExceptSections { get; set; }
        public List<ForeignLanguage> NewSections { get; set; }
    }
}