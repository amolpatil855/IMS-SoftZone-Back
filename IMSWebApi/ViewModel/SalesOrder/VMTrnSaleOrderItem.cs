﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.SalesInvoice
{
    public class VMTrnSaleOrderItem
    {
        public long id { get; set; }
        public long saleOrderId { get; set; }
        [Required]
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        [MaxLength(20)]
        public string sizeCode { get; set; }
        public Nullable<long> accessoryId { get; set; }
        [Required]
        public decimal orderQuantity { get; set; }
        public Nullable<decimal> deliverQuantity { get; set; }
        public Nullable<decimal> balanceQuantity { get; set; }
        [MaxLength(20)]
        public string orderType { get; set; }
        public Nullable<decimal> rate { get; set; }
        public Nullable<decimal> discountPercentage { get; set; }
        public Nullable<int> amount { get; set; }
        public Nullable<decimal> rateWithGST { get; set; }
        public Nullable<int> amountWithGST { get; set; }
        public Nullable<int> gst { get; set; }
        [MaxLength(20)]
        public string status { get; set; }

        public string categoryName { get; set; }
        public string collectionName { get; set; }
        public string serialno { get; set; }
        public string size { get; set; }
        public string accessoryName { get; set; }
        public decimal availableStock { get; set; }

        public virtual VMAccessory MstAccessory { get; set; }
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFomSize MstFomSize { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMMatSize MstMatSize { get; set; }
        //public virtual VMTrnSaleOrder TrnSaleOrder { get; set; }
    }
}