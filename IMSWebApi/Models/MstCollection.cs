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
    
    public partial class MstCollection
    {
        public MstCollection()
        {
            this.MstFWRDesigns = new HashSet<MstFWRDesign>();
            this.MstFomDensities = new HashSet<MstFomDensity>();
            this.MstFomSizes = new HashSet<MstFomSize>();
            this.MstFomSuggestedMMs = new HashSet<MstFomSuggestedMM>();
            this.MstMatSizes = new HashSet<MstMatSize>();
            this.MstQualities = new HashSet<MstQuality>();
            this.MstFWRShades = new HashSet<MstFWRShade>();
            this.TrnProductStockDetails = new HashSet<TrnProductStockDetail>();
            this.TrnProductStocks = new HashSet<TrnProductStock>();
            this.TrnPurchaseOrderItems = new HashSet<TrnPurchaseOrderItem>();
            this.TrnGoodReceiveNoteItems = new HashSet<TrnGoodReceiveNoteItem>();
            this.TrnSaleOrderItems = new HashSet<TrnSaleOrderItem>();
            this.TrnGoodIssueNoteItems = new HashSet<TrnGoodIssueNoteItem>();
            this.TrnSalesInvoiceItems = new HashSet<TrnSalesInvoiceItem>();
        }
    
        public long id { get; set; }
        public long categoryId { get; set; }
        public long supplierId { get; set; }
        public string collectionCode { get; set; }
        public string collectionName { get; set; }
        public Nullable<decimal> purchaseDiscount { get; set; }
        public string description { get; set; }
        public string manufacturerName { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstSupplier MstSupplier { get; set; }
        public virtual ICollection<MstFWRDesign> MstFWRDesigns { get; set; }
        public virtual ICollection<MstFomDensity> MstFomDensities { get; set; }
        public virtual ICollection<MstFomSize> MstFomSizes { get; set; }
        public virtual ICollection<MstFomSuggestedMM> MstFomSuggestedMMs { get; set; }
        public virtual ICollection<MstMatSize> MstMatSizes { get; set; }
        public virtual ICollection<MstQuality> MstQualities { get; set; }
        public virtual ICollection<MstFWRShade> MstFWRShades { get; set; }
        public virtual ICollection<TrnProductStockDetail> TrnProductStockDetails { get; set; }
        public virtual ICollection<TrnProductStock> TrnProductStocks { get; set; }
        public virtual ICollection<TrnPurchaseOrderItem> TrnPurchaseOrderItems { get; set; }
        public virtual ICollection<TrnGoodReceiveNoteItem> TrnGoodReceiveNoteItems { get; set; }
        public virtual ICollection<TrnSaleOrderItem> TrnSaleOrderItems { get; set; }
        public virtual ICollection<TrnGoodIssueNoteItem> TrnGoodIssueNoteItems { get; set; }
        public virtual ICollection<TrnSalesInvoiceItem> TrnSalesInvoiceItems { get; set; }
    }
}
