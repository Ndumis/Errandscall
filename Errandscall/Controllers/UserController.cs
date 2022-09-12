using Errandscall.Data;
using ErrandscallDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Errandscall.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult User()
        {
            List<Client> clients = new List<Client>();
            var login = db.Login.Where(l => l.UserRoleId == 1 || l.UserRoleId == 2).ToList();
            foreach (var item in login)
            {
               var user =  db.Client.FirstOrDefault(c=>c.Id == item.ClientId);
                clients.Add(user);
            }
            
            return View(clients);
        }

        // GET: Profile
        public ActionResult Profile(int? Id)
        {
            if (Id != null && Id != 0)
            {

            }
            else
            {
                Id = CookieId();
            }
            Client client = db.Client.FirstOrDefault(c=>c.Id == Id);


            return View(client);
        }

        // GET: ClientManagement
        public ActionResult ClientManagement()
        {
            List<Client> clients = new List<Client>();
            var login = db.Login.Where(l => l.UserRoleId == 3).ToList();
            foreach (var item in login)
            {
                var user = db.Client.FirstOrDefault(c => c.Id == item.ClientId);
                clients.Add(user);
            }

            return View(clients);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }


        public ActionResult NewUser()
        {
            return View(new Client());
        }

        [HttpPost]
        public ActionResult NewUser(Client client)
        {
            try
            {
                // TODO: Add insert logic here
                client.CitizenId = 1;
                client.AddedOnDateTime = DateTime.Now;
                client.LastModifiedDateTime = DateTime.Now;

                db.Client.Add(client);
                if(db.SaveChanges() != 0)
                {
                    Login login = new Login();
                    login.ClientId = client.Id;
                    login.UserRoleId = 2; //System user

                    db.Login.Add(login);
                    db.SaveChanges();
                }

                return RedirectToAction("User");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult EditUser(int Id)
        {
            Client client = db.Client.FirstOrDefault(c => c.Id == Id);

            return View(client);
        }

        [HttpPost]
        public ActionResult EditUser(Client client)
        {
            try
            {
                // TODO: Add update logic here
                Client cli = db.Client.FirstOrDefault(c=>c.Id == client.Id);
                if (cli != null)
                {
                    cli.Initials = client.Initials;
                    cli.Name = client.Name;
                    cli.Surname = client.Surname;
                    cli.Email = client.Email;
                    cli.CellNumber = client.CellNumber;
                    cli.IdNo = client.IdNo;
                    cli.Dob = client.Dob;
                    cli.Address1 = client.Address1;
                    cli.Address2 = client.Address2;
                    cli.Address3 = client.Address3;
                    cli.Address4 = client.Address4;
                    cli.PostalCode = client.PostalCode;
                    cli.LastModifiedDateTime = DateTime.Now;
                    db.SaveChanges();
                }

                
                ShowSuccess("Saved Successfull");
                return RedirectToAction("Profile", new { @Id = client.Id});
            }
            catch
            {
                return View();
            }
        }


        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int DeleteId, string Comment)
        {
            try
            {
                // TODO: Add delete logic here


                ShowSuccess("Record was deleted successfully...");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }
    }
}
