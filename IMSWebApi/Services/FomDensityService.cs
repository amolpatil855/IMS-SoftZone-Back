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
    public class FomDensityService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public FomDensityService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMFomDensity> getFoamDensity(int pageSize, int page, string search)
        {
            List<VMFomDensity> foamDensityView;
            if (pageSize > 0)
            {
                var result = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search) : true)
                    .OrderBy(f => f.id).Skip(page * pageSize).Take(pageSize).ToList();
                foamDensityView = Mapper.Map<List<MstFomDensity>, List<VMFomDensity>>(result);
            }
            else
            {
                var result = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search) : true).ToList();
                foamDensityView = Mapper.Map<List<MstFomDensity>, List<VMFomDensity>>(result);
            }

            return new ListResult<VMFomDensity>
            {
                Data = foamDensityView,
                TotalCount = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFomDensity getFoamDensityById(Int64 id)
        {
            var result = repo.MstFomDensities.Where(q => q.id == id).FirstOrDefault();
            var foamDensityView = Mapper.Map<MstFomDensity, VMFomDensity>(result);
            return foamDensityView;
        }

    }
}