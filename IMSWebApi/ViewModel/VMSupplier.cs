using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMSupplier
    {
        public long id { get; set; }
        [Required]
        [MaxLength(50)]
        public string code { get; set; }
        [Required]
        [MaxLength(100)]
        public string name { get; set; }
        [Required]
        [MaxLength(50)]
        public string firmName { get; set; }
        [MaxLength(1000)]
        public string description { get; set; }
        [Required]
        [MaxLength(254)]
        public string email { get; set; }
        [Required]
        [MaxLength(20)]
        public string phone { get; set; }
        [MaxLength(100)]
        public string accountPersonName { get; set; }
        [MaxLength(254)]
        public string accountPersonEmail { get; set; }
        [MaxLength(20)]
        public string accountPersonPhone { get; set; }
        [MaxLength(100)]
        public string warehousePersonName { get; set; }
        [MaxLength(254)]
        public string warehousePersonEmail { get; set; }
        [MaxLength(20)]
        public string warehousePersonPhone { get; set; }
        [MaxLength(100)]
        public string dispatchPersonName { get; set; }
        [MaxLength(254)]
        public string dispatchPersonEmail { get; set; }
        [MaxLength(20)]
        public string dispatchPersonPhone { get; set; }



        public virtual List<VMSupplierAddress> MstSupplierAddresses { get; set; }
    }
}