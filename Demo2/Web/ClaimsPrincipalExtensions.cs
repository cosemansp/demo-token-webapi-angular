using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Demo2.Web
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsInRole(this ClaimsPrincipal principal, IList<string> roles)
        {
            return roles.Any(principal.IsInRole);
        }

        public static string GetClaim(this ClaimsPrincipal principal, string name)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return null;   // not authenticated

            var claim = principal.Claims.FirstOrDefault(x => x.Type == "name");
            if (claim == null)
                return null;    // no claim

            return claim.Value;
        } 

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return null;   // not authenticated

            var claim = principal.Claims.FirstOrDefault(x => x.Type == "userId");
            if (claim == null)
                return null;    // no 'userId' claim

            return claim.Value;
        } 
    }
}