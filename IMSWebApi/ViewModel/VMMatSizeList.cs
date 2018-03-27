using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMMatSizeList
    {
        public long id { get; set; }
        public string collectionCode { get; set; }
        public string qualityCode { get; set; }
        public string thicknessCode { get; set; }
        public string sizeCode { get; set; }
        public decimal purchaseRate { get; set; }
    }
}