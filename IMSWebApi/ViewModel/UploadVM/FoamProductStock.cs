﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.UploadVM
{
    public class FoamProductStock
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Location does not exist")]
        public long locationId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Collection does not exist")]
        public long collectionId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Foam Item does not exist")]
        public long fomSizeId { get; set; }

        [Required]
        [Range(1, 99999.99, ErrorMessage = "Stock is out of range")]
        public decimal stock { get; set; }

        [Required]
        [Range(1, 99999.99, ErrorMessage = "Stock(in KG) is out of range")]
        public decimal stockInKg { get; set; }
    }
}