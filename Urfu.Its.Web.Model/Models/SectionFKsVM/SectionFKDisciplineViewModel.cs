using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class SectionFKDisciplineViewModel
    {

        public SectionFKDisciplineViewModel()
        {
        }

        public Discipline Discipline { get; set; }
        public SectionFKDiscipline SectionFKDiscipline { get; set; }
    }

    public class ForeignLanguageDisciplineViewModel
    {

        public ForeignLanguageDisciplineViewModel()
        {
        }

        public Discipline Discipline { get; set; }
        public ForeignLanguageDiscipline ForeignLanguageDiscipline { get; set; }
    }

    public class ProjectDisciplineViewModel
    {

        public ProjectDisciplineViewModel()
        {
        }

        public Discipline Discipline { get; set; }
        public ProjectDiscipline ProjectDiscipline { get; set; }
    }

    public class MUPDisciplineViewModel
    {

        public MUPDisciplineViewModel()
        {
        }

        public Discipline Discipline { get; set; }
        public MUPDiscipline MUPDiscipline { get; set; }
    }
}