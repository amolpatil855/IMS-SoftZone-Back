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
    
    public partial class TrnProductStock
    {
        public long id { get; set; }
        public long categoryId { get; set; }
        public long collectionid { get; set; }
        public Nullable<long> fwrShadeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public long locationId { get; set; }
        public decimal stock { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstFomSize MstFomSize { get; set; }
        public virtual MstFWRShade MstFWRShade { get; set; }
        public virtual MstMatSize MstMatSize { get; set; }
    }
}
