using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMMatThickNess
    {
        public long id { get; set; }
        [MaxLength(10)]
        public string thickNessCode { get; set; }
        public Nullable<decimal> size { get; set; }
    }
}