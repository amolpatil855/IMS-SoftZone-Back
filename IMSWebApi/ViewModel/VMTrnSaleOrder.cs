using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnSaleOrder
    {
        public long id { get; set; }
        [Required]
        [MaxLength(10)]
        public string orderNumber { get; set; }
        [Required]
        public long customerId { get; set; }
        [Required]
        [MaxLength(500)]
        public string shippingAddress { get; set; }
        public Nullable<long> courierId { get; set; }
        [MaxLength(20)]
        public string courierMode { get; set; }
        public Nullable<long> referById { get; set; }
        public DateTime orderDate { get; set; }
        [MaxLength(100)]
        public string remark { get; set; }
        [Required]
        [MaxLength(20)]
        public string status { get; set; }
        [Required]
        [MaxLength(10)]
        public string financialYear { get; set; }
        
        public virtual VMAgent MstAgent { get; set; }
        public virtual VMCourier MstCourier { get; set; }
        public virtual VMCustomer MstCustomer { get; set; }
    }
}