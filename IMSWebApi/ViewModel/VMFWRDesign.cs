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
        public long categoryId { get; set; }
        [Required]
        public long collectionId { get; set; }
        [Required]
        public long qualityId { get; set; }
        [Required]
        [MaxLength(50)]
        public string designCode { get; set; }
        [Required]
        [MaxLength(100)]
        public string designName { get; set; }
        [Required]
        [MaxLength(500)]
        public string description { get; set; }

        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMQuality MstQuality { get; set; }
    }
}