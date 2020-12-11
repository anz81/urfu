using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class SectionFKTmersPeriodRowViewModel
    {
        public SectionFKDisciplineTmer Tmer { get; set; }
        public SectionFKPeriod Period { get; set; }
        public bool Checked { get; set; }
        public string Course => Period.Course == null ? "Все" : Period.Course.ToString();

    }

    public class SectionFKTmersPeriodViewModel
    {
        public SectionFKDiscipline Discipline { get; set; }
        public SectionFK SectionFK { get; set; }

        public List<SectionFKTmersPeriodRowViewModel> Rows { get; set; }

        public SectionFKTmersPeriodViewModel()
        {
        }

        public SectionFKTmersPeriodViewModel(SectionFK sectionFK, SectionFKDiscipline discipline)
        {
            SectionFK = sectionFK;
            Discipline = discipline;
            Rows = new List<SectionFKTmersPeriodRowViewModel>();

            foreach (var t in discipline.Tmers)
            {
                foreach (var p in sectionFK.Periods)
                {
                    if (!Rows.Any(r => r.Tmer.Id == t.Id && r.Period.Year == p.Year && r.Period.SemesterId == p.SemesterId && r.Period.Course == p.Course))
                    {
                        var r = new SectionFKTmersPeriodRowViewModel
                        {
                            Tmer = t,
                            Period = p,
                            Checked = t.Periods.Any(f => f.SectionFKPeriodId == p.Id)
                        };
                        Rows.Add(r);
                    }
                }
            }

           Rows= Rows.OrderBy(r => r.Tmer.TmerId).ThenBy(r=>r.Period.Year).ThenBy(r => r.Period.Semester.Id).ThenBy(r => r.Period.Course).ToList();
        }

        public int GetPeriodCount()
        {
            return SectionFK.Periods?.Select(p => new { p.Year, p.SemesterId,p.Course })?.Distinct()?.Count() ?? 0;
        }
    }

    public class ForeignLanguageTmersPeriodRowViewModel
    {
        public ForeignLanguageDisciplineTmer Tmer { get; set; }
        public ForeignLanguagePeriod Period { get; set; }
        public bool Checked { get; set; }
    }

    public class ForeignLanguageTmersPeriodViewModel
    {
        public ForeignLanguageDiscipline Discipline { get; set; }
        public ForeignLanguage ForeignLanguage { get; set; }

        public List<ForeignLanguageTmersPeriodRowViewModel> Rows { get; set; }

        public ForeignLanguageTmersPeriodViewModel()
        {
        }

        public ForeignLanguageTmersPeriodViewModel(ForeignLanguage sectionFK, ForeignLanguageDiscipline discipline)
        {
            ForeignLanguage = sectionFK;
            Discipline = discipline;
            Rows = new List<ForeignLanguageTmersPeriodRowViewModel>();

            foreach (var t in discipline.Tmers)
            {
                foreach (var p in sectionFK.Periods)
                {
                    var r = new ForeignLanguageTmersPeriodRowViewModel
                    {
                        Tmer = t,
                        Period = p,
                        Checked = t.Periods.Any(f => f.ForeignLanguagePeriodId == p.Id)
                    };
                    Rows.Add(r);
                }
            }
        }

        public int GetPeriodCount()
        {
            return ForeignLanguage.Periods?.Count ?? 0;
        }
    }

    public class ProjectTmersPeriodRowViewModel
    {
        public ProjectDisciplineTmer Tmer { get; set; }
        public ProjectPeriod Period { get; set; }
        public bool Checked { get; set; }
    }

    public class ProjectTmersPeriodViewModel
    {
        public ProjectDiscipline Discipline { get; set; }
        public Project Project { get; set; }

        public List<ProjectTmersPeriodRowViewModel> Rows { get; set; }

        public ProjectTmersPeriodViewModel()
        {
        }

        public ProjectTmersPeriodViewModel(Project project, ProjectDiscipline discipline)
        {
            Project = project;
            Discipline = discipline;
            Rows = new List<ProjectTmersPeriodRowViewModel>();

            foreach (var t in discipline.Tmers)
            {
                foreach (var p in project.Periods)
                {
                    var r = new ProjectTmersPeriodRowViewModel
                    {
                        Tmer = t,
                        Period = p,
                        Checked = t.Periods.Any(f => f.ProjectPeriodId == p.Id)
                    };
                    Rows.Add(r);
                }
            }
        }

        public int GetPeriodCount()
        {
            return Project.Periods?.Count ?? 0;
        }
    }

    public class MUPTmersPeriodRowViewModel
    {
        public MUPDisciplineTmer Tmer { get; set; }
        public MUPPeriod Period { get; set; }
        public bool Checked { get; set; }
    }

    public class MUPTmersPeriodViewModel
    {
        public MUPDiscipline Discipline { get; set; }
        public MUP MUP { get; set; }

        public List<MUPTmersPeriodRowViewModel> Rows { get; set; }

        public MUPTmersPeriodViewModel()
        {
        }

        public MUPTmersPeriodViewModel(MUP mup, MUPDiscipline discipline)
        {
            MUP = mup;
            Discipline = discipline;
            Rows = new List<MUPTmersPeriodRowViewModel>();

            foreach (var t in discipline.Tmers)
            {
                foreach (var p in mup.Periods)
                {
                    var r = new MUPTmersPeriodRowViewModel
                    {
                        Tmer = t,
                        Period = p,
                        Checked = t.Periods.Any(f => f.MUPPeriodId == p.Id)
                    };
                    Rows.Add(r);
                }
            }
        }

        public int GetPeriodCount()
        {
            return MUP.Periods?.Count ?? 0;
        }
    }
}