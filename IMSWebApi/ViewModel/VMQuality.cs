using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMQuality
    {
        public long id { get; set; }
        [Required]
        public long categoryId { get; set; }
        [Required]
        public long collectionId { get; set; }
        [Required]
        [MaxLength(50)]
        public string qualityCode { get; set; }
        [Required]
        [MaxLength(100)]
        public string qualityName { get; set; }
        [MaxLength(500)]
        public string description { get; set; }
        public Nullable<decimal> width { get; set; }
        public Nullable<decimal> size { get; set; }
        [Required]
        public long hsnId { get; set; }
        public Nullable<decimal> cutRate { get; set; }
        public Nullable<decimal> roleRate { get; set; }
        public Nullable<decimal> rrp { get; set; }
        public Nullable<decimal> maxCutRateDisc { get; set; }
        public Nullable<decimal> maxRoleRateDisc { get; set; }
        public Nullable<decimal> flatRate { get; set; }
        public Nullable<decimal> maxFlatRateDisc { get; set; }
        public Nullable<decimal> custRatePerSqFeet { get; set; }
        public Nullable<decimal> maxDiscout { get; set; }

        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMHsn MstHsn { get; set; }
    }
}