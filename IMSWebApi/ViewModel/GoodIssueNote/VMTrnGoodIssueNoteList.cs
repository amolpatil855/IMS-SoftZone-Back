using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnGoodIssueNoteList
    {
        public long id { get; set; }
        public string ginNumber { get; set; }
        public Nullable<System.DateTime> ginDate { get; set; }
        public string salesOrderNumber { get; set; }
        public string  customerName { get; set; }
        public string status { get; set; }
    }
}