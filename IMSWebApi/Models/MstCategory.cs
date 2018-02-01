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
    
    public partial class MstCategory
    {
        public MstCategory()
        {
            this.MstFWRDesigns = new HashSet<MstFWRDesign>();
            this.MstFomDensities = new HashSet<MstFomDensity>();
            this.MstFomSizes = new HashSet<MstFomSize>();
            this.MstFomSuggestedMMs = new HashSet<MstFomSuggestedMM>();
            this.MstQualities = new HashSet<MstQuality>();
            this.MstFWRShades = new HashSet<MstFWRShade>();
            this.MstMatSizes = new HashSet<MstMatSize>();
            this.TrnProductStocks = new HashSet<TrnProductStock>();
            this.MstCollections = new HashSet<MstCollection>();
        }
    
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual ICollection<MstFWRDesign> MstFWRDesigns { get; set; }
        public virtual ICollection<MstFomDensity> MstFomDensities { get; set; }
        public virtual ICollection<MstFomSize> MstFomSizes { get; set; }
        public virtual ICollection<MstFomSuggestedMM> MstFomSuggestedMMs { get; set; }
        public virtual ICollection<MstQuality> MstQualities { get; set; }
        public virtual ICollection<MstFWRShade> MstFWRShades { get; set; }
        public virtual ICollection<MstMatSize> MstMatSizes { get; set; }
        public virtual ICollection<TrnProductStock> TrnProductStocks { get; set; }
        public virtual ICollection<MstCollection> MstCollections { get; set; }
    }
}
