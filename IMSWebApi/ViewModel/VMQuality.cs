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
        public Nullable<decimal> cutRate { get; set; }
        public Nullable<decimal> roleRate { get; set; }
        public Nullable<decimal> rrp { get; set; }
        public Nullable<int> maxCutRateDisc { get; set; }
        public Nullable<int> maxRoleRateDisc { get; set; }
        public Nullable<decimal> flatRate { get; set; }
        public Nullable<int> maxflatCutRateDisc { get; set; }
        public Nullable<int> maxflatRoleRateDisc { get; set; }
        public Nullable<decimal> custRatePerSqFeet { get; set; }
        public Nullable<decimal> purchaseRatePerMM { get; set; }
        public Nullable<decimal> sellingRatePerMM { get; set; }
        public Nullable<int> maxDiscout { get; set; }
        

        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMHsn MstHsn { get; set; }
    }
}