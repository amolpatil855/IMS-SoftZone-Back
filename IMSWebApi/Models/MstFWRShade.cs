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
    
    public partial class MstFWRShade
    {
        public MstFWRShade()
        {
            this.TrnProductStocks = new HashSet<TrnProductStock>();
            this.TrnGoodReceiveNoteItems = new HashSet<TrnGoodReceiveNoteItem>();
            this.TrnPurchaseOrderItems = new HashSet<TrnPurchaseOrderItem>();
            this.TrnSaleOrderItems = new HashSet<TrnSaleOrderItem>();
        }
    
        public long id { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public long qualityId { get; set; }
        public long designId { get; set; }
        public string shadeCode { get; set; }
        public string shadeName { get; set; }
        public int serialNumber { get; set; }
        public string description { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
        public int stockReorderLevel { get; set; }
    
        public virtual MstCategory MstCategory { get; set; }
        public virtual ICollection<TrnProductStock> TrnProductStocks { get; set; }
        public virtual MstFWRDesign MstFWRDesign { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstQuality MstQuality { get; set; }
        public virtual ICollection<TrnGoodReceiveNoteItem> TrnGoodReceiveNoteItems { get; set; }
        public virtual ICollection<TrnPurchaseOrderItem> TrnPurchaseOrderItems { get; set; }
        public virtual ICollection<TrnSaleOrderItem> TrnSaleOrderItems { get; set; }
    }
}
