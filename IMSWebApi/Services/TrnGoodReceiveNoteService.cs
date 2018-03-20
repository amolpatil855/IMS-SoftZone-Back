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
    public class TrnGoodReceiveNoteService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;
        TrnProductStockService _productStockService = null;
        SendEmail emailNotification = null;

        public TrnGoodReceiveNoteService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
            _productStockService = new TrnProductStockService();
            emailNotification = new SendEmail();
        }

        public ListResult<VMTrnGoodReceiveNoteList> getGoodReceiveNote(int pageSize, int page, string search)
        {
            List<VMTrnGoodReceiveNoteList> goodReceiveNoteView;
            goodReceiveNoteView = repo.TrnGoodReceiveNotes.Where(grn => !string.IsNullOrEmpty(search)
                    ? grn.grnNumber.ToString().StartsWith(search)
                    || grn.grnDate.ToString().StartsWith(search)
                    || grn.TrnGoodReceiveNoteItems.Any(grnItem => grnItem.TrnPurchaseOrder.orderNumber.Contains(search))
                    || grn.MstSupplier.code.StartsWith(search)
                    || grn.MstCompanyLocation.locationCode.StartsWith(search)
                    || grn.totalAmount.ToString().StartsWith(search) : true)
                    .Select(grn => new VMTrnGoodReceiveNoteList
                    {
                        id = grn.id,
                        grnDate = grn.grnDate,
                        grnNumber = grn.grnNumber,
                        purchaseOrderNumbers = grn.TrnGoodReceiveNoteItems.Select(s=>s.TrnPurchaseOrder.orderNumber).Distinct().ToList(),
                        supplierName = grn.MstSupplier != null ? grn.MstSupplier.code : null ,
                        locationCode = grn.MstCompanyLocation != null ? grn.MstCompanyLocation.locationCode : null,
                        totalAmount = grn.totalAmount
                    })
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
               
                return new ListResult<VMTrnGoodReceiveNoteList>
            {
                Data = goodReceiveNoteView,
                TotalCount = repo.TrnGoodReceiveNotes.Where(grn => !string.IsNullOrEmpty(search)
                    ? grn.grnNumber.ToString().StartsWith(search)
                    || grn.grnDate.ToString().StartsWith(search)
                    || grn.TrnGoodReceiveNoteItems.Any(grnItem => grnItem.TrnPurchaseOrder.orderNumber.Contains(search))
                    || grn.MstSupplier.code.StartsWith(search)
                    || grn.MstCompanyLocation.locationCode.StartsWith(search)
                    || grn.totalAmount.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnGoodReceiveNote getGoodReceiveNoteById(Int64 id)
        {
            var result = repo.TrnGoodReceiveNotes.Where(grn => grn.id == id).FirstOrDefault();
            VMTrnGoodReceiveNote goodReceiveNoteView = Mapper.Map<TrnGoodReceiveNote, VMTrnGoodReceiveNote>(result);
            goodReceiveNoteView.supplierName = result.MstSupplier.name;
            goodReceiveNoteView.shippingAddress = result.MstCompanyLocation.addressLine1 + " " + result.MstCompanyLocation.addressLine2 +
                                                   "," + result.MstCompanyLocation.city + ", " + result.MstCompanyLocation.state + " PINCODE - "
                                                   + result.MstCompanyLocation.pin;

            goodReceiveNoteView.TrnGoodReceiveNoteItems.ForEach(grnItem =>
            {
                grnItem.categoryName = grnItem.MstCategory.name;
                grnItem.collectionName = grnItem.collectionId != null ? grnItem.MstCollection.collectionCode + " (" + grnItem.MstCollection.MstSupplier.code + ")" : null;
                grnItem.serialno = grnItem.MstCategory.code.Equals("Fabric") 
                                || grnItem.MstCategory.code.Equals("Rug") 
                                || grnItem.MstCategory.code.Equals("Wallpaper") 
                                ? grnItem.MstFWRShade.serialNumber + "(" + grnItem.MstFWRShade.shadeCode + "-" + grnItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                grnItem.size = grnItem.MstMatSize != null ? grnItem.MstMatSize.sizeCode + " (" + grnItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + grnItem.MstMatSize.MstQuality.qualityCode + ")" :
                                grnItem.MstFomSize != null ? grnItem.MstFomSize.itemCode : grnItem.matSizeCode;
                grnItem.accessoryName = grnItem.accessoryId != null ? grnItem.MstAccessory.itemCode : null;
                grnItem.purchaseOrderNumber = grnItem.TrnPurchaseOrder.orderNumber;
            });
            return goodReceiveNoteView;
        }

        public List<VMLookUpItem> getSupplierForGRN()
        {
            return repo.TrnPurchaseOrders.Where(po => po.status.Equals("Approved") || po.status.Equals("PartialCompleted"))
                            .Select(s => new VMLookUpItem
                            {
                                value = s.MstSupplier.id,
                                label = s.MstSupplier.code
                            })
                            .OrderBy(o=>o.label)
                            .Distinct()
                            .ToList();
        }

        public List<VMTrnGoodReceiveNoteItem> getPOListForSelectedItem(Int64 categoryId, Int64? collectionId, Int64? parameterId, string matSizeCode, Int64? matQualityId, Int64? matThicknessId)
        {
            string categoryCode = repo.MstCategories.Where(c => c.id == categoryId).Select(a => a.code).FirstOrDefault();
            List<VMTrnGoodReceiveNoteItem> poItemDetails = new List<VMTrnGoodReceiveNoteItem>();

            if (categoryCode != null && (categoryCode.Equals("Fabric") || categoryCode.Equals("Rug") || categoryCode.Equals("Wallpaper")))
            {
                poItemDetails = repo.TrnPurchaseOrderItems.Where(po => po.categoryId == categoryId
                                                                  && po.collectionId == collectionId
                                                                  && po.shadeId == parameterId
                                                                  && (po.status.Equals("Approved") || po.status.Equals("PartialCompleted")))
                                                          .Select(s => new VMTrnGoodReceiveNoteItem
                                                          {
                                                              purchaseOrderId = s.TrnPurchaseOrder.id,
                                                              orderQuantity = s.balanceQuantity,
                                                              rate = (decimal)s.rate,
                                                              rateWithGST = s.rateWithGST,
                                                              orderType = s.orderType,
                                                              gst = s.gst,
                                                              purchaseOrderNumber = s.TrnPurchaseOrder.orderNumber,
                                                              purchaseDiscount = s.MstCollection.purchaseDiscount,
                                                              cutRate = s.MstFWRShade.MstQuality.cutRate,
                                                              roleRate = s.MstFWRShade.MstQuality.roleRate,
                                                              purchaseFlatRate = s.MstFWRShade.MstQuality.purchaseFlatRate
                                                          }).ToList();
            }
            if (categoryCode != null && categoryCode.Equals("Foam"))
            {
                poItemDetails = repo.TrnPurchaseOrderItems.Where(po => po.categoryId == categoryId
                                                                  && po.collectionId == collectionId
                                                                  && po.fomSizeId == parameterId
                                                                  && (po.status.Equals("Approved") || po.status.Equals("PartialCompleted")))
                                                          .Select(s => new VMTrnGoodReceiveNoteItem
                                                          {
                                                              purchaseOrderId = s.TrnPurchaseOrder.id,
                                                              orderQuantity = s.balanceQuantity,
                                                              rate = (decimal)s.rate,
                                                              rateWithGST = s.rateWithGST,
                                                              orderType = s.orderType,
                                                              gst = s.gst,
                                                              purchaseOrderNumber = s.TrnPurchaseOrder.orderNumber,
                                                              purchaseDiscount = s.MstCollection.purchaseDiscount
                                                          }).ToList();
            }
            if (categoryCode != null && categoryCode.Equals("Mattress"))
            {
                if (parameterId != null && parameterId != -1)
                {
                    poItemDetails = repo.TrnPurchaseOrderItems.Where(po => po.categoryId == categoryId
                                                                  && po.collectionId == collectionId
                                                                  && po.matSizeId == parameterId
                                                                  && (po.status.Equals("Approved") || po.status.Equals("PartialCompleted")))
                                                          .Select(s => new VMTrnGoodReceiveNoteItem
                                                          {
                                                              purchaseOrderId = s.TrnPurchaseOrder.id,
                                                              orderQuantity = s.balanceQuantity,
                                                              rate = (decimal)s.rate,
                                                              rateWithGST = s.rateWithGST,
                                                              orderType = s.orderType,
                                                              gst = s.gst,
                                                              purchaseOrderNumber = s.TrnPurchaseOrder.orderNumber,
                                                              purchaseDiscount = s.MstCollection.purchaseDiscount
                                                          }).ToList();
                }
                else if (matSizeCode != null)
                {
                    poItemDetails = repo.TrnPurchaseOrderItems.Where(po => po.categoryId == categoryId
                                                                  && po.collectionId == collectionId
                                                                  && po.matSizeCode.Equals(matSizeCode)
                                                                  && po.matQualityId == matQualityId
                                                                  && po.matThicknessId == matThicknessId
                                                                  && (po.status.Equals("Approved") || po.status.Equals("PartialCompleted")))
                                                          .Select(s => new VMTrnGoodReceiveNoteItem
                                                          {
                                                              purchaseOrderId = s.TrnPurchaseOrder.id,
                                                              orderQuantity = s.balanceQuantity,
                                                              rate = (decimal)s.rate,
                                                              rateWithGST = s.rateWithGST,
                                                              orderType = s.orderType,
                                                              gst = s.gst,
                                                              purchaseOrderNumber = s.TrnPurchaseOrder.orderNumber,
                                                              purchaseDiscount = s.MstCollection.purchaseDiscount
                                                          }).ToList();
                }
            }
            if (categoryCode != null && categoryCode.Equals("Accessories"))
            {
                poItemDetails = repo.TrnPurchaseOrderItems.Where(po => po.categoryId == categoryId
                                                                  && po.collectionId == collectionId
                                                                  && po.accessoryId == parameterId
                                                                  && (po.status.Equals("Approved") || po.status.Equals("PartialCompleted")))
                                                          .Select(s => new VMTrnGoodReceiveNoteItem
                                                          {
                                                              purchaseOrderId = s.TrnPurchaseOrder.id,
                                                              orderQuantity = s.balanceQuantity,
                                                              rate = (decimal)s.rate,
                                                              rateWithGST = s.rateWithGST,
                                                              orderType = s.orderType,
                                                              gst = s.gst,
                                                              purchaseOrderNumber = s.TrnPurchaseOrder.orderNumber
                                                          }).ToList();
            }
            return poItemDetails;
        }

        public ResponseMessage postGRN(VMTrnGoodReceiveNote goodReceiveNote)
        {
            using (var transaction = new TransactionScope())
            {
                TrnGoodReceiveNote goodReceiveNoteToPost = Mapper.Map<VMTrnGoodReceiveNote, TrnGoodReceiveNote>(goodReceiveNote);
                var goodReceiveNoteItems = goodReceiveNoteToPost.TrnGoodReceiveNoteItems.ToList();

                goodReceiveNoteItems.ForEach(grnItems =>
                {
                    grnItems.matSizeId = grnItems.matSizeId == -1 ? null : grnItems.matSizeId;     //set null for custom matSize
                    grnItems.createdOn = DateTime.Now;
                    grnItems.createdBy = _LoggedInuserId;
                    updateStatusAndBalQtyForPOItem(grnItems);
                    addItemInProductDetails(grnItems, goodReceiveNoteToPost.locationId);
                });

                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= goodReceiveNote.grnDate && f.endDate >= goodReceiveNote.grnDate).FirstOrDefault();
                string orderNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.grnNumber,"GR");
                goodReceiveNoteToPost.grnNumber = orderNo;
                goodReceiveNoteToPost.createdOn = DateTime.Now;
                goodReceiveNoteToPost.createdBy = _LoggedInuserId;

                repo.TrnGoodReceiveNotes.Add(goodReceiveNoteToPost);

                financialYear.grnNumber += 1;
                repo.SaveChanges();

                mailNotificationForPendingGIN(goodReceiveNoteToPost);

                transaction.Complete();
                return new ResponseMessage(goodReceiveNoteToPost.id, resourceManager.GetString("GRNAdded"), ResponseType.Success);
            }
        }

        public void updateStatusAndBalQtyForPOItem(TrnGoodReceiveNoteItem grnItem)
        {
            TrnPurchaseOrderItem poItem = repo.TrnPurchaseOrderItems.Where(po => po.categoryId == grnItem.categoryId
                                                                          && po.collectionId == grnItem.collectionId
                                                                          && po.shadeId == grnItem.shadeId
                                                                          && po.fomSizeId == grnItem.fomSizeId
                                                                          && po.matSizeId == grnItem.matSizeId
                                                                          && po.accessoryId == grnItem.accessoryId
                                                                          && po.matQualityId == grnItem.matQualityId
                                                                          && po.matThicknessId == grnItem.matThicknessId
                                                                          && po.matSizeCode.Equals(grnItem.matSizeCode)
                                                                          && po.purchaseOrderId == grnItem.purchaseOrderId).FirstOrDefault();

            poItem.balanceQuantity = grnItem.receivedQuantity > poItem.balanceQuantity ? 0 : poItem.balanceQuantity - grnItem.receivedQuantity;
            poItem.status = poItem.balanceQuantity > 0 && poItem.balanceQuantity < poItem.orderQuantity ?
                                PurchaseOrderStatus.PartialCompleted.ToString() : PurchaseOrderStatus.Completed.ToString();
            poItem.updatedBy = _LoggedInuserId;
            poItem.updatedOn = DateTime.Now;
            repo.SaveChanges();

            //Set PO status Completed, if all its item's status is completed or closed
            //else set PO status PartialCompleted
            int poItemCount = repo.TrnPurchaseOrderItems.Where(po => po.purchaseOrderId == grnItem.purchaseOrderId).Count();
            int completePOItemCount = repo.TrnPurchaseOrderItems.Where(po => po.purchaseOrderId == grnItem.purchaseOrderId && (po.status.Equals("Completed") || po.status.Equals("Closed"))).Count();
            if (poItemCount == completePOItemCount)
            {
                poItem.TrnPurchaseOrder.status = PurchaseOrderStatus.Completed.ToString();
            }
            else
            {
                poItem.TrnPurchaseOrder.status = PurchaseOrderStatus.PartialCompleted.ToString();
            }
            repo.SaveChanges();


        }

        public void addItemInProductDetails(TrnGoodReceiveNoteItem grnItem, Int64 locationId)
        {
            TrnProductStockDetail productStockDetail = new TrnProductStockDetail();
            productStockDetail.categoryId = grnItem.categoryId;
            productStockDetail.collectionId = grnItem.collectionId;
            productStockDetail.accessoryId = grnItem.accessoryId;
            productStockDetail.fwrShadeId = grnItem.shadeId;
            productStockDetail.fomSizeId = grnItem.fomSizeId;
            productStockDetail.matSizeId = grnItem.matSizeId;
            productStockDetail.qualityId = grnItem.matQualityId;
            productStockDetail.matThicknessId = grnItem.matThicknessId;
            productStockDetail.matSizeCode = grnItem.matSizeCode;
            productStockDetail.locationId = locationId;
            productStockDetail.stock = grnItem.receivedQuantity;
            productStockDetail.stockInKg = grnItem.fomQuantityInKG;
            productStockDetail.kgPerUnit = grnItem.fomQuantityInKG != null ? grnItem.fomQuantityInKG / grnItem.receivedQuantity : null;
            productStockDetail.createdBy = _LoggedInuserId;
            productStockDetail.createdOn = DateTime.Now;

            repo.TrnProductStockDetails.Add(productStockDetail);

            _productStockService.updatePOItemInStockForGRN(grnItem);

        }

        public void mailNotificationForPendingGIN(TrnGoodReceiveNote goodReceiveNote)
        {
            List<string> ginNumbers = new List<string>();
            foreach (var grnItem in goodReceiveNote.TrnGoodReceiveNoteItems)
            {
                ginNumbers = repo.TrnGoodIssueNoteItems.Where(ginItem => ginItem.categoryId == grnItem.categoryId
                                             && ginItem.collectionId == grnItem.collectionId
                                             && ginItem.shadeId == grnItem.shadeId
                                             && ginItem.fomSizeId == grnItem.fomSizeId
                                             && ginItem.matSizeId == grnItem.matSizeId
                                             && ginItem.accessoryId == grnItem.accessoryId
                                             && ginItem.matQualityId == grnItem.matQualityId
                                             && ginItem.matThicknessId == grnItem.matThicknessId
                                             && ginItem.sizeCode.Equals(grnItem.matSizeCode)
                                             && ginItem.orderQuantity <= grnItem.receivedQuantity
                                             && ginItem.status.Equals("Created")).Select(gin => gin.TrnGoodIssueNote.ginNumber).ToList();
            }
            ginNumbers.Distinct();
            string adminEmail = repo.MstUsers.Where(u => u.userName.Equals("Administrator")).FirstOrDefault().email;
            emailNotification.notificationForPendingGIN(goodReceiveNote.grnNumber, "NotificationForPendingGIN", ginNumbers, adminEmail);
        }

        public List<VMLookUpItem> getCustomMatSizeCodeLookup(Int64 categoryId, Int64 collectionId, Int64 matQualityId, Int64 matThicknessId)
        {
            return repo.TrnPurchaseOrderItems.Where(poItem => poItem.categoryId == categoryId
                                                    && poItem.collectionId == collectionId
                                                    && poItem.matQualityId == matQualityId
                                                    && poItem.matThicknessId == matThicknessId
                                                    && (poItem.status.Equals("Approved") || poItem.status.Equals("PartialCompleted")))
                                            .Select(p => new VMLookUpItem
                                            {
                                                label = p.matSizeCode,
                                                value = -1
                                            }).ToList();
                                            
        }

    }
}