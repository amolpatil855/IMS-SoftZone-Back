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
        [MaxLength(20)]
        public string mobile { get; set; }
        [MaxLength(20)]
        public string fax { get; set; }
        [MaxLength(50)]
        public string webSite { get; set; }
        [MaxLength(100)]
        public string address1 { get; set; }
        [MaxLength(50)]
        public string address2 { get; set; }
        [Required]
        [MaxLength(20)]
        public string gstin { get; set; }
        [Required]
        [MaxLength(100)]
        public string companyLogo { get; set; }
    }
}