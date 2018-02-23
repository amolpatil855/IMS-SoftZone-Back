using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Enums
{
    public enum ResponseType
    {
        Success = 1, Warning = 2, Error = 3
    }

    public enum PurchaseOrderStatus
    {
        Created = 1,
        Approved = 2,
        PartialCompleted = 3,
        Closed = 4,
        Completed = 5
    }

    public enum SaleOrderStatus
    {
        Created = 1,
        Approved = 2,
        PartialCompleted = 3,
        Closed = 4,
        Completed = 5
    }
}