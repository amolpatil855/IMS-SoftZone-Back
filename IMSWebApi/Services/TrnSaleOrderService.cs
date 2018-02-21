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

namespace IMSWebApi.Services
{
    public class TrnSaleOrderService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;
        SendEmail emailNotification = null;

        public TrnSaleOrderService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
            emailNotification = new SendEmail();
        }

        public ListResult<VMTrnSaleOrder> getSaleOrders(int pageSize, int page, string search)
        {
            List<VMTrnSaleOrder> saleOrderView;
            if (pageSize > 0)
            {
                var result = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(search)
                    ? so.orderNumber.StartsWith(search)
                    || so.MstCustomer.name.StartsWith(search)
                    || so.MstCourier.name.StartsWith(search) 
                    || so.status.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                saleOrderView = Mapper.Map<List<TrnSaleOrder>, List<VMTrnSaleOrder>>(result);
            }
            else
            {
                var result = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(search)
                    ? so.orderNumber.StartsWith(search)
                    || so.MstCustomer.name.StartsWith(search)
                    || so.MstCourier.name.StartsWith(search)
                    || so.status.StartsWith(search) : true).ToList();
                saleOrderView = Mapper.Map<List<TrnSaleOrder>, List<VMTrnSaleOrder>>(result);
            }

            return new ListResult<VMTrnSaleOrder>
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
            saleOrderView.courierName = result.MstCourier.name;
            saleOrderView.customerName = result.MstCustomer.name;

            foreach (var soItem in saleOrderView.TrnSaleOrderItems)
            {
                soItem.categoryName = soItem.MstCategory.name;
                soItem.collectionName = soItem.MstCollection.collectionName;
                soItem.serialno = soItem.MstCategory.code.Equals("Fabric") || soItem.MstCategory.code.Equals("Rug") || soItem.MstCategory.code.Equals("Wallpaper") ? soItem.MstFWRShade.serialNumber + "(" + soItem.MstFWRShade.shadeCode + ")" : null;
                soItem.size = soItem.MstMatSize != null ? soItem.MstMatSize.sizeCode : soItem.MstFomSize != null ? soItem.MstFomSize.sizeCode : null;
            }
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
                    soItems.status = SaleOrderStatus.Generated.ToString();
                    soItems.balanceQuantity = soItems.orderQuantity;
                    soItems.deliverQuantity = 0;
                    soItems.createdOn = DateTime.Now;
                    soItems.createdBy = _LoggedInuserId;
                }

                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= saleOrder.orderDate && f.endDate >= saleOrder.orderDate).FirstOrDefault();
                string orderNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.soNumber);
                saleOrderToPost.orderNumber = orderNo;
                saleOrderToPost.financialYear = financialYear.financialYear;
                saleOrderToPost.status = PurchaseOrderStatus.Generated.ToString();
                saleOrderToPost.createdOn = DateTime.Now;
                saleOrderToPost.createdBy = _LoggedInuserId;

                repo.TrnSaleOrders.Add(saleOrderToPost);
                financialYear.soNumber += 1;
                repo.SaveChanges();

                MstUser loggedInUser = repo.MstUsers.Where(u => u.id == _LoggedInuserId).FirstOrDefault();
                string adminEmail = repo.MstUsers.Where(u => u.userName.Equals("Administrator")).FirstOrDefault().email;

                emailNotification.notificationForSO(saleOrder, "NotificationForSO", loggedInUser, adminEmail);

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
                saleOrderToPut.courierId = saleOrder.courierId;
                saleOrderToPut.courierMode = saleOrder.courierMode;
                saleOrderToPut.referById = saleOrder.referById;
                saleOrderToPut.orderDate = saleOrder.orderDate;
                saleOrderToPut.expectedDeliveryDate = saleOrder.expectedDeliveryDate;
                saleOrderToPut.totalAmount = saleOrder.totalAmount;
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
                    soItemToPut.sizeCode= x.sizeCode;
                    soItemToPut.accessoryId = x.accessoryId;
                    soItemToPut.orderQuantity = x.orderQuantity;
                    soItemToPut.deliverQuantity = x.deliverQuantity;
                    soItemToPut.balanceQuantity = x.balanceQuantity;
                    soItemToPut.orderType = x.orderType;
                    soItemToPut.rate = x.rate;
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
                    soItem.status = SaleOrderStatus.Generated.ToString();
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
                saleOrder.status = SaleOrderStatus.Approve.ToString();
                foreach (var soItem in saleOrder.TrnSaleOrderItems)
                {
                    soItem.status = SaleOrderStatus.Approve.ToString();
                }
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("SOApproved"), ResponseType.Success);
            }
        }
    }
}