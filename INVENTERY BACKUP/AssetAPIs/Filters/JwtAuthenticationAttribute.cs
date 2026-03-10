using AssetAPIs.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace AssetAPIs.Filters
{
    public class JwtAuthenticationAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                var authHeader = actionContext.Request.Headers.Authorization;

                if (authHeader == null || authHeader.Scheme != "Bearer")
                {
                    return false;
                }

                var token = authHeader.Parameter;
                var principal = JwtHelper.ValidateToken(token);

                if (principal == null)
                {
                    return false;
                }

                // Set the current principal
                actionContext.RequestContext.Principal = principal;
                System.Threading.Thread.CurrentPrincipal = principal;

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
            {
                Message = "Unauthorized: Invalid or expired token"
            });
        }
    }
}