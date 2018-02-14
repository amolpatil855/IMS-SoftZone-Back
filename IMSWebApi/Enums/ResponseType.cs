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
        Generated = 1,
        Approve = 2,
        PartialPending = 3,
        Cancel = 4,
        Complete = 5
    }

    public enum SaleOrderStatus
    {
        Generated = 1,
        Approve = 2,
        PartialPending = 3,
        Cancel = 4,
        Complete = 5
    }
}