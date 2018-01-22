using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCFGRoleMenu
    {
        public long id { get; set; }

        [Required]
        public long roleId { get; set; }
        [Required]
        public long menuId { get; set; }
        public VMMenu MstMenu { get; set; }
    }
}