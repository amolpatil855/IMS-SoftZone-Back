//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMSWebApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TrnMaterialQuotationItem
    {
        public long id { get; set; }
        public long materialQuotationId { get; set; }
        public string selectionType { get; set; }
        public string area { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> matThicknessId { get; set; }
        public Nullable<long> qualityId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public Nullable<decimal> matHeight { get; set; }
        public Nullable<decimal> matWidth { get; set; }
        public decimal orderQuantity { get; set; }
        public decimal deliverQuantity { get; set; }
        public decimal balanceQuantity { get; set; }
        public string orderType { get; set; }
        public decimal rate { get; set; }
        public decimal discountPercentage { get; set; }
        public long amount { get; set; }
        public decimal rateWithGST { get; set; }
        public long amountWithGST { get; set; }
        public Nullable<int> gst { get; set; }
        public string status { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstFWRShade MstFWRShade { get; set; }
        public virtual MstMatSize MstMatSize { get; set; }
        public virtual MstMatThickness MstMatThickness { get; set; }
        public virtual MstQuality MstQuality { get; set; }
        public virtual TrnMaterialQuotation TrnMaterialQuotation { get; set; }
    }
}
