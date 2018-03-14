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
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                materialQuotationView = Mapper.Map<List<TrnMaterialQuotation>, List<VMTrnMaterialQuotation>>(result);
            }
            else
            {
                var result = repo.TrnMaterialQuotations.Where(mq => !string.IsNullOrEmpty(search)
                    ? mq.materialQuotationNumber.StartsWith(search)
                    || mq.MstCustomer.name.StartsWith(search)
                    || mq.status.StartsWith(search) : true).OrderByDescending(p => p.id).ToList();
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
                mqItem.collectionName = mqItem.collectionId != null ? mqItem.MstCollection.collectionCode : null;
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

                materialQuotationToPost.TrnMaterialSelection.isQuotationCreated = true;

                repo.TrnMaterialQuotations.Add(materialQuotationToPost);

                financialYear.materialQuotationNumber += 1;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(materialQuotationToPost.id, resourceManager.GetString("MQCreated"), ResponseType.Success);
            }
        }

        public ResponseMessage putMaterialQuotation(VMTrnMaterialQuotation materialQuotation)
        {
            using (var transaction = new TransactionScope())
            {
                var materialQuotationToPut = repo.TrnMaterialQuotations.Where(ms => ms.id == materialQuotation.id).FirstOrDefault();

                updateMQItems(materialQuotation);

                materialQuotationToPut.updatedBy = _LoggedInuserId;
                materialQuotationToPut.updatedOn = DateTime.Now;

                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(materialQuotation.id, resourceManager.GetString("MQUpdated"), ResponseType.Success);
            }
        }

        public void updateMQItems(VMTrnMaterialQuotation materialQuotation)
        {
            var materialQuotationToPut = repo.TrnMaterialQuotations.Where(q => q.id == materialQuotation.id).FirstOrDefault();

            List<TrnMaterialQuotationItem> itemsToRemove = new List<TrnMaterialQuotationItem>();
            foreach (var mqItem in materialQuotationToPut.TrnMaterialQuotationItems)
            {
                if (materialQuotation.TrnMaterialQuotationItems.Any(y => y.id == mqItem.id))
                {
                    continue;
                }
                else
                {
                    itemsToRemove.Add(mqItem);
                }
            }

            repo.TrnMaterialQuotationItems.RemoveRange(itemsToRemove);
            repo.SaveChanges();

            materialQuotation.TrnMaterialQuotationItems.ForEach(x =>
            {
                if (materialQuotationToPut.TrnMaterialQuotationItems.Any(y => y.id == x.id))
                {
                    var mqItemToPut = repo.TrnMaterialQuotationItems.Where(p => p.id == x.id).FirstOrDefault();

                    mqItemToPut.selectionType = x.selectionType;
                    mqItemToPut.area = x.area;
                    mqItemToPut.categoryId = x.categoryId;
                    mqItemToPut.collectionId = x.collectionId;
                    mqItemToPut.shadeId = x.shadeId;
                    mqItemToPut.matSizeId = x.matSizeId;
                    mqItemToPut.matThicknessId = x.matThicknessId;
                    mqItemToPut.qualityId = x.qualityId;
                    mqItemToPut.matHeight = x.matHeight;
                    mqItemToPut.matWidth = x.matWidth;
                    mqItemToPut.orderQuantity = x.orderQuantity;
                    mqItemToPut.deliverQuantity = x.deliverQuantity;
                    mqItemToPut.balanceQuantity = x.balanceQuantity;
                    mqItemToPut.rate = x.rate;
                    mqItemToPut.discountPercentage = x.discountPercentage;
                    mqItemToPut.amount = x.amount;
                    mqItemToPut.rateWithGST = x.rateWithGST;
                    mqItemToPut.amountWithGST = x.amountWithGST;
                    mqItemToPut.gst = x.gst;
                    mqItemToPut.status = x.status;
                    mqItemToPut.updatedOn = DateTime.Now;
                    mqItemToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
            });
        }


    }
}