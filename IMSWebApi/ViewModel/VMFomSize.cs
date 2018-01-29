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
        [Required]
        public long categoryId { get; set; }
        [Required]
        public long collectionId { get; set; }
        [Required]
        public long qualityId { get; set; }
        [Required]
        public long fomDensityId { get; set; }
        [Required]
        public long fomSuggestedMMId { get; set; }
        [Required]
        public decimal width { get; set; }
        [Required]
        public decimal length { get; set; }
        [Required]
        [MaxLength(20)]
        public string sizeCode { get; set; }
        [Required]
        public int stockReorderLevel { get; set; }
       
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFomDensity MstFomDensity { get; set; }
        public virtual VMQuality MstQuality { get; set; }
        public virtual VMFomSuggestedMM MstFomSuggestedMM { get; set; }
    }
}