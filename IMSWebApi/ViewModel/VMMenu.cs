using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMMenu
    {
        public long id { get; set; }
        public string menuName { get; set; }
        public Nullable<long> menuParentId { get; set; }
        public Nullable<long> logicalSequence { get; set; }
    }
}