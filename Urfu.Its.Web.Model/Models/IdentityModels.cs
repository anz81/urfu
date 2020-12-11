using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Urfu.Its.Web.DataContext;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace Urfu.Its.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    [Table("AspNetUsers")]
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser GetUser(string UserId)
        {
            DbSet<ApplicationUser> allUsers;
            using (var db = new ApplicationDbContext())
            {
                allUsers = db.Users;

                foreach (var user in allUsers)
                {
                    if (user.Id == UserId) return user;
                }
                return null;
            }
        }
        /*public async Task<ClaimsIdentity> GenerateUserIdentityAsync(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }*/

        //[Required]
        [DisplayName("Имя")]
        public string FirstName { get; set; }

        //[Required]
        [DisplayName("Отчество")]
        public string Patronymic { get; set; }

        //[Required]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }

        [Required, MaxLength(127)]
        [DisplayName("Логин в ActiveDirectory")]
        [Index("IX_ApplicationUser_ADName")]
        public string AdName { get; set; }

        [DisplayName("Необходимо сменить пароль")]
        public bool ShouldChangePassword { get; set; }

        [DisplayName("Направления")]
        public virtual ICollection<UserDirection> Directions { get; set; }

        [DisplayName("Майноры")]
        public virtual ICollection<UserMinor> Minors { get; set; }

        [DisplayName("Институты")]
        public virtual ICollection<UserDivision> UserDivisions { get; set; }

        public string SamAccountName { get; set; }

        public virtual ICollection<RoleSetContent> Roles { get; set; }
    }
}
