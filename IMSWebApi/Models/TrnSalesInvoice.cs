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
    
    public partial class TrnSalesInvoice
    {
        public TrnSalesInvoice()
        {
            this.TrnSalesInvoiceItems = new HashSet<TrnSalesInvoiceItem>();
        }
    
        public long id { get; set; }
        public long goodIssueNoteId { get; set; }
        public long salesOrderId { get; set; }
        public string invoiceNumber { get; set; }
        public System.DateTime invoiceDate { get; set; }
        public string buyersOrderNumber { get; set; }
        public Nullable<System.DateTime> expectedDeliveryDate { get; set; }
        public int totalAmount { get; set; }
        public int amountPaid { get; set; }
        public string status { get; set; }
        public string courierDockYardNumber { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual TrnSaleOrder TrnSaleOrder { get; set; }
        public virtual ICollection<TrnSalesInvoiceItem> TrnSalesInvoiceItems { get; set; }
        public virtual TrnGoodIssueNote TrnGoodIssueNote { get; set; }
    }
}
