using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMSupplierAddressDetail
    {
        public long id { get; set; }
        public long supplierId { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string steate { get; set; }
        public string pin { get; set; }
    }
}