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

        public ListResult<VMTrnGoodIssueNote> getGoodIssueNotes(int pageSize, int page, string search)
        {
            List<VMTrnGoodIssueNote> goodIssueNoteView;
            if (pageSize > 0)
            {
                var result = repo.TrnGoodIssueNotes.Where(gin => !string.IsNullOrEmpty(search)
                    ? gin.ginNumber.StartsWith(search)
                    || gin.MstCustomer.name.StartsWith(search)
                    || gin.salesOrderNumber.StartsWith(search)
                    || gin.status.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                goodIssueNoteView = Mapper.Map<List<TrnGoodIssueNote>, List<VMTrnGoodIssueNote>>(result);
            }
            else
            {
                var result = repo.TrnGoodIssueNotes.Where(gin => !string.IsNullOrEmpty(search)
                    ? gin.ginNumber.StartsWith(search)
                    || gin.MstCustomer.name.StartsWith(search)
                    || gin.salesOrderNumber.StartsWith(search)
                    || gin.status.StartsWith(search) : true).ToList();
                goodIssueNoteView = Mapper.Map<List<TrnGoodIssueNote>, List<VMTrnGoodIssueNote>>(result);
            }
            goodIssueNoteView.ForEach(gin => gin.TrnGoodIssueNoteItems.ForEach(ginItems => ginItems.TrnGoodIssueNote = null));
            return new ListResult<VMTrnGoodIssueNote>
            {
                Data = goodIssueNoteView,
                TotalCount = repo.TrnGoodIssueNotes.Where(gin => !string.IsNullOrEmpty(search)
                    ? gin.ginNumber.StartsWith(search)
                    || gin.MstCustomer.name.StartsWith(search)
                    || gin.salesOrderNumber.StartsWith(search)
                    || gin.status.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnGoodIssueNote getGoodIssueNoteById(Int64 id)
        {
            var result = repo.TrnGoodIssueNotes.Where(gin => gin.id == id).FirstOrDefault();
            VMTrnGoodIssueNote goodIssueNoteView = Mapper.Map<TrnGoodIssueNote, VMTrnGoodIssueNote>(result);
            goodIssueNoteView.TrnGoodIssueNoteItems.ForEach(ginItems => ginItems.TrnGoodIssueNote = null);

            goodIssueNoteView.TrnGoodIssueNoteItems.ForEach(ginItem =>
            {
                ginItem.categoryName = ginItem.MstCategory.name;
                ginItem.collectionName = ginItem.collectionId != null ? ginItem.MstCollection.collectionName : null;
                ginItem.serialno = ginItem.MstCategory.code.Equals("Fabric")
                                || ginItem.MstCategory.code.Equals("Rug")
                                || ginItem.MstCategory.code.Equals("Wallpaper")
                                ? ginItem.MstFWRShade.serialNumber + "(" + ginItem.MstFWRShade.shadeCode + ")" : null;
                ginItem.size = ginItem.MstMatSize != null ? ginItem.MstMatSize.sizeCode + " (" + ginItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + ginItem.MstMatSize.MstQuality.qualityCode + ")" :
                            ginItem.MstFomSize != null ? ginItem.MstFomSize.itemCode : null;
                ginItem.accessoryName = ginItem.accessoryId != null ? ginItem.MstAccessory.name : null;
            });


            return goodIssueNoteView;
        }

        public void postGoodIssueNote(VMTrnSaleOrder saleOrder)
        {
            using (var transaction = new TransactionScope())
            {
                TrnGoodIssueNote goodIssueNoteToPost = new TrnGoodIssueNote();
                goodIssueNoteToPost.customerId = saleOrder.customerId;
                goodIssueNoteToPost.salesOrderId = saleOrder.id;
                goodIssueNoteToPost.salesOrderNumber = saleOrder.orderNumber;
                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= saleOrder.orderDate && f.endDate >= saleOrder.orderDate).FirstOrDefault();
                string orderNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.ginNumber);
                goodIssueNoteToPost.ginNumber = orderNo;
                goodIssueNoteToPost.ginDate = DateTime.Now;
                goodIssueNoteToPost.status = GINStatus.Created.ToString();
                goodIssueNoteToPost.createdOn = DateTime.Now;
                goodIssueNoteToPost.createdBy = _LoggedInuserId;

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
                        ginItem.amount = Convert.ToInt32(soItem.amount);
                        ginItem.status = GINStatus.Created.ToString();
                        ginItem.createdOn = DateTime.Now;
                        ginItem.createdBy = _LoggedInuserId;

                        goodIssueNoteToPost.TrnGoodIssueNoteItems.Add(ginItem);
                    }
                });
                repo.TrnGoodIssueNotes.Add(goodIssueNoteToPost);
                financialYear.ginNumber += 1;

                repo.SaveChanges();
                transaction.Complete();
            }
        }

        public ResponseMessage putGoodIssueNote(VMTrnGoodIssueNote goodIssueNote)
        {
            if (goodIssueNote.TrnGoodIssueNoteItems.Where(ginItems=>ginItems.issuedQuantity > 0).Count() > 0)
            {
                using (var transaction = new TransactionScope())
                {
                    var goodIssueNoteToPut = repo.TrnGoodIssueNotes.Where(q => q.id == goodIssueNote.id).FirstOrDefault();

                    goodIssueNoteToPut.status = GINStatus.Completed.ToString();

                    updateGINItems(goodIssueNote);

                    goodIssueNoteToPut.updatedOn = DateTime.Now;
                    goodIssueNoteToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();

                    _trnSalesInvoiceService.createInvoiceForGIN(goodIssueNote);

                    createGINForRemainingItems(goodIssueNote.salesOrderId);

                    transaction.Complete();
                    return new ResponseMessage(goodIssueNote.id, resourceManager.GetString("InvoiceCreated"), ResponseType.Success);
                }
            }
            else
            {
                return new ResponseMessage(goodIssueNote.id, "Please enter issued quantity for atleast one item", ResponseType.Error);
            }
        }

        public void updateGINItems(VMTrnGoodIssueNote goodIssueNote)
        {
            var goodIssueNoteToPut = repo.TrnGoodIssueNotes.Where(q => q.id == goodIssueNote.id).FirstOrDefault();

            goodIssueNote.TrnGoodIssueNoteItems.ForEach(x =>
            {
                var ginItemToPut = repo.TrnGoodIssueNoteItems.Where(p => p.id == x.id).FirstOrDefault();

                ginItemToPut.issuedQuantity = x.issuedQuantity;
                ginItemToPut.amount = x.amount;
                ginItemToPut.status = GINStatus.Completed.ToString();
                ginItemToPut.statusChangeDate = DateTime.Now;
                ginItemToPut.updatedOn = DateTime.Now;
                ginItemToPut.updatedBy = _LoggedInuserId;
                repo.SaveChanges();

                updateStatusAndBalQtyForSOItem(ginItemToPut);
                _trnProductStockService.SubtractItemQtyFromStockDetailsForGIN(ginItemToPut);
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

        public void createGINForRemainingItems(Int64 salesOrderId)
        {
            TrnSaleOrder saleOrder = repo.TrnSaleOrders.Where(so => so.id == salesOrderId && so.status.Equals("Approved")).FirstOrDefault();

            if (saleOrder != null)
            {
                //int itemsWithBalQty = saleOrder.TrnSaleOrderItems.Where(soItems => soItems.balanceQuantity != 0).Count();
                VMTrnSaleOrder VMSaleOrder = Mapper.Map<TrnSaleOrder,VMTrnSaleOrder>(saleOrder);
                postGoodIssueNote(VMSaleOrder);
            }
        }

        //List of GIN Items whose physical stock is available
        public List<VMLookUpItem> getGINLookupForItemsWithStockAvailable()
        {
            List<VMLookUpItem> ginForItemswithAvailableStock = new List<VMLookUpItem>();

            var ginItemWithStatusCreated = repo.TrnGoodIssueNoteItems.Where(ginItems => ginItems.status.Equals(SaleOrderStatus.Created.ToString())).ToList();

            ginItemWithStatusCreated.ForEach(ginItem =>
            {
                decimal stockAvailable = repo.TrnProductStocks.Where(p => p.categoryId == ginItem.categoryId
                                                                     && p.collectionId == ginItem.collectionId
                                                                     && p.fwrShadeId == ginItem.shadeId
                                                                     && p.fomSizeId == ginItem.fomSizeId
                                                                     && p.matSizeId == ginItem.matSizeId
                                                                     && p.accessoryId == ginItem.accessoryId).FirstOrDefault().stock;
                if (stockAvailable > ginItem.orderQuantity)
                {
                    ginForItemswithAvailableStock.Add(new VMLookUpItem { label = ginItem.TrnGoodIssueNote.ginNumber, value = ginItem.goodIssueNoteId });
                }
            });

            ginForItemswithAvailableStock = ginForItemswithAvailableStock.Distinct(new VMLookUpItem()).ToList();

            return ginForItemswithAvailableStock;
        }
    }
}