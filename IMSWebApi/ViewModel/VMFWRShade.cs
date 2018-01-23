using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMFWRShade
    {
        public long id { get; set; }
        [Required]
        public long categoryId { get; set; }
        [Required]
        public long collectionId { get; set; }
        [Required]
        public long qualityId { get; set; }
        [Required]
        public long designId { get; set; }
        [Required]
        [MaxLength(50)]
        public string shadeCode { get; set; }
        [Required]
        [MaxLength(100)]
        public string shadeName { get; set; }
        [Required]
        public int serialNumber { get; set; }
        [MaxLength(500)]
        public string description { get; set; }       
    
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFWRDesign MstFWRDesign { get; set; }
        public virtual VMQuality MstQuality { get; set; }
    }
}