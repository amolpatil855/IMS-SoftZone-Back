using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMDesign
    {
        public long id { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public long qualityId { get; set; }
        public string designCode { get; set; }
        public string designName { get; set; }
        public string description { get; set; }

        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstQuality MstQuality { get; set; }
    }
}