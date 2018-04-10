using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTailorPatternChargeDetail
    {
        public long id { get; set; }
        public long tailorId { get; set; }
        [Required]
        public long patternId { get; set; }
        [Required]
        public decimal charge { get; set; }
        
        public virtual VMPattern MstPattern { get; set; }
        public virtual VMTailor MstTailor { get; set; }
    }
}