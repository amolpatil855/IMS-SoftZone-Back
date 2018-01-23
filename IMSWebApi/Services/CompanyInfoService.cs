using AutoMapper;
using IMSWebApi.Enums;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace IMSWebApi.Services
{
    public class CompanyInfoService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
         Int64 _LoggedInuserId;
         public CompanyInfoService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public VMCompanyInfo getCompanyInfo()
        {
            var result = repo.MstCompanyInfoes.FirstOrDefault();
            VMCompanyInfo companyInfoView = Mapper.Map<MstCompanyInfo, VMCompanyInfo>(result);
            return companyInfoView;
        }

        public VMCompanyInfo getCompanyInfoById(Int64 id)
        {
            var result = repo.MstCompanyInfoes.Where(c => c.id == id).FirstOrDefault();
            VMCompanyInfo companyInfoView = Mapper.Map<MstCompanyInfo, VMCompanyInfo>(result);
            return companyInfoView;
        }

        public ResponseMessage postCompanyInfo(VMCompanyInfo companyInfo)
        {
            MstCompanyInfo companyInfoToPost= Mapper.Map<VMCompanyInfo, MstCompanyInfo>(companyInfo);
            companyInfoToPost.createdOn = DateTime.Now;
            companyInfoToPost.createdBy = _LoggedInuserId;
            repo.MstCompanyInfoes.Add(companyInfoToPost);
            repo.SaveChanges();
            return new ResponseMessage(companyInfoToPost.id, "Company Information Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putCompanyInfo(VMCompanyInfo companyInfo)
        {
            var companyInfoToPut = repo.MstCompanyInfoes.Where(c => c.id == companyInfo.id).FirstOrDefault();
            companyInfoToPut = Mapper.Map<VMCompanyInfo, MstCompanyInfo>(companyInfo, companyInfoToPut);
            companyInfoToPut.updatedOn = DateTime.Now;
            companyInfoToPut.updatedBy = _LoggedInuserId;
            repo.SaveChanges();
            return new ResponseMessage(companyInfoToPut.id, "Company Information Updated Successfully", ResponseType.Success);
        }


    }
}