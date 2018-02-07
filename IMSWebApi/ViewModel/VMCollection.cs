using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCollection
    {
        public long id { get; set; }

        [Required]
        public long categoryId { get; set; }
        [Required]
        public long supplierId { get; set; }
        [Required]
        [MaxLength(50)]
        public string collectionCode { get; set; }
        [Required]
        [MaxLength(100)]
        public string collectionName { get; set; }
        public Nullable<decimal> purchaseDiscount { get; set; }
        [MaxLength(500)]
        public string description { get; set; }
        [MaxLength(100)]
        public string manufacturerName { get; set; }
        public string categoryCode { get; set; }

        public virtual VMSupplier MstSupplier { get; set; }
        public virtual VMCategory MstCategory { get; set; }
    }
}