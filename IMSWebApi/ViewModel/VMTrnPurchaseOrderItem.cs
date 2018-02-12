using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnPurchaseOrderItem
    {
        public long id { get; set; }
        [Required]
        public long purchaseOrderId { get; set; }
        [Required]
        public long categoryId { get; set; }
        [Required]
        public long collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        [MaxLength(20)]
        public string sizeCode { get; set; }
        [Required]
        public decimal orderQuantity { get; set; }
        [Required]
        public decimal balanceQuantity { get; set; }
        [MaxLength(20)]
        public string orderType { get; set; }
        public Nullable<decimal> rate { get; set; }
        public Nullable<int> amount { get; set; }
        [MaxLength(20)]
        public string status { get; set; }
        
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFomSize MstFomSize { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
        public virtual VMTrnPurchaseOrder TrnPurchaseOrder { get; set; }
    }
}