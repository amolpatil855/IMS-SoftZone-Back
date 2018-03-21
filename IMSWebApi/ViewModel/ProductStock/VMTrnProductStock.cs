using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnProductStock
    {
        public long id { get; set; }
        [Required]
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> fwrShadeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> accessoryId { get; set; }
        public Nullable<long> qualityId { get; set; }
        public Nullable<long> matThicknessId { get; set; }
        [MaxLength(20)]
        public string matSizeCode { get; set; }
        [Required]
        public decimal stock { get; set; }
        public Nullable<decimal> stockInKg { get; set; }
        [Required]
        public decimal soQuanity { get; set; }
        [Required]
        public decimal poQuantity { get; set; }

        public string serialno { get; set; }
        public string fomItem { get; set; }
        public string matSize { get; set; }
        public string accessoryCode { get; set; }
        
        public virtual VMAccessory MstAccessory { get; set; }
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFomSize MstFomSize { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
        public virtual VMMatThickness MstMatThickness { get; set; }
        public virtual VMQuality MstQuality { get; set; }
    }
}