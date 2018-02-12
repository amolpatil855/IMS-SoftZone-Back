using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;

namespace IMSWebApi.Services
{
    public class TrnGoodReceiveNoteService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        public TrnGoodReceiveNoteService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMTrnGoodReceiveNote> getGoodReceiveNote(int pageSize, int page, string search)
        {
            List<VMTrnGoodReceiveNote> goodReceiveNoteView;
            if (pageSize > 0)
            {
                var result = repo.TrnGoodReceiveNotes.Where(grn => !string.IsNullOrEmpty(search)
                    ? grn.grnNumber.ToString().StartsWith(search)
                    || grn.grnDate.ToString().StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                goodReceiveNoteView = Mapper.Map<List<TrnGoodReceiveNote>, List<VMTrnGoodReceiveNote>>(result);
            }
            else
            {
                var result = repo.TrnGoodReceiveNotes.Where(grn => !string.IsNullOrEmpty(search)
                    ? grn.grnNumber.ToString().StartsWith(search)
                    || grn.grnDate.ToString().StartsWith(search) : true).ToList();
                goodReceiveNoteView = Mapper.Map<List<TrnGoodReceiveNote>, List<VMTrnGoodReceiveNote>>(result);
            }

            return new ListResult<VMTrnGoodReceiveNote>
            {
                Data = goodReceiveNoteView,
                TotalCount = repo.TrnGoodReceiveNotes.Where(grn => !string.IsNullOrEmpty(search)
                    ? grn.grnNumber.ToString().StartsWith(search)
                    || grn.grnDate.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnGoodReceiveNote getGoodReceiveNoteById(Int64 id)
        {
            var result = repo.TrnGoodReceiveNotes.Where(grn => grn.id == id).FirstOrDefault();
            VMTrnGoodReceiveNote goodReceiveNoteView = Mapper.Map<TrnGoodReceiveNote, VMTrnGoodReceiveNote>(result);
            return goodReceiveNoteView;
        }
    }
}