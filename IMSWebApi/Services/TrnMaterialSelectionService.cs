using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.ViewModel;
using IMSWebApi.Common;
using AutoMapper;
using System.Transactions;
using IMSWebApi.Enums;

namespace IMSWebApi.Services
{
    public class TrnMaterialSelectionService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;

        public TrnMaterialSelectionService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
        }

        public ListResult<VMTrnMaterialSelection> getMaterialSelections(int pageSize, int page, string search)
        {
            List<VMTrnMaterialSelection> materialSelectionView;
            if (pageSize > 0)
            {
                var result = repo.TrnMaterialSelections.Where(ms => !string.IsNullOrEmpty(search)
                    ? ms.materialSelectionNumber.StartsWith(search)
                    || ms.MstCustomer.name.StartsWith(search)
                    || ms.isQuotationCreated.ToString().StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                materialSelectionView = Mapper.Map<List<TrnMaterialSelection>, List<VMTrnMaterialSelection>>(result);
            }
            else
            {
                var result = repo.TrnMaterialSelections.Where(ms => !string.IsNullOrEmpty(search)
                    ? ms.materialSelectionNumber.StartsWith(search)
                    || ms.MstCustomer.name.StartsWith(search)
                    || ms.isQuotationCreated.ToString().StartsWith(search) : true).ToList();
                materialSelectionView = Mapper.Map<List<TrnMaterialSelection>, List<VMTrnMaterialSelection>>(result);
            }
            materialSelectionView.ForEach(ms => ms.TrnMaterialSelectionItems.ForEach(msItem => msItem.TrnMaterialSelection = null));
            return new ListResult<VMTrnMaterialSelection>
            {
                Data = materialSelectionView,
                TotalCount = materialSelectionView.Count(),
                Page = page
            };
        }

        public VMTrnMaterialSelection getMaterialSelectionById(Int64 id)
        {
            var result = repo.TrnMaterialSelections.Where(ms => ms.id == id).FirstOrDefault();
            VMTrnMaterialSelection materialSelectionView = Mapper.Map<TrnMaterialSelection, VMTrnMaterialSelection>(result);
            materialSelectionView.customerName = result.MstCustomer.name;

            materialSelectionView.TrnMaterialSelectionItems.ForEach(msItem =>
            {
                msItem.categoryName = msItem.MstCategory.name;
                msItem.collectionName = msItem.collectionId != null ? msItem.MstCollection.collectionName : null;
                msItem.serialno = msItem.MstCategory.code.Equals("Fabric") || msItem.MstCategory.code.Equals("Rug") || msItem.MstCategory.code.Equals("Wallpaper") ? msItem.MstFWRShade.serialNumber + "(" + msItem.MstFWRShade.shadeCode + "-" + msItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                msItem.size = msItem.MstMatSize != null ? msItem.MstMatSize.sizeCode + " (" + msItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + msItem.MstMatSize.MstQuality.qualityCode + ")" : null;
            });
            materialSelectionView.TrnMaterialSelectionItems.ForEach(msItem => msItem.TrnMaterialSelection = null);
            return materialSelectionView;
        }

        public ResponseMessage postMaterialSelection(VMTrnMaterialSelection materialSelection)
        {
            using (var transaction = new TransactionScope())
            {
                TrnMaterialSelection materialSelectionToPost = Mapper.Map<VMTrnMaterialSelection, TrnMaterialSelection>(materialSelection);
                var materialSelectionItems = materialSelectionToPost.TrnMaterialSelectionItems.ToList();

                foreach (var msItems in materialSelectionItems)
                {
                    msItems.matSizeId = msItems.matSizeId == -1 ? null : msItems.matSizeId;     //set null for custom matSize
                    msItems.createdOn = DateTime.Now;
                    msItems.createdBy = _LoggedInuserId;
                }
                //materialSelectionToPost.materialSelectionDate = DateTime.Now;
                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= materialSelectionToPost.materialSelectionDate && f.endDate >= materialSelectionToPost.materialSelectionDate).FirstOrDefault();
                string materialselectionNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.materialSelectionNumber, "MS");
                materialSelectionToPost.materialSelectionNumber = materialselectionNo;
                materialSelectionToPost.createdOn = DateTime.Now;
                materialSelectionToPost.createdBy = _LoggedInuserId;

                repo.TrnMaterialSelections.Add(materialSelectionToPost);

                financialYear.materialSelectionNumber += 1;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(materialSelectionToPost.id, resourceManager.GetString("MSAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putMaterialSelection(VMTrnMaterialSelection materialSelection)
        {
            using (var transaction = new TransactionScope())
            {
                var materialSelectionToPut = repo.TrnMaterialSelections.Where(ms => ms.id == materialSelection.id).FirstOrDefault();

                materialSelectionToPut.isQuotationCreated = materialSelection.isQuotationCreated;

                updateMSItems(materialSelection);

                materialSelectionToPut.updatedOn = DateTime.Now;
                materialSelectionToPut.updatedBy = _LoggedInuserId;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(materialSelection.id, resourceManager.GetString("MSUpdated"), ResponseType.Success);
            }
        }

        public void updateMSItems(VMTrnMaterialSelection materialSelection)
        {
            var materialSelectionToPut = repo.TrnMaterialSelections.Where(q => q.id == materialSelection.id).FirstOrDefault();

            List<TrnMaterialSelectionItem> itemsToRemove = new List<TrnMaterialSelectionItem>();
            foreach (var msItem in materialSelectionToPut.TrnMaterialSelectionItems)
            {
                if (materialSelection.TrnMaterialSelectionItems.Any(y => y.id == msItem.id))
                {
                    continue;
                }
                else
                {
                    itemsToRemove.Add(msItem);
                }
            }

            repo.TrnMaterialSelectionItems.RemoveRange(itemsToRemove);
            repo.SaveChanges();

            materialSelection.TrnMaterialSelectionItems.ForEach(x =>
            {
                if (materialSelectionToPut.TrnMaterialSelectionItems.Any(y => y.id == x.id))
                {
                    var msItemToPut = repo.TrnMaterialSelectionItems.Where(p => p.id == x.id).FirstOrDefault();

                    msItemToPut.selectionType = x.selectionType;
                    msItemToPut.area = x.area;
                    msItemToPut.categoryId = x.categoryId;
                    msItemToPut.collectionId = x.collectionId;
                    msItemToPut.shadeId = x.shadeId;
                    msItemToPut.matSizeId = x.matSizeId;
                    msItemToPut.matThicknessId = x.matThicknessId;
                    msItemToPut.matHeight = x.matHeight;
                    msItemToPut.matWidth = x.matWidth;
                    msItemToPut.updatedOn = DateTime.Now;
                    msItemToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
                else
                {
                    TrnMaterialSelectionItem msItem = Mapper.Map<VMTrnMaterialSelectionItem, TrnMaterialSelectionItem>(x);
                    msItem.materialSelectionId = materialSelection.id;
                    msItem.matSizeId = msItem.matSizeId == -1 ? null : msItem.matSizeId;
                    msItem.createdBy = _LoggedInuserId;
                    msItem.createdOn = DateTime.Now;
                    repo.TrnMaterialSelectionItems.Add(msItem);
                    repo.SaveChanges();
                }
            });

        }
    }
}