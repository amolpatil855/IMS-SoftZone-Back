using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnGoodReceiveNoteItem
    {
        public long id { get; set; }
        public long grnId { get; set; }
        [Required]
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public Nullable<long> matQualityId { get; set; }
        public Nullable<long> matThicknessId { get; set; }
        [MaxLength(20)]
        public string matSizeCode { get; set; }
        public Nullable<long> accessoryId { get; set; }
        public long purchaseOrderId { get; set; }
        public decimal orderQuantity { get; set; }
        public decimal receivedQuantity { get; set; }
        public Nullable<decimal> fomQuantityInKG { get; set; }
        [Required]
        public decimal rate { get; set; }
        [Required]
        public long amount { get; set; }
        public Nullable<decimal> rateWithGST { get; set; }
        public Nullable<long> amountWithGST { get; set; }
        public Nullable<int> gst { get; set; }

        public string purchaseOrderNumber { get; set; }
        public string orderType { get; set; }
        public string categoryName { get; set; }
        public string collectionName { get; set; }
        public string serialno { get; set; }
        public string size { get; set; }
        public string accessoryName { get; set; }
        public decimal? purchaseDiscount { get; set; }
        public decimal? cutRate { get; set; }
        public decimal? roleRate { get; set; }
        public decimal? purchaseFlatRate { get; set; }

        public virtual VMAccessory MstAccessory { get; set; }
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFomSize MstFomSize { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
        public virtual VMMatThickness MstMatThickness { get; set; }
        public virtual VMQuality MstQuality { get; set; }
        public virtual VMTrnPurchaseOrder TrnPurchaseOrder { get; set; }
        //public virtual VMTrnGoodReceiveNote TrnGoodReceiveNote { get; set; }
    }
}