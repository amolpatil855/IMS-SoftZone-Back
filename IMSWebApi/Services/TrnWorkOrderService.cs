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
using IMSWebApi.Enums;
using System.Transactions;
using AutoMapper;

namespace IMSWebApi.Services
{
    public class TrnWorkOrderService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;
        SendEmail emailNotification = new SendEmail();
        TrnProductStockService _trnProductStockService = null;
        TrnGoodIssueNoteService _trnGoodIssueNoteService = null;

        public TrnWorkOrderService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
            _trnProductStockService = new TrnProductStockService();
            _trnGoodIssueNoteService = new TrnGoodIssueNoteService();
        }

        public ListResult<VMTrnWorkOrderList> getWorkOrders(int pageSize, int page, string search)
        {
            List<VMTrnWorkOrderList> workOrderView;
            var result = repo.TrnWorkOrders
                    .Select(wo => new VMTrnWorkOrderList
                    {
                        id = wo.id,
                        workOrderNumber = wo.workOrderNumber,
                        workOrderDate = wo.workOrderDate,
                        customerName = wo.MstCustomer.name,
                        tailorName = wo.MstTailor != null ? wo.MstTailor.name : string.Empty,
                        status = wo.status,
                    })
                    .Where(wo => !string.IsNullOrEmpty(search)
                    ? wo.workOrderNumber.StartsWith(search)
                    || wo.customerName.StartsWith(search)
                    || wo.tailorName.StartsWith(search)
                    || wo.status.StartsWith(search) : true)
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            workOrderView = result;

            return new ListResult<VMTrnWorkOrderList>
            {
                Data = workOrderView,
                TotalCount = repo.TrnWorkOrders.Where(wo => !string.IsNullOrEmpty(search)
                    ? wo.workOrderNumber.StartsWith(search)
                    || wo.MstCustomer.name.StartsWith(search)
                    || (wo.MstTailor != null ? wo.MstTailor.name.StartsWith(search) : true)
                    || wo.status.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnWorkOrder getWorkOrderById(Int64 id)
        {
            var result = repo.TrnWorkOrders.Where(wo => wo.id == id).FirstOrDefault();
            VMTrnWorkOrder workOrderView = Mapper.Map<TrnWorkOrder, VMTrnWorkOrder>(result);
            workOrderView.customerName = result.MstCustomer != null ? result.MstCustomer.name : string.Empty;
            workOrderView.curtainQuotationNo = result.TrnCurtainQuotation != null ? result.TrnCurtainQuotation.curtainQuotationNumber : string.Empty;
            workOrderView.TrnWorkOrderItems.ForEach(woItem =>
            {
                woItem.categoryName = woItem.MstCategory.name;
                woItem.collectionName = woItem.collectionId != null ? woItem.MstCollection.collectionCode : null;
                woItem.serialno = woItem.MstCategory.code.Equals("Fabric") ? woItem.MstFWRShade.serialNumber + "(" + woItem.MstFWRShade.shadeCode + "-" + woItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                woItem.itemCode = woItem.MstCategory.code.Equals("Accessories") ? woItem.MstAccessory.itemCode : null;
                woItem.patternName = woItem.MstPattern != null ? woItem.MstPattern.name : null;
            });
            workOrderView.TrnWorkOrderItems.ForEach(woItem => woItem.TrnWorkOrder = null);
            workOrderView.TrnCurtainQuotation.TrnCurtainQuotationItems.ForEach(cqItem => cqItem.TrnCurtainQuotation = null);
            workOrderView.TrnCurtainQuotation.TrnCurtainSelection = null;
            if (workOrderView.MstTailor != null)
            {
                workOrderView.MstTailor.MstTailorPatternChargeDetails.ForEach(tpDetails => tpDetails.MstTailor = null);    
            }
            return workOrderView;
        }

        public void createWorkOrder(TrnCurtainQuotation curtainQuotation)
        {
            using (var transaction = new TransactionScope())
            {
                if (curtainQuotation != null)
                {
                    TrnWorkOrder workOrder = new TrnWorkOrder();
                    workOrder.curtainQuotationId = curtainQuotation.id;

                    DateTime currentDate = DateTime.Now.Date;
                    var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= currentDate && f.endDate >= currentDate).FirstOrDefault();
                    string workOrderNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.jobCardNumber, "WO");
                    workOrder.workOrderNumber = workOrderNo;
                    workOrder.workOrderDate = DateTime.Now;
                    workOrder.expectedDeliveryDate = curtainQuotation.expectedDeliveryDate;
                    workOrder.isLabourChargesPaid = false;

                    workOrder.customerId = curtainQuotation.customerId;
                    workOrder.financialYear = financialYear.financialYear;

                    foreach (var cqItem in curtainQuotation.TrnCurtainQuotationItems)
                    {
                        TrnWorkOrderItem workOrderItems = new TrnWorkOrderItem();
                        workOrderItems.categoryId = cqItem.categoryId;
                        workOrderItems.rate = cqItem.rate;
                        if (cqItem.isRod || cqItem.isRodAccessory)
                        {
                            workOrderItems.accessoryId = cqItem.accessoryId;
                            workOrderItems.isPatch = workOrderItems.isVerticalPatch = workOrderItems.isHorizontalPatch = workOrderItems.isLining = workOrderItems.isTrack = workOrderItems.isRemote = workOrderItems.isMotor = false;
                            workOrderItems.isRod = cqItem.isRod;
                            workOrderItems.isRodAccessory = cqItem.isRodAccessory;
                            workOrderItems.orderQuantity = cqItem.orderQuantity;
                            workOrderItems.orderQuantity = adjustOrderQuantity(Convert.ToDecimal(workOrderItems.orderQuantity));
                            workOrderItems.balanceQuantity = workOrderItems.orderQuantity;
                            workOrderItems.deliverQuantity = 0;
                            workOrderItems.createdBy = _LoggedInuserId;
                            workOrderItems.createdOn = DateTime.Now;
                            workOrder.TrnWorkOrderItems.Add(workOrderItems);
                        }
                        else if (cqItem.isTrack || cqItem.isRemote || cqItem.isMotor)
                        {
                            workOrderItems.area = cqItem.area;
                            workOrderItems.unit = cqItem.unit;
                            workOrderItems.unitHeight = cqItem.unitHeight;
                            workOrderItems.unitWidth = cqItem.unitWidth;
                            workOrderItems.patternId = cqItem.patternId;
                            workOrderItems.accessoryId = cqItem.accessoryId;
                            workOrderItems.isPatch = workOrderItems.isVerticalPatch = workOrderItems.isHorizontalPatch = workOrderItems.isLining = workOrderItems.isRod = workOrderItems.isRodAccessory = false;
                            workOrderItems.isTrack = cqItem.isTrack;
                            workOrderItems.isRemote = cqItem.isRemote;
                            workOrderItems.isMotor = cqItem.isMotor;
                            workOrderItems.trackSize = cqItem.isTrack ? cqItem.unitWidth : null;  //track size will be width of unit
                            workOrderItems.orderQuantity = cqItem.orderQuantity;
                            workOrderItems.orderQuantity = adjustOrderQuantity(Convert.ToDecimal(workOrderItems.orderQuantity));
                            workOrderItems.balanceQuantity = workOrderItems.orderQuantity;
                            workOrderItems.deliverQuantity = 0;
                            workOrderItems.createdBy = _LoggedInuserId;
                            workOrderItems.createdOn = DateTime.Now;
                            workOrder.TrnWorkOrderItems.Add(workOrderItems);
                        }
                        else if ((cqItem.isLining || (!cqItem.isLining && !cqItem.isPatch)) && cqItem.shadeId != null)
                        {
                            workOrderItems.area = cqItem.area;
                            workOrderItems.unit = cqItem.unit;
                            workOrderItems.unitHeight = cqItem.unitHeight;
                            workOrderItems.unitWidth = cqItem.unitWidth;
                            workOrderItems.numberOfPanel = cqItem.numberOfPanel;
                            workOrderItems.patternId = cqItem.patternId;
                            workOrderItems.collectionId = cqItem.collectionId;
                            workOrderItems.shadeId = cqItem.shadeId;
                            workOrderItems.isPatch = workOrderItems.isVerticalPatch = workOrderItems.isHorizontalPatch = workOrderItems.isTrack = workOrderItems.isRod = workOrderItems.isRemote = workOrderItems.isMotor = workOrderItems.isRodAccessory = false;
                            workOrderItems.isLining = cqItem.isLining;
                            if (cqItem.MstFWRShade.MstQuality.width <= 100)
                            {
                                if (cqItem.isLining)
                                    workOrderItems.orderQuantity = Math.Round(Convert.ToDecimal(((cqItem.unitHeight + cqItem.MstPattern.woLiningHeight) / cqItem.MstPattern.meterPerInch)), 2);
                                else
                                    workOrderItems.orderQuantity = Math.Round(Convert.ToDecimal(((cqItem.unitHeight + cqItem.MstPattern.woFabricHeight) / cqItem.MstPattern.meterPerInch)), 2);
                                workOrderItems.orderQuantity = cqItem.numberOfPanel != null ? (workOrderItems.orderQuantity * cqItem.numberOfPanel) : workOrderItems.orderQuantity;
                            }
                            else
                            {
                                if (cqItem.isLining && cqItem.fabricDirection.Equals("Vertical"))
                                {
                                    workOrderItems.orderQuantity = Math.Round(Convert.ToDecimal(((54.00 * cqItem.numberOfPanel) / cqItem.MstPattern.meterPerInch)), 2);
                                    decimal qtyToBeAdded = Convert.ToDecimal(workOrderItems.orderQuantity);
                                    decimal fabricWidth = Convert.ToDecimal(cqItem.MstFWRShade.MstQuality.width);
                                    while (fabricWidth < cqItem.unitHeight)
                                    {
                                        fabricWidth = fabricWidth + Convert.ToDecimal(cqItem.MstFWRShade.MstQuality.width);
                                        workOrderItems.orderQuantity = workOrderItems.orderQuantity + qtyToBeAdded;
                                    }
                                }
                                else if (cqItem.isLining && cqItem.fabricDirection.Equals("Horizontal"))
                                    workOrderItems.orderQuantity = Math.Round(Convert.ToDecimal(((cqItem.unitHeight + cqItem.MstPattern.woLiningHeight) / cqItem.MstPattern.meterPerInch) * (Math.Ceiling(Convert.ToDecimal(cqItem.unitWidth / 50)))), 2);
                                else if (!cqItem.isLining && cqItem.fabricDirection.Equals("Vertical"))
                                {
                                    workOrderItems.orderQuantity = Math.Round(Convert.ToDecimal(((54.00 * cqItem.numberOfPanel) / cqItem.MstPattern.meterPerInch)), 2);
                                    decimal qtyToBeAdded = Convert.ToDecimal(workOrderItems.orderQuantity);
                                    decimal fabricWidth = Convert.ToDecimal(cqItem.MstFWRShade.MstQuality.width);
                                    while (fabricWidth < cqItem.unitHeight)
                                    {
                                        fabricWidth = fabricWidth + Convert.ToDecimal(cqItem.MstFWRShade.MstQuality.width);
                                        workOrderItems.orderQuantity = workOrderItems.orderQuantity + qtyToBeAdded;
                                    }
                                }
                                else if (!cqItem.isLining && cqItem.fabricDirection.Equals("Horizontal"))
                                    workOrderItems.orderQuantity = Math.Round(Convert.ToDecimal(((cqItem.unitHeight + cqItem.MstPattern.woFabricHeight) / cqItem.MstPattern.meterPerInch) * (Math.Ceiling(Convert.ToDecimal(cqItem.unitWidth / 50)))), 2);
                            }
                            workOrderItems.orderQuantity = adjustOrderQuantity(Convert.ToDecimal(workOrderItems.orderQuantity));
                            workOrderItems.balanceQuantity = workOrderItems.orderQuantity;
                            workOrderItems.deliverQuantity = 0;
                            workOrderItems.createdBy = _LoggedInuserId;
                            workOrderItems.createdOn = DateTime.Now;
                            workOrder.TrnWorkOrderItems.Add(workOrderItems);
                        }
                        else if (cqItem.isPatch && cqItem.shadeId != null)
                        {
                            workOrderItems.area = cqItem.area;
                            workOrderItems.unit = cqItem.unit;
                            workOrderItems.unitHeight = cqItem.unitHeight;
                            workOrderItems.unitWidth = cqItem.unitWidth;
                            workOrderItems.numberOfPanel = cqItem.numberOfPanel;
                            workOrderItems.patternId = cqItem.patternId;
                            workOrderItems.collectionId = cqItem.collectionId;
                            workOrderItems.shadeId = cqItem.shadeId;
                            workOrderItems.isTrack = workOrderItems.isRemote = workOrderItems.isMotor = workOrderItems.isLining = workOrderItems.isRod = workOrderItems.isRodAccessory = false;

                            workOrderItems.isPatch = cqItem.isPatch;
                            workOrderItems.isVerticalPatch = cqItem.isVerticalPatch;
                            workOrderItems.noOfVerticalPatch = cqItem.noOfVerticalPatch;
                            workOrderItems.verticalPatchWidth = cqItem.verticalPatchWidth;
                            workOrderItems.verticalPatchQuantity = cqItem.verticalPatchQuantity;
                            workOrderItems.isHorizontalPatch = cqItem.isHorizontalPatch;
                            workOrderItems.noOfHorizontalPatch = cqItem.noOfHorizontalPatch;
                            workOrderItems.horizontalPatchHeight = cqItem.horizontalPatchHeight;
                            workOrderItems.horizontalPatchQuantity = cqItem.horizontalPatchQuantity;
                            workOrderItems.orderQuantity = cqItem.orderQuantity;
                            workOrderItems.orderQuantity = adjustOrderQuantity(Convert.ToDecimal(workOrderItems.orderQuantity));
                            workOrderItems.balanceQuantity = workOrderItems.orderQuantity;
                            workOrderItems.deliverQuantity = 0;
                            workOrderItems.createdBy = _LoggedInuserId;
                            workOrderItems.createdOn = DateTime.Now;
                            workOrder.TrnWorkOrderItems.Add(workOrderItems);
                        }
                        else
                        {
                            workOrderItems.area = cqItem.area;
                            workOrderItems.unit = cqItem.unit;
                            workOrderItems.patternId = cqItem.patternId;
                            workOrderItems.accessoryId = cqItem.accessoryId;
                            workOrderItems.isPatch = workOrderItems.isVerticalPatch = workOrderItems.isHorizontalPatch = workOrderItems.isLining = workOrderItems.isRod = workOrderItems.isRodAccessory = false;
                            workOrderItems.isTrack = cqItem.isTrack;
                            workOrderItems.isRemote = cqItem.isRemote;
                            workOrderItems.isMotor = cqItem.isMotor;
                            workOrderItems.orderQuantity = cqItem.orderQuantity;
                            workOrderItems.orderQuantity = adjustOrderQuantity(Convert.ToDecimal(workOrderItems.orderQuantity));
                            workOrderItems.balanceQuantity = workOrderItems.orderQuantity;
                            workOrderItems.deliverQuantity = 0;
                            workOrderItems.createdBy = _LoggedInuserId;
                            workOrderItems.createdOn = DateTime.Now;
                            workOrder.TrnWorkOrderItems.Add(workOrderItems);
                        }


                        if (cqItem.isTrack)
                        {
                            var endCapAccessory = repo.MstAccessories.Where(a => a.name.Replace(" ", string.Empty).ToLower().Contains("endcap")
                                                                            || a.itemCode.Replace(" ", string.Empty).ToLower().Contains("endcap")).FirstOrDefault();
                            TrnWorkOrderItem woEndCap = new TrnWorkOrderItem();
                            woEndCap.unit = cqItem.unit;
                            woEndCap.area = cqItem.area;
                            woEndCap.unitHeight = cqItem.unitHeight;
                            woEndCap.unitWidth = cqItem.unitWidth;
                            woEndCap.patternId = cqItem.patternId;
                            woEndCap.categoryId = cqItem.categoryId;
                            woEndCap.accessoryId = endCapAccessory.id;
                            woEndCap.numberOfPanel = cqItem.numberOfPanel;
                            woEndCap.isPatch = woEndCap.isVerticalPatch = woEndCap.isHorizontalPatch = woEndCap.isLining = woEndCap.isRod = woEndCap.isRodAccessory = false;
                            woEndCap.isTrack = false;
                            woEndCap.isRemote = cqItem.isRemote;
                            woEndCap.isMotor = cqItem.isMotor;
                            woEndCap.trackSize = cqItem.isTrack ? cqItem.unitWidth : null;  //track size will be width of unit
                            woEndCap.orderQuantity = 2;
                            woEndCap.rate = endCapAccessory.sellingRate;
                            woEndCap.balanceQuantity = woEndCap.orderQuantity;
                            woEndCap.deliverQuantity = 0;
                            woEndCap.createdBy = _LoggedInuserId;
                            woEndCap.createdOn = DateTime.Now;
                            workOrder.TrnWorkOrderItems.Add(woEndCap);

                            var runnerAccessory = repo.MstAccessories.Where(a => a.name.Replace(" ", string.Empty).ToLower().Contains("runner")
                                                                            || a.itemCode.Replace(" ", string.Empty).ToLower().Contains("runner")).FirstOrDefault();
                            TrnWorkOrderItem woRunner = new TrnWorkOrderItem();
                            woRunner.unit = cqItem.unit;
                            woRunner.area = cqItem.area;
                            woRunner.unitHeight = cqItem.unitHeight;
                            woRunner.unitWidth = cqItem.unitWidth;
                            woRunner.patternId = cqItem.patternId;
                            woRunner.categoryId = cqItem.categoryId;
                            woRunner.accessoryId = runnerAccessory.id;
                            woRunner.numberOfPanel = cqItem.numberOfPanel;
                            woRunner.isPatch = woRunner.isVerticalPatch = woRunner.isHorizontalPatch = woRunner.isLining = woRunner.isRod = woRunner.isRodAccessory = false;
                            woRunner.isTrack = false;
                            woRunner.isRemote = cqItem.isRemote;
                            woRunner.isMotor = cqItem.isMotor;
                            woRunner.trackSize = cqItem.isTrack ? cqItem.unitWidth : null;  //track size will be width of unit
                            woRunner.orderQuantity = cqItem.numberOfPanel * 6;
                            woRunner.rate = runnerAccessory.sellingRate;
                            woRunner.balanceQuantity = woRunner.orderQuantity;
                            woRunner.deliverQuantity = 0;
                            woRunner.createdBy = _LoggedInuserId;
                            woRunner.createdOn = DateTime.Now;
                            workOrder.TrnWorkOrderItems.Add(woRunner);

                            var bracketAccessory = repo.MstAccessories.Where(a => a.name.Replace(" ", string.Empty).ToLower().Contains("bracket")
                                                                           || a.itemCode.Replace(" ", string.Empty).ToLower().Contains("bracket")).FirstOrDefault();
                            TrnWorkOrderItem woBracket = new TrnWorkOrderItem();
                            woBracket.unit = cqItem.unit;
                            woBracket.area = cqItem.area;
                            woBracket.unitHeight = cqItem.unitHeight;
                            woBracket.unitWidth = cqItem.unitWidth;
                            woBracket.patternId = cqItem.patternId;
                            woBracket.categoryId = cqItem.categoryId;
                            woBracket.accessoryId = bracketAccessory.id;
                            woBracket.numberOfPanel = cqItem.numberOfPanel;
                            woBracket.isPatch = woBracket.isVerticalPatch = woBracket.isHorizontalPatch = woBracket.isLining = woBracket.isRod = woBracket.isRodAccessory = false;
                            woBracket.isTrack = false;
                            woBracket.isRemote = cqItem.isRemote;
                            woBracket.isMotor = cqItem.isMotor;
                            woBracket.trackSize = cqItem.isTrack ? cqItem.unitWidth : null;  //track size will be width of unit
                            woBracket.orderQuantity = Math.Ceiling(Convert.ToDecimal(cqItem.unitWidth / 24)) < 2 ? 2 : Math.Ceiling(Convert.ToDecimal(cqItem.unitWidth / 24));
                            woBracket.rate = bracketAccessory.sellingRate;
                            woBracket.balanceQuantity = woBracket.orderQuantity;
                            woBracket.deliverQuantity = 0;
                            woBracket.createdBy = _LoggedInuserId;
                            woBracket.createdOn = DateTime.Now;
                            workOrder.TrnWorkOrderItems.Add(woBracket);
                        }
                    }
                    financialYear.jobCardNumber += 1;

                    workOrder.status = WorkOrderStatus.Created.ToString();
                    workOrder.createdBy = _LoggedInuserId;
                    workOrder.createdOn = DateTime.Now;
                    repo.TrnWorkOrders.Add(workOrder);
                    repo.SaveChanges();

                }
                transaction.Complete();
            }
        }

        public decimal adjustOrderQuantity(decimal orderQuantity)
        {
            if (orderQuantity != null)
            {
                decimal value = orderQuantity - Math.Floor(orderQuantity);
                if (value > Convert.ToDecimal(0.50))
                {
                    value = value * 10;
                    value = Math.Ceiling(value);
                    value = value / 10;
                    orderQuantity = Math.Floor(orderQuantity) + value;
                }
                else if (value > Convert.ToDecimal(0.00))
                    orderQuantity = orderQuantity + (Convert.ToDecimal(0.50) - value);
                return orderQuantity;
            }
            else
                return 0;
        }

        public ResponseMessage putWorkOrder(VMTrnWorkOrder wordOrder)
        {
            using (var transaction = new TransactionScope())
            {
                var workOrderToPut = repo.TrnWorkOrders.Where(wo => wo.id == wordOrder.id).FirstOrDefault();

                workOrderToPut.tailorId = wordOrder.tailorId;

                updateWOItems(wordOrder);

                workOrderToPut.updatedOn = DateTime.Now;
                workOrderToPut.updatedBy = _LoggedInuserId;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(wordOrder.id, resourceManager.GetString("WOUpdated"), ResponseType.Success);
            }
        }

        public void updateWOItems(VMTrnWorkOrder workOrder)
        {
            var workOrderToPut = repo.TrnWorkOrders.Where(so => so.id == workOrder.id).FirstOrDefault();
            workOrder.TrnWorkOrderItems.ForEach(x =>
            {
                if (workOrderToPut.TrnWorkOrderItems.Any(y => y.id == x.id))
                {
                    var woItemToPut = repo.TrnWorkOrderItems.Where(p => p.id == x.id).FirstOrDefault();

                    woItemToPut.labourCharges = x.labourCharges;
                    woItemToPut.updatedOn = DateTime.Now;
                    woItemToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
            });

        }

        public ResponseMessage approveWorkOrder(Int64 id)
        {
            using (var transaction = new TransactionScope())
            {
                var workOrder = repo.TrnWorkOrders.Where(wo => wo.id == id).FirstOrDefault();
                //string adminEmail = repo.MstUsers.Where(u => u.userName.Equals("Administrator")).FirstOrDefault().email;
                workOrder.status = WorkOrderStatus.Approved.ToString();
                foreach (var woItem in workOrder.TrnWorkOrderItems)
                {
                    _trnProductStockService.AddwoIteminStock(woItem);
                }
                repo.SaveChanges();
                _trnGoodIssueNoteService.postGoodIssueNoteForWO(workOrder);

                //string customerEmail = saleOrder.MstCustomer.email;

                //emailNotification.approvedSONotificationForCustomer(VMSaleOrder, "ApprovedSONotificationForCustomer", customerEmail, adminEmail, saleOrder.orderNumber);

                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("WOApproved"), ResponseType.Success);
            }
        }
    }
}