using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCustomerAddressDetail
    {
        public long id { get; set; }
        public long customerId { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string pin { get; set; }
        public long createdBy { get; set; }
        public Nullable<long> updatedBy { get; set; }
    }
}