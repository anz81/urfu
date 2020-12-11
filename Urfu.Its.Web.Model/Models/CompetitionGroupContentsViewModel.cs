using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{

    public class CompetitionGroupDivisionViewModelRow
    {
        public CompetitionGroupDivisionViewModelRow()
        {
        }

        public CompetitionGroupDivisionViewModelRow(List<Division> divisions, List<Division> levels, List<GroupRow> groups,
            ICompetitionGroup competitionGroup)
        {
            if (levels.Count > 0)
            {
                children = new List<CompetitionGroupDivisionViewModelRow>();
            }
            else
                leaf = true;

            foreach (var l in levels)
            {
                var nexts = divisions.Where(d => d.parent == l.uuid).ToList();

                if (l.typeCode == "chair") // добавляем группы
                {
                    foreach (var @group in groups.Where(g => g.ChairId == l.uuid))
                    {

                        var m = new CompetitionGroupDivisionViewModelRow(divisions, nexts, groups, competitionGroup)
                        {
                            text = $"{@group.Name}",
                            nodeId = @group.Id,
                            @checked = competitionGroup.Groups.Any(d => d.Id == group.Id),
                            parent = parent,
                            leaf = true,
                            IsGroup = true

                        };
                        if (parent != null)
                        {
                            parent.children.Add(m);
                        }
                        else
                        {
                            children.Add(m);
                        }

                    }
                }
                else
                {
                    var m = new CompetitionGroupDivisionViewModelRow(divisions, nexts, groups, competitionGroup)
                    {
                        text = $"{l.typeTitle} {l.title}",
                        nodeId = l.uuid,
                        parent = this
                    };
                    if (m.children != null && m.children.Count > 0)
                        m.@checked = m.children.All(_ => _.@checked);



                    children.Add(m);
                }
            }
        }

        public string nodeId { get; set; }
        public bool @checked { get; set; }
        public string text { get; set; }
        public bool leaf { get; set; }
        //public Division Division { get; set; }
        [JsonIgnore]
        public bool IsGroup { get; set; }

        [JsonIgnore]
        public CompetitionGroupDivisionViewModelRow parent { get; set; }

        public List<CompetitionGroupDivisionViewModelRow> children { get; set; }
    }

    public class CompetitionGroupContentsViewModel
    {
        public CompetitionGroupContentsViewModel()
        {
        }

        public CompetitionGroupContentsViewModel(List<Division> divisions, List<GroupRow> groups,
            ICompetitionGroup competitionGroup)
        {
            var level_1 = divisions.Where(d => divisions.All(p => p.uuid != d.parent)).ToList();

            Roots = new List<CompetitionGroupDivisionViewModelRow>();

            foreach (var l in level_1)
            {
                var nexts = divisions.Where(d => d.parent == l.uuid).ToList();
                if (l.typeCode == "chair") // добавляем группы
                {
                    foreach (var @group in groups.Where(g => g.ChairId == l.uuid))
                    {
                        var m = new CompetitionGroupDivisionViewModelRow(divisions, nexts, groups, competitionGroup)
                        {
                            text = $"{@group.Name}",
                            nodeId = @group.Id,
                            @checked = competitionGroup.Groups.Any(d => d.Id == @group.Id),
                            leaf = true,
                            IsGroup = true
                        };
                        Roots.Add(m);
                    }
                }
                else
                {
                    var m = new CompetitionGroupDivisionViewModelRow(divisions, nexts, groups, competitionGroup)
                    {
                        text = $"{l.typeTitle} {l.title}",
                        nodeId = l.uuid
                    };

                    Roots.Add(m);
                }
            }

            while (ContainsEmptyLists(Roots))
            {
                DeleteNotGroups(Roots);
            }
            MakeChecked(Roots, false);
        }

        public List<CompetitionGroupDivisionViewModelRow> Roots { get; set; }

        public List<CompetitionGroupDivisionViewModelRow> GetAllRows()
        {
            var l = new List<CompetitionGroupDivisionViewModelRow>();

            AddRows(l, Roots);

            return l;
        }

        private void AddRows(List<CompetitionGroupDivisionViewModelRow> all,
            List<CompetitionGroupDivisionViewModelRow> list)
        {
            if (list == null) return;

            foreach (var r in list)
            {
                //добавляем только кафедры, у остальных r.Division == null
                if (r.nodeId != null)
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
        /// <param name="isChecked"></param>
        /// <returns></returns>
        private bool MakeChecked(List<CompetitionGroupDivisionViewModelRow> rows, bool isChecked)
        {
            foreach (var row in rows)
            {
                isChecked = isChecked || row.@checked;
                if (row.children != null)
                {
                    row.@checked = MakeChecked(row.children, row.@checked);
                    isChecked = isChecked || row.@checked;
                }
            }
            return isChecked;
        }

        /// <summary>
        /// Проверяет, есть ли ветки, у которых нет групп 
        /// </summary>
        /// <param name="roots"></param>
        /// <returns></returns>
        private bool ContainsEmptyLists(List<CompetitionGroupDivisionViewModelRow> roots)
        {
            bool result = false;

            foreach (var root in roots)
            {
                if (!root.IsGroup)
                {
                    if (root.children == null || root.children.Count == 0)
                        result = true;
                    else
                        result = result || ContainsEmptyLists(root.children);
                }
            }

            return result;
        }

        /// <summary>
        /// Удаляет ветки, которые не содержат группы
        /// </summary>
        /// <param name="roots"></param>
        private void DeleteNotGroups(List<CompetitionGroupDivisionViewModelRow> roots)
        {
            for (int i = 0; i < roots.Count; i++)
            {
                if (!roots[i].IsGroup)
                {
                    if (roots[i].children == null || roots[i].children.Count == 0)
                    {
                        roots.RemoveAt(i);
                        i = -1;
                    }
                    else
                        DeleteNotGroups(roots[i].children);

                }
            }
        }

    }

    public class GroupRow
    {
        public String Id { get; set; }
        public String ChairId { get; set; }
        public String Name { get; set; }
    }

}
