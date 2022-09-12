using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Errandscall.Controllers
{
    public class AuditController : Controller
    {
        // GET: Audit
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ActivityLog()
        {
            return View();
        }
    }
}