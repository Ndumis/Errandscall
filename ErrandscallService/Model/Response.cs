using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrandscallService.Model
{
    public class Response
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string InnerResponseMessage { get; set; }
        public string ErrorReference { get; set; }
    }
}
