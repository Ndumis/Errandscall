using Errandscall.Models;
using Errandscall.Data;
using ErrandscallDatabase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Errandscall.Controllers
{
    public class LoginController : BaseController
    {
        // GET: Login

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(new LoginDetails());
        }

        // GET: Login/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new Client());
        }

        // GET: Login/ForgotPassword

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginDetails loginDetails, string ReturnUrl)
        {
            try
            {
                // TODO: Add delete logic here

                if (loginDetails.RememberMe == null)
                    loginDetails.RememberMe = false;

                string userData = string.Empty;
                string enTick = string.Empty;

                if (!loginDetails.Password.IsNullOrEmpty())
                {
                    Encoder encoder = new Encoder();
                    loginDetails.Password = encoder.Encode(loginDetails.Password);
                }



                if (!loginDetails.Username.IsNullOrEmpty() && !loginDetails.Password.IsNullOrEmpty())
                {
                    //The username from client table
                    Client client = db.Client.FirstOrDefault(c => c.Email == loginDetails.Username || c.IdNo == loginDetails.Username);
                    if (client != null)
                    {

                        Login log = db.Login.Where(e => e.Password == loginDetails.Password || e.OTP == loginDetails.Password).FirstOrDefault();

                        if (log != null)
                        {
                            //Check if otp expired
                            if (log.OTP != null && log.OTP != string.Empty)
                            {
                                var hours = (DateTime.Now - (DateTime)log.RequestDateTime).TotalHours;
                                if (hours > 1)
                                {
                                    ShowException("Your OTP has expired. Please request a new OPT. * Click Forgot Password *");
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    return RedirectToAction("NewPassword", new { @Id = log.ClientId, @username = loginDetails.Username });
                                }
                            }

                            var active = db.Client.FirstOrDefault(e => e.Id == log.ClientId && e.Active == false);
                            if (active != null)
                            {
                                ShowException("User is not active!!!");
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                //if (log.Otp == log.Password)
                                //{
                                //    log.Otp = null;
                                //}

                                log.LastLogin = DateTime.Now;
                                db.SaveChanges();

                                Client cli = db.Client.FirstOrDefault(e => e.Id == log.ClientId);
                                if (cli != null)
                                {
                                    CustomSerializeModel login = new Models.CustomSerializeModel()
                                    {
                                        UserId = cli.Id,
                                        FirstName = cli.Name,
                                        LastName = cli.Surname,
                                        RoleName = log.UserRole.Description,
                                        Initials = cli.Initials,
                                        Email = cli.Email,

                                    };

                                    //cli.OnlineStatus = true;
                                    db.SaveChanges();

                                    userData = JsonConvert.SerializeObject(login);

                                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                                    (1,
                                        cli.Email,
                                        DateTime.Now,
                                        DateTime.Now.Add(FormsAuthentication.Timeout),
                                        loginDetails.RememberMe.ToBool(),
                                        userData
                                    );

                                    enTick = FormsAuthentication.Encrypt(ticket);
                                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, enTick);
                                    Response.Cookies.Add(cookie);

                                    if (ReturnUrl != null)
                                    {
                                        string[] textSplit = ReturnUrl.Split('/');
                                        if (textSplit.Count() >= 4)
                                        {
                                            if (ReturnUrl.Contains("_"))
                                            {
                                                ShowInfo("Your last data was not processed successfully. Please check and re-submit");
                                                return RedirectToAction("Index", "Home");
                                            }
                                            else if (ReturnUrl.Contains("Claim"))
                                            {
                                                return RedirectToAction("Index", "Home");
                                            }
                                            else
                                                return RedirectToAction("Index", "Home");
                                        }
                                        else
                                        {
                                            if (ReturnUrl.Contains("_"))
                                            {
                                                ShowInfo("Your last data was not processed successfully. Please check and re-submit");
                                                return RedirectToAction("Index", "Home");
                                            }
                                            return Redirect(ReturnUrl);
                                        }
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "Home");
                                    }
                                }
                            }

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ShowException("Username and/or password is incorrect.");
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ShowException("Username and/or password is incorrect.");
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ShowException("Username and/or password is missing.");
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                ShowException(ex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Obsolete]
        public ActionResult ForgotPassword(LoginDetails loginDetails, string Otp)
        {
            try
            {
                // TODO: Add delete logic herestring pin = string.Empty;
                string pin = string.Empty;
                Client client; ;
                if (Otp != null && Otp != string.Empty)
                {
                    switch (Otp)
                    {
                        case "EmailAddress":
                            client = db.Client.FirstOrDefault(e => e.Email == loginDetails.EmailAddress);
                            if (client != null)
                            {
                                new Messenger(client, MessageMode.Email).SendMessage("New Account Registration Confirmation", out pin);

                                Logpin(client, pin);

                                ShowSuccess("Temp pin was sent successfully to your email " + client.Email);
                            }
                            else
                            {
                                ShowException("Email does not exist");
                            }
                            break;
                        case "CellphoneNumber":
                            client = db.Client.FirstOrDefault(e => e.CellNumber == loginDetails.SMS);
                            if (client != null)
                            {
                                new Messenger(client, MessageMode.SMS).SendMessage("New Account Registration Confirmation", out pin);

                                Logpin(client, pin);

                                ShowSuccess("Temp pin was sent successfully to your number " + client.CellNumber);
                            }
                            else
                            {
                                ShowException("Cell number does not exist");
                            }
                            break;
                            //var login = db.Login.FirstOrDefault(l => l.Email == loginDetails.Email);
                    }
                }
                else
                {
                    ShowInfo("Please make sure to select the type of option to receive OTP");

                    return View();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ErrandscallDatabase.Login Logpin(Client client, string pin)
        {
            var login = db.Login.FirstOrDefault(l => l.ClientId == client.Id);
            if (login != null)
            {
                if (!pin.IsNullOrEmpty())
                {
                    Encoder encoder = new Encoder();
                    pin = encoder.Encode(pin);
                }

                login.OTP = pin;
                login.RequestDateTime = DateTime.Now;
                db.SaveChanges();

            }

            return login;
        }


        public ActionResult LogOff()
        {
            try
            {

                HttpCookie cookie = new HttpCookie("ErrandscallsCookie", "")
                {
                    Expires = DateTime.Now.AddYears(-1)
                };
                Response.Cookies.Add(cookie);

                FormsAuthentication.SignOut();
                System.Web.HttpContext.Current.User = null;

                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                ShowException(ex);
                return RedirectToAction("Index", "Login");
            }

        }


        [HttpPost]
        public ActionResult Register(Client client, string Citizen, string ConfrimEmail, List<HttpPostedFileBase> IdDocuments)
        {
            try
            {
                // TODO: Add delete logic here     
                if (Citizen == "IdNumber")
                {
                    client.CitizenId = 1;
                }
                else
                {
                    client.CitizenId = 2;
                }
                client.AddedOnDateTime = DateTime.Now;
                client.LastModifiedDateTime = DateTime.Now;

                db.Client.Add(client);
                if (db.SaveChanges() != 0)
                {
                    Login login = new Login();
                    login.ClientId = client.Id;
                    login.UserRoleId = 3; //Client user

                    db.Login.Add(login);
                    db.SaveChanges();
                }


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult NewPassword(int Id, string username)
        {
            Client login = db.Client.FirstOrDefault(c => c.Id == Id);
            LoginDetails loginDetails = new LoginDetails();
            loginDetails.UserId = login.Id;
            loginDetails.Username = username;
            return View(loginDetails);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult NewPassword(LoginDetails loginDetails)
        {
            try
            {
                // TODO: Add delete logic here
                if (loginDetails.Password == loginDetails.ConfirmPassword)
                {

                    loginDetails.RememberMe = true;

                    string userData = string.Empty;
                    string enTick = string.Empty;

                    if (!loginDetails.Password.IsNullOrEmpty())
                    {
                        Encoder encoder = new Encoder();
                        loginDetails.Password = encoder.Encode(loginDetails.Password);
                    }

                    if (!loginDetails.Username.IsNullOrEmpty() && !loginDetails.Password.IsNullOrEmpty())
                    {
                        //The username from client table

                        Login log = db.Login.Where(e => e.ClientId == loginDetails.UserId).FirstOrDefault();

                        if (log != null)
                        {
                            //UPdate password
                            log.OTP = null;
                            log.Password = loginDetails.Password;
                            log.LastLogin = DateTime.Now;
                            db.SaveChanges();

                            var active = db.Client.FirstOrDefault(e => e.Id == log.ClientId && e.Active == false);
                            if (active != null)
                            {
                                ShowException("User is not active!!!");
                                return RedirectToAction("Index");
                            }
                            else
                            {

                                Client cli = db.Client.FirstOrDefault(e => e.Id == log.ClientId);
                                if (cli != null)
                                {
                                    CustomSerializeModel login = new Models.CustomSerializeModel()
                                    {
                                        UserId = cli.Id,
                                        FirstName = cli.Name,
                                        LastName = cli.Surname,
                                        RoleName = log.UserRole.Description,
                                        Initials = cli.Initials,
                                        Email = cli.Email,

                                    };

                                    //cli.OnlineStatus = true;
                                    //db.SaveChanges();

                                    userData = JsonConvert.SerializeObject(login);

                                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                                    (1,
                                        cli.Email,
                                        DateTime.Now,
                                        DateTime.Now.Add(FormsAuthentication.Timeout),
                                        loginDetails.RememberMe.ToBool(),
                                        userData
                                    );

                                    enTick = FormsAuthentication.Encrypt(ticket);
                                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, enTick);
                                    Response.Cookies.Add(cookie);

                                    return RedirectToAction("Index", "Home");

                                }
                            }

                            return View(loginDetails);
                        }
                        else
                        {
                            ShowException("Username and/or password is incorrect.");
                            return View(loginDetails);
                        }
                    }
                    else
                    {
                        ShowException("Username and/or password is missing.");
                        return View(loginDetails);
                    }
                }
                else
                {
                    ShowException("Username and/or password is missing.");
                    return View(loginDetails);
                }

            }
            catch (Exception ex)
            {
                ShowException(ex);
                return View(loginDetails);
            }
        }


    }
}
