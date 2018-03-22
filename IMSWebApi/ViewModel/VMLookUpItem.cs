﻿using System;
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

    public class VMLookUpItemForMatSizeCode : IEqualityComparer<VMLookUpItemForMatSizeCode>
    {
        public string value { get; set; }
        public string label { get; set; }

        public bool Equals(VMLookUpItemForMatSizeCode x, VMLookUpItemForMatSizeCode y)
        {
            return x.value.Equals(y.value);
        }

        public int GetHashCode(VMLookUpItemForMatSizeCode obj)
        {
            return obj.value.GetHashCode();
        }
    }

}