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
    
    public partial class TrnMaterialQuotation
    {
        public TrnMaterialQuotation()
        {
            this.TrnMaterialQuotationItems = new HashSet<TrnMaterialQuotationItem>();
        }
    
        public long id { get; set; }
        public long materialSelectionId { get; set; }
        public string materialQuotationNumber { get; set; }
        public System.DateTime materialQuotationDate { get; set; }
        public long customerId { get; set; }
        public Nullable<long> referById { get; set; }
        public string status { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstAgent MstAgent { get; set; }
        public virtual ICollection<TrnMaterialQuotationItem> TrnMaterialQuotationItems { get; set; }
        public virtual TrnMaterialSelection TrnMaterialSelection { get; set; }
    }
}
