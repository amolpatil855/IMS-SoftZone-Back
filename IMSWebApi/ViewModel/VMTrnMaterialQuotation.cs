using System;
using System.Collections.Generic;
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
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }

        public string customerName { get; set; }

        public virtual VMAgent MstAgent { get; set; }
        public virtual VMCustomer MstCustomer { get; set; }
        public virtual VMTrnMaterialSelection TrnMaterialSelection { get; set; }
        public virtual List<VMTrnMaterialQuotationItem> TrnMaterialQuotationItems { get; set; }
    }
}