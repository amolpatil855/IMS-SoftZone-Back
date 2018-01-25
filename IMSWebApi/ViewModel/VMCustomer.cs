using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMCustomer
    {
        public long id { get; set; }
        [Required]
        [MaxLength(50)]
        public string code { get; set; }
        [Required]
        [MaxLength(100)]
        public string name { get; set; }
        
        [MaxLength(100)]
        public string nickName { get; set; }
        [MaxLength(20)]
        public string gstin { get; set; }
        [Required]
        [MaxLength(254)]
        public string email { get; set; }
        [Required]
        [MaxLength(20)]
        public string phone { get; set; }
        [MaxLength(254)]
        public string alternateEmail1 { get; set; }
        [MaxLength(254)]
        public string alternateEmail2 { get; set; }
        [MaxLength(20)]
        public string alternatePhone1 { get; set; }
        [MaxLength(20)]
        public string alternatePhone2 { get; set; }
        [MaxLength(10)]
        public string pan { get; set; }
        public Nullable<bool> isWholesaleCustomer { get; set; }
        [MaxLength(100)]
        public string accountPersonName { get; set; }
        [MaxLength(254)]
        public string accountPersonEmail { get; set; }
        [MaxLength(20)]
        public string accountPersonPhone { get; set; }
        public Nullable<long> userId { get; set; }
        public string userName { get; set; }
        
        public virtual VMUser MstUser { get; set; }
        public virtual List<VMCustomerAddress> MstCustomerAddresses { get; set; }
    }
}