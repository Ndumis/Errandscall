using System;
using ErrandscallDatabase;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Errandscall.Models;

namespace Errandscall.Controllers
{
    public class SettingController : BaseController
    {
        // GET: Setting
        public ActionResult Index()
        {
            Lookup lookup = new Lookup();
            lookup.vehicleMakes = db.VehicleMake.ToList();
            lookup.Services = db.Services.ToList();
            lookup.documentTypes = db.DocumentType.ToList();

            return View(lookup);
        }

        [HttpGet]
        public ActionResult _VehicleType(int? Id)
        {
            VehicleModel vehicle = db.VehicleModel.FirstOrDefault(x => x.Id == Id);

            return PartialView(vehicle);
        }

        [HttpPost]
        public ActionResult _VehicleType(VehicleModel vehicle)
        {
            VehicleModel model = db.VehicleModel.FirstOrDefault(x => x.Id == vehicle.Id);
            if (model != null)
            {
                model.Description = vehicle.Description;
                db.SaveChanges();

                model.VehicleMake.Description = vehicle.VehicleMake.Description;
                db.SaveChanges();
            }
            else
            {
                VehicleMake vehicleMake = db.VehicleMake.FirstOrDefault(x=>x.Description == vehicle.VehicleMake.Description);
                if (vehicleMake == null)
                {
                    db.VehicleMake.Add(vehicle.VehicleMake);
                    db.SaveChanges();
                }
                //else already exist

                db.VehicleModel.Add(vehicle);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult _ServiceType(int? Id)
        {
            Services service = db.Services.FirstOrDefault(x => x.Id == Id);

            return PartialView(service);
        }

        [HttpPost]
        public ActionResult _ServiceType(Services services)
        {
            Services service = db.Services.FirstOrDefault(x => x.Id == services.Id);
            if (service != null)
            {
                service.Description = services.Description;
                db.SaveChanges();
            }
            else
            {
                db.Services.Add(services);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult _DocumentType(int? Id)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(x => x.Id == Id);
            return PartialView(documentType);
        }

        [HttpPost]
        public ActionResult _DocumentType(DocumentType documentType)
        {
            DocumentType document = db.DocumentType.FirstOrDefault(x => x.Id == documentType.Id);
            if (document != null)
            {
                document.Description = documentType.Description;
                db.SaveChanges();
            }
            else
            {
                db.DocumentType.Add(documentType);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}