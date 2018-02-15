using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;

namespace IMSWebApi.Services
{
    public class TrnProductStockService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;

        public TrnProductStockService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMTrnProductStock> getTrnProductStock(int pageSize, int page, string search)
        {
            List<VMTrnProductStock> trnProductStockView;
            if (pageSize > 0)
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search) 
                    || q.stock.ToString().StartsWith(search): true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                trnProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }
            else
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true).ToList();
                trnProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }

            return new ListResult<VMTrnProductStock>
            {
                Data = trnProductStockView,
                TotalCount = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public decimal getProductStockAvailablity(Int64 categoryId, Int64 collectionId, Int64 parameterId)
        {
            decimal stockAvailabe = 0;
            TrnProductStock ProductStock = null;
            string categoryCode = repo.MstCategories.Where(c => c.id == categoryId).Select(a => a.code).FirstOrDefault();
            
            if (categoryCode!=null && categoryCode.Equals("Fabric"))
            {
                ProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                      && z.collectionId == collectionId
                                                      && z.fwrShadeId == parameterId).FirstOrDefault();
            }
            if (categoryCode != null && categoryCode.Equals("Foam"))
            {
                ProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                      && z.collectionId == collectionId
                                                      && z.fomSizeId == parameterId).FirstOrDefault();
            }
            if (categoryCode != null && categoryCode.Equals("Mattress"))
            {
                ProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                        && z.collectionId == collectionId
                                                        && z.matSizeId == parameterId).FirstOrDefault();
            }
            if (categoryCode != null && categoryCode.Equals("Accessories"))
            {
                ProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                        && z.collectionId == collectionId
                                                        && z.accessoryId == parameterId).FirstOrDefault();

            }
            stockAvailabe = ProductStock != null ? ProductStock.stock : 0;
            //List<VMTrnProductStock> stockQty = new List<VMTrnProductStock>(result);
            //List<VMTrnProductStock> productStock = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            //decimal currentProductStock = productStock.Aggregate<VMTrnProductStock, decimal>(0, (productQty, p) => productQty += p.stock);
            return stockAvailabe;
        }


    }
}