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
    public class FomDensityService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        CategoryService _categoryService = null;
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;

        public FomDensityService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMFomDensity> getFomDensity(int pageSize, int page, string search)
        {
            List<VMFomDensity> fomDensityView;
            if (pageSize > 0)
            {
                var result = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search)
                    || f.purchaseRatePerMM.ToString().StartsWith(search)
                    || f.purchaseRatePerKG.ToString().StartsWith(search)
                    || f.sellingRatePercentage.ToString().StartsWith(search) : true)
                    .OrderBy(f => f.id).Skip(page * pageSize).Take(pageSize).ToList();
                fomDensityView = Mapper.Map<List<MstFomDensity>, List<VMFomDensity>>(result);
            }
            else
            {
                var result = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search)
                    || f.purchaseRatePerMM.ToString().StartsWith(search)
                    || f.purchaseRatePerKG.ToString().StartsWith(search)
                    || f.sellingRatePercentage.ToString().StartsWith(search) : true).ToList();
                fomDensityView = Mapper.Map<List<MstFomDensity>, List<VMFomDensity>>(result);
            }

            return new ListResult<VMFomDensity>
            {
                Data = fomDensityView,
                TotalCount = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search)
                    || f.purchaseRatePerMM.ToString().StartsWith(search)
                    || f.purchaseRatePerKG.ToString().StartsWith(search)
                    || f.sellingRatePercentage.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFomDensity getFomDensityById(Int64 id)
        {
            var result = repo.MstFomDensities.Where(q => q.id == id).FirstOrDefault();
            var fomDensityView = Mapper.Map<MstFomDensity, VMFomDensity>(result);
            return fomDensityView;
        }

        public List<VMLookUpItem> getFomDensityLookUpByQualityId(Int64 qualityId)
        {
            return repo.MstFomDensities.Where(f=>f.qualityId == qualityId)
                .OrderBy(q => q.density)
                .Select(q => new VMLookUpItem { value = q.id, label = q.density })
                .ToList();
        }

        public ResponseMessage postFomDensity(VMFomDensity fomDensity)
        {
            MstFomDensity fomDensityToPost = Mapper.Map<VMFomDensity, MstFomDensity>(fomDensity);
            fomDensityToPost.categoryId = _categoryService.getFoamCategory().id;
            fomDensityToPost.createdOn = DateTime.Now;
            fomDensityToPost.createdBy = _LoggedInuserId;

            repo.MstFomDensities.Add(fomDensityToPost);
            repo.SaveChanges();
            return new ResponseMessage(fomDensityToPost.id, resourceManager.GetString("FomDensityAdded"), ResponseType.Success);
        }

        public ResponseMessage putFomDensity(VMFomDensity fomDensity)
        {
            var fomDensityToPut = repo.MstFomDensities.Where(q => q.id == fomDensity.id).FirstOrDefault();
            //fomDensityToPut.categoryId = fomDensity.categoryId;
            fomDensityToPut.collectionId = fomDensity.collectionId;
            fomDensityToPut.qualityId = fomDensity.qualityId;
            fomDensityToPut.density = fomDensity.density;
            fomDensityToPut.description = fomDensity.description;
            fomDensityToPut.purchaseRatePerMM = fomDensity.purchaseRatePerMM;
            fomDensityToPut.purchaseRatePerKG = fomDensity.purchaseRatePerKG;
            fomDensityToPut.sellingRatePercentage = fomDensity.sellingRatePercentage;
            fomDensityToPut.sellingRatePerMM = fomDensity.sellingRatePerMM;
            fomDensityToPut.sellingRatePerKG = fomDensity.sellingRatePerKG;

            fomDensityToPut.updatedBy = _LoggedInuserId;
            fomDensityToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(fomDensity.id, resourceManager.GetString("FomDensityUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteFomDensity(Int64 id)
        {
            repo.MstFomDensities.Remove(repo.MstFomDensities.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("FomDensityDeleted"), ResponseType.Success);
        }
    }
}