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
        public DateTime startDate { get; set; }
        [Required]
        public DateTime endDate { get; set; }
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
        public int ginNumber { get; set; }
        [Required]
        public int poInvoiceNumber { get; set; }
        [Required]
        public int soInvoiceNumber { get; set; }
        [Required]
        public int materialSelectionNumber { get; set; }
        [Required]
        public int materialQuotationNumber { get; set; }
        [Required]
        public int curtainSelectionNumber { get; set; }
        [Required]
        public int curtainQuotationNumber { get; set; }
        [Required]
        public int jobCardNumber { get; set; }
    }
}