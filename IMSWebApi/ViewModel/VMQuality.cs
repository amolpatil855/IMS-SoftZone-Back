using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMQuality
    {
        public long id { get; set; }
        public long categoryId { get; set; }
        public long collectionId { get; set; }
        public string qualityCode { get; set; }
        public string qualityName { get; set; }
        public string description { get; set; }
        public Nullable<decimal> width { get; set; }
        public Nullable<decimal> size { get; set; }
        public Nullable<long> hsnId { get; set; }
        public decimal cutRate { get; set; }
        public decimal roleRate { get; set; }
        public decimal rrp { get; set; }
        public int maxCutRateDisc { get; set; }
        public int maxRoleRateDisc { get; set; }
        public Nullable<decimal> floorRate { get; set; }
        public Nullable<int> maxFloorCutRateDisc { get; set; }
        public Nullable<int> maxFloorRoleRateDisc { get; set; }
        

        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMHsn MstHsn { get; set; }
    }
}