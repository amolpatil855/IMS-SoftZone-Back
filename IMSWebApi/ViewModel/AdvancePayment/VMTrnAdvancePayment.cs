using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public Nullable<long> materialQuotationId { get; set; }
        public Nullable<long> curtainQuotationId { get; set; }
        public string quotationType { get; set; }
        public long amount { get; set; }
        public string paymentMode { get; set; }
        public string chequeNumber { get; set; }
        public Nullable<System.DateTime> chequeDate { get; set; }
        public string bankName { get; set; }
        public string bankBranch { get; set; }
        [MaxLength(10)]
        public string financialYear { get; set; }

        public string customerName { get; set; }
        public string materialQuotationNumber { get; set; }
        
        public virtual VMCustomer MstCustomer { get; set; }
        public virtual VMTrnMaterialQuotation TrnMaterialQuotation { get; set; }
    }
}