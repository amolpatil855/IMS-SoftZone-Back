using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCourier
    {
        public long id { get; set; }
        [Required]
        [MaxLength(100)]
        public string name { get; set; }
        [MaxLength(20)]
        public string phone { get; set; }
        [MaxLength(20)]
        public string mobile { get; set; }
        [MaxLength(254)]
        public string email { get; set; }
        [MaxLength(500)]
        public string address { get; set; }
        [MaxLength(10)]
        public string pin { get; set; }
    }
}