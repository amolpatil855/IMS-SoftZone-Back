using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMProductForCS
    {
        // For Fabrics
        public Nullable<long> shadeId { get; set; }
        public string serialno { get; set; }
        public Nullable<decimal> rrp { get; set; }
        public Nullable<decimal> flatRate { get; set; }
        public Nullable<decimal> maxFlatRateDisc { get; set; }
        public Nullable<decimal> maxCutRateDisc { get; set; }
        public Nullable<decimal> maxRoleRateDisc { get; set; }
        
        // For Accessories
        public Nullable<long> accessoryId { get; set; }
        public string itemCode { get; set; }
        public Nullable<decimal> sellingRate { get; set; }

    }
}