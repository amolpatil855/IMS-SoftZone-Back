
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
    
public partial class CFGRoleMenu
{

    public long id { get; set; }

    public long roleId { get; set; }

    public Nullable<long> menuId { get; set; }

    public System.DateTime createdOn { get; set; }

    public long createdBy { get; set; }

    public Nullable<System.DateTime> updatedOn { get; set; }

    public Nullable<System.DateTime> updatedBy { get; set; }



    public virtual MstMenu MstMenu { get; set; }

    public virtual MstRole MstRole { get; set; }

}

}
