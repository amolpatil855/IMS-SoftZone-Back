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
    
    public partial class vwLabourJob
    {
        public long workOrderId { get; set; }
        public Nullable<decimal> labourcharges { get; set; }
        public string workOrderNumber { get; set; }
        public System.DateTime workOrderDate { get; set; }
        public string curtainQuotationNumber { get; set; }
        public long tailorId { get; set; }
        public string tailorName { get; set; }
        public bool isLabourChargesPaid { get; set; }
    }
}
