using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCFGRoleMenu
    {
        public long id { get; set; }
        public long roleId { get; set; }
        public Nullable<long> menuId { get; set; }
        public VMMenu MstMenu { get; set; }
    }
}