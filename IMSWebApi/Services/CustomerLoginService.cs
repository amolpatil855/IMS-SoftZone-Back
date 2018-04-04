using AutoMapper;
using IMSWebApi.Common;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.ViewModel.SalesInvoice;
using IMSWebApi.ViewModel.SlaesOrder;

namespace IMSWebApi.Services
{
    public class CustomerLoginService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public CustomerLoginService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMvwAccessory> getAccessoryProducts(int pageSize, int page, string search)
        {
            List<VMvwAccessory> accessoryProductsView;
            accessoryProductsView = repo.vwAccessories.Where(a => (!string.IsNullOrEmpty(search) ?
                                            a.itemCode.StartsWith(search)
                                            || a.name.StartsWith(search)
                                            || a.size.StartsWith(search)
                                            || a.uom.StartsWith(search)
                                            || a.hsnCode.StartsWith(search)
                                            || a.sellingRate.ToString().StartsWith(search)
                                            || a.sellingRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? a.availableStock > 0 : search.ToLower().Equals("no") ? a.availableStock <= 0 : false) : true))
                    .Select(a => new VMvwAccessory
                    {
                        Category = a.Category,
                        itemCode = a.itemCode,
                        name = a.name,
                        size = a.size,
                        uom = a.uom,
                        hsnCode = a.hsnCode,
                        gst = a.gst,
                        sellingRate = a.sellingRate,
                        sellingRateWithGst = a.sellingRateWithGst,
                        availableStock = a.availableStock > 0 ? a.availableStock : 0,
                        hsnWithGST = a.hsnCode + " (" + a.gst + ")"
                    })
                    .OrderBy(q => q.itemCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();
            
            return new ListResult<VMvwAccessory>
            {
                Data = accessoryProductsView,
                TotalCount = repo.vwAccessories.Where(a => (!string.IsNullOrEmpty(search) ?
                                            a.itemCode.StartsWith(search)
                                            || a.name.StartsWith(search)
                                            || a.size.StartsWith(search)
                                            || a.uom.StartsWith(search)
                                            || a.hsnCode.StartsWith(search)
                                            || a.sellingRate.ToString().StartsWith(search)
                                            || a.sellingRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? a.availableStock > 0 : search.ToLower().Equals("no") ? a.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getFabricProducts(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? designId, Int64? shadeId)
        {
            List<VMvwFWR> fabricProductsView;
            
            fabricProductsView = repo.vwFWRs.Where(f => f.Category.Equals("Fabric") && f.flatRate != null
                                                     && (collectionId != null ? f.collectionId == collectionId : true)
                                                    && (qualityId != null ? f.qualityId == qualityId : true)
                                                    && (designId != null ? f.designId == designId : true)
                                                    && (shadeId != null ? f.shadeId == shadeId : true)
                                                     && (!string.IsNullOrEmpty(search) ?
                                                    f.Collection.StartsWith(search)
                                                    || f.QDS.StartsWith(search)
                                                    || f.serialNumber.ToString().StartsWith(search)
                                                    || f.uom.StartsWith(search)
                                                    || f.hsnCode.StartsWith(search)
                                                    || f.width.ToString().StartsWith(search)
                                                    || f.flatRate.ToString().StartsWith(search)
                                                    || f.flatRateWithGst.ToString().StartsWith(search)
                                                    || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true))
                    .Select(f => new VMvwFWR
                    {
                        Category = f.Category,
                        Collection = f.Collection,
                        QDS = f.QDS,
                        serialNumber = f.serialNumber,
                        uom = f.uom,
                        hsnCode = f.hsnCode,
                        gst = f.gst,
                        width = f.width,
                        size = f.size,
                        flatRate = f.flatRate,
                        flatRateWithGst = f.flatRateWithGst,
                        availableStock = f.availableStock > 0 ? f.availableStock : 0,
                        hsnWithGST = f.hsnCode + " (" + f.gst + ")"
                    })
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();
                    
            return new ListResult<VMvwFWR>
            {
                Data = fabricProductsView,
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Fabric") && f.flatRate != null
                                                     && (!string.IsNullOrEmpty(search) ?
                                                    f.Collection.StartsWith(search)
                                                    || f.QDS.StartsWith(search)
                                                    || f.serialNumber.ToString().StartsWith(search)
                                                    || f.uom.StartsWith(search)
                                                    || f.hsnCode.StartsWith(search)
                                                    || f.width.ToString().StartsWith(search)
                                                    || f.flatRate.ToString().StartsWith(search)
                                                    || f.flatRateWithGst.ToString().StartsWith(search)
                                                    || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFoam> getFoamProducts(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? densityId, Int64? fomSuggestedMMId, Int64? fomSizeId)
        {   
            List<VMvwFoam> foamProductView;
            foamProductView = repo.vwFoams.Where(f => (collectionId != null ? f.collectionId == collectionId : true)
                                            && (qualityId != null ? f.qualityId == qualityId : true)
                                            && (densityId != null ? f.desityId == densityId : true)
                                            && (fomSuggestedMMId != null ? f.fomSuggestedMMId == fomSuggestedMMId : true)
                                            && (fomSizeId != null ? f.sizeId == fomSizeId : true)
                                            && (!string.IsNullOrEmpty(search) ?
                                            f.Collection.StartsWith(search)
                                            || f.qualityCode.StartsWith(search)
                                            || f.density.ToString().StartsWith(search)
                                            || f.sizeCode.StartsWith(search)
                                            || f.itemCode.StartsWith(search)
                                            || f.uom.StartsWith(search)
                                            || f.hsnCode.StartsWith(search)
                                            || f.sellingRatePerMM.ToString().StartsWith(search)
                                            || f.sellingRatePerMMWithGst.ToString().StartsWith(search)
                                            || f.sellingRatePerKG.ToString().StartsWith(search)
                                            || f.sellingRatePerKGWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true))
                    .Select(f => new VMvwFoam
                    {
                        Category = f.Category,
                        Collection = f.Collection,
                        qualityCode = f.qualityCode,
                        density = f.density,
                        sizeCode = f.sizeCode,
                        itemCode = f.itemCode,
                        uom = f.uom,
                        hsnCode = f.hsnCode,
                        gst = f.gst,
                        sellingRatePerKG = f.sellingRatePerKG,
                        sellingRatePerKGWithGst = f.sellingRatePerKGWithGst,
                        sellingRatePerMM = f.sellingRatePerMM,
                        sellingRatePerMMWithGst = f.sellingRatePerMMWithGst,
                        availableStock = f.availableStock > 0 ? f.availableStock : 0,
                        hsnWithGST = f.hsnCode + " (" + f.gst +")"
                    })
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMvwFoam>
            {
                Data = foamProductView,
                TotalCount = repo.vwFoams.Where(f => (collectionId != null ? f.collectionId == collectionId : true)
                                            && (qualityId != null ? f.qualityId == qualityId : true)
                                            && (densityId != null ? f.desityId == densityId : true)
                                            && (fomSuggestedMMId != null ? f.fomSuggestedMMId == fomSuggestedMMId : true)
                                            && (fomSizeId != null ? f.sizeId == fomSizeId : true)
                                            && (!string.IsNullOrEmpty(search) ?
                                            f.Collection.StartsWith(search)
                                            || f.qualityCode.StartsWith(search)
                                            || f.density.ToString().StartsWith(search)
                                            || f.sizeCode.StartsWith(search)
                                            || f.itemCode.StartsWith(search)
                                            || f.uom.StartsWith(search)
                                            || f.hsnCode.StartsWith(search)
                                            || f.sellingRatePerMM.ToString().StartsWith(search)
                                            || f.sellingRatePerMMWithGst.ToString().StartsWith(search)
                                            || f.sellingRatePerKG.ToString().StartsWith(search)
                                            || f.sellingRatePerKGWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

      
        //Category Wise List of Products for Export
        public List<VMvwAccessory> getAccessoryProductsForExport()
        {
            List<VMvwAccessory> accessoryProductsView;
            accessoryProductsView = repo.vwAccessories
                    .Select(a => new VMvwAccessory
                    {
                        Category = a.Category,
                        itemCode = a.itemCode,
                        name = a.name,
                        size = a.size,
                        uom = a.uom,
                        hsnCode = a.hsnCode,
                        gst = a.gst,
                        sellingRate = a.sellingRate,
                        sellingRateWithGst = a.sellingRateWithGst,
                        availableStock = a.availableStock > 0 ? a.availableStock : 0,
                        hsnWithGST = a.hsnCode + " (" + a.gst + ")"
                    })
                    .OrderBy(q => q.itemCode).ToList();
                    
            return accessoryProductsView;
        }

        public List<VMvwFWR> getFabricProductsForExport()
        {

            List<VMvwFWR> fabricProductsView;
            fabricProductsView = repo.vwFWRs.Where(f => f.Category.Equals("Fabric") && f.flatRate != null)
                    .Select(f => new VMvwFWR
                    {
                        Category = f.Category,
                        Collection = f.Collection,
                        QDS = f.QDS,
                        serialNumber = f.serialNumber,
                        uom = f.uom,
                        hsnCode = f.hsnCode,
                        gst = f.gst,
                        width = f.width,
                        size = f.size,
                        rrp = f.rrp,
                        rrpWithGst = f.rrpWithGst,
                        flatRate = f.flatRate,
                        flatRateWithGst = f.flatRateWithGst,
                        availableStock = f.availableStock > 0 ? f.availableStock : 0,
                        hsnWithGST = f.hsnCode + " (" + f.gst + ")"
                    })
                    .OrderBy(q => q.Collection)
                    .ToList();

            return fabricProductsView;
        }

        public List<VMvwFoam> getFoamProductsForExport()
        {
            List<VMvwFoam> foamProductView;
            foamProductView = repo.vwFoams
                    .Select(f => new VMvwFoam
                    {
                        Category = f.Category,
                        Collection = f.Collection,
                        qualityCode = f.qualityCode,
                        density = f.density,
                        sizeCode = f.sizeCode,
                        itemCode = f.itemCode,
                        uom = f.uom,
                        hsnCode = f.hsnCode,
                        gst = f.gst,
                        sellingRatePerKG = f.sellingRatePerKG,
                        sellingRatePerKGWithGst = f.sellingRatePerKGWithGst,
                        sellingRatePerMM = f.sellingRatePerMM,
                        sellingRatePerMMWithGst = f.sellingRatePerMMWithGst,
                        availableStock = f.availableStock > 0 ? f.availableStock : 0,
                        hsnWithGST = f.hsnCode + " (" + f.gst + ")"
                    })
                    .OrderBy(q => q.Collection)
                    .ToList();
           
            return foamProductView;
        }

        //DashBoard data

        public VMvwDashboard getDashboardData()
        {
            VMvwDashboard dashboardData = new VMvwDashboard();
            Int64 customerId = repo.MstCustomers.Where(c => c.userId == _LoggedInuserId).FirstOrDefault().id;
            int totalSalesCount = repo.TrnSaleOrders.Where(so => so.customerId == customerId && !(so.status.Equals("Completed"))).Count();
            long totalOutstandingAmount = repo.TrnSalesInvoices.Where(si => (si.isPaid == false) 
                                                                  && (si.TrnSaleOrder != null ? si.TrnSaleOrder.customerId == customerId : false) 
                                                                  && (si.status.Equals("Approved")))
                                                               .Select(si=>si.totalAmount).DefaultIfEmpty(0).Sum();
            dashboardData.totalSalesCount = totalSalesCount;
            dashboardData.totalOutStandingAmount = totalOutstandingAmount;
            return dashboardData;
        }

        public ListResult<VMTrnSalesInvoiceList> getRecordsForTotalOutstandingAmt(int pageSize, int page, string search)
        {
            List<VMTrnSalesInvoiceList> salesInvoiceView;
            Int64 customerId = repo.MstCustomers.Where(c => c.userId == _LoggedInuserId).FirstOrDefault().id;
            var result = repo.TrnSalesInvoices
                      .Where(s => (s.isPaid == false) 
                          && (s.TrnSaleOrder != null ? s.TrnSaleOrder.customerId == customerId : false) 
                          && (s.status.Equals("Approved")) 
                          && (!string.IsNullOrEmpty(search) 
                          ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                        || s.invoiceNumber.StartsWith(search)
                        || s.totalAmount.ToString().StartsWith(search)
                        || s.courierDockYardNumber.StartsWith(search)
                        || s.status.StartsWith(search)
                        || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true))
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
                        .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            salesInvoiceView = result;

            return new ListResult<VMTrnSalesInvoiceList>
            {
                Data = salesInvoiceView,
                TotalCount = repo.TrnSalesInvoices
                      .Where(s => (s.isPaid == false)
                          && (s.TrnSaleOrder != null ? s.TrnSaleOrder.customerId == customerId : false)
                          && (s.status.Equals("Approved"))
                          && (!string.IsNullOrEmpty(search)
                          ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                        || s.invoiceNumber.StartsWith(search)
                        || s.totalAmount.ToString().StartsWith(search)
                        || s.courierDockYardNumber.StartsWith(search)
                        || s.status.StartsWith(search)
                        || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true))
                    .Count(),
                Page = page
            };
        }

        public ListResult<VMTrnSaleOrderList> getRecordsForSOCount(int pageSize, int page, string search)
        {
            List<VMTrnSaleOrderList> saleOrderView;
            Int64 customerId = repo.MstCustomers.Where(c => c.userId == _LoggedInuserId).FirstOrDefault().id;
            var result = repo.TrnSaleOrders
                        .Where(so => !(so.status.Equals("Completed"))
                            && so.customerId == customerId
                            && (!string.IsNullOrEmpty(search)
                            ? so.orderNumber.StartsWith(search)
                            || so.MstCustomer.name.StartsWith(search)
                            || so.MstCourier.name.StartsWith(search)
                            || so.MstAgent.name.StartsWith(search)
                            || so.status.StartsWith(search)
                            || so.totalAmount.ToString().StartsWith(search) : true))
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
                        .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            saleOrderView = result;

            return new ListResult<VMTrnSaleOrderList>
            {
                Data = saleOrderView,
                TotalCount = repo.TrnSaleOrders
                        .Where(so => !(so.status.Equals("Completed"))
                            && so.customerId == customerId
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
    }
}