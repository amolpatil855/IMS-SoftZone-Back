using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMUser
    {
        public long id { get; set; }
        public long roleId { get; set; }
        public long userTypeId { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public long createdBy { get; set; }
        public Nullable<DateTime> lastLogin { get; set; }
        public Nullable<bool> isActive { get; set; }
        public VMRole MstRole { get; set; }
        public string oldPassword{ get; set; }
        
    }
}