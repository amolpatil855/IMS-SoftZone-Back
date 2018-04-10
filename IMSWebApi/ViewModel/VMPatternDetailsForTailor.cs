using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMPatternDetailsForTailor
    {
        public string name { get; set; }
        public long patternId { get; set; }
        public Nullable<decimal> charge { get; set; }
    }
}