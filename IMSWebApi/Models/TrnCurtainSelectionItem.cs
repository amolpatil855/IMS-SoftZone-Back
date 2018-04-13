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
    
    public partial class TrnCurtainSelectionItem
    {
        public long id { get; set; }
        public long curtainSelectionId { get; set; }
        public string area { get; set; }
        public string unit { get; set; }
        public long patternId { get; set; }
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> accessoryId { get; set; }
        public bool isPatch { get; set; }
        public bool isLining { get; set; }
        public Nullable<decimal> rate { get; set; }
        public Nullable<decimal> discount { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstAccessory MstAccessory { get; set; }
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstFWRShade MstFWRShade { get; set; }
        public virtual MstPattern MstPattern { get; set; }
        public virtual TrnCurtainSelection TrnCurtainSelection { get; set; }
    }
}
