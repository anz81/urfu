using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class EditUserDirectionsViewModel
    {
        public EditUserDirectionsViewModel()
        {
        }

        public EditUserDirectionsViewModel(ApplicationUser user, List<Direction> directions)
        {
            var selectedIds = new HashSet<string>(user.Directions?.Select(d=>d.DirectionId)??Enumerable.Empty<string>());
            Rows =new List<EditUserDirectionRowViewModel>();
            foreach (var d in directions)
            {
                Rows.Add(new EditUserDirectionRowViewModel
                {
                    Checked = selectedIds.Contains(d.uid),
                    directionId = d.uid,
                    Okso = d.okso,
                    Title = d.title,
                    Standard = d.standard
                });
            }
            UserName = user.UserName;
            UserFIO = string.Format("{0} {1} {2}", user.LastName, user.FirstName, user.Patronymic);
        }

        [Required]
        public string UserName { get; set; }

        public string UserFIO { get; set; }

        public List<EditUserDirectionRowViewModel> Rows { get; set; }
    }

    public class EditUserDirectionRowViewModel
    {
        public EditUserDirectionRowViewModel()
        {
        }

        [Key,Required]
        public string directionId { get; set; }

        public bool Checked { get; set; }

        public string Okso { get; set; }

        public string Title { get; set; }
        public string Standard { get; set; }
    }
}