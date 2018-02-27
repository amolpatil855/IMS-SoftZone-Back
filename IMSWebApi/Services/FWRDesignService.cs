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
    public class FWRDesignService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;

        public FWRDesignService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMFWRDesign> getDesign(int pageSize, int page, string search)
        {
            List<VMFWRDesign> designView;
            if (pageSize > 0)
            {
                var result = repo.MstFWRDesigns.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    ||q.MstCollection.collectionCode.StartsWith(search) 
                    || q.MstQuality.qualityCode.StartsWith(search) 
                    || q.designCode.StartsWith(search) 
                    || q.designName.StartsWith(search) : true)
                    .OrderBy(q => q.MstCategory.id).ThenBy(q=>q.MstCollection.collectionCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();
                designView = Mapper.Map<List<MstFWRDesign>, List<VMFWRDesign>>(result);
            }
            else
            {
                var result = repo.MstFWRDesigns.Where(q => !string.IsNullOrEmpty(search)
                   ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.designCode.StartsWith(search)
                    || q.designName.StartsWith(search) : true).ToList();
                designView = Mapper.Map<List<MstFWRDesign>, List<VMFWRDesign>>(result);
            }

            return new ListResult<VMFWRDesign>
            {
                Data = designView,
                TotalCount = repo.MstFWRDesigns.Where(q => !string.IsNullOrEmpty(search)
                     ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.designCode.StartsWith(search)
                    || q.designName.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFWRDesign getDesignById(Int64 id)
        {
            var result = repo.MstFWRDesigns.Where(q => q.id == id).FirstOrDefault();
            var designView = Mapper.Map<MstFWRDesign, VMFWRDesign>(result);
            return designView;
        }

        public List<VMLookUpItem> getDesignLookUpByQuality(Int64 qualityId)
        {
            return repo.MstFWRDesigns.Where(q => q.qualityId == qualityId)
                .OrderBy(s=>s.designCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.designCode }).ToList();
        }

        public ResponseMessage postDesign(VMFWRDesign design)
        {
            var designToPost = Mapper.Map<VMFWRDesign, MstFWRDesign>(design);
            designToPost.createdOn = DateTime.Now;
            designToPost.createdBy = _LoggedInuserId;

            repo.MstFWRDesigns.Add(designToPost);
            repo.SaveChanges();
            return new ResponseMessage(designToPost.id, resourceManager.GetString("FWRDesignAdded"), ResponseType.Success);
        }

        public ResponseMessage putDesign(VMFWRDesign design)
        {
            var designToPut = repo.MstFWRDesigns.Where(q => q.id == design.id).FirstOrDefault();

            designToPut.categoryId = design.categoryId;
            designToPut.collectionId = design.collectionId;
            designToPut.qualityId = design.qualityId;
            designToPut.designCode = design.designCode;
            designToPut.designName = design.designName;
            designToPut.description = design.description;

            designToPut.updatedBy = _LoggedInuserId;
            designToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(design.id, resourceManager.GetString("FWRDesignUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteDesign(Int64 id)
        {
            repo.MstFWRDesigns.Remove(repo.MstFWRDesigns.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("FWRDesignDeleted"), ResponseType.Success);
        }
    }
}