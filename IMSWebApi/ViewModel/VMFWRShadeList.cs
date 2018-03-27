using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMFWRShadeList
    {
        public long id { get; set; }
        public long categoryId { get; set; }
        public string categoryCode { get; set; }
        public string collectionCode { get; set; }
        public string qualityCode { get; set; }
        public string designCode { get; set; }
        public string shadeCode { get; set; }
        public string shadeName { get; set; }
        public int serialNumber { get; set; }
    }
}