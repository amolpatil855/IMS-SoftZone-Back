using IMSWebApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class ResponseMessage
    {
        public Int64 id { get; set; }
        public string message { get; set; }
        public string type { get; set; }
        public ResponseMessage(Int64 _id, string _message, ResponseType _type)
        {
            this.id = _id;
            this.message = _message;
            this.type = Convert.ToString(_type);
        }
    }
}