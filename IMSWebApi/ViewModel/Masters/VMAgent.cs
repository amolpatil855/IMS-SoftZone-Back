using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMAgent
    {
        public long id { get; set; }
        [Required]
        [MaxLength(100)]
        public string name { get; set; }
        [Required]
        [MaxLength(20)]
        public string phone { get; set; }
        [Required]
        [MaxLength(254)]
        public string email { get; set; }
        [Required]
        [MaxLength(500)]
        public string address1 { get; set; }
        [MaxLength(500)]
        public string address2 { get; set; }
        [Required]
        [MaxLength(50)]
        public string city { get; set; }
        [Required]
        [MaxLength(50)]
        public string state { get; set; }
        [Required]
        [MaxLength(10)]
        public string pin { get; set; }
        [Required]
        public int commision { get; set; }
    }
}