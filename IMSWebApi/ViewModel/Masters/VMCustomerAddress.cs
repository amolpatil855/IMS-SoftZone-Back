﻿using System;
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
        [Required]
        [MaxLength(100)]
        public string addressLine1 { get; set; }
        [MaxLength(100)]
        public string addressLine2 { get; set; }
        [Required]
        [MaxLength(50)]
        public string city { get; set; }
        [Required]
        [MaxLength(50)]
        public string state { get; set; }
        [MaxLength(50)]
        public string country { get; set; }
        [Required]
        [MaxLength(10)]
        public string pin { get; set; }
        [MaxLength(15)]
        public string gstin { get; set; }
        public Nullable<bool> isPrimary { get; set; }
    }
}