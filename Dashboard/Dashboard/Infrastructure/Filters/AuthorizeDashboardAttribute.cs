using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Dashboard.Infrastructure.Filters
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        private const string AuthFailedReason = "authFailedReason";

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new  ArgumentNullException(nameof(actionContext));

            var principal = actionContext.ControllerContext.RequestContext.Principal;
            
            if (principal?.Identity != null && principal.Identity.IsAuthenticated)
            {
                if (base.IsAuthorized(actionContext))
                {
                    return true;
                }

                actionContext.ActionArguments.Add(AuthFailedReason, HttpStatusCode.Forbidden);
            }
            else
            {
                actionContext.ActionArguments.Add(AuthFailedReason, HttpStatusCode.Unauthorized);
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException(nameof(actionContext));

            object responseCode;

            if (actionContext.ActionArguments.TryGetValue(AuthFailedReason, out responseCode))
            {
                actionContext.Response =
                    actionContext.ControllerContext.Request.CreateErrorResponse((HttpStatusCode)responseCode,
                        "Request forbidden");
            }
            else
            {
                actionContext.Response =
                    actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                        "Request not authorized");
            }
        }
    }
}
