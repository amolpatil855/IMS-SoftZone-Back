using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMvwAccessory
    {
        public string itemCode { get; set; }
        public string name { get; set; }
        public Nullable<decimal> availableStock { get; set; }
    }
}