using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Common
{
    public class ListResult<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
    }
}