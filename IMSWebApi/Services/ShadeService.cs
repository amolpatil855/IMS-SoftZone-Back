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

namespace IMSWebApi.ServicesDesign
{
    public class ShadeService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public ShadeService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMShade> getShade(int pageSize, int page, string search)
        {
            List<VMShade> shadeView;
            if (pageSize > 0)
            {
                var result = repo.MstShades.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCollection.collectionCode.StartsWith(search) 
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstDesign.designCode.StartsWith(search)
                    || q.shadeCode.StartsWith(search)
                    || q.serialNumber.ToString().StartsWith(search)
                    || q.shadeName.StartsWith(search) : true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                shadeView = Mapper.Map<List<MstShade>, List<VMShade>>(result);
            }
            else
            {
                var result = repo.MstShades.Where(q => !string.IsNullOrEmpty(search)
                   ? q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstDesign.designCode.StartsWith(search)
                    || q.shadeCode.StartsWith(search)
                    || q.serialNumber.ToString().StartsWith(search)
                    || q.shadeName.StartsWith(search) : true).ToList();
                shadeView = Mapper.Map<List<MstShade>, List<VMShade>>(result);
            }

            return new ListResult<VMShade>
            {
                Data = shadeView,
                TotalCount = repo.MstShades.Where(q => !string.IsNullOrEmpty(search)
                     ? q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstDesign.designCode.StartsWith(search)
                    || q.shadeCode.StartsWith(search)
                    || q.serialNumber.ToString().StartsWith(search)
                    || q.shadeName.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMShade getShadeById(Int64 id)
        {
            var result = repo.MstShades.Where(q => q.id == id).FirstOrDefault();
            var shadeView = Mapper.Map<MstShade, VMShade>(result);
            return shadeView;
        }

        public List<VMLookUpItem> getSerialNumberLookUpByDesign(Int64 designId)
        {
            return repo.MstShades.Where(q => q.designId == designId)
                .Select(q => new VMLookUpItem { key = q.id, value = q.serialNumber.ToString() 
                    + "-" + q.shadeCode }).ToList();
        }

        public List<VMLookUpItem> getSerialNumberLookUpByCollection(Int64 collectionId)
        {
            return repo.MstShades.Where(q => q.collectionId == collectionId)
                .Select(q => new VMLookUpItem
                {
                    key = q.id,
                    value = q.serialNumber.ToString()
                        + "-" + q.shadeCode
                }).ToList();
        }

        public ResponseMessage postShade(VMShade shade)
        {
            var shadeToPost = Mapper.Map<VMShade, MstShade>(shade);
            shadeToPost.createdOn = DateTime.Now;
            shadeToPost.createdBy = _LoggedInuserId;

            repo.MstShades.Add(shadeToPost);
            repo.SaveChanges();
            return new ResponseMessage(shadeToPost.id, "Shade Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putShade(VMShade shade)
        {
            var shadeToPut = repo.MstShades.Where(q => q.id == shade.id).FirstOrDefault();

            shadeToPut = Mapper.Map<VMShade, MstShade>(shade, shadeToPut);
            
            shadeToPut.updatedBy = _LoggedInuserId;
            shadeToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(shade.id, "Shade Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteShade(Int64 id)
        {
            repo.MstShades.Remove(repo.MstShades.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Shade Deleted Successfully", ResponseType.Success);
        }
    }
}