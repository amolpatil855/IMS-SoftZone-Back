using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnGoodIssueNote
    {
        public long id { get; set; }
        public string ginNumber { get; set; }
        [Required]
        public long customerId { get; set; }
        public Nullable<long> salesOrderId { get; set; }
        public string salesOrderNumber { get; set; }
        public Nullable<long> materialQuotationId { get; set; }
        public string materialQuotationNumber { get; set; }
        public Nullable<System.DateTime> ginDate { get; set; }
        public string status { get; set; }

        public virtual VMCustomer MstCustomer { get; set; }
        public virtual VMTrnSaleOrder TrnSaleOrder { get; set; }
        public virtual List<VMTrnGoodIssueNoteItem> TrnGoodIssueNoteItems { get; set; }
    }
}