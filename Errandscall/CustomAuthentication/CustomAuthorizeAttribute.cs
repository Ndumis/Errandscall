using Errandscall.CustomAuthentication;
using Errandscall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Errandscall.CustomAuthentication
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected virtual CustomPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (string.IsNullOrEmpty(Roles))
                return true;

            return CurrentUser != null ;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            RedirectToRouteResult routeData = null;
            string controller = string.Empty;
            string action = string.Empty;

            if (CurrentUser == null)
            {
                controller = "Login";
                action = "Index";
            }
            else
            {

                controller = "Error";
                action = "AccessDenied";
            }

            var rvd = new RouteValueDictionary(new { controller = controller, action = action });
            routeData = new RedirectToRouteResult(rvd);

            filterContext.Result = routeData;
        }
    }
}