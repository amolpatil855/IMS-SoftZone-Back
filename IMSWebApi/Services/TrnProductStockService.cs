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

        public VMProductDetails getProductStockAvailablity(Int64 categoryId, Int64 collectionId, Int64? parameterId,Int64? qualityId)
        {
            TrnProductStock TrnProductStock = null;
            VMProductDetails productDetails = new VMProductDetails();
            string categoryCode = repo.MstCategories.Where(c => c.id == categoryId).Select(a => a.code).FirstOrDefault();
            
            if (categoryCode!=null && categoryCode.Equals("Fabric"))
            {
                TrnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                      && z.collectionId == collectionId
                                                      && z.fwrShadeId == parameterId).FirstOrDefault();

                productDetails.cutRate = TrnProductStock.MstFWRShade.MstQuality.cutRate;
                productDetails.roleRate = TrnProductStock.MstFWRShade.MstQuality.roleRate;
                productDetails.rrp = TrnProductStock.MstFWRShade.MstQuality.rrp;
                productDetails.maxCutRateDisc = TrnProductStock.MstFWRShade.MstQuality.maxCutRateDisc;
                productDetails.maxRoleRateDisc = TrnProductStock.MstFWRShade.MstQuality.maxRoleRateDisc;
                productDetails.flatRate = TrnProductStock.MstFWRShade.MstQuality.flatRate;
                productDetails.purchaseFlatRate = TrnProductStock.MstFWRShade.MstQuality.purchaseFlatRate;
                productDetails.maxFlatRateDisc = TrnProductStock.MstFWRShade.MstQuality.maxFlatRateDisc;
                productDetails.stock = TrnProductStock.stock + TrnProductStock.poQuantity - TrnProductStock.soQuanity;
                productDetails.gst = TrnProductStock.MstFWRShade.MstQuality.MstHsn.gst;
            }
            if (categoryCode != null && categoryCode.Equals("Foam"))
            {
                TrnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                      && z.collectionId == collectionId
                                                      && z.fomSizeId == parameterId).FirstOrDefault();
                
                productDetails.sellingRatePerMM = TrnProductStock.MstFomSize.MstFomDensity.sellingRatePerMM;
                productDetails.sellingRatePerKG = TrnProductStock.MstFomSize.MstFomDensity.sellingRatePerKG;
                productDetails.sellingRatePercentage = TrnProductStock.MstFomSize.MstFomDensity.sellingRatePercentage;
                productDetails.suggestedMM = TrnProductStock.MstFomSize.MstFomSuggestedMM.suggestedMM;
                productDetails.purchaseRatePerMM = TrnProductStock.MstFomSize.MstFomDensity.purchaseRatePerMM;
                productDetails.purchaseRatePerKG = TrnProductStock.MstFomSize.MstFomDensity.purchaseRatePerKG;
                productDetails.maxDiscount = TrnProductStock.MstFomSize.MstQuality.maxDiscount;
                productDetails.length = TrnProductStock.MstFomSize.length;
                productDetails.width = TrnProductStock.MstFomSize.width;
                productDetails.gst = TrnProductStock.MstFomSize.MstQuality.MstHsn.gst;
                productDetails.stock = TrnProductStock.stock + TrnProductStock.poQuantity - TrnProductStock.soQuanity;
            }
            if (categoryCode != null && categoryCode.Equals("Mattress"))
            {
                if (parameterId != null)
                {
                    TrnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                        && z.collectionId == collectionId
                                                        && z.matSizeId == parameterId).FirstOrDefault();
                    productDetails.rate = TrnProductStock.MstMatSize.rate;
                    productDetails.purchaseRate = TrnProductStock.MstMatSize.purchaseRate;
                    productDetails.gst = TrnProductStock.MstMatSize.MstQuality.MstHsn.gst;
                    productDetails.stock = TrnProductStock.stock + TrnProductStock.poQuantity - TrnProductStock.soQuanity;
                }
                else if(qualityId!=null)
                {
                    var customSizeMat = repo.MstQualities.Where(z => z.categoryId == categoryId
                                                    && z.collectionId == collectionId
                                                    && z.id == qualityId).FirstOrDefault();

                    productDetails.custRatePerSqFeet = customSizeMat.custRatePerSqFeet;
                    productDetails.maxDiscount = customSizeMat.maxDiscount;
                    
                    productDetails.gst = customSizeMat.MstHsn.gst;
                }
            }
            if (categoryCode != null && categoryCode.Equals("Accessories"))
            {
                TrnProductStock = repo.TrnProductStocks.Where(z => z.categoryId == categoryId
                                                        && z.collectionId == collectionId
                                                        && z.accessoryId == parameterId).FirstOrDefault();
                productDetails.sellingRate = TrnProductStock.MstAccessory.sellingRate;
                productDetails.purchaseRate = TrnProductStock.MstAccessory.purchaseRate;
                productDetails.gst = TrnProductStock.MstAccessory.MstHsn.gst;
                productDetails.stock = TrnProductStock.stock + TrnProductStock.poQuantity - TrnProductStock.soQuanity;
            }
            //VMProductStock = Mapper.Map<TrnProductStock, VMTrnProductStock>(TrnProductStock);
            //VMProductStock.MstCollection.MstCategory = null;
            //stockAvailabe = ProductStock != null ? ProductStock.stock : 0;
            //List<VMTrnProductStock> stockQty = new List<VMTrnProductStock>(result);
            //List<VMTrnProductStock> productStock = Mapper.Map<List<TrnProductStock>, List<VMTrnProductStock>>(result);
            //decimal currentProductStock = productStock.Aggregate<VMTrnProductStock, decimal>(0, (productQty, p) => productQty += p.stock);
             

            return productDetails;
        }

        public void AddProductInStock(VMTrnProductStockDetail trnProductStockDetail,bool isUpdate,decimal qty)
        {
            TrnProductStock product = repo.TrnProductStocks.Where(z => z.categoryId == trnProductStockDetail.categoryId
                                                      && z.collectionId == trnProductStockDetail.collectionId
                                                      && z.fwrShadeId == trnProductStockDetail.fwrShadeId
                                                      && z.fomSizeId == trnProductStockDetail.fomSizeId
                                                      && z.matSizeId == trnProductStockDetail.matSizeId
                                                      && z.accessoryId == trnProductStockDetail.matSizeId).FirstOrDefault();

            if (product!=null)
            {
                product.stock = isUpdate ? product.stock + qty : product.stock + trnProductStockDetail.stock;
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
                productStockToAdd.poQuantity = productStockToAdd.soQuanity = 0;
                repo.TrnProductStocks.Add(productStockToAdd);
                repo.SaveChanges();
            }
        }
    }
}