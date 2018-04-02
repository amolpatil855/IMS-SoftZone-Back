using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMvwFWR
    {
        public string Category { get; set; }
        public string Collection { get; set; }
        public string QDS { get; set; }
        public int serialNumber { get; set; }
        public string uom { get; set; }
        public string hsnCode { get; set; }
        public string gst { get; set; }
        public Nullable<decimal> width { get; set; }
        public Nullable<decimal> size { get; set; }
        public Nullable<decimal> cutRate { get; set; }
        public Nullable<decimal> cutRateWithGst { get; set; }
        public Nullable<decimal> roleRate { get; set; }
        public Nullable<decimal> rollRateWithGst { get; set; }
        public Nullable<decimal> rrp { get; set; }
        public Nullable<decimal> rrpWithGst { get; set; }
        public Nullable<decimal> flatRate { get; set; }
        public Nullable<decimal> flatRateWithGst { get; set; }
        public Nullable<decimal> purchaseFlatRate { get; set; }
        public Nullable<decimal> purchaseFlatRateWithGst { get; set; }
        public Nullable<decimal> availableStock { get; set; }
        public string hsnWithGST { get; set; }
    }
}