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
        [Range(1, long.MaxValue, ErrorMessage="Category does not exist")]
        public long categoryId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage="Collection does not exist")]      
        public long collectionId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage="Quality does not exist")]
        public long qualityId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage="Design does not exist")]
        public long designId { get; set; }

        [Required]
        [MaxLength(50), MinLength(1)]
        public string shadeCode { get; set; }

        [Required]
        [MaxLength(100), MinLength(2)]
        public string shadeName { get; set; }

        [Required]    
        //[Range(typeof(int), "1", "1000000000")]
        [Range(1, int.MaxValue)]
        public int serialNumber { get; set; }

        [MaxLength(500)]
        public string description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int stockReorderLevel { get; set; }
    
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFWRDesign MstFWRDesign { get; set; }
        public virtual VMQuality MstQuality { get; set; }
    }
}