using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCompanyLocation
    {
        public long id { get; set; }
        public string locationCode { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string pin { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string phone { get; set; }
    }
}