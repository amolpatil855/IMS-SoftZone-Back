using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMProductDetails
    {
        public decimal stock { get; set; }
        public int gst { get; set; }
        public Nullable<decimal> purchaseDiscount { get; set; }
        
        //FOM
        public Nullable<decimal> purchaseRatePerMM { get; set; }
        public Nullable<decimal> purchaseRatePerKG { get; set; }
        public Nullable<decimal> sellingRatePerMM { get; set; }
        public Nullable<decimal> sellingRatePerKG { get; set; }
        public Nullable<decimal> suggestedMM { get; set; }
        public Nullable<decimal> length { get; set; }       //Mat
        public Nullable<decimal> width { get; set; }        //Mat
        
        //FWR
        public Nullable<decimal> cutRate { get; set; }
        public Nullable<decimal> roleRate { get; set; }
        public Nullable<decimal> rrp { get; set; }
        public Nullable<decimal> maxCutRateDisc { get; set; }
        public Nullable<decimal> maxRoleRateDisc { get; set; }
        public Nullable<decimal> flatRate { get; set; }
        public Nullable<decimal> purchaseFlatRate { get; set; }
        public Nullable<decimal> maxFlatRateDisc { get; set; }
        
        //Mat
        public Nullable<decimal> rate { get; set; }
        public Nullable<decimal> purchaseRate { get; set; }     //Accessory
        public Nullable<decimal> custRatePerSqFeet { get; set; }
        public Nullable<decimal> maxDiscount { get; set; }
        public Nullable<decimal> size { get; set; }         //Mat Thickness size
        
        //Accessory
        public Nullable<decimal> sellingRate { get; set; }
    }
}