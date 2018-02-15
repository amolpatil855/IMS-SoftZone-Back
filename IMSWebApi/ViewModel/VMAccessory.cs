using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMAccessory
    {
        public long id { get; set; }
        public long categoryId { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }
        [Required]
        [MaxLength(50)]
        public string itemCode { get; set; }
        public long hsnId { get; set; }
        public long uomId { get; set; }
        [Required]
        public decimal sellingRate { get; set; }
        [Required]
        public decimal purchaseRate { get; set; }
        [Required]
        [MaxLength(20)]
        public string size { get; set; }
        [Required]
        [MaxLength(10)]
        public string description { get; set; }
        
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMHsn MstHsn { get; set; }
        public virtual VMUnitOfMeasure MstUnitOfMeasure { get; set; }
    }
}