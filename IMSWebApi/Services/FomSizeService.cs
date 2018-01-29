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
    public class FomSizeService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        CategoryService _categoryService = null;
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;

        public FomSizeService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMFomSize> getFomSize(int pageSize, int page, string search)
        {
            List<VMFomSize> fomSizeView;
            if (pageSize > 0)
            {
                var result = repo.MstFomSizes.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFomDensity.density.ToString().StartsWith(search)
                    || q.width.ToString().StartsWith(search)
                    || q.length.ToString().StartsWith(search): true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                fomSizeView = Mapper.Map<List<MstFomSize>, List<VMFomSize>>(result);
            }
            else
            {
                var result = repo.MstFomSizes.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFomDensity.density.ToString().StartsWith(search)
                    || q.width.ToString().StartsWith(search)
                    || q.length.ToString().StartsWith(search) : true).ToList();
                fomSizeView = Mapper.Map<List<MstFomSize>, List<VMFomSize>>(result);
            }

            return new ListResult<VMFomSize>
            {
                Data = fomSizeView,
                TotalCount = repo.MstFomSizes.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFomDensity.density.ToString().StartsWith(search)
                    || q.width.ToString().StartsWith(search)
                    || q.length.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFomSize getFomSizeById(Int64 id)
        {
            var result = repo.MstFomSizes.Where(q => q.id == id).FirstOrDefault();
            var qualityView = Mapper.Map<MstFomSize, VMFomSize>(result);
            return qualityView;
        }

        public List<VMLookUpItem> getFomSizeLookUpByCollection(Int64 collectionId)
        {
            return repo.MstFomSizes.Where(m=>m.collectionId == collectionId)
                .OrderBy(m => m.sizeCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.sizeCode }).ToList();
        }

        public ResponseMessage postFomSize(VMFomSize fomSize)
        {
            MstFomSize fomSizeToPost = Mapper.Map<VMFomSize, MstFomSize>(fomSize);
            fomSizeToPost.categoryId = _categoryService.getFoamCategory().id;
            fomSizeToPost.createdOn = DateTime.Now;
            fomSizeToPost.createdBy = _LoggedInuserId;

            repo.MstFomSizes.Add(fomSizeToPost);
            repo.SaveChanges();
            return new ResponseMessage(fomSizeToPost.id, resourceManager.GetString("FomSizeAdded"), ResponseType.Success);
        }

        public ResponseMessage putFomSize(VMFomSize fomSize)
        {
            var fomSizeToPut = repo.MstFomSizes.Where(q => q.id == fomSize.id).FirstOrDefault();
            fomSizeToPut.categoryId = fomSize.categoryId;
            fomSizeToPut.collectionId = fomSize.collectionId;
            fomSizeToPut.qualityId = fomSize.qualityId;
            fomSizeToPut.fomDensityId = fomSize.fomDensityId;
            fomSizeToPut.fomSuggestedMMId = fomSize.fomSuggestedMMId;
            fomSizeToPut.width = fomSize.width;
            fomSizeToPut.length = fomSize.length;
            fomSizeToPut.sizeCode = fomSize.sizeCode;

            fomSizeToPut.updatedBy = _LoggedInuserId;
            fomSizeToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(fomSize.id, resourceManager.GetString("FomSizeUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteFomSize(Int64 id)
        {
            repo.MstFomSizes.Remove(repo.MstFomSizes.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("FomSizeDeleted"), ResponseType.Success);
        }
    }
}