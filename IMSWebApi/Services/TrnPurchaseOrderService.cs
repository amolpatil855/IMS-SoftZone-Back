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
    public class TrnPurchaseOrderService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        public TrnPurchaseOrderService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public List<VMLookUpItem> getCollectionBySuppliernCategoryId(Int64 supplierId,Int64 categoryId)
        {
            return repo.MstCollections.Where(c => c.categoryId == categoryId && c.supplierId == supplierId)
                                        .OrderBy(o => o.collectionCode)
                                        .Select(s => new VMLookUpItem { value = s.id, label = s.collectionCode })
                                        .ToList();
        }

        public ListResult<VMTrnPurchaseOrder> getPurchaseOrder(int pageSize, int page, string search)
        {
            List<VMTrnPurchaseOrder> purchaseOrderView;
            if (pageSize > 0)
            {
                var result = repo.TrnPurchaseOrders.Where(po => !string.IsNullOrEmpty(search)
                    ? po.orderNumber.ToString().StartsWith(search)
                    || po.orderDate.ToString().StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                purchaseOrderView = Mapper.Map<List<TrnPurchaseOrder>, List<VMTrnPurchaseOrder>>(result);
            }
            else
            {
                var result = repo.TrnPurchaseOrders.Where(po => !string.IsNullOrEmpty(search)
                    ? po.orderNumber.ToString().StartsWith(search)
                    || po.orderDate.ToString().StartsWith(search) : true).ToList();
                purchaseOrderView = Mapper.Map<List<TrnPurchaseOrder>, List<VMTrnPurchaseOrder>>(result);
            }

            return new ListResult<VMTrnPurchaseOrder>
            {
                Data = purchaseOrderView,
                TotalCount = repo.TrnPurchaseOrders.Where(po => !string.IsNullOrEmpty(search)
                    ? po.orderNumber.ToString().StartsWith(search)
                    || po.orderDate.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnPurchaseOrder getPurchaseOrderById(Int64 id)
        {
            var result = repo.TrnPurchaseOrders.Where(po => po.id == id).FirstOrDefault();
            VMTrnPurchaseOrder purchaseOrderView = Mapper.Map<TrnPurchaseOrder, VMTrnPurchaseOrder>(result);
            return purchaseOrderView;
        }

        public ResponseMessage postPurchaseOrder(VMTrnPurchaseOrder purchaseOrder)
        {
            using (var transaction = new TransactionScope())
            {
                TrnPurchaseOrder purchaseOrderToPost = Mapper.Map<VMTrnPurchaseOrder, TrnPurchaseOrder>(purchaseOrder);
                var purchaseOrderItems = purchaseOrderToPost.TrnPurchaseOrderItems.ToList();
                foreach (var poItems in purchaseOrderItems)
                {
                    poItems.createdOn = DateTime.Now;
                    poItems.createdBy = _LoggedInuserId;
                }
                int startingPONumber = repo.MstFinancialYears.ToList().LastOrDefault().soNumber;
                int currentSOCount = repo.TrnPurchaseOrders.Count();
                purchaseOrderToPost.orderNumber = startingPONumber + currentSOCount;
                purchaseOrderToPost.createdOn = DateTime.Now;
                purchaseOrderToPost.createdBy = _LoggedInuserId;

                repo.TrnPurchaseOrders.Add(purchaseOrderToPost);
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(purchaseOrderToPost.id, resourceManager.GetString("SupplierAdded"), ResponseType.Success);
            }
        }
    }
}