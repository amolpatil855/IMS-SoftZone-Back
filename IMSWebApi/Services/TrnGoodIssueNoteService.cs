using IMSWebApi.Common;
using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.ViewModel;
using AutoMapper;
using System.Transactions;
using IMSWebApi.Enums;
using IMSWebApi.ViewModel.SalesInvoice;

namespace IMSWebApi.Services
{
    public class TrnGoodIssueNoteService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;
        SendEmail emailNotification = new SendEmail();
        TrnProductStockService _trnProductStockService = null;
        TrnSalesInvoiceService _trnSalesInvoiceService = null;

        public TrnGoodIssueNoteService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
            _trnProductStockService = new TrnProductStockService();
            _trnSalesInvoiceService = new TrnSalesInvoiceService();
        }

        public ListResult<VMTrnGoodIssueNoteList> getGoodIssueNotes(int pageSize, int page, string search)
        {
            List<VMTrnGoodIssueNoteList> goodIssueNoteView;
            goodIssueNoteView = repo.TrnGoodIssueNotes.Where(gin => !string.IsNullOrEmpty(search)
                    ? gin.ginNumber.StartsWith(search)
                    || gin.MstCustomer.name.StartsWith(search)
                    || gin.salesOrderNumber.StartsWith(search)
                    || gin.materialQuotationNumber.StartsWith(search)
                    || gin.workOrderNumber.StartsWith(search)
                    || gin.status.StartsWith(search) : true)
                    .Select(gin => new VMTrnGoodIssueNoteList
                    {
                        id = gin.id,
                        ginNumber = gin.ginNumber,
                        ginDate = gin.ginDate,
                        salesOrderNumber = gin.TrnSaleOrder != null ? gin.TrnSaleOrder.orderNumber : string.Empty,
                        materialQuotationNumber = gin.TrnMaterialQuotation != null ? gin.TrnMaterialQuotation.materialQuotationNumber : string.Empty,
                        workOrderNumber = gin.TrnWorkOrder != null ? gin.TrnWorkOrder.workOrderNumber : string.Empty,
                        customerName = gin.MstCustomer.name,
                        status = gin.status
                    })
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMTrnGoodIssueNoteList>
            {
                Data = goodIssueNoteView,
                TotalCount = repo.TrnGoodIssueNotes.Where(gin => !string.IsNullOrEmpty(search)
                    ? gin.ginNumber.StartsWith(search)
                    || gin.MstCustomer.name.StartsWith(search)
                    || gin.salesOrderNumber.StartsWith(search)
                    || gin.materialQuotationNumber.StartsWith(search)
                    || gin.workOrderNumber.StartsWith(search)
                    || gin.status.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnGoodIssueNote getGoodIssueNoteById(Int64 id)
        {
            var result = repo.TrnGoodIssueNotes.Where(gin => gin.id == id).FirstOrDefault();
            decimal stockAvailable = 0;
            
            VMTrnGoodIssueNote goodIssueNoteView = Mapper.Map<TrnGoodIssueNote, VMTrnGoodIssueNote>(result);
            goodIssueNoteView.TrnGoodIssueNoteItems.ForEach(ginItems => ginItems.TrnGoodIssueNote = null);

            goodIssueNoteView.TrnGoodIssueNoteItems.ForEach(ginItem =>
            {
                ginItem.categoryName = ginItem.MstCategory.name;
                ginItem.collectionName = ginItem.collectionId != null ? ginItem.MstCollection.collectionCode + " (" + ginItem.MstCollection.MstSupplier.code + ")" : null;
                ginItem.serialno = ginItem.MstCategory.code.Equals("Fabric")
                                || ginItem.MstCategory.code.Equals("Rug")
                                || ginItem.MstCategory.code.Equals("Wallpaper")
                                ? ginItem.MstFWRShade.serialNumber + "(" + ginItem.MstFWRShade.shadeCode + "-" + ginItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                ginItem.size = ginItem.MstMatSize != null ? ginItem.MstMatSize.sizeCode + " (" + ginItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + ginItem.MstMatSize.MstQuality.qualityCode + ")" :
                            ginItem.MstFomSize != null ? ginItem.MstFomSize.itemCode : 
                            ginItem.sizeCode != null ? ginItem.sizeCode + " (" + ginItem.MstMatThickness.thicknessCode + "-" + ginItem.MstQuality.qualityCode + ")" : null;
                ginItem.accessoryName = ginItem.accessoryId != null ? ginItem.MstAccessory.itemCode : null;

                stockAvailable = repo.TrnProductStocks.Where(p => p.categoryId == ginItem.categoryId
                                                                     && p.collectionId == ginItem.collectionId
                                                                     && p.fwrShadeId == ginItem.shadeId
                                                                     && p.fomSizeId == ginItem.fomSizeId
                                                                     && p.matSizeId == ginItem.matSizeId
                                                                     && p.matSizeCode.Equals(ginItem.sizeCode)
                                                                     && p.qualityId == ginItem.matQualityId
                                                                     && p.matThicknessId == ginItem.matThicknessId
                                                                     && p.accessoryId == ginItem.accessoryId).FirstOrDefault().stock;
                ginItem.availableStock = stockAvailable;
            });
            if (goodIssueNoteView.TrnMaterialQuotation != null)
            {
                goodIssueNoteView.TrnMaterialQuotation.TrnMaterialSelection = null;
                goodIssueNoteView.TrnMaterialQuotation.TrnMaterialQuotationItems.ForEach(mqItem => mqItem.TrnMaterialQuotation = null);
            }
            if (goodIssueNoteView.TrnWorkOrder != null)
            {
                goodIssueNoteView.TrnWorkOrder.TrnWorkOrderItems.ForEach(woItems => woItems.TrnWorkOrder = null);
                goodIssueNoteView.TrnWorkOrder.MstTailor.MstTailorPatternChargeDetails.ForEach(tpDetails => tpDetails.MstTailor = null);
                goodIssueNoteView.TrnWorkOrder.TrnCurtainQuotation.TrnCurtainSelection.TrnCurtainSelectionItems.ForEach(csItems => csItems.TrnCurtainSelection = null);
                goodIssueNoteView.TrnWorkOrder.TrnCurtainQuotation.TrnCurtainSelection.TrnCurtainQuotations = null;
                goodIssueNoteView.TrnWorkOrder.TrnCurtainQuotation.TrnCurtainQuotationItems.ForEach(cqItems => cqItems.TrnCurtainQuotation = null);
            }
            return goodIssueNoteView;
        }

        public void postGoodIssueNote(VMTrnSaleOrder saleOrder, VMTrnMaterialQuotation materialQuotation)
        {
            using (var transaction = new TransactionScope())
            {
                TrnGoodIssueNote goodIssueNoteToPost = new TrnGoodIssueNote();
                goodIssueNoteToPost.customerId = saleOrder != null ? saleOrder.customerId : materialQuotation.customerId;
                goodIssueNoteToPost.salesOrderId = saleOrder != null ? saleOrder.id : (long?)null;
                goodIssueNoteToPost.salesOrderNumber = saleOrder != null ? saleOrder.orderNumber : null;
                goodIssueNoteToPost.materialQuotationId = materialQuotation != null ? materialQuotation.id : (long?)null;
                goodIssueNoteToPost.materialQuotationNumber = materialQuotation != null ? materialQuotation.materialQuotationNumber : null;
                DateTime dateForFinancialYear = saleOrder != null ? saleOrder.orderDate : materialQuotation.materialQuotationDate;
                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= dateForFinancialYear.Date && f.endDate >= dateForFinancialYear.Date).FirstOrDefault();
                string orderNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.ginNumber, "GI");
                goodIssueNoteToPost.ginNumber = orderNo;
                goodIssueNoteToPost.ginDate = DateTime.Now;
                goodIssueNoteToPost.status = GINStatus.Created.ToString();
                goodIssueNoteToPost.financialYear = financialYear.financialYear;
                goodIssueNoteToPost.createdOn = DateTime.Now;
                goodIssueNoteToPost.createdBy = _LoggedInuserId;

                if (saleOrder != null)
                {
                    saleOrder.TrnSaleOrderItems.ForEach(soItem =>
                    {
                        if (soItem.balanceQuantity > 0)
                        {
                            TrnGoodIssueNoteItem ginItem = new TrnGoodIssueNoteItem();
                            ginItem.categoryId = soItem.categoryId;
                            ginItem.collectionId = soItem.collectionId;
                            ginItem.shadeId = soItem.shadeId;
                            ginItem.fomSizeId = soItem.fomSizeId;
                            ginItem.matSizeId = soItem.matSizeId;
                            ginItem.sizeCode = soItem.sizeCode;
                            ginItem.accessoryId = soItem.accessoryId;
                            ginItem.orderQuantity = Convert.ToDecimal(soItem.balanceQuantity);
                            ginItem.issuedQuantity = 0;
                            ginItem.rate = Convert.ToDecimal(soItem.rate);
                            ginItem.discountPercentage = soItem.discountPercentage;
                            ginItem.amount = 0;
                            ginItem.status = GINStatus.Created.ToString();
                            ginItem.createdOn = DateTime.Now;
                            ginItem.createdBy = _LoggedInuserId;

                            goodIssueNoteToPost.TrnGoodIssueNoteItems.Add(ginItem);
                        }
                    });
                }
                else
                {
                    materialQuotation.TrnMaterialQuotationItems.ForEach(mqItem =>
                        {
                            if (mqItem.balanceQuantity > 0)
                            {
                                TrnGoodIssueNoteItem ginItem = new TrnGoodIssueNoteItem();
                                ginItem.categoryId = mqItem.categoryId;
                                ginItem.collectionId = mqItem.collectionId;
                                ginItem.shadeId = mqItem.shadeId;
                                ginItem.fomSizeId = null;
                                ginItem.matSizeId = mqItem.matSizeId;
                                ginItem.matQualityId = mqItem.qualityId;
                                ginItem.matThicknessId = mqItem.matThicknessId;
                                ginItem.sizeCode = mqItem.matSizeId == null && (mqItem.matHeight != null && mqItem.matWidth != null) ? (mqItem.matHeight + "x" + mqItem.matWidth) : null;
                                ginItem.accessoryId = null;
                                ginItem.orderQuantity = mqItem.balanceQuantity;
                                ginItem.issuedQuantity = 0;
                                ginItem.rate = mqItem.rate;
                                ginItem.discountPercentage = mqItem.discountPercentage;
                                ginItem.amount = 0;
                                ginItem.status = GINStatus.Created.ToString();
                                ginItem.createdOn = DateTime.Now;
                                ginItem.createdBy = _LoggedInuserId;

                                goodIssueNoteToPost.TrnGoodIssueNoteItems.Add(ginItem);
                            }
                        });
                }
                repo.TrnGoodIssueNotes.Add(goodIssueNoteToPost);
                financialYear.ginNumber += 1;

                repo.SaveChanges();
                transaction.Complete();
            }
        }

        //Create GIN for approved WorkOrder
        public void postGoodIssueNoteForWO(TrnWorkOrder workOrder)
        {
            using (var transaction = new TransactionScope())
            {
                TrnGoodIssueNote goodIssueNoteToPost = new TrnGoodIssueNote();
                goodIssueNoteToPost.customerId = workOrder.customerId;
                goodIssueNoteToPost.workOrderId = workOrder != null ? workOrder.id : (long?)null;
                goodIssueNoteToPost.workOrderNumber = workOrder != null ? workOrder.workOrderNumber : null;
                DateTime dateForFinancialYear = workOrder != null ? workOrder.workOrderDate : DateTime.Now;
                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= dateForFinancialYear.Date && f.endDate >= dateForFinancialYear.Date).FirstOrDefault();
                string orderNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.ginNumber, "GI");
                goodIssueNoteToPost.ginNumber = orderNo;
                goodIssueNoteToPost.ginDate = DateTime.Now;
                goodIssueNoteToPost.status = GINStatus.Created.ToString();
                goodIssueNoteToPost.financialYear = financialYear.financialYear;
                goodIssueNoteToPost.createdOn = DateTime.Now;
                goodIssueNoteToPost.createdBy = _LoggedInuserId;

                if (workOrder != null)
                {
                    List<TrnGoodIssueNoteItem> ginItems = workOrder.TrnWorkOrderItems.Where(woItem => woItem.balanceQuantity > 0).GroupBy(g => new
                                                                                            {
                                                                                                g.categoryId,
                                                                                                g.collectionId,
                                                                                                g.shadeId,
                                                                                                g.accessoryId
                                                                                            }).Select(group => new TrnGoodIssueNoteItem
                                                                                            {
                                                                                                categoryId = group.Key.categoryId,
                                                                                                collectionId = group.Key.collectionId,
                                                                                                shadeId = group.Key.shadeId,
                                                                                                accessoryId = group.Key.accessoryId,
                                                                                                orderQuantity = Convert.ToDecimal(group.Sum(woItem => woItem.balanceQuantity))
                                                                                            }).ToList();
                    ginItems.ForEach(ginItem =>
                    {
                        ginItem.status = GINStatus.Created.ToString();
                        ginItem.createdOn = DateTime.Now;
                        ginItem.createdBy = _LoggedInuserId;
                    });
                    goodIssueNoteToPost.TrnGoodIssueNoteItems = ginItems;
                }
                
                repo.TrnGoodIssueNotes.Add(goodIssueNoteToPost);
                financialYear.ginNumber += 1;

                repo.SaveChanges();
                transaction.Complete();
            }
        }

        public ResponseMessage putGoodIssueNote(VMTrnGoodIssueNote goodIssueNote)
        {
            if (goodIssueNote.TrnGoodIssueNoteItems.Where(ginItems => ginItems.issuedQuantity > 0).Count() > 0)
            {
                using (var transaction = new TransactionScope())
                {
                    var goodIssueNoteToPut = repo.TrnGoodIssueNotes.Where(q => q.id == goodIssueNote.id).FirstOrDefault();

                    goodIssueNoteToPut.status = GINStatus.Completed.ToString();

                    updateGINItems(goodIssueNote);

                    goodIssueNoteToPut.updatedOn = DateTime.Now;
                    goodIssueNoteToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                    if (goodIssueNoteToPut.workOrderId == null)
                    {
                        _trnSalesInvoiceService.createInvoiceForGIN(goodIssueNote);
                    }
                    createGINForRemainingItems(goodIssueNote.salesOrderId, goodIssueNote.materialQuotationId, goodIssueNote.workOrderId);    
                    
                    transaction.Complete();
                    return new ResponseMessage(goodIssueNote.id, resourceManager.GetString("InvoiceCreated"), ResponseType.Success);
                }
            }
            else
            {
                return new ResponseMessage(goodIssueNote.id, resourceManager.GetString("GINnotUpdated"), ResponseType.Error);
            }
        }

        public void updateGINItems(VMTrnGoodIssueNote goodIssueNote)
        {
            //var goodIssueNoteToPut = repo.TrnGoodIssueNotes.Where(q => q.id == goodIssueNote.id).FirstOrDefault();

            goodIssueNote.TrnGoodIssueNoteItems.ForEach(x =>
            {
                if (x.issuedQuantity == 0 || x.issuedQuantity == null)
                {
                    repo.TrnGoodIssueNoteItems.Remove(repo.TrnGoodIssueNoteItems.Where(g => g.id == x.id).FirstOrDefault());
                    repo.SaveChanges();
                }
                else
                {
                    var ginItemToPut = repo.TrnGoodIssueNoteItems.Where(p => p.id == x.id).FirstOrDefault();

                    ginItemToPut.issuedQuantity = x.issuedQuantity;
                    //ginItemToPut.amount = Convert.ToInt32(Math.Round((x.rate - (x.rate * (Convert.ToDecimal(x.discountPercentage) / 100))) * (decimal)x.issuedQuantity));
                    decimal discountAmt = (x.rate * Convert.ToDecimal(x.issuedQuantity)) - ((x.rate * Convert.ToDecimal(x.issuedQuantity) * Convert.ToDecimal(x.discountPercentage)) / 100);
                    ginItemToPut.amount = Convert.ToInt64(Math.Round(discountAmt, MidpointRounding.AwayFromZero));
                    ginItemToPut.status = GINStatus.Completed.ToString();
                    ginItemToPut.statusChangeDate = DateTime.Now;
                    ginItemToPut.updatedOn = DateTime.Now;
                    ginItemToPut.updatedBy = _LoggedInuserId;

                    repo.SaveChanges();
                    if (goodIssueNote.salesOrderId != null)
                    {
                        updateStatusAndBalQtyForSOItem(ginItemToPut);
                    }
                    else if(goodIssueNote.materialQuotationId != null)
                    {
                        updateStatusAndBalQtyForMQItem(ginItemToPut);
                    }
                    else if(goodIssueNote.workOrderId != null)
                    {
                        updateStatusAndBalQtyForWOItem(ginItemToPut);       
                    }
                    _trnProductStockService.SubtractItemQtyFromStockDetailsForGIN(ginItemToPut);
                }
            });
        }

        public void updateStatusAndBalQtyForSOItem(TrnGoodIssueNoteItem ginItem)
        {
            TrnSaleOrderItem soItem = repo.TrnSaleOrderItems.Where(so => so.categoryId == ginItem.categoryId
                                                                          && so.collectionId == ginItem.collectionId
                                                                          && so.shadeId == ginItem.shadeId
                                                                          && so.fomSizeId == ginItem.fomSizeId
                                                                          && so.matSizeId == ginItem.matSizeId
                                                                          && so.accessoryId == ginItem.accessoryId
                                                                          && so.sizeCode.Equals(ginItem.sizeCode)
                                                                          && so.saleOrderId == ginItem.TrnGoodIssueNote.salesOrderId).FirstOrDefault();

            soItem.balanceQuantity = ginItem.issuedQuantity > soItem.balanceQuantity ? 0 : soItem.balanceQuantity - ginItem.issuedQuantity;
            soItem.deliverQuantity += ginItem.issuedQuantity;
            soItem.status = soItem.balanceQuantity == 0 ? SaleOrderStatus.Completed.ToString() : soItem.status;
            soItem.updatedBy = _LoggedInuserId;
            soItem.updatedOn = DateTime.Now;
            repo.SaveChanges();

            //Set SO status Completed, if all its item's status is completed or closed
            int soItemCount = repo.TrnSaleOrderItems.Where(so => so.saleOrderId == ginItem.TrnGoodIssueNote.salesOrderId).Count();
            int completeSOItemCount = repo.TrnSaleOrderItems.Where(so => so.saleOrderId == ginItem.TrnGoodIssueNote.salesOrderId && (so.status.Equals("Completed"))).Count();
            if (soItemCount == completeSOItemCount)
            {
                soItem.TrnSaleOrder.status = SaleOrderStatus.Completed.ToString();
            }
            repo.SaveChanges();
        }

        public void updateStatusAndBalQtyForMQItem(TrnGoodIssueNoteItem ginItem)
        {
            TrnMaterialQuotationItem mqItem = new TrnMaterialQuotationItem();
            if (ginItem.matSizeId != null || ginItem.shadeId != null)
            {
                mqItem = repo.TrnMaterialQuotationItems.Where(mq => mq.categoryId == ginItem.categoryId
                                                                          && mq.collectionId == ginItem.collectionId
                                                                          && mq.shadeId == ginItem.shadeId
                                                                          && mq.matSizeId == ginItem.matSizeId
                                                                          && mq.materialQuotationId == ginItem.TrnGoodIssueNote.materialQuotationId).FirstOrDefault();
            }
            else if(ginItem.matSizeId == null && ginItem.sizeCode != null)
            {
                mqItem = repo.TrnMaterialQuotationItems.Where(mq => mq.categoryId == ginItem.categoryId
                                                                          && mq.collectionId == ginItem.collectionId
                                                                          && mq.shadeId == ginItem.shadeId
                                                                          && (mq.matHeight + "x" + mq.matWidth).Equals(ginItem.sizeCode)
                                                                          && mq.qualityId == ginItem.matQualityId
                                                                          && mq.matThicknessId == ginItem.matThicknessId
                                                                          && mq.materialQuotationId == ginItem.TrnGoodIssueNote.materialQuotationId).FirstOrDefault();
            }

            mqItem.balanceQuantity = ginItem.issuedQuantity > mqItem.balanceQuantity ? 0 : mqItem.balanceQuantity - Convert.ToDecimal(ginItem.issuedQuantity);
            mqItem.deliverQuantity += Convert.ToDecimal(ginItem.issuedQuantity);
            mqItem.status = mqItem.balanceQuantity == 0 ? MaterialQuotationStatus.Completed.ToString() : mqItem.status;
            mqItem.updatedBy = _LoggedInuserId;
            mqItem.updatedOn = DateTime.Now;
            repo.SaveChanges();

            //Set MQ status Completed, if all its item's status is completed or closed
            int mqItemCount = repo.TrnMaterialQuotationItems.Where(mq => mq.materialQuotationId == ginItem.TrnGoodIssueNote.materialQuotationId).Count();
            int completeMQItemCount = repo.TrnMaterialQuotationItems.Where(mq => mq.materialQuotationId == ginItem.TrnGoodIssueNote.materialQuotationId && (mq.status.Equals("Completed"))).Count();
            if (mqItemCount == completeMQItemCount)
            {
                mqItem.TrnMaterialQuotation.status = MaterialQuotationStatus.Completed.ToString();
            }
            repo.SaveChanges();
        }

        public void updateStatusAndBalQtyForWOItem(TrnGoodIssueNoteItem ginItem)
        {
            List<TrnWorkOrderItem> woItems = repo.TrnWorkOrderItems.Where(woItem => woItem.workOrderId == ginItem.TrnGoodIssueNote.workOrderId
                                                                          && woItem.categoryId == ginItem.categoryId
                                                                          && woItem.collectionId == ginItem.collectionId
                                                                          && woItem.shadeId == ginItem.shadeId
                                                                          && woItem.accessoryId == ginItem.accessoryId
                                                                          && woItem.balanceQuantity > 0).ToList();


            decimal balQty = Convert.ToDecimal(ginItem.issuedQuantity);
            int i = 0;
            while(balQty>0)
            {
                if (balQty > Convert.ToDecimal(woItems[i].balanceQuantity))
                {
                    balQty = balQty - Convert.ToDecimal(woItems[i].balanceQuantity);
                    woItems[i].deliverQuantity += woItems[i].balanceQuantity;
                    woItems[i].balanceQuantity = 0;
                }
                else
                {
                    woItems[i].balanceQuantity = woItems[i].balanceQuantity - balQty;
                    woItems[i].deliverQuantity += balQty; 
                    balQty = 0;
                }
                i++;
            }

            //Set WO status Completed, if all its item's status is completed or closed
            int woItemCount = repo.TrnWorkOrderItems.Where(wo => wo.workOrderId == ginItem.TrnGoodIssueNote.workOrderId).Count();
            int completeWOItemCount = repo.TrnWorkOrderItems.Where(wo => wo.workOrderId == ginItem.TrnGoodIssueNote.workOrderId && (wo.orderQuantity == wo.deliverQuantity)).Count();
            if (woItemCount == completeWOItemCount)
            {
                ginItem.TrnGoodIssueNote.TrnWorkOrder.status = WorkOrderStatus.Completed.ToString();
            }
            repo.SaveChanges();
        }

        //Create GIN for remaining Items in SalesOrder OR Material Quotation OR Work Order
        public void createGINForRemainingItems(Int64? salesOrderId,Int64? materialQuotationId, Int64? workOrderId)
        {
            if (salesOrderId != null)
            {
                TrnSaleOrder saleOrder = repo.TrnSaleOrders.Where(so => so.id == salesOrderId && so.status.Equals("Approved")).FirstOrDefault();
                if (saleOrder != null)
                {
                    //int itemsWithBalQty = saleOrder.TrnSaleOrderItems.Where(soItems => soItems.balanceQuantity != 0).Count();
                    VMTrnSaleOrder VMSaleOrder = Mapper.Map<TrnSaleOrder, VMTrnSaleOrder>(saleOrder);
                    postGoodIssueNote(VMSaleOrder, null);
                }
            }
            else if(materialQuotationId != null)
            {
                TrnMaterialQuotation materialQuotation = repo.TrnMaterialQuotations.Where(mq => mq.id == materialQuotationId && mq.status.Equals("Approved")).FirstOrDefault();
                if (materialQuotation != null)
                {
                    //int itemsWithBalQty = saleOrder.TrnSaleOrderItems.Where(soItems => soItems.balanceQuantity != 0).Count();
                    VMTrnMaterialQuotation VMMaterialQuotation = Mapper.Map<TrnMaterialQuotation, VMTrnMaterialQuotation>(materialQuotation);
                    postGoodIssueNote(null, VMMaterialQuotation);
                }
            }
            else if (workOrderId != null)
            {
                TrnWorkOrder workOrder = repo.TrnWorkOrders.Where(wo => wo.id == workOrderId && wo.status.Equals("Approved")).FirstOrDefault();
                if (workOrder != null)
                {
                    postGoodIssueNoteForWO(workOrder);
                }
            }

            
        }

        //List of GINs whose items physical stock is available/greater than orderQuantity
        public ListResult<VMTrnGoodIssueNoteList> getGINsForItemsWithStockAvailable(int pageSize, int page, string search)
        {
            List<Int64> ginIdsForItemswithAvailableStock = new List<Int64>();
            List<VMTrnGoodIssueNoteList> ginsWitStockAvailable;
            var ginItemWithStatusCreated = repo.TrnGoodIssueNoteItems.Where(ginItems => ginItems.status.Equals(SaleOrderStatus.Created.ToString())).ToList();

            ginItemWithStatusCreated.ForEach(ginItem =>
            {
                decimal stockAvailable = 0;
                stockAvailable = repo.TrnProductStocks.Where(p => p.categoryId == ginItem.categoryId
                                                                    && p.collectionId == ginItem.collectionId
                                                                    && p.fwrShadeId == ginItem.shadeId
                                                                    && p.fomSizeId == ginItem.fomSizeId
                                                                    && p.matSizeId == ginItem.matSizeId
                                                                    && p.qualityId == ginItem.matQualityId
                                                                    && p.matThicknessId == ginItem.matThicknessId
                                                                    && p.matSizeCode.Equals(ginItem.sizeCode)
                                                                    && p.accessoryId == ginItem.accessoryId).FirstOrDefault().stock;

                if (stockAvailable >= ginItem.orderQuantity)
                {
                    ginIdsForItemswithAvailableStock.Add(ginItem.TrnGoodIssueNote.id);
                }
            });

            ginIdsForItemswithAvailableStock = ginIdsForItemswithAvailableStock.Distinct().ToList();
            ginsWitStockAvailable = repo.TrnGoodIssueNotes.Where(gin => !string.IsNullOrEmpty(search)
                                                            ? gin.ginNumber.StartsWith(search)
                                                            || gin.MstCustomer.name.StartsWith(search)
                                                            || gin.salesOrderNumber.StartsWith(search)
                                                            || gin.materialQuotationNumber.StartsWith(search)
                                                            || gin.workOrderNumber.StartsWith(search)
                                                            || gin.status.StartsWith(search) : true
                                                            && ginIdsForItemswithAvailableStock.Contains(gin.id))
                                                            .Select(gin => new VMTrnGoodIssueNoteList
                                                            {
                                                                id = gin.id,
                                                                ginNumber = gin.ginNumber,
                                                                ginDate = gin.ginDate,
                                                                salesOrderNumber = gin.TrnSaleOrder != null ? gin.TrnSaleOrder.orderNumber : null,
                                                                materialQuotationNumber = gin.TrnMaterialQuotation != null ? gin.TrnMaterialQuotation.materialQuotationNumber : null,
                                                                workOrderNumber = gin.TrnWorkOrder != null ? gin.TrnWorkOrder.workOrderNumber : string.Empty,
                                                                customerName = gin.MstCustomer.name,
                                                                status = gin.status
                                                            })
                                                            .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMTrnGoodIssueNoteList>
            {
                Data = ginsWitStockAvailable,
                TotalCount = ginsWitStockAvailable.Count(),
                Page = page
            };
        }

        //List of GIN items whose physical stock is available / greater than orderQuantity
        public List<VMTrnGoodIssueNoteItem> getGINItemsWithAvailableInStockByginId(Int64 ginId)
        {
            var gin = repo.TrnGoodIssueNotes.Where(g => g.id == ginId).FirstOrDefault();

            List<VMTrnGoodIssueNoteItem> ginItemsWithAvailableInStock = new List<VMTrnGoodIssueNoteItem>();
            foreach (var ginItem in gin.TrnGoodIssueNoteItems)
            {
                decimal stockAvailable = repo.TrnProductStocks.Where(p => p.categoryId == ginItem.categoryId
                                                                     && p.collectionId == ginItem.collectionId
                                                                     && p.fwrShadeId == ginItem.shadeId
                                                                     && p.fomSizeId == ginItem.fomSizeId
                                                                     && p.matSizeId == ginItem.matSizeId
                                                                     && p.qualityId == ginItem.matQualityId
                                                                     && p.matThicknessId == ginItem.matThicknessId
                                                                     && p.matSizeCode.Equals(ginItem.sizeCode)
                                                                     && p.accessoryId == ginItem.accessoryId).FirstOrDefault().stock;
                if (stockAvailable >= ginItem.orderQuantity)
                {
                    VMTrnGoodIssueNoteItem VMGinItem = Mapper.Map<TrnGoodIssueNoteItem, VMTrnGoodIssueNoteItem>(ginItem);
                    VMGinItem.availableStock = stockAvailable;
                    VMGinItem.categoryName = VMGinItem.MstCategory.name;
                    VMGinItem.collectionName = VMGinItem.collectionId != null ? VMGinItem.MstCollection.collectionCode : null;
                    VMGinItem.serialno = VMGinItem.MstCategory.code.Equals("Fabric")
                                    || VMGinItem.MstCategory.code.Equals("Rug")
                                    || VMGinItem.MstCategory.code.Equals("Wallpaper")
                                    ? VMGinItem.MstFWRShade.serialNumber + "(" + VMGinItem.MstFWRShade.shadeCode + "-" + VMGinItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                    VMGinItem.size = VMGinItem.MstMatSize != null ? VMGinItem.MstMatSize.sizeCode + " (" + VMGinItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + VMGinItem.MstMatSize.MstQuality.qualityCode + ")" :
                                VMGinItem.MstFomSize != null ? VMGinItem.MstFomSize.itemCode : null;
                    VMGinItem.accessoryName = VMGinItem.accessoryId != null ? VMGinItem.MstAccessory.itemCode : null;
                    VMGinItem.TrnGoodIssueNote.TrnGoodIssueNoteItems = null;
                    ginItemsWithAvailableInStock.Add(VMGinItem);
                }
            }
            return ginItemsWithAvailableInStock;
        }
    }
}