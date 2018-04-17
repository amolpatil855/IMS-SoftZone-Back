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
using AutoMapper;
using System.Transactions;
using IMSWebApi.Enums;

namespace IMSWebApi.Services
{
    public class TrnCurtainSelectionService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;

        public TrnCurtainSelectionService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
        }

        public ListResult<VMTrnCurtainSelectionList> getCurtainSelections(int pageSize, int page, string search)
        {
            List<VMTrnCurtainSelectionList> curtainSelectionView;
            curtainSelectionView = repo.TrnCurtainSelections.Where(cs => !string.IsNullOrEmpty(search)
                    ? cs.curtainSelectionNumber.StartsWith(search)
                    || cs.MstCustomer.name.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? cs.isQuotationCreated : search.ToLower().Equals("no") ? !(cs.isQuotationCreated) : false) : true)
                    .Select(cs => new VMTrnCurtainSelectionList
                    {
                        id = cs.id,
                        curtainSelectionNumber = cs.curtainSelectionNumber,
                        curtainSelectionDate = cs.curtainSelectionDate,
                        customerName = cs.MstCustomer != null ? cs.MstCustomer.name : null,
                        isQuotationCreated = cs.isQuotationCreated
                    })
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMTrnCurtainSelectionList>
            {
                Data = curtainSelectionView,
                TotalCount = repo.TrnCurtainSelections.Where(cs => !string.IsNullOrEmpty(search)
                    ? cs.curtainSelectionNumber.StartsWith(search)
                    || cs.MstCustomer.name.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? cs.isQuotationCreated : search.ToLower().Equals("no") ? !(cs.isQuotationCreated) : false) : true).Count(),
                Page = page
            };
        }

        public List<VMProductForCS> getSerialNumberForCS(Int64 collectionId)
        {
            var result = repo.MstFWRShades.Where(s => s.collectionId == collectionId)
                                    .Select(s => new VMProductForCS
                                    {
                                        shadeId = s.id,
                                        serialno = s.serialNumber.ToString() + " (" + s.shadeCode + "-" + s.MstFWRDesign.designCode + ")",
                                        rrp = s.MstQuality.rrp != null ? s.MstQuality.rrp : null,
                                        flatRate = s.MstQuality.flatRate != null ? s.MstQuality.flatRate : null,
                                        maxFlatRateDisc = s.MstQuality.maxFlatRateDisc != null ? s.MstQuality.maxFlatRateDisc : null,
                                        maxCutRateDisc = s.MstQuality.maxCutRateDisc != null ? s.MstQuality.maxCutRateDisc : null,
                                        maxRoleRateDisc = s.MstQuality.maxRoleRateDisc != null ? s.MstQuality.maxRoleRateDisc : null,
                                        fabricWidth = s.MstQuality.width != null ? s.MstQuality.width : null,
                                        gst = s.MstQuality.MstHsn != null ? s.MstQuality.MstHsn.gst : 0
                                    }).ToList();
            return result;
        }

        public List<VMProductForCS> getAccessoryItemCodeForCS()
        {
            return repo.MstAccessories.Where(a => !(a.name.ToLower().Contains("rod")
                                                    || a.name.ToLower().Contains("track")
                                                    || a.name.ToLower().Contains("remote")
                                                    || a.name.ToLower().Contains("motor")
                                                    || a.itemCode.ToLower().Contains("rod")
                                                    || a.itemCode.ToLower().Contains("track")
                                                    || a.itemCode.ToLower().Contains("remote")
                                                    || a.itemCode.ToLower().Contains("motor")))
                                    .Select(a => new VMProductForCS
                                    {
                                        accessoryId = a.id,
                                        itemCode = a.itemCode,
                                        sellingRate = a.sellingRate,
                                        gst = a.MstHsn.gst
                                    }).ToList();
        }

        public VMTrnCurtainSelection getCurtainSelectionById(Int64 id)
        {
            var result = repo.TrnCurtainSelections.Where(ms => ms.id == id).FirstOrDefault();
            VMTrnCurtainSelection curtainSelectionView = Mapper.Map<TrnCurtainSelection, VMTrnCurtainSelection>(result);
            curtainSelectionView.customerName = result.MstCustomer.name;

            curtainSelectionView.TrnCurtainSelectionItems.ForEach(csItem =>
            {
                csItem.categoryName = csItem.MstCategory.name;
                csItem.collectionName = csItem.collectionId != null ? csItem.MstCollection.collectionCode : null;
                csItem.serialno = csItem.MstCategory.code.Equals("Fabric") || csItem.MstCategory.code.Equals("Rug") || csItem.MstCategory.code.Equals("Wallpaper")
                    ? csItem.MstFWRShade.serialNumber + "(" + csItem.MstFWRShade.shadeCode + "-" + csItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                csItem.itemCode = csItem.MstCategory.code.Equals("Accessories") ? csItem.MstAccessory.itemCode : null;
                csItem.shadeList = csItem.collectionId != null ? getSerialNumberForCS(Convert.ToInt64(csItem.collectionId)) : null;
            });
            curtainSelectionView.TrnCurtainSelectionItems.ForEach(csItem => csItem.TrnCurtainSelection = null);
            curtainSelectionView.TrnCurtainQuotations = null;
            return curtainSelectionView;
        }

        public ResponseMessage postCurtainSelection(VMTrnCurtainSelection curtainSelection)
        {
            using (var transaction = new TransactionScope())
            {
                TrnCurtainSelection curtainSelectionToPost = Mapper.Map<VMTrnCurtainSelection, TrnCurtainSelection>(curtainSelection);
                var curtainSelectionItems = curtainSelectionToPost.TrnCurtainSelectionItems.ToList();

                curtainSelectionItems.ForEach(csItems =>
                {
                    csItems.createdOn = DateTime.Now;
                    csItems.createdBy = _LoggedInuserId;
                });

                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= curtainSelectionToPost.curtainSelectionDate.Date && f.endDate >= curtainSelectionToPost.curtainSelectionDate.Date).FirstOrDefault();
                string curtainSelectionNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.curtainSelectionNumber, "CS");
                curtainSelectionToPost.curtainSelectionNumber = curtainSelectionNo;
                curtainSelectionToPost.financialYear = financialYear.financialYear;
                curtainSelectionToPost.createdOn = DateTime.Now;
                curtainSelectionToPost.createdBy = _LoggedInuserId;

                repo.TrnCurtainSelections.Add(curtainSelectionToPost);

                financialYear.curtainSelectionNumber += 1;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(curtainSelectionToPost.id, resourceManager.GetString("CSAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putCurtainSelection(VMTrnCurtainSelection curtainSelection)
        {
            using (var transaction = new TransactionScope())
            {
                var curtainSelectionToPut = repo.TrnCurtainSelections.Where(ms => ms.id == curtainSelection.id).FirstOrDefault();
                curtainSelectionToPut.customerId = curtainSelection.customerId;
                curtainSelectionToPut.shippingAddressId = curtainSelection.shippingAddressId;
                curtainSelectionToPut.referById = curtainSelection.referById;
                curtainSelectionToPut.isQuotationCreated = curtainSelection.isQuotationCreated;

                updateCSItems(curtainSelection);

                curtainSelectionToPut.updatedOn = DateTime.Now;
                curtainSelectionToPut.updatedBy = _LoggedInuserId;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(curtainSelection.id, resourceManager.GetString("CSUpdated"), ResponseType.Success);
            }
        }

        public void updateCSItems(VMTrnCurtainSelection curtainSelection)
        {
            var curtainSelectionToPut = repo.TrnCurtainSelections.Where(q => q.id == curtainSelection.id).FirstOrDefault();

            List<TrnCurtainSelectionItem> itemsToRemove = new List<TrnCurtainSelectionItem>();
            foreach (var csItem in curtainSelectionToPut.TrnCurtainSelectionItems)
            {
                if (curtainSelection.TrnCurtainSelectionItems.Any(y => y.id == csItem.id))
                {
                    continue;
                }
                else
                {
                    itemsToRemove.Add(csItem);
                }
            }

            repo.TrnCurtainSelectionItems.RemoveRange(itemsToRemove);
            repo.SaveChanges();

            curtainSelection.TrnCurtainSelectionItems.ForEach(x =>
            {
                if (curtainSelectionToPut.TrnCurtainSelectionItems.Any(y => y.id == x.id))
                {
                    var csItemToPut = repo.TrnCurtainSelectionItems.Where(p => p.id == x.id).FirstOrDefault();

                    csItemToPut.area = x.area;
                    csItemToPut.unit = x.unit;
                    csItemToPut.patternId = x.patternId;
                    csItemToPut.categoryId = x.categoryId;
                    csItemToPut.collectionId = x.collectionId;
                    csItemToPut.shadeId = x.shadeId;
                    csItemToPut.accessoryId = x.accessoryId;
                    csItemToPut.isPatch = x.isPatch;
                    csItemToPut.isLining = x.isLining;
                    csItemToPut.rate = x.rate;
                    csItemToPut.discount = x.discount;
                    csItemToPut.updatedOn = DateTime.Now;
                    csItemToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
                else
                {
                    TrnCurtainSelectionItem csItem = Mapper.Map<VMTrnCurtainSelectionItem, TrnCurtainSelectionItem>(x);
                    csItem.curtainSelectionId = curtainSelection.id;
                    csItem.createdBy = _LoggedInuserId;
                    csItem.createdOn = DateTime.Now;
                    repo.TrnCurtainSelectionItems.Add(csItem);
                    repo.SaveChanges();
                }
            });

        }

        public VMTrnCurtainQuotation createCurtainQuotation(Int64 curtainSelectionId)
        {
            var curtainSelection = repo.TrnCurtainSelections.Where(ms => ms.id == curtainSelectionId).FirstOrDefault();

            VMTrnCurtainQuotation VMCurtainQuotation = new VMTrnCurtainQuotation();
            VMCurtainQuotation.TrnCurtainQuotationItems = new List<VMTrnCurtainQuotationItem>();
            VMCurtainQuotation.curtainSelectionId = curtainSelectionId;
            VMCurtainQuotation.curtainSelectionNo = curtainSelection.curtainSelectionNumber;

            VMCurtainQuotation.customerId = curtainSelection.customerId;
            VMCurtainQuotation.shippingAddressId = curtainSelection.shippingAddressId;
            //VMCurtainQuotation.MstCustomerAddress = Mapper.Map<MstCustomerAddress, VMCustomerAddress>(curtainSelection.MstCustomerAddress);
            VMCurtainQuotation.customerName = curtainSelection.MstCustomer.name;

            VMCurtainQuotation.referById = curtainSelection.referById;
            VMCurtainQuotation.agentName = curtainSelection.MstAgent != null ? curtainSelection.MstAgent.name : null;

            foreach (var csItem in curtainSelection.TrnCurtainSelectionItems)
            {
                VMTrnCurtainQuotationItem cqItem = new VMTrnCurtainQuotationItem();

                cqItem.area = csItem.area;
                cqItem.unit = csItem.unit;
                cqItem.patternId = csItem.patternId;
                cqItem.categoryId = csItem.categoryId;
                cqItem.categoryName = csItem.MstCategory != null ? csItem.MstCategory.code : null;
                cqItem.collectionId = csItem.collectionId;
                cqItem.collectionName = csItem.MstCollection != null ? csItem.MstCollection.collectionCode : null;
                cqItem.shadeId = csItem.shadeId;
                cqItem.serialno = csItem.shadeId != null ? csItem.MstFWRShade.serialNumber + "(" + csItem.MstFWRShade.shadeCode + "-" + csItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                cqItem.accessoryId = csItem.accessoryId;
                cqItem.itemCode = csItem.MstAccessory != null ? csItem.MstAccessory.itemCode : null;
                cqItem.isPatch = csItem.isPatch;
                cqItem.isLining = csItem.isLining;

                cqItem.MstPattern = Mapper.Map<MstPattern, VMPattern>(csItem.MstPattern);
                
                if (csItem.shadeId != null)
                {
                    VMProductForCS shadeInfo = new VMProductForCS();
                    shadeInfo.shadeId = csItem.shadeId;
                    shadeInfo.serialno = csItem.MstFWRShade.serialNumber + "(" + csItem.MstFWRShade.shadeCode + "-" + csItem.MstFWRShade.MstFWRDesign.designCode + ")";
                    shadeInfo.rrp = csItem.MstFWRShade.MstQuality.rrp;
                    shadeInfo.flatRate = csItem.MstFWRShade.MstQuality.flatRate;
                    shadeInfo.maxFlatRateDisc = csItem.MstFWRShade.MstQuality.maxFlatRateDisc;
                    shadeInfo.maxCutRateDisc = csItem.MstFWRShade.MstQuality.maxCutRateDisc;
                    shadeInfo.maxRoleRateDisc = csItem.MstFWRShade.MstQuality.maxRoleRateDisc;
                    shadeInfo.fabricWidth = csItem.MstFWRShade.MstQuality.width;
                    shadeInfo.gst = csItem.MstFWRShade.MstQuality.MstHsn.gst;
                    cqItem.shadeDetails = shadeInfo;
                    cqItem.rate = shadeInfo.rrp != null ? shadeInfo.rrp : shadeInfo.flatRate;
                    cqItem.rateWithGST = Math.Round(Convert.ToDecimal(cqItem.rate + ((cqItem.rate * csItem.MstFWRShade.MstQuality.MstHsn.gst) / 100)), 2);
                    cqItem.fabricDirection = ((cqItem.isPatch == false && cqItem.isLining == false) || cqItem.isLining) && (shadeInfo.fabricWidth > 100) ? "Vertical" : null;

                }
                else if (csItem.accessoryId != null)
                {
                    VMProductForCS accessoryInfo = new VMProductForCS();
                    accessoryInfo.accessoryId = csItem.accessoryId;
                    accessoryInfo.itemCode = csItem.MstAccessory.itemCode;
                    accessoryInfo.sellingRate = csItem.MstAccessory.sellingRate;
                    accessoryInfo.gst = csItem.MstAccessory.MstHsn.gst;
                    cqItem.accessoriesDetails = accessoryInfo;
                    cqItem.rate = accessoryInfo.sellingRate;
                    cqItem.rateWithGST = Math.Round(Convert.ToDecimal(cqItem.rate + ((cqItem.rate * csItem.MstAccessory.MstHsn.gst) / 100)), 2);
    
                }
                VMCurtainQuotation.TrnCurtainQuotationItems.Add(cqItem);
            }
            return VMCurtainQuotation;
        }

        public Int64 viewCurtainQuotation(Int64 curtainSelectionId)
        {
            Int64 curtainQuotationId = repo.TrnCurtainQuotations.Where(cq => cq.curtainSelectionId == curtainSelectionId).FirstOrDefault().id;
            return curtainQuotationId;
        }
    }
}