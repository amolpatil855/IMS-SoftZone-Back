using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using AutoMapper;
using IMSWebApi.ViewModel;
using System.Transactions;
using IMSWebApi.Enums;
using IMSWebApi.ViewModel.SlaesOrder;
using IMSWebApi.ViewModel.SalesInvoice;

namespace IMSWebApi.Services
{
    public class TrnSaleOrderService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        bool _IsCustomer;
        bool _IsAdministrator;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;
        SendEmail emailNotification = null;
        TrnProductStockService _trnProductStockService = null;
        TrnGoodIssueNoteService _trnGoodIssueNoteServie = null;

        public TrnSaleOrderService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _IsCustomer = HttpContext.Current.User.IsInRole("Customer");
            _IsAdministrator = HttpContext.Current.User.IsInRole("Administrator");
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
            emailNotification = new SendEmail();
            _trnProductStockService = new TrnProductStockService();
            _trnGoodIssueNoteServie = new TrnGoodIssueNoteService();
        }

        public ListResult<VMTrnSaleOrderList> getSaleOrders(int pageSize, int page, string search)
        {
            List<VMTrnSaleOrderList> saleOrderView;
            var result = repo.TrnSaleOrders
                        .Select(so => new VMTrnSaleOrderList
                        {
                            id = so.id,
                            orderNumber = so.orderNumber,
                            orderDate = so.orderDate,
                            customerName = so.MstCustomer != null ? so.MstCustomer.name : string.Empty,
                            courierName = so.MstCourier != null ? so.MstCourier.name : string.Empty,
                            agentName = so.MstAgent != null ? so.MstAgent.name : string.Empty,
                            status = so.status
                        })
                        .Where(so => !string.IsNullOrEmpty(search)
                            ? so.orderNumber.StartsWith(search)
                            || so.customerName.StartsWith(search)
                            || so.courierName.StartsWith(search)
                            || so.agentName.StartsWith(search)
                            || so.status.StartsWith(search) : true)
                            .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            saleOrderView = result;


            return new ListResult<VMTrnSaleOrderList>
            {
                Data = saleOrderView,
                TotalCount = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(search)
                    ? so.orderNumber.StartsWith(search)
                    || so.MstCustomer.name.StartsWith(search)
                    || so.MstCourier.name.StartsWith(search)
                    || so.status.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnSaleOrder getSaleOrderById(Int64 id)
        {
            var result = repo.TrnSaleOrders.Where(so => so.id == id).FirstOrDefault();
            VMTrnSaleOrder saleOrderView = Mapper.Map<TrnSaleOrder, VMTrnSaleOrder>(result);
            saleOrderView.courierName = result.courierId != null ? result.MstCourier.name : null;
            saleOrderView.customerName = result.MstCustomer.name;

            saleOrderView.TrnSaleOrderItems.ForEach(soItem =>
            {
                soItem.categoryName = soItem.MstCategory.name;
                soItem.collectionName = soItem.collectionId != null ? soItem.MstCollection.collectionCode + " (" + soItem.MstCollection.MstSupplier.code + ")": null;
                soItem.serialno = soItem.MstCategory.code.Equals("Fabric")
                                || soItem.MstCategory.code.Equals("Rug")
                                || soItem.MstCategory.code.Equals("Wallpaper")
                                ? soItem.MstFWRShade.serialNumber + "(" + soItem.MstFWRShade.shadeCode + "-" + soItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                soItem.size = soItem.MstMatSize != null ? soItem.MstMatSize.sizeCode + " (" + soItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + soItem.MstMatSize.MstQuality.qualityCode + ")" :
                            soItem.MstFomSize != null ? soItem.MstFomSize.itemCode : null;
                soItem.accessoryName = soItem.accessoryId != null ? soItem.MstAccessory.itemCode : null;

                decimal stockAvailable = repo.TrnProductStocks.Where(p => p.categoryId == soItem.categoryId
                                                                     && p.collectionId == soItem.collectionId
                                                                     && p.fwrShadeId == soItem.shadeId
                                                                     && p.fomSizeId == soItem.fomSizeId
                                                                     && p.matSizeId == soItem.matSizeId
                                                                     && p.accessoryId == soItem.accessoryId)
                                                                     .Select(s => s.stock + s.poQuantity - s.soQuanity).FirstOrDefault();
                stockAvailable = stockAvailable > 0 ? stockAvailable : 0;
                soItem.availableStock = stockAvailable;
            });

            return saleOrderView;
        }

        public ResponseMessage postSaleOrder(VMTrnSaleOrder saleOrder)
        {
            using (var transaction = new TransactionScope())
            {
                TrnSaleOrder saleOrderToPost = Mapper.Map<VMTrnSaleOrder, TrnSaleOrder>(saleOrder);
                var saleOrderItems = saleOrderToPost.TrnSaleOrderItems.ToList();

                foreach (var soItems in saleOrderItems)
                {
                    soItems.status = _IsCustomer ? SaleOrderStatus.Created.ToString() : SaleOrderStatus.Approved.ToString();
                    soItems.balanceQuantity = soItems.orderQuantity;
                    soItems.deliverQuantity = 0;
                    soItems.createdOn = DateTime.Now;
                    soItems.createdBy = _LoggedInuserId;
                    if (!_IsCustomer)
                    {
                        _trnProductStockService.AddsoOrmqIteminStock(soItems,null);
                    }
                }

                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= saleOrder.orderDate.Date && f.endDate >= saleOrder.orderDate.Date).FirstOrDefault();
                string orderNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.soNumber,"SO");
                saleOrderToPost.orderNumber = orderNo;
                saleOrderToPost.financialYear = financialYear.financialYear;
                saleOrderToPost.status = _IsCustomer ? SaleOrderStatus.Created.ToString() : SaleOrderStatus.Approved.ToString();
                saleOrderToPost.createdOn = DateTime.Now;
                saleOrderToPost.createdBy = _LoggedInuserId;

                repo.TrnSaleOrders.Add(saleOrderToPost);
                financialYear.soNumber += 1;
                repo.SaveChanges();

                MstUser loggedInUser = repo.MstUsers.Where(u => u.id == _LoggedInuserId).FirstOrDefault();
                string adminEmail = repo.MstUsers.Where(u => u.userName.Equals("Administrator")).FirstOrDefault().email;
                string customerEmail = repo.MstCustomers.Where(c => c.id == saleOrder.customerId).FirstOrDefault().email;

                if (!_IsCustomer)
                {
                    VMTrnSaleOrder VMSaleOrderToPost = Mapper.Map<TrnSaleOrder, VMTrnSaleOrder>(saleOrderToPost);
                    _trnGoodIssueNoteServie.postGoodIssueNote(VMSaleOrderToPost,null);
                    emailNotification.approvedSONotificationForCustomer(saleOrder, "ApprovedSONotificationForCustomer", customerEmail, adminEmail);
                }
                else
                {
                    emailNotification.notificationForSO(saleOrder, "NotificationForSO", loggedInUser, adminEmail);
                }

                transaction.Complete();
                return new ResponseMessage(saleOrderToPost.id, resourceManager.GetString("SOAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putSaleOrder(VMTrnSaleOrder saleOrder)
        {
            using (var transaction = new TransactionScope())
            {
                var saleOrderToPut = repo.TrnSaleOrders.Where(q => q.id == saleOrder.id).FirstOrDefault();

                saleOrderToPut.orderNumber = saleOrder.orderNumber;
                saleOrderToPut.shippingAddressId = saleOrder.shippingAddressId;
                saleOrderToPut.courierId = saleOrder.courierId;
                saleOrderToPut.courierMode = saleOrder.courierMode;
                saleOrderToPut.referById = saleOrder.referById;
                saleOrderToPut.orderDate = saleOrder.orderDate;
                saleOrderToPut.expectedDeliveryDate = saleOrder.expectedDeliveryDate;
                saleOrderToPut.totalAmount = saleOrder.totalAmount;
                saleOrderToPut.paymentMode = saleOrder.paymentMode;
                saleOrderToPut.chequeNumber = saleOrder.chequeNumber;
                saleOrderToPut.chequeDate = saleOrder.chequeDate;
                saleOrderToPut.bankName = saleOrder.bankName;
                saleOrderToPut.bankBranch = saleOrder.bankBranch;
                saleOrderToPut.remark = saleOrder.remark;
                saleOrderToPut.status = saleOrder.status;
                saleOrderToPut.financialYear = saleOrder.financialYear;

                updateSOItems(saleOrder);

                saleOrderToPut.updatedOn = DateTime.Now;
                saleOrderToPut.updatedBy = _LoggedInuserId;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(saleOrder.id, resourceManager.GetString("SOUpdated"), ResponseType.Success);
            }
        }

        public void updateSOItems(VMTrnSaleOrder saleOrder)
        {
            var saleOrderToPut = repo.TrnSaleOrders.Where(so => so.id == saleOrder.id).FirstOrDefault();

            List<TrnSaleOrderItem> itemsToRemove = new List<TrnSaleOrderItem>();
            foreach (var soItem in saleOrderToPut.TrnSaleOrderItems)
            {
                if (saleOrder.TrnSaleOrderItems.Any(y => y.id == soItem.id))
                {
                    continue;
                }
                else
                {
                    itemsToRemove.Add(soItem);
                }
            }

            repo.TrnSaleOrderItems.RemoveRange(itemsToRemove);
            repo.SaveChanges();

            saleOrder.TrnSaleOrderItems.ForEach(x =>
            {
                if (saleOrderToPut.TrnSaleOrderItems.Any(y => y.id == x.id))
                {
                    var soItemToPut = repo.TrnSaleOrderItems.Where(p => p.id == x.id).FirstOrDefault();

                    soItemToPut.categoryId = x.categoryId;
                    soItemToPut.collectionId = x.collectionId;
                    soItemToPut.shadeId = x.shadeId;
                    soItemToPut.fomSizeId = x.fomSizeId;
                    soItemToPut.matSizeId = x.matSizeId;
                    soItemToPut.sizeCode = x.sizeCode;
                    soItemToPut.accessoryId = x.accessoryId;
                    soItemToPut.orderQuantity = x.orderQuantity;
                    soItemToPut.deliverQuantity = x.deliverQuantity;
                    soItemToPut.balanceQuantity = x.balanceQuantity;
                    soItemToPut.orderType = x.orderType;
                    soItemToPut.rate = x.rate;
                    soItemToPut.discountPercentage = x.discountPercentage;
                    soItemToPut.rateWithGST = x.rateWithGST;
                    soItemToPut.amount = x.amount;
                    soItemToPut.amountWithGST = x.amountWithGST;
                    soItemToPut.gst = x.gst;
                    soItemToPut.status = x.status;
                    soItemToPut.updatedOn = DateTime.Now;
                    soItemToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
                else
                {
                    TrnSaleOrderItem soItem = Mapper.Map<VMTrnSaleOrderItem, TrnSaleOrderItem>(x);
                    soItem.saleOrderId = saleOrder.id;
                    soItem.status = SaleOrderStatus.Created.ToString();
                    soItem.balanceQuantity = soItem.orderQuantity;
                    soItem.deliverQuantity = 0;
                    soItem.createdBy = _LoggedInuserId;
                    soItem.createdOn = DateTime.Now;
                    repo.TrnSaleOrderItems.Add(soItem);
                    repo.SaveChanges();
                }
            });

        }

        public ResponseMessage approveSO(Int64 id)
        {
            using (var transaction = new TransactionScope())
            {
                var saleOrder = repo.TrnSaleOrders.Where(so => so.id == id).FirstOrDefault();
                string adminEmail = repo.MstUsers.Where(u => u.userName.Equals("Administrator")).FirstOrDefault().email;
                saleOrder.status = SaleOrderStatus.Approved.ToString();
                foreach (var soItem in saleOrder.TrnSaleOrderItems)
                {
                    soItem.status = SaleOrderStatus.Approved.ToString();
                    _trnProductStockService.AddsoOrmqIteminStock(soItem,null);
                }
                repo.SaveChanges();
                VMTrnSaleOrder VMSaleOrder = Mapper.Map<TrnSaleOrder, VMTrnSaleOrder>(saleOrder);
                _trnGoodIssueNoteServie.postGoodIssueNote(VMSaleOrder,null);

                string customerEmail = saleOrder.MstCustomer.email;

                emailNotification.approvedSONotificationForCustomer(VMSaleOrder, "ApprovedSONotificationForCustomer", customerEmail, adminEmail);

                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("SOApproved"), ResponseType.Success);
            }
        }

        public ResponseMessage cancelSO(Int64 id)
        {
            String messageToDisplay;
            ResponseType type;
            using (var transaction = new TransactionScope())
            {
                var saleOrder = repo.TrnSaleOrders.Where(so => so.id == id).FirstOrDefault();
                string adminEmail = repo.MstUsers.Where(u => u.userName.Equals("Administrator")).FirstOrDefault().email;

                if (saleOrder.status.Equals("Created"))
                {
                    saleOrder.status = SaleOrderStatus.Cancelled.ToString();
                    foreach (var soItem in saleOrder.TrnSaleOrderItems)
                    {
                        soItem.status = SaleOrderStatus.Cancelled.ToString();
                    }
                    messageToDisplay = "SOCancelled";
                    type = ResponseType.Success;
                    
                    VMTrnSaleOrder VMsaleOrder = Mapper.Map<TrnSaleOrder, VMTrnSaleOrder>(saleOrder);
                    emailNotification.cancelledSONotificationForCustomer(VMsaleOrder, "CancelledSONotificationForCustomer", adminEmail);
                }
                else if (saleOrder.status.Equals("Approved") && _IsAdministrator)
                {
                    int itemCountWithOrderQtyNotEqualBalQty = saleOrder.TrnSaleOrderItems.Where(soItem => soItem.orderQuantity != soItem.balanceQuantity).Count();
                    if (itemCountWithOrderQtyNotEqualBalQty == 0)
                    {
                        saleOrder.status = SaleOrderStatus.Cancelled.ToString();
                        foreach (var soItem in saleOrder.TrnSaleOrderItems)
                        {
                            soItem.status = SaleOrderStatus.Cancelled.ToString();
                            _trnProductStockService.SubSOItemFromStock(soItem,null);
                        }
                        var ginToUpdate = repo.TrnGoodIssueNotes.Where(gin => gin.salesOrderId == saleOrder.id && gin.status.Equals("Created"))
                                    .FirstOrDefault();
                        ginToUpdate.status = GINStatus.Cancelled.ToString();

                        foreach (var ginItem in ginToUpdate.TrnGoodIssueNoteItems)
                        {
                            ginItem.status = GINStatus.Cancelled.ToString();
                            ginItem.statusChangeDate = DateTime.Now;
                            ginItem.updatedOn = DateTime.Now;
                            ginItem.updatedBy = _LoggedInuserId;
                        }

                        messageToDisplay = "SOCancelled";
                        type = ResponseType.Success;

                        VMTrnSaleOrder VMsaleOrder = Mapper.Map<TrnSaleOrder, VMTrnSaleOrder>(saleOrder);
                        emailNotification.cancelledSONotificationForCustomer(VMsaleOrder, "CancelledSONotificationForCustomer", adminEmail);
                    }
                    else
                    {
                        messageToDisplay = "GINExists";
                        type = ResponseType.Error;
                    }
                }
                else
                {
                    messageToDisplay = "SOApprovedByAdmin";
                    type = ResponseType.Error;
                }

                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString(messageToDisplay), type);
            }
        }

        public ResponseMessage completeSO(Int64 id)
        {
            using (var transaction = new TransactionScope())
            {
                var saleOrder = repo.TrnSaleOrders.Where(so => so.id == id).FirstOrDefault();

                saleOrder.status = SaleOrderStatus.Completed.ToString();
                foreach (var soItem in saleOrder.TrnSaleOrderItems)
                {
                    soItem.status = SaleOrderStatus.Completed.ToString();
                    _trnProductStockService.SubSOItemFromStock(soItem,null);
                }
                repo.SaveChanges();
                var ginToUpdate = repo.TrnGoodIssueNotes.Where(gin => gin.salesOrderId == id && gin.status.Equals("Created"))
                                    .FirstOrDefault();
                ginToUpdate.status = GINStatus.Cancelled.ToString();

                foreach (var ginItem in ginToUpdate.TrnGoodIssueNoteItems)
                {
                    ginItem.status = GINStatus.Cancelled.ToString();
                    ginItem.statusChangeDate = DateTime.Now;
                    ginItem.updatedOn = DateTime.Now;
                    ginItem.updatedBy = _LoggedInuserId;
                }
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("SOForcefullyComplete"), ResponseType.Success);
            }
        }

        public ListResult<VMTrnSaleOrderList> getSalesOrdersForLoggedInUser(int pageSize, int page, string search)
        {
            List<VMTrnSaleOrderList> saleOrderView = null;
            
            if (_IsCustomer)
            {
                Int64 customerId = repo.MstCustomers.Where(c => c.userId == _LoggedInuserId).FirstOrDefault().id;
                var result = repo.TrnSaleOrders.Where(saleOrder => saleOrder.customerId == customerId &&
                                (!string.IsNullOrEmpty(search)
                                ? saleOrder.orderNumber.StartsWith(search)
                                || saleOrder.MstCustomer.name.StartsWith(search)
                                || saleOrder.MstCourier.name.StartsWith(search)
                                || saleOrder.MstAgent.name.StartsWith(search)
                                || saleOrder.status.StartsWith(search) : true))
                                .Select(so => new VMTrnSaleOrderList
                                {
                                    id = so.id,
                                    orderNumber = so.orderNumber,
                                    orderDate = so.orderDate,
                                    customerName = so.MstCustomer != null ? so.MstCustomer.name : string.Empty,
                                    courierName = so.MstCourier != null ? so.MstCourier.name : string.Empty,
                                    agentName = so.MstAgent != null ? so.MstAgent.name : string.Empty,
                                    status = so.status
                                })
                                .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                saleOrderView = result;
            }
            
            return new ListResult<VMTrnSaleOrderList>
            {
                Data = saleOrderView,
                TotalCount = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(search)
                    ? so.orderNumber.StartsWith(search)
                    || so.MstCustomer.name.StartsWith(search)
                    || so.MstCourier.name.StartsWith(search)
                    || so.status.StartsWith(search) : true).Count(),
                Page = page
            };
        }
    }
}