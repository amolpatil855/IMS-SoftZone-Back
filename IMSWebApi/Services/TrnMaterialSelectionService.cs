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

                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= materialSelection.materialSelectionDate && f.endDate >= materialSelection.materialSelectionDate).FirstOrDefault();
                string materialselectionNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.materialSelectionNumber);
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
    }
}