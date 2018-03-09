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
        Completed = 5,
        Cancelled = 6
    }


    public enum SaleOrderStatus
    {
        Created = 1,
        Approved = 2,
        PartialCompleted = 3,
        Closed = 4,
        Completed = 5,
        Cancelled = 6
    }

    public enum GINStatus
    {
        Created = 1,
        Completed = 2,
        Cancelled = 3
    }

    public enum InvoiceStatus
    {
        Created = 1,
        Approved = 2
    }
}