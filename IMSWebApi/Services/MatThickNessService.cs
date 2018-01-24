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
    public class MatThicknessService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public MatThicknessService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMMatThickness> getMatThickness(int pageSize, int page, string search)
        {
            List<VMMatThickness> matThicknessView;
            if (pageSize > 0)
            {
                var result = repo.MstMatThicknesses.Where(q => !string.IsNullOrEmpty(search)
                    ? q.thicknessCode.StartsWith(search) : true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                matThicknessView = Mapper.Map<List<MstMatThickness>, List<VMMatThickness>>(result);
            }
            else
            {
                var result = repo.MstMatThicknesses.Where(q => !string.IsNullOrEmpty(search)
                   ? q.thicknessCode.StartsWith(search) : true).ToList();
                matThicknessView = Mapper.Map<List<MstMatThickness>, List<VMMatThickness>>(result);
            }

            return new ListResult<VMMatThickness>
            {
                Data = matThicknessView,
                TotalCount = repo.MstMatThicknesses.Where(q => !string.IsNullOrEmpty(search)
                     ? q.thicknessCode.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMMatThickness getMatThicknessById(Int64 id)
        {
            var result = repo.MstMatThicknesses.Where(q => q.id == id).FirstOrDefault();
            var matThicknessView = Mapper.Map<MstMatThickness, VMMatThickness>(result);
            return matThicknessView;
        }

        public List<VMLookUpItem> getMatThicknessLookUp()
        {
            return repo.MstMatThicknesses
                .Select(q => new VMLookUpItem { value = q.id, label = q.thicknessCode }).ToList();
        }

        public ResponseMessage postMatThickness(VMMatThickness matThickness)
        {
            MstMatThickness matThicknessToPost = Mapper.Map<VMMatThickness, MstMatThickness>(matThickness);
            matThicknessToPost.createdOn = DateTime.Now;
            matThicknessToPost.createdBy = _LoggedInuserId;

            repo.MstMatThicknesses.Add(matThicknessToPost);
            repo.SaveChanges();
            return new ResponseMessage(matThicknessToPost.id, "Mat Thickness Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putMatThickness(VMMatThickness matThickness)
        {
            var matThicknessToPut = repo.MstMatThicknesses.Where(q => q.id == matThickness.id).FirstOrDefault();

            matThicknessToPut = Mapper.Map<VMMatThickness, MstMatThickness>(matThickness, matThicknessToPut);
            matThicknessToPut.updatedBy = _LoggedInuserId;
            matThicknessToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(matThicknessToPut.id, "Mat Thickness Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteMatThickness(Int64 id)
        {
            repo.MstMatThicknesses.Remove(repo.MstMatThicknesses.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Mat Thickness Deleted Successfully", ResponseType.Success);
        }
    }
}