using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnCurtainSelectionList
    {
        public long id { get; set; }
        public string curtainSelectionNumber { get; set; }
        public System.DateTime curtainSelectionDate { get; set; }
        public string customerName { get; set; }
        public bool isQuotationCreated { get; set; }
    }
}