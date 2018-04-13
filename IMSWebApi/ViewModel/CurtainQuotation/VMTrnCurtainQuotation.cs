using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnCurtainQuotation
    {
        public long id { get; set; }
        public long curtainSelectionId { get; set; }
        public string curtainQuotationNumber { get; set; }
        public System.DateTime curtainQuotationDate { get; set; }
        public long customerId { get; set; }
        public long shippingAddressId { get; set; }
        public Nullable<long> referById { get; set; }
        public long totalAmount { get; set; }
        public string financialYear { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> expectedDeliveryDate { get; set; }

        public string customerName { get; set; }
        public string agentName { get; set; }
        public string curtainSelectionNo { get; set; }
        public long advanceAmount { get; set; }

        public virtual VMAgent MstAgent { get; set; }
        public virtual VMCustomer MstCustomer { get; set; }
        public virtual VMCustomerAddress MstCustomerAddress { get; set; }
        public virtual VMTrnCurtainSelection TrnCurtainSelection { get; set; }
        public virtual List<VMTrnCurtainQuotationItem> TrnCurtainQuotationItems { get; set; }
    }
}