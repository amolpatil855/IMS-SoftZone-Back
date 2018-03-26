using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMFomSizeList
    {
        public long id { get; set; }
        public string collectionCode { get; set; }
        public string qualityCode { get; set; }
        public string density { get; set; }
        public string suggestedMM { get; set; }
        public string sizeCode { get; set; }
        public string itemCode { get; set; }
    }
}