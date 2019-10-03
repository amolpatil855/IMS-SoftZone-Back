using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.UploadVM
{
    public class FWRProductStock
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Category does not exist")]
        public long categoryId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Location does not exist")]
        public long locationId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Collection does not exist")]
        public long collectionId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Serial Number does not exist")]
        public long fwrShadeId { get; set; }

        [Required]
        [Range(1, 99999.99, ErrorMessage = "Stock is out of range")]
        public decimal stock { get; set; }
    }
}