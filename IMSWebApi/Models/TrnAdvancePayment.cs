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
    
    public partial class TrnAdvancePayment
    {
        public long id { get; set; }
        public string advancePaymentNumber { get; set; }
        public System.DateTime advancePaymentDate { get; set; }
        public long customerId { get; set; }
        public long materialQuotationId { get; set; }
        public long amount { get; set; }
        public string paymentMode { get; set; }
        public string chequeNumber { get; set; }
        public Nullable<System.DateTime> chequeDate { get; set; }
        public string bankName { get; set; }
        public string bankBranch { get; set; }
        public string financialYear { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCustomer MstCustomer { get; set; }
        public virtual TrnMaterialQuotation TrnMaterialQuotation { get; set; }
    }
}
