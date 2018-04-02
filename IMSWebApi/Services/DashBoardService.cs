﻿using IMSWebApi.Common;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using IMSWebApi.ViewModel.SalesInvoice;
using IMSWebApi.ViewModel.SlaesOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Services
{
    public class DashboardService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public DashboardService()
        {  
        }

        public vwDasBoard getDashboardData()
        {
            return repo.vwDasBoards.FirstOrDefault();
        }

        public ListResult<VMTrnSalesInvoiceList> getRecordsForTotalOutstandingAmt(int pageSize, int page, string search)
        {
            List<VMTrnSalesInvoiceList> salesInvoiceView;
            var result = repo.TrnSalesInvoices
                    .Select(s => new VMTrnSalesInvoiceList
                    {
                        id = s.id,
                        invoiceNumber = s.invoiceNumber,
                        invoiceDate = s.invoiceDate,
                        ginNumber = s.TrnGoodIssueNote.ginNumber,
                        totalAmount = s.totalAmount,
                        status = s.status,
                        courierDockYardNumber = s.courierDockYardNumber,
                        isPaid = s.isPaid
                    })
                    .Where(s => (!string.IsNullOrEmpty(search)
                    ? s.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.totalAmount.ToString().StartsWith(search)
                    || s.courierDockYardNumber.StartsWith(search)
                    || s.status.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true) 
                    && s.isPaid == false)
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            salesInvoiceView = result;

            return new ListResult<VMTrnSalesInvoiceList>
            {
                Data = salesInvoiceView,
                TotalCount = repo.TrnSalesInvoices.Where(s => (!string.IsNullOrEmpty(search)
                    ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.totalAmount.ToString().StartsWith(search)
                    || s.courierDockYardNumber.StartsWith(search)
                    || s.status.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true)
                    && s.isPaid == false)
                    .Count(),
                Page = page
            };
        }

        public ListResult<VMTrnSaleOrderList> getRecordsForSOCount(int pageSize, int page, string search)
        {
            List<VMTrnSaleOrderList> saleOrderView;
            var result = repo.TrnSaleOrders
                        .Select(so => new VMTrnSaleOrderList
                        {
                            id = so.id,
                            orderNumber = so.orderNumber,
                            orderDate = so.orderDate,
                            customerName = so.MstCustomer != null ? so.MstCustomer.name : string.Empty,
                            courierName = so.MstCourier != null ? so.MstCourier.name : string.Empty,
                            agentName = so.MstAgent != null ? so.MstAgent.name : string.Empty,
                            status = so.status,
                            totalAmount = so.totalAmount
                        })
                        .Where(so =>(so.status.Equals("Approved") || so.status.Equals("Created"))
                            && (!string.IsNullOrEmpty(search)
                            ? so.orderNumber.StartsWith(search)
                            || so.customerName.StartsWith(search)
                            || so.courierName.StartsWith(search)
                            || so.agentName.StartsWith(search)
                            || so.status.StartsWith(search)
                            || so.totalAmount.ToString().StartsWith(search) : true))
                            .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            saleOrderView = result;

            return new ListResult<VMTrnSaleOrderList>
            {
                Data = saleOrderView,
                TotalCount = repo.TrnSaleOrders.Where(so => (so.status.Equals("Approved") || so.status.Equals("Created"))
                            && (!string.IsNullOrEmpty(search)
                            ? so.orderNumber.StartsWith(search)
                            || so.MstCustomer.name.StartsWith(search)
                            || so.MstCourier.name.StartsWith(search)
                            || so.MstAgent.name.StartsWith(search)
                            || so.status.StartsWith(search)
                            || so.totalAmount.ToString().StartsWith(search) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMTrnPurchaseOrderList> getRecorsForPOCount(int pageSize, int page, string search)
        {
            List<VMTrnPurchaseOrderList> purchaseOrderListingView;
            purchaseOrderListingView = repo.TrnPurchaseOrders.Where(po => 
                    (po.status.Equals("Approved") || po.status.Equals("Created"))
                    && (!string.IsNullOrEmpty(search)
                    ? po.orderNumber.ToString().StartsWith(search)
                    || po.MstSupplier.code.ToString().StartsWith(search)
                    || po.MstCourier.name.StartsWith(search)
                    || po.courierMode.StartsWith(search)
                    || po.totalAmount.ToString().StartsWith(search)
                    || po.status.StartsWith(search) : true))
                    .Select(po => new VMTrnPurchaseOrderList
                    {
                        id = po.id,
                        orderNumber = po.orderNumber,
                        orderDate = po.orderDate,
                        supplierName = po.MstSupplier != null ? po.MstSupplier.code : null,
                        courierName = po.MstCourier != null ? po.MstCourier.name : null,
                        courierMode = po.courierMode,
                        totalAmount = po.totalAmount,
                        status = po.status
                    })
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMTrnPurchaseOrderList>
            {
                Data = purchaseOrderListingView,
                TotalCount = repo.TrnPurchaseOrders.Where(po => 
                    (po.status.Equals("Approved") || po.status.Equals("Created"))
                    && (!string.IsNullOrEmpty(search)
                    ? po.orderNumber.ToString().StartsWith(search)
                    || po.MstSupplier.name.ToString().StartsWith(search)
                    || po.MstCourier.name.StartsWith(search)
                    || po.courierMode.StartsWith(search)
                    || po.totalAmount.ToString().StartsWith(search)
                    || po.status.StartsWith(search) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMTrnSalesInvoiceList> getRecordsForYTDSale(int pageSize, int page, string search)
        {
            DateTime currentDate = DateTime.Now.Date;
            MstFinancialYear yearDates = repo.MstFinancialYears.Where(f => f.startDate <= currentDate && f.endDate >= currentDate).FirstOrDefault();
            List<VMTrnSalesInvoiceList> salesInvoiceView;
            var result = repo.TrnSalesInvoices
                    .Select(s => new VMTrnSalesInvoiceList
                    {
                        id = s.id,
                        invoiceNumber = s.invoiceNumber,
                        invoiceDate = s.invoiceDate,
                        ginNumber = s.TrnGoodIssueNote.ginNumber,
                        totalAmount = s.totalAmount,
                        status = s.status,
                        courierDockYardNumber = s.courierDockYardNumber,
                        isPaid = s.isPaid
                    })
                    .Where(s => (!string.IsNullOrEmpty(search)
                    ? s.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.totalAmount.ToString().StartsWith(search)
                    || s.courierDockYardNumber.StartsWith(search)
                    || s.status.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true)
                    && (s.invoiceDate >= yearDates.startDate && s.invoiceDate <= yearDates.endDate))
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            salesInvoiceView = result;

            return new ListResult<VMTrnSalesInvoiceList>
            {
                Data = salesInvoiceView,
                TotalCount = repo.TrnSalesInvoices.Where(s => (!string.IsNullOrEmpty(search)
                    ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.totalAmount.ToString().StartsWith(search)
                    || s.courierDockYardNumber.StartsWith(search)
                    || s.status.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true)
                     && (s.invoiceDate >= yearDates.startDate && s.invoiceDate <= yearDates.endDate))
                    .Count(),
                Page = page
            };
        }

        public ListResult<VMTrnSalesInvoiceList> getRecordsForCurrentMonthSale(int pageSize, int page, string search)
        {
            DateTime now = new DateTime(2018,03,28);
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
            List<VMTrnSalesInvoiceList> salesInvoiceView;
            var result = repo.TrnSalesInvoices
                    .Select(s => new VMTrnSalesInvoiceList
                    {
                        id = s.id,
                        invoiceNumber = s.invoiceNumber,
                        invoiceDate = s.invoiceDate,
                        ginNumber = s.TrnGoodIssueNote.ginNumber,
                        totalAmount = s.totalAmount,
                        status = s.status,
                        courierDockYardNumber = s.courierDockYardNumber,
                        isPaid = s.isPaid
                    })
                    .Where(s => (!string.IsNullOrEmpty(search)
                    ? s.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.totalAmount.ToString().StartsWith(search)
                    || s.courierDockYardNumber.StartsWith(search)
                    || s.status.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true)
                    && (s.invoiceDate >= startDate && s.invoiceDate <= endDate))
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            salesInvoiceView = result;

            return new ListResult<VMTrnSalesInvoiceList>
            {
                Data = salesInvoiceView,
                TotalCount = repo.TrnSalesInvoices.Where(s => (!string.IsNullOrEmpty(search)
                    ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.totalAmount.ToString().StartsWith(search)
                    || s.courierDockYardNumber.StartsWith(search)
                    || s.status.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true)
                     && (s.invoiceDate >= startDate && s.invoiceDate <= endDate))
                    .Count(),
                Page = page
            };
        }
    }
}