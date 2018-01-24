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
            this.MstCollections = new HashSet<MstCollection>();
            this.MstQualities = new HashSet<MstQuality>();
            this.MstFomDensities = new HashSet<MstFomDensity>();
            this.MstMatSizes = new HashSet<MstMatSize>();
            this.MstFWRDesigns = new HashSet<MstFWRDesign>();
            this.MstFWRShades = new HashSet<MstFWRShade>();
            this.MstFomSizes = new HashSet<MstFomSize>();
            this.MstFomSuggestdMMs = new HashSet<MstFomSuggestdMM>();
        }
    
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual ICollection<MstCollection> MstCollections { get; set; }
        public virtual ICollection<MstQuality> MstQualities { get; set; }
        public virtual ICollection<MstFomDensity> MstFomDensities { get; set; }
        public virtual ICollection<MstMatSize> MstMatSizes { get; set; }
        public virtual ICollection<MstFWRDesign> MstFWRDesigns { get; set; }
        public virtual ICollection<MstFWRShade> MstFWRShades { get; set; }
        public virtual ICollection<MstFomSize> MstFomSizes { get; set; }
        public virtual ICollection<MstFomSuggestdMM> MstFomSuggestdMMs { get; set; }
    }
}
