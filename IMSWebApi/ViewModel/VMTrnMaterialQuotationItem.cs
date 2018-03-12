using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnMaterialQuotationItem
    {
        public long id { get; set; }
        public long materialQuotationId { get; set; }
        [Required]
        [MaxLength(20)]
        public string selectionType { get; set; }
        [Required]
        [MaxLength(20)]
        public string area { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> matThicknessId { get; set; }
        public Nullable<long> qualityId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public Nullable<decimal> matHeight { get; set; }
        public Nullable<decimal> matWidth { get; set; }
        [Required]
        public decimal orderQuantity { get; set; }
        public decimal deliverQuantity { get; set; }
        public decimal balanceQuantity { get; set; }
        public decimal rate { get; set; }
        public decimal discountPercentage { get; set; }
        public int amount { get; set; }
        public decimal rateWithSGT { get; set; }
        public decimal amountWithGST { get; set; }
        public Nullable<int> gst { get; set; }
        public string status { get; set; }

        public string categoryName { get; set; }
        public string collectionName { get; set; }
        public string serialno { get; set; }
        public string size { get; set; }

        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
        public virtual VMMatThickness MstMatThickness { get; set; }
        public virtual VMQuality MstQuality { get; set; }
        public virtual VMTrnMaterialQuotation TrnMaterialQuotation { get; set; }
    }
}