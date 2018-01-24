using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMMatThickness
    {
        public long id { get; set; }
        [MaxLength(10)]
        public string thicknessCode { get; set; }
        public Nullable<decimal> size { get; set; }
    }
}