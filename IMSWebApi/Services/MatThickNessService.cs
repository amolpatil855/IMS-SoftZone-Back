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
    public class MatThickNessService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public MatThickNessService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMMatThickNess> getMatThickNess(int pageSize, int page, string search)
        {
            List<VMMatThickNess> matThickNessView;
            if (pageSize > 0)
            {
                var result = repo.MstMatThickNesses.Where(q => !string.IsNullOrEmpty(search)
                    ? q.thickNessCode.StartsWith(search) : true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                matThickNessView = Mapper.Map<List<MstMatThickNess>, List<VMMatThickNess>>(result);
            }
            else
            {
                var result = repo.MstMatThickNesses.Where(q => !string.IsNullOrEmpty(search)
                   ? q.thickNessCode.StartsWith(search) : true).ToList();
                matThickNessView = Mapper.Map<List<MstMatThickNess>, List<VMMatThickNess>>(result);
            }

            return new ListResult<VMMatThickNess>
            {
                Data = matThickNessView,
                TotalCount = repo.MstMatThickNesses.Where(q => !string.IsNullOrEmpty(search)
                     ? q.thickNessCode.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMMatThickNess getMatThickNessById(Int64 id)
        {
            var result = repo.MstMatThickNesses.Where(q => q.id == id).FirstOrDefault();
            var matThickNessView = Mapper.Map<MstMatThickNess, VMMatThickNess>(result);
            return matThickNessView;
        }

        public List<VMLookUpItem> getMatThickNessLookUp()
        {
            return repo.MstMatThickNesses
                .Select(q => new VMLookUpItem { value = q.id, label = q.thickNessCode }).ToList();
        }

        public ResponseMessage postMatThickNess(VMMatThickNess matThickNess)
        {
            MstMatThickNess matThickNessToPost = Mapper.Map<VMMatThickNess, MstMatThickNess>(matThickNess);
            matThickNessToPost.createdOn = DateTime.Now;
            matThickNessToPost.createdBy = _LoggedInuserId;

            repo.MstMatThickNesses.Add(matThickNessToPost);
            repo.SaveChanges();
            return new ResponseMessage(matThickNessToPost.id, "Mat ThickNess Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putMatThickNess(VMMatThickNess matThickNess)
        {
            var matThickNessToPut = repo.MstMatThickNesses.Where(q => q.id == matThickNess.id).FirstOrDefault();

            matThickNessToPut = Mapper.Map<VMMatThickNess, MstMatThickNess>(matThickNess, matThickNessToPut);
            matThickNessToPut.updatedBy = _LoggedInuserId;
            matThickNessToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(matThickNess.id, "Mat ThickNess Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteMatThickNess(Int64 id)
        {
            repo.MstMatThickNesses.Remove(repo.MstMatThickNesses.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Mat ThickNess Deleted Successfully", ResponseType.Success);
        }
    }
}