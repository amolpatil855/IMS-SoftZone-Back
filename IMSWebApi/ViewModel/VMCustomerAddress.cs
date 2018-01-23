using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCustomerAddress
    {
        public long id { get; set; }
        [Required]
        public long customerId { get; set; }
        [MaxLength(1000)]
        public string address { get; set; }
        [MaxLength(50)]
        public string city { get; set; }
        [MaxLength(50)]
        public string state { get; set; }
        [MaxLength(50)]
        public string country { get; set; }
        [MaxLength(10)]
        public string pin { get; set; }
    }
}