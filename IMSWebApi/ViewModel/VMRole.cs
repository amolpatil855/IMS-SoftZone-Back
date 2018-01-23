using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMRole
    {
        public long id { get; set; }
        [Required]
        [MaxLength(50)]
        public string roleName { get; set; }
        [MaxLength(100)]
        public string roleDescription { get; set; }

        public virtual List<VMCFGRoleMenu> CFGRoleMenus { get; set; }
        //public virtual ICollection<MstUser> MstUsers { get; set; }
    }
}