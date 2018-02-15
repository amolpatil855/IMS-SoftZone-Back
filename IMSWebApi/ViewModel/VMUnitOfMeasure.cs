using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMUnitOfMeasure
    {
        public long id { get; set; }
        [Required]
        [MaxLength(20)]
        public string uomCode { get; set; }
        [Required]
        [MaxLength(50)]
        public string uomName { get; set; }
        
        public virtual List<VMAccessory> MstAccessories { get; set; }
    }
}