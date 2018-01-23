using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMUserType
    {
        public long id { get; set; }
        [Required]
        [MaxLength(50)]
        public string userTypeName { get; set; }
    }
}