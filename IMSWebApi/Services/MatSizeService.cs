using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;
using IMSWebApi.Enums;
using System.Resources;
using System.Reflection;

namespace IMSWebApi.Services
{
    public class MatSizeService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        CategoryService _categoryService;
        ResourceManager resourceManager = null;

        public MatSizeService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMMatSize> getMatSizes(int pageSize, int page, string search)
        {
            List<VMMatSize> matSizeView;
            if (pageSize > 0)
            {
                var result = repo.MstMatSizes.Where(m => !string.IsNullOrEmpty(search) 
                    ? m.sizeCode.StartsWith(search) 
                    || m.MstCollection.collectionCode.StartsWith(search)
                    || m.MstQuality.qualityCode.StartsWith(search)
                    || m.MstMatThickness.thicknessCode.StartsWith(search)
                    || m.purchaseRate.ToString().StartsWith(search) : true)
                    .OrderBy(m => m.MstCollection.collectionCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();
                matSizeView = Mapper.Map<List<MstMatSize>, List<VMMatSize>>(result);
            }
            else
            {
                var result = repo.MstMatSizes.Where(m => !string.IsNullOrEmpty(search) 
                    ? m.sizeCode.StartsWith(search)
                    || m.MstCollection.collectionCode.StartsWith(search)
                    || m.MstQuality.qualityCode.StartsWith(search)
                    || m.MstMatThickness.thicknessCode.StartsWith(search)
                    || m.purchaseRate.ToString().StartsWith(search) : true).ToList();
                matSizeView = Mapper.Map<List<MstMatSize>, List<VMMatSize>>(result);
            }

            return new ListResult<VMMatSize>
            {
                Data = matSizeView,
                TotalCount = repo.MstMatSizes.Where(m => !string.IsNullOrEmpty(search) 
                    ? m.sizeCode.StartsWith(search) 
                    || m.MstCollection.collectionCode.StartsWith(search)
                    || m.MstQuality.qualityCode.StartsWith(search)
                    || m.MstMatThickness.thicknessCode.StartsWith(search)
                    || m.purchaseRate.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMMatSize getMatSizeById(Int64 id)
        {
            var result = repo.MstMatSizes.Where(q => q.id == id).FirstOrDefault();

            var matSizeView = Mapper.Map<MstMatSize, VMMatSize>(result);
            return matSizeView;
        }

        public List<VMLookUpItem> getMatSizeLookUpByCollectionId(Int64 collectionId)
        {
            return repo.MstMatSizes.Where(m=>m.collectionId == collectionId)
                .OrderBy(m=>m.sizeCode)
                .Select(q => new VMLookUpItem { value = q.id, 
                    label = q.sizeCode + " (" + q.MstMatThickness.thicknessCode +"-"+ q.MstQuality.qualityCode+")"}).ToList();
        }

        public List<VMLookUpItem> getMatSizeLookUpForGRN(Int64 collectionId)
        {
            return repo.TrnPurchaseOrderItems.Where(m => m.collectionId == collectionId 
                                                    && m.matSizeId!=null
                                                   && (m.status.Equals("Approved") || m.status.Equals("PartialCompleted")))
                .OrderBy(m => m.MstMatSize.sizeCode)
                .Select(q => new VMLookUpItem
                {
                    value = q.MstMatSize.id,
                    label = q.MstMatSize.sizeCode + " (" + q.MstMatSize.MstMatThickness.thicknessCode + "-" + q.MstMatSize.MstQuality.qualityCode + ")"
                }).Distinct().ToList();
        }

        public ResponseMessage postMatSize(VMMatSize matSize)
        {
            MstMatSize matSizeToPost = Mapper.Map<VMMatSize, MstMatSize>(matSize);
            matSizeToPost.categoryId = _categoryService.getMatressCategory().id;
            matSizeToPost.createdOn = DateTime.Now;
            matSizeToPost.createdBy = _LoggedInuserId;

            repo.MstMatSizes.Add(matSizeToPost);
            repo.SaveChanges();
            return new ResponseMessage(matSizeToPost.id, resourceManager.GetString("MatSizeAdded"), ResponseType.Success);
        }

        public ResponseMessage putMatSize(VMMatSize matSize)
        {
            var matSizeToPut = repo.MstMatSizes.Where(q => q.id == matSize.id).FirstOrDefault();

            //matSizeToPut.categoryId = matSize.categoryId;
            matSizeToPut.collectionId = matSize.collectionId;
            matSizeToPut.qualityId = matSize.qualityId;
            matSizeToPut.thicknessId = matSize.thicknessId;
            matSizeToPut.length = matSize.length;
            matSizeToPut.width = matSize.width;
            matSizeToPut.sizeCode = matSize.sizeCode;
            matSizeToPut.rate = matSize.rate;
            matSizeToPut.purchaseRate = matSize.purchaseRate;
            matSizeToPut.stockReorderLevel = matSize.stockReorderLevel;

            matSizeToPut.updatedBy = _LoggedInuserId;
            matSizeToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(matSize.id, resourceManager.GetString("MatSizeUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteMatSize(Int64 id)
        {
            repo.MstMatSizes.Remove(repo.MstMatSizes.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("MatSizeDeleted"), ResponseType.Success);
        }
    }

}