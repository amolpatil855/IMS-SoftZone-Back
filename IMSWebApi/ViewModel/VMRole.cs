using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMRole
    {
        public long id { get; set; }
        public string roleName { get; set; }
        public string roleDescription { get; set; }

        //public virtual ICollection<CFGRoleMenu> CFGRoleMenus { get; set; }
        //public virtual ICollection<MstUser> MstUsers { get; set; }
    }
}