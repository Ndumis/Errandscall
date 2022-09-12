using ErrandscallDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Errandscall.Models
{
    public class Helper
    {
        public class PieChart
        {
            public string labels { get; set; }
            public int data { get; set; }
        }

    }

    public class Lookup
    {
        public List<VehicleMake> vehicleMakes = new List<VehicleMake>();
        public List<Services> Services = new List<Services>();
        public List<DocumentType> documentTypes = new List<DocumentType>();
    }
}