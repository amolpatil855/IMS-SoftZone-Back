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
        public string quotationNumber { get; set; }
        public string quotationType { get; set; }
        public string customerName { get; set; }
        public long amount { get; set; }
        public string paymentMode { get; set; }
    }
}