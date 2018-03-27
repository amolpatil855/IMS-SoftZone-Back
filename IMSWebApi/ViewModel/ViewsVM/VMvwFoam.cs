using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMvwFoam
    {
        public string Collection { get; set; }
        public string qualityCode { get; set; }
        public string itemCode { get; set; }
        public Nullable<decimal> availableStock { get; set; }
    }
}