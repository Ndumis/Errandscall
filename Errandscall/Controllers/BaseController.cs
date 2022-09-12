using Errandscall.Data;
using ErrandscallDatabase;
using Errandscall.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Errandscall.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected ErrandscallEntities db = new ErrandscallEntities();

        // GET: Base
        public SqlConnection Conn
        {
            get
            {
                string ConnectString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
                var con = new SqlConnection(ConnectString);

                con.Open();
                return con;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var value = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (value != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(value.Value);

                    string name = ticket.Name;
                    string userData = ticket.UserData;
                    var data = JsonConvert.DeserializeObject<Models.CustomSerializeModel>(userData);

                    if (data != null)
                    {
                        ViewBag.UserId = data.UserId;
                        ViewBag.Username = data.FirstName;
                        ViewBag.Role = data.RoleName;

                        //ShowSuccess("Welcome back " + data.FirstName + " !!!");
                    }
                }

            }
            catch
            {

            }
        }




        public int CookieId()
        {
            int id = 0;
            var value = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (value != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(value.Value);

                string name = ticket.Name;
                string userData = ticket.UserData;
                LoginDetails data = JsonConvert.DeserializeObject<LoginDetails>(userData);
                if (data != null)
                {
                    id = data.UserId;
                }
            }

            return id;
        }


        public int CalculateAge(DateTime Dob)
        {
            try
            {
                int Years = new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
                return Years;
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
                return 0;
            }

        }

        #region Alerts

        internal ActionResult ReturnJsonException(Exception ex)
        {
            return Json(new JSONReturn()
            {
                Data = null,
                Message = ex.Message,
                AlertType = AlertTypes.Danger
            });
        }

        internal void ShowException(Exception ex)
        {
            TempData["Alerts"] = new JSONReturn()
            {
                Data = null,
                Message = ex.Message,
                AlertType = AlertTypes.Danger
            };
        }

        internal void ShowException(string Message)
        {
            TempData["Alerts"] = new JSONReturn()
            {
                Data = null,
                Message = Message,
                AlertType = AlertTypes.Danger
            };
        }

        internal void ShowSuccess(string Message, object Data = null)
        {
            TempData["Alerts"] = new JSONReturn()
            {
                Data = Data,
                Message = Message,
                AlertType = AlertTypes.Success
            };
        }

        internal void ShowInfo(string Message, object Data = null)
        {
            TempData["Alerts"] = new JSONReturn()
            {
                Data = Data,
                Message = Message,
                AlertType = AlertTypes.Info
            };
        }


        internal ActionResult ShowJsonSuccess(string message, object Data = null)
        {
            return Json(new JSONReturn()
            {
                Data = Data,
                Message = message,
                AlertType = AlertTypes.Success
            });
        }
        #endregion

    }
}