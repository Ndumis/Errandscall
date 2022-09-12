using Errandscall.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Errandscall.Controllers
{
    public class NewsController : Controller
    {
        // GET: NewsFeed
        public ActionResult NewsFeed()
        {
            return View();
        }
    }
}