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
        bool _IsAdministrator;
        ResourceManager resourceManager = null;
        TrnProductStockService _trnProductStockService = null;
        GenerateOrderNumber generateOrderNumber = null;
        SendEmail emailNotification = new SendEmail();

        public TrnPurchaseOrderService()
        {
            _trnProductStockService = new TrnProductStockService();
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _IsAdministrator = HttpContext.Current.User.IsInRole("Administrator");
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
        }

        public List<VMLookUpItem> getCollectionBySuppliernCategoryId(Int64 supplierId,Int64 categoryId)
        {
            return repo.MstCollections.Where(c => c.categoryId == categoryId && c.supplierId == supplierId)
                                        .OrderBy(o => o.collectionCode)
                                        .Select(s => new VMLookUpItem { value = s.id, label = s.collectionCode })
                                        .ToList();
        }

        //public List<VMTrnPurchaseOrderItem> getPOItemsBySOId(Int64 saleOrderId,Int64 supplierId)
        //{
        //    List<VMTrnPurchaseOrderItem> poItems = new List<VMTrnPurchaseOrderItem>();
        //    var soItems = repo.TrnSaleOrderItems.Where(i => i.saleOrderId == saleOrderId && i.MstCollection.supplierId == supplierId).ToList();
        //    soItems.ForEach(x=> {
        //        string categoryCode = repo.MstCategories.Where(c => c.id == x.categoryId).Select(a => a.code).FirstOrDefault();
        //        long parameterId = Convert.ToInt64(categoryCode.Equals("Fabric") ? x.shadeId : categoryCode.Equals("Mattress") ? x.matSizeId : x.fomSizeId);
        //        decimal? stockQty = _trnProductStockService.getProductStockAvailablity(x.categoryId, x.collectionId,parameterId);
        //        if (x.orderQuantity > stockQty)
        //        {
        //            VMTrnPurchaseOrderItem poItem = new VMTrnPurchaseOrderItem();
        //            poItem.categoryId = x.categoryId;
        //            poItem.collectionId = x.collectionId;
        //            poItem.shadeId = x.shadeId != null ? x.shadeId : null;
        //            poItem.fomSizeId = x.fomSizeId!= null ? x.fomSizeId: null;
        //            poItem.matSizeId = x.matSizeId != null ? x.matSizeId : null;
        //            poItem.sizeCode = x.sizeCode;
        //            poItem.orderQuantity = x.orderQuantity;
        //            poItem.balanceQuantity = Convert.ToInt64(x.balanceQuantity);
        //            poItem.orderType = x.orderType;
        //            poItem.rate = x.rate;
        //            poItem.amount = x.amount;
        //            poItem.status = x.status;
        //            poItem.MstCategory = Mapper.Map<MstCategory, VMCategory>(x.MstCategory);
        //            poItem.MstCollection = Mapper.Map<MstCollection, VMCollection>(x.MstCollection);
        //            poItem.MstFWRShade = Mapper.Map<MstFWRShade, VMFWRShade>(x.MstFWRShade);
        //            poItem.MstFomSize = Mapper.Map<MstFomSize, VMFomSize>(x.MstFomSize);
        //            poItem.MstMatSize = Mapper.Map<MstMatSize, VMMatSize>(x.MstMatSize);
        //            poItems.Add(poItem);
        //        }   
        //    });
        //    return poItems;
        //}

        public ListResult<VMTrnPurchaseOrder> getPurchaseOrders(int pageSize, int page, string search)
        {
            List<VMTrnPurchaseOrder> purchaseOrderView;
            if (pageSize > 0)
            {
                var result = repo.TrnPurchaseOrders.Where(po => !string.IsNullOrEmpty(search)
                    ? po.orderNumber.ToString().StartsWith(search)
                    || po.MstSupplier.name.ToString().StartsWith(search)
                    || po.MstCourier.name.StartsWith(search)
                    || po.courierMode.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                purchaseOrderView = Mapper.Map<List<TrnPurchaseOrder>, List<VMTrnPurchaseOrder>>(result);
            }
            else
            {
                var result = repo.TrnPurchaseOrders.Where(po => !string.IsNullOrEmpty(search)
                    ? po.orderNumber.ToString().StartsWith(search)
                    || po.MstSupplier.name.ToString().StartsWith(search)
                    || po.MstCourier.name.StartsWith(search)
                    || po.courierMode.StartsWith(search) : true).ToList();
                purchaseOrderView = Mapper.Map<List<TrnPurchaseOrder>, List<VMTrnPurchaseOrder>>(result);
            }

            return new ListResult<VMTrnPurchaseOrder>
            {
                Data = purchaseOrderView,
                TotalCount = repo.TrnPurchaseOrders.Where(po => !string.IsNullOrEmpty(search)
                    ? po.orderNumber.ToString().StartsWith(search)
                    || po.MstSupplier.name.ToString().StartsWith(search)
                    || po.MstCourier.name.StartsWith(search)
                    || po.courierMode.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnPurchaseOrder getPurchaseOrderById(Int64 id)
        {
            var result = repo.TrnPurchaseOrders.Where(po => po.id == id).FirstOrDefault();
            VMTrnPurchaseOrder purchaseOrderView = Mapper.Map<TrnPurchaseOrder, VMTrnPurchaseOrder>(result);
            purchaseOrderView.courierName = result.MstCourier.name;
            purchaseOrderView.supplierName = result.MstSupplier.name;
            purchaseOrderView.shippingAddress = result.MstCompanyLocation.addressLine1 + " " + result.MstCompanyLocation.addressLine2 +
                                                   "," + result.MstCompanyLocation.city + ", " + result.MstCompanyLocation.state + " PINCODE - "
                                                   + result.MstCompanyLocation.pin;
           
            purchaseOrderView.TrnPurchaseOrderItems.ForEach(poItem =>
            {
                poItem.categoryName = poItem.MstCategory.name;
                poItem.collectionName = poItem.collectionId != null?  poItem.MstCollection.collectionName : null;
                poItem.serialno = poItem.MstCategory.code.Equals("Fabric") || poItem.MstCategory.code.Equals("Rug") || poItem.MstCategory.code.Equals("Wallpaper") ? poItem.MstFWRShade.serialNumber + "(" + poItem.MstFWRShade.shadeCode +"-" + poItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                poItem.size = poItem.MstMatSize != null ? poItem.MstMatSize.sizeCode + " (" + poItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + poItem.MstMatSize.MstQuality.qualityCode + ")" : 
                                poItem.MstFomSize != null ? poItem.MstFomSize.itemCode : poItem.matSizeCode;
                poItem.accessoryName = poItem.accessoryId != null ? poItem.MstAccessory.itemCode : null;
            });
           
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
                    poItems.matSizeId = poItems.matSizeId == -1 ? null : poItems.matSizeId;     //set null for custom matSize
                    poItems.status = _IsAdministrator ? PurchaseOrderStatus.Approved.ToString() : PurchaseOrderStatus.Created.ToString();
                    poItems.balanceQuantity = poItems.orderQuantity;
                    poItems.createdOn = DateTime.Now;
                    poItems.createdBy = _LoggedInuserId;
                    if (!(poItems.categoryId == 4 && poItems.matSizeId == null) && _IsAdministrator)
                    {
                        _trnProductStockService.AddpoIteminStock(poItems);
                    }
                }
                
                var financialYear = repo.MstFinancialYears.Where(f=>f.startDate <= purchaseOrder.orderDate && f.endDate >= purchaseOrder.orderDate).FirstOrDefault();
                string orderNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.poNumber);
                purchaseOrderToPost.orderNumber = orderNo;
                purchaseOrderToPost.financialYear = financialYear.financialYear;
                purchaseOrderToPost.status = _IsAdministrator ? PurchaseOrderStatus.Approved.ToString() : PurchaseOrderStatus.Created.ToString();
                purchaseOrderToPost.createdOn = DateTime.Now;
                purchaseOrderToPost.createdBy = _LoggedInuserId;
                
                repo.TrnPurchaseOrders.Add(purchaseOrderToPost);

                financialYear.poNumber += 1; 
                repo.SaveChanges();
                MstUser loggedInUser = repo.MstUsers.Where(u=>u.id == _LoggedInuserId).FirstOrDefault();
                string adminEmail = repo.MstUsers.Where(u=>u.userName.Equals("Administrator")).FirstOrDefault().email;
                string supplierEmail = repo.MstSuppliers.Where(s=>s.id == purchaseOrder.supplierId).FirstOrDefault().email;

                if (_IsAdministrator)
                {
                    emailNotification.notifySupplierForPO(purchaseOrder, "NotifySupplierForPO", supplierEmail);
                }
                else
                {
                    emailNotification.notificationForPO(purchaseOrder, "NotificationForPO", loggedInUser, adminEmail);
                }

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
                purchaseOrderToPut.totalAmount = purchaseOrder.totalAmount;
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
                {
                    itemsToRemove.Add(poItem);
                }
	        }
            
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
                    poItemToPut.accessoryId = x.accessoryId;
                    poItemToPut.matSizeId = x.matSizeId;
                    poItemToPut.matQualityId = x.matQualityId;
                    poItemToPut.matThicknessId = x.matThicknessId;
                    poItemToPut.matSizeCode = x.matSizeCode;
                    poItemToPut.orderQuantity = x.orderQuantity;
                    poItemToPut.balanceQuantity = x.balanceQuantity;
                    poItemToPut.orderType = x.orderType;
                    poItemToPut.rate = x.rate;
                    poItemToPut.rateWithGST = x.rateWithGST;
                    poItemToPut.amount = x.amount;
                    poItemToPut.amountWithGST = x.amountWithGST;
                    poItemToPut.gst = x.gst;
                    poItemToPut.status = x.status;
                    poItemToPut.updatedOn = DateTime.Now;
                    poItemToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
                else
                {
                    TrnPurchaseOrderItem poItem = Mapper.Map<VMTrnPurchaseOrderItem, TrnPurchaseOrderItem>(x);
                    poItem.purchaseOrderId = purchaseOrder.id;
                    poItem.status = PurchaseOrderStatus.Created.ToString();
                    poItem.balanceQuantity = poItem.orderQuantity;
                    poItem.createdBy = _LoggedInuserId;
                    poItem.createdOn = DateTime.Now;
                    repo.TrnPurchaseOrderItems.Add(poItem);
                    repo.SaveChanges();
                }
            });

        }

        public ResponseMessage approvePO(Int64 id)
        {
            using (var transaction = new TransactionScope())
            {
                var purchaseOrder = repo.TrnPurchaseOrders.Where(po => po.id == id).FirstOrDefault();
                purchaseOrder.status = PurchaseOrderStatus.Approved.ToString();
                foreach (var poItem in purchaseOrder.TrnPurchaseOrderItems)
                {
                    poItem.status = PurchaseOrderStatus.Approved.ToString();
                    if (!(poItem.categoryId == 4 && poItem.matSizeId==null))
                    {
                        _trnProductStockService.AddpoIteminStock(poItem);
                    }
                }
                purchaseOrder.updatedOn = DateTime.Now;
                purchaseOrder.updatedBy = _LoggedInuserId;
                repo.SaveChanges();
                VMTrnPurchaseOrder VMPurchaseOrder = Mapper.Map<TrnPurchaseOrder, VMTrnPurchaseOrder>(purchaseOrder);
                MstUser loggedInUser = repo.MstUsers.Where(u => u.id == _LoggedInuserId).FirstOrDefault();
                string supplierEmail = VMPurchaseOrder.MstSupplier.email;

                emailNotification.notifySupplierForPO(VMPurchaseOrder, "NotifySupplierForPO", supplierEmail);

                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("POApproved"), ResponseType.Success);
            }
        }

        public ResponseMessage cancelPO(Int64 id)
        {
            String messageToDisplay;
            ResponseType type;
            using (var transaction = new TransactionScope())
            {
                var purchaseOrder = repo.TrnPurchaseOrders.Where(so => so.id == id).FirstOrDefault();
                if (purchaseOrder.status.Equals("Created"))
                {
                    purchaseOrder.status = PurchaseOrderStatus.Cancelled.ToString();

                    foreach (var poItem in purchaseOrder.TrnPurchaseOrderItems)
                    {
                        poItem.status = PurchaseOrderStatus.Cancelled.ToString();
                    }
                    purchaseOrder.updatedOn = DateTime.Now;
                    purchaseOrder.updatedBy = _LoggedInuserId;
                    messageToDisplay = "POCancelled";
                    type = ResponseType.Success;
                }
                else
                {
                    messageToDisplay = "PONotCancelled";
                    type = ResponseType.Error;
                }
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString(messageToDisplay), type);
            }
        }

        public List<VMLookUpItem> getSupplierListForPO()
        {
            #region Old Code
            //List<Int64?> collectionIds = new List<Int64?>();
            //List<VMLookUpItem> supplierForAccesory = new List<VMLookUpItem>();
            //var soItems = repo.TrnSaleOrderItems.Where(s => s.status.Equals("Approved") && !s.status.Equals("PartialCompleted")).ToList();
            //soItems.ForEach(so =>
            //{
            //    decimal stockAvailable = repo.TrnProductStocks.Where(t => t.categoryId == so.categoryId
            //                                                           && t.collectionId == so.collectionId
            //                                                           && t.fwrShadeId == so.shadeId
            //                                                           && t.fomSizeId == so.fomSizeId
            //                                                           && t.matSizeId == so.matSizeId
            //                                                           && t.accessoryId == so.accessoryId)
            //                                                   .Select(s => s.stock + s.poQuantity - s.soQuanity).FirstOrDefault();
            //    if (so.orderQuantity > stockAvailable && so.collectionId != null)
            //    {
            //        collectionIds.Add(so.collectionId);
            //    }
            //    else if (so.orderQuantity > stockAvailable && so.accessoryId != null)
            //    {
            //        supplierForAccesory.Add(new VMLookUpItem { label = so.MstAccessory.MstSupplier.name, value = so.MstAccessory.MstSupplier.id });
            //    }

            //});
            //var result = repo.MstCollections.Where(c => collectionIds.Contains(c.id))
            //        .Select(s => new VMLookUpItem
            //        {
            //            value = s.MstSupplier.id,
            //            label = s.MstSupplier.name
            //        }).ToList();
            //if (supplierForAccesory.Count > 0)
            //{
            //    result.AddRange(supplierForAccesory);
            //}
            //result = result.Distinct(new VMLookUpItem()).ToList();
            //return result; 
            #endregion

            return repo.TrnProductStocks.Where(stk => (stk.stock + stk.poQuantity) < stk.soQuanity)
                .Select(s=> new VMLookUpItem
                {
                    label = s.collectionId!= null ? s.MstCollection.MstSupplier.name : s.MstAccessory.MstSupplier.code,
                    value = s.collectionId != null ? s.MstCollection.MstSupplier.id : s.MstAccessory.supplierId
                }).Distinct().ToList();
        }

        //public List<VMPOagainstSO> getSOitemsWithStockInsufficient()
        //{
        //    List<VMPOagainstSO> poItemListAgainstSO = new List<VMPOagainstSO>();
        //    var soItems = repo.TrnSaleOrderItems.Where(s => s.status.Equals("Approved") && !s.status.Equals("PartialCompleted")).ToList();
        //    soItems.ForEach(so =>
        //    {
        //        VMPOagainstSO poItemAgainstSO = new VMPOagainstSO();
        //        decimal stockAvailable = repo.TrnProductStocks.Where(t => t.categoryId == so.categoryId
        //                                                               && t.collectionId == so.collectionId
        //                                                               && t.fwrShadeId == so.shadeId
        //                                                               && t.fomSizeId == so.fomSizeId
        //                                                               && t.matSizeId == so.matSizeId
        //                                                               && t.accessoryId == so.accessoryId)
        //                                                       .Select(s => s.stock + s.poQuantity).FirstOrDefault();
        //        stockAvailable = stockAvailable < 0 ? 0 : stockAvailable;
        //        if (so.balanceQuantity > stockAvailable && so.collectionId != null)       //For Fabrics with Flat rate and Foam
        //        {
        //            var result = poItemListAgainstSO.Where(poItem => so.shadeId != null ? poItem.shadeId == so.shadeId : poItem.fomSizeId == so.fomSizeId).FirstOrDefault();
        //            if (result != null)   //if shade/fom item already exist , add required qty only
        //            {
        //                result.requiredQuantity = result.requiredQuantity + Convert.ToDecimal(so.balanceQuantity + stockAvailable);
        //            }
        //            else
        //            {
        //                poItemAgainstSO.supplierId = so.MstCollection.supplierId;
        //                poItemAgainstSO.categoryId = so.categoryId;
        //                poItemAgainstSO.categoryName = so.MstCategory.name;
        //                poItemAgainstSO.collectionId = so.collectionId;
        //                poItemAgainstSO.collectionName = so.MstCollection.collectionName;
        //                poItemAgainstSO.shadeId = so.shadeId;
        //                poItemAgainstSO.serialno = so.shadeId != null ? so.MstFWRShade.serialNumber + "(" + so.MstFWRShade.shadeCode + ")" : null;
        //                poItemAgainstSO.fomSizeId = so.fomSizeId;
        //                poItemAgainstSO.sizeForListing = so.fomSizeId != null ? so.MstFomSize.itemCode : null;
        //                //values for calculation of rate for fabrics
        //                poItemAgainstSO.cutRate = so.shadeId != null ? so.MstFWRShade.MstQuality.cutRate : null;
        //                poItemAgainstSO.roleRate = so.shadeId != null ? so.MstFWRShade.MstQuality.roleRate : null;
        //                poItemAgainstSO.rrp = so.shadeId != null ? so.MstFWRShade.MstQuality.rrp : null;
        //                poItemAgainstSO.maxCutRateDisc = so.shadeId != null ? so.MstFWRShade.MstQuality.maxCutRateDisc : null;
        //                poItemAgainstSO.maxRoleRateDisc = so.shadeId != null ? so.MstFWRShade.MstQuality.maxRoleRateDisc : null;
        //                poItemAgainstSO.flatRate = so.shadeId != null ? so.MstFWRShade.MstQuality.flatRate : null;
        //                poItemAgainstSO.purchaseFlatRate = so.shadeId != null ? so.MstFWRShade.MstQuality.purchaseFlatRate : null;
        //                poItemAgainstSO.maxFlatRateDisc = so.shadeId != null ? so.MstFWRShade.MstQuality.maxFlatRateDisc : null;
        //                poItemAgainstSO.purchaseDiscount = so.shadeId != null ? so.MstFWRShade.MstCollection.purchaseDiscount : null;

        //                //values for calculation of rate for fom
        //                poItemAgainstSO.sellingRatePerMM = so.fomSizeId != null ? so.MstFomSize.MstFomDensity.sellingRatePerMM : (decimal?)null;
        //                poItemAgainstSO.sellingRatePerKG = so.fomSizeId != null ? so.MstFomSize.MstFomDensity.sellingRatePerKG : (decimal?)null;
        //                poItemAgainstSO.suggestedMM = so.fomSizeId != null ? so.MstFomSize.MstFomSuggestedMM.suggestedMM : (decimal?)null;
        //                poItemAgainstSO.purchaseRatePerMM = so.fomSizeId != null ? so.MstFomSize.MstFomDensity.purchaseRatePerMM : (decimal?)null;
        //                poItemAgainstSO.purchaseRatePerKG = so.fomSizeId != null ? so.MstFomSize.MstFomDensity.purchaseRatePerKG : (decimal?)null;
        //                poItemAgainstSO.maxDiscount = so.fomSizeId != null ? so.MstFomSize.MstQuality.maxDiscount : null;
        //                poItemAgainstSO.length = so.fomSizeId != null ? so.MstFomSize.length : (decimal?)null;
        //                poItemAgainstSO.width = so.fomSizeId != null ? so.MstFomSize.width : (decimal?)null;
        //                poItemAgainstSO.purchaseDiscount = so.fomSizeId != null ? so.MstFomSize.MstCollection.purchaseDiscount : null;

        //                poItemListAgainstSO.Add(poItemAgainstSO);
        //            }
        //        }
        //        else if (so.balanceQuantity > stockAvailable && so.accessoryId != null)   //For Accessories
        //        {
        //            var result = poItemListAgainstSO.Where(poItem => poItem.accessoryId == so.accessoryId).FirstOrDefault();

        //            if (result != null) //if accessory item already exist , add required qty only
        //            {
        //                result.requiredQuantity = result.requiredQuantity + Convert.ToDecimal(so.balanceQuantity - stockAvailable);
        //            }
        //            else
        //            {
        //                poItemAgainstSO.supplierId = so.MstAccessory.supplierId;
        //                poItemAgainstSO.categoryId = so.categoryId;
        //                poItemAgainstSO.categoryName = so.MstCategory.name;
        //                poItemAgainstSO.accessoryId = so.accessoryId;
        //                poItemAgainstSO.accessoryName = so.MstAccessory.name;
        //                poItemAgainstSO.requiredQuantity = Convert.ToDecimal(so.balanceQuantity - stockAvailable);
        //                poItemAgainstSO.gst = so.MstAccessory.MstHsn.gst;

        //                poItemAgainstSO.sellingRate = so.MstAccessory.sellingRate;
        //                poItemAgainstSO.purchaseRate = so.MstAccessory.purchaseRate;
        //                poItemListAgainstSO.Add(poItemAgainstSO);
        //            }
        //        }

        //    });
        //    return poItemListAgainstSO;
        //}

        public List<VMPOagainstSO> getItemsWithStockInsufficient()
        {
            List<VMPOagainstSO> poItemListAgainstSO = new List<VMPOagainstSO>();
            var itemsToBeOrdered = repo.TrnProductStocks.Where(stk => (stk.stock + stk.poQuantity) < stk.soQuanity).ToList();

            itemsToBeOrdered.ForEach(item => {
                VMPOagainstSO poItemAgainstSO = new VMPOagainstSO();

                poItemAgainstSO.supplierId = item.collectionId != null ? item.MstCollection.supplierId : item.MstAccessory.supplierId;
                poItemAgainstSO.categoryId = item.categoryId;
                poItemAgainstSO.categoryName = item.MstCategory.name;
                poItemAgainstSO.collectionId = item.collectionId != null ? item.collectionId : null;
                poItemAgainstSO.collectionName = item.collectionId != null ? item.MstCollection.collectionName : null;
                poItemAgainstSO.shadeId = item.fwrShadeId != null ? item.fwrShadeId : null;
                poItemAgainstSO.serialno = item.fwrShadeId != null ? item.MstFWRShade.serialNumber + "(" + item.MstFWRShade.shadeCode + ")" : null;
                poItemAgainstSO.fomSizeId = item.fomSizeId != null ? item.fomSizeId : null;
                poItemAgainstSO.sizeForListing = item.fomSizeId != null ? item.MstFomSize.itemCode : null;
                //values for calculation of rate for fabrics
                poItemAgainstSO.cutRate = item.fwrShadeId != null ? item.MstFWRShade.MstQuality.cutRate : null;
                poItemAgainstSO.roleRate = item.fwrShadeId != null ? item.MstFWRShade.MstQuality.roleRate : null;
                poItemAgainstSO.rrp = item.fwrShadeId != null ? item.MstFWRShade.MstQuality.rrp : null;
                poItemAgainstSO.maxCutRateDisc = item.fwrShadeId != null ? item.MstFWRShade.MstQuality.maxCutRateDisc : null;
                poItemAgainstSO.maxRoleRateDisc = item.fwrShadeId != null ? item.MstFWRShade.MstQuality.maxRoleRateDisc : null;
                poItemAgainstSO.flatRate = item.fwrShadeId != null ? item.MstFWRShade.MstQuality.flatRate : null;
                poItemAgainstSO.purchaseFlatRate = item.fwrShadeId != null ? item.MstFWRShade.MstQuality.purchaseFlatRate : null;
                poItemAgainstSO.maxFlatRateDisc = item.fwrShadeId != null ? item.MstFWRShade.MstQuality.maxFlatRateDisc : null;
                poItemAgainstSO.purchaseDiscount = item.fwrShadeId != null ? item.MstFWRShade.MstCollection.purchaseDiscount : 
                    item.fomSizeId != null ? item.MstFomSize.MstCollection.purchaseDiscount : null;

                //values for calculation of rate for fom
                poItemAgainstSO.sellingRatePerMM = item.fomSizeId != null ? item.MstFomSize.MstFomDensity.sellingRatePerMM : (decimal?)null;
                poItemAgainstSO.sellingRatePerKG = item.fomSizeId != null ? item.MstFomSize.MstFomDensity.sellingRatePerKG : (decimal?)null;
                poItemAgainstSO.suggestedMM = item.fomSizeId != null ? item.MstFomSize.MstFomSuggestedMM.suggestedMM : (decimal?)null;
                poItemAgainstSO.purchaseRatePerMM = item.fomSizeId != null ? item.MstFomSize.MstFomDensity.purchaseRatePerMM : (decimal?)null;
                poItemAgainstSO.purchaseRatePerKG = item.fomSizeId != null ? item.MstFomSize.MstFomDensity.purchaseRatePerKG : (decimal?)null;
                poItemAgainstSO.maxDiscount = item.fomSizeId != null ? item.MstFomSize.MstQuality.maxDiscount : null;
                poItemAgainstSO.length = item.fomSizeId != null ? item.MstFomSize.length : (decimal?)null;
                poItemAgainstSO.width = item.fomSizeId != null ? item.MstFomSize.width : (decimal?)null;
                
                //values for accessory
                poItemAgainstSO.accessoryId = item.accessoryId != null ? item.accessoryId : null;
                poItemAgainstSO.accessoryName = item.accessoryId != null ? item.MstAccessory.name : null;
                poItemAgainstSO.sellingRate = item.accessoryId != null ? item.MstAccessory.sellingRate : (decimal?)null;
                poItemAgainstSO.purchaseRate = item.accessoryId != null ? item.MstAccessory.purchaseRate : (decimal?)null;

                poItemAgainstSO.gst = item.fomSizeId != null ? item.MstFomSize.MstQuality.MstHsn.gst :
                    item.fwrShadeId != null ? item.MstFWRShade.MstQuality.MstHsn.gst : 
                    item.matSizeId != null ? item.MstMatSize.MstQuality.MstHsn.gst : item.MstAccessory.MstHsn.gst;

                poItemAgainstSO.requiredQuantity = -Convert.ToDecimal(item.stock + item.poQuantity - item.soQuanity);
                poItemListAgainstSO.Add(poItemAgainstSO);

            });
            return poItemListAgainstSO;
        }
    }
}