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
    
    public partial class TrnGoodIssueNote
    {
        public TrnGoodIssueNote()
        {
            this.TrnGoodIssueNoteItems = new HashSet<TrnGoodIssueNoteItem>();
            this.TrnSalesInvoices = new HashSet<TrnSalesInvoice>();
        }
    
        public long id { get; set; }
        public string ginNumber { get; set; }
        public long customerId { get; set; }
        public Nullable<long> salesOrderId { get; set; }
        public string salesOrderNumber { get; set; }
        public Nullable<long> materialQuotationId { get; set; }
        public string materialQuotationNumber { get; set; }
        public Nullable<long> workOrderId { get; set; }
        public string workOrderNumber { get; set; }
        public Nullable<System.DateTime> ginDate { get; set; }
        public string status { get; set; }
        public string financialYear { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCustomer MstCustomer { get; set; }
        public virtual ICollection<TrnGoodIssueNoteItem> TrnGoodIssueNoteItems { get; set; }
        public virtual TrnMaterialQuotation TrnMaterialQuotation { get; set; }
        public virtual TrnWorkOrder TrnWorkOrder { get; set; }
        public virtual ICollection<TrnSalesInvoice> TrnSalesInvoices { get; set; }
        public virtual TrnSaleOrder TrnSaleOrder { get; set; }
    }
}
