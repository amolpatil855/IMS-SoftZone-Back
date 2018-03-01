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

namespace IMSWebApi.Services
{
    public class TrnSalesInvoiceService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;
        SendEmail emailNotification = new SendEmail();
        TrnProductStockService _trnProductStockService = null;

        public TrnSalesInvoiceService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
            _trnProductStockService = new TrnProductStockService();
        }

        public ListResult<VMTrnSalesInvoice> getSalesInvoices(int pageSize, int page, string search)
        {
            List<VMTrnSalesInvoice> salesInvoiceView;
            if (pageSize > 0)
            {
                var result = repo.TrnSalesInvoices.Where(s => !string.IsNullOrEmpty(search)
                    ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.TrnSaleOrder.orderNumber.StartsWith(search)
                    || s.status.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                salesInvoiceView = Mapper.Map<List<TrnSalesInvoice>, List<VMTrnSalesInvoice>>(result);
            }
            else
            {
                var result = repo.TrnSalesInvoices.Where(s => !string.IsNullOrEmpty(search)
                    ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.TrnSaleOrder.orderNumber.StartsWith(search)
                    || s.status.StartsWith(search) : true).ToList();
                salesInvoiceView = Mapper.Map<List<TrnSalesInvoice>, List<VMTrnSalesInvoice>>(result);
            }
            salesInvoiceView.ForEach(s => s.TrnGoodIssueNote.TrnGoodIssueNoteItems.ForEach(ginItems => ginItems.TrnGoodIssueNote = null));
            return new ListResult<VMTrnSalesInvoice>
            {
                Data = salesInvoiceView,
                TotalCount = repo.TrnSalesInvoices.Where(s => !string.IsNullOrEmpty(search)
                    ? s.TrnGoodIssueNote.ginNumber.StartsWith(search)
                    || s.invoiceNumber.StartsWith(search)
                    || s.TrnSaleOrder.orderNumber.StartsWith(search)
                    || s.status.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnSalesInvoice getSalesInvoiceById(Int64 id)
        {
            var result = repo.TrnSalesInvoices.Where(s => s.id == id).FirstOrDefault();
            VMTrnSalesInvoice salesInvoiceView = Mapper.Map<TrnSalesInvoice, VMTrnSalesInvoice>(result);
            salesInvoiceView.TrnGoodIssueNote.TrnGoodIssueNoteItems.ForEach(ginItems => ginItems.TrnGoodIssueNote = null);
            return salesInvoiceView;
        }

        public void createInvoiceForGIN(VMTrnGoodIssueNote goodIssueNote)
        {
            TrnSalesInvoice salesInvoice = new TrnSalesInvoice();
            salesInvoice.goodIssueNoteId = goodIssueNote.id;
            salesInvoice.salesOrderId = goodIssueNote.salesOrderId;

            var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= goodIssueNote.ginDate && f.endDate >= goodIssueNote.ginDate).FirstOrDefault();
            string invoiceNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.soInvoiceNumber);
            salesInvoice.invoiceNumber = invoiceNo;

            salesInvoice.invoiceDate = DateTime.Now;
            salesInvoice.status = InvoiceStatus.Created.ToString();

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
                    salesInvoiceItem.discountPrecentage = ginItem.discountPercentage;
                    salesInvoiceItem.amount = ginItem.amount;
                    salesInvoiceItem.gst = ginItem.shadeId != null ? ginItem.MstFWRShade.MstQuality.MstHsn.gst :
                        ginItem.fomSizeId != null ? ginItem.MstFomSize.MstQuality.MstHsn.gst :
                        ginItem.matSizeId != null ? ginItem.MstMatSize.MstQuality.MstHsn.gst :
                        ginItem.MstAccessory.MstHsn.gst;
                    salesInvoiceItem.uom = ginItem.categoryId != 7 ?  ginItem.MstCategory.MstUnitOfMeasure.uomCode : null;
                    salesInvoiceItem.hsnCode = ginItem.shadeId != null ? ginItem.MstFWRShade.MstQuality.MstHsn.hsnCode :
                        ginItem.fomSizeId != null ? ginItem.MstFomSize.MstQuality.MstHsn.hsnCode :
                        ginItem.matSizeId != null ? ginItem.MstMatSize.MstQuality.MstHsn.hsnCode :
                        ginItem.MstAccessory.MstHsn.hsnCode;

                    salesInvoiceItem.createdOn = DateTime.Now;
                    salesInvoiceItem.createdBy = _LoggedInuserId;
                    salesInvoice.TrnSalesInvoiceItems.Add(salesInvoiceItem);
                }
            });

            salesInvoice.createdOn = DateTime.Now;
            salesInvoice.createdBy = _LoggedInuserId;

            repo.TrnSalesInvoices.Add(salesInvoice);

            financialYear.soInvoiceNumber += 1;
            repo.SaveChanges();
        }
    }
}