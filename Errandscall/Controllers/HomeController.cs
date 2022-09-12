using Errandscall.Data;
using Errandscall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static Errandscall.Models.Helper;

namespace Errandscall.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.DiscRenewal = db.Request.Where(x => x.ServiceId == 1).Count();
            ViewBag.VehicleReg = db.Request.Where(x => x.ServiceId == 2).Count();
            ViewBag.ChangeOfOwnership = db.Request.Where(x => x.ServiceId == 3).Count();
            ViewBag.Roadworthy = db.Request.Where(x => x.ServiceId == 4).Count();

            return View();
        }

        [HttpPost]
        public ActionResult pieChart()
        {
            List<PieChart> pieChart = new List<PieChart>();

            var DisCount = db.Request.Where(x => x.ServiceId == 1);
            if (DisCount != null)
            {
                PieChart pie = new PieChart();
                pie.labels = "License Dis Renewal";
                pie.data = DisCount.Count();

                pieChart.Add(pie);
            }
            else
            {
                PieChart pie = new PieChart();
                pie.labels = "License Dis Renewal";
                pie.data = 0;

                pieChart.Add(pie);
            }

            var RegsCount = db.Request.Where(x => x.ServiceId == 2);
            if (RegsCount != null)
            {
                PieChart pie = new PieChart();
                pie.labels = "Vehicle Registration";
                pie.data = RegsCount.Count();

                pieChart.Add(pie);
            }
            else
            {
                PieChart pie = new PieChart();
                pie.labels = "Vehicle Registration";
                pie.data = 0;

                pieChart.Add(pie);
            }

            var OwnershipsCount = db.Request.Where(x => x.ServiceId == 3);
            if (OwnershipsCount != null)
            {
                PieChart pie = new PieChart();
                pie.labels = "Change Of Ownership";
                pie.data = OwnershipsCount.Count();

                pieChart.Add(pie);
            }
            else
            {
                PieChart pie = new PieChart();
                pie.labels = "Change Of Ownership";
                pie.data = 0;

                pieChart.Add(pie);
            }

            var RoadworthysCount = db.Request.Where(x => x.ServiceId == 4);
            if (RoadworthysCount != null)
            {
                PieChart pie = new PieChart();
                pie.labels = "Roadworthy";
                pie.data = RoadworthysCount.Count();

                pieChart.Add(pie);
            }
            else
            {
                PieChart pie = new PieChart();
                pie.labels = "Roadworthy";
                pie.data = 0;

                pieChart.Add(pie);
            }



            var json = new JavaScriptSerializer().Serialize(pieChart);

            return Json(json);
        }
    }
}