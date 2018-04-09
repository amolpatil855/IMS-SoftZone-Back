using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnCurtainQuotationList
    {
        public long id { get; set; }
        public string curtainQuotationNumber { get; set; }
        public System.DateTime curtainQuotationDate { get; set; }
        public long customerId { get; set; }
        public string customerName { get; set; }
        public long totalAmount { get; set; }
        public long balanceAmount { get; set; }
        public string status { get; set; }
    }
}