using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnMaterialQuotation
    {
        public long id { get; set; }
        public long materialSelectionId { get; set; }
        public string materialQuotationNumber { get; set; }
        public System.DateTime materialQuotationDate { get; set; }
        public long customerId { get; set; }
        public Nullable<long> referById { get; set; }
        public string status { get; set; }
        public int totalAmount { get; set; }
        [MaxLength(10)]
        public string financialYear { get; set; }
        
        public string customerName { get; set; }
        public string agentName { get; set; }
        public string materialSelectionNo { get; set; }
        public int advanceAmount { get; set; }

        public virtual VMAgent MstAgent { get; set; }
        public virtual VMCustomer MstCustomer { get; set; }
        public virtual VMTrnMaterialSelection TrnMaterialSelection { get; set; }
        public virtual List<VMTrnMaterialQuotationItem> TrnMaterialQuotationItems { get; set; }
    }
}