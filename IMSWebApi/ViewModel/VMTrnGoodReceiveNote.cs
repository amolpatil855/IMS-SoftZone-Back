using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnGoodReceiveNote
    {
        public long id { get; set; }
        [MaxLength(50)]
        public string grnNumber { get; set; }
        public Nullable<DateTime> grnDate { get; set; }
        [Required]
        public long supplierId { get; set; }
        [Required]
        public long locationId { get; set; }
        [Required]
        public int totalAmount { get; set; }

        public string supplierName { get; set; }
        public string shippingAddress { get; set; }
        
        public virtual VMCompanyLocation MstCompanyLocation { get; set; }
        public virtual VMSupplier MstSupplier { get; set; }
        public virtual List<VMTrnGoodReceiveNoteItem> TrnGoodReceiveNoteItems { get; set; }
    }
}