using AutoMapper;
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

        public ListResult<VMvwAccessory> getAccessoryProductsForML(int pageSize, int page, string search)
        {
            StringBuilder stringBuilder;
            List<VMvwAccessory> accessoryProductView;
            var result = repo.vwAccessories.Where(a => (!string.IsNullOrEmpty(search) ?
                                            a.itemCode.StartsWith(search)
                                            || a.name.StartsWith(search)
                                            || a.size.StartsWith(search)
                                            || a.uom.StartsWith(search)
                                            || a.hsnCode.StartsWith(search)
                                            || a.sellingRate.ToString().StartsWith(search)
                                            || a.sellingRateWithGst.ToString().StartsWith(search)
                                            || a.purchaseRate.ToString().StartsWith(search)
                                            || a.purchaseRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? a.availableStock > 0 : search.ToLower().Equals("no") ? a.availableStock <= 0 : false) : true))
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
                TotalCount = repo.vwAccessories.Where(a => (!string.IsNullOrEmpty(search) ?
                                            a.itemCode.StartsWith(search)
                                            || a.name.StartsWith(search)
                                            || a.size.StartsWith(search)
                                            || a.uom.StartsWith(search)
                                            || a.hsnCode.StartsWith(search)
                                            || a.sellingRate.ToString().StartsWith(search)
                                            || a.sellingRateWithGst.ToString().StartsWith(search)
                                            || a.purchaseRate.ToString().StartsWith(search)
                                            || a.purchaseRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? a.availableStock > 0 : search.ToLower().Equals("no") ? a.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getFabricProductsForML(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? designId, Int64? shadeId)
        {
            StringBuilder stringBuilder;
            List<VMvwFWR> fabricProductView;
            var result = repo.vwFWRs.Where(f => f.Category.Equals("Fabric")
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
                                            || f.cutRate.ToString().StartsWith(search)
                                            || f.cutRateWithGst.ToString().StartsWith(search)
                                            || f.roleRate.ToString().StartsWith(search)
                                            || f.rollRateWithGst.ToString().StartsWith(search)
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
                                            || f.flatRate.ToString().StartsWith(search)
                                            || f.flatRateWithGst.ToString().StartsWith(search)
                                            || f.purchaseFlatRate.ToString().StartsWith(search)
                                            || f.purchaseFlatRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0  : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true))
                                    .OrderBy(q => q.Collection)
                                    .Skip(page * pageSize).Take(pageSize).ToList();
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
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Fabric")
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
                                            || f.cutRate.ToString().StartsWith(search)
                                            || f.cutRateWithGst.ToString().StartsWith(search)
                                            || f.roleRate.ToString().StartsWith(search)
                                            || f.rollRateWithGst.ToString().StartsWith(search)
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
                                            || f.flatRate.ToString().StartsWith(search)
                                            || f.flatRateWithGst.ToString().StartsWith(search)
                                            || f.purchaseFlatRate.ToString().StartsWith(search)
                                            || f.purchaseFlatRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFoam> getFoamProductsForML(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? densityId, Int64? fomSuggestedMMId, Int64? fomSizeId)
        {
            StringBuilder stringBuilder;
            List<VMvwFoam> foamProductView;
            var result = repo.vwFoams.Where(f => (collectionId != null ? f.collectionId == collectionId : true)
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
                                            || f.purchaseRatePerMM.ToString().StartsWith(search)
                                            || f.purchaseRatePerMMWithGst.ToString().StartsWith(search)
                                            || f.purchaseRatePerKG.ToString().StartsWith(search)
                                            || f.purchaseRatePerKGWithGst.ToString().StartsWith(search)
                                            || f.sellingRatePerMM.ToString().StartsWith(search)
                                            || f.sellingRatePerMMWithGst.ToString().StartsWith(search)
                                            || f.sellingRatePerKG.ToString().StartsWith(search)
                                            || f.sellingRatePerKGWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true))
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
                                            || f.purchaseRatePerMM.ToString().StartsWith(search)
                                            || f.purchaseRatePerMMWithGst.ToString().StartsWith(search)
                                            || f.purchaseRatePerKG.ToString().StartsWith(search)
                                            || f.purchaseRatePerKGWithGst.ToString().StartsWith(search)
                                            || f.sellingRatePerMM.ToString().StartsWith(search)
                                            || f.sellingRatePerMMWithGst.ToString().StartsWith(search)
                                            || f.sellingRatePerKG.ToString().StartsWith(search)
                                            || f.sellingRatePerKGWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getRugProductsForML(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? designId, Int64? shadeId)
        {
            StringBuilder stringBuilder;
            List<VMvwFWR> rugProductView;
            var result = repo.vwFWRs.Where(f => f.Category.Equals("Rug")
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
                                            || f.cutRate.ToString().StartsWith(search)
                                            || f.cutRateWithGst.ToString().StartsWith(search)
                                            || f.roleRate.ToString().StartsWith(search)
                                            || f.rollRateWithGst.ToString().StartsWith(search)
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
                                            || f.flatRate.ToString().StartsWith(search)
                                            || f.flatRateWithGst.ToString().StartsWith(search)
                                            || f.purchaseFlatRate.ToString().StartsWith(search)
                                            || f.purchaseFlatRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true))
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();
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
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Rug")
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
                                            || f.cutRate.ToString().StartsWith(search)
                                            || f.cutRateWithGst.ToString().StartsWith(search)
                                            || f.roleRate.ToString().StartsWith(search)
                                            || f.rollRateWithGst.ToString().StartsWith(search)
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
                                            || f.flatRate.ToString().StartsWith(search)
                                            || f.flatRateWithGst.ToString().StartsWith(search)
                                            || f.purchaseFlatRate.ToString().StartsWith(search)
                                            || f.purchaseFlatRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getWallpaperProductsForML(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? designId, Int64? shadeId)
        {
            StringBuilder stringBuilder;
            List<VMvwFWR> wallpaperProductView;
            var result = repo.vwFWRs.Where(f => f.Category.Equals("Wallpaper")
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
                                            || f.cutRate.ToString().StartsWith(search)
                                            || f.cutRateWithGst.ToString().StartsWith(search)
                                            || f.roleRate.ToString().StartsWith(search)
                                            || f.rollRateWithGst.ToString().StartsWith(search)
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
                                            || f.flatRate.ToString().StartsWith(search)
                                            || f.flatRateWithGst.ToString().StartsWith(search)
                                            || f.purchaseFlatRate.ToString().StartsWith(search)
                                            || f.purchaseFlatRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true))
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();
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
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Wallpaper")
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
                                            || f.cutRate.ToString().StartsWith(search)
                                            || f.cutRateWithGst.ToString().StartsWith(search)
                                            || f.roleRate.ToString().StartsWith(search)
                                            || f.rollRateWithGst.ToString().StartsWith(search)
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
                                            || f.flatRate.ToString().StartsWith(search)
                                            || f.flatRateWithGst.ToString().StartsWith(search)
                                            || f.purchaseFlatRate.ToString().StartsWith(search)
                                            || f.purchaseFlatRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMvwMattress> getMattressProductsForML(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? matThicknessId, Int64? matSizeId)
        {
            StringBuilder stringBuilder;
            List<VMvwMattress> mattressProductView;
            var result = repo.vwMattresses.Where(m => (collectionId != null ? m.collectionId == collectionId : true)
                                            && (qualityId != null ? m.qualityId == qualityId : true)
                                            && (matThicknessId != null ? m.matThicknessId == matThicknessId : true)
                                            && (matSizeId != null ? m.matSizeId == matSizeId : true)
                                            && (!string.IsNullOrEmpty(search) ?
                                            m.Collection.StartsWith(search)
                                            || m.qualityCode.StartsWith(search)
                                            || m.thicknessCode.StartsWith(search)
                                            || m.sizeCode.StartsWith(search)
                                            || m.uom.StartsWith(search)
                                            || m.hsnCode.StartsWith(search)
                                            || m.rate.ToString().StartsWith(search)
                                            || m.rateWithGst.ToString().StartsWith(search)
                                            || m.purchaseRate.ToString().StartsWith(search)
                                            || m.purchaseRateWithGst.ToString().StartsWith(search)
                                            || m.customRatePerSqFeet.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? m.availableStock > 0 : search.ToLower().Equals("no") ? m.availableStock <= 0 : false) : true))
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();
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
                TotalCount = repo.vwMattresses.Where(m => (collectionId != null ? m.collectionId == collectionId : true)
                                            && (qualityId != null ? m.qualityId == qualityId : true)
                                            && (matThicknessId != null ? m.matThicknessId == matThicknessId : true)
                                            && (matSizeId != null ? m.matSizeId == matSizeId : true)
                                            && (!string.IsNullOrEmpty(search) ?
                                            m.Collection.StartsWith(search)
                                            || m.qualityCode.StartsWith(search)
                                            || m.thicknessCode.StartsWith(search)
                                            || m.sizeCode.StartsWith(search)
                                            || m.uom.StartsWith(search)
                                            || m.hsnCode.StartsWith(search)
                                            || m.rate.ToString().StartsWith(search)
                                            || m.rateWithGst.ToString().StartsWith(search)
                                            || m.purchaseRate.ToString().StartsWith(search)
                                            || m.purchaseRateWithGst.ToString().StartsWith(search)
                                            || m.customRatePerSqFeet.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? m.availableStock > 0 : search.ToLower().Equals("no") ? m.availableStock <= 0 : false) : true)).Count(),
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
                    .OrderBy(q => q.Collection).ToList();
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
                    .OrderBy(q => q.Collection).ToList();
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
                    .OrderBy(q => q.Collection).ToList();
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
                    .OrderBy(q => q.Collection).ToList();
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

        public ListResult<VMvwAccessory> getAccessoryProductsForCL(int pageSize, int page, string search)
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
                    .Where(a => (!string.IsNullOrEmpty(search) ?
                                            a.itemCode.StartsWith(search)
                                            || a.name.StartsWith(search)
                                            || a.size.StartsWith(search)
                                            || a.uom.StartsWith(search)
                                            || a.hsnCode.StartsWith(search)
                                            || a.sellingRate.ToString().StartsWith(search)
                                            || a.sellingRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? a.availableStock > 0 : search.ToLower().Equals("no") ? a.availableStock <= 0 : false) : true))
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

        public ListResult<VMvwFWR> getFabricProductsForCL(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? designId, Int64? shadeId)
        {
            List<VMvwFWR> fabricProductsView;

            fabricProductsView = repo.vwFWRs.Where(f => f.Category.Equals("Fabric")
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
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
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
                        rrp = f.rrp,
                        rrpWithGst = f.rrpWithGst,
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
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Fabric")
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
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
                                            || f.flatRate.ToString().StartsWith(search)
                                            || f.flatRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFoam> getFoamProductsForCL(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? densityId, Int64? fomSuggestedMMId, Int64? fomSizeId)
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
                        hsnWithGST = f.hsnCode + " (" + f.gst + ")"
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

        public ListResult<VMvwFWR> getRugProductsForCL(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? designId, Int64? shadeId)
        {
            List<VMvwFWR> rugProductView;
            rugProductView = repo.vwFWRs.Where(f => f.Category.Equals("Rug")
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
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
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
                        rrp = f.rrp,
                        rrpWithGst = f.rrpWithGst,
                        flatRate = f.flatRate,
                        flatRateWithGst = f.flatRateWithGst,
                        availableStock = f.availableStock > 0 ? f.availableStock : 0,
                        hsnWithGST = f.hsnCode + " (" + f.gst + ")"
                    })
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMvwFWR>
            {
                Data = rugProductView,
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Rug")
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
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
                                            || f.flatRate.ToString().StartsWith(search)
                                            || f.flatRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getWallpaperProductsForCL(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? designId, Int64? shadeId)
        {
            List<VMvwFWR> wallpaperProductView;
            wallpaperProductView = repo.vwFWRs.Where(f => f.Category.Equals("Wallpaper")
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
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
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
                         rrp = f.rrp,
                         rrpWithGst = f.rrpWithGst,
                         flatRate = f.flatRate,
                         flatRateWithGst = f.flatRateWithGst,
                         availableStock = f.availableStock > 0 ? f.availableStock : 0,
                         hsnWithGST = f.hsnCode + " (" + f.gst + ")"
                     })
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMvwFWR>
            {
                Data = wallpaperProductView,
                TotalCount = repo.vwFWRs.Where(f => f.Category.Equals("Wallpaper")
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
                                            || f.rrp.ToString().StartsWith(search)
                                            || f.rrpWithGst.ToString().StartsWith(search)
                                            || f.flatRate.ToString().StartsWith(search)
                                            || f.flatRateWithGst.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)).Count(),
                Page = page
            };
        }

        public ListResult<VMvwMattress> getMattressProductsForCL(int pageSize, int page, string search, Int64? collectionId, Int64? qualityId, Int64? matThicknessId, Int64? matSizeId)
        {
            List<VMvwMattress> mattressProductView;
            mattressProductView = repo.vwMattresses.Where(m => (collectionId != null ? m.collectionId == collectionId : true)
                                            && (qualityId != null ? m.qualityId == qualityId : true)
                                            && (matThicknessId != null ? m.matThicknessId == matThicknessId : true)
                                            && (matSizeId != null ? m.matSizeId == matSizeId : true)
                                            && (!string.IsNullOrEmpty(search) ?
                                            m.Collection.StartsWith(search)
                                            || m.qualityCode.StartsWith(search)
                                            || m.thicknessCode.StartsWith(search)
                                            || m.sizeCode.StartsWith(search)
                                            || m.uom.StartsWith(search)
                                            || m.hsnCode.StartsWith(search)
                                            || m.rate.ToString().StartsWith(search)
                                            || m.rateWithGst.ToString().StartsWith(search)
                                            || m.customRatePerSqFeet.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? m.availableStock > 0 : search.ToLower().Equals("no") ? m.availableStock <= 0 : false) : true))
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
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMvwMattress>
            {
                Data = mattressProductView,
                TotalCount = repo.vwMattresses.Where(m => (collectionId != null ? m.collectionId == collectionId : true)
                                            && (qualityId != null ? m.qualityId == qualityId : true)
                                            && (matThicknessId != null ? m.matThicknessId == matThicknessId : true)
                                            && (matSizeId != null ? m.matSizeId == matSizeId : true)
                                            && (!string.IsNullOrEmpty(search) ?
                                            m.Collection.StartsWith(search)
                                            || m.qualityCode.StartsWith(search)
                                            || m.thicknessCode.StartsWith(search)
                                            || m.sizeCode.StartsWith(search)
                                            || m.uom.StartsWith(search)
                                            || m.hsnCode.StartsWith(search)
                                            || m.rate.ToString().StartsWith(search)
                                            || m.rateWithGst.ToString().StartsWith(search)
                                            || m.customRatePerSqFeet.ToString().StartsWith(search)
                                            || (search.ToLower().Equals("yes") ? m.availableStock > 0 : search.ToLower().Equals("no") ? m.availableStock <= 0 : false) : true)).Count(),
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
                    .ToList();

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
                    .ToList();
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
                    .ToList();
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
                    .ToList();
            return mattressProductView;
        }

        //PO order status report
        public ListResult<VMTrnPurchaseOrderList> getPOorderStatusReport(int pageSize, int page, string status)
        {
            List<VMTrnPurchaseOrderList> purchaseOrderListView;
            purchaseOrderListView = repo.TrnPurchaseOrders.Where(po => !string.IsNullOrEmpty(status) ? po.status.Equals(status) : true)
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
                TotalCount = repo.TrnPurchaseOrders.Where(po => !string.IsNullOrEmpty(status) ? po.status.Equals(status) : true).Count(),
                Page = page
            };
        }

        //SO order status report
        public ListResult<VMTrnSaleOrderList> getSOorderStatusReport(int pageSize, int page, string status)
        {
            List<VMTrnSaleOrderList> saleOrderListView;
            saleOrderListView = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(status) ? so.status.Equals(status) : true)
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
                TotalCount = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(status) ? so.status.Equals(status) : true).Count(),
                Page = page
            };
        }

        //Sales Invoice Status and Payment Report
        public ListResult<VMTrnSalesInvoiceList> getSalesInvoicePaymentStatusReport(int pageSize, int page, string status, string isPaid)
        {
            List<VMTrnSalesInvoiceList> salesInvoiceView;
            var result = repo.TrnSalesInvoices
                .Where(si => (!string.IsNullOrEmpty(status) ? si.status.Equals(status) : true)
                        && (!string.IsNullOrEmpty(isPaid) ? (isPaid.ToLower().Equals("yes") ? si.isPaid : !(si.isPaid)) : true))
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
                    .Where(si => (!string.IsNullOrEmpty(status) ? si.status.Equals(status) : true)
                        && (!string.IsNullOrEmpty(isPaid) ? (isPaid.ToLower().Equals("yes") ? si.isPaid : !(si.isPaid)) : true)).Count(),
                Page = page
            };
        }
    }
}