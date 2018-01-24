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

namespace IMSWebApi.Services
{
    public class FomDensityService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public FomDensityService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMFomDensity> getFomDensity(int pageSize, int page, string search)
        {
            List<VMFomDensity> fomDensityView;
            if (pageSize > 0)
            {
                var result = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search) : true)
                    .OrderBy(f => f.id).Skip(page * pageSize).Take(pageSize).ToList();
                fomDensityView = Mapper.Map<List<MstFomDensity>, List<VMFomDensity>>(result);
            }
            else
            {
                var result = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search) : true).ToList();
                fomDensityView = Mapper.Map<List<MstFomDensity>, List<VMFomDensity>>(result);
            }

            return new ListResult<VMFomDensity>
            {
                Data = fomDensityView,
                TotalCount = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFomDensity getFomDensityById(Int64 id)
        {
            var result = repo.MstFomDensities.Where(q => q.id == id).FirstOrDefault();
            var fomDensityView = Mapper.Map<MstFomDensity, VMFomDensity>(result);
            return fomDensityView;
        }

        public List<VMLookUpItem> getFomDensityLookUp()
        {
            return repo.MstFomDensities
                .Select(q => new VMLookUpItem { value = q.id, label = q.density })
                .OrderBy(q=>q.label)
                .ToList();
        }

        public ResponseMessage postFomDensity(VMFomDensity fomDensity)
        {
            MstFomDensity fomDensityToPost = Mapper.Map<VMFomDensity, MstFomDensity>(fomDensity);
            fomDensityToPost.createdOn = DateTime.Now;
            fomDensityToPost.createdBy = _LoggedInuserId;

            repo.MstFomDensities.Add(fomDensityToPost);
            repo.SaveChanges();
            return new ResponseMessage(fomDensityToPost.id, "Fom Density Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putFomDensity(VMFomDensity fomDensity)
        {
            var fomDensityToPut = repo.MstFomDensities.Where(q => q.id == fomDensity.id).FirstOrDefault();
            MstCategory fomDensityCategory = fomDensityToPut.MstCategory;
            MstCollection fomDensityCollection = fomDensityToPut.MstCollection;
            MstQuality fomDensityQuality = fomDensityToPut.MstQuality;

            fomDensityToPut = Mapper.Map<VMFomDensity, MstFomDensity>(fomDensity, fomDensityToPut);
            fomDensityToPut.MstCategory = fomDensityCategory;
            fomDensityToPut.MstCollection = fomDensityCollection;
            fomDensityToPut.MstQuality = fomDensityQuality;
            fomDensityToPut.updatedBy = _LoggedInuserId;
            fomDensityToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(fomDensity.id, "Fom Density Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteFomDensity(Int64 id)
        {
            repo.MstFomDensities.Remove(repo.MstFomDensities.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Fom Density Deleted Successfully", ResponseType.Success);
        }
    }
}