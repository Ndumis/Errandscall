using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Website.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(string name, string surname, string email, string subject, string message)
        {

            MailMessage mail = null;
            string preparedMessage = PreparedContactMessage(name, surname, email, string.Format("Errandscall Website: {0}", subject), message);
            mail = new MailMessage(email, "support@errandscall.co.za", //Send to info or enquiries
               subject, preparedMessage);
            mail.IsBodyHtml = true;


            mail.Priority = MailPriority.High;
            mail.BodyEncoding = Encoding.UTF8;
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;


            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);

            SmtpClient client = smtpClient();

            client.Send(mail);

            string html = "<p class='success'><strong>Success!</strong> Your message has been sent to us.</p>";
            return Content(html, "text/html");
        }

        private string PreparedContactMessage(string name, string surname, string email, string subject, string message)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Hi there");
            sb.AppendLine("<br />");
            sb.AppendLine("<br />");
            sb.AppendLine($"You have a new message from errandscall website, See details below:");
            sb.AppendLine("<br />");
            sb.AppendLine("<br />");
            sb.AppendLine(string.Format("From: {0} {1}", name, surname));
            sb.AppendLine("<br />");
            sb.AppendLine(string.Format("Email: {0}", email));
            sb.AppendLine("<br />");
            sb.AppendLine(string.Format("Subject: {0}", subject));
            sb.AppendLine("<br />");
            sb.AppendLine(string.Format("Message:"));
            sb.AppendLine("<br />");
            sb.AppendLine(message);
            sb.AppendLine("<br />");
            sb.AppendLine("<br />");
            sb.AppendLine("<b>Kind Regards,</b>");
            sb.AppendLine("<br />");
            sb.AppendLine($"<b>{ name + " " + surname}</b>");

            return
                sb.ToString();
        }

        public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public SmtpClient smtpClient()
        {
            //bool EnableSsl = false;

            //var SMTP_ENABLESSL = entities.Setting.FirstOrDefault(s => s.Name == "SMTP_ENABLESSL").Value;
            //if (SMTP_ENABLESSL != null)
            //{
            //    EnableSsl = true;
            //}            

            //SmtpClient client = new SmtpClient();
            //client.Host = entities.Setting.FirstOrDefault(s => s.Name == "SMTP_HOST").Value;
            //client.Host = "mcsvr.asantedinoko.com";
            //client.EnableSsl = EnableSsl;
            //client.Port = entities.Setting.FirstOrDefault(s => s.Name == "SMTP_PORT").Value.ToInt();
            //client.Port = 465;
            //client.Credentials = new NetworkCredential(entities.Setting.FirstOrDefault(s => s.Name == "SMTP_USERNAME").Value, entities.Setting.FirstOrDefault(s => s.Name == "SMTP_PASSWORD").Value);
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false; 


            SmtpClient client = new SmtpClient();
            client.Host = "mail.errandscall.co.za";
            client.EnableSsl = true;
            client.Port = 25;
            client.Credentials = new NetworkCredential("support@errandscall.co.za", "errandscall@2022");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;


            return client;
        }
    }
}