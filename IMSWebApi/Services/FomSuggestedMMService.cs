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
    public class FomSuggestedMMService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        CategoryService _categoryService = null;
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;

        public FomSuggestedMMService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMFomSuggestedMMList> getFomSuggestedMMs(int pageSize, int page, string search)
        {
            List<VMFomSuggestedMMList> fomSuggestedMMListView;
            fomSuggestedMMListView= repo.MstFomSuggestedMMs.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstFomDensity.density.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search) 
                    || q.suggestedMM.ToString().StartsWith(search): true)
                     .Select(f => new VMFomSuggestedMMList
                     {
                         id = f.id,
                         collectionCode = f.MstCollection != null ? f.MstCollection.collectionCode : string.Empty,
                         qualityCode = f.MstQuality != null ? f.MstQuality.qualityCode : string.Empty,
                         density = f.MstFomDensity != null ? f.MstFomDensity.density : string.Empty,
                         suggestedMM = f.suggestedMM
                     })
                    .OrderBy(q => q.collectionCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMFomSuggestedMMList>
            {
                Data = fomSuggestedMMListView,
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
            return new ResponseMessage(fomSuggestedMMToPost.id, resourceManager.GetString("FomSuggestedMMAdded"), ResponseType.Success);
        }

        public ResponseMessage putFomSuggestedMM(VMFomSuggestedMM fomSuggestedMM)
        {
            var fomSuggestedMMToPut = repo.MstFomSuggestedMMs.Where(q => q.id == fomSuggestedMM.id).FirstOrDefault();

            //fomSuggestedMMToPut.categoryId = fomSuggestedMM.categoryId;
            fomSuggestedMMToPut.collectionId = fomSuggestedMM.collectionId;
            fomSuggestedMMToPut.qualityId = fomSuggestedMM.qualityId;
            fomSuggestedMMToPut.fomDensityId = fomSuggestedMM.fomDensityId;
            fomSuggestedMMToPut.suggestedMM = fomSuggestedMM.suggestedMM;

            fomSuggestedMMToPut.updatedBy = _LoggedInuserId;
            fomSuggestedMMToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(fomSuggestedMMToPut.id, resourceManager.GetString("FomSuggestedMMUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteFomSuggestedMM(Int64 id)
        {
            repo.MstFomSuggestedMMs.Remove(repo.MstFomSuggestedMMs.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("FomSuggestedMMDeleted"), ResponseType.Success);
        }
    }
}