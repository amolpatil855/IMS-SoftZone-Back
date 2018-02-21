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

namespace IMSWebApi.Services
{
    public class TrnProductStockService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
       
        public TrnProductStockService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMTrnProductStock> getTrnProductStock(int pageSize, int page, string search)
         {	        
		 List<VMTrnProductStock> trnProductStockView;
             if (pageSize > 0)	           
             {	            
                 var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)	             
                     ? q.MstCategory.code.StartsWith(search)	                   
                     || q.MstCollection.collectionCode.StartsWith(search)	       
                     || q.MstFWRShade.serialNumber.ToString().StartsWith(search)	
                     || q.MstMatSize.sizeCode.StartsWith(search)
                     || q.MstFomSize.sizeCode.StartsWith(search)
					 || q.stock.ToString().StartsWith(search): true)
                     .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();	            
                 trnProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);	
             }	            
			 else
             {	             
                 var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)	              
                     ? q.MstCategory.code.StartsWith(search)	                 
                     || q.MstCollection.collectionCode.StartsWith(search)	     
                     || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                     || q.MstMatSize.sizeCode.StartsWith(search)	            
                     || q.MstFomSize.sizeCode.StartsWith(search)	            
                     || q.stock.ToString().StartsWith(search) : true).ToList();	
                 trnProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);	                
             }
             foreach (var productStock in trnProductStockView)
             {
                 productStock.accessoryCode = productStock.MstCategory.code.Equals("Accessories") ? productStock.MstAccessory.itemCode : null;
                 productStock.serialno = productStock.MstCategory.code.Equals("Fabric") 
                                        || productStock.MstCategory.code.Equals("Rug") 
                                        || productStock.MstCategory.code.Equals("Wallpaper") 
                                        ? productStock.MstFWRShade.serialNumber + " ("+productStock.MstFWRShade.shadeCode+"-"+
                                        productStock.MstFWRShade.MstFWRDesign.designCode+")" : null;
                 productStock.fomItem = productStock.MstCategory.code.Equals("Foam") ? productStock.MstFomSize.itemCode : null;
                 productStock.matSize = productStock.MstCategory.code.Equals("Mattress") ? 
                                        productStock.MstMatSize.sizeCode + " ("+productStock.MstMatSize.MstMatThickNess.thicknessCode+
                                        "-"+productStock.MstMatSize.MstQuality.qualityCode+")" : null;
             }


             return new ListResult<VMTrnProductStock>	       
             {	             
			 Data = trnProductStockView,
                 TotalCount = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)	             
                     ? q.MstCategory.code.StartsWith(search)	                   
					 || q.MstCollection.collectionCode.StartsWith(search)
                     || q.MstFWRShade.serialNumber.ToString().StartsWith(search)	                  
                     || q.MstMatSize.sizeCode.StartsWith(search)	                    
                     || q.MstFomSize.sizeCode.StartsWith(search)	                    
                     || q.stock.ToString().StartsWith(search) : true).Count(),	        
                 Page = page	              
             };	          
         }	         

        public ListResult<VMTrnProductStock> getFWRProductStock(int pageSize, int page, string search)
        {
            List<VMTrnProductStock> FWRProductStockView;
            if (pageSize > 0)
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.stock.ToString().StartsWith(search): true && q.matSizeId == null && q.fomSizeId == null && q.accessoryId == null)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                FWRProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }
            else
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.matSizeId == null && q.fomSizeId == null && q.accessoryId == null).ToList();
                FWRProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }

            return new ListResult<VMTrnProductStock>
            {
                Data = FWRProductStockView,
                TotalCount = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.matSizeId == null && q.fomSizeId == null && q.accessoryId == null).Count(),
                Page = page
            };
        }

        public ListResult<VMTrnProductStock> getMatProductStock(int pageSize, int page, string search)
        {
            List<VMTrnProductStock> MatProductStockView;
            if (pageSize > 0)
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.fwrShadeId == null && q.fomSizeId == null && q.accessoryId == null)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                MatProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }
            else
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.fwrShadeId == null && q.fomSizeId == null && q.accessoryId == null).ToList();
                MatProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }

            return new ListResult<VMTrnProductStock>
            {
                Data = MatProductStockView,
                TotalCount = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.fwrShadeId == null && q.fomSizeId == null && q.accessoryId == null).Count(),
                Page = page
            };
        }

        public ListResult<VMTrnProductStock> getFomProductStock(int pageSize, int page, string search)
        {
            List<VMTrnProductStock> FomProductStockView;
            if (pageSize > 0)
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.fwrShadeId == null && q.matSizeId == null && q.accessoryId == null)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                FomProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }
            else
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.fwrShadeId == null && q.matSizeId == null && q.accessoryId == null).ToList();
                FomProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }

            return new ListResult<VMTrnProductStock>
            {
                Data = FomProductStockView,
                TotalCount = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstFomSize.sizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.fwrShadeId == null && q.matSizeId == null && q.accessoryId == null).Count(),
                Page = page
            };
        }

        public ListResult<VMTrnProductStock> getAccessoryProductStock(int pageSize, int page, string search)
        {
            List<VMTrnProductStock> AccessoryProductStockView;
            if (pageSize > 0)
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstAccessory.name.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.fwrShadeId == null && q.matSizeId == null && q.fomSizeId == null)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                AccessoryProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }
            else
            {
                var result = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstAccessory.name.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.fwrShadeId == null && q.matSizeId == null && q.fomSizeId == null).ToList();
                AccessoryProductStockView = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            }

            return new ListResult<VMTrnProductStock>
            {
                Data = AccessoryProductStockView,
                TotalCount = repo.TrnProductStocks.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstAccessory.name.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true && q.fwrShadeId == null && q.matSizeId == null && q.fomSizeId == null).Count(),
                Page = page
            };
        }

        public VMProductDetails getProductStockAvailablity(Int64 categoryId, Int64? collectionId, Int64? parameterId,Int64? qualityId)
        {
            TrnProductStock TrnProductStock = null;
            MstFWRShade fwrShade = null;
            MstFomSize fomSize = null;
            MstMatSize matSize = null;
            MstAccessory accessory = null;
            VMProductDetails productDetails = new VMProductDetails();
            string categoryCode = repo.MstCategories.Where(c => c.id == categoryId).Select(a => a.code).FirstOrDefault();

            if (categoryCode != null && (categoryCode.Equals("Fabric") || categoryCode.Equals("Rug") || categoryCode.Equals("Wallpaper")))
            {
                TrnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                      && z.collectionId == collectionId
                                                      && z.fwrShadeId == parameterId).FirstOrDefault();

                fwrShade = repo.MstFWRShades.Where(s => s.id == parameterId).FirstOrDefault();

                productDetails.cutRate = fwrShade.MstQuality.cutRate;
                productDetails.roleRate = fwrShade.MstQuality.roleRate;
                productDetails.rrp = fwrShade.MstQuality.rrp;
                productDetails.maxCutRateDisc = fwrShade.MstQuality.maxCutRateDisc;
                productDetails.maxRoleRateDisc = fwrShade.MstQuality.maxRoleRateDisc;
                productDetails.flatRate = fwrShade.MstQuality.flatRate;
                productDetails.purchaseFlatRate = fwrShade.MstQuality.purchaseFlatRate;
                productDetails.maxFlatRateDisc = fwrShade.MstQuality.maxFlatRateDisc;
                productDetails.stock = TrnProductStock!=null ? TrnProductStock.stock + TrnProductStock.poQuantity - TrnProductStock.soQuanity : 0;
                productDetails.gst = fwrShade.MstQuality.MstHsn.gst;
                productDetails.purchaseDiscount = fwrShade.MstCollection.purchaseDiscount;
            }
            if (categoryCode != null && categoryCode.Equals("Foam"))
            {
                TrnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                      && z.collectionId == collectionId
                                                      && z.fomSizeId == parameterId).FirstOrDefault();

                fomSize = repo.MstFomSizes.Where(f => f.id == parameterId).FirstOrDefault();

                productDetails.sellingRatePerMM = fomSize.MstFomDensity.sellingRatePerMM;
                productDetails.sellingRatePerKG = fomSize.MstFomDensity.sellingRatePerKG;
                productDetails.suggestedMM = fomSize.MstFomSuggestedMM.suggestedMM;
                productDetails.purchaseRatePerMM = fomSize.MstFomDensity.purchaseRatePerMM;
                productDetails.purchaseRatePerKG = fomSize.MstFomDensity.purchaseRatePerKG;
                productDetails.maxDiscount = fomSize.MstQuality.maxDiscount;
                productDetails.length = fomSize.length;
                productDetails.width = fomSize.width;
                productDetails.gst = fomSize.MstQuality.MstHsn.gst;
                productDetails.stock = TrnProductStock!=null ? TrnProductStock.stock + TrnProductStock.poQuantity - TrnProductStock.soQuanity : 0;
                productDetails.purchaseDiscount = fomSize.MstCollection.purchaseDiscount;
            }
            if (categoryCode != null && categoryCode.Equals("Mattress"))
            {
                if (parameterId != null)
                {
                    TrnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                        && z.collectionId == collectionId
                                                        && z.matSizeId == parameterId).FirstOrDefault();

                    matSize = repo.MstMatSizes.Where(m => m.id == parameterId).FirstOrDefault();

                    productDetails.rate = matSize.rate;
                    productDetails.purchaseRate = matSize.purchaseRate;
                    productDetails.gst = matSize.MstQuality.MstHsn.gst;
                    productDetails.stock = TrnProductStock!=null ? TrnProductStock.stock + TrnProductStock.poQuantity - TrnProductStock.soQuanity : 0;
                    productDetails.purchaseDiscount = matSize.MstCollection.purchaseDiscount;
                }
                else if(qualityId!=null)
                {
                    var customSizeMat = repo.MstQualities.Where(z => z.categoryId == categoryId
                                                    && z.collectionId == collectionId
                                                    && z.id == qualityId).FirstOrDefault();

                    productDetails.custRatePerSqFeet = customSizeMat.custRatePerSqFeet;
                    productDetails.maxDiscount = customSizeMat.maxDiscount;
                    productDetails.purchaseDiscount = customSizeMat.MstCollection.purchaseDiscount;
                    productDetails.gst = customSizeMat.MstHsn.gst;
                }
            }
            if (categoryCode != null && categoryCode.Equals("Accessories"))
            {
                TrnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                        && z.accessoryId == parameterId).FirstOrDefault();

                accessory = repo.MstAccessories.Where(a => a.id == parameterId).FirstOrDefault();

                productDetails.sellingRate = accessory.sellingRate;
                productDetails.purchaseRate = accessory.purchaseRate;
                productDetails.gst = accessory.MstHsn.gst;
                productDetails.stock = TrnProductStock!=null ? TrnProductStock.stock + TrnProductStock.poQuantity - TrnProductStock.soQuanity : 0;
            }
            
            return productDetails;
        }

        public void AddProductInStock(VMTrnProductStockDetail trnProductStockDetail,bool isUpdate,decimal stock,decimal? stockInKg )
        {
            TrnProductStock product = repo.TrnProductStocks.Where(z => z.categoryId == trnProductStockDetail.categoryId
                                                      && z.collectionId == trnProductStockDetail.collectionId
                                                      && z.fwrShadeId == trnProductStockDetail.fwrShadeId
                                                      && z.fomSizeId == trnProductStockDetail.fomSizeId
                                                      && z.matSizeId == trnProductStockDetail.matSizeId
                                                      && z.accessoryId == trnProductStockDetail.accessoryId).FirstOrDefault();

            if (product!=null)
            {   
                product.stock = isUpdate ? product.stock + stock : product.stock + trnProductStockDetail.stock;
                product.stockInKg = isUpdate ? product.stockInKg + stock : product.stockInKg + trnProductStockDetail.stockInKg;  
                product.updatedOn = DateTime.Now;
                product.updatedBy = _LoggedInuserId;
                repo.SaveChanges();
            }    
            else
            {
                TrnProductStock productStockToAdd = new TrnProductStock();
                productStockToAdd.categoryId = trnProductStockDetail.categoryId;
                productStockToAdd.collectionId = trnProductStockDetail.collectionId;
                productStockToAdd.fwrShadeId = trnProductStockDetail.fwrShadeId;
                productStockToAdd.fomSizeId = trnProductStockDetail.fomSizeId;
                productStockToAdd.matSizeId = trnProductStockDetail.matSizeId;
                productStockToAdd.accessoryId = trnProductStockDetail.accessoryId;
                productStockToAdd.stock = trnProductStockDetail.stock;
                productStockToAdd.stockInKg = trnProductStockDetail.stockInKg;
                productStockToAdd.poQuantity = productStockToAdd.soQuanity = 0;
                productStockToAdd.createdOn = DateTime.Now;
                productStockToAdd.createdBy = _LoggedInuserId;
                repo.TrnProductStocks.Add(productStockToAdd);
                repo.SaveChanges();
            }
        }

        public void AddpoIteminStock(TrnPurchaseOrderItem purchaseOrderItem)
        {
            TrnProductStock product = repo.TrnProductStocks.Where(z => z.categoryId == purchaseOrderItem.categoryId
                                                      && z.collectionId == purchaseOrderItem.collectionId
                                                      && z.fwrShadeId == purchaseOrderItem.shadeId
                                                      && z.fomSizeId == purchaseOrderItem.fomSizeId
                                                      && z.matSizeId == purchaseOrderItem.matSizeId
                                                      && z.accessoryId == purchaseOrderItem.matSizeId).FirstOrDefault();
                if (product != null)
                {
                    product.poQuantity = product.poQuantity + purchaseOrderItem.orderQuantity;
                    product.updatedOn = DateTime.Now;
                    product.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
                else
                {
                    TrnProductStock productStockToAdd = new TrnProductStock();
                    productStockToAdd.categoryId = purchaseOrderItem.categoryId;
                    productStockToAdd.collectionId = purchaseOrderItem.collectionId;
                    productStockToAdd.fwrShadeId = purchaseOrderItem.shadeId;
                    productStockToAdd.fomSizeId = purchaseOrderItem.fomSizeId;
                    productStockToAdd.matSizeId = purchaseOrderItem.matSizeId;
                    productStockToAdd.poQuantity = purchaseOrderItem.orderQuantity;
                    productStockToAdd.stock = productStockToAdd.soQuanity = 0;
                    productStockToAdd.createdOn = DateTime.Now;
                    productStockToAdd.createdBy = _LoggedInuserId;
                    repo.TrnProductStocks.Add(productStockToAdd);
                    repo.SaveChanges();
                }
        }
    }
}