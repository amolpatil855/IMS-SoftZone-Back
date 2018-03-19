using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnAdvancePaymentList
    {
        public long id { get; set; }
        public string advancePaymentNumber { get; set; }
        public System.DateTime advancePaymentDate { get; set; }
        public string materialQuotationNumber { get; set; }
        public string customerName { get; set; }
        public int amount { get; set; }
        public string paymentMode { get; set; }
    }
}