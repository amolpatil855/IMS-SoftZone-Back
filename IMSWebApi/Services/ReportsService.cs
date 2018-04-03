﻿using AutoMapper;
using IMSWebApi.Common;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using IMSWebApi.ViewModel.SalesInvoice;
using IMSWebApi.ViewModel.SlaesOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IMSWebApi.Services
{
    public class ReportsService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public ReportsService()
        {
        }

        //Master Price List

        public ListResult<VMvwAccessory> getAccessoryProductsForML(int pageSize, int page)
        {
            StringBuilder stringBuilder;
            List<VMvwAccessory> accessoryProductView;
            var result = repo.vwAccessories
                    .OrderBy(q => q.itemCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();
            accessoryProductView = Mapper.Map<List<vwAccessory>, List<VMvwAccessory>>(result);
            accessoryProductView.ForEach(a => 
                {
                    stringBuilder = new StringBuilder();
                    a.availableStock = a.availableStock > 0 ? a.availableStock : 0;
                    a.hsnWithGST = stringBuilder.Append(a.hsnCode).Append(" (").Append(a.gst).Append(")").ToString();
                });

            return new ListResult<VMvwAccessory>
            {
                Data = accessoryProductView,
                TotalCount = repo.vwAccessories.Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getFabricProductsForML(int pageSize, int page)
        {
            StringBuilder stringBuilder;
            List<VMvwFWR> fabricProductView;
            var result = repo.vwFWRs.Where(f => f.Category.Equals("Fabric"))
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).Distinct().ToList();
            fabricProductView = Mapper.Map<List<vwFWR>, List<VMvwFWR>>(result);
            fabricProductView.ForEach(f =>
                {
                    stringBuilder = new StringBuilder();
                    f.availableStock = f.availableStock > 0 ? f.availableStock : 0;
                    f.hsnWithGST = stringBuilder.Append(f.hsnCode).Append(" (").Append(f.gst).Append(")").ToString();
                });
            return new ListResult<VMvwFWR>
            {
                Data = fabricProductView,
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Fabric")).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFoam> getFoamProductsForML(int pageSize, int page)
        {
            StringBuilder stringBuilder;
            List<VMvwFoam> foamProductView;
            var result = repo.vwFoams
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();
            foamProductView = Mapper.Map<List<vwFoam>, List<VMvwFoam>>(result);
            foamProductView.ForEach(f =>
                {
                    stringBuilder = new StringBuilder();
                    f.availableStock = f.availableStock > 0 ? f.availableStock : 0;
                    f.hsnWithGST = stringBuilder.Append(f.hsnCode).Append(" (").Append(f.gst).Append(")").ToString();
                });
            return new ListResult<VMvwFoam>
            {
                Data = foamProductView,
                TotalCount = repo.vwFoams.Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getRugProductsForML(int pageSize, int page)
        {
            StringBuilder stringBuilder;
            List<VMvwFWR> rugProductView;
            var result = repo.vwFWRs.Where(f => f.Category.Equals("Rug"))
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).Distinct().ToList();
            rugProductView = Mapper.Map<List<vwFWR>, List<VMvwFWR>>(result);
            rugProductView.ForEach(f =>
                {
                    stringBuilder = new StringBuilder();
                    f.availableStock = f.availableStock > 0 ? f.availableStock : 0;
                    f.hsnWithGST = stringBuilder.Append(f.hsnCode).Append(" (").Append(f.gst).Append(")").ToString();
                });
            return new ListResult<VMvwFWR>
            {
                Data = rugProductView,
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Rug")).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getWallpaperProductsForML(int pageSize, int page)
        {
            StringBuilder stringBuilder;
            List<VMvwFWR> wallpaperProductView;
            var result = repo.vwFWRs.Where(f => f.Category.Equals("Wallpaper"))
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).Distinct().ToList();
            wallpaperProductView = Mapper.Map<List<vwFWR>, List<VMvwFWR>>(result);
            wallpaperProductView.ForEach(f => 
                {
                    stringBuilder = new StringBuilder();
                    f.availableStock = f.availableStock > 0 ? f.availableStock : 0;
                    f.hsnWithGST = stringBuilder.Append(f.hsnCode).Append(" (").Append(f.gst).Append(")").ToString();
                });
            return new ListResult<VMvwFWR>
            {
                Data = wallpaperProductView,
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Wallpaper")).Count(),
                Page = page
            };
        }

        public ListResult<VMvwMattress> getMattressProductsForML(int pageSize, int page)
        {
            StringBuilder stringBuilder;
            List<VMvwMattress> mattressProductView;
            var result = repo.vwMattresses
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).Distinct().ToList();
            mattressProductView = Mapper.Map<List<vwMattress>, List<VMvwMattress>>(result);
            mattressProductView.ForEach(m =>
                {
                    stringBuilder = new StringBuilder();
                    m.availableStock = m.availableStock > 0 ? m.availableStock : 0;
                    m.hsnWithGST = stringBuilder.Append(m.hsnCode).Append(" (").Append(m.gst).Append(")").ToString();
                });
            return new ListResult<VMvwMattress>
            {
                Data = mattressProductView,
                TotalCount = repo.vwMattresses.Count(),
                Page = page
            };
        }

        //Category Wise List of Products for Master List Export
        public List<VMvwAccessory> getAccessoryProductsForMLExport()
        {
            StringBuilder stringBuilder;
            List<VMvwAccessory> accessoryProductView;
            var result = repo.vwAccessories
                    .OrderBy(q => q.itemCode)
                    .ToList();
            accessoryProductView = Mapper.Map<List<vwAccessory>, List<VMvwAccessory>>(result);
            accessoryProductView.ForEach(a =>
            {
                stringBuilder = new StringBuilder();
                a.availableStock = a.availableStock > 0 ? a.availableStock : 0;
                a.hsnWithGST = stringBuilder.Append(a.hsnCode).Append(" (").Append(a.gst).Append(")").ToString();
            });
            return accessoryProductView;
        }

        public List<VMvwFWR> getFabricProductsForMLExport()
        {
            StringBuilder stringBuilder;
            List<VMvwFWR> fabricProductView;
            var result = repo.vwFWRs.Where(f => f.Category.Equals("Fabric"))
                    .OrderBy(q => q.Collection).Distinct().ToList();
            fabricProductView = Mapper.Map<List<vwFWR>, List<VMvwFWR>>(result);
            fabricProductView.ForEach(f =>
            {
                stringBuilder = new StringBuilder();
                f.availableStock = f.availableStock > 0 ? f.availableStock : 0;
                f.hsnWithGST = stringBuilder.Append(f.hsnCode).Append(" (").Append(f.gst).Append(")").ToString();
            });
            return fabricProductView;
        }

        public List<VMvwFoam> getFoamProductsForMLExport()
        {
            StringBuilder stringBuilder;
            List<VMvwFoam> foamProductView;
            var result = repo.vwFoams
                    .OrderBy(q => q.Collection).ToList();
            foamProductView = Mapper.Map<List<vwFoam>, List<VMvwFoam>>(result);
            foamProductView.ForEach(f =>
            {
                stringBuilder = new StringBuilder();
                f.availableStock = f.availableStock > 0 ? f.availableStock : 0;
                f.hsnWithGST = stringBuilder.Append(f.hsnCode).Append(" (").Append(f.gst).Append(")").ToString();
            });
            return foamProductView;
        }

        public List<VMvwFWR> getRugProductsForMLExport()
        {
            StringBuilder stringBuilder;
            List<VMvwFWR> rugProductView;
            var result = repo.vwFWRs.Where(f => f.Category.Equals("Rug"))
                    .OrderBy(q => q.Collection).Distinct().ToList();
            rugProductView = Mapper.Map<List<vwFWR>, List<VMvwFWR>>(result);
            rugProductView.ForEach(f =>
            {
                stringBuilder = new StringBuilder();
                f.availableStock = f.availableStock > 0 ? f.availableStock : 0;
                f.hsnWithGST = stringBuilder.Append(f.hsnCode).Append(" (").Append(f.gst).Append(")").ToString();
            });
            return rugProductView;
        }

        public List<VMvwFWR> getWallpaperProductsForMLExport()
        {
            StringBuilder stringBuilder;
            List<VMvwFWR> wallpaperProductView;
            var result = repo.vwFWRs.Where(f => f.Category.Equals("Wallpaper"))
                    .OrderBy(q => q.Collection).Distinct().ToList();
            wallpaperProductView = Mapper.Map<List<vwFWR>, List<VMvwFWR>>(result);
            wallpaperProductView.ForEach(f =>
            {
                stringBuilder = new StringBuilder();
                f.availableStock = f.availableStock > 0 ? f.availableStock : 0;
                f.hsnWithGST = stringBuilder.Append(f.hsnCode).Append(" (").Append(f.gst).Append(")").ToString();
            });
            return wallpaperProductView;
        }

        public List<VMvwMattress> getMattressProductsForMLExport()
        {
            StringBuilder stringBuilder;
            List<VMvwMattress> mattressProductView;
            var result = repo.vwMattresses
                    .OrderBy(q => q.Collection).Distinct().ToList();
            mattressProductView = Mapper.Map<List<vwMattress>, List<VMvwMattress>>(result);
            mattressProductView.ForEach(m =>
            {
                stringBuilder = new StringBuilder();
                m.availableStock = m.availableStock > 0 ? m.availableStock : 0;
                m.hsnWithGST = stringBuilder.Append(m.hsnCode).Append(" (").Append(m.gst).Append(")").ToString();
            });
            return mattressProductView;
        }

        //Client List

        public ListResult<VMvwAccessory> getAccessoryProductsForCL(int pageSize, int page)
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
                    .OrderBy(q => q.itemCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMvwAccessory>
            {
                Data = accessoryProductsView,
                TotalCount = repo.vwAccessories.Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getFabricProductsForCL(int pageSize, int page)
        {
            List<VMvwFWR> fabricProductsView;

            fabricProductsView = repo.vwFWRs.Where(f => f.Category.Equals("Fabric"))
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
                    .Skip(page * pageSize).Take(pageSize).Distinct().ToList();

            return new ListResult<VMvwFWR>
            {
                Data = fabricProductsView,
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Fabric")).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFoam> getFoamProductsForCL(int pageSize, int page)
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
                    .Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMvwFoam>
            {
                Data = foamProductView,
                TotalCount = repo.vwFoams.Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getRugProductsForCL(int pageSize, int page)
        {
            List<VMvwFWR> rugProductView;
            rugProductView = repo.vwFWRs.Where(f => f.Category.Equals("Rug"))
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
                    .Skip(page * pageSize).Take(pageSize).Distinct().ToList();

            return new ListResult<VMvwFWR>
            {
                Data = rugProductView,
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Rug")).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getWallpaperProductsForCL(int pageSize, int page)
        {
            List<VMvwFWR> wallpaperProductView;
            wallpaperProductView = repo.vwFWRs.Where(f => f.Category.Equals("Wallpaper"))
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
                    .Skip(page * pageSize).Take(pageSize).Distinct().ToList();

            return new ListResult<VMvwFWR>
            {
                Data = wallpaperProductView,
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Wallpaper")).Count(),
                Page = page
            };
        }

        public ListResult<VMvwMattress> getMattressProductsForCL(int pageSize, int page)
        {
            List<VMvwMattress> mattressProductView;
            mattressProductView = repo.vwMattresses
                    .OrderBy(q => q.Collection)
                    .Select(m => new VMvwMattress
                    {
                        Category = m.Category,
                        Collection = m.Collection,
                        qualityCode = m.qualityCode,
                        thicknessCode = m.thicknessCode,
                        sizeCode = m.sizeCode,
                        uom = m.uom,
                        hsnCode = m.hsnCode,
                        gst = m.gst,
                        rate = m.rate,
                        rateWithGst = m.rateWithGst,
                        customRatePerSqFeet = m.customRatePerSqFeet,
                        availableStock = m.availableStock > 0 ? m.availableStock : 0,
                        hsnWithGST = m.hsnCode + " (" + m.gst + ")"
                    })
                    .Skip(page * pageSize).Take(pageSize).Distinct().ToList();

            return new ListResult<VMvwMattress>
            {
                Data = mattressProductView,
                TotalCount = repo.vwMattresses.Count(),
                Page = page
            };
        }

        //Category Wise List of Products for Client List Export
        public List<VMvwAccessory> getAccessoryProductsForCLExport()
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

        public List<VMvwFWR> getFabricProductsForCLExport()
        {

            List<VMvwFWR> fabricProductsView;
            fabricProductsView = repo.vwFWRs.Where(f => f.Category.Equals("Fabric"))
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
                    .Distinct().ToList();

            return fabricProductsView;
        }

        public List<VMvwFoam> getFoamProductsForCLExport()
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

        public List<VMvwFWR> getRugProductsForCLExport()
        {
            List<VMvwFWR> rugProductView;
            rugProductView = repo.vwFWRs.Where(f => f.Category.Equals("Rug"))
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
                    .Distinct().ToList();
            return rugProductView;
        }

        public List<VMvwFWR> getWallpaperProductsForCLExport()
        {
            List<VMvwFWR> wallpaperProductView;
            wallpaperProductView = repo.vwFWRs.Where(f => f.Category.Equals("Wallpaper"))
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
                    .Distinct().ToList();
            return wallpaperProductView;
        }

        public List<VMvwMattress> getMattressProductsForCLExport()
        {
            List<VMvwMattress> mattressProductView;
            mattressProductView = repo.vwMattresses
                    .OrderBy(q => q.Collection)
                    .Select(m => new VMvwMattress
                    {
                        Category = m.Category,
                        Collection = m.Collection,
                        qualityCode = m.qualityCode,
                        thicknessCode = m.thicknessCode,
                        sizeCode = m.sizeCode,
                        uom = m.uom,
                        hsnCode = m.hsnCode,
                        gst = m.gst,
                        rate = m.rate,
                        rateWithGst = m.rateWithGst,
                        customRatePerSqFeet = m.customRatePerSqFeet,
                        availableStock = m.availableStock > 0 ? m.availableStock : 0,
                        hsnWithGST = m.hsnCode + " (" + m.gst + ")"
                    })
                    .Distinct().ToList();
            return mattressProductView;
        }

        //PO order status report
        public ListResult<VMTrnPurchaseOrderList> getPOorderStatusReport(int pageSize, int page, string status)
        {
            List<VMTrnPurchaseOrderList> purchaseOrderListView;
            purchaseOrderListView = repo.TrnPurchaseOrders.Where(po => po.status.Equals(status))
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
                    .OrderByDescending(o=>o.id)
                    .Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMTrnPurchaseOrderList>
            {
                Data = purchaseOrderListView,
                TotalCount = repo.TrnPurchaseOrders.Where(po=>po.status.Equals(status)).Count(),
                Page = page
            };
        }

        //SO order status report
        public ListResult<VMTrnSaleOrderList> getSOorderStatusReport(int pageSize, int page, string status)
        {
            List<VMTrnSaleOrderList> saleOrderListView;
            saleOrderListView = repo.TrnSaleOrders.Where(so => so.status.Equals(status))
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
                    .OrderByDescending(o => o.id)
                    .Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMTrnSaleOrderList>
            {
                Data = saleOrderListView,
                TotalCount = repo.TrnSaleOrders.Where(so => so.status.Equals(status)).Count(),
                Page = page
            };
        }

        //Sales Invoice Status and Payment Report
        public ListResult<VMTrnSalesInvoiceList> getSalesInvoicePaymentStatusReport(int pageSize, int page, string status, bool isPaid)
        {
            List<VMTrnSalesInvoiceList> salesInvoiceView;
            var result = repo.TrnSalesInvoices
                    .Where(si => si.status.Equals(status) && si.isPaid == isPaid)
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
                    .Where(si => si.status.Equals(status) && si.isPaid == isPaid).Count(),
                Page = page
            };
        }
    }
}