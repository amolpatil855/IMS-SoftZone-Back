using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMFinancialYear
    {
        public long id { get; set; }
        [Required]
        public System.DateTime startDate { get; set; }
        [Required]
        public System.DateTime endDate { get; set; }
        [Required]
        [MaxLength(10)]
        public string financialYear { get; set; }
        [Required]
        public int poNumber { get; set; }
        [Required]
        public int soNumber { get; set; }
        [Required]
        public int grnNumber { get; set; }
        [Required]
        public int invoiceNumber { get; set; }
    }
}