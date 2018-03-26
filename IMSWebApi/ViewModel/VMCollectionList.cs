using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCollectionList
    {
        public long id { get; set; }
        public string categoryCode { get; set; }
        public string collectionCode { get; set; }
        public string collectionName { get; set; }
        public string supplierCode { get; set; }
        public string manufacturerName { get; set; }
    }
}