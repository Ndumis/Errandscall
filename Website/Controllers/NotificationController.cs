using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Website.Controllers
{
    public class NotificationController : BaseApiController
    {

        [HttpGet]
        public HttpResponseMessage Get()
        {
            string username = Thread.CurrentPrincipal.Identity.Name;

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
