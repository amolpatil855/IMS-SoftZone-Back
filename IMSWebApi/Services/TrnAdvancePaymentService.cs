using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Reflection;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;
using System.Transactions;
using IMSWebApi.Enums;

namespace IMSWebApi.Services
{
    public class TrnAdvancePaymentService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        GenerateOrderNumber generateOrderNumber = null;

        public TrnAdvancePaymentService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
            generateOrderNumber = new GenerateOrderNumber();
        }

        public ListResult<VMTrnAdvancePaymentList> getAdvancePayments(int pageSize, int page, string search, string quotationType)
        {
            List<VMTrnAdvancePaymentList> advancePaymentView;
            advancePaymentView = repo.TrnAdvancePayments.Where(ap => (!string.IsNullOrEmpty(search)
                    ? ap.advancePaymentNumber.StartsWith(search)
                    || ap.MstCustomer.name.StartsWith(search)
                    || (ap.TrnMaterialQuotation != null ? ap.TrnMaterialQuotation.materialQuotationNumber.StartsWith(search) : false)
                    || (ap.TrnCurtainQuotation != null ? ap.TrnCurtainQuotation.curtainQuotationNumber.StartsWith(search) : false)
                    || ap.amount.ToString().StartsWith(search): true)
                    && (!string.IsNullOrEmpty(quotationType) ? ap.quotationType.Equals(quotationType) : true))
                    .Select(advPayment => new VMTrnAdvancePaymentList
                    {
                        id = advPayment.id,
                        advancePaymentNumber = advPayment.advancePaymentNumber,
                        advancePaymentDate = advPayment.advancePaymentDate,
                        customerName = advPayment.customerId != null ? advPayment.MstCustomer.name : string.Empty,
                        quotationNumber = advPayment.materialQuotationId != null ? advPayment.TrnMaterialQuotation.materialQuotationNumber : 
                                advPayment.curtainQuotationId != null ? advPayment.TrnCurtainQuotation.curtainQuotationNumber : string.Empty,
                        quotationType = advPayment.quotationType,
                        amount = advPayment.amount,
                        paymentMode = advPayment.paymentMode
                    })
                    .OrderByDescending(q => q.id)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMTrnAdvancePaymentList>
            {
                Data = advancePaymentView,
                TotalCount = repo.TrnAdvancePayments.Where(ap => (!string.IsNullOrEmpty(search)
                    ? ap.advancePaymentNumber.StartsWith(search)
                    || ap.MstCustomer.name.StartsWith(search)
                    || (ap.TrnMaterialQuotation != null ? ap.TrnMaterialQuotation.materialQuotationNumber.StartsWith(search) : false)
                    || (ap.TrnCurtainQuotation != null ? ap.TrnCurtainQuotation.curtainQuotationNumber.StartsWith(search) : false)
                    || ap.amount.ToString().StartsWith(search): true)
                    && (!string.IsNullOrEmpty(quotationType) ? ap.quotationType.Equals(quotationType) : true)).Count(),
                Page = page
            };
        }

        public VMTrnAdvancePayment getAdvancePaymentById(Int64 id)
        {
            var result = repo.TrnAdvancePayments.Where(ap => ap.id == id).FirstOrDefault();
            VMTrnAdvancePayment advancePaymentView = Mapper.Map<TrnAdvancePayment, VMTrnAdvancePayment>(result);
            advancePaymentView.quotationNumber = result.TrnMaterialQuotation != null ? result.TrnMaterialQuotation.materialQuotationNumber :
                result.TrnCurtainQuotation != null ? result.TrnCurtainQuotation.curtainQuotationNumber : string.Empty;
            advancePaymentView.customerName = result.MstCustomer != null ? result.MstCustomer.name : string.Empty;

            if (advancePaymentView.TrnMaterialQuotation != null)
            {
                advancePaymentView.TrnMaterialQuotation.TrnMaterialSelection = null;
                advancePaymentView.TrnMaterialQuotation.TrnMaterialQuotationItems.ForEach(mqItem => mqItem.TrnMaterialQuotation = null);
            }
            else if (advancePaymentView.TrnCurtainQuotation != null)
            {
                advancePaymentView.TrnCurtainQuotation.TrnCurtainSelection = null;
                advancePaymentView.TrnCurtainQuotation.TrnCurtainQuotationItems.ForEach(cqItem => cqItem.TrnCurtainQuotation = null);
            }
            return advancePaymentView;
        }

        public ResponseMessage postAdvancePayment(VMTrnAdvancePayment advancePayment)
        {
            using (var transaction = new TransactionScope())
            {
                TrnAdvancePayment advancePaymentToPost = Mapper.Map<VMTrnAdvancePayment, TrnAdvancePayment>(advancePayment);

                var financialYear = repo.MstFinancialYears.Where(f => f.startDate <= advancePayment.advancePaymentDate.Date && f.endDate >= advancePayment.advancePaymentDate.Date).FirstOrDefault();
                string advPaymentNo = generateOrderNumber.orderNumber(financialYear.startDate.ToString("yy"), financialYear.endDate.ToString("yy"), financialYear.apNumber, "AP");
                advancePaymentToPost.advancePaymentNumber = advPaymentNo;
                advancePaymentToPost.financialYear = financialYear.financialYear;
                advancePaymentToPost.createdOn = DateTime.Now;
                advancePaymentToPost.createdBy = _LoggedInuserId;

                repo.TrnAdvancePayments.Add(advancePaymentToPost);
                financialYear.apNumber += 1;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(advancePaymentToPost.id, resourceManager.GetString("APAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putAdvancePayment(VMTrnAdvancePayment advancePayment)
        {
            using (var transaction = new TransactionScope())
            {
                var advancePaymentToPut = repo.TrnAdvancePayments.Where(ap => ap.id == advancePayment.id).FirstOrDefault();

                advancePaymentToPut.customerId = advancePayment.customerId;
                advancePaymentToPut.materialQuotationId = advancePayment.materialQuotationId;
                advancePaymentToPut.curtainQuotationId = advancePayment.curtainQuotationId;
                advancePaymentToPut.quotationType = advancePayment.quotationType;
                advancePaymentToPut.amount = advancePayment.amount;
                advancePaymentToPut.paymentMode = advancePayment.paymentMode;
                advancePaymentToPut.chequeNumber = advancePayment.chequeNumber;
                advancePaymentToPut.chequeDate = advancePayment.chequeDate;
                advancePaymentToPut.bankName = advancePayment.bankName;
                advancePaymentToPut.bankBranch = advancePayment.bankBranch;

                advancePaymentToPut.updatedOn = DateTime.Now;
                advancePaymentToPut.updatedBy = _LoggedInuserId;
                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(advancePayment.id, resourceManager.GetString("APUpdated"), ResponseType.Success);
            }
        }
    }
}