using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Model.Models
{
    public class DivisionsViewModelRow
    {
        public DivisionsViewModelRow()
        {
        }

        public DivisionsViewModelRow(Division level, List<Division> allDivisions, List<Division> divisions, List<Division> levels, HashSet<string> selectedIds)
        {
            Division = level;
            @checked = selectedIds.Contains(level.uuid);
            text = $"{level.typeTitle} {level.title}";
            nodeId = level.uuid;
            expanded = @checked;

            if (levels.Count > 0) // есть потомки
            {
                children = new List<DivisionsViewModelRow>();
                leaf = false;
            }
            else // лист дерева
                leaf = true;

            foreach (var l in levels)
            {
                var nexts = allDivisions.Where(d => d.parent == l.uuid).ToList();

                var m = new DivisionsViewModelRow(l, allDivisions, divisions, nexts, selectedIds);
                children.Add(m);
            }
        }

        public string nodeId { get; set; }

        public bool @checked { get; set; }

        public string text { get; set; }

        public bool leaf { get; set; }

        public bool expanded { get; set; }

        [JsonIgnore]
        public Division Division { get; set; }

        public List<DivisionsViewModelRow> children { get; set; }
    }

    public class DivisionsViewModel
    {
        public DivisionsViewModel(ApplicationUser user, List<Division> allDivisions, List<Division> filteringDivisions)
        {
            var selectedIds = new HashSet<string>(user.UserDivisions?.Select(d => d.DivisionId) ?? Enumerable.Empty<string>());

            User = user;

            var level_1 = filteringDivisions.Where(d => filteringDivisions.All(p => p.uuid != d.parent)).ToList();

            Roots = new List<DivisionsViewModelRow>();

            foreach (var l in level_1)
            {
                var nexts = allDivisions.Where(d => d.parent == l.uuid).ToList();

                var m = new DivisionsViewModelRow(l, allDivisions, filteringDivisions, nexts, selectedIds);
                Roots.Add(m);

            }
            MakeExpanded(Roots, false);
        }

        public ApplicationUser User { get; set; }

        public int PeriodId { get; set; }
        public int DisciplineId { get; set; }
        public IDisciplineTmerPeriod Period { get; set; }
        public List<DivisionsViewModelRow> Roots { get; set; }

        public List<DivisionsViewModelRow> GetAllRows()
        {
            var l = new List<DivisionsViewModelRow>();

            AddRows(l, Roots);

            return l;
        }

        private void AddRows(List<DivisionsViewModelRow> all, List<DivisionsViewModelRow> list)
        {
            if (list == null) return;

            foreach (var r in list)
            {
                //добавляем только кафедры, у остальных r.Division == null
                if (r.Division != null)
                    all.Add(r);

                if (r.children != null)
                    AddRows(all, r.children);
            }
        }

        /// <summary>
        /// Проходит по всему дереву, отмечает галочкой и открывает родителей, 
        /// у которых отмечен хотя бы один ребенок 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="expanded"></param>
        /// <returns></returns>
        private bool MakeExpanded(List<DivisionsViewModelRow> rows, bool expanded)
        {
            foreach (var row in rows)
            {
                expanded = expanded || row.expanded;
                if (row.children != null)
                {
                    row.expanded = MakeExpanded(row.children, row.expanded);
                    row.@checked = row.expanded;
                    expanded = expanded || row.expanded;
                }
            }
            return expanded;
        }
    }
}
