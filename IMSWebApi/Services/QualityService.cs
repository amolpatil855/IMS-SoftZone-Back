﻿using IMSWebApi.Models;
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
    public class QualityService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;

        public QualityService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMQuality> getQuality(int pageSize, int page, string search)
        {
            List<VMQuality> qualityView;
            if (pageSize > 0)
            {
                var result = repo.MstQualities.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search) 
                    || q.MstCollection.collectionCode.StartsWith(search) 
                    || q.qualityCode.StartsWith(search) 
                    || q.qualityName.StartsWith(search)
                    || q.MstHsn.hsnCode.StartsWith(search) : true)
                    .OrderBy(q => q.MstCategory.id).ThenBy(q=>q.MstCollection.collectionCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();
                qualityView = Mapper.Map<List<MstQuality>, List<VMQuality>>(result);
            }
            else
            {
                var result = repo.MstQualities.Where(q => !string.IsNullOrEmpty(search)
                   ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.qualityCode.StartsWith(search)
                    || q.qualityName.StartsWith(search)
                    || q.MstHsn.hsnCode.StartsWith(search) : true).ToList();
                qualityView = Mapper.Map<List<MstQuality>, List<VMQuality>>(result);
            }

            return new ListResult<VMQuality>
            {
                Data = qualityView,
                TotalCount = repo.MstQualities.Where(q => !string.IsNullOrEmpty(search)
                     ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.qualityCode.StartsWith(search)
                    || q.qualityName.StartsWith(search)
                    || q.MstHsn.hsnCode.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMQuality getQualityById(Int64 id)
        {
            var result = repo.MstQualities.Where(q => q.id == id).FirstOrDefault();
            var qualityView = Mapper.Map<MstQuality, VMQuality>(result);
            return qualityView;
        }

        public List<VMLookUpItem> getQualityLookUpByCollection(Int64 collectionId)
        {
            return repo.MstQualities.Where(q => q.collectionId == collectionId)
                .OrderBy(q=>q.qualityCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.qualityCode }).ToList();
        }

        public ResponseMessage postQuality(VMQuality Quality)
        {
            MstQuality QualityToPost = Mapper.Map<VMQuality, MstQuality>(Quality);
            QualityToPost.createdOn = DateTime.Now;
            QualityToPost.createdBy = _LoggedInuserId;

            repo.MstQualities.Add(QualityToPost);
            repo.SaveChanges();
            return new ResponseMessage(QualityToPost.id, resourceManager.GetString("QualityAdded"), ResponseType.Success);
        }

        public ResponseMessage putQuality(VMQuality quality)
        {
            var qualityToPut = repo.MstQualities.Where(q => q.id == quality.id).FirstOrDefault();
            //MstCategory qualityCategory = qualityToPut.MstCategory;
            //MstCollection qualityCollection = qualityToPut.MstCollection;

            //qualityToPut = Mapper.Map<VMQuality, MstQuality>(quality, qualityToPut);
            //qualityToPut.MstCategory = qualityCategory;
            //qualityToPut.MstCollection = qualityCollection;
            qualityToPut.categoryId = quality.categoryId;
            qualityToPut.collectionId = quality.collectionId;
            qualityToPut.qualityCode = quality.qualityCode;
            qualityToPut.qualityName = quality.qualityName;
            qualityToPut.description = quality.description;
            qualityToPut.width = quality.width;
            qualityToPut.size = quality.size;
            qualityToPut.hsnId = quality.hsnId;
            qualityToPut.cutRate = quality.cutRate;
            qualityToPut.roleRate = quality.roleRate;
            qualityToPut.rrp = quality.rrp;
            qualityToPut.maxCutRateDisc = quality.maxCutRateDisc;
            qualityToPut.maxRoleRateDisc = quality.maxRoleRateDisc;
            qualityToPut.flatRate = quality.flatRate;
            qualityToPut.purchaseFlatRate = quality.purchaseFlatRate;
            qualityToPut.maxFlatRateDisc = quality.maxFlatRateDisc;
            qualityToPut.custRatePerSqFeet = quality.custRatePerSqFeet;
            qualityToPut.maxDiscount = quality.maxDiscount;
            qualityToPut.updatedBy = _LoggedInuserId;
            qualityToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(quality.id, resourceManager.GetString("QualityUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteQuality(Int64 id)
        {
            repo.MstQualities.Remove(repo.MstQualities.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("QualityDeleted"), ResponseType.Success);
        }

    }
}