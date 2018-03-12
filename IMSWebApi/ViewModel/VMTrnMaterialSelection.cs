using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnMaterialSelection
    {
        public long id { get; set; }
        public long customerId { get; set; }
        public string materialSelectionNumber { get; set; }
        public long referById { get; set; }
        public System.DateTime materialSelectionDate { get; set; }
        public long isQuotationCreated { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }

        public string customerName { get; set; }

        public virtual VMAgent MstAgent { get; set; }
        public virtual VMCustomer MstCustomer { get; set; }
        public virtual List<VMTrnMaterialQuotation> TrnMaterialQuotations { get; set; }
        public virtual List<VMTrnMaterialSelectionItem> TrnMaterialSelectionItems { get; set; }
    }
}