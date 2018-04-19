using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMvwLabourJob
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