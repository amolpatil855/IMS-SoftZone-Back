using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnCurtainSelection
    {
        public long id { get; set; }
        [Required]
        public long customerId { get; set; }
        [MaxLength(20)]
        public string curtainSelectionNumber { get; set; }
        public System.DateTime curtainSelectionDate { get; set; }
        public Nullable<long> referById { get; set; }
        [Required]
        public bool isQuotationCreated { get; set; }
        [MaxLength(10)]
        public string financialYear { get; set; }

        public string customerName { get; set; }

        public virtual VMAgent MstAgent { get; set; }
        public virtual VMCustomer MstCustomer { get; set; }
        public virtual List<VMTrnCurtainSelectionItem> TrnCurtainSelectionItems { get; set; }
    }
}