using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnCurtainQuotationItem
    {
        public long id { get; set; }
        public long curtainQuotationId { get; set; }
        public string area { get; set; }
        public string unit { get; set; }
        public long patternId { get; set; }
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> accessoryId { get; set; }
        public Nullable<bool> isPatch { get; set; }
        public Nullable<bool> isLining { get; set; }
        public Nullable<bool> isTrack { get; set; }
        public decimal height { get; set; }
        public decimal width { get; set; }
        public decimal orderQuantity { get; set; }
        public decimal balanceQuantity { get; set; }
        public decimal deliverQuantity { get; set; }
        public string orderType { get; set; }
        public Nullable<decimal> discount { get; set; }
        public decimal rate { get; set; }
        public decimal rateWithGST { get; set; }
        public decimal amount { get; set; }
        public decimal amountWithGST { get; set; }
        public Nullable<int> gst { get; set; }
        public string status { get; set; }

        public string categoryName { get; set; }
        public string collectionName { get; set; }
        public string serialno { get; set; }
        public string itemCode { get; set; }

        public virtual VMAccessory MstAccessory { get; set; }
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMPattern MstPattern { get; set; }
        public virtual VMTrnCurtainQuotation TrnCurtainQuotation { get; set; }
    }
}