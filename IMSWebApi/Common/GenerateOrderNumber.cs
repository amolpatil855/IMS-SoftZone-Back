using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Common
{
    public class GenerateOrderNumber
    {
        public string orderNumber(string startYear,string endYear,int orderNumber,string initials)
        {
            string result = initials + startYear + endYear + "-" + orderNumber.ToString().PadLeft(5, '0');
            return result;
        }
    }
}