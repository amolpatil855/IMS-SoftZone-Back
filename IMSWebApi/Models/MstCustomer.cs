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
    
    public partial class MstCustomer
    {
        public MstCustomer()
        {
            this.MstCustomerAddressDetails = new HashSet<MstCustomerAddressDetail>();
        }
    
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string nickName { get; set; }
        public string gstin { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string alternateEmail1 { get; set; }
        public string alternateEamil2 { get; set; }
        public string alternatePhone1 { get; set; }
        public string alternatePhone2 { get; set; }
        public string pan { get; set; }
        public Nullable<bool> isWholesaleCustomer { get; set; }
        public string accountPersonName { get; set; }
        public string accountPersonEmail { get; set; }
        public string accountPersonPhone { get; set; }
        public Nullable<long> userId { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<System.DateTime> updatedBy { get; set; }
    
        public virtual MstUser MstUser { get; set; }
        public virtual ICollection<MstCustomerAddressDetail> MstCustomerAddressDetails { get; set; }
    }
}
