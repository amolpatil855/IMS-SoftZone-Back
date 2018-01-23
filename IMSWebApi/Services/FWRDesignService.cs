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
    public class FWRDesignService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public FWRDesignService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMFWRDesign> getDesign(int pageSize, int page, string search)
        {
            List<VMFWRDesign> designView;
            if (pageSize > 0)
            {
                var result = repo.MstFWRDesigns.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCollection.collectionCode.StartsWith(search) 
                    || q.MstQuality.qualityCode.StartsWith(search) 
                    || q.designCode.StartsWith(search) 
                    || q.designName.StartsWith(search) : true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                designView = Mapper.Map<List<MstFWRDesign>, List<VMFWRDesign>>(result);
            }
            else
            {
                var result = repo.MstFWRDesigns.Where(q => !string.IsNullOrEmpty(search)
                   ? q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.designCode.StartsWith(search)
                    || q.designName.StartsWith(search) : true).ToList();
                designView = Mapper.Map<List<MstFWRDesign>, List<VMFWRDesign>>(result);
            }

            return new ListResult<VMFWRDesign>
            {
                Data = designView,
                TotalCount = repo.MstFWRDesigns.Where(q => !string.IsNullOrEmpty(search)
                     ? q.MstCollection.collectionCode.StartsWith(search)
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
                .Select(q => new VMLookUpItem { value = q.id, label = q.designCode }).ToList();
        }

        public ResponseMessage postDesign(VMFWRDesign design)
        {
            var designToPost = Mapper.Map<VMFWRDesign, MstFWRDesign>(design);
            designToPost.createdOn = DateTime.Now;
            designToPost.createdBy = _LoggedInuserId;

            repo.MstFWRDesigns.Add(designToPost);
            repo.SaveChanges();
            return new ResponseMessage(designToPost.id, "Design Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putDesign(VMFWRDesign design)
        {
            var designToPut = repo.MstFWRDesigns.Where(q => q.id == design.id).FirstOrDefault();
            MstCategory designCategory = designToPut.MstCategory;
            MstCollection designCollection = designToPut.MstCollection;
            MstQuality designQuality = designToPut.MstQuality;
            designToPut = Mapper.Map<VMFWRDesign, MstFWRDesign>(design, designToPut);
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
            repo.MstFWRDesigns.Remove(repo.MstFWRDesigns.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Design Deleted Successfully", ResponseType.Success);
        }
    }
}