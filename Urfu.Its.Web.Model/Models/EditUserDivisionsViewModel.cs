using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class EditUserDivisionsViewModel
    {
        public EditUserDivisionsViewModel()
        {
        }

        public EditUserDivisionsViewModel(ApplicationUser user)
        {
            UserName = user.UserName;
            UserFIO = string.Format("{0} {1} {2}", user.LastName, user.FirstName, user.Patronymic);
            Id = user.Id;
        }

        [Required]
        public string UserName { get; set; }

        public string UserFIO { get; set; }

        public string Id { get; set; }
        
    }
}