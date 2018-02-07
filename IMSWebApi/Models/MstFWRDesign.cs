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
    
    public partial class MstFWRDesign
    {
        public MstFWRDesign()
        {
            this.MstFWRShades = new HashSet<MstFWRShade>();
        }
    
        public long id { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public long qualityId { get; set; }
        public string designCode { get; set; }
        public string designName { get; set; }
        public string description { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCategory MstCategory { get; set; }
        public virtual ICollection<MstFWRShade> MstFWRShades { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstQuality MstQuality { get; set; }
    }
}
