using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class OksoViewModelRow
    {
        public List<OksoViewModelRow> children { get; set; }

        public string text { get; set; }
        
        public string nodeId { get; set; }

        public bool @checked { get; set; }

        public bool leaf { get; set; }

        public bool expanded { get; set; }

        public bool isDirection { get; set; }

        /// <summary>
        /// Используется для создания узлов-направлений
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="_checked"></param>
        /// <param name="childs"></param>
        public OksoViewModelRow(string id, string text, bool _checked, bool _isDirection, bool isLeaf)
        {
            children = new List<OksoViewModelRow>();
            isDirection = _isDirection; 
            leaf = isLeaf;
            expanded = false;
            @checked = _checked;
            nodeId = id;
            this.text = text; 
        }
        
    }

    public class OksoViewModel
    {
        public IQueryable<OksoViewModelRow> Roots { get;  set; }

        public OksoViewModel(List<Direction> directions, List<Profile> profiles, IEnumerable<ICollection<ContractLimit>> limits)
        {
            var roots = new List<OksoViewModelRow>();
            directions.OrderBy(d => d.okso);
            profiles.OrderBy(d => d.CODE);
            var checkedDirections = GetDirections(limits);
            var checkedProfiles = GetProfiles(limits);

            foreach(var cd in checkedDirections)
            {
                if (directions.Contains(cd))
                {
                    roots.Add(new OksoViewModelRow(cd.uid, text: cd.OksoAndTitleStandard, _checked: true, _isDirection: true, isLeaf: false));
                }
            }

            foreach(var d in directions)
            {
                if (!checkedDirections.Contains(d))
                {
                    var children = profiles.Where(p => p.DIRECTION_ID == d.uid).ToList();
                    roots.Add(new OksoViewModelRow(d.uid, text: d.OksoAndTitleStandard, _checked: false, _isDirection: true, isLeaf: false));
                }
            }

            // добавление потомков (образовательных программ)
            foreach(var root in roots)
            {
                var rootProfiles = profiles.Where(p => p.DIRECTION_ID == root.nodeId);

                var children = new List<OksoViewModelRow>();

                foreach(var rp in rootProfiles)
                {
                    var child = new OksoViewModelRow(
                        rp.ID,
                        text: rp.OksoAndTitle, 
                        _checked: checkedProfiles.Contains(rp),
                        _isDirection: false,
                        isLeaf: true);
                    children.Add(child);
                }

                root.children = children;
            }

            Roots = roots.AsQueryable();
        }

        private List<Direction> GetDirections(IEnumerable<ICollection<ContractLimit>> limits)
        {
            var directions = new List<Direction>();

            foreach(var limit in limits)
            {
                var d = limit.Where(l => l.Direction != null).Select(l => l.Direction).AsEnumerable();
                directions.AddRange(d);
            }

            directions = directions.GroupBy(d => d.uid).Select(g => g.First()).ToList();

            return directions;
        }

        private List<Profile> GetProfiles(IEnumerable<ICollection<ContractLimit>> limits)
        {
            var profiles = new List<Profile>();

            foreach (var limit in limits)
            {
                var p = limit.Where(l => l.Profile != null).Select(l => l.Profile).AsEnumerable();
                profiles.AddRange(p);
            }

            profiles = profiles.GroupBy(p => p.ID).Select(g => g.First()).ToList();

            return profiles;
        }
    }
}
