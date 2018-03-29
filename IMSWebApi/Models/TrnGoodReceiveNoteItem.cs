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
    
    public partial class TrnGoodReceiveNoteItem
    {
        public long id { get; set; }
        public long grnId { get; set; }
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public Nullable<long> matQualityId { get; set; }
        public Nullable<long> matThicknessId { get; set; }
        public string matSizeCode { get; set; }
        public Nullable<long> accessoryId { get; set; }
        public long purchaseOrderId { get; set; }
        public decimal orderQuantity { get; set; }
        public decimal receivedQuantity { get; set; }
        public Nullable<decimal> fomQuantityInKG { get; set; }
        public decimal rate { get; set; }
        public long amount { get; set; }
        public Nullable<decimal> rateWithGST { get; set; }
        public Nullable<long> amountWithGST { get; set; }
        public Nullable<int> gst { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstAccessory MstAccessory { get; set; }
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstFomSize MstFomSize { get; set; }
        public virtual MstFWRShade MstFWRShade { get; set; }
        public virtual MstMatSize MstMatSize { get; set; }
        public virtual MstMatThickness MstMatThickness { get; set; }
        public virtual MstQuality MstQuality { get; set; }
        public virtual TrnGoodReceiveNote TrnGoodReceiveNote { get; set; }
        public virtual TrnPurchaseOrder TrnPurchaseOrder { get; set; }
    }
}
