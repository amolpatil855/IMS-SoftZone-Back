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
    
    public partial class MstMatThickness
    {
        public MstMatThickness()
        {
            this.MstMatSizes = new HashSet<MstMatSize>();
            this.TrnGoodReceiveNoteItems = new HashSet<TrnGoodReceiveNoteItem>();
            this.TrnPurchaseOrderItems = new HashSet<TrnPurchaseOrderItem>();
        }
    
        public long id { get; set; }
        public string thicknessCode { get; set; }
        public decimal size { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual ICollection<MstMatSize> MstMatSizes { get; set; }
        public virtual ICollection<TrnGoodReceiveNoteItem> TrnGoodReceiveNoteItems { get; set; }
        public virtual ICollection<TrnPurchaseOrderItem> TrnPurchaseOrderItems { get; set; }
    }
}
