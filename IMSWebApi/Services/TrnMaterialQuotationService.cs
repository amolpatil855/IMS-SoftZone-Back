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
    public class TrnMaterialQuotationService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        
        public TrnMaterialQuotationService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMTrnMaterialQuotation> getMaterialQuotations(int pageSize, int page, string search)
        {
            List<VMTrnMaterialQuotation> materialQuotationView;
            if (pageSize > 0)
            {
                var result = repo.TrnMaterialQuotations.Where(mq => !string.IsNullOrEmpty(search)
                    ? mq.materialQuotationNumber.StartsWith(search)
                    || mq.MstCustomer.name.StartsWith(search)
                    || mq.status.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                materialQuotationView = Mapper.Map<List<TrnMaterialQuotation>, List<VMTrnMaterialQuotation>>(result);
            }
            else
            {
                var result = repo.TrnMaterialQuotations.Where(mq => !string.IsNullOrEmpty(search)
                    ? mq.materialQuotationNumber.StartsWith(search)
                    || mq.MstCustomer.name.StartsWith(search)
                    || mq.status.StartsWith(search) : true).ToList();
                materialQuotationView = Mapper.Map<List<TrnMaterialQuotation>, List<VMTrnMaterialQuotation>>(result);
            }
            materialQuotationView.ForEach(mq => mq.TrnMaterialQuotationItems.ForEach(mqItem => mqItem.TrnMaterialQuotation = null));
            return new ListResult<VMTrnMaterialQuotation>
            {
                Data = materialQuotationView,
                TotalCount = materialQuotationView.Count(),
                Page = page
            };
        }

        public VMTrnMaterialQuotation getMaterialQuotationById(Int64 id)
        {
            var result = repo.TrnMaterialQuotations.Where(mq => mq.id == id).FirstOrDefault();
            VMTrnMaterialQuotation materialQuotationView = Mapper.Map<TrnMaterialQuotation, VMTrnMaterialQuotation>(result);
            materialQuotationView.customerName = result.MstCustomer.name;

            materialQuotationView.TrnMaterialQuotationItems.ForEach(mqItem =>
            {
                mqItem.categoryName = mqItem.MstCategory.name;
                mqItem.collectionName = mqItem.collectionId != null ? mqItem.MstCollection.collectionName : null;
                mqItem.serialno = mqItem.MstCategory.code.Equals("Fabric") || mqItem.MstCategory.code.Equals("Rug") || mqItem.MstCategory.code.Equals("Wallpaper") ? mqItem.MstFWRShade.serialNumber + "(" + mqItem.MstFWRShade.shadeCode + "-" + mqItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                mqItem.size = mqItem.MstMatSize != null ? mqItem.MstMatSize.sizeCode + " (" + mqItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + mqItem.MstMatSize.MstQuality.qualityCode + ")" : null;
            });
            materialQuotationView.TrnMaterialQuotationItems.ForEach(mqItem => mqItem.TrnMaterialQuotation = null);
            return materialQuotationView;
        }
    }
}