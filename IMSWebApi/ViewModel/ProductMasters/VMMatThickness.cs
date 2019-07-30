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
        [Required]
        [MaxLength(10, ErrorMessage="Max length of Thickness Code is out of range")]
        public string thicknessCode { get; set; }
        [Required] 
        [Range(00.01, 99.99, ErrorMessage="Size is out of range")]
        public decimal size { get; set; }
    }
}