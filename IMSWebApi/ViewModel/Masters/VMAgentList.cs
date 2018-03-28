using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMAgentList
    {
        public long id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int commision { get; set; }
    }
}