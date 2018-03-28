using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMFomDensityList
    {
        public long id { get; set; }
        public string collectionCode { get; set; }
        public string qualityCode { get; set; }
        public string density { get; set; }
        public decimal purchaseRatePerMM { get; set; }
        public decimal purchaseRatePerKG { get; set; }
    }
}