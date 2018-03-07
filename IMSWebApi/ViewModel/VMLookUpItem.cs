using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMLookUpItem : IEqualityComparer<VMLookUpItem>
    {
        public long value { get; set; }
        public string label { get; set; }

        public bool Equals(VMLookUpItem x, VMLookUpItem y)
        {
            return x.value.Equals(y.value);
        }

        public int GetHashCode(VMLookUpItem obj)
        {
            return (int)obj.value;
        }
    }

}