using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCustomer
    {
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string nickName { get; set; }
        public string gstin { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string alternateEmail1 { get; set; }
        public string alternateEmail2 { get; set; }
        public string alternatePhone1 { get; set; }
        public string alternatePhone2 { get; set; }
        public string pan { get; set; }
        public Nullable<bool> isWholesaleCustomer { get; set; }
        public string accountPersonName { get; set; }
        public string accountPersonEmail { get; set; }
        public string accountPersonPhone { get; set; }
        public Nullable<long> userId { get; set; }
        public long createdBy { get; set; }
        public Nullable<long> updatedBy { get; set; }

        public virtual VMUser MstUser { get; set; }
        public virtual List<VMCustomerAddressDetail> MstCustomerAddressDetails { get; set; }
    }
}