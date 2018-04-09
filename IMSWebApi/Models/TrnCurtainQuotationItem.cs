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
    
    public partial class TrnCurtainQuotationItem
    {
        public long id { get; set; }
        public long curtainQuotationId { get; set; }
        public string area { get; set; }
        public string unit { get; set; }
        public long patternId { get; set; }
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> accessoryId { get; set; }
        public Nullable<bool> isPatch { get; set; }
        public Nullable<int> numberOfPatches { get; set; }
        public Nullable<bool> isLining { get; set; }
        public Nullable<bool> isTrack { get; set; }
        public decimal height { get; set; }
        public decimal width { get; set; }
        public decimal orderQuantity { get; set; }
        public decimal balanceQuantity { get; set; }
        public decimal deliverQuantity { get; set; }
        public string orderType { get; set; }
        public Nullable<decimal> discount { get; set; }
        public decimal rate { get; set; }
        public decimal rateWithGST { get; set; }
        public decimal amount { get; set; }
        public decimal amountWithGST { get; set; }
        public Nullable<int> gst { get; set; }
        public string status { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstAccessory MstAccessory { get; set; }
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstFWRShade MstFWRShade { get; set; }
        public virtual MstPattern MstPattern { get; set; }
        public virtual TrnCurtainQuotation TrnCurtainQuotation { get; set; }
    }
}
