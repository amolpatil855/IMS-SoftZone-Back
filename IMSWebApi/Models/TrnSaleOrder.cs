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
    
    public partial class TrnSaleOrder
    {
        public TrnSaleOrder()
        {
            this.TrnPurchaseOrders = new HashSet<TrnPurchaseOrder>();
        }
    
        public long id { get; set; }
        public string orderNumber { get; set; }
        public long customerId { get; set; }
        public string shippingAddress { get; set; }
        public Nullable<long> courierId { get; set; }
        public string courierMode { get; set; }
        public Nullable<long> referById { get; set; }
        public System.DateTime orderDate { get; set; }
        public string remark { get; set; }
        public string status { get; set; }
        public string financialYear { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstAgent MstAgent { get; set; }
        public virtual MstCourier MstCourier { get; set; }
        public virtual MstCustomer MstCustomer { get; set; }
        public virtual ICollection<TrnPurchaseOrder> TrnPurchaseOrders { get; set; }
    }
}