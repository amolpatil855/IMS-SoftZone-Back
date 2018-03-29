using IMSWebApi.ViewModel.SalesInvoice;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.SalesInvoice
{
    public class VMTrnSalesInvoice
    {
        public long id { get; set; }
        public long goodIssueNoteId { get; set; }
        public Nullable<long> salesOrderId { get; set; }
        public Nullable<long> materialQuotationId { get; set; }
        [MaxLength(50)]
        public string invoiceNumber { get; set; }
        public System.DateTime invoiceDate { get; set; }
        [MaxLength(50)]
        public string buyersOrderNumber { get; set; }
        public Nullable<System.DateTime> expectedDeliveryDate { get; set; }
        public long totalAmount { get; set; }
        public long amountPaid { get; set; }
        public bool isPaid { get; set; }
        [MaxLength(20)]
        public string status { get; set; }
        [MaxLength(20)]
        public string courierDockYardNumber { get; set; }
        [MaxLength(10)]
        public string financialYear { get; set; }

        public bool isApproved { get; set; }
        
        public virtual VMCompanyInfo MstCompanyInfo { get; set; }
        public virtual VMTrnMaterialQuotation TrnMaterialQuotation { get; set; }
        public virtual VMTrnGoodIssueNote TrnGoodIssueNote { get; set; }
        public virtual VMTrnSaleOrder TrnSaleOrder { get; set; }
        public virtual List<VMTrnSalesInvoiceItem> TrnSalesInvoiceItems { get; set; }
    }
}