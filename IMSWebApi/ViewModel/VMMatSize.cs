using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMMatSize
    {
        public long id { get; set; }
        //[Required]
        //public long categoryId { get; set; }
        [Required]
        public long collectionId { get; set; }
        [Required]
        public long qualityId { get; set; }
        [Required]
        public long thicknessId { get; set; }
        [Required]
        public int length { get; set; }
        [Required]
        public int width { get; set; }
        [Required]
        [MaxLength(10)]
        public string sizeCode { get; set; }
        [Required]
        public decimal rate { get; set; }
        [Required]
        public decimal purchaseDiscount { get; set; }
        [Required]
        public decimal purchaseRate { get; set; }
        [Required]
        public int stockReorderLevel { get; set; }
       
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMMatThickness MstMatThickNess { get; set; }
        public virtual VMQuality MstQuality { get; set; }
    }
}