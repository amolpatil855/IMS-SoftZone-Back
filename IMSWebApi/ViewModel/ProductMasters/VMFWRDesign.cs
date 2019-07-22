using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMFWRDesign
    {
        public long id { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Category does not exist")]
        public long categoryId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Collection does not exist")]  
        public long collectionId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Quality does not exist")]
        public long qualityId { get; set; }

        [Required]
        [MaxLength(50), MinLength(1)]
        public string designCode { get; set; }

        [Required]
        [MaxLength(100), MinLength(2)]
        public string designName { get; set; }

        [MaxLength(500)]
        public string description { get; set; }

        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMQuality MstQuality { get; set; }
    }
}