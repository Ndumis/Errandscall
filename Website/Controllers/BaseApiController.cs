using ErrandscallDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Website.Controllers
{
    public class BaseApiController : ApiController
    {
        protected ErrandscallEntities db = new ErrandscallEntities();


    }
}
