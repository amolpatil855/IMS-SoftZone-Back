using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMUser
    {
        public long id { get; set; }
        //[Required]
        public long roleId { get; set; }
        //[Required]
        public long userTypeId { get; set; }
        public Nullable<long> locationId { get; set; }
        //[Required]
        [MaxLength(100)]
        public string userName { get; set; }
        public string password { get; set; }
        //[Required]
        [MaxLength(254)]
        public string email { get; set; }
        //[Required]
        [MaxLength(20)]
        public string phone { get; set; }
        public Nullable<DateTime> lastLogin { get; set; }
        public bool isActive { get; set; }
        public string oldPassword{ get; set; }

        public virtual VMCompanyLocation MstCompanyLocation { get; set; }
        public virtual VMRole MstRole { get; set; }
        public virtual VMUserType MstuserType { get; set; }
    }
}