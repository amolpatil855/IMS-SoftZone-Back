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
using System.Resources;
using System.Reflection;

namespace IMSWebApi.Services
{
    public class CollectionService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        CategoryService _categoryService;
        ResourceManager resourceManager = null;
        public CollectionService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMCollection> getCollection(int pageSize, int page, string search)
        {
            List<VMCollection> supplierView;
            if (pageSize > 0)
            {
                var result = repo.MstCollections.Where(c => !string.IsNullOrEmpty(search) 
                    ? c.MstCategory.code.StartsWith(search) 
                    || c.collectionName.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                supplierView = Mapper.Map<List<MstCollection>, List<VMCollection>>(result);
            }
            else
            {
                var result = repo.MstCollections.Where(c => !string.IsNullOrEmpty(search)
                    ? c.MstCategory.code.StartsWith(search)
                    || c.collectionName.StartsWith(search) : true).ToList();
                supplierView = Mapper.Map<List<MstCollection>, List<VMCollection>>(result);
            }

            return new ListResult<VMCollection>
            {
                Data = supplierView,
                TotalCount = repo.MstCollections.Where(c => !string.IsNullOrEmpty(search)
                    ? c.MstCategory.code.StartsWith(search)
                    || c.collectionName.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMCollection getCollectionById(Int64 id)
        {
            var result = repo.MstCollections.Where(s => s.id == id).FirstOrDefault();
            VMCollection collectionView = Mapper.Map<MstCollection, VMCollection>(result);
            return collectionView;
        }

        public List<VMLookUpItem> getCollectionLookUpByCategoryId(Int64 categoryId)
        {
            return repo.MstCollections.Where(c => c.categoryId == categoryId)
                .OrderBy(s=>s.collectionCode)
                .Select(s => new VMLookUpItem{ value = s.id, label = s.collectionCode +"-" + 
                    s.MstSupplier.code }).ToList();
        }
        
        public List<VMLookUpItem> getMatCollectionLookUp()
        {
            var matCategoryId = _categoryService.getMatressCategory().id;
            return repo.MstCollections.Where(c => c.categoryId == matCategoryId)
                .OrderBy(s => s.collectionCode)
                .Select(s => new VMLookUpItem
                {
                    value = s.id,
                    label = s.collectionCode + "-" +
                        s.MstSupplier.code
                }).ToList();
        }

        public List<VMLookUpItem> getFomCollectionLookUp()
        {
            var fomCategoryId = _categoryService.getFoamCategory().id;
            return repo.MstCollections.Where(c => c.categoryId == fomCategoryId)
                .OrderBy(s => s.collectionCode)
                .Select(s => new VMLookUpItem
                {
                    value = s.id,
                    label = s.collectionCode + "-" +
                        s.MstSupplier.code
                }).ToList();
        }

       

        public ResponseMessage postCollection(VMCollection collection)
        {
            MstCollection collectionToPost = Mapper.Map<VMCollection, MstCollection>(collection);
            collectionToPost.createdOn = DateTime.Now;
            collectionToPost.createdBy = _LoggedInuserId;

            repo.MstCollections.Add(collectionToPost);
            repo.SaveChanges();
            return new ResponseMessage(collectionToPost.id,
                resourceManager.GetString("CollectionAdded"), ResponseType.Success);
        }

        public ResponseMessage putCollection(VMCollection collection)
        {
            var collectionToPut = repo.MstCollections.Where(s => s.id == collection.id).FirstOrDefault();
            collectionToPut.categoryId = collection.categoryId;
            collectionToPut.supplierId = collection.supplierId;
            collectionToPut.collectionCode = collection.collectionCode;
            collectionToPut.collectionName = collection.collectionName;
            collectionToPut.description = collection.description;
            collectionToPut.manufacturerName = collection.manufacturerName;
            collectionToPut.updatedBy = _LoggedInuserId;
            collectionToPut.updatedOn = DateTime.Now;
            
            repo.SaveChanges();
            return new ResponseMessage(collection.id, resourceManager.GetString("CollectionUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteCollection(Int64 id)
        {  
            repo.MstCollections.Remove(repo.MstCollections.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("CollectionDeleted"), ResponseType.Success);
        }
    }
}