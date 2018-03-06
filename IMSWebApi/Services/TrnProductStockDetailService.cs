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
                    || q.MstAccessory.itemCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.itemCode.StartsWith(search) 
                    || q.stock.ToString().StartsWith(search): true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                trnProductStockDetailView = Mapper.Map<List<TrnProductStockDetail>, List<VMTrnProductStockDetail>>(result);
            }
            else
            {
                var result = repo.TrnProductStockDetails.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstAccessory.itemCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.itemCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true).ToList();
                trnProductStockDetailView = Mapper.Map<List<TrnProductStockDetail>, List<VMTrnProductStockDetail>>(result);
            }
            foreach (var productStock in trnProductStockDetailView)
            {
                productStock.accessoryCode = productStock.MstCategory.code.Equals("Accessories") ? productStock.MstAccessory.itemCode : null;
                productStock.serialno = productStock.MstCategory.code.Equals("Fabric")
                                       || productStock.MstCategory.code.Equals("Rug")
                                       || productStock.MstCategory.code.Equals("Wallpaper")
                                       ? productStock.MstFWRShade.serialNumber + " (" + productStock.MstFWRShade.shadeCode + "-" +
                                       productStock.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                productStock.fomItem = productStock.MstCategory.code.Equals("Foam") ? productStock.MstFomSize.itemCode : null;
                productStock.matSize = productStock.MstCategory.code.Equals("Mattress") ?
                                       productStock.MstMatSize.sizeCode + " (" + productStock.MstMatSize.MstMatThickNess.thicknessCode +
                                       "-" + productStock.MstMatSize.MstQuality.qualityCode + ")" : null;
            }
            return new ListResult<VMTrnProductStockDetail>
            {
                Data = trnProductStockDetailView,
                TotalCount = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstAccessory.itemCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.itemCode.StartsWith(search)
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
                trnProductStockDetailToPost.kgPerUnit = trnProductStockDetail.stockInKg!=null ? trnProductStockDetailToPost.stockInKg / trnProductStockDetailToPost.stock : null;
                trnProductStockDetailToPost.createdOn = DateTime.Now;
                trnProductStockDetailToPost.createdBy = _LoggedInuserId;

                repo.TrnProductStockDetails.Add(trnProductStockDetailToPost);

                _trnProductStockService.AddProductInStock(trnProductStockDetail, false,0,null);

                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(trnProductStockDetailToPost.id, resourceManager.GetString("TrnProductStockAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putTrnProductStockDetail(VMTrnProductStockDetail trnProductStockDetail)
        {
            decimal stock = 0;
            decimal? stockInKg = null;
            var trnProductStockDetailToPut = repo.TrnProductStockDetails.Where(q => q.id == trnProductStockDetail.id).FirstOrDefault();

            trnProductStockDetailToPut.categoryId = trnProductStockDetail.categoryId;
            trnProductStockDetailToPut.collectionId = trnProductStockDetail.collectionId;
            trnProductStockDetailToPut.fomSizeId = trnProductStockDetail.fomSizeId;
            trnProductStockDetailToPut.fwrShadeId = trnProductStockDetail.fwrShadeId;
            trnProductStockDetailToPut.matSizeId = trnProductStockDetail.matSizeId;
            trnProductStockDetailToPut.accessoryId = trnProductStockDetail.accessoryId;

            stock = trnProductStockDetail.stock - trnProductStockDetailToPut.stock;
            stockInKg = trnProductStockDetail.stockInKg - trnProductStockDetailToPut.stockInKg;
            //trnProductStockToPut.locationId = trnProductStock.locationId;
            trnProductStockDetailToPut.stock = trnProductStockDetail.stock;
            trnProductStockDetailToPut.stockInKg = trnProductStockDetail.stockInKg;
            trnProductStockDetailToPut.kgPerUnit = trnProductStockDetailToPut.stock == 0 ? 0 : trnProductStockDetailToPut.stockInKg / trnProductStockDetailToPut.stock;

            trnProductStockDetailToPut.updatedBy = _LoggedInuserId;
            trnProductStockDetailToPut.updatedOn = DateTime.Now;

            _trnProductStockService.AddProductInStock(trnProductStockDetail, true, stock, stockInKg);


            repo.SaveChanges();
            return new ResponseMessage(trnProductStockDetail.id, resourceManager.GetString("TrnProductStockUpdated"), ResponseType.Success);
        }
    }
}