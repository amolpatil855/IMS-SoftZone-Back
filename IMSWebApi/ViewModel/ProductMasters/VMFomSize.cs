using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMFomSize
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
        [Range(1, long.MaxValue, ErrorMessage = "Foam density does not exist")]
        public long fomDensityId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Foam suggested MM does not exist")]
        public long fomSuggestedMMId { get; set; }

        [Required]
        public decimal width { get; set; }
        [Required]
        public decimal length { get; set; }
        //[Required]
        //[MaxLength(20)]
        public string sizeCode { get; set; }
        [MaxLength(50)]
        public string itemCode { get; set; }
        [Required]
        public int stockReorderLevel { get; set; }
       
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFomDensity MstFomDensity { get; set; }
        public virtual VMQuality MstQuality { get; set; }
        public virtual VMFomSuggestedMM MstFomSuggestedMM { get; set; }
    }
}