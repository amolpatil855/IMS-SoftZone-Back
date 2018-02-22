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
    
    public partial class TrnGoodReceiveNote
    {
        public TrnGoodReceiveNote()
        {
            this.TrnGoodReceiveNoteItems = new HashSet<TrnGoodReceiveNoteItem>();
        }
    
        public long id { get; set; }
        public string grnNumber { get; set; }
        public Nullable<System.DateTime> grnDate { get; set; }
        public long supplierId { get; set; }
        public long locationId { get; set; }
        public int totalAmount { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCompanyLocation MstCompanyLocation { get; set; }
        public virtual MstSupplier MstSupplier { get; set; }
        public virtual ICollection<TrnGoodReceiveNoteItem> TrnGoodReceiveNoteItems { get; set; }
    }
}
