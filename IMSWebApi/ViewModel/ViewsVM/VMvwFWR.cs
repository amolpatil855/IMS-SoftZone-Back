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
        public string cutRateWithGst { get; set; }
        public Nullable<decimal> roleRate { get; set; }
        public string rollRateWithGst { get; set; }
        public Nullable<decimal> rrp { get; set; }
        public string rrpWithGst { get; set; }
        public Nullable<decimal> flatRate { get; set; }
        public string flatRateWithGst { get; set; }
        public Nullable<decimal> purchaseFlatRate { get; set; }
        public string purchaseFlatRateWithGst { get; set; }
        public Nullable<decimal> availableStock { get; set; }
        public string hsnWithGST { get; set; }
    }
}