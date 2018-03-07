using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnSalesInvoice
    {
        public long id { get; set; }
        public long goodIssueNoteId { get; set; }
        public long salesOrderId { get; set; }
        public string invoiceNumber { get; set; }
        public System.DateTime invoiceDate { get; set; }
        public int totalAmount { get; set; }
        public int amountPaid { get; set; }
        public string status { get; set; }
        public string courierDockYardNumber { get; set; }
        
        public virtual VMCompanyInfo MstCompanyInfo { get; set; }
        public virtual VMTrnGoodIssueNote TrnGoodIssueNote { get; set; }
        public virtual VMTrnSaleOrder TrnSaleOrder { get; set; }
        public virtual List<VMTrnSalesInvoiceItem> TrnSalesInvoiceItems { get; set; }
    }
}