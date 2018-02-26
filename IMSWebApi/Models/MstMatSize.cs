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
    
    public partial class MstMatSize
    {
        public MstMatSize()
        {
            this.TrnProductStockDetails = new HashSet<TrnProductStockDetail>();
            this.TrnProductStocks = new HashSet<TrnProductStock>();
            this.TrnPurchaseOrderItems = new HashSet<TrnPurchaseOrderItem>();
            this.TrnGoodReceiveNoteItems = new HashSet<TrnGoodReceiveNoteItem>();
            this.TrnSaleOrderItems = new HashSet<TrnSaleOrderItem>();
        }
    
        public long id { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public long qualityId { get; set; }
        public long thicknessId { get; set; }
        public decimal length { get; set; }
        public decimal width { get; set; }
        public string sizeCode { get; set; }
        public decimal rate { get; set; }
        public decimal purchaseRate { get; set; }
        public int stockReorderLevel { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstMatThickness MstMatThickness { get; set; }
        public virtual MstQuality MstQuality { get; set; }
        public virtual ICollection<TrnProductStockDetail> TrnProductStockDetails { get; set; }
        public virtual ICollection<TrnProductStock> TrnProductStocks { get; set; }
        public virtual ICollection<TrnPurchaseOrderItem> TrnPurchaseOrderItems { get; set; }
        public virtual ICollection<TrnGoodReceiveNoteItem> TrnGoodReceiveNoteItems { get; set; }
        public virtual ICollection<TrnSaleOrderItem> TrnSaleOrderItems { get; set; }
    }
}
