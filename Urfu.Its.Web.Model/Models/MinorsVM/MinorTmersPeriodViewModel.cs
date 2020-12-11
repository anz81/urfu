using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class MinorTmersPeriodRowViewModel
    {
        public MinorDisciplineTmer Tmer { get; set; }
        public MinorPeriod Period { get; set; }
        public bool Checked { get; set; }
    }

    public class MinorTmersPeriodViewModel
    {
        public MinorDiscipline Discipline { get; set; }
        public Minor Minor { get; set; }

        public List<MinorTmersPeriodRowViewModel> Rows { get; set; }

        public MinorTmersPeriodViewModel()
        {
        }

        public MinorTmersPeriodViewModel(Minor minor, MinorDiscipline discipline)
        {
            Minor = minor;
            Discipline = discipline;
            Rows = new List<MinorTmersPeriodRowViewModel>();

            foreach (var t in discipline.Tmers)
            {
                foreach (var p in minor.Periods)
                {
                    var r = new MinorTmersPeriodRowViewModel
                    {
                        Tmer = t,
                        Period = p,
                        Checked = t.Periods.Any(f => f.MinorPeriodId == p.Id)
                    };
                    Rows.Add(r);
                }
            }
        }

        public int GetPeriodCount()
        {
            return Minor.Periods?.Count ?? 0;
        }
    }
}