using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;

namespace IMSWebApi.Services
{
    public class VMFomSizeService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public VMFomSizeService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMFomSize> getFomSize(int pageSize, int page, string search)
        {
            List<VMFomSize> fomSizeView;
            if (pageSize > 0)
            {
                var result = repo.MstFomSizes.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFomDensity.density.ToString().StartsWith(search)
                    || q.width.ToString().StartsWith(search)
                    || q.length.ToString().StartsWith(search): true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                fomSizeView = Mapper.Map<List<MstFomSize>, List<VMFomSize>>(result);
            }
            else
            {
                var result = repo.MstFomSizes.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFomDensity.density.ToString().StartsWith(search)
                    || q.width.ToString().StartsWith(search)
                    || q.length.ToString().StartsWith(search) : true).ToList();
                fomSizeView = Mapper.Map<List<MstFomSize>, List<VMFomSize>>(result);
            }

            return new ListResult<VMFomSize>
            {
                Data = fomSizeView,
                TotalCount = repo.MstFomSizes.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFomDensity.density.ToString().StartsWith(search)
                    || q.width.ToString().StartsWith(search)
                    || q.length.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFomSize getFomSizeById(Int64 id)
        {
            var result = repo.MstFomSizes.Where(q => q.id == id).FirstOrDefault();
            var qualityView = Mapper.Map<MstFomSize, VMFomSize>(result);
            return qualityView;
        }
    }
}