using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel.UploadVM
{
    public class VMPatternForTailor
    {
        [Required]
        public int tailorId { get; set; }

        [Required]
        public int patternId { get; set; }

        public Nullable<decimal> charge { get; set; }
    }
}