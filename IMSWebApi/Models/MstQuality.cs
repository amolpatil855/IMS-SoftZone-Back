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
    
    public partial class MstQuality
    {
        public MstQuality()
        {
            this.MstDesigns = new HashSet<MstDesign>();
            this.MstShades = new HashSet<MstShade>();
        }
    
        public long id { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public string qualityCode { get; set; }
        public string qualityName { get; set; }
        public string description { get; set; }
        public Nullable<decimal> width { get; set; }
        public Nullable<decimal> size { get; set; }
        public Nullable<long> hsnId { get; set; }
        public decimal cutRate { get; set; }
        public decimal roleRate { get; set; }
        public decimal rrp { get; set; }
        public int maxCutRateDisc { get; set; }
        public int maxRoleRateDisc { get; set; }
        public Nullable<decimal> floorRate { get; set; }
        public Nullable<int> maxFloorCutRateDisc { get; set; }
        public Nullable<int> maxFloorRoleRateDisc { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual ICollection<MstDesign> MstDesigns { get; set; }
        public virtual MstHsn MstHsn { get; set; }
        public virtual ICollection<MstShade> MstShades { get; set; }
    }
}
