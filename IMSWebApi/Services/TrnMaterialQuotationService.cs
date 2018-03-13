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
using System.Transactions;
using IMSWebApi.Enums;

namespace IMSWebApi.Services
{
    public class TrnMaterialQuotationService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;
        
        public TrnMaterialQuotationService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
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

        public ResponseMessage postMaterialQuotation(VMTrnMaterialQuotation materialQuotation)
        {
            using (var transaction = new TransactionScope())
            {
                TrnMaterialQuotation materialQuotationToPost = Mapper.Map<VMTrnMaterialQuotation, TrnMaterialQuotation>(materialQuotation);
                var materialQuotationItems = materialQuotationToPost.TrnMaterialQuotationItems.ToList();

                foreach (var mqItems in materialQuotationItems)
                {
                    mqItems.matSizeId = mqItems.matSizeId == -1 ? null : mqItems.matSizeId;     //set null for custom matSize
                    mqItems.balanceQuantity = mqItems.orderQuantity;
                    mqItems.deliverQuantity = 0;
                    mqItems.status = MaterialQuotationStatus.Created.ToString();
                    mqItems.createdOn = DateTime.Now;
                    mqItems.createdBy = _LoggedInuserId;
                }
                //materialSelectionToPost.materialSelectionDate = DateTime.Now;
                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= materialQuotationToPost.materialQuotationDate && f.endDate >= materialQuotationToPost.materialQuotationDate).FirstOrDefault();
                string materialQuotationNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.materialSelectionNumber, "MQ");
                materialQuotationToPost.materialQuotationNumber = materialQuotationNo;
                materialQuotationToPost.status = MaterialQuotationStatus.Created.ToString();
                materialQuotationToPost.createdOn = DateTime.Now;
                materialQuotationToPost.createdBy = _LoggedInuserId;

                repo.TrnMaterialQuotations.Add(materialQuotationToPost);

                financialYear.materialQuotationNumber += 1;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(materialQuotationToPost.id, resourceManager.GetString("QuotationCreated"), ResponseType.Success);
            }
        }
    }
}