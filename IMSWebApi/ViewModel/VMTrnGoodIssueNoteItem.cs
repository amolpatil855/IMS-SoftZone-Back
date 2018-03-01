using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnGoodIssueNoteItem
    {
        public long id { get; set; }
        public long goodIssueNoteId { get; set; }
        [Required]
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public string sizeCode { get; set; }
        public Nullable<long> accessoryId { get; set; }
        public decimal orderQuantity { get; set; }
        public Nullable<decimal> issuedQuantity { get; set; }
        public decimal rate { get; set; }
        public Nullable<decimal> discountPercentage { get; set; }
        public int amount { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> statusChangeDate { get; set; }
       
        public virtual VMAccessory MstAccessory { get; set; }
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFomSize MstFomSize { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
        public virtual VMTrnGoodIssueNote TrnGoodIssueNote { get; set; }
    }
}