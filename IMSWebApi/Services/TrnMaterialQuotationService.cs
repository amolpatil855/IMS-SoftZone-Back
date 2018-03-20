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
        bool _IsAdministrator;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;
        TrnProductStockService _trnProductStockService = null;
        TrnGoodIssueNoteService _trnGoodIssueNoteServie = null;
        
        public TrnMaterialQuotationService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            _IsAdministrator = HttpContext.Current.User.IsInRole("Administrator");
            generateOrderNumber = new GenerateOrderNumber();
            _trnProductStockService = new TrnProductStockService();
            _trnGoodIssueNoteServie = new TrnGoodIssueNoteService();
        }

        public ListResult<VMTrnMaterialQuotationList> getMaterialQuotations(int pageSize, int page, string search)
        {
            List<VMTrnMaterialQuotationList> materialQuotationView;

            materialQuotationView = repo.TrnMaterialQuotations.Where(mq => !string.IsNullOrEmpty(search)
                    ? mq.materialQuotationNumber.StartsWith(search)
                    || mq.MstCustomer.name.StartsWith(search)
                    || mq.status.StartsWith(search) : true)
                    .Select(mq => new VMTrnMaterialQuotationList
                    {
                        id = mq.id,
                        materialQuotationNumber = mq.materialQuotationNumber,
                        materialQuotationDate = mq.materialQuotationDate,
                        customerName = mq.MstCustomer != null ? mq.MstCustomer.name : null,
                        totalAmount = mq.totalAmount,
                        status = mq.status
                    })
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMTrnMaterialQuotationList>
            {
                Data = materialQuotationView,
                TotalCount = materialQuotationView.Count(),
                Page = page
            };
        }

        public List<VMLookUpItem> getMaterialQuotationLookup()
        {
            return repo.TrnMaterialQuotations.
                    Select(mq => new VMLookUpItem
                    {
                        label = mq.materialQuotationNumber,
                        value = mq.id
                    })
                    .OrderByDescending(o=>o.value)
                    .ToList();
        }

        public VMLookUpItem getCustomerLookupByMaterialQuotationId(Int64 materialQuotationId)
        {
            return repo.TrnMaterialQuotations.Where(mq => mq.id == materialQuotationId)
                    .Select(c => new VMLookUpItem
                    {
                        label = c.customerId != null ? c.MstCustomer.name : string.Empty,
                        value = c.customerId
                    }).OrderByDescending(o => o.label).FirstOrDefault();
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
            materialQuotationView.TrnMaterialSelection.TrnMaterialQuotations = null;
            materialQuotationView.TrnMaterialSelection.TrnMaterialSelectionItems.ForEach(msItems => msItems.TrnMaterialSelection = null);
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
                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= materialQuotationToPost.materialQuotationDate && f.endDate >= materialQuotationToPost.materialQuotationDate).FirstOrDefault();
                string materialQuotationNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.materialSelectionNumber, "MQ");
                materialQuotationToPost.materialQuotationNumber = materialQuotationNo;
                materialQuotationToPost.status = MaterialQuotationStatus.Created.ToString();
                materialQuotationToPost.createdOn = DateTime.Now;
                materialQuotationToPost.createdBy = _LoggedInuserId;
                
                var materialSelection = repo.TrnMaterialSelections.Where(ms => ms.id == materialQuotation.materialSelectionId).FirstOrDefault();
                materialSelection.isQuotationCreated = true;

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
                materialQuotationToPut.materialQuotationDate = materialQuotation.materialQuotationDate;
                materialQuotationToPut.referById = materialQuotation.referById;
                materialQuotationToPut.totalAmount = materialQuotation.totalAmount;

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
                    mqItemToPut.orderType = x.orderType;
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

        public ResponseMessage approveMaterialQuotation(Int64 id)
        {
            using (var transaction = new TransactionScope())
            {
                var materialQuotation = repo.TrnMaterialQuotations.Where(mq => mq.id == id).FirstOrDefault();
                materialQuotation.status = MaterialQuotationStatus.Approved.ToString();
                foreach (var mqItem in materialQuotation.TrnMaterialQuotationItems)
                {
                    mqItem.status = MaterialQuotationStatus.Approved.ToString();
                    if (!(mqItem.categoryId == 4 && mqItem.matSizeId == null))
                    {
                        _trnProductStockService.AddsoOrmqIteminStock(null,mqItem);    
                    }
                    mqItem.updatedOn = DateTime.Now;
                    mqItem.updatedBy = _LoggedInuserId;
                }
                repo.SaveChanges();
                VMTrnMaterialQuotation VMMaterialQuotation = Mapper.Map<TrnMaterialQuotation, VMTrnMaterialQuotation>(materialQuotation);
                _trnGoodIssueNoteServie.postGoodIssueNote(null, VMMaterialQuotation);

                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("MQApproved"), ResponseType.Success);
            }
        }

        public ResponseMessage cancelMaterialQuotation(Int64 id)
        {
            String messageToDisplay;
            ResponseType type;
            using (var transaction = new TransactionScope())
            {
                var materialQuotation = repo.TrnMaterialQuotations.Where(so => so.id == id).FirstOrDefault();
                if (materialQuotation.status.Equals("Created"))
                {
                    materialQuotation.status = SaleOrderStatus.Cancelled.ToString();
                    foreach (var mqItem in materialQuotation.TrnMaterialQuotationItems)
                    {
                        mqItem.status = MaterialQuotationStatus.Cancelled.ToString();
                    }
                    messageToDisplay = "MQCancelled";
                    type = ResponseType.Success;

                    VMTrnMaterialQuotation VMMaterialQuotation = Mapper.Map<TrnMaterialQuotation, VMTrnMaterialQuotation>(materialQuotation);
                    //emailNotification.cancelledSONotificationForCustomer(VMsaleOrder, "CancelledSONotificationForCustomer");
                }
                else if (materialQuotation.status.Equals("Approved") && _IsAdministrator)
                {
                    int itemCountWithOrderQtyNotEqualBalQty = materialQuotation.TrnMaterialQuotationItems.Where(mqItem => mqItem.orderQuantity != mqItem.balanceQuantity).Count();
                    if (itemCountWithOrderQtyNotEqualBalQty == 0)
                    {
                        materialQuotation.status = MaterialQuotationStatus.Cancelled.ToString();
                        foreach (var mqItem in materialQuotation.TrnMaterialQuotationItems)
                        {
                            mqItem.status = MaterialQuotationStatus.Cancelled.ToString();
                            _trnProductStockService.SubSOItemFromStock(null,mqItem);
                        }
                        var ginToUpdate = repo.TrnGoodIssueNotes.Where(gin => gin.materialQuotationId == materialQuotation.id && gin.status.Equals("Created"))
                                    .FirstOrDefault();
                        ginToUpdate.status = GINStatus.Cancelled.ToString();

                        foreach (var ginItem in ginToUpdate.TrnGoodIssueNoteItems)
                        {
                            ginItem.status = GINStatus.Cancelled.ToString();
                            ginItem.statusChangeDate = DateTime.Now;
                            ginItem.updatedOn = DateTime.Now;
                            ginItem.updatedBy = _LoggedInuserId;
                        }

                        messageToDisplay = "MQCancelled";
                        type = ResponseType.Success;

                        //VMTrnSaleOrder VMsaleOrder = Mapper.Map<TrnSaleOrder, VMTrnSaleOrder>(saleOrder);
                        //emailNotification.cancelledSONotificationForCustomer(VMsaleOrder, "CancelledSONotificationForCustomer");
                    }
                    else
                    {
                        messageToDisplay = "GINExists";
                        type = ResponseType.Error;
                    }
                }
                else
                {
                    messageToDisplay = "MQApprovedByAdmin";
                    type = ResponseType.Error;
                }

                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString(messageToDisplay), type);
            }
        }
    }
}