using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Frames.Controllers
{
    public class UserSecurity
    {
        public static bool IsAdmin(string userName, ApplicationDbContext db)
        {
            var usernameoverride = ConfigurationManager.AppSettings["OverrideUserName"];
            if (!string.IsNullOrWhiteSpace(usernameoverride))
                userName = usernameoverride;
            var adminRoleId = db.Roles.FirstOrDefault(f => f.Name == ItsRoles.Admin).Id;
            var user = db.Users.FirstOrDefault(u => u.AdName == userName);
            if (user == null)
                return false;
            if (!user.Roles.Any(r => r.RoleId == adminRoleId))
                return false;
            return true;
        }

        /// <summary>
        /// Заходить на страницу фрейма с проектами может пользователь с ролью Работа с Проектным обучением или РОП Проектного обучения
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static bool IsProjectView(string userName, ApplicationDbContext db)
        {
            var usernameoverride = ConfigurationManager.AppSettings["OverrideUserName"];
            if (!string.IsNullOrWhiteSpace(usernameoverride))
                userName = usernameoverride;
            var roleIds = db.Roles.Where(f => f.Name == ItsRoles.ProjectManager || f.Name == ItsRoles.ProjectROP).Select(f => f.Id);
            var user = db.Users.FirstOrDefault(u => u.AdName == userName);
            if (user == null)
                return false;
            return user.Roles.Any(r => roleIds.Contains(r.RoleId));
        }

        public static string StudentID(IPrincipal user, ApplicationDbContext db)
        {
            var posibleIds = user.GetStudentIds()?.ToArray();
            if (posibleIds == null || posibleIds?.Length <= 1)
            {
                return posibleIds?.FirstOrDefault();
            }
            else
            {
                string studentId = null;
                foreach (var id in posibleIds)
                {
                    studentId = id;
                    if (db.Students.Where(StudentsExtension.ActivityPredicate).Any(s => s.Id == id))
                        break;
                }

                return studentId;
            }
        }


        public static List<string> StudentIDs(IPrincipal user, ApplicationDbContext db)
        {
            var posibleIds = user.GetStudentIds()?.ToArray();
            if (posibleIds == null || posibleIds?.Length == 0)
            {
                return new List<string>();
            }
            else
            {
                var studentIds = new List<string>();
                foreach (var id in posibleIds)
                {
                    if (db.Students.Where(StudentsExtension.ActivityPredicate).Any(s => s.Id == id))
                        studentIds.Add(id);
                }

                return studentIds;
            }
        }
    }
}