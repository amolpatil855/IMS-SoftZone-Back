﻿using IMSWebApi.Common;
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
    public class TrnCurtainQuotationService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        bool _IsAdministrator;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;
        SendEmail emailNotification = null;

        public TrnCurtainQuotationService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            _IsAdministrator = HttpContext.Current.User.IsInRole("Administrator");
            generateOrderNumber = new GenerateOrderNumber();
            emailNotification = new SendEmail();
        }

        public ListResult<VMTrnCurtainQuotationList> getCurtainQuotations(int pageSize, int page, string search)
        {
            List<VMTrnCurtainQuotationList> curtainQuotationView;

            curtainQuotationView = repo.TrnCurtainQuotations.Where(cq => !string.IsNullOrEmpty(search)
                    ? cq.curtainQuotationNumber.StartsWith(search)
                    || cq.MstCustomer.name.StartsWith(search)
                    || cq.status.StartsWith(search)
                    || cq.totalAmount.ToString().StartsWith(search) : true)
                    .Select(cq => new VMTrnCurtainQuotationList
                    {
                        id = cq.id,
                        curtainQuotationNumber = cq.curtainQuotationNumber,
                        curtainQuotationDate = cq.curtainQuotationDate,
                        customerName = cq.MstCustomer != null ? cq.MstCustomer.name : null,
                        totalAmount = cq.totalAmount,
                        status = cq.status
                    })
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMTrnCurtainQuotationList>
            {
                Data = curtainQuotationView,
                TotalCount = repo.TrnCurtainQuotations.Where(cq => !string.IsNullOrEmpty(search)
                    ? cq.curtainQuotationNumber.StartsWith(search)
                    || cq.MstCustomer.name.StartsWith(search)
                    || cq.status.StartsWith(search)
                    || cq.totalAmount.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public List<VMTrnCurtainQuotationList> getCurtainQuotationLookup()
        {
            var result = repo.TrnCurtainQuotations.Where(mq => mq.status.Equals("Created") || mq.status.Equals("Approved"))
                .Select(cq => new VMTrnCurtainQuotationList
                {
                    id = cq.id,
                    curtainQuotationNumber = cq.curtainQuotationNumber,
                    totalAmount = cq.totalAmount,
                    customerName = cq.MstCustomer != null ? cq.MstCustomer.name : null,
                    customerId = cq.MstCustomer.id,
                    balanceAmount = cq.totalAmount - cq.TrnAdvancePayments.Select(ap => ap.amount).DefaultIfEmpty(0).Sum()
                })
                    .OrderByDescending(o => o.id)
                    .ToList();
            return result;
        }

        public List<VMProductForCS> getRodAccessoryForCQ()
        {
            return repo.MstAccessories.Where(a => a.name.ToLower().Contains("rod") || a.itemCode.ToLower().Contains("rod"))
                                        .Select(a => new VMProductForCS
                                        {
                                            accessoryId = a.id,
                                            itemCode = a.itemCode,
                                            sellingRate = a.sellingRate,
                                            gst = a.MstHsn.gst
                                        }).ToList();
        }

        public List<VMProductForCS> getTrackAccessoryForCQ()
        {
            return repo.MstAccessories.Where(a => a.name.ToLower().Contains("track") || a.itemCode.ToLower().Contains("track"))
                                        .Select(a => new VMProductForCS
                                        {
                                            accessoryId = a.id,
                                            itemCode = a.itemCode,
                                            sellingRate = a.sellingRate,
                                            gst = a.MstHsn.gst
                                        }).ToList();
        }

        public VMTrnCurtainQuotation getCurtainQuotationById(Int64 id)
        {
            var result = repo.TrnCurtainQuotations.Where(cq => cq.id == id).FirstOrDefault();
            VMTrnCurtainQuotation curtainQuotationView = Mapper.Map<TrnCurtainQuotation, VMTrnCurtainQuotation>(result);
            curtainQuotationView.customerName = result.MstCustomer.name;
            curtainQuotationView.curtainSelectionNo = result.TrnCurtainSelection != null ? result.TrnCurtainSelection.curtainSelectionNumber : null;
            curtainQuotationView.TrnCurtainQuotationItems.ForEach(cqItem =>
            {
                cqItem.categoryName = cqItem.MstCategory.name;
                cqItem.collectionName = cqItem.collectionId != null ? cqItem.MstCollection.collectionCode : null;
                cqItem.serialno = cqItem.MstCategory.code.Equals("Fabric") ? cqItem.MstFWRShade.serialNumber + "(" + cqItem.MstFWRShade.shadeCode + "-" + cqItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
            });
            curtainQuotationView.advanceAmount = repo.TrnAdvancePayments.Where(ap => ap.materialQuotationId == id).Select(ap => ap.amount).DefaultIfEmpty(0).Sum();
            curtainQuotationView.TrnCurtainQuotationItems.ForEach(cqItem => cqItem.TrnCurtainQuotation = null);
            curtainQuotationView.TrnCurtainSelection.TrnCurtainQuotations = null;
            curtainQuotationView.TrnCurtainSelection.TrnCurtainSelectionItems.ForEach(csItems => csItems.TrnCurtainSelection = null);
            return curtainQuotationView;
        }

        public ResponseMessage postCurtainQuotation(VMTrnCurtainQuotation curtainQuotation)
        {
            using (var transaction = new TransactionScope())
            {
                TrnCurtainQuotation curtainQuotationToPost = Mapper.Map<VMTrnCurtainQuotation, TrnCurtainQuotation>(curtainQuotation);
                var curtainQuotationItems = curtainQuotationToPost.TrnCurtainQuotationItems.ToList();

                foreach (var cqItems in curtainQuotationItems)
                {
                    cqItems.balanceQuantity = Convert.ToInt64(cqItems.orderQuantity) + Convert.ToInt64(cqItems.horizontalPatchQuantity) + Convert.ToInt64(cqItems.verticalPatchQuantity);
                    cqItems.deliverQuantity = 0;
                    cqItems.status = CurtainQuotationStatus.Created.ToString();
                    cqItems.createdOn = DateTime.Now;
                    cqItems.createdBy = _LoggedInuserId;
                }
                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= curtainQuotationToPost.curtainQuotationDate.Date && f.endDate >= curtainQuotationToPost.curtainQuotationDate.Date).FirstOrDefault();
                string curtainQuotationNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.curtainQuotationNumber, "CQ");
                curtainQuotationToPost.curtainQuotationNumber = curtainQuotationNo;
                curtainQuotationToPost.status = CurtainQuotationStatus.Created.ToString();
                curtainQuotationToPost.financialYear = financialYear.financialYear;
                curtainQuotationToPost.createdOn = DateTime.Now;
                curtainQuotationToPost.createdBy = _LoggedInuserId;

                var curtainSelection = repo.TrnCurtainSelections.Where(ms => ms.id == curtainQuotation.curtainSelectionId).FirstOrDefault();
                curtainSelection.isQuotationCreated = true;

                repo.TrnCurtainQuotations.Add(curtainQuotationToPost);

                financialYear.curtainQuotationNumber += 1;
                repo.SaveChanges();

                // Mail Notification 
                //MstUser loggedInUser = repo.MstUsers.Where(u => u.id == _LoggedInuserId).FirstOrDefault();
                //string adminEmail = repo.MstUsers.Where(u => u.userName.Equals("Administrator")).FirstOrDefault().email;
                //emailNotification.notifyAdminForCreatedMQ(materialQuotation, "NotifyAdminForCreatedMQ", loggedInUser, adminEmail, materialQuotationNo);

                transaction.Complete();
                return new ResponseMessage(curtainQuotationToPost.id, resourceManager.GetString("CQCreated"), ResponseType.Success);
            }
        }

        public ResponseMessage putCurtainQuotation(VMTrnCurtainQuotation curtainQuotation)
        {
            using (var transaction = new TransactionScope())
            {
                var curtainQuotationToPut = repo.TrnCurtainQuotations.Where(ms => ms.id == curtainQuotation.id).FirstOrDefault();
                curtainQuotationToPut.curtainQuotationDate = curtainQuotation.curtainQuotationDate;
                curtainQuotationToPut.referById = curtainQuotation.referById;
                curtainQuotationToPut.totalAmount = curtainQuotation.totalAmount;

                updateCQItems(curtainQuotation);

                curtainQuotationToPut.updatedBy = _LoggedInuserId;
                curtainQuotationToPut.updatedOn = DateTime.Now;

                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(curtainQuotation.id, resourceManager.GetString("CQUpdated"), ResponseType.Success);
            }
        }

        public void updateCQItems(VMTrnCurtainQuotation curtainQuotation)
        {
            var curtainQuotationToPut = repo.TrnCurtainQuotations.Where(q => q.id == curtainQuotation.id).FirstOrDefault();

            List<TrnCurtainQuotationItem> itemsToRemove = new List<TrnCurtainQuotationItem>();
            foreach (var cqItem in curtainQuotationToPut.TrnCurtainQuotationItems)
            {
                if (curtainQuotation.TrnCurtainQuotationItems.Any(y => y.id == cqItem.id))
                {
                    continue;
                }
                else
                {
                    itemsToRemove.Add(cqItem);
                }
            }

            repo.TrnCurtainQuotationItems.RemoveRange(itemsToRemove);
            repo.SaveChanges();

            curtainQuotation.TrnCurtainQuotationItems.ForEach(x =>
            {
                if (curtainQuotationToPut.TrnCurtainQuotationItems.Any(y => y.id == x.id))
                {
                    var cqItemToPut = repo.TrnCurtainQuotationItems.Where(p => p.id == x.id).FirstOrDefault();

                    cqItemToPut.area = x.area;
                    cqItemToPut.unit = x.unit;
                    cqItemToPut.patternId = x.patternId;
                    cqItemToPut.categoryId = x.categoryId;
                    cqItemToPut.collectionId = x.collectionId;
                    cqItemToPut.shadeId = x.shadeId;
                    cqItemToPut.accessoryId = x.accessoryId;
                    cqItemToPut.isPatch = x.isPatch;

                    cqItemToPut.isVerticalPatch = x.isVerticalPatch;
                    cqItemToPut.noOfVerticalPatch = x.noOfVerticalPatch;
                    cqItemToPut.verticalPatchWidth = x.verticalPatchWidth;
                    cqItemToPut.verticalPatchQuantity = x.verticalPatchQuantity;
                    cqItemToPut.verticalPatchAmount = x.verticalPatchAmount;
                    cqItemToPut.verticalPatchAmountWithGST = x.verticalPatchAmountWithGST;

                    cqItemToPut.isHorizontalPatch = x.isHorizontalPatch;
                    cqItemToPut.noOfHorizontalPatch = x.noOfHorizontalPatch;
                    cqItemToPut.horizontalPatchHeight = x.horizontalPatchHeight;
                    cqItemToPut.horizontalPatchQuantity = x.horizontalPatchQuantity;
                    cqItemToPut.horizontalPatchAmount = x.horizontalPatchAmount;
                    cqItemToPut.horizontalPatchAmountWithGST = x.horizontalPatchAmountWithGST;
                    
                    cqItemToPut.isLining = x.isLining;
                    cqItemToPut.isTrack = x.isTrack;
                    cqItemToPut.unitHeight = x.unitHeight;
                    cqItemToPut.unitWidth = x.unitWidth;
                    cqItemToPut.isRod = x.isRod;
                    cqItemToPut.orderQuantity = x.orderQuantity;
                    cqItemToPut.deliverQuantity = x.deliverQuantity;
                    cqItemToPut.balanceQuantity = Convert.ToInt64(x.orderQuantity) + Convert.ToInt64(x.horizontalPatchQuantity) + Convert.ToInt64(x.verticalPatchQuantity);
                    cqItemToPut.orderType = x.orderType;
                    cqItemToPut.rate = x.rate;
                    cqItemToPut.discount = x.discount;
                    cqItemToPut.amount = x.amount;
                    cqItemToPut.rateWithGST = x.rateWithGST;
                    cqItemToPut.amountWithGST = x.amountWithGST;
                    cqItemToPut.gst = x.gst;
                    cqItemToPut.status = x.status;
                    cqItemToPut.updatedOn = DateTime.Now;
                    cqItemToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
                else
                {
                    TrnCurtainQuotationItem cqItem = Mapper.Map<VMTrnCurtainQuotationItem, TrnCurtainQuotationItem>(x);
                    cqItem.curtainQuotationId = curtainQuotation.id;
                    cqItem.status = CurtainQuotationStatus.Created.ToString();
                    cqItem.balanceQuantity = cqItem.orderQuantity != null ? cqItem.orderQuantity : (Convert.ToInt64(cqItem.horizontalPatchQuantity) + Convert.ToInt64(cqItem.verticalPatchQuantity));
                    cqItem.deliverQuantity = 0;
                    cqItem.createdBy = _LoggedInuserId;
                    cqItem.createdOn = DateTime.Now;
                    repo.TrnCurtainQuotationItems.Add(cqItem);
                    repo.SaveChanges();
                }
            });
        }
    }
}