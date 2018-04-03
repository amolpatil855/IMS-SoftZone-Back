using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMvwDashboard
    {
        public Nullable<long> totalOutStandingAmount { get; set; }
        public Nullable<int> totalSalesCount { get; set; }
        public int totalPaymentDue { get; set; }
        public Nullable<int> totalPurchaseCount { get; set; }
        public Nullable<long> ytdSale { get; set; }
        public Nullable<long> currentMonthSale { get; set; }
        public Nullable<int> itemsCountBelowReorderLevel { get; set; }
        public Nullable<long> totalStockValue { get; set; }
    }
}