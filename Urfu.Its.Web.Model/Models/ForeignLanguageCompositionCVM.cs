using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class ForeignLanguageCompositionCVM
    {
        public ForeignLanguageCompetitionGroup SrcCompetitionGroup;
        public ForeignLanguageCompetitionGroup DstCompetitionGroup;
        public bool Subgroups;
        public bool Composition;
        public string Message;
        public bool DownloadReport;

        public ForeignLanguageCompositionCVM(ForeignLanguageCompetitionGroup src, ForeignLanguageCompetitionGroup dst, bool subgroups, bool composition)
        {
            SrcCompetitionGroup = src;
            DstCompetitionGroup = dst;
            Subgroups = subgroups;
            Composition = composition;
        }

        public List<string> ExceptedSubgroupCounts { get; set; }
        //public List<> ExceptedStudents { get; set; }
    }
}
