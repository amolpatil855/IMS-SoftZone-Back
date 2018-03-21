using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnProductStockDetailList
    {
        public long id { get; set; }
        public string categoryName { get; set; }
        public string collectionName { get; set; }
        public string serialno { get; set; }
        public string fomItem { get; set; }
        public string matSize { get; set; }
        public string accessoryCode { get; set; }
        public decimal stock { get; set; }
    }
}