using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnPurchaseOrderList
    {
        public long id { get; set; }
        public string orderNumber { get; set; }
        public Nullable<DateTime> orderDate { get; set; }
        public string supplierName { get; set; }
        public string courierName { get; set; }
        public string courierMode { get; set; }
        public Nullable<int> totalAmount { get; set; }
        public string status { get; set; }
    }
}