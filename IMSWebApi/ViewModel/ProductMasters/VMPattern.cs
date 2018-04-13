using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMPattern
    {
        public long id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }
        [Required]
        public decimal fabricHeight { get; set; }
        public Nullable<decimal> liningHeight { get; set; }
        [Required]
        public int meterPerInch { get; set; }
        [Required]
        public int widthPerInch { get; set; }
        [Required]
        public decimal setRate { get; set; }
        [Required]
        public decimal setRateForCustomer { get; set; }
        public Nullable<decimal> verticalPatch { get; set; }
        public Nullable<decimal> horizontalPatch { get; set; }
    }
}