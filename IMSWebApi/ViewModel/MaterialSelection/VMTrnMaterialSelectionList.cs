using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnMaterialSelectionList
    {
        public long id { get; set; }
        public string materialSelectionNumber { get; set; }
        public System.DateTime materialSelectionDate { get; set; }
        public string customerName { get; set; }
        public bool isQuotationCreated { get; set; }
    }
}