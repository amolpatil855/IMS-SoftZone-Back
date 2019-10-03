using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.UploadVM
{
    public class AccessoryProductStock
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Location does not exist")]
        public long locationId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Accessory does not exist")]
        public long accessoryId { get; set; }

        [Required]
        [Range(1, 99999.99, ErrorMessage = "Stock is out of range")]
        public decimal stock { get; set; }
    }
}