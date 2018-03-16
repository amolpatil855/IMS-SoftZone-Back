using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnGoodReceiveNoteList
    {
        public long id { get; set; }
        public string grnNumber { get; set; }
        public Nullable<DateTime> grnDate { get; set; }
        public List<string> purchaseOrderNumbers { get; set; }
        public string supplierName { get; set; }
        public string locationCode { get; set; }
        public int totalAmount { get; set; }
    }
}