using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;

namespace IMSWebApi.Services
{
    public class ProductListService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public ProductListService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<vwAccessory> getAccessoryProducts(int pageSize, int page, string search)
        {
            List<vwAccessory> accessoryProductView;
            accessoryProductView = repo.vwAccessories.Where(a => !string.IsNullOrEmpty(search)
                    ? a.itemCode.StartsWith(search)
                    || a.name.StartsWith(search)
                    || a.size.StartsWith(search): true)
                    .OrderBy(q => q.itemCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<vwAccessory>
            {
                Data = accessoryProductView,
                TotalCount = repo.vwAccessories.Where(a => !string.IsNullOrEmpty(search)
                    ? a.itemCode.StartsWith(search)
                    || a.name.StartsWith(search)
                    || a.size.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public ListResult<vwFWR> getFabricProducts(int pageSize, int page, string search)
        {
            List<vwFWR> fabricProductView;
            fabricProductView = repo.vwFWRs.Where(f => !string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.QDS.StartsWith(search)
                    || f.serialNumber.ToString().StartsWith(search) : true
                    && f.Category.Equals("Fabric"))
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<vwFWR>
            {
                Data = fabricProductView,
                TotalCount = repo.vwFWRs.Where(f => !string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.QDS.StartsWith(search)
                    || f.serialNumber.ToString().StartsWith(search) : true
                    && f.Category.Equals("Fabric")).Count(),
                Page = page
            };
        }

        public ListResult<vwFWR> getRugProducts(int pageSize, int page, string search)
        {
            List<vwFWR> rugProductView;
            rugProductView = repo.vwFWRs.Where(f => !string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.QDS.StartsWith(search)
                    || f.serialNumber.ToString().StartsWith(search) : true
                    && f.Category.Equals("Rug"))
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<vwFWR>
            {
                Data = rugProductView,
                TotalCount = repo.vwFWRs.Where(f => !string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.QDS.StartsWith(search)
                    || f.serialNumber.ToString().StartsWith(search) : true
                    && f.Category.Equals("Rug")).Count(),
                Page = page
            };
        }

        public ListResult<vwFWR> getWallpaperProducts(int pageSize, int page, string search)
        {
            List<vwFWR> wallpaperProductView;
            wallpaperProductView = repo.vwFWRs.Where(f => !string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.QDS.StartsWith(search)
                    || f.serialNumber.ToString().StartsWith(search) : true
                    && f.Category.Equals("Wallpaper"))
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<vwFWR>
            {
                Data = wallpaperProductView,
                TotalCount = repo.vwFWRs.Where(f => !string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.QDS.StartsWith(search)
                    || f.serialNumber.ToString().StartsWith(search) : true
                    && f.Category.Equals("Wallpaper")).Count(),
                Page = page
            };
        }

        public ListResult<vwFoam> getFoamProducts(int pageSize, int page, string search)
        {
            List<vwFoam> foamProductView;
            foamProductView = repo.vwFoams.Where(f => !string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search)
                    || f.sizeCode.StartsWith(search)
                    || f.itemCode.StartsWith(search) : true)
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<vwFoam>
            {
                Data = foamProductView,
                TotalCount = repo.vwFoams.Where(f => !string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search)
                    || f.sizeCode.StartsWith(search)
                    || f.itemCode.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public ListResult<vwMattress> getMattressProducts(int pageSize, int page, string search)
        {
            List<vwMattress> mattressProductView;
            mattressProductView = repo.vwMattresses.Where(m => !string.IsNullOrEmpty(search)
                    ? m.Collection.StartsWith(search)
                    || m.qualityCode.StartsWith(search)
                    || m.thicknessCode.ToString().StartsWith(search)
                    || m.sizeCode.StartsWith(search) : true)
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<vwMattress>
            {
                Data = mattressProductView,
                TotalCount = repo.vwMattresses.Where(m => !string.IsNullOrEmpty(search)
                    ? m.Collection.StartsWith(search)
                    || m.qualityCode.StartsWith(search)
                    || m.thicknessCode.ToString().StartsWith(search)
                    || m.sizeCode.StartsWith(search) : true).Count(),
                Page = page
            };
        }
    }
}