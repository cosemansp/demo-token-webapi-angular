using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using log4net;

namespace Demo2.Web
{
    public class RoleAuthorizeFilterAttribute : ActionFilterAttribute
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ClaimsAuthorizeFilterAttribute).Name);

        public string Deny { get; set; }
        public string Allow { get; set; }

        public RoleAuthorizeFilterAttribute()
        {
        }

        public RoleAuthorizeFilterAttribute(string allowRoles)
        {
            Allow = allowRoles;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // make sure we are authenticated
            var principal = actionContext.Request.GetOwinContext().Request.User as ClaimsPrincipal;
            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                actionContext.Response = Unauthorized(actionContext, "Missing access token");
                _log.WarnFormat("'{0}' is Unauthorized", actionContext.Request.RequestUri.AbsoluteUri);
                return;
            }

            if (!HasAccess(principal) && !principal.IsInRole("SysAdmin"))
            {
                // claim not present for user
                actionContext.Response = Forbidden(actionContext);
                _log.WarnFormat("'{0}' is Forbidden for '{1}'", actionContext.Request.RequestUri.AbsoluteUri, principal.Identity.Name);
            }
        }

        private bool HasAccess(ClaimsPrincipal principal)
        {
            if (!string.IsNullOrEmpty(Deny))
            {
                var denyRoles = Deny.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                if (principal.IsInRole(denyRoles))
                    return false;
            }
            if (!string.IsNullOrEmpty(Allow))
            {
                var allowRoles = Allow.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!principal.IsInRole(allowRoles))
                    return false;
            }

            return true;
        }

        private HttpResponseMessage Forbidden(HttpActionContext actionContext)
        {
            return actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Insufficient access");
        }

        private HttpResponseMessage Unauthorized(HttpActionContext actionContext, string message)
        {
            return actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
        }
    }
}