using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnProductStockDetail
    {
        public long id { get; set; }
        [Required]
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> fwrShadeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> accessoryId { get; set; }
        [Required]
        public long locationId { get; set; }
        [Required]
        public decimal stock { get; set; }
        public Nullable<decimal> stockInKg { get; set; }
        public Nullable<decimal> kgPerUnit { get; set; }

        public string serialno { get; set; }
        public string fomItem { get; set; }
        public string matSize { get; set; }
        public string accessoryCode { get; set; }

        public virtual VMAccessory MstAccessory { get; set; }
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMCompanyLocation  MstCompanyLocation { get; set; }
        public virtual VMFomSize MstFomSize { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
    }
}