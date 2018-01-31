using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Reflection;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;
using IMSWebApi.Enums;

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
                    || q.MstFWRShade.shadeCode.StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search) : true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                trnProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }
            else
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFWRShade.shadeCode.StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search) : true).ToList();
                trnProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }

            return new ListResult<VMTrnProductStock>
            {
                Data = trnProductStockView,
                TotalCount = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFWRShade.shadeCode.StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnProductStock getTrnProductStockById(Int64 id)
        {
            var result = repo.TrnProductStocks.Where(q => q.id == id).FirstOrDefault();
            var trnProductStockView = Mapper.Map<TrnProductStock, VMTrnProductStock>(result);
            return trnProductStockView;
        }

        public ResponseMessage postTrnProductStock(VMTrnProductStock trnProductStock)
        {
            trnProductStock.fwrShadeId = trnProductStock.fwrShadeId == 0 ? null : trnProductStock.fwrShadeId;
            trnProductStock.fomSizeId = trnProductStock.fomSizeId == 0 ? null : trnProductStock.fomSizeId;
            trnProductStock.matSizeId = trnProductStock.matSizeId == 0 ? null : trnProductStock.matSizeId;
           
            TrnProductStock trnProductStockToPost = Mapper.Map<VMTrnProductStock, TrnProductStock>(trnProductStock);
            trnProductStockToPost.createdOn = DateTime.Now;
            trnProductStockToPost.createdBy = _LoggedInuserId;

            repo.TrnProductStocks.Add(trnProductStockToPost);
            repo.SaveChanges();
            return new ResponseMessage(trnProductStockToPost.id, resourceManager.GetString("TrnProductStockAdded"), ResponseType.Success);
        }

        public ResponseMessage putTrnProductStock(VMTrnProductStock trnProductStock)
        {
            trnProductStock.fwrShadeId = trnProductStock.fwrShadeId == 0 ? null : trnProductStock.fwrShadeId;
            trnProductStock.fomSizeId = trnProductStock.fomSizeId == 0 ? null : trnProductStock.fomSizeId;
            trnProductStock.matSizeId = trnProductStock.matSizeId == 0 ? null : trnProductStock.matSizeId;

            var trnProductStockToPut = repo.TrnProductStocks.Where(q => q.id == trnProductStock.id).FirstOrDefault();
            
            trnProductStockToPut.categoryId = trnProductStock.categoryId;
            trnProductStockToPut.collectionId = trnProductStock.collectionId;
            trnProductStockToPut.fomSizeId = trnProductStock.fomSizeId;
            trnProductStockToPut.fwrShadeId = trnProductStock.fwrShadeId;
            trnProductStockToPut.matSizeId = trnProductStock.matSizeId;
            trnProductStockToPut.locationId = trnProductStock.locationId;
            trnProductStockToPut.stock = trnProductStock.stock;
           
            trnProductStockToPut.updatedBy = _LoggedInuserId;
            trnProductStockToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(trnProductStock.id, resourceManager.GetString("TrnProductStockUpdated"), ResponseType.Success);
        }
    }
}