using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMAccessoryList
    {
        public long id { get; set; }
        public string name { get; set; }
        public string itemCode { get; set; }
        public string supplierCode { get; set; }
        public decimal sellingRate { get; set; }
        public decimal purchaseRate { get; set; }
        public string size { get; set; }
    }
}