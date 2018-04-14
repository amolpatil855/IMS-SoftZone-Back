using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnWorkOrder
    {
        public long id { get; set; }
        public long curtainQuotationId { get; set; }
        public string workOrderNumber { get; set; }
        public System.DateTime workOrderDate { get; set; }
        public long customerId { get; set; }
        public Nullable<long> tailorId { get; set; }
        public string financialYear { get; set; }
        public Nullable<System.DateTime> expectedDeliveryDate { get; set; }
        public string status { get; set; }

        public string customerName { get; set; }
        public string curtainQuotationNo{ get; set; }

        public virtual VMCustomer MstCustomer { get; set; }
        public virtual VMTailor MstTailor { get; set; }
        public virtual VMTrnCurtainQuotation TrnCurtainQuotation { get; set; }
        public virtual List<VMTrnWorkOrderItem> TrnWorkOrderItems { get; set; }
    }
}