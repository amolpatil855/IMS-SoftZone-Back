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
                var result = repo.MstDesigns.Where(s => !string.IsNullOrEmpty(search) ? s.designCode.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                designView = Mapper.Map<List<MstDesign>, List<VMDesign>>(result);
            }
            else
            {
                var result = repo.MstDesigns.Where(s => !string.IsNullOrEmpty(search) ? s.designCode.StartsWith(search) : true).ToList();
                designView = Mapper.Map<List<MstDesign>, List<VMDesign>>(result);
            }

            return new ListResult<VMDesign>
            {
                Data = designView,
                TotalCount = repo.MstDesigns.Where(s => !string.IsNullOrEmpty(search) ? s.designCode.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMDesign getDesignById(Int64 id)
        {
            var result = repo.MstDesigns.Where(s => s.id == id).FirstOrDefault();
            var design = Mapper.Map<MstDesign, VMDesign>(result);
            return design;
        }

        public ResponseMessage postDesign(VMDesign design)
        {
            MstDesign designToPost = Mapper.Map<VMDesign, MstDesign>(design);
            designToPost.createdOn = DateTime.Now;
            designToPost.createdBy = _LoggedInuserId;

            repo.MstDesigns.Add(designToPost);
            repo.SaveChanges();
            return new ResponseMessage(designToPost.id, "Design Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putDesign(VMDesign design)
        {   
            MstDesign designToPut = repo.MstDesigns.Where(s => s.id == design.id).FirstOrDefault();
            MstCategory designCategory = designToPut.MstCategory;
            MstCollection designCollection = designToPut.MstCollection;
            designToPut =  Mapper.Map<VMDesign,MstDesign>(design,designToPut);
            designToPut.MstCollection = designCollection;
            designToPut.MstCategory = designCategory;
            //designToPut.categoryId = design.categoryId;
            //designToPut.qualityId = design.qualityId;
            //designToPut.collectionId = design.collectionId;
            //designToPut.designCode= design.designCode;
            //designToPut.designName = design.designName;
            //designToPut.description = design.description;
            designToPut.updatedOn = DateTime.Now;
            designToPut.updatedBy = _LoggedInuserId;
            
            repo.SaveChanges();

            return new ResponseMessage(design.id, "Design Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteDesign(Int64 id)
        {  
            repo.MstDesigns.Remove(repo.MstDesigns.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Design Deleted Successfully", ResponseType.Success);
        }
    }
}