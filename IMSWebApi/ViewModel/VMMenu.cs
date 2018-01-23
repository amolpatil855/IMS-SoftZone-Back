using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMMenu
    {
        public long id { get; set; }
        [Required]
        [MaxLength(50)]
        public string menuName { get; set; }
        public Nullable<long> menuParentId { get; set; }
        public Nullable<long> logicalSequence { get; set; }
    }
}