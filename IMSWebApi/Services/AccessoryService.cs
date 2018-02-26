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
using IMSWebApi.Enums;

namespace IMSWebApi.Services
{
    public class AccessoryService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        CategoryService _categoryService = null;
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        public AccessoryService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMAccessory> getAccessory(int pageSize, int page, string search)
        {
            List<VMAccessory> accessoryView;
            if (pageSize > 0)
            {
                var result = repo.MstAccessories.Where(a => !string.IsNullOrEmpty(search)
                    ? a.name.StartsWith(search)
                    || a.itemCode.StartsWith(search)
                    || a.MstHsn.hsnCode.StartsWith(search)
                    || a.MstUnitOfMeasure.uomCode.StartsWith(search)
                    || a.size.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                accessoryView = Mapper.Map<List<MstAccessory>, List<VMAccessory>>(result);
            }
            else
            {
                var result = repo.MstAccessories.Where(a => !string.IsNullOrEmpty(search)
                    ? a.name.StartsWith(search)
                    || a.itemCode.StartsWith(search)
                    || a.MstHsn.hsnCode.StartsWith(search)
                    || a.MstUnitOfMeasure.uomCode.StartsWith(search)
                    || a.size.StartsWith(search) : true).ToList();
                accessoryView = Mapper.Map<List<MstAccessory>, List<VMAccessory>>(result);
            }
            return new ListResult<VMAccessory>
            {
                Data = accessoryView,
                TotalCount = repo.MstAccessories.Where(a => !string.IsNullOrEmpty(search)
                    ? a.name.StartsWith(search)
                    || a.itemCode.StartsWith(search)
                    || a.MstHsn.hsnCode.StartsWith(search)
                    || a.MstUnitOfMeasure.uomCode.StartsWith(search)
                    || a.size.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMAccessory getAccessoryById(Int64 id)
        {
            var result = repo.MstAccessories.Where(s => s.id == id).FirstOrDefault();
            VMAccessory accessoryView = Mapper.Map<MstAccessory, VMAccessory>(result);
            return accessoryView;
        }

        public List<VMLookUpItem> getAccessoryLookUp()
        {
            return repo.MstAccessories
                .OrderBy(m => m.itemCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.itemCode}).ToList();
        }

        public List<VMLookUpItem> getAccessoryLookUpForGRN()
        {
            return repo.TrnPurchaseOrderItems.Where(p => p.categoryId == 7 && (p.status.Equals("Approved") || p.status.Equals("PartialCompleted")))
                .OrderBy(m => m.MstAccessory.itemCode)
                .Select(q => new VMLookUpItem { value = q.MstAccessory.id, label = q.MstAccessory.itemCode }).Distinct().ToList();
        }

        public ResponseMessage postAccessory(VMAccessory accessory)
        {
            MstAccessory accessoryToPost = Mapper.Map<VMAccessory, MstAccessory>(accessory);
            accessoryToPost.categoryId = _categoryService.getAccessoryCategory().id;
            accessoryToPost.createdOn = DateTime.Now;
            accessoryToPost.createdBy = _LoggedInuserId;

            repo.MstAccessories.Add(accessoryToPost);
            repo.SaveChanges();
            return new ResponseMessage(accessoryToPost.id,
                resourceManager.GetString("AccessoryAdded"), ResponseType.Success);
        }

        public ResponseMessage putAccessory(VMAccessory accessory)
        {
            var accessoryToPut = repo.MstAccessories.Where(s => s.id == accessory.id).FirstOrDefault();
            accessoryToPut.name = accessory.name;
            accessoryToPut.itemCode = accessory.itemCode;
            accessoryToPut.supplierId = accessory.supplierId;
            accessoryToPut.hsnId = accessory.hsnId;
            accessoryToPut.uomId = accessory.uomId;
            accessoryToPut.sellingRate = accessory.sellingRate;
            accessoryToPut.purchaseRate = accessory.purchaseRate;
            accessoryToPut.size = accessory.size;
            accessoryToPut.description = accessory.description;
            accessoryToPut.updatedBy = _LoggedInuserId;
            accessoryToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(accessory.id, resourceManager.GetString("AccessoryUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteAccessory(Int64 id)
        {
            repo.MstAccessories.Remove(repo.MstAccessories.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("AccessoryDeleted"), ResponseType.Success);
        }
    }
}