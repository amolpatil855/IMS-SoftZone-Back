using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnMaterialQuotationList
    {
        public long id { get; set; }
        public string materialQuotationNumber { get; set; }
        public System.DateTime materialQuotationDate { get; set; }
        public long customerId { get; set; }
        public string customerName { get; set; }
        public string status { get; set; }
        public int totalAmount { get; set; }
    }
}