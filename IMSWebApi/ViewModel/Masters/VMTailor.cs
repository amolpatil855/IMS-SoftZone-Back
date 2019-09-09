using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTailor
    {
        public long id { get; set; }
        [Required]
        [MaxLength(100)]
        public string name { get; set; }
        [MaxLength(254)]
        public string email { get; set; }
        [Required]
        [MaxLength(10)]
        public string phone { get; set; }
        [MaxLength(10)]
        public string alternatePhone1 { get; set; }
        [Required]
        [MaxLength(500)]
        public string addressLine1 { get; set; }
        [MaxLength(500)]
        public string addressLine2 { get; set; }
        [Required]
        [MaxLength(50)]
        public string city { get; set; }
        [Required]
        [MaxLength(50)]
        public string state { get; set; }
        public string country { get; set; }
        [Required]
        [MaxLength(6)]
        public string pin { get; set; }
        
        public virtual List<VMTailorPatternChargeDetail> MstTailorPatternChargeDetails { get; set; }
    }
}