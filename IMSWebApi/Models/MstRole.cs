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
    
    public partial class MstRole
    {
        public MstRole()
        {
            this.CFGRoleMenus = new HashSet<CFGRoleMenu>();
            this.MstUsers = new HashSet<MstUser>();
        }
    
        public long id { get; set; }
        public string roleName { get; set; }
        public string roleDescription { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual ICollection<CFGRoleMenu> CFGRoleMenus { get; set; }
        public virtual ICollection<MstUser> MstUsers { get; set; }
    }
}
