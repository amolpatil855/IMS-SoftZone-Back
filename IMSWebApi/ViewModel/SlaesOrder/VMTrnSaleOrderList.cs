using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.SlaesOrder
{
    public class VMTrnSaleOrderList
    {
        public long id { get; set; }
        public string orderNumber { get; set; }
        public string courierName { get; set; }
        public string customerName { get; set; }
        public string agentName { get; set; }
        public DateTime orderDate { get; set; }
        public string status { get; set; }
    }
}