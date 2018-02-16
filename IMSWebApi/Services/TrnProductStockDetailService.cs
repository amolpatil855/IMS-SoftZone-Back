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
using System.Transactions;

namespace IMSWebApi.Services
{
    public class TrnProductStockDetailService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        TrnProductStockService _trnProductStockService = null;

        public TrnProductStockDetailService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _trnProductStockService = new TrnProductStockService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMTrnProductStockDetail> getTrnProductStockDetail(int pageSize, int page, string search)
        {
            List<VMTrnProductStockDetail> trnProductStockDetailView;
            if (pageSize > 0)
            {
                var result = repo.TrnProductStockDetails.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search) 
                    || q.stock.ToString().StartsWith(search): true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                trnProductStockDetailView = Mapper.Map<List<TrnProductStockDetail>, List<VMTrnProductStockDetail>>(result);
            }
            else
            {
                var result = repo.TrnProductStockDetails.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true).ToList();
                trnProductStockDetailView = Mapper.Map<List<TrnProductStockDetail>, List<VMTrnProductStockDetail>>(result);
            }

            return new ListResult<VMTrnProductStockDetail>
            {
                Data = trnProductStockDetailView,
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

        public VMTrnProductStockDetail getTrnProductStockDetailById(Int64 id)
        {
            var result = repo.TrnProductStockDetails.Where(q => q.id == id).FirstOrDefault();
            var trnProductStockDetailView = Mapper.Map<TrnProductStockDetail, VMTrnProductStockDetail>(result);
            return trnProductStockDetailView;
        }

        public ResponseMessage postTrnProductStockDetail(VMTrnProductStockDetail trnProductStockDetail)
        {  
            using (var transaction = new TransactionScope())
            {
                TrnProductStockDetail trnProductStockDetailToPost = Mapper.Map<VMTrnProductStockDetail, TrnProductStockDetail>(trnProductStockDetail);
                trnProductStockDetailToPost.createdOn = DateTime.Now;
                trnProductStockDetailToPost.createdBy = _LoggedInuserId;

                repo.TrnProductStockDetails.Add(trnProductStockDetailToPost);

                _trnProductStockService.AddProductInStock(trnProductStockDetail, false,0);

                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(trnProductStockDetailToPost.id, resourceManager.GetString("TrnProductStockAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putTrnProductStockDetail(VMTrnProductStockDetail trnProductStockDetail)
        {
            decimal qty = 0;
            var trnProductStockDetailToPut = repo.TrnProductStockDetails.Where(q => q.id == trnProductStockDetail.id).FirstOrDefault();

            trnProductStockDetailToPut.categoryId = trnProductStockDetail.categoryId;
            trnProductStockDetailToPut.collectionId = trnProductStockDetail.collectionId;
            trnProductStockDetailToPut.fomSizeId = trnProductStockDetail.fomSizeId;
            trnProductStockDetailToPut.fwrShadeId = trnProductStockDetail.fwrShadeId;
            trnProductStockDetailToPut.matSizeId = trnProductStockDetail.matSizeId;
            trnProductStockDetailToPut.accessoryId = trnProductStockDetail.accessoryId;

            qty = trnProductStockDetail.stock - trnProductStockDetailToPut.stock;
            //trnProductStockToPut.locationId = trnProductStock.locationId;
            trnProductStockDetailToPut.stock = trnProductStockDetail.stock;

            trnProductStockDetailToPut.updatedBy = _LoggedInuserId;
            trnProductStockDetailToPut.updatedOn = DateTime.Now;
            
            _trnProductStockService.AddProductInStock(trnProductStockDetail, true, qty);


            repo.SaveChanges();
            return new ResponseMessage(trnProductStockDetail.id, resourceManager.GetString("TrnProductStockUpdated"), ResponseType.Success);
        }
    }
}