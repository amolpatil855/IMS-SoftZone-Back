using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCompanyInfo
    {
        public long id { get; set; }
        public string companyName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string gstin { get; set; }
        public string companyLogo { get; set; }
        public long createdBy { get; set; }
        public Nullable<long> updatedBy { get; set; }
    }
}