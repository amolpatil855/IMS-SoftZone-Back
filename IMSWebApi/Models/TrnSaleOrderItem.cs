//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMSWebApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TrnSaleOrderItem
    {
        public long id { get; set; }
        public long saleOrderId { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> fomSizeId { get; set; }
        public Nullable<long> matSizeId { get; set; }
        public string sizeCode { get; set; }
        public decimal orderQuantity { get; set; }
        public Nullable<decimal> deliverQuantity { get; set; }
        public Nullable<decimal> balanceQuantity { get; set; }
        public string orderType { get; set; }
        public Nullable<decimal> rate { get; set; }
        public Nullable<int> amount { get; set; }
        public string status { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstFomSize MstFomSize { get; set; }
        public virtual MstFWRShade MstFWRShade { get; set; }
        public virtual MstMatSize MstMatSize { get; set; }
    }
}
