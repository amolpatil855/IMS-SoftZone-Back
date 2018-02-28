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
    
    public partial class MstAccessory
    {
        public MstAccessory()
        {
            this.TrnProductStocks = new HashSet<TrnProductStock>();
            this.TrnProductStockDetails = new HashSet<TrnProductStockDetail>();
            this.TrnPurchaseOrderItems = new HashSet<TrnPurchaseOrderItem>();
            this.TrnGoodReceiveNoteItems = new HashSet<TrnGoodReceiveNoteItem>();
            this.TrnSaleOrderItems = new HashSet<TrnSaleOrderItem>();
            this.TrnGoodIssueNoteItems = new HashSet<TrnGoodIssueNoteItem>();
            this.TrnSalesInvoiceItems = new HashSet<TrnSalesInvoiceItem>();
        }
    
        public long id { get; set; }
        public long categoryId { get; set; }
        public string itemCode { get; set; }
        public string name { get; set; }
        public long supplierId { get; set; }
        public long hsnId { get; set; }
        public long uomId { get; set; }
        public decimal sellingRate { get; set; }
        public decimal purchaseRate { get; set; }
        public string size { get; set; }
        public string description { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstHsn MstHsn { get; set; }
        public virtual MstSupplier MstSupplier { get; set; }
        public virtual MstUnitOfMeasure MstUnitOfMeasure { get; set; }
        public virtual ICollection<TrnProductStock> TrnProductStocks { get; set; }
        public virtual ICollection<TrnProductStockDetail> TrnProductStockDetails { get; set; }
        public virtual ICollection<TrnPurchaseOrderItem> TrnPurchaseOrderItems { get; set; }
        public virtual ICollection<TrnGoodReceiveNoteItem> TrnGoodReceiveNoteItems { get; set; }
        public virtual ICollection<TrnSaleOrderItem> TrnSaleOrderItems { get; set; }
        public virtual ICollection<TrnGoodIssueNoteItem> TrnGoodIssueNoteItems { get; set; }
        public virtual ICollection<TrnSalesInvoiceItem> TrnSalesInvoiceItems { get; set; }
    }
}
