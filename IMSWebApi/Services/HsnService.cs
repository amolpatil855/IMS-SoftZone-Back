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
    public class HsnService
    {
         WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;

        public HsnService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMHsn> getHsn(int pageSize, int page, string search)
        {
            List<VMHsn> HsnView;
            if (pageSize > 0)
            {
                var result = repo.MstHsns.Where(h => !string.IsNullOrEmpty(search)
                    ? h.hsnCode.StartsWith(search)
                    || h.gst.ToString().StartsWith(search) : true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                HsnView = Mapper.Map<List<MstHsn>, List<VMHsn>>(result);
            }
            else
            {
                var result = repo.MstHsns.Where(h => !string.IsNullOrEmpty(search)
                    ? h.hsnCode.StartsWith(search) : true).ToList();
                HsnView = Mapper.Map<List<MstHsn>, List<VMHsn>>(result);
            }

            return new ListResult<VMHsn>
            {
                Data = HsnView,
                TotalCount = repo.MstHsns.Where(h => !string.IsNullOrEmpty(search)
                     ? h.hsnCode.StartsWith(search) : true).ToList().Count(),
                Page = page
            };
        }

        public VMHsn getHsnById(Int64 id)
        {
            var result = repo.MstHsns.Where(h => h.id == id).FirstOrDefault();
            var HsnView = Mapper.Map<MstHsn, VMHsn>(result);
            return HsnView;
        }

        public List<VMLookUpItem> getHsnLookUp(Int64 collectionId)
        {
            return repo.MstHsns.Select(h => new VMLookUpItem { key = h.id, value = h.hsnCode }).ToList();
        }

        public ResponseMessage postHsn(VMHsn Hsn)
        {
            MstHsn HsnToPost = Mapper.Map<VMHsn, MstHsn>(Hsn);
            HsnToPost.createdOn = DateTime.Now;
            HsnToPost.createdBy = _LoggedInuserId;

            repo.MstHsns.Add(HsnToPost);
            repo.SaveChanges();
            return new ResponseMessage(HsnToPost.id, "Hsn Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putHsn(VMHsn Hsn)
        {
            var HsnToPut = repo.MstHsns.Where(h => h.id == Hsn.id).FirstOrDefault();

            HsnToPut = Mapper.Map<VMHsn, MstHsn>(Hsn, HsnToPut);            
            HsnToPut.updatedBy = _LoggedInuserId;
            HsnToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(Hsn.id, "Hsn Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteHsn(Int64 id)
        {
            repo.MstHsns.Remove(repo.MstHsns.Where(h => h.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Hsn Deleted Successfully", ResponseType.Success);
        }
    }
}