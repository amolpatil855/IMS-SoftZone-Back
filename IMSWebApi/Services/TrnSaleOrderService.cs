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
                    || so.orderDate.ToString().StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                saleOrderView = Mapper.Map<List<TrnSaleOrder>, List<VMTrnSaleOrder>>(result);
            }
            else
            {
                var result = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(search)
                    ? so.orderNumber.StartsWith(search)
                    || so.orderDate.ToString().StartsWith(search) : true).ToList();
                saleOrderView = Mapper.Map<List<TrnSaleOrder>, List<VMTrnSaleOrder>>(result);
            }

            return new ListResult<VMTrnSaleOrder>
            {
                Data = saleOrderView,
                TotalCount = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(search)
                    ? so.orderNumber.StartsWith(search)
                    || so.orderDate.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnSaleOrder getSaleOrderById(Int64 id)
        {
            var result = repo.TrnSaleOrders.Where(so => so.id == id).FirstOrDefault();
            VMTrnSaleOrder saleOrderView = Mapper.Map<TrnSaleOrder, VMTrnSaleOrder>(result);
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
    }
}