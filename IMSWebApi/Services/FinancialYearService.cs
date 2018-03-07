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
using IMSWebApi.Enums;

namespace IMSWebApi.Services
{
    public class FinancialYearService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        public FinancialYearService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMFinancialYear> getFinancialYear(int pageSize, int page, string search)
        {
            List<VMFinancialYear> financialYearView;
            if (pageSize > 0)
            {
                var result = repo.MstFinancialYears.Where(f => !string.IsNullOrEmpty(search)
                    ? f.financialYear.StartsWith(search)
                    || f.startDate.ToString().StartsWith(search)
                    || f.endDate.ToString().StartsWith(search)
                    || f.soNumber.ToString().StartsWith(search)
                    || f.poNumber.ToString().StartsWith(search)
                    || f.grnNumber.ToString().StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                financialYearView = Mapper.Map<List<MstFinancialYear>, List<VMFinancialYear>>(result);
            }
            else
            {
                var result = repo.MstFinancialYears.Where(f => !string.IsNullOrEmpty(search)
                    ? f.financialYear.StartsWith(search)
                    || f.startDate.ToString().StartsWith(search)
                    || f.endDate.ToString().StartsWith(search)
                    || f.soNumber.ToString().StartsWith(search)
                    || f.poNumber.ToString().StartsWith(search)
                    || f.grnNumber.ToString().StartsWith(search) : true).ToList();
                financialYearView = Mapper.Map<List<MstFinancialYear>, List<VMFinancialYear>>(result);
            }

            return new ListResult<VMFinancialYear>
            {
                Data = financialYearView,
                TotalCount = repo.MstFinancialYears.Where(f => !string.IsNullOrEmpty(search)
                    ? f.financialYear.StartsWith(search)
                    || f.startDate.ToString().StartsWith(search)
                    || f.endDate.ToString().StartsWith(search)
                    || f.soNumber.ToString().StartsWith(search)
                    || f.poNumber.ToString().StartsWith(search)
                    || f.grnNumber.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFinancialYear getFinancialYearById(Int64 id)
        {
            var result = repo.MstFinancialYears.Where(s => s.id == id).FirstOrDefault();
            VMFinancialYear financialYearView = Mapper.Map<MstFinancialYear, VMFinancialYear>(result);
            return financialYearView;
        }

        public ResponseMessage postFinancial(VMFinancialYear financialYear)
        {
            MstFinancialYear financialYearToPost = Mapper.Map<VMFinancialYear, MstFinancialYear>(financialYear);
            financialYearToPost.createdOn = DateTime.Now;
            financialYearToPost.createdBy = _LoggedInuserId;

            repo.MstFinancialYears.Add(financialYearToPost);
            repo.SaveChanges();
            return new ResponseMessage(financialYearToPost.id,
                resourceManager.GetString("FinancialYearAdded"), ResponseType.Success);
        }

        public ResponseMessage putFinancialYear(VMFinancialYear financialYear)
        {
            var financialToPut = repo.MstFinancialYears.Where(s => s.id == financialYear.id).FirstOrDefault();
            financialToPut.startDate = financialYear.startDate;
            financialToPut.endDate = financialYear.endDate;
            financialToPut.financialYear = financialYear.financialYear;
            financialToPut.poNumber = financialYear.poNumber;
            financialToPut.soNumber = financialYear.soNumber;
            financialToPut.grnNumber = financialYear.grnNumber;
            financialToPut.ginNumber = financialYear.ginNumber;
            financialToPut.poInvoiceNumber = financialYear.poInvoiceNumber;
            financialToPut.soInvoiceNumber= financialYear.soInvoiceNumber;
            financialToPut.materialSelectionNumber = financialYear.materialSelectionNumber;
            financialToPut.materialQuotationNumber = financialYear.materialQuotationNumber;
            financialToPut.curtainSelectionNumber= financialYear.curtainSelectionNumber;
            financialToPut.curtainQuotationNumber = financialYear.curtainQuotationNumber;
            financialToPut.jobCardNumber = financialYear.jobCardNumber;
            
            financialToPut.updatedBy = _LoggedInuserId;
            financialToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(financialYear.id, resourceManager.GetString("FinancialYearUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteFinancialYear(Int64 id)
        {
            repo.MstFinancialYears.Remove(repo.MstFinancialYears.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("FinancialYearDeleted"), ResponseType.Success);
        }
    }
}