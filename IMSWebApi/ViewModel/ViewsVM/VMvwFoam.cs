using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMvwFoam
    {
        public string Category { get; set; }
        public string Collection { get; set; }
        public long collectionId { get; set; }
        public string qualityCode { get; set; }
        public long qualityId { get; set; }
        public string density { get; set; }
        public long desityId { get; set; }
        public string sizeCode { get; set; }
        public long sizeId { get; set; }
        public string itemCode { get; set; }
        public string uom { get; set; }
        public string hsnCode { get; set; }
        public string gst { get; set; }
        public decimal purchaseRatePerMM { get; set; }
        public Nullable<decimal> purchaseRatePerMMWithGst { get; set; }
        public decimal purchaseRatePerKG { get; set; }
        public Nullable<decimal> purchaseRatePerKGWithGst { get; set; }
        public decimal sellingRatePerMM { get; set; }
        public Nullable<decimal> sellingRatePerMMWithGst { get; set; }
        public decimal sellingRatePerKG { get; set; }
        public Nullable<decimal> sellingRatePerKGWithGst { get; set; }
        public Nullable<decimal> availableStock { get; set; }
        public string hsnWithGST { get; set; }
    }
}