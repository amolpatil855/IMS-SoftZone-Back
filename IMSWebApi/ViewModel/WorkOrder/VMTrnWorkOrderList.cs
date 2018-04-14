using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnWorkOrderList
    {
        public long id { get; set; }
        public string workOrderNumber { get; set; }
        public System.DateTime workOrderDate { get; set; }
        public string customerName { get; set; }
        public string tailorName { get; set; }
        public string status { get; set; }
    }
}