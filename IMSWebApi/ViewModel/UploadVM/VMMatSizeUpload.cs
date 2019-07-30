using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.UploadVM
{
    public class VMMatSizeUpload
    {
        [Required]
        public long collectionId { get; set; }
        [Required]
        public long qualityId { get; set; }
        [Required]
        public long thicknessId { get; set; }
        [Required]
        [Range(000.01, 999.99, ErrorMessage = "Length is out of range")]
        public decimal length { get; set; }
        [Required]
        [Range(000.01, 999.99, ErrorMessage = "Width is out of range")]
        public decimal width { get; set; }
        [Required]
        [Range(1, 99999.99, ErrorMessage = "Rate is out of range")]
        public decimal rate { get; set; }
        [Required]
        [Range(1, 99999.99, ErrorMessage = "Purchase Rate is out of range")]
        public decimal purchaseRate { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Stock Reorder Level value is out of range")]
        public int stockReorderLevel { get; set; }
    }
}