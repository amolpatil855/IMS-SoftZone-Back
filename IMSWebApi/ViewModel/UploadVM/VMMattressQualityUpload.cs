﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.UploadVM
{
    public class VMMattressQualityUpload
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Category does not exist")]
        public long categoryId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Collection does not exist")]
        public long collectionId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Max Character allowed in Name is 50"), MinLength(2, ErrorMessage = "Minimum 2 characters required in Code")]
        public string qualityCode { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Max Character allowed in Name is 100"), MinLength(2, ErrorMessage = "Minimum 2 characters required in Name")]
        public string qualityName { get; set; }

        [MaxLength(500, ErrorMessage = "Max Character allowed in Name is 500")]
        public string description { get; set; }

        [Required]
        public long hsnId { get; set; }

        [Required]
        [Range(1, 99999.99, ErrorMessage = "Customer Rate/Sq. Feet is out of range")]
        public Nullable<decimal> custRatePerSqFeet { get; set; }

        [Required]
        [Range(1, 99.99, ErrorMessage = "Max Discount(%) is out of range")]
        public Nullable<decimal> maxDiscount { get; set; }

    }
}