using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMHsn
    {
        public long id { get; set; }

        [Required]
        [MaxLength(10)]
        public string hsnCode { get; set; }
        public int gst { get; set; }
    }
}