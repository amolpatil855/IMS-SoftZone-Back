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
    
    public partial class MstCustomerAddressDetail
    {
        public long id { get; set; }
        public long customerId { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string steate { get; set; }
        public string country { get; set; }
        public string pin { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<System.DateTime> updatedBy { get; set; }
    
        public virtual MstCustomer MstCustomer { get; set; }
    }
}
