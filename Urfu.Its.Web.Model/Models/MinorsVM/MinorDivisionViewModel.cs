using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class MinorDivisionViewModelRow
    {
        public MinorDivisionViewModelRow()
        {
        }

        public MinorDivisionViewModelRow(List<Division> divisions, List<Division> levels, IDisciplineTmerPeriod period)
        {
            if (levels.Count > 0)
            {
                Childs = new List<MinorDivisionViewModelRow>();
            }

            foreach (var l in levels)
            {
                var nexts = divisions.Where(d => d.parent == l.uuid).ToList();

                var m = new MinorDivisionViewModelRow (divisions, nexts, period);

                m.Division = l;
                m.DivisionID = l.uuid;
                m.Selected = period.Divisions.Any(d => d.uuid == l.uuid);

                Childs.Add(m);
            }
        }

        public string DivisionID { get; set; }
        public bool Selected { get; set; }

        public Division Division { get; set; }
        
        public List<MinorDivisionViewModelRow> Childs { get; set; } 
    }

    public class MinorDivisionViewModel
    {
        public MinorDivisionViewModel()
        {
        }

        public MinorDivisionViewModel(List<Division> divisions, IDisciplineTmerPeriod period, int disciplineId)
        {
            Period = period;
            DisciplineId = disciplineId;

            var level_1 = divisions.Where(d =>  divisions.All(p=>p.uuid != d.parent)).ToList();

            Roots = new List<MinorDivisionViewModelRow>();

            foreach (var l in level_1)
            {
                var nexts = divisions.Where(d => d.parent == l.uuid).ToList();

                var m = new MinorDivisionViewModelRow(divisions, nexts, period);

                m.Division = l;

                m.DivisionID = l.uuid;
                m.Selected = period.Divisions.Any(d => d.uuid == l.uuid);

                Roots.Add(m);
            }
        }

        public int PeriodId { get; set; }
        public int DisciplineId { get; set; }
        public IDisciplineTmerPeriod Period { get; set; }
        public List<MinorDivisionViewModelRow> Roots { get; set; }

        public List<MinorDivisionViewModelRow> GetAllRows()
        {
            var l = new List<MinorDivisionViewModelRow>();

            AddRows(l, Roots);

            return l;
        }

        private void AddRows(List<MinorDivisionViewModelRow> all, List<MinorDivisionViewModelRow> list)
        {
            if (list == null) return;

            foreach (var r in list)
            {
                //добавляем только кафедры, у остальных r.Division == null
                if (r.DivisionID != null)
                    all.Add(r);

                if (r.Childs != null)
                    AddRows(all, r.Childs);
            }
        }
    }

}