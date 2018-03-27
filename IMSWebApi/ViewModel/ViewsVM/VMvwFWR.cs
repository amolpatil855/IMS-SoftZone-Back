using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMvwFWR
    {
        public string Collection { get; set; }
        public string QDS { get; set; }
        public int serialNumber { get; set; }
        public Nullable<decimal> availableStock { get; set; }
    }
}