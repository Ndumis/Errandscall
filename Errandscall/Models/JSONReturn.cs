using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Errandscall.Models
{
    public class JSONReturn
    {
        public string Message { get; set; }
        public object Data { get; set; }
        public string AlertType { get; set; }
    }
}