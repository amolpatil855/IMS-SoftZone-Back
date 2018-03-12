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
            this.MstCustomerAddresses = new HashSet<MstCustomerAddress>();
            this.TrnGoodIssueNotes = new HashSet<TrnGoodIssueNote>();
            this.TrnSaleOrders = new HashSet<TrnSaleOrder>();
            this.TrnMaterialSelections = new HashSet<TrnMaterialSelection>();
            this.TrnMaterialQuotations = new HashSet<TrnMaterialQuotation>();
        }
    
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string nickName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string alternateEmail1 { get; set; }
        public string alternateEmail2 { get; set; }
        public string alternatePhone1 { get; set; }
        public string alternatePhone2 { get; set; }
        public string pan { get; set; }
        public Nullable<bool> isWholesaleCustomer { get; set; }
        public string accountPersonName { get; set; }
        public string accountPersonEmail { get; set; }
        public string accountPersonPhone { get; set; }
        public Nullable<long> userId { get; set; }
        public Nullable<int> creditPeriodDays { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstUser MstUser { get; set; }
        public virtual ICollection<MstCustomerAddress> MstCustomerAddresses { get; set; }
        public virtual ICollection<TrnGoodIssueNote> TrnGoodIssueNotes { get; set; }
        public virtual ICollection<TrnSaleOrder> TrnSaleOrders { get; set; }
        public virtual ICollection<TrnMaterialSelection> TrnMaterialSelections { get; set; }
        public virtual ICollection<TrnMaterialQuotation> TrnMaterialQuotations { get; set; }
    }
}
