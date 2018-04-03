using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMvwitemBelowReOrderLevelList
    {
        public string categoryCode { get; set; }
        public string collectionCode { get; set; }
        public string qualityCode { get; set; }
        public string designCode { get; set; }
        public string shadeCode { get; set; }
        public Nullable<int> serialNumber { get; set; }
        public string itemCode { get; set; }
        public string thicknessCode { get; set; }
        public string sizeCode { get; set; }
    }
}