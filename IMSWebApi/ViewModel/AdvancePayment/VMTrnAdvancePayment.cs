using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnAdvancePayment
    {
        public long id { get; set; }
        public string advancePaymentNumber { get; set; }
        public System.DateTime advancePaymentDate { get; set; }
        public long customerId { get; set; }
        public long materialQuotationId { get; set; }
        public int amount { get; set; }
        public string paymentMode { get; set; }
        public string chequeNumber { get; set; }
        public Nullable<System.DateTime> chequeDate { get; set; }
        public string bankName { get; set; }
        public string bankBranch { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }

        public string customerName { get; set; }
        public string materialQuotationNumber { get; set; }

        public virtual VMCustomer MstCustomer { get; set; }
        public virtual VMTrnMaterialQuotation TrnMaterialQuotation { get; set; }
    }
}