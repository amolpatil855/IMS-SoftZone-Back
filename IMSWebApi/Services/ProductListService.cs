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
       
        public ProductListService()
        {
        }

        public ListResult<VMvwAccessory> getAccessoryProducts(int pageSize, int page, string search)
        {
            List<VMvwAccessory> accessoryProductView;
            accessoryProductView = repo.vwAccessories.Where(a => !string.IsNullOrEmpty(search)
                    ? a.itemCode.StartsWith(search)
                    || a.name.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? a.availableStock > 0 : search.ToLower().Equals("no") ? a.availableStock <= 0 : false) : true)
                    .Select(a => new VMvwAccessory
                    {
                        name = a.name,
                        itemCode = a.itemCode,
                        availableStock = a.availableStock
                    })
                    .OrderBy(q => q.itemCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();
            accessoryProductView.ForEach(a => a.availableStock = a.availableStock > 0 ? a.availableStock : 0);
            return new ListResult<VMvwAccessory>
            {
                Data = accessoryProductView,
                TotalCount = repo.vwAccessories.Where(a => !string.IsNullOrEmpty(search)
                    ? a.itemCode.StartsWith(search)
                    || a.name.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? a.availableStock > 0 : search.ToLower().Equals("no") ? a.availableStock <= 0 : false) : true).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFWR> getFabricProducts(int pageSize, int page, string search)
        {
            List<VMvwFWR> fabricProductView;
            fabricProductView = repo.vwFWRs.Where(f => (!string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.QDS.StartsWith(search)
                    || f.serialNumber.ToString().StartsWith(search) 
                    || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false ) : true)
                    && f.Category.Equals("Fabric")
                    && f.flatRate != null)
                    .Select(f => new VMvwFWR
                    {
                        Collection = f.Collection,
                        QDS = f.QDS,
                        serialNumber = f.serialNumber,
                        availableStock = f.availableStock
                    })
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).Distinct().ToList();
            fabricProductView.ForEach(f => f.availableStock = f.availableStock > 0 ? f.availableStock : 0);
            return new ListResult<VMvwFWR>
            {
                Data = fabricProductView,
                TotalCount = repo.vwFWRs.Where(f => (!string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.QDS.StartsWith(search)
                    || f.serialNumber.ToString().StartsWith(search)
                    || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)
                    && f.Category.Equals("Fabric")
                    && f.flatRate != null).Count(),
                Page = page
            };
        }

        public ListResult<VMvwFoam> getFoamProducts(int pageSize, int page, string search)
        {
            List<VMvwFoam> foamProductView;
            foamProductView = repo.vwFoams.Where(f => !string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.qualityCode.StartsWith(search)
                    || f.itemCode.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true)
                    .Select(f => new VMvwFoam
                    {
                        Collection = f.Collection,
                        qualityCode = f.qualityCode,
                        itemCode = f.itemCode,
                        availableStock =f.availableStock
                    })
                    .OrderBy(q => q.Collection)
                    .Skip(page * pageSize).Take(pageSize).ToList();
            foamProductView.ForEach(f => f.availableStock = f.availableStock > 0 ? f.availableStock : 0);
            return new ListResult<VMvwFoam>
            {
                Data = foamProductView,
                TotalCount = repo.vwFoams.Where(f => !string.IsNullOrEmpty(search)
                    ? f.Collection.StartsWith(search)
                    || f.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? f.availableStock > 0 : search.ToLower().Equals("no") ? f.availableStock <= 0 : false) : true).Count(),
                Page = page
            };
        }

        public decimal getProductStock(long categoryId, long? collectionId, long parameterId)
        {
            TrnProductStock trnProductStock = null;
            decimal productStock = 0;
            string categoryCode = repo.MstCategories.Where(c => c.id == categoryId).Select(a => a.code).FirstOrDefault();

            if (categoryCode != null && (categoryCode.Equals("Fabric") || categoryCode.Equals("Rug") || categoryCode.Equals("Wallpaper")))
            {
                trnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                      && z.collectionId == collectionId
                                                      && z.fwrShadeId == parameterId).FirstOrDefault();
                productStock = trnProductStock != null ? trnProductStock.stock + trnProductStock.poQuantity - trnProductStock.soQuanity : 0;
                productStock = productStock < 0 ? 0 : productStock;
                
            }
            if (categoryCode != null && categoryCode.Equals("Foam"))
            {
                trnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                      && z.collectionId == collectionId
                                                      && z.fomSizeId == parameterId).FirstOrDefault();

                productStock = trnProductStock != null ? trnProductStock.stock + trnProductStock.poQuantity - trnProductStock.soQuanity : 0;
                productStock = productStock < 0 ? 0 : productStock;
            }
           
            if (categoryCode != null && categoryCode.Equals("Accessories"))
            {
                trnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                        && z.accessoryId == parameterId).FirstOrDefault();
                
                productStock = trnProductStock != null ? trnProductStock.stock + trnProductStock.poQuantity - trnProductStock.soQuanity : 0;
                productStock = productStock < 0 ? 0 : productStock;
            }

            return productStock;
        }


    }
        
}