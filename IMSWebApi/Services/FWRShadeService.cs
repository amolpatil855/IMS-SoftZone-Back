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

namespace IMSWebApi.ServicesDesign
{
    public class FWRShadeService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;

        public FWRShadeService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMFWRShade> getShade(int pageSize, int page, string search)
        {
            List<VMFWRShade> shadeView;
            if (pageSize > 0)
            {
                var result = repo.MstFWRShades.Where(q => !string.IsNullOrEmpty(search)
                    ? q.serialNumber.ToString().StartsWith(search)
                    || q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFWRDesign.designCode.StartsWith(search)
                    || q.shadeCode.StartsWith(search)
                    || q.shadeName.StartsWith(search) : true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                shadeView = Mapper.Map<List<MstFWRShade>, List<VMFWRShade>>(result);
            }
            else
            {
                var result = repo.MstFWRShades.Where(q => !string.IsNullOrEmpty(search)
                   ? q.serialNumber.ToString().StartsWith(search)
                    || q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFWRDesign.designCode.StartsWith(search)
                    || q.shadeCode.StartsWith(search)
                    || q.shadeName.StartsWith(search) : true).ToList();
                shadeView = Mapper.Map<List<MstFWRShade>, List<VMFWRShade>>(result);
            }

            return new ListResult<VMFWRShade>
            {
                Data = shadeView,
                TotalCount = repo.MstFWRShades.Where(q => !string.IsNullOrEmpty(search)
                     ? q.serialNumber.ToString().StartsWith(search)
                    || q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFWRDesign.designCode.StartsWith(search)
                    || q.shadeCode.StartsWith(search)
                    || q.shadeName.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFWRShade getShadeById(Int64 id)
        {
            var result = repo.MstFWRShades.Where(q => q.id == id).FirstOrDefault();
            var shadeView = Mapper.Map<MstFWRShade, VMFWRShade>(result);
            return shadeView;
        }

        public List<VMLookUpItem> getSerialNumberLookUpByDesign(Int64 designId)
        {
            return repo.MstFWRShades.Where(q => q.designId == designId)
                .OrderBy(s=>s.serialNumber).ThenBy(s=>s.shadeCode)
                .Select(q => new VMLookUpItem
                {
                    value = q.id,
                    label = q.serialNumber.ToString()
                        + " (" + q.shadeCode + ")"
                }).ToList();
        }

        public List<VMLookUpItem> getSerialNumberLookUpByCollection(Int64 collectionId)
        {
            return repo.MstFWRShades.Where(q => q.collectionId == collectionId)
                .OrderBy(s => s.serialNumber).ThenBy(s => s.shadeCode)
                .Select(q => new VMLookUpItem
                {
                    value = q.id,
                    label = q.serialNumber.ToString()
                        + " (" + q.shadeCode + ")"
                }).ToList();
        }

        public ResponseMessage postShade(VMFWRShade shade)
        {
            var shadeToPost = Mapper.Map<VMFWRShade, MstFWRShade>(shade);
            shadeToPost.createdOn = DateTime.Now;
            shadeToPost.createdBy = _LoggedInuserId;

            repo.MstFWRShades.Add(shadeToPost);
            repo.SaveChanges();
            return new ResponseMessage(shadeToPost.id, resourceManager.GetString("ShadeAdded"), ResponseType.Success);
        }

        public ResponseMessage putShade(VMFWRShade shade)
        {
            var shadeToPut = repo.MstFWRShades.Where(q => q.id == shade.id).FirstOrDefault();

            shadeToPut.categoryId = shade.categoryId;
            shadeToPut.collectionId = shade.collectionId;
            shadeToPut.qualityId = shade.qualityId;
            shadeToPut.designId = shade.designId;
            shadeToPut.shadeCode = shade.shadeCode;
            shadeToPut.shadeName = shade.shadeName;
            shadeToPut.serialNumber = shade.serialNumber;
            shadeToPut.description = shade.description;
            shadeToPut.stockReorderLevel = shade.stockReorderLevel;

            shadeToPut.updatedBy = _LoggedInuserId;
            shadeToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(shade.id, resourceManager.GetString("ShadeUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteShade(Int64 id)
        {
            repo.MstFWRShades.Remove(repo.MstFWRShades.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("ShadeDeleted"), ResponseType.Success);
        }
    }
}