//#define isLocal
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
//using Owin;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList.Core;
using Urfu.Its.Web.Excel;
//using SortDirection = Ext.Utilities.SortDirection;
using Urfu.Its.Web.Model.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SortDirection = Ext.Utilities.SortDirection;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationUserManager _userManager;
        private IDataProtector _protector;
        public SignInManager<ApplicationUser> _signinmanager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            _signinmanager = new SignInManager<ApplicationUser>(userManager, null, null, null, null, null, null);
        }
        public AccountController(ApplicationUserManager userManager, IHttpContextAccessor ca, IUserClaimsPrincipalFactory<ApplicationUser> claim, Microsoft.Extensions.Options.IOptions<IdentityOptions> io,
            Microsoft.Extensions.Logging.ILogger<SignInManager<ApplicationUser>> log, Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider scheme, IUserConfirmation<ApplicationUser> confirm)
        {
            UserManager = userManager;
            _signinmanager = new SignInManager<ApplicationUser>(userManager, ca, claim, io, log, scheme, confirm);
        }
        public AccountController(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("AccountController");
        }
        public SignInManager<ApplicationUser> SignInM
        {
            get
            {
                return _signinmanager ?? HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
            }
            set
            {
                _signinmanager = value;
            }
        }
        
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.RequestServices.GetRequiredService<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string role = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (role != null)
                ViewBag.SecurityMessage = "Для запрошенной страницы необходима роль '" + ItsRoles.GetDescription(role) + "'";
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string role = null)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = TryFindADUser(model);
                if (user == null)
                    user = await UserManager.FindByNameAsync(model.UserName);//, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    if (user.ShouldChangePassword)
                        return RedirectToAction("Manage", "Account");
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            if (role != null)
                ViewBag.SecurityMessage = "Для этой страницы необходима роль '" + ItsRoles.GetDescription(role) + "'";
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        internal async Task<bool> ApiLogin(string userName, string password)
        {
            var model = new LoginViewModel { UserName = userName, Password = password };
            ApplicationUser user = TryFindADUser(model);
            if (user == null)
                user = await UserManager.FindByNameAsync(model.UserName);//, model.Password);
            if (user != null && await UserManager.IsInRoleAsync(user, ItsRoles.ServiceLogin))
            {
                //Task.Factory.StartNew(() => SignInAsync(user, model.RememberMe)).Wait();
                return true;
            }
            return false;
        }

        private ApplicationUser TryFindADUser(LoginViewModel model)
        {
            ApplicationUser dbUser;
            using (var db = new ApplicationDbContext())
            {
                dbUser = db.Users.FirstOrDefault(u => u.AdName == model.UserName);
            }
#if isLocal
            return dbUser;
#endif
            bool credentialsValid = false;
            try
            {
                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain))
                {
                    credentialsValid = principalContext.ValidateCredentials(model.UserName, model.Password);
                }
            }
            catch (Exception e)
            {
                if (e is PrincipalServerDownException || e is LdapException)
                    Logger.Warning("Сервер Active Directory недоступен");
                else
                    throw;
            }
            if (credentialsValid)
                return dbUser;
            return null;
        }

        //
        // GET: /Account/Register
        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = ItsRoles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserName = model.UserName.Trim();

                var user = new ApplicationUser
                {
                    Id = model.UserName,
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Patronymic = model.Patronymic,
                    AdName = model.AdName,
                };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //await SignInAsync(user, isPersistent: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public JsonResult IsAdNameAvailable(string AdName)
        {
            using (var db = new ApplicationDbContext())
            {
                var exist = db.Users.Any(_ => _.AdName.Equals(AdName, StringComparison.InvariantCultureIgnoreCase));
                if (!exist)
                {
                    return Json(true, new Newtonsoft.Json.JsonSerializerSettings());
                }
                return Json("Пользователь с таким логин AD уже существует", new Newtonsoft.Json.JsonSerializerSettings());
            }
        }

        public JsonResult IsUserNameAvailable(string UserName)
        {
            using (var db = new ApplicationDbContext())
            {
                var exist = db.Users.Any(_ => _.UserName.Equals(UserName, StringComparison.InvariantCultureIgnoreCase));
                if (!exist)
                {
                    return Json(true, new Newtonsoft.Json.JsonSerializerSettings());
                }
                return Json("Пользователь с таким логином уже существует", new Newtonsoft.Json.JsonSerializerSettings());
            }
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult Index(int? page, int? limit, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                ViewBag.Focus = focus;
                return View();
            }
            using (var db = new ApplicationDbContext())
            {
                var users = db.Users.Select(u => new
                {
                    Id = u.UserName,
                    u.UserName,
                    u.FirstName,
                    u.LastName,
                    u.Patronymic,
                    u.AdName,
                    u.Email,
                    Directions = u.Directions.Select(d => d.Direction.okso),
                    Minors = u.Minors.Select(m => m.Module.title),
                    Divisions = u.UserDivisions.Select(d => d.Division.title),
                    AllDirections = u.Roles.Any(r => r.RoleId == db.Roles.FirstOrDefault(rx => rx.Name == ItsRoles.AllDirections).Id),
                    AllMinors = u.Roles.Any(r => r.RoleId == db.Roles.FirstOrDefault(rx => rx.Name == ItsRoles.AllMinors).Id),
                    RoleSets = db.RoleSets
                        .Where(r =>
                            r.Contents.All(c => u.Roles.Any(rx => rx.RoleId == c.RoleId))
                        ).OrderByDescending(r => r.Contents.Count).Select(r => r.Name)
                });

                SortRules sortRules = SortRules.Deserialize(sort);

                var rule = sortRules.FirstOrDefault();
                if (rule?.Property == "Directions")
                {
                    if (rule.Direction == SortDirection.Ascending)
                        users = users.OrderBy(v => v.AllDirections).ThenBy(v => v.Directions.FirstOrDefault());
                    else
                        users = users.OrderByDescending(v => v.AllDirections).ThenByDescending(v => v.Directions.FirstOrDefault());
                }
                else if (rule?.Property == "Divisions")
                {
                    if (rule.Direction == SortDirection.Ascending)
                        users = users.OrderBy(v => v.AllDirections).ThenBy(v => v.Divisions.FirstOrDefault());
                    else
                        users = users.OrderByDescending(v => v.AllDirections).ThenByDescending(v => v.Divisions.FirstOrDefault());
                }
                else if (rule?.Property == "Minors")
                {
                    if (rule.Direction == SortDirection.Ascending)
                        users = users.OrderBy(v => v.AllMinors).ThenBy(v => v.Minors.FirstOrDefault());
                    else
                        users = users.OrderByDescending(v => v.AllMinors).ThenByDescending(v => v.Minors.FirstOrDefault());
                }
                else if (rule?.Property == "RoleSets")
                {
                    if (rule.Direction == SortDirection.Ascending)
                        users = users.OrderBy(v => v.RoleSets.FirstOrDefault());
                    else
                        users = users.OrderByDescending(v => v.RoleSets.FirstOrDefault());
                }
                else
                {
                    users = rule != null ? users.OrderBy(rule, v => v.LastName) : users.OrderBy(_=>_.LastName).ThenBy(_=>_.FirstName).ThenBy(_=>_.Patronymic);
                }

                users = users.Where(FilterRules.Deserialize(filter));

                var paginated = users.ToPagedList(page ?? 1, limit ?? 25);

                return JsonNet(new
                {
                    data = paginated,
                    total = users.Count()
                });
            }
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult DownloadReport()
        {
            using (var db = new ApplicationDbContext())
            {
                var users =
                    db.Users.Select(
                        u =>
                            new
                            {
                                Id = u.UserName,
                                u.UserName,
                                u.FirstName,
                                u.LastName,
                                u.Patronymic,
                                u.AdName,
                                u.Email,
                                Directions = u.Directions.Select(d => d.Direction.okso),
                                Divisions = u.UserDivisions.Select(d => d.Division.shortTitle),
                                AllDirections =
                                    u.Roles.Any(
                                        r =>
                                            r.RoleId ==
                                            db.Roles.FirstOrDefault(rx => rx.Name == ItsRoles.AllDirections).Id),
                                RoleSets = db.RoleSets
                        .Where(r =>
                            r.Contents.All(c => u.Roles.Any(rx => rx.RoleId == c.RoleId))
                        ).OrderByDescending(r => r.Contents.Count).Select(r => r.Name)
                            }).ToList()
                            .Select(u => new
                            {
                                u.UserName,
                                u.FirstName,
                                u.LastName,
                                u.Patronymic,
                                u.AdName,
                                u.Email,
                                RoleSets = string.Join(", ", u.RoleSets),
                                Directions = u.AllDirections ? "Все" : string.Join(", ", u.Directions),
                                Divisions = u.AllDirections ? "Все" : string.Join(", ", u.Divisions),
                            });
                var stream = new VariantExport().Export(new { Rows = users },
                    "usersReportTemplate.xlsx");


                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Отчёт по пользователям.xlsx".ToDownloadFileName());
            }
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult Edit(string id, ManageMessageId? Message = null)
        {
            ApplicationUser user;
            using (var db = new ApplicationDbContext())
            {
                user = db.Users.First(u => u.UserName == id);

                var model = new EditUserViewModel(user);
                ViewBag.MessageId = Message;
                return View(model);
            }
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public async Task<ActionResult> Impersonate(string id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                await SignInAsync(user, true);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (model.ResetPassword)
            {
                if (model.NewPassword != model.NewPasswordConfirmation)
                    ModelState.AddModelError("NewPassword", "Введённые парли не совпадают");

                if (string.IsNullOrWhiteSpace(model.NewPassword))
                    ModelState.AddModelError("NewPassword", "Введите новый пароль");
            }

            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext())
                {
                    var user = db.Users.First(u => u.UserName == model.UserName);
                    // Update the user data:
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.AdName = model.AdName;
                    user.Patronymic = model.Patronymic;

                    db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await db.SaveChangesAsync();

                    if (model.ResetPassword)
                    {
                        await UserManager.RemovePasswordAsync(user);
                        await UserManager.UpdateSecurityStampAsync(user);
                        await UserManager.AddPasswordAsync(user, model.NewPassword);
                    }
                    
                }
                return RedirectToAction("Index", new { focus = model.UserName });
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult Delete(string id = null)
        {
            ApplicationUser user;
            using (var db = new ApplicationDbContext())
            {
                user = db.Users.First(u => u.UserName == id);

                var model = new EditUserViewModel(user);
                if (user == null)
                {
                    return NotFound();
                }
                return View(model);
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult DeleteConfirmed(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.First(u => u.UserName == id);
                db.Users.Remove(user);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult UserRoles(string id)
        {
            ApplicationUser user;
            using (var db = new ApplicationDbContext())
            {
                user = db.Users.First(u => u.UserName == id);

                var model = new SelectUserRolesViewModel(user);
                return View(model);
            }
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult ApplyRoleSet(string id, int set)
        {
            var user = new ApplicationUser();
            user = user.GetUser(id);
            using (var db = new ApplicationDbContext())
            {
                bool success = true;
                var rs = db.RoleSets.Find(set);
                var um = _userManager; //new ApplicationUserManager(new UserStore<ApplicationUser>(db), new IdentityOptions, new IPasswordHasher<ApplicationUser>,
                  

                var currentRoles = um.GetRolesAsync(user).Result;

                var toRemove = currentRoles.Where(r => rs.Contents.All(cr => cr.Role.Name != r));
                var toAdd = rs.Contents.Where(r => currentRoles.All(cr => r.Role.Name != cr));
                foreach (var role in toAdd)
                {
                    var result = um.AddToRoleAsync(user, role.Role.Name).Result;
                    if (!result.Succeeded)
                        success = false;
                }
                foreach (var role in toRemove)
                {
                    if (user.Id == "Administrator")
                        continue;
                    var result = um.RemoveFromRoleAsync(user, role).Result;
                    if (!result.Succeeded)
                        success = false;
                }

                return RedirectToAction("UserRoles", new { id });
            }
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult AppendRoleSet(string id, int set)
        {
            var user = new ApplicationUser();
            user = user.GetUser(id);
            using (var db = new ApplicationDbContext())
            {
                bool success = true;
                var rs = db.RoleSets.Find(set);
                var um = _userManager; //new ApplicationUserManager(new UserStore<ApplicationUser>(db));

                var currentRoles = um.GetRolesAsync(user).Result;

                var toAdd = rs.Contents.Where(r => currentRoles.All(cr => r.Role.Name != cr));
                foreach (var role in toAdd)
                {
                    var result = um.AddToRoleAsync(user, role.Role.Name).Result;
                    if (!result.Succeeded)
                        success = false;
                }

                return RedirectToAction("UserRoles", new { id });
            }
        }


        [HttpPost]
        [Authorize(Roles = ItsRoles.Admin)]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext())
                {
                    bool success = true;
                    var um = UserManager;

                    var currentRoles = um.GetRolesAsync(user).Result;

                    var toRemove = model.Roles.Where(r => !r.Selected && currentRoles.Any(cr => r.RoleName == cr));
                    var toAdd = model.Roles.Where(r => r.Selected && currentRoles.All(cr => r.RoleName != cr));
                    foreach (var role in toAdd)
                    {
                        var result = um.AddToRoleAsync(user, role.RoleName).Result;
                        if (!result.Succeeded)
                            success = false;
                    }
                    foreach (var role in toRemove)
                    {
                        if (model.UserName == "Administrator" && role.RoleName == ItsRoles.Admin)
                            continue;
                        var result = um.RemoveFromRoleAsync(user, role.RoleName).Result;
                        if (!result.Succeeded)
                            success = false;
                    }

                    if (!success)
                        return View();


                    return RedirectToAction("Index", new { focus = model.UserName });
                }
            }
            return View();
        }


        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult UserDirections(string id)
        {
            ApplicationUser user;
            using (var db = new ApplicationDbContext())
            {
                user = db.Users.Include(u => u.Directions).Single(u => u.UserName == id);
                var model = new EditUserDirectionsViewModel(user, db.Directions.OrderBy(d => d.okso).ThenBy(d => d.title).ToList());
                return View(model);
            }
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.Admin)]
        [ValidateAntiForgeryToken]
        public ActionResult UserDirections(EditUserDirectionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext())
                {
                    var user = db.Users
                        .Include(u => u.Directions)
                        .Single(u => u.UserName == model.UserName);

                    user.Directions.Clear();

                    foreach (var row in model.Rows)
                    {
                        if (row.Checked)
                            user.Directions.Add(new UserDirection
                            {
                                DirectionId = row.directionId,
                                UserName = user.UserName
                            });
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index", new { focus = model.UserName });
                }
            }
            return View();
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult UserMinors(string id)
        {
            ApplicationUser user;
            using (var db = new ApplicationDbContext())
            {
                user = db.Users.Include(u => u.Minors).Single(u => u.UserName == id);
                var model = new EditUserMinorsViewModel(user, db.UniModules().Where(m => m.type.Contains("Майноры")).OrderBy(m => m.title).ToList());
                return View(model);
            }
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.Admin)]
        [ValidateAntiForgeryToken]
        public ActionResult UserMinors(EditUserMinorsViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext())
                {
                    var user = db.Users
                        .Include(u => u.Minors)
                        .Single(u => u.UserName == model.UserName);

                    user.Minors.Clear();

                    foreach (var row in model.Rows)
                    {
                        if (row.Checked)
                            user.Minors.Add(new UserMinor
                            {
                                ModuleId = row.moduleId,
                                UserName = user.UserName
                            });
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index", new { focus = model.UserName });
                }
            }
            return View();
        }


        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult UserDivisions(string id)
        {
            ApplicationUser user;
            using (var db = new ApplicationDbContext())
            {
                user = db.Users.Include(u => u.UserDivisions).Single(u => u.UserName == id);

                var model = new EditUserDivisionsViewModel(user);

                return View(model);
            }
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult UserDivisionsTree(string id, string filter)
        {
            ApplicationUser user;
            using (var db = new ApplicationDbContext())
            {
                user = db.Users.Include(u => u.UserDivisions).Single(u => u.UserName == id);

                var userFilteringDivisions = db.PossibleDivisionsForUser(user.UserName).Where(FilterRules.Deserialize(filter)).ToList();

                var userDivisions = db.PossibleDivisionsForUser(user.UserName).ToList();

                var model = new DivisionsViewModel(user, userDivisions, userFilteringDivisions);
                return JsonNet(model.Roots);
            }
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.Admin)]
        //[ValidateAntiForgeryToken]
        public ActionResult UserDivisions(string userName, List<string> checkedIds)
        {
            if (userName != null)
            {
                using (var db = new ApplicationDbContext())
                {
                    var user = db.Users
                        .Include(u => u.UserDivisions)
                        .Single(u => u.UserName == userName);

                    user.UserDivisions.Clear();

                    if (checkedIds != null)
                    {
                        foreach (var id in checkedIds)
                        {
                            user.UserDivisions.Add(new UserDivision
                            {
                                DivisionId = id,
                                UserName = user.UserName
                            });
                        }
                    }
                    db.SaveChanges();
                    return Json("");
                }
            }
            return NotFound("user not found");
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult GetDivisions(string userName)
        {
            if (userName != null)
            {
                using (var db = new ApplicationDbContext())
                {                                 
                    var userDivisions = db.PossibleDivisionsForUser(userName).Select(u => u.uuid).ToList();
                    var divisions = db.Divisions.Where(d => (d.typeCode == "institute" ||d.typeCode =="branch" )&& 
                                                            !userDivisions.Contains(d.uuid))
                    .OrderBy(d => d.title).
                    Select(d => new
                    {
                        InstituteId = d.uuid,
                        Name = d.typeTitle + " " + d.title
                    });
                    return JsonNet(new { data = divisions });
                }
            }
            return NotFound("user not found");
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            var user = new ApplicationUser();
            user = user.GetUser(userId);
            if (userId == null || code == null)
            {
                return View("Error");
            }

            IdentityResult result = await UserManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            else
            {
                AddErrors(result);
                return View();
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user)))
                {
                    ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                    return View();
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            if (code == null)
            {
                return View("Error");
            }
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No user found.");
                    return View();
                }
                IdentityResult result = await UserManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    AddErrors(result);
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (result.Succeeded)
            {                
                await SignInAsync(user, isPersistent: false);
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.Admin)]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    using (var db = new ApplicationDbContext())
                    {
                        db.Users.Find(User.Identity.Name).ShouldChangePassword = false;
                        db.SaveChanges();
                    }
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                //ModelState state = ModelState["OldPassword"];
                if (ModelState["OldPassword"] != null)
                {
                    ModelState["OldPassword"].Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            
            var loginInfo = await _signinmanager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.ProviderKey });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.Name);
        }
        [AllowAnonymous]
        public async Task<IEnumerable<AuthenticationScheme>> AskAuthScheme()
        {
            //var sim = new SignInManager<ApplicationUser>(UserManager, null, null, null, null, null, null);
            var loginInfo = await _signinmanager.GetExternalAuthenticationSchemesAsync();
            //var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.Name);
            
            return loginInfo;
        }
        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            //var sim = new SignInManager<ApplicationUser>(UserManager, null, null, null, null, null, null);
            var loginInfo = await _signinmanager.GetExternalLoginInfoAsync();
            //var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.Name);
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            IdentityResult result = await UserManager.AddLoginAsync(user, loginInfo);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                //var sim = new SignInManager<ApplicationUser>(UserManager, null, null, null, null, null, null);
                var info = await _signinmanager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");

                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var sim = new SignInManager<ApplicationUser>(UserManager, null, null, null, null, null, null);
            sim.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //[ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;

            var linkedAccounts = UserManager.GetLoginsAsync(user).Result;
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult AdUsers()
        {
            var model = new List<EditUserViewModel>();

            using (var context = new PrincipalContext(ContextType.Domain))
            {
                using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    foreach (var result in searcher.FindAll())
                    {
                        try
                        {
                            DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                            if (de == null)
                                continue;
                            var u = new EditUserViewModel()
                            {
                                FirstName = de.Properties["givenName"].Value.ToString(),
                                LastName = de.Properties["sn"].Value.ToString(),
                                AdName =
                                    "SAM account name   : " + de.Properties["samAccountName"].Value +
                                    "User principal name: " + de.Properties["userPrincipalName"].Value,
                            };

                            model.Add(u);
                        }
                        catch
                        {

                        }
                    }
                }
            }

            return View(model);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        /*private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }*/

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            var sim = new SignInManager<ApplicationUser>(UserManager, null, null, null, null, null, null);
            await sim.SignOutAsync();
            await sim.SignInAsync(user, new AuthenticationProperties() { IsPersistent = isPersistent });//, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Code);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private void SendEmail(string email, string callbackUrl, string subject, string message)
        {
            // For information on sending mail, please visit http://go.microsoft.com/fwlink/?LinkID=320771
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = ItsRoles.Admin)]
        public ActionResult Sessions()
        {
           /* var protectedText = HttpContext.Request.Cookies[".AspNetCore.Session"];

            var sessionKey = "";
            var protectedData = Convert.FromBase64String(protectedText);
            if (protectedData == null)
            {
                sessionKey = string.Empty;
            }
           
            var userData = _protector.Unprotect(protectedData);
            if (userData == null)
            {
                sessionKey = string.Empty;
            }

            sessionKey = Encoding.UTF8.GetString(userData);

            return Content(sessionKey);*/
            return View(Startup.SessionKeys.ToArray().Select(sk => sk.Value).Where(s => s.UserName != null));
        }

        private class ChallengeResult : UnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

           /* public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Items[XsrfKey] = UserId;
                }
                AuthenticationHttpContextExtensions.ChallengeAsync(context.HttpContext, properties);
                //context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }*/
        }
        #endregion
    }
}