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
                var result = repo.MstCollections.Where(c => !string.IsNullOrEmpty(search) ? c.collectionName.StartsWith(search) : true)
                                                .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                supplierView = Mapper.Map<List<MstCollection>, List<VMCollection>>(result);
            }
            else
            {
                var result = repo.MstCollections.Where(c => !string.IsNullOrEmpty(search) ? c.collectionName.StartsWith(search) : true)
                                                .ToList();
                supplierView = Mapper.Map<List<MstCollection>, List<VMCollection>>(result);
            }

            return new ListResult<VMCollection>
            {
                Data = supplierView,
                TotalCount = repo.MstCollections.Where(c => !string.IsNullOrEmpty(search) ? c.collectionName.StartsWith(search) : true)
                                                .Count(),
                Page = page
            };
        }

        public VMCollection getCollectionById(Int64 id)
        {
            var result = repo.MstCollections.Where(s => s.id == id).FirstOrDefault();
            VMCollection collectionView = Mapper.Map<MstCollection, VMCollection>(result);
            return collectionView;
        }

        public List<VMLookUpItem> getCollectionLookUp()
        {
            return repo.MstCollections.Select(s => new VMLookUpItem { id = s.id, Name = s.collectionCode +" " + s.MstSupplier.code }).ToList();
        }


        public ResponseMessage postCollection(VMCollection collection)
        {
            MstCollection collectionToPost = Mapper.Map<VMCollection, MstCollection>(collection);
            collectionToPost.createdOn = DateTime.Now;
            collectionToPost.createdBy = _LoggedInuserId;

            repo.MstCollections.Add(collectionToPost);
            repo.SaveChanges();
            return new ResponseMessage(collectionToPost.id, "Collection Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putCollection(VMCollection collection)
        {
            var collectionToPut = repo.MstCollections.Where(s => s.id == collection.id).FirstOrDefault();
            collectionToPut.supplierId = collection.supplierId;
            collectionToPut.categoryId = collection.categoryId;
            collectionToPut.collectionCode = collection.collectionCode;
            collectionToPut.collectionName = collection.collectionName;
            collectionToPut.description = collection.description;
            collectionToPut.manufacturerName = collection.manufacturerName;
            collectionToPut.updatedBy = _LoggedInuserId;
            collectionToPut.updatedOn = DateTime.Now;
            
            repo.SaveChanges();
            return new ResponseMessage(collection.id, "Collection Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteCollection(Int64 id)
        {  
            repo.MstCollections.Remove(repo.MstCollections.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Collection Deleted Successfully", ResponseType.Success);
        }
    }
}