using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
//using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Urfu.Its.Common;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Urfu.Its.Web.Controllers
{
    /*[System.Web.Http.Authorize(Roles = ItsRoles.NsiEdit)]*/
    [IdentityBasicAuthentication]
    public class StudentSelectionChangedController : BaseController
    {
        public object Post(List<StudentSelectionDto> selections)
        {
            Logger.Info("Запрос api выбора студентов");
            var orphanIdList = new List<string>();
            try
            {
                var affected = SyncEngine.WriteStudentSelectionsToDatabase(selections, orphanIdList);
                Logger.Info("Запрос api выбора студентов выполнен");
                return new {affected, orphanIdList};
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new {error = ex.ToString()};
            }
        }
    }

    public abstract class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        public string Realm { get; set; }

      /*  public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var serviceLogonAllowed = context.Principal !=null && context.Principal.IsInRole(ItsRoles.ServiceLogin);
            if (serviceLogonAllowed)
                return;
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            if (authorization == null)
            {
                // No authentication was attempted (for this authentication method).
                // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
                context.ErrorResult = new AuthenticationFailureResult("Missing authorization", request);
                return;
            }

            if (authorization.Scheme != "Basic")
            {
                // No authentication was attempted (for this authentication method).
                // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
                context.ErrorResult = new AuthenticationFailureResult("Invalid authorization scheme", request);
                return;
            }

            if (String.IsNullOrEmpty(authorization.Parameter))
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            Tuple<string, string> userNameAndPasword = ExtractUserNameAndPassword(authorization.Parameter);

            if (userNameAndPasword == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                return;
            }

            string userName = userNameAndPasword.Item1;
            string password = userNameAndPasword.Item2;

            IPrincipal principal = await AuthenticateAsync(userName, password, cancellationToken);

            if (principal == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);                
            }
            else
            {
                // Authentication was attempted and succeeded. Set Principal to the authenticated user.
                context.Principal = principal;
            }
        }*/

        protected abstract Task<IPrincipal> AuthenticateAsync(string userName, string password,
            CancellationToken cancellationToken);

        private static Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
        {
            byte[] credentialBytes;

            try
            {
                credentialBytes = Convert.FromBase64String(authorizationParameter);
            }
            catch (FormatException)
            {
                return null;
            }

            // The currently approved HTTP 1.1 specification says characters here are ISO-8859-1.
            // However, the current draft updated specification for HTTP 1.1 indicates this encoding is infrequently
            // used in practice and defines behavior only for ASCII.
            Encoding encoding = Encoding.ASCII;
            // Make a writable copy of the encoding to enable setting a decoder fallback.
            encoding = (Encoding)encoding.Clone();
            // Fail on invalid bytes rather than silently replacing and continuing.
            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
            string decodedCredentials;

            try
            {
                decodedCredentials = encoding.GetString(credentialBytes);
            }
            catch (DecoderFallbackException)
            {
                return null;
            }

            if (String.IsNullOrEmpty(decodedCredentials))
            {
                return null;
            }

            int colonIndex = decodedCredentials.IndexOf(':');

            if (colonIndex == -1)
            {
                return null;
            }

            string userName = decodedCredentials.Substring(0, colonIndex);
            string password = decodedCredentials.Substring(colonIndex + 1);
            return new Tuple<string, string>(userName, password);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter;

            if (String.IsNullOrEmpty(Realm))
            {
                parameter = null;
            }
            else
            {
                // A correct implementation should verify that Realm does not contain a quote character unless properly
                // escaped (precededed by a backslash that is not itself escaped).
                parameter = "realm=\"" + Realm + "\"";
            }

            context.ChallengeWith("Basic", parameter);
        }

        public virtual bool AllowMultiple
        {
            get { return false; }
        }
    }

    public static class HttpAuthenticationChallengeContextExtensions
    {
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme)
        {
            ChallengeWith(context, new AuthenticationHeaderValue(scheme));
        }

        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme, string parameter)
        {
            ChallengeWith(context, new AuthenticationHeaderValue(scheme, parameter));
        }

        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, AuthenticationHeaderValue challenge)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
        }
    }

    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected override async Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();


            using (var applicationDbContext = new ApplicationDbContext())
            {
                if (!new AccountController(new ApplicationUserManager(new UserStore<ApplicationUser>(applicationDbContext))).ApiLogin(userName, password).Result)
                {
                    // No user with userName/password exists.
                    return null;
                }
            }

            // Create a ClaimsIdentity with all the claims for this user.
            Claim nameClaim = new Claim(ClaimTypes.Name, userName);
            List<Claim> claims = new List<Claim> { nameClaim };

            // important to set the identity this way, otherwise IsAuthenticated will be false
            // see: http://leastprivilege.com/2012/09/24/claimsidentity-isauthenticated-and-authenticationtype-in-net-4-5/
            ClaimsIdentity identity = new ClaimsIdentity(claims);

            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

    }


    public class AuthenticationFailureResult : IActionResult
    {
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public async Task ExecuteResultAsync(ActionContext action)
        {
            
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase;
            return response;
        }
    }


    public class AddChallengeOnUnauthorizedResult : IActionResult
    {
        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IActionResult innerResult)
        {
            Challenge = challenge;
            InnerResult = innerResult;
        }

        public AuthenticationHeaderValue Challenge { get; private set; }

        public IActionResult InnerResult { get; private set; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            HttpResponse response = context.HttpContext.Response;
                await InnerResult.ExecuteResultAsync(context);

            if (response.StatusCode == 401) //HttpStatusCode.Unauthorized)
            {
                // Only add one challenge per authentication scheme.
                if (!response.Headers.TryGetValue("Authorization", out StringValues authToken))
                {
                    response.Headers.Add(Challenge.ToString(), authToken);
                }
            }

            //return response;
        }
               
    }
}