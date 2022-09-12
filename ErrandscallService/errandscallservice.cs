using ErrandscallDatabase;
using System;
using System.ComponentModel;
using System.ServiceProcess;
using ErrandscallService.Data;
using System.Configuration;
using System.Text;
using System.Net.Http;
using ErrandscallService.Model;

namespace ErrandscallService
{
    [RunInstaller(true)]
    public partial class ErrandscallService : ServiceBase
    {
        protected ErrandscallEntities db = new ErrandscallEntities();

        Logger logger = null;
        System.Timers.Timer timer = null;
        private const string URL = "http://bima-api.asante-intranet.co.za/api/Notification/";

        public ErrandscallService()
        {
            InitializeComponent();
        }

        public void RunDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            logger = new Logger();
            ServiceMessage serviceMessage = new ServiceMessage();
            serviceMessage.Method = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            try
            {

                var birtdayTimer = ConfigurationManager.AppSettings["NotificatonTimer"];
                timer = new System.Timers.Timer();
                timer.Interval = double.Parse(birtdayTimer);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(this.Notificaton);
                timer.Enabled = true;

                serviceMessage.Message = "Message: The service has started successfully";
                logger.Log(serviceMessage);
            }
            catch (Exception Ex)
            {
                serviceMessage.Message = Ex.Message.ToString();
                logger.Log(serviceMessage);
                this.Stop();
            }
        }

        protected override void OnStop()
        {
            logger = new Logger();
            ServiceMessage serviceMessage = new ServiceMessage();
            serviceMessage.Method = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                serviceMessage.Message = "The service has stopped successfully";
                logger.Log(serviceMessage);

                // Add code here to perform any tear-down necessary to stop your service.
                timer.Enabled = false;
                timer.Dispose();
                timer = null;
            }
            catch (Exception Ex)
            {
                serviceMessage.Message = Ex.Message.ToString();
                logger.Log(serviceMessage);
                this.Stop();
            }
        }


        private void Notificaton(object sender, EventArgs e)
        {
            logger = new Logger();
            ServiceMessage serviceMessage = new ServiceMessage();
            serviceMessage.Method = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new System.Uri(URL);
                byte[] cred = UTF8Encoding.UTF8.GetBytes("errandscall:errandscall@[2013]!");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //System.Net.Http.HttpContent content = new StringContent(UTF8Encoding.UTF8, "application/json");

                Response response = new Response();
                HttpResponseMessage messge = client.GetAsync(URL).Result;
                if (messge.IsSuccessStatusCode)
                {
                    response.ResponseCode = (int)messge.StatusCode;
                    response.ResponseMessage = messge.Content.ReadAsStringAsync().Result;                    
                }
                else
                {
                    response.ResponseCode = (int)messge.StatusCode;
                    response.ResponseMessage = messge.Content.ReadAsStringAsync().Result;

                    //Send email
                }

                serviceMessage.Message = response.ResponseMessage;
                logger.Log(serviceMessage);
            }
            catch (Exception Ex)
            {
                serviceMessage.Message = Ex.Message.ToString();
                logger.Log(serviceMessage);
                this.Stop();
            }

        }

    }
}
