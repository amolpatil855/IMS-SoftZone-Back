using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMvwAccessory
    {
        public string Category { get; set; }
        public string itemCode { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string uom { get; set; }
        public string hsnCode { get; set; }
        public string hsnWithGST { get; set; }
        public string gst { get; set; }
        public decimal sellingRate { get; set; }
        public Nullable<decimal> sellingRateWithGst { get; set; }
        public decimal purchaseRate { get; set; }
        public Nullable<decimal> purchaseRateWithGst { get; set; }
        public Nullable<decimal> availableStock { get; set; }
    }
}