using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;

namespace IMSWebApi.App_Start
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<CFGRoleMenu, VMCFGRoleMenu>();
            Mapper.CreateMap<VMCFGRoleMenu, CFGRoleMenu>();
            Mapper.CreateMap<VMMenu, MstMenu>();
            Mapper.CreateMap<VMRole, MstRole>();
            Mapper.CreateMap<MstRole, VMRole>();
            
            Mapper.CreateMap<VMRole, MstRole>();
            Mapper.CreateMap<MstMenu, VMMenu>();
            
            Mapper.CreateMap<MstUser, VMUser>();
            Mapper.CreateMap<VMUser, MstUser>();
            
            Mapper.CreateMap<MstuserType, VMUserType>();
            
            Mapper.CreateMap<MstCompanyInfo, VMCompanyInfo>();
            Mapper.CreateMap<VMCompanyInfo, MstCompanyInfo>();
            
            Mapper.CreateMap<MstCustomer, VMCustomer>();
            Mapper.CreateMap<VMCustomer, MstCustomer>();
            
            Mapper.CreateMap<MstCustomerAddressDetail, VMCustomerAddressDetail>();
            Mapper.CreateMap<VMCustomerAddressDetail, MstCustomerAddressDetail>();
            
            Mapper.CreateMap<MstSupplier, VMSupplier>();
            Mapper.CreateMap<VMSupplier, MstSupplier>();
            
            Mapper.CreateMap<MstSupplierAddressDetail, VMSupplierAddressDetail>();
            Mapper.CreateMap<VMSupplierAddressDetail, MstSupplierAddressDetail>();

            Mapper.CreateMap<MstCollection, VMCollection>();
            Mapper.CreateMap<VMCollection, MstCollection>();

            Mapper.CreateMap<MstCategory, VMCategory>();
            Mapper.CreateMap<VMCategory, MstCategory>();
        }
    }
}