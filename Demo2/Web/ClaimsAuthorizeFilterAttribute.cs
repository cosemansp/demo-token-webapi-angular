using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using log4net;

namespace Demo2.Web
{
    public class ClaimsAuthorizeFilterAttribute : ActionFilterAttribute
    {
        private readonly string _type;
        private readonly string _value;
        private readonly ILog _log = LogManager.GetLogger(typeof(ClaimsAuthorizeFilterAttribute).Name);

        public ClaimsAuthorizeFilterAttribute(string type, string value)
        {
            _type = type;
            _value = value;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // make sure we are authenticated
            var principal = actionContext.Request.GetOwinContext().Request.User as ClaimsPrincipal;
            if (principal == null)
            {
                actionContext.Response = Unauthorized(actionContext, "Missing access token");
                _log.WarnFormat("'{0}' is Unauthorized", actionContext.Request.RequestUri.AbsoluteUri);
                return;
            }

            var claim = principal.Claims.FirstOrDefault(x => x.Type == _type && x.Value.Contains(_value));
            if (claim == null)
            {
                // claim not present for user
                actionContext.Response = Forbidden(actionContext);
                _log.WarnFormat("'{0}' is Forbidden for '{1}'", actionContext.Request.RequestUri.AbsoluteUri, principal.Identity.Name);
            }
        }

        private HttpResponseMessage Unauthorized(HttpActionContext actionContext, string message)
        {
            return actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
        }

        private HttpResponseMessage Forbidden(HttpActionContext actionContext)
        {
            return actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Insufficient access");
        }
    }
}