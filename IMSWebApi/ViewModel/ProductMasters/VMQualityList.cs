using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMQualityList
    {
        public long id { get; set; }
        public long categoryId { get; set; }
        public string categoryCode { get; set; }
        public string collectionCode { get; set; }
        public string qualityCode { get; set; }
        public string qualityName { get; set; }
        public string hsnCode { get; set; }
    }
}