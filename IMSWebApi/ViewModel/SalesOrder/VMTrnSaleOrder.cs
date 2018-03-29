using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.SalesInvoice
{
    public class VMTrnSaleOrder
    {
        public long id { get; set; }
        [MaxLength(50)]
        public string orderNumber { get; set; }
        [Required]
        public long customerId { get; set; }
        [Required]
        public long shippingAddressId { get; set; }
        public Nullable<long> courierId { get; set; }
        [MaxLength(20)]
        public string courierMode { get; set; }
        public Nullable<long> referById { get; set; }
        public DateTime orderDate { get; set; }
        public Nullable<DateTime> expectedDeliveryDate { get; set; }
        public Nullable<long> totalAmount { get; set; }
        [MaxLength(20)]
        public string paymentMode { get; set; }
        [MaxLength(20)]
        public string chequeNumber { get; set; }
        public Nullable<System.DateTime> chequeDate { get; set; }
        [MaxLength(50)]
        public string bankName { get; set; }
        [MaxLength(20)]
        public string bankBranch { get; set; }
        [MaxLength(100)]
        public string remark { get; set; }
        [MaxLength(20)]
        public string status { get; set; }
        [MaxLength(10)]
        public string financialYear { get; set; }

        public string courierName { get; set; }
        public string customerName { get; set; }
        public VMCustomerAddress shippingAddress { get; set; }

        public virtual VMAgent MstAgent { get; set; }
        public virtual VMCourier MstCourier { get; set; }
        public virtual VMCustomer MstCustomer { get; set; }
        public virtual VMCustomerAddress MstCustomerAddress { get; set; }
        public virtual List<VMTrnPurchaseOrder> TrnPurchaseOrders { get; set; }
        public virtual List<VMTrnSaleOrderItem> TrnSaleOrderItems { get; set; }
    }
}