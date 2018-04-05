using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnCurtainSelectionItem
    {
        public long id { get; set; }
        public long curtainSelectionId { get; set; }
        [MaxLength(20)]
        public string area { get; set; }
        [MaxLength(20)]
        public string unit { get; set; }
        [Required]
        public long patternId { get; set; }
        [Required]
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> accessoryId { get; set; }
        [Required]
        public bool isPatch { get; set; }
        [Required]
        public bool isLining { get; set; }
        [Required]
        public decimal rate { get; set; }
        public Nullable<decimal> discount { get; set; }

        public string categoryName { get; set; }
        public string collectionName { get; set; }
        public string serialno { get; set; }
        public string itemCode { get; set; }

        public virtual VMAccessory MstAccessory { get; set; }
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMPattern MstPattern { get; set; }
        public virtual VMTrnCurtainSelection TrnCurtainSelection { get; set; }
    }
}