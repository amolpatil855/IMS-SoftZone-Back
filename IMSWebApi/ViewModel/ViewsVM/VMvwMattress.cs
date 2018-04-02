using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMvwMattress
    {
        public string Category { get; set; }
        public string Collection { get; set; }
        public string qualityCode { get; set; }
        public string thicknessCode { get; set; }
        public string sizeCode { get; set; }
        public string uom { get; set; }
        public string hsnCode { get; set; }
        public string gst { get; set; }
        public decimal rate { get; set; }
        public Nullable<decimal> rateWithGst { get; set; }
        public decimal purchaseRate { get; set; }
        public Nullable<decimal> purchaseRateWithGst { get; set; }
        public Nullable<decimal> customRatePerSqFeet { get; set; }
        public Nullable<decimal> availableStock { get; set; }
        public string hsnWithGST { get; set; }
    }
}