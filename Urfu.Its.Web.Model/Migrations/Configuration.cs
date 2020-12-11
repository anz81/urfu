using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Urfu.Its.Integration;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Migrations
{
    using System;
    //using System.Data.Entity;
    //using System.Data.Entity.Migrations;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Urfu.Its.Web.DataContext.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Urfu.Its.Web.DataContext.ApplicationDbContext";
            
        }
        
        protected override void Seed(Urfu.Its.Web.DataContext.ApplicationDbContext context)
        {
            var roles = ItsRoles.RoleNames;


            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore,null,null,null,null);

            foreach (var roleName in roles)
            {
                if (!context.Roles.Any(r => r.Name == roleName))
                {
                    var role = new IdentityRole { Name = roleName };

                    roleManager.CreateAsync(role);
                }
            }

            if (!context.Users.Any(u => u.UserName == "Administrator"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store,null,null,null,null,null,null,null,null);
                var user = new ApplicationUser
                {
                    Email = "amborodin@urfu.ru",
                    UserName = "Administrator",
                    ShouldChangePassword = true,
                    FirstName = "Администратор",
                    LastName = "Администратор",
                    Patronymic = "Администратор",
                    AdName = "Administrator",
                    Id = "Administrator"
                };

                manager.CreateAsync(user, "P@ssw0rd");
                foreach (var role in roles)
                {
                    manager.AddToRoleAsync(user, role);
                }
            }

            //context.Database.ExecuteSqlCommand("insert into AspNetUserRoles select UserId,(select Id from AspNetRoles where Name = 'StudentAdmission.view') from AspNetUserRoles where RoleId = (select Id from AspNetRoles where Name = 'StudentAdmission')");

/*
            var eduPrograms = context.EduPrograms.Where(p => p.departmentId == null).ToList();
            if (eduPrograms.Count > 0)
            {
                var divisions = new UniDivisionsService().GetDivisions();

                foreach (var program in eduPrograms)
                {
                    DivisionDto inst;
                    if(!divisions.TryGetValue(program.chairId,out inst))
                        continue;
                    while (!departmentTypeTitles.Contains(inst.typeTitle))
                    {
                        inst = divisions[inst.parent];
                    }
                    program.departmentId = inst.uuid;
                }
            }*/


        }

        private static readonly string[] departmentTypeTitles = { "Факультет", "Институт", "Филиал", "Департамент" };
    }
}
