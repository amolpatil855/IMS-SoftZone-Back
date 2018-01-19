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
    public class CollectionService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        public CollectionService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public ListResult<VMCollection> getCollection(int pageSize, int page, string search)
        {
            List<VMCollection> supplierView;
            if (pageSize > 0)
            {
                var result = repo.MstCollections.OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                supplierView = Mapper.Map<List<MstCollection>, List<VMCollection>>(result);
            }
            else
            {
                var result = repo.MstCollections.ToList();
                supplierView = Mapper.Map<List<MstCollection>, List<VMCollection>>(result);
            }

            return new ListResult<VMCollection>
            {
                Data = supplierView,
                TotalCount = repo.MstCollections.Count(),
                Page = page
            };
        }

        public VMCollection getCollectionById(Int64 id)
        {
            var result = repo.MstCollections.Where(s => s.id == id).FirstOrDefault();
            VMCollection collectionView = Mapper.Map<MstCollection, VMCollection>(result);
            return collectionView;
        }


    }
}