using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store, null, null, null, null, null, null, null, null)
        {
        }

        /* public static ApplicationUserManager Create(DbContext context, IServiceProvider services) 
         { 
             var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context), services.GetService().Options, 

                 );
             // Configure validation logic for usernames
             manager.UserValidator = new UserValidator<ApplicationUser>(manager)
             {
                 AllowOnlyAlphanumericUserNames = false,
                 RequireUniqueEmail = true
             };
             // Configure validation logic for passwords
             manager.PasswordValidator = new PasswordValidator
             {
                 RequiredLength = 5,
                 //RequireNonLetterOrDigit = true,
                 RequireDigit = true,
                 //RequireLowercase = true,
                 //RequireUppercase = true,
             };
             // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
             // You can write your own provider and plug in here.
             manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
             {
                 MessageFormat = "Your security code is: {0}"
             });
             manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
             {
                 Subject = "Security Code",
                 BodyFormat = "Your security code is: {0}"
             });
             manager.EmailService = new EmailService();
             manager.SmsService = new SmsService();
             var dataProtectionProvider = options.DataProtectionProvider;
             if (dataProtectionProvider != null)
             {
                 manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
             }
             return manager;
         }
        */
        public override async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var claims = await base.GetClaimsAsync(user);
            //ApplicationUser user;
            using (var db = new ApplicationDbContext())
            {
                user = db.Users.Find(user.Id);

                if (user.SamAccountName == null)
                {
                    user.SamAccountName = GetSamAccountName(user.AdName);
                    db.SaveChanges();
                }
            }

            if (user.SamAccountName != null)
            {
                claims.Add(new Claim("samaccountname", user.SamAccountName));
            }

            return claims;
        }

        private string GetSamAccountName(string userName)
        {
            try
            {
                Logger.LogDebug($"Попытка создания PrincipalContext для доступа в АД от имени '{WindowsIdentity.GetCurrent().Name}'");
                using (var principalContext = new PrincipalContext(ContextType.Domain, "at.urfu.ru", "its_wrk.srv", "4h4#e8fj9vDv"))
                {
                    Logger.LogDebug("Создан PrincipalContext для доступа AD");
                    Principal filter = new UserPrincipal(principalContext);
                    filter.UserPrincipalName = userName;                    
                    var principalSearcher = new PrincipalSearcher(filter);
                    var principal = principalSearcher.FindOne();
                    Logger.LogDebug(principal == null ? $"Пользователь Principal.Name='{userName}' не найден" : $"Пользователь Principal.Name='{principal.Name}' найден. Principal.SamAccountName='{principal.SamAccountName}'");
                    //var principals = principalSearcher.FindAll();
                    //var userPrincipal = new UserPrincipal(principalContext);
                    var samAccountName = principal?.SamAccountName;
                    return samAccountName;
                }
            }
            catch (Exception e)
            {
                if (e is PrincipalServerDownException || e is LdapException)
                    Logger.LogDebug($"Сервер Active Directory недоступен.");
                Logger.LogDebug($"SamAccountName не получен для пользователя {userName}.");
                Logger.LogDebug(e.ToString());
                return null;
            }
        }
    }

    public class EmailService
    {
        public Task SendAsync(string to, string subject, string body)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService
    {
        public Task SendAsync(string to, string subject, string body)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
