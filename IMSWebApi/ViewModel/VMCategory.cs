using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCategory
    {  
        public long id { get; set; }

        [Required]
        [MaxLength(20)]
        public string code { get; set; }

        [Required]
        [MaxLength(50)]
        public string name { get; set; }
    }
}