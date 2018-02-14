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
        [Required]
        public long grnId { get; set; }
        [Required]
        public long categoryId { get; set; }
        [Required]
        public long collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        [MaxLength(20)]
        public string matSizeCode { get; set; }
        public Nullable<decimal> orderQuantity { get; set; }
        public Nullable<decimal> receivedQuantity { get; set; }
        [Required]
        public decimal rate { get; set; }
        [Required]
        public int Amount { get; set; }

        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFomSize MstFomSize { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
        //public virtual VMTrnGoodReceiveNote TrnGoodReceiveNote { get; set; }
    }
}