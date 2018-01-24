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

namespace IMSWebApi.Services
{
    public class FomSuggestedMMService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        CategoryService _categoryService = null;
        Int64 _LoggedInuserId;

        public FomSuggestedMMService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
        }

        public ListResult<VMFomSuggestedMM> getFomSuggestedMM(int pageSize, int page, string search)
        {
            List<VMFomSuggestedMM> fomSuggestedMMView;
            if (pageSize > 0)
            {
                var result = repo.MstFomSuggestedMMs.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstFomDensity.density.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search) 
                    || q.suggestedMM.ToString().StartsWith(search): true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                fomSuggestedMMView = Mapper.Map<List<MstFomSuggestedMM>, List<VMFomSuggestedMM>>(result);
            }
            else
            {
                var result = repo.MstFomSuggestedMMs.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstFomDensity.density.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.suggestedMM.ToString().StartsWith(search) : true).ToList();
                fomSuggestedMMView = Mapper.Map<List<MstFomSuggestedMM>, List<VMFomSuggestedMM>>(result);
            }

            return new ListResult<VMFomSuggestedMM>
            {
                Data = fomSuggestedMMView,
                TotalCount = repo.MstFomSuggestedMMs.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstFomDensity.density.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.suggestedMM.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFomSuggestedMM getFomSuggestedMMById(Int64 id)
        {
            var result = repo.MstFomSuggestedMMs.Where(q => q.id == id).FirstOrDefault();
            var fomSuggestedMMView = Mapper.Map<MstFomSuggestedMM, VMFomSuggestedMM>(result);
            return fomSuggestedMMView;
        }

        public List<VMLookUpItem> getFomSuggestedMMLookUpByFomDensity(Int64 fomDensityId)
        {
            return repo.MstFomSuggestedMMs.Where(q => q.fomDensityId == fomDensityId)
                .OrderBy(q => q.suggestedMM)
                .Select(q => new VMLookUpItem { value = q.id, label = q.suggestedMM.ToString() })
                .ToList();
        }

        public ResponseMessage postFomSuggestedMM(VMFomSuggestedMM fomSuggestedMM)
        {
            MstFomSuggestedMM fomSuggestedMMToPost = Mapper.Map<VMFomSuggestedMM, MstFomSuggestedMM>(fomSuggestedMM);
            fomSuggestedMMToPost.categoryId = _categoryService.getFoamCategory().id;
            fomSuggestedMMToPost.createdOn = DateTime.Now;
            fomSuggestedMMToPost.createdBy = _LoggedInuserId;

            repo.MstFomSuggestedMMs.Add(fomSuggestedMMToPost);
            repo.SaveChanges();
            return new ResponseMessage(fomSuggestedMMToPost.id, "Foam Suggested MM Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putFomSuggestedMM(VMFomSuggestedMM fomSuggestedMM)
        {
            var fomSuggestedMMToPut = repo.MstFomSuggestedMMs.Where(q => q.id == fomSuggestedMM.id).FirstOrDefault();

            fomSuggestedMMToPut.categoryId = fomSuggestedMM.categoryId;
            fomSuggestedMMToPut.collectionId = fomSuggestedMM.collectionId;
            fomSuggestedMMToPut.qualityId = fomSuggestedMM.qualityId;
            fomSuggestedMMToPut.fomDensityId = fomSuggestedMM.fomDensityId;
            fomSuggestedMMToPut.suggestedMM = fomSuggestedMM.suggestedMM;

            fomSuggestedMMToPut.updatedBy = _LoggedInuserId;
            fomSuggestedMMToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(fomSuggestedMMToPut.id, "Foam Suggested MM Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteFomSuggestedMM(Int64 id)
        {
            repo.MstFomSuggestedMMs.Remove(repo.MstFomSuggestedMMs.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Foam Suggested MM Deleted Successfully", ResponseType.Success);
        }
    }
}