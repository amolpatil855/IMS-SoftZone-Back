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
        TrnProductStockService _trnProductStockService = null;
        GenerateOrderNumber generateOrderNumber = new GenerateOrderNumber();

        public TrnPurchaseOrderService()
        {
            _trnProductStockService = new TrnProductStockService();
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

        public List<VMTrnPurchaseOrderItem> getPOItemsBySOId(Int64 saleOrderId,Int64 supplierId)
        {
            List<VMTrnPurchaseOrderItem> poItems = new List<VMTrnPurchaseOrderItem>();
            var soItems = repo.TrnSaleOrderItems.Where(i => i.saleOrderId == saleOrderId && i.MstCollection.supplierId == supplierId).ToList();
            soItems.ForEach(x=> {
                string categoryCode = repo.MstCategories.Where(c => c.id == x.categoryId).Select(a => a.code).FirstOrDefault();
                long parameterId = Convert.ToInt64(categoryCode.Equals("Fabric") ? x.shadeId : categoryCode.Equals("Mattress") ? x.matSizeId : x.fomSizeId);
                decimal? stockQty = _trnProductStockService.getProductStockAvailablity(x.categoryId, x.collectionId,parameterId);
                if (x.orderQuantity > stockQty)
	            {
                    VMTrnPurchaseOrderItem poItem = new VMTrnPurchaseOrderItem();
                    poItem.categoryId = x.categoryId;
                    poItem.collectionId = x.collectionId;
                    poItem.shadeId = x.shadeId != null ? x.shadeId : null;
                    poItem.fomSizeId = x.fomSizeId!= null ? x.fomSizeId: null;
                    poItem.matSizeId = x.matSizeId != null ? x.matSizeId : null;
                    poItem.sizeCode = x.sizeCode;
                    poItem.orderQuantity = x.orderQuantity;
                    poItem.balanceQuantity = Convert.ToInt64(x.balanceQuantity);
                    poItem.orderType = x.orderType;
                    poItem.rate = x.rate;
                    poItem.amount = x.amount;
                    poItem.status = x.status;
                    poItem.MstCategory = Mapper.Map<MstCategory, VMCategory>(x.MstCategory);
                    poItem.MstCollection = Mapper.Map<MstCollection, VMCollection>(x.MstCollection);
                    poItem.MstFWRShade = Mapper.Map<MstFWRShade, VMFWRShade>(x.MstFWRShade);
                    poItem.MstFomSize = Mapper.Map<MstFomSize, VMFomSize>(x.MstFomSize);
                    poItem.MstMatSize = Mapper.Map<MstMatSize, VMMatSize>(x.MstMatSize);
                    poItems.Add(poItem);
	            }   
            });
            return poItems;
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
                
                var financialYear = repo.MstFinancialYears.Where(f=>f.startDate <= purchaseOrder.orderDate && f.endDate >= purchaseOrder.orderDate).FirstOrDefault();
                string orderNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.poNumber);
                purchaseOrderToPost.orderNumber = 1;
                purchaseOrder.financialYear = financialYear.financialYear;
                purchaseOrder.status = PurchaseOrderStatus.Generated.ToString();
                purchaseOrderToPost.createdOn = DateTime.Now;
                purchaseOrderToPost.createdBy = _LoggedInuserId;

                repo.TrnPurchaseOrders.Add(purchaseOrderToPost);

                financialYear.poNumber += 1; 
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(purchaseOrderToPost.id, resourceManager.GetString("POAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putPurchaseOrder(VMTrnPurchaseOrder purchaseOrder)
        {
            using (var transaction = new TransactionScope())
            {
                var purchaseOrderToPut = repo.TrnPurchaseOrders.Where(q => q.id == purchaseOrder.id).FirstOrDefault();

                purchaseOrderToPut.courierId = purchaseOrder.courierId;
                purchaseOrderToPut.courierMode = purchaseOrder.courierMode;
                purchaseOrderToPut.saleOrderId = purchaseOrder.saleOrderId;
                purchaseOrderToPut.saleOrderNumber = purchaseOrder.saleOrderNumber;
                purchaseOrderToPut.supplierId = purchaseOrder.supplierId;
                purchaseOrderToPut.locationId = purchaseOrder.locationId;
                purchaseOrderToPut.orderNumber = purchaseOrder.orderNumber;
                purchaseOrderToPut.orderDate = purchaseOrder.orderDate;
                purchaseOrderToPut.expectedDeliveryDate = purchaseOrder.expectedDeliveryDate;
                purchaseOrderToPut.remark = purchaseOrder.remark;
                purchaseOrderToPut.status = purchaseOrder.status;
                purchaseOrderToPut.financialYear = purchaseOrder.financialYear;

                updatePOItems(purchaseOrder);

                purchaseOrderToPut.updatedOn = DateTime.Now;
                purchaseOrderToPut.updatedBy = _LoggedInuserId;
                repo.SaveChanges();
                
                transaction.Complete();
                return new ResponseMessage(purchaseOrder.id, resourceManager.GetString("POUpdated"), ResponseType.Success);
            }
        }

        public void updatePOItems(VMTrnPurchaseOrder purchaseOrder)
        {
            var purchaseOrderToPut = repo.TrnPurchaseOrders.Where(q => q.id == purchaseOrder.id).FirstOrDefault();
            List<TrnPurchaseOrderItem> itemsToRemove = new List<TrnPurchaseOrderItem>();
            foreach (var poItem in purchaseOrderToPut.TrnPurchaseOrderItems)
	        {
                if (purchaseOrder.TrnPurchaseOrderItems.Any(y => y.id == poItem.id))
                {
                    continue;
                }
                else
                    itemsToRemove.Add(poItem);
	        }
            //idsToRemove.ForEach(x =>
            //{
            //    repo.TrnPurchaseOrderItems.Remove(repo.TrnPurchaseOrderItems.Where(po => po.id == x).FirstOrDefault());
            //});
            repo.TrnPurchaseOrderItems.RemoveRange(itemsToRemove);
            repo.SaveChanges();

            purchaseOrder.TrnPurchaseOrderItems.ForEach(x =>
            {
                if (purchaseOrderToPut.TrnPurchaseOrderItems.Any(y => y.id == x.id))
                {
                    var poItemToPut = repo.TrnPurchaseOrderItems.Where(p => p.id == x.id).FirstOrDefault();
                    poItemToPut.categoryId = x.categoryId;
                    poItemToPut.collectionId = x.collectionId;
                    poItemToPut.shadeId = x.shadeId;
                    poItemToPut.fomSizeId = x.fomSizeId;
                    poItemToPut.matSizeId = x.matSizeId;
                    poItemToPut.sizeCode = x.sizeCode;
                    poItemToPut.orderQuantity = x.orderQuantity;
                    poItemToPut.balanceQuantity = x.balanceQuantity;
                    poItemToPut.orderType = x.orderType;
                    poItemToPut.rate = x.rate;
                    poItemToPut.amount = x.amount;
                    poItemToPut.status = x.status;
                    poItemToPut.updatedOn = DateTime.Now;
                    poItemToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();

                }
                else
                {
                    TrnPurchaseOrderItem poItem = Mapper.Map<VMTrnPurchaseOrderItem, TrnPurchaseOrderItem>(x);
                    poItem.purchaseOrderId = purchaseOrder.id;
                    poItem.createdBy = _LoggedInuserId;
                    poItem.createdOn = DateTime.Now;
                    repo.TrnPurchaseOrderItems.Add(poItem);
                    repo.SaveChanges();
                }
            });

        }


    }
}