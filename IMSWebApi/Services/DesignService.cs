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
    public class DesignService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public DesignService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMDesign> getDesign(int pageSize, int page, string search)
        {
            List<VMDesign> designView;
            if (pageSize > 0)
            {
                var result = repo.MstDesigns.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCollection.collectionCode.StartsWith(search) 
                    || q.MstQuality.qualityCode.StartsWith(search) 
                    || q.designCode.StartsWith(search) 
                    || q.designName.StartsWith(search) : true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                designView = Mapper.Map<List<MstDesign>, List<VMDesign>>(result);
            }
            else
            {
                var result = repo.MstDesigns.Where(q => !string.IsNullOrEmpty(search)
                   ? q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.designCode.StartsWith(search)
                    || q.designName.StartsWith(search) : true).ToList();
                designView = Mapper.Map<List<MstDesign>, List<VMDesign>>(result);
            }

            return new ListResult<VMDesign>
            {
                Data = designView,
                TotalCount = repo.MstDesigns.Where(q => !string.IsNullOrEmpty(search)
                     ? q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.designCode.StartsWith(search)
                    || q.designName.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMDesign getDesignById(Int64 id)
        {
            var result = repo.MstDesigns.Where(q => q.id == id).FirstOrDefault();
            var designView = Mapper.Map<MstDesign, VMDesign>(result);
            return designView;
        }

        public List<VMLookUpItem> getDesignLookUpByQuality(Int64 qualityId)
        {
            return repo.MstDesigns.Where(q => q.qualityId == qualityId)
                .Select(q => new VMLookUpItem { value = q.id, label = q.designCode }).ToList();
        }

        public ResponseMessage postDesign(VMDesign design)
        {
            var designToPost = Mapper.Map<VMDesign, MstDesign>(design);
            designToPost.createdOn = DateTime.Now;
            designToPost.createdBy = _LoggedInuserId;

            repo.MstDesigns.Add(designToPost);
            repo.SaveChanges();
            return new ResponseMessage(designToPost.id, "Design Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putDesign(VMDesign design)
        {
            var designToPut = repo.MstDesigns.Where(q => q.id == design.id).FirstOrDefault();
            MstCategory designCategory = designToPut.MstCategory;
            MstCollection designCollection = designToPut.MstCollection;
            MstQuality designQuality = designToPut.MstQuality;
            designToPut = Mapper.Map<VMDesign, MstDesign>(design, designToPut);
            designToPut.MstQuality = designQuality;
            designToPut.MstCollection = designCollection;
            designToPut.MstCategory = designCategory;
            designToPut.updatedBy = _LoggedInuserId;
            designToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(design.id, "Design Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteDesign(Int64 id)
        {
            repo.MstDesigns.Remove(repo.MstDesigns.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Design Deleted Successfully", ResponseType.Success);
        }
    }
}