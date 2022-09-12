using Errandscall.Controllers;
using Errandscall.Models;
using ErrandscallDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Errandscall.Data
{
    public class Mail
    {
        public string To { get; set; }
        public string From { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string Celllphone { get; set; }
        [AllowHtml]
        public string EmailMessage { get; set; }
        public string SmsMessage { get; set; }

    }

    public class Message_Number
    {
        public string Mobile_Number { get; set; }
        public string SmS_Message { get; set; }
    }

    public enum MessageMode
    {
        Email = default(int),
        SMS
    }

    public class Messenger : BaseController
    {
        private Client Client { get; set; }
        private MessageMode Mode { get; set; }

        [Obsolete]
        public Messenger(Client client, MessageMode mode)
        {
            this.Client = client;
            this.Mode = mode;

        }

        public void SendMessage(string subject, out string pin)
        {
            pin = string.Empty;
            MailMessage mail = null;

            if (Mode == MessageMode.Email)
            {
                if (Client != null)
                {
                    string message = PreparedOtpMessage(out pin);
                    mail = new MailMessage("noreply@bimaconcepts.co.za", Client.Email,
                       subject, message);
                }
                else
                {

                }

                mail.Priority = MailPriority.High;
                mail.BodyEncoding = Encoding.UTF8;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

                DataAccess dataAccess = new DataAccess();

                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(dataAccess.ValidateServerCertificate);

                SmtpClient smtpClient = dataAccess.smtpClient();

                smtpClient.Send(mail);
                ShowSuccess("Email Sent Seccessfully ");
            }
            else if (Mode == MessageMode.SMS)
            {
                //SMS Logic Here
            }
        }


        private string PreparedOtpMessage(out string pin)
        {
            int newPin = GenerateOTP();
            pin = newPin.ToString();

            if (pin != string.Empty)
            {
                string pinn = pin;

                if (!pinn.IsNullOrEmpty())
                {
                    Encoder encoder = new Encoder();
                    pinn = encoder.Encode(pinn);
                }

                var exist = db.Login.FirstOrDefault(p => p.Password == pinn);
                if (exist != null)
                {
                    PreparedOtpMessage(out pin);
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.Append($"Hi {Client.Name}");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"Your Temporary pin is: {pin}.");
            sb.AppendLine();
            sb.AppendLine("If you did not register or request the password reset, your account is safe, please ignore.");
            sb.AppendLine();
            sb.AppendLine("Regards");
            sb.AppendLine("Errandscall");

            return
                sb.ToString();
        }

        private int GenerateOTP()
        {
            Random random = new Random();
            return
                random.Next(10000, 99999);
        }


    }
}
