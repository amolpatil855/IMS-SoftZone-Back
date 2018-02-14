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
            this.MstFomDensities = new HashSet<MstFomDensity>();
            this.MstFomSizes = new HashSet<MstFomSize>();
            this.MstFomSuggestedMMs = new HashSet<MstFomSuggestedMM>();
            this.MstFWRDesigns = new HashSet<MstFWRDesign>();
            this.MstFWRShades = new HashSet<MstFWRShade>();
            this.MstMatSizes = new HashSet<MstMatSize>();
        }
    
        public long id { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public string qualityCode { get; set; }
        public string qualityName { get; set; }
        public string description { get; set; }
        public Nullable<decimal> width { get; set; }
        public Nullable<decimal> size { get; set; }
        public long hsnId { get; set; }
        public Nullable<decimal> cutRate { get; set; }
        public Nullable<decimal> roleRate { get; set; }
        public Nullable<decimal> rrp { get; set; }
        public Nullable<decimal> maxCutRateDisc { get; set; }
        public Nullable<decimal> maxRoleRateDisc { get; set; }
        public Nullable<decimal> flatRate { get; set; }
        public Nullable<decimal> purchaseFlatRate { get; set; }
        public Nullable<decimal> maxFlatRateDisc { get; set; }
        public Nullable<decimal> custRatePerSqFeet { get; set; }
        public Nullable<decimal> maxDiscount { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual ICollection<MstFomDensity> MstFomDensities { get; set; }
        public virtual ICollection<MstFomSize> MstFomSizes { get; set; }
        public virtual ICollection<MstFomSuggestedMM> MstFomSuggestedMMs { get; set; }
        public virtual ICollection<MstFWRDesign> MstFWRDesigns { get; set; }
        public virtual ICollection<MstFWRShade> MstFWRShades { get; set; }
        public virtual MstHsn MstHsn { get; set; }
        public virtual ICollection<MstMatSize> MstMatSizes { get; set; }
    }
}
