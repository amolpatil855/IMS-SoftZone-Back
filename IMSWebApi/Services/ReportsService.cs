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
    public class ReportsService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public ReportsService()
        {
        }

        public ListResult<VMvwAccessory> getAccessoryProducts(int pageSize, int page)
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

        public ListResult<VMvwFWR> getFabricProducts(int pageSize, int page)
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

        public ListResult<VMvwFoam> getFoamProducts(int pageSize, int page)
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

        public ListResult<VMvwFWR> getRugProducts(int pageSize, int page)
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

        public ListResult<VMvwFWR> getWallpaperProducts(int pageSize, int page)
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

        public ListResult<VMvwMattress> getMattressProducts(int pageSize, int page)
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

        //Category Wise List of Products for Export
        public List<VMvwAccessory> getAccessoryProductsForExport()
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

        public List<VMvwFWR> getFabricProductsForExport()
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

        public List<VMvwFoam> getFoamProductsForExport()
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

        public List<VMvwFWR> getRugProductsForExport()
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

        public List<VMvwFWR> getWallpaperProductsForExport()
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

        public List<VMvwMattress> getMattressProductsForExport()
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
    }
}