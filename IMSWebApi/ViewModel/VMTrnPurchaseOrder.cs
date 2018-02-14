using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnPurchaseOrder
    {
        public long id { get; set; }
        [Required]
        public long courierId { get; set; }
        [Required]
        [MaxLength(20)]
        public string courierMode { get; set; }
        public Nullable<long> saleOrderId { get; set; }
        public Nullable<int> saleOrderNumber { get; set; }
        [Required]
        public long supplierId { get; set; }
        [Required]
        public int orderNumber { get; set; }
        public Nullable<DateTime> orderDate { get; set; }
        public Nullable<DateTime> expectedDeliveryDate { get; set; }
        [Required]
        public long locationId { get; set; }
        [MaxLength(100)]
        public string remark { get; set; }
        [MaxLength(20)]
        public string status { get; set; }
        [MaxLength(10)]
        public string financialYear { get; set; }
        
        public virtual VMCompanyLocation MstCompanyLocation { get; set; }
        public virtual VMCourier MstCourier { get; set; }
        public virtual VMSupplier MstSupplier { get; set; }
        public virtual List<VMTrnGoodReceiveNote> TrnGoodReceiveNotes { get; set; }
        public virtual List<VMTrnPurchaseOrderItem> TrnPurchaseOrderItems { get; set; }
        public virtual VMTrnSaleOrder TrnSaleOrder { get; set; }
    }
}