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
        [Required]
        public int grnNumber { get; set; }
        public Nullable<DateTime> grnDate { get; set; }
        [Required]
        public long purchaseOrderId { get; set; }
        [Required]
        public long locationId { get; set; }
        [Required]
        public int totalAmount { get; set; }
        public Nullable<int> amountPaid { get; set; }
        public Nullable<int> balanceAmount { get; set; }

        public virtual VMCompanyLocation MstCompanyLocation { get; set; }
        public virtual VMTrnPurchaseOrder TrnPurchaseOrder { get; set; }
        public virtual List<VMTrnGoodReceiveNoteItem> TrnGoodReceiveNoteItems { get; set; }
    }
}