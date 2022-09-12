using Errandscall.CustomAuthentication;
using Errandscall.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Errandscall
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }


        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                bool isAjax = new HttpRequestWrapper(System.Web.HttpContext.Current.Request).IsAjaxRequest();
                try
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    authTicket = RefreshLoginCookie(isAjax);

                    var serializeModel = JsonConvert.DeserializeObject<CustomSerializeModel>(authTicket.UserData);

                    CustomPrincipal principal = new CustomPrincipal(authTicket.Name)
                    {
                        UserId = serializeModel.UserId,
                        FirstName = serializeModel.FirstName,
                        LastName = serializeModel.LastName,
                        Email = serializeModel.Email,
                        Roles = serializeModel.RoleName,
                    };

                    HttpContext.Current.User = principal;

                }
                catch (Exception)
                {
                    HttpContext.Current.User = null;
                    return;
                }


            }
            else
            {
                return;
            }
        }

        internal static FormsAuthenticationTicket RefreshLoginCookie(bool retainCurrentExpiry)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == null)
                return null;

            FormsAuthenticationTicket oldTicket = FormsAuthentication.Decrypt(authCookie.Value);

            DateTime expiryDate = (retainCurrentExpiry ? oldTicket.Expiration : DateTime.Now.Add(FormsAuthentication.Timeout));
            HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);

            var newTicket = new FormsAuthenticationTicket(oldTicket.Version, oldTicket.Name, oldTicket.IssueDate, expiryDate,
                oldTicket.IsPersistent, oldTicket.UserData, oldTicket.CookiePath);

            HttpCookie newAuthCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(newTicket));
            HttpContext.Current.Response.Cookies.Add(newAuthCookie);

            return newTicket;

        }



    }
}
