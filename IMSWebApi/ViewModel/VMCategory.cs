using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCategory
    {
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public long createdBy { get; set; }
        public Nullable<long> updatedBy { get; set; }
    }
}