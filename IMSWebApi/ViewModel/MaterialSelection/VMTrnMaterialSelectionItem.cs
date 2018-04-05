using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnMaterialSelectionItem
    {
        public long id { get; set; }
        public long materialSelectionId { get; set; }
        [Required]
        public string selectionType { get; set; }
        [Required]
        public string area { get; set; }
        [Required]
        public long categoryId { get; set; }
        [Required]
        public long collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> matThicknessId { get; set; }
        public Nullable<long> qualityId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public Nullable<decimal> matHeight { get; set; }
        public Nullable<decimal> matWidth { get; set; }
        
        public string categoryName { get; set; }
        public string collectionName { get; set; }
        public string serialno { get; set; }
        public string size { get; set; }
        
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
        public virtual VMMatThickness MstMatThickness { get; set; }
        public virtual VMQuality MstQuality { get; set; }
        public virtual VMTrnMaterialSelection TrnMaterialSelection { get; set; }
    }
}