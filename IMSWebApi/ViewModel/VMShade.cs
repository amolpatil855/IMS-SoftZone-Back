using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMShade
    {
        public long id { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public long qualityId { get; set; }
        public long designId { get; set; }
        public string shadeCode { get; set; }
        public string shadeName { get; set; }
        public int serialNumber { get; set; }
        public string description { get; set; }       
    
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMDesign MstDesign { get; set; }
        public virtual VMQuality MstQuality { get; set; }
    }
}