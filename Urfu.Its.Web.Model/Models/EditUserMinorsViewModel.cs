using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class EditUserMinorsViewModel
    {
        public EditUserMinorsViewModel()
        {
        }

        public EditUserMinorsViewModel(ApplicationUser user, List<Module> minors)
        {
            var selectedIds = new HashSet<string>(user.Minors?.Select(m => m.ModuleId) ?? Enumerable.Empty<string>());
            Rows = new List<EditUserMinorRowViewModel>();
            foreach (var m in minors)
            {
                Rows.Add(new EditUserMinorRowViewModel
                {
                    Checked = selectedIds.Contains(m.uuid),
                    moduleId = m.uuid,
                    Title = m.title
                });
            }
            UserName = user.UserName;
            UserFIO = string.Format("{0} {1} {2}", user.LastName, user.FirstName, user.Patronymic);
        }

        [Required]
        public string UserName { get; set; }

        public string UserFIO { get; set; }

        public List<EditUserMinorRowViewModel> Rows { get; set; }

    }

    public class EditUserMinorRowViewModel
    {
        public EditUserMinorRowViewModel()
        {
        }

        [Key, Required]
        public string moduleId { get; set; }

        public bool Checked { get; set; }

        public string Title { get; set; }

    }

}