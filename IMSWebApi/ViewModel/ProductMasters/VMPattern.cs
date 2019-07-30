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
        [Range(0.1, 9999999999999999.99)]
        public decimal fabricHeight { get; set; }
        [Required]
        [Range(0.1, 9999999999999999.99)]
        public decimal liningHeight { get; set; }
        [Required]
        [Range(0.1, 9999999999999999.99)]
        public decimal woFabricHeight { get; set; }
        [Required]
        [Range(0.1, 9999999999999999.99)]
        public decimal woLiningHeight { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int meterPerInch { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int widthPerInch { get; set; }
        [Required]
        [Range(1, 99999.99)]
        public decimal setRateForCustomer { get; set; }
        [Required]
        [Range(0.1, 9999999999999999.99)]
        public Nullable<decimal> verticalPatch { get; set; }
        [Required]
        [Range(0.1, 9999999999999999.99)]
        public Nullable<decimal> horizontalPatch { get; set; }
    }
}