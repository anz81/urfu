using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;

namespace Urfu.Its.Frames.Controllers
{
    static class PrincipalExtensions
    {
        private static string groupsToIgnoreClaim =
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/groupstoignore";

        private static string uniGuidPrefix = "UNI:Guid:";

        public static string GetADName(this IPrincipal principal)
        {
            return (principal as ClaimsPrincipal)?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Upn)?.Value;
        }

        public static IEnumerable<string> GetStudentIds(this IPrincipal principal)
        {
            return (principal as ClaimsPrincipal)?.Claims
                .Where(c => c.Type == groupsToIgnoreClaim && c.Value?.StartsWith(uniGuidPrefix) == true)
                .Select(x => x?.Value.Substring(uniGuidPrefix.Length));
        }
    }
}