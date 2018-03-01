using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnSalesInvoiceItem
    {
        public long id { get; set; }
        public long salesInvoiceId { get; set; }
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public string sizeCode { get; set; }
        public Nullable<long> accessoryId { get; set; }
        public decimal quantity { get; set; }
        public decimal rate { get; set; }
        public Nullable<decimal> discountPrecentage { get; set; }
        public int amount { get; set; }
        public Nullable<decimal> rateWithGST { get; set; }
        public Nullable<int> amountWithGST { get; set; }
        public Nullable<int> gst { get; set; }
        public string uom { get; set; }
        public string hsnCode { get; set; }

        public string categoryName { get; set; }
        public string collectionName { get; set; }
        public string serialno { get; set; }
        public string size { get; set; }
        public string accessoryName { get; set; }

        public virtual VMAccessory MstAccessory { get; set; }
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFomSize MstFomSize { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
        public virtual VMTrnSalesInvoice TrnSalesInvoice { get; set; }
    }
}