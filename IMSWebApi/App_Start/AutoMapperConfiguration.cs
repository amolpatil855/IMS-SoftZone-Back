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
            Mapper.CreateMap<MstSupplier, VMSupplier>()
                .ForMember(dest=>dest.SupplierAddressDetails, opt=>opt.MapFrom(src=>src.MstSupplierAddressDetails));
            Mapper.CreateMap<MstSupplierAddressDetail, VMSupplierAddressDetail>();
            Mapper.CreateMap<VMSupplier, MstSupplier>()
                .ForMember(dest=>dest.MstSupplierAddressDetails, opt=>opt.MapFrom(src=>src.SupplierAddressDetails));
            Mapper.CreateMap<VMSupplierAddressDetail, MstSupplierAddressDetail>();
        }
    }
}