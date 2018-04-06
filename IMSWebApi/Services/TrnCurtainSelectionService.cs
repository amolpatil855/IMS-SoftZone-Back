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
                                        serialno =  s.serialNumber.ToString() + " (" + s.shadeCode + "-" + s.MstFWRDesign.designCode +  ")",
                                        rrp = s.MstQuality.rrp != null ? s.MstQuality.rrp : null,
                                        flatRate = s.MstQuality.flatRate != null ? s.MstQuality.flatRate : null,
                                        maxFlatRateDisc = s.MstQuality.maxFlatRateDisc != null ? s.MstQuality.maxFlatRateDisc : null
                                    }).ToList();
            return result;
        }

        public List<VMProductForCS> getAccessoryItemCodeForCS()
        {
            return repo.MstAccessories.Select(a => new VMProductForCS
                                    {
                                        accessoryId = a.id,
                                        itemCode = a.itemCode,
                                        sellingRate = a.sellingRate
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
                
            });
            curtainSelectionView.TrnCurtainSelectionItems.ForEach(csItem => csItem.TrnCurtainSelection = null);

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
                curtainSelectionToPut.referById = curtainSelection.referById;
                curtainSelectionToPut.isQuotationCreated = curtainSelection.isQuotationCreated;

                updateMSItems(curtainSelection);

                curtainSelectionToPut.updatedOn = DateTime.Now;
                curtainSelectionToPut.updatedBy = _LoggedInuserId;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(curtainSelection.id, resourceManager.GetString("CSUpdated"), ResponseType.Success);
            }
        }

        public void updateMSItems(VMTrnCurtainSelection curtainSelection)
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
    }
}