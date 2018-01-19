using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMSupplier
    {
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string firmName { get; set; }
        public string description { get; set; }
        public string gstin { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string accountPersonName { get; set; }
        public string accountPersonEmail { get; set; }
        public string accountPersonPhone { get; set; }
        public string warehousePersonName { get; set; }
        public string warehousePersonEmail { get; set; }
        public string warehousePersonPhone { get; set; }
        public string dispatchPersonName { get; set; }
        public string dispatchPersonEmail { get; set; }
        public string dispatchPersonPhone { get; set; }

        public virtual List<VMSupplierAddressDetail> MstSupplierAddressDetails { get; set; }
    }
}