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
    
    public partial class MstSupplier
    {
        public MstSupplier()
        {
            this.MstSupplierAddressDetails = new HashSet<MstSupplierAddressDetail>();
        }
    
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string firmName { get; set; }
        public string description { get; set; }
        public string gstin { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string accountPersonName { get; set; }
        public string accountPersonEmail { get; set; }
        public string accountPersonPhone { get; set; }
        public string warehousePersonName { get; set; }
        public string warehousePersonEmail { get; set; }
        public string warehousePersonPhone { get; set; }
        public string dispatchPersonName { get; set; }
        public string dispatchPersonEmail { get; set; }
        public string dispatchPersonPhone { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual ICollection<MstSupplierAddressDetail> MstSupplierAddressDetails { get; set; }
    }
}
