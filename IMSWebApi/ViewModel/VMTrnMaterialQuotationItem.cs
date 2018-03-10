using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnMaterialQuotationItem
    {
        public long id { get; set; }
        public long materialQuotationId { get; set; }
        public string selectionType { get; set; }
        public string area { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> matThickNessId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public Nullable<decimal> matHeight { get; set; }
        public Nullable<decimal> matWidth { get; set; }
        public decimal orderQuantity { get; set; }
        public decimal deliverQuantity { get; set; }
        public decimal balanceQuantity { get; set; }
        public decimal rate { get; set; }
        public decimal discountPercentage { get; set; }
        public int amount { get; set; }
        public decimal rateWithSGT { get; set; }
        public decimal amountWithGST { get; set; }
        public Nullable<int> gst { get; set; }
        public string status { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }

        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
        public virtual VMMatThickness MstMatThickness { get; set; }
        public virtual VMTrnMaterialQuotation TrnMaterialQuotation { get; set; }
    }
}