using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class VariantGroupModel
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public VariantGroupType GroupType { get; set; }
        public int TestUnits { get; set; }
        public IEnumerable<int> SemestersTestUnits { get; set; }
        public int WithoutGroupSelection { get; set; }
        public int WithGroupSelection { get; set; }

        /// <summary>
        /// Признак общей группы, для которой считаются все з.е.
        /// Группа с эти признаком участвует в итоговом подсчете з.е. и для нее можно редактировать з.е. по плану
        /// </summary>
        public bool GeneralGroup { get; set; }

        public VariantGroupModel(VariantGroup group, IEnumerable<EditVariantContentRowViewModel> rows)
        {
            string addition = group.SubgroupType == VariantGroupType.Selectable ? " (По выбору)" : "";

            Id = group.Id;
            GroupName = $"{EnumHelper<VariantGroupType>.GetDisplayValue(group.GroupType)}{addition}";
            GroupType = group.GroupType;
            TestUnits = group.TestUnits;
            SemestersTestUnits = Enumerable.Range(1, 8)
                .Select(term => TestUnitsForTerm(term, rows.Where(r => r.RealGroupType == group.GroupType && (r.Selected || r.Base) 
                                                                            && (!group.SubgroupType.HasValue ||
                                                                                        group.SubgroupType == VariantGroupType.Selectable
                                                                                        && r.SubgroupType == group.SubgroupType))));
            GeneralGroup = !group.SubgroupType.HasValue;
        }

        private int TestUnitsForTerm(int term, IEnumerable<EditVariantContentRowViewModel> rows)
        {
            int sum = 0;
            foreach (var row in rows.GroupBy(r => r.SelectionGroupId))
            {
                var plans = row.SelectMany(r => r.Plans.Select(p => p.GetTermTestUnits(term))).Where(r => r > 0);

                sum += row.Key.HasValue && plans.Count() > 0
                    ? plans.Sum(p => p) / plans.Count()
                    : plans.Sum(p => p);
            }
            return sum;
        }
    }
}
