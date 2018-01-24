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
    public class MatSizeService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public MatSizeService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMMatSize> getMatSize(int pageSize, int page, string search)
        {
            List<VMMatSize> matSizeView;
            if (pageSize > 0)
            {
                var result = repo.MstMatSizes.Where(m => !string.IsNullOrEmpty(search)
                    ? m.sizeCode.StartsWith(search) : true)
                    .OrderBy(m => m.id).Skip(page * pageSize).Take(pageSize).ToList();
                matSizeView = Mapper.Map<List<MstMatSize>, List<VMMatSize>>(result);
            }
            else
            {
                var result = repo.MstMatSizes.Where(m => !string.IsNullOrEmpty(search)
                   ? m.sizeCode.StartsWith(search) : true).ToList();
                matSizeView = Mapper.Map<List<MstMatSize>, List<VMMatSize>>(result);
            }

            return new ListResult<VMMatSize>
            {
                Data = matSizeView,
                TotalCount = repo.MstMatSizes.Where(m => !string.IsNullOrEmpty(search)
                     ? m.sizeCode.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMMatSize getMatSizeById(Int64 id)
        {
            var result = repo.MstMatSizes.Where(q => q.id == id).FirstOrDefault();
            var matSizeView = Mapper.Map<MstMatSize, VMMatSize>(result);
            return matSizeView;
        }

        public List<VMLookUpItem> getMatSizeLookUp()
        {
            return repo.MstMatSizes
                .OrderBy(m=>m.sizeCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.sizeCode }).ToList();
        }

        public ResponseMessage postMatSize(VMMatSize matSize)
        {
            MstMatSize matSizeToPost = Mapper.Map<VMMatSize, MstMatSize>(matSize);
            matSizeToPost.createdOn = DateTime.Now;
            matSizeToPost.createdBy = _LoggedInuserId;

            repo.MstMatSizes.Add(matSizeToPost);
            repo.SaveChanges();
            return new ResponseMessage(matSizeToPost.id, "Mat Size Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putMatSize(VMMatSize matSize)
        {
            var matSizeToPut = repo.MstMatSizes.Where(q => q.id == matSize.id).FirstOrDefault();
            MstCategory mattressSizeCategory = matSizeToPut.MstCategory;
            MstCollection mattressSizeCollection = matSizeToPut.MstCollection;
            MstMatThickness mattressSizeThickness = matSizeToPut.MstMatThickness;
            MstQuality mattressSizeQuality = matSizeToPut.MstQuality;


            matSizeToPut = Mapper.Map<VMMatSize, MstMatSize>(matSize, matSizeToPut);
            matSizeToPut.MstCategory = mattressSizeCategory;
            matSizeToPut.MstCollection = mattressSizeCollection;
            matSizeToPut.MstMatThickness = mattressSizeThickness;
            matSizeToPut.MstQuality = mattressSizeQuality;
            matSizeToPut.updatedBy = _LoggedInuserId;
            matSizeToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(matSize.id, "Mat Size Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteMatSize(Int64 id)
        {
            repo.MstMatSizes.Remove(repo.MstMatSizes.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Mat Size Deleted Successfully", ResponseType.Success);
        }
    }

}