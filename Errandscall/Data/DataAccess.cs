using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Errandscall.Data
{
    public class DataAccess
    {


        public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public SmtpClient smtpClient()
        {

            SmtpClient client = new SmtpClient();
            client.Host = "mcsvr.asantedinoko.com";
            client.EnableSsl = true;
            client.Port = 587;
            client.Credentials = new NetworkCredential("noreply@bimaconcepts.co.za", "Posseidonslayer86");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;


            return client;
        }

    }
}