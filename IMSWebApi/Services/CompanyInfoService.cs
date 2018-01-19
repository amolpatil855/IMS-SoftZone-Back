using AutoMapper;
using IMSWebApi.Enums;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Services
{
    public class CompanyInfoService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public List<VMCompanyInfo> getCompanyInfo()
        {
            var result = repo.MstCompanyInfoes.ToList();
            List<VMCompanyInfo> companyInfoViews = Mapper.Map<List<MstCompanyInfo>, List<VMCompanyInfo>>(result);
            return companyInfoViews;
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
            repo.MstCompanyInfoes.Add(companyInfoToPost);
            repo.SaveChanges();
            return new ResponseMessage(companyInfoToPost.id, "Company Information Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putCompanyInfo(VMCompanyInfo companyInfo)
        {
            var companyInfoToPut = repo.MstCompanyInfoes.Where(c => c.id == companyInfo.id).FirstOrDefault();
            companyInfoToPut.companyName = companyInfo.companyName;
            companyInfoToPut.email = companyInfo.email;
            companyInfoToPut.phone = companyInfo.phone;
            companyInfoToPut.gstin = companyInfo.gstin;
            companyInfoToPut.companyLogo = companyInfo.companyLogo;
            companyInfoToPut.updatedOn = DateTime.Now;
            repo.SaveChanges();
            return new ResponseMessage(companyInfoToPut.id, "Company Information Updated Successfully", ResponseType.Success);
        }


    }
}