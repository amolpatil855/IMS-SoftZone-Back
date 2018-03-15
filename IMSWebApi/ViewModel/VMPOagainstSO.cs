using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMPOagainstSO
    {
        public long supplierId { get; set; }
        
        //fields to be used for PO items
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> matQualityId { get; set; }
        public Nullable<long> matThicknessId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        [MaxLength(20)]
        public string matSizeCode { get; set; }
        public Nullable<long> accessoryId { get; set; }
        [Required]
        public decimal orderQuantity { get; set; }
        public decimal balanceQuantity { get; set; }
        [MaxLength(20)]
        public string orderType { get; set; }
        public Nullable<decimal> rate { get; set; }                 //Will be used in MAT also
        public Nullable<decimal> rateWithGST { get; set; }
        public Nullable<int> amount { get; set; }
        public Nullable<int> amountWithGST { get; set; }
        public Nullable<int> gst { get; set; }
        [MaxLength(20)]
        public string status { get; set; }
        //fields to be used for PO items

        //Fields for showing values in listing
        public string categoryName { get; set; }
        public string collectionName { get; set; }
        public string serialno { get; set; }
        //public string sizeForListing { get; set; }                  
        public string accessoryName { get; set; }
        //Fields for showing values in listing

        public decimal requiredQuantity { get; set; }           //Quantity required to fulfill Sales Order
        public decimal availableStock { get; set; }             //Quantity available in Stock


        //Properties required for calculations
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
                //public Nullable<decimal> rate { get; set; }
                public Nullable<decimal> purchaseRate { get; set; }     //Accessory
                public Nullable<decimal> custRatePerSqFeet { get; set; }
                public Nullable<decimal> maxDiscount { get; set; }
                public string size { get; set; }         //Mat Thickness size   For Display(15/03/2018)     Nullable<decimal> to string

                //Accessory
                public Nullable<decimal> sellingRate { get; set; }

        //Properties required for calculations
    }
}