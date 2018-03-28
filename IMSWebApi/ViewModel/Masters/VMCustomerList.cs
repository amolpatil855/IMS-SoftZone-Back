﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCustomerList
    {
        public long id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string nickName { get; set; }
        public string code { get; set; }
        public Nullable<bool> isWholesaleCustomer { get; set; }
        public string gstin { get; set; }
    }
}