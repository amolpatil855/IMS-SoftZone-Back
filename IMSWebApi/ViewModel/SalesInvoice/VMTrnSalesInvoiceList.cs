using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.SalesInvoice
{
    public class VMTrnSalesInvoiceList
    {
        public long id { get; set; }
        public string invoiceNumber { get; set; }
        public DateTime invoiceDate { get; set; }    
        public string ginNumber { get; set; }
        public string curtainQuotationNumber { get; set; }
        public string status { get; set; }
        public string courierDockYardNumber { get; set; }
        public bool isPaid { get; set; }
        public long totalAmount { get; set; }
    }
}