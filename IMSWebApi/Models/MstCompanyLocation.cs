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
    
    public partial class MstCompanyLocation
    {
        public MstCompanyLocation()
        {
            this.TrnProductStocks = new HashSet<TrnProductStock>();
        }
    
        public long id { get; set; }
        public string locationCode { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string pin { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
        public string phone { get; set; }
    
        public virtual ICollection<TrnProductStock> TrnProductStocks { get; set; }
    }
}