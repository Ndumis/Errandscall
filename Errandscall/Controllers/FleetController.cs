using Errandscall.Data;
using ErrandscallDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Errandscall.Controllers
{
    public class FleetController : BaseController
    {
        // GET: VehicleManagement
        public ActionResult VehicleManagement()
        {
            List<Vehicle> vehicles = db.Vehicle.ToList();
            return View(vehicles);
        }

        // GET: Request
        public ActionResult Request()
        {
            List<Request> requests = db.Request.ToList();
            return View(requests);
        }

        // GET: _NewVehicle
        [HttpGet]
        public ActionResult _NewVehicle(int? Id)
        {
            Vehicle vehicle = new Vehicle();
            if (Id != null && Id != 0)
            {
                vehicle = db.Vehicle.FirstOrDefault(v => v.Id == Id);

                ViewBag.VehicleMake = new SelectList(db.VehicleMake, "Id", "Description", vehicle.MakeId);
                ViewBag.VehicleModel = new SelectList(db.VehicleModel, "Id", "Description", vehicle.ModelId);
            }
            else
            {
                ViewBag.VehicleMake = new SelectList(db.VehicleMake, "Id", "Description");
                ViewBag.VehicleModel = new SelectList(db.VehicleModel, "Id", "Description");
            }

            return PartialView(vehicle);
        }

        public ActionResult ViewVehicle(int Id)
        {
            Vehicle vehicle = db.Vehicle.FirstOrDefault(v => v.Id == Id);

            return View(vehicle);
        }

        [HttpGet]
        public ActionResult _VehicleDocumet(int? Id, int vId)
        {
            VehicleDocument vehicleDocument = db.VehicleDocument.FirstOrDefault(v => v.VehicleId == Id);
            if (vehicleDocument != null)
            {
                ViewBag.DocumentType = new SelectList(db.DocumentType, "Id", "Description", vehicleDocument.TypeId);
            }
            else
            {
                vehicleDocument = new VehicleDocument();
                vehicleDocument.VehicleId = vId;
                ViewBag.DocumentType = new SelectList(db.DocumentType, "Id", "Description");
            }

            return PartialView(vehicleDocument);
        }

        [HttpPost]
        public ActionResult _VehicleDocumet(VehicleDocument vehicleDocument, HttpPostedFileBase VehicleDoc)
        {
            VehicleDocument rDocument = vehicleDocumentDoc(vehicleDocument, VehicleDoc);

            VehicleDocument document = db.VehicleDocument.FirstOrDefault(r => r.Id == vehicleDocument.Id);
            if (document != null)
            {
                document.document_name = rDocument.document_name;
                document.document_mime = rDocument.document_mime;
                document.document_data = rDocument.document_data;
                document.document_index = rDocument.document_index;
                document.TypeId = rDocument.TypeId;
                document.VehicleId = rDocument.VehicleId;
                document.LastModifiedDateTime = DateTime.Now;
                db.SaveChanges();
                ShowSuccess("Document was saved successfully");
            }
            else
            {
                rDocument.AddedOnDateTime = DateTime.Now;
                rDocument.LastModifiedDateTime = DateTime.Now;
                db.VehicleDocument.Add(rDocument);
                db.SaveChanges();
                ShowSuccess("Document was saved successfully");
            }

            return RedirectToAction("ViewVehicle", new { Id = vehicleDocument.VehicleId });
        }

        public VehicleDocument vehicleDocumentDoc(VehicleDocument request, HttpPostedFileBase VehicleDoc)
        {
            VehicleDocument vehicleDocument = new VehicleDocument();
            using (BinaryReader b = new BinaryReader(VehicleDoc.InputStream))
            {
                byte[] binData = b.ReadBytes(VehicleDoc.ContentLength);

                vehicleDocument.document_name = VehicleDoc.FileName;
                vehicleDocument.document_mime = VehicleDoc.ContentType;
                vehicleDocument.document_data = binData;
                vehicleDocument.document_index = 1;
                vehicleDocument.TypeId = request.TypeId;
                vehicleDocument.VehicleId = request.VehicleId;
            }

            return vehicleDocument;
        }

        // GET: Fleet/Create
        public ActionResult _Request(int VId)
        {
            var vehicle = db.Request.Where(v => v.VehicleId == VId && v.StatusId == 1).Select(s => s.ServiceId).ToList();
            Request request = new Request();

            request.Vehicle = db.Vehicle.FirstOrDefault(v => v.Id == VId);

            ViewBag.Service = new SelectList(db.Services.Where(s => !vehicle.Contains(s.Id)), "Id", "Description");

            return PartialView(request);
        }

        public ActionResult _EditRequest(int Id)
        {
            Request request = db.Request.FirstOrDefault(v => v.Id == Id);


            ViewBag.Service = new SelectList(db.Services, "Id", "Description", request.ServiceId);

            return PartialView(request);
        }

        public ActionResult ViewRequest(int Id)
        {
            Request request = db.Request.FirstOrDefault(v => v.Id == Id);

            return View(request);
        }

        [HttpGet]
        public ActionResult _RequestMessage(int Id)
        {

            Request request = db.Request.FirstOrDefault(r=>r.Id == Id);
            Message message = db.Message.FirstOrDefault(m=>m.RequestId == Id);
            if (message != null)
            {

            }
            else
            {
                message = new Message();
                message.Request = request;
                message.RequestId = request.Id;
                message.Client = request.Client;
            }

            return PartialView(message);
        }

        [HttpPost]
        public ActionResult _RequestMessage(Message message)
        {
            Request request = db.Request.FirstOrDefault(r => r.Id == message.RequestId);
            Message msg = new Message();
            msg.ClientId = CookieId();
            msg.RequestId = request.Id;
            msg.Message1 = message.Message1;
            msg.IsRead = false;
            msg.AddedOnDateTime = DateTime.Now;
            msg.LastModifiedDateTime = DateTime.Now;

            db.Message.Add(msg);
            db.SaveChanges();

            //TODO
            //Send notification to admin
            //

            ShowSuccess("Your message was sent successfully...");
            return RedirectToAction("ViewRequest", new { Id = message.RequestId });
        }

        [HttpGet]
        public ActionResult _RequestDocumet(int? Id, int rId)
        {
            RequestDocument requestDocument = db.RequestDocument.FirstOrDefault(v => v.RequestId == Id);
            if (requestDocument != null)
            {
                ViewBag.DocumentType = new SelectList(db.DocumentType, "Id", "Description", requestDocument.TypeId);
            }
            else
            {
                requestDocument = new RequestDocument();
                requestDocument.RequestId = rId;
                ViewBag.DocumentType = new SelectList(db.DocumentType, "Id", "Description");
            }

            return PartialView(requestDocument);
        }

        [HttpPost]
        public ActionResult _RequestDocumet(RequestDocument requestDocument, HttpPostedFileBase requestDoc)
        {
            RequestDocument rDocument = requestDocumentDoc(requestDocument, requestDoc);

            RequestDocument document = db.RequestDocument.FirstOrDefault(r => r.Id == requestDocument.Id);

            if (document != null)
            {
                document.document_name = rDocument.document_name;
                document.document_mime = rDocument.document_mime;
                document.document_data = rDocument.document_data;
                document.document_index = rDocument.document_index;
                document.TypeId = rDocument.TypeId;
                document.RequestId = rDocument.RequestId;
                document.LastModifiedDateTime = DateTime.Now;
                db.SaveChanges();
                ShowSuccess("Document was saved successfully");
            }
            else
            {
                rDocument.AddedOnDateTime = DateTime.Now;
                rDocument.LastModifiedDateTime = DateTime.Now;
                db.RequestDocument.Add(rDocument);
                db.SaveChanges();
                ShowSuccess("Document was saved successfully");
            }

            return RedirectToAction("ViewRequest", new { Id = requestDocument.RequestId });
        }

        public RequestDocument requestDocumentDoc(RequestDocument request, HttpPostedFileBase requestDoc)
        {
            RequestDocument requestDocument = new RequestDocument();
            using (BinaryReader b = new BinaryReader(requestDoc.InputStream))
            {
                byte[] binData = b.ReadBytes(requestDoc.ContentLength);

                requestDocument.document_name = requestDoc.FileName;
                requestDocument.document_mime = requestDoc.ContentType;
                requestDocument.document_data = binData;
                requestDocument.document_index = 1;
                requestDocument.TypeId = request.TypeId;
                requestDocument.RequestId = request.RequestId;
            }

            return requestDocument;
        }

        [HttpPost]
        public ActionResult _Request(Request request, string Comment)
        {
            var req = db.Request.FirstOrDefault(r => r.Id == request.Id);
            if (req != null)
            {
                //req.ClientId = CookieId();
                req.ServiceId = request.ServiceId;
                req.LastModifiedDateTime = DateTime.Now;
                db.SaveChanges();
            }
            else
            {
                request.Vehicle = null;
                request.ClientId = CookieId();
                request.StatusId = 1;
                request.AddedOnDateTime = DateTime.Now;
                request.LastModifiedDateTime = DateTime.Now;
                db.Request.Add(request);
                db.SaveChanges();
            }

            ShowSuccess("Saved Successfully");
            return RedirectToAction("Request");
        }


        // POST: Fleet/Create
        [HttpPost]
        public ActionResult _NewVehicle(Vehicle vehicle)
        {
            var veh = db.Vehicle.FirstOrDefault(v => v.Id == vehicle.Id);
            if (veh != null)
            {
                veh.VIN = vehicle.VIN;
                veh.Reg = vehicle.Reg;
                veh.DayOfExpire = vehicle.DayOfExpire;
                veh.MakeId = vehicle.MakeId;
                veh.ModelId = vehicle.ModelId;
                veh.LastModifiedDateTime = DateTime.Now;
                db.SaveChanges();
            }
            else
            {
                // TODO: Add insert logic here
                vehicle.ClientId = CookieId();
                vehicle.AddedOnDateTime = DateTime.Now;
                vehicle.LastModifiedDateTime = DateTime.Now;

                db.Vehicle.Add(vehicle);
                db.SaveChanges();
            }

            ShowSuccess("Saved Successfully");

            return RedirectToAction("VehicleManagement");
        }


        [HttpPost]
        public ActionResult VehicleModel(int id)
        {
            SelectList html = null;

            var vehicle = db.VehicleModel.FirstOrDefault(p => p.MakeId == id);
            if (vehicle != null)
            {
                html = new SelectList(db.VehicleModel.Where(n => n.MakeId == id).ToList(), "Id", "Description");
            }

            return Json(html, JsonRequestBehavior.AllowGet);
        }

        /// POST: User/Delete/5
        [HttpPost]
        public ActionResult DeleteVehicleId(int DeleteId, string Comment)
        {
            try
            {
                // TODO: Add delete logic here


                ShowSuccess("Record was deleted successfully...");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        /// POST: User/Delete/5
        [HttpPost]
        public ActionResult DeleteRequest(int DeleteRequestId, string Comment)
        {
            try
            {
                // TODO: Add delete logic here


                ShowSuccess("Record was deleted successfully...");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

    }
}
