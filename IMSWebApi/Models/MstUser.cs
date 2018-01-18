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
    
    public partial class MstUser
    {
        public MstUser()
        {
            this.MstCustomers = new HashSet<MstCustomer>();
        }
    
        public long id { get; set; }
        public long roleId { get; set; }
        public long userTypeId { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public Nullable<System.DateTime> lastLogin { get; set; }
        public Nullable<bool> isActive { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual ICollection<MstCustomer> MstCustomers { get; set; }
        public virtual MstRole MstRole { get; set; }
        public virtual MstuserType MstuserType { get; set; }
    }
}
