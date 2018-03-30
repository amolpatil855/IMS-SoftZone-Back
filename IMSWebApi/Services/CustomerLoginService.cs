using AutoMapper;
using IMSWebApi.Common;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IMSWebApi.Services
{
    public class CustomerLoginService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public CustomerLoginService()
        {
        }

        public ListResult<VMvwAccessory> getAccessoryProducts(int pageSize, int page)
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

        public ListResult<VMvwFWR> getFabricProducts(int pageSize, int page)
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

        public ListResult<VMvwFoam> getFoamProducts(int pageSize, int page)
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
                        hsnWithGST = f.hsnCode + " (" + f.gst +")"
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

        public ListResult<VMvwFWR> getRugProducts(int pageSize, int page)
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

        public ListResult<VMvwFWR> getWallpaperProducts(int pageSize, int page)
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

        public ListResult<VMvwMattress> getMattressProducts(int pageSize, int page)
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

        public List<VMvwFWR> getRugProductsForExport()
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

        public List<VMvwFWR> getWallpaperProductsForExport()
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

        public List<VMvwMattress> getMattressProductsForExport()
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
    }
}