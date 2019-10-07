using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMFomDensity
    {
        public long id { get; set; }
        //[Required]
        //public long categoryId { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Collection does not exist")]
        public long collectionId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Quality does not exist")]
        public long qualityId { get; set; }

        [Required]
        [MaxLength(10)]
        public string density { get; set; }

        [MaxLength(500)]
        public string description { get; set; }
        [Required]
        [Range(1, 99999.99, ErrorMessage = "Invalid Purchase Rate/MM")]        
        public decimal purchaseRatePerMM { get; set; }
        [Required]
        [Range(1, 99999.99, ErrorMessage = "Invalid Purchase Rate/KG")]                
        public decimal purchaseRatePerKG { get; set; }
        [Required]
        [Range(1, 99999.99, ErrorMessage = "Invalid Selling Rate/MM")]               
        public decimal sellingRatePerMM { get; set; }
        [Required]
        [Range(1, 99999.99, ErrorMessage = "Invalid Selling Rate/KG")]                
        public decimal sellingRatePerKG { get; set; }
       
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMQuality MstQuality { get; set; }
    }
}