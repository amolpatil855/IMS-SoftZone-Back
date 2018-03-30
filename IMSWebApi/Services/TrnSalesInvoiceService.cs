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
using IMSWebApi.Enums;
using AutoMapper;
using System.Transactions;
using IMSWebApi.ViewModel.SalesInvoice;

namespace IMSWebApi.Services
{
    public class TrnSalesInvoiceService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        bool _IsCustomer;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;
        SendEmail emailNotification = new SendEmail();
        TrnProductStockService _trnProductStockService = null;

        public TrnSalesInvoiceService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _IsCustomer = HttpContext.Current.User.IsInRole("Customer");
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
            _trnProductStockService = new TrnProductStockService();
        }

        public ListResult<VMTrnSalesInvoiceList> getSalesInvoices(int pageSize, int page, string search)
        {
            List<VMTrnSalesInvoiceList> salesInvoiceView;
            var result = repo.TrnSalesInvoices
                    .Select(s => new VMTrnSalesInvoiceList
                    {
                        id = s.id,
                        invoiceNumber = s.invoiceNumber,
                        invoiceDate = s.invoiceDate,
                        ginNumber = s.TrnGoodIssueNote.ginNumber,
                        totalAmount = s.totalAmount,
                        status = s.status,
                        courierDockYardNumber = s.courierDockYardNumber,
                        isPaid = s.isPaid
                    })
                    .Where(s => !string.IsNullOrEmpty(search)
                    ? s.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.ginNumber.StartsWith(search)
                    || s.totalAmount.ToString().StartsWith(search)
                    || s.courierDockYardNumber.StartsWith(search)
                    || s.status.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? s.isPaid  : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true)
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            salesInvoiceView = result;

            return new ListResult<VMTrnSalesInvoiceList>
            {
                Data = salesInvoiceView,
                TotalCount = repo.TrnSalesInvoices.Where(s => !string.IsNullOrEmpty(search)
                    ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.TrnSaleOrder.orderNumber.StartsWith(search)
                    || s.status.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true).Count(),
                Page = page
            };
        }

        public VMTrnSalesInvoice getSalesInvoiceById(Int64 id)
        {
            TrnSalesInvoice result;
            VMTrnSalesInvoice salesInvoiceView = new VMTrnSalesInvoice();
            if (!_IsCustomer)
            {
                result = repo.TrnSalesInvoices.Where(s => s.id == id).FirstOrDefault();
            }
            else
            {
                Int64 customerId = repo.MstCustomers.Where(c => c.userId == _LoggedInuserId).FirstOrDefault().id;
                result = repo.TrnSalesInvoices.Where(s => s.id == id
                                                     && !(s.status.Equals("Created"))
                                                     && (s.TrnMaterialQuotation != null ? s.TrnMaterialQuotation.customerId == customerId : s.TrnSaleOrder.customerId == customerId)).FirstOrDefault();
            }
            if (result != null)
            {
                salesInvoiceView = Mapper.Map<TrnSalesInvoice, VMTrnSalesInvoice>(result);
                salesInvoiceView.TrnGoodIssueNote.TrnGoodIssueNoteItems.ForEach(ginItems => ginItems.TrnGoodIssueNote = null);
                salesInvoiceView.TrnSalesInvoiceItems.ForEach(salesInvoiceItems => salesInvoiceItems.TrnSalesInvoice = null);
                if (salesInvoiceView.TrnMaterialQuotation != null)
                {
                    salesInvoiceView.TrnMaterialQuotation.TrnMaterialQuotationItems.ForEach(mqItem => mqItem.TrnMaterialQuotation = null);
                    salesInvoiceView.TrnMaterialQuotation.TrnMaterialSelection = null;
                }
                salesInvoiceView.TrnSalesInvoiceItems.ForEach(salesInvoiceItem =>
                {
                    salesInvoiceItem.categoryName = salesInvoiceItem.MstCategory.name;
                    salesInvoiceItem.collectionName = salesInvoiceItem.collectionId != null ? salesInvoiceItem.MstCollection.collectionCode
                        + " (" + salesInvoiceItem.MstCollection.MstSupplier.code + ")" : null;
                    salesInvoiceItem.serialno = salesInvoiceItem.MstCategory.code.Equals("Fabric")
                                    || salesInvoiceItem.MstCategory.code.Equals("Rug")
                                    || salesInvoiceItem.MstCategory.code.Equals("Wallpaper")
                                    ? salesInvoiceItem.MstFWRShade.serialNumber + "(" + salesInvoiceItem.MstFWRShade.shadeCode + "-" + salesInvoiceItem.MstFWRShade.MstFWRDesign.designCode + ")" : null;
                    salesInvoiceItem.size = salesInvoiceItem.MstMatSize != null ?
                        salesInvoiceItem.MstMatSize.sizeCode + " (" + salesInvoiceItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + salesInvoiceItem.MstMatSize.MstQuality.qualityCode + ")" :
                                salesInvoiceItem.MstFomSize != null ? salesInvoiceItem.MstFomSize.itemCode :
                                salesInvoiceItem.sizeCode != null ? salesInvoiceItem.sizeCode : null;
                    salesInvoiceItem.accessoryName = salesInvoiceItem.accessoryId != null ? salesInvoiceItem.MstAccessory.itemCode : null;
                });
                salesInvoiceView.MstCompanyInfo = Mapper.Map<MstCompanyInfo, VMCompanyInfo>(repo.MstCompanyInfoes.FirstOrDefault());
                salesInvoiceView.isApproved = salesInvoiceView.status.Equals("Created") ? false : true;
            }

            return salesInvoiceView;
        }

        public void createInvoiceForGIN(VMTrnGoodIssueNote goodIssueNote)
        {
            TrnSalesInvoice salesInvoice = new TrnSalesInvoice();
            salesInvoice.goodIssueNoteId = goodIssueNote.id;
            salesInvoice.salesOrderId = goodIssueNote.salesOrderId;
            salesInvoice.materialQuotationId = goodIssueNote.materialQuotationId;

            DateTime invoiceDate = goodIssueNote.ginDate != null ? goodIssueNote.ginDate.Value.Date : DateTime.Now;
            var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= invoiceDate && f.endDate >= invoiceDate).FirstOrDefault();
            string invoiceNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.soInvoiceNumber,"IN");
            salesInvoice.invoiceNumber = invoiceNo;
            salesInvoice.isPaid = false;
            salesInvoice.invoiceDate = DateTime.Now;
            salesInvoice.status = InvoiceStatus.Created.ToString();
            salesInvoice.financialYear = financialYear.financialYear;
            salesInvoice.expectedDeliveryDate = salesInvoice.invoiceDate.AddDays(goodIssueNote.customerId != 0 ? Convert.ToDouble(goodIssueNote.MstCustomer.creditPeriodDays) : 0);

            goodIssueNote.TrnGoodIssueNoteItems.ForEach(ginItem =>
            {
                if (ginItem.issuedQuantity > 0)
                {
                    TrnSalesInvoiceItem salesInvoiceItem = new TrnSalesInvoiceItem();
                    salesInvoiceItem.categoryId = ginItem.categoryId;
                    salesInvoiceItem.collectionId = ginItem.collectionId;
                    salesInvoiceItem.shadeId = ginItem.shadeId;
                    salesInvoiceItem.fomSizeId = ginItem.fomSizeId;
                    salesInvoiceItem.matSizeId = ginItem.matSizeId;
                    salesInvoiceItem.sizeCode = ginItem.sizeCode;
                    salesInvoiceItem.accessoryId = ginItem.accessoryId;
                    salesInvoiceItem.quantity = Convert.ToDecimal(ginItem.issuedQuantity);
                    salesInvoiceItem.rate = ginItem.rate;
                    salesInvoiceItem.discountPercentage = ginItem.discountPercentage != null ? ginItem.discountPercentage : 0;
                    decimal _amount = (ginItem.rate * Convert.ToDecimal(ginItem.issuedQuantity));
                    decimal discountAmt = _amount - (_amount * Convert.ToDecimal(ginItem.discountPercentage) / 100);
                    salesInvoiceItem.amount = Convert.ToInt64(Math.Round(discountAmt, MidpointRounding.AwayFromZero));
                    //salesInvoiceItem.amount = Convert.ToInt32(Math.Round((ginItem.rate - (ginItem.rate * Convert.ToDecimal(ginItem.discountPercentage) ) / 100) * Convert.ToDecimal(ginItem.issuedQuantity))); 
                    salesInvoiceItem.gst = ginItem.shadeId != null ? ginItem.MstFWRShade.MstQuality.MstHsn.gst :
                        ginItem.fomSizeId != null ? ginItem.MstFomSize.MstQuality.MstHsn.gst :
                        ginItem.matSizeId != null ? ginItem.MstMatSize.MstQuality.MstHsn.gst :
                        ginItem.accessoryId != null ? ginItem.MstAccessory.MstHsn.gst :
                        ginItem.MstQuality.MstHsn.gst;
                    salesInvoiceItem.uom = ginItem.categoryId != 7 ? ginItem.MstCategory.MstUnitOfMeasure.uomCode : null;
                    salesInvoiceItem.hsnCode = ginItem.shadeId != null ? ginItem.MstFWRShade.MstQuality.MstHsn.hsnCode :
                        ginItem.fomSizeId != null ? ginItem.MstFomSize.MstQuality.MstHsn.hsnCode :
                        ginItem.matSizeId != null ? ginItem.MstMatSize.MstQuality.MstHsn.hsnCode :
                        ginItem.accessoryId != null ? ginItem.MstAccessory.MstHsn.hsnCode :
                        ginItem.MstQuality.MstHsn.hsnCode;

                    salesInvoiceItem.rateWithGST = salesInvoiceItem.rate + (salesInvoiceItem.rate * salesInvoiceItem.gst) / 100;
                    //salesInvoiceItem.amountWithGST = Convert.ToInt32(Math.Round((Convert.ToDecimal(salesInvoiceItem.rateWithGST) - (Convert.ToDecimal(salesInvoiceItem.rateWithGST) * Convert.ToDecimal(ginItem.discountPercentage)) / 100) * Convert.ToDecimal(ginItem.issuedQuantity))); 

                    //salesInvoiceItem.amountWithGST = Convert.ToInt32(Math.Round(Convert.ToDecimal(discountAmt + ((discountAmt * salesInvoiceItem.gst) / 100)),MidpointRounding.AwayFromZero));
                    salesInvoiceItem.amountWithGST = Convert.ToInt64(Math.Round(Convert.ToDecimal(salesInvoiceItem.amount + Convert.ToDouble(Convert.ToDouble(salesInvoiceItem.amount) * Convert.ToDouble(salesInvoiceItem.gst) / 100)), MidpointRounding.AwayFromZero));
                    salesInvoiceItem.createdOn = DateTime.Now;
                    salesInvoiceItem.createdBy = _LoggedInuserId;
                    salesInvoice.TrnSalesInvoiceItems.Add(salesInvoiceItem);
                }
            });

            salesInvoice.totalAmount = Convert.ToInt64(salesInvoice.TrnSalesInvoiceItems.Select(salesInvoiceItem => salesInvoiceItem.amountWithGST).Sum());
            salesInvoice.createdOn = DateTime.Now;
            salesInvoice.createdBy = _LoggedInuserId;

            repo.TrnSalesInvoices.Add(salesInvoice);

            financialYear.soInvoiceNumber += 1;
            repo.SaveChanges();
        }

        public ResponseMessage putSalesInvoice(VMTrnSalesInvoice salesInvoice)
        {
            using (var transaction = new TransactionScope())
            {
                var salesInvoiceToPut = repo.TrnSalesInvoices.Where(q => q.id == salesInvoice.id).FirstOrDefault();

                salesInvoiceToPut.buyersOrderNumber = salesInvoice.buyersOrderNumber;
                salesInvoiceToPut.totalAmount = salesInvoice.totalAmount;
                salesInvoiceToPut.amountPaid = salesInvoice.amountPaid;
                salesInvoiceToPut.courierDockYardNumber = salesInvoice.courierDockYardNumber;
                salesInvoiceToPut.isPaid = salesInvoice.isPaid;

                salesInvoiceToPut.status = salesInvoice.isApproved ? InvoiceStatus.Approved.ToString() : salesInvoice.status;

                updateSalesInvoiceItems(salesInvoice);

                salesInvoiceToPut.updatedOn = DateTime.Now;
                salesInvoiceToPut.updatedBy = _LoggedInuserId;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(salesInvoice.id, resourceManager.GetString("SalesInvoiceUpdated"), ResponseType.Success);
            }
        }

        public void updateSalesInvoiceItems(VMTrnSalesInvoice salesInvoice)
        {
            var salesInvoiceToPut = repo.TrnSalesInvoices.Where(so => so.id == salesInvoice.id).FirstOrDefault();

            salesInvoice.TrnSalesInvoiceItems.ForEach(x =>
            {
                if (salesInvoiceToPut.TrnSalesInvoiceItems.Any(y => y.id == x.id))
                {
                    var salesInvoiceItemToPut = repo.TrnSalesInvoiceItems.Where(p => p.id == x.id).FirstOrDefault();

                    salesInvoiceItemToPut.rateWithGST = x.rateWithGST;
                    salesInvoiceItemToPut.amountWithGST = x.amountWithGST;
                    salesInvoiceItemToPut.updatedOn = DateTime.Now;
                    salesInvoiceItemToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
            });

        }

        public ResponseMessage approveSalesInvoice(Int64 id)
        {
            using (var transaction = new TransactionScope())
            {
                var salesInvoice = repo.TrnSalesInvoices.Where(po => po.id == id).FirstOrDefault();
                salesInvoice.status = InvoiceStatus.Approved.ToString();

                salesInvoice.updatedBy = _LoggedInuserId;
                salesInvoice.updatedOn = DateTime.Now;

                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("SalesInvoiceApproved"), ResponseType.Success);
            }
        }

        public ListResult<VMTrnSalesInvoiceList> getSalesInvoicesForLoggedInUser(int pageSize, int page, string search)
        {
            Int64 customerId = repo.MstCustomers.Where(c => c.userId == _LoggedInuserId).FirstOrDefault().id;
            List<VMTrnSalesInvoiceList> salesInvoiceView;
            salesInvoiceView = repo.TrnSalesInvoices
                    .Where(s => (!string.IsNullOrEmpty(search)
                    ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.totalAmount.ToString().StartsWith(search)
                    || s.courierDockYardNumber.StartsWith(search)
                    || s.status.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true)
                    && (s.TrnMaterialQuotation != null ? s.TrnMaterialQuotation.customerId == customerId : s.TrnSaleOrder.customerId == customerId)
                    && !(s.status.Equals("Created")))
                     .Select(s => new VMTrnSalesInvoiceList
                     {
                         id = s.id,
                         invoiceNumber = s.invoiceNumber,
                         invoiceDate = s.invoiceDate,
                         ginNumber = s.TrnGoodIssueNote.ginNumber,
                         totalAmount = s.totalAmount,
                         status = s.status,
                         courierDockYardNumber = s.courierDockYardNumber,
                         isPaid = s.isPaid
                     })
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            
            return new ListResult<VMTrnSalesInvoiceList>
            {
                Data = salesInvoiceView,
                TotalCount = repo.TrnSalesInvoices
                    .Where(s => (!string.IsNullOrEmpty(search)
                    ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.totalAmount.ToString().StartsWith(search)
                    || s.courierDockYardNumber.StartsWith(search)
                    || s.status.StartsWith(search)
                    || (search.ToLower().Equals("yes") ? s.isPaid : search.ToLower().Equals("no") ? !(s.isPaid) : false) : true)
                    && (s.TrnMaterialQuotation != null ? s.TrnMaterialQuotation.customerId == customerId : s.TrnSaleOrder.customerId == customerId)
                    && !(s.status.Equals("Created"))).Count(),
                Page = page
            };
        }
    }
}