using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCompanyInfo
    {
        public long id { get; set; }
        [Required]
        [MaxLength(100)]
        public string companyName { get; set; }
        [Required]
        [MaxLength(254)]
        public string email { get; set; }
        [Required]
        [MaxLength(20)]
        public string phone { get; set; }
        [Required]
        [MaxLength(20)]
        public string gstin { get; set; }
        [Required]
        [MaxLength(100)]
        public string companyLogo { get; set; }
    }
}