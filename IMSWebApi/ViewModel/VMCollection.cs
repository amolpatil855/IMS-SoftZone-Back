using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCollection
    {
        public long id { get; set; }
        public long categoryId { get; set; }
        public long supplierId { get; set; }
        public string collectionCode { get; set; }
        public string collectionName { get; set; }
        public string description { get; set; }
        public string manufacturerName { get; set; }
        public long createdBy { get; set; }
        public Nullable<long> updatedBy { get; set; }
        public string categoryCode { get; set; }

        public virtual VMSupplier MstSupplier { get; set; }
        public virtual VMCategory MstCategory { get; set; }
    }
}