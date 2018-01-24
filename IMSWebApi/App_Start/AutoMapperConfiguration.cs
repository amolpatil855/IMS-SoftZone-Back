﻿using System;
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
            Mapper.CreateMap<MstMenu, VMMenu>();

            Mapper.CreateMap<VMRole, MstRole>();
            Mapper.CreateMap<MstRole, VMRole>();
            
            Mapper.CreateMap<MstUser, VMUser>();
            Mapper.CreateMap<VMUser, MstUser>();
            
            Mapper.CreateMap<MstuserType, VMUserType>();
            
            Mapper.CreateMap<MstCompanyInfo, VMCompanyInfo>();
            Mapper.CreateMap<VMCompanyInfo, MstCompanyInfo>();
            
            Mapper.CreateMap<MstCustomer, VMCustomer>();
            Mapper.CreateMap<VMCustomer, MstCustomer>();
            
            Mapper.CreateMap<MstCustomerAddress, VMCustomerAddress>();
            Mapper.CreateMap<VMCustomerAddress, MstCustomerAddress>();
            
            Mapper.CreateMap<MstSupplier, VMSupplier>();
            Mapper.CreateMap<VMSupplier, MstSupplier>();
            
            Mapper.CreateMap<MstSupplierAddress, VMSupplierAddress>();
            Mapper.CreateMap<VMSupplierAddress, MstSupplierAddress>();

            Mapper.CreateMap<MstCollection, VMCollection>();
            Mapper.CreateMap<VMCollection, MstCollection>();

            Mapper.CreateMap<MstCategory, VMCategory>();
            Mapper.CreateMap<VMCategory, MstCategory>();

            Mapper.CreateMap<MstQuality, VMQuality>();
            Mapper.CreateMap<VMQuality, MstQuality>();

            Mapper.CreateMap<MstFWRDesign, VMFWRDesign>();
            Mapper.CreateMap<VMFWRDesign, MstFWRDesign>();

            Mapper.CreateMap<MstFWRShade, VMFWRShade>();
            Mapper.CreateMap<VMFWRShade, MstFWRShade>();

            Mapper.CreateMap<MstHsn, VMHsn>();
            Mapper.CreateMap<VMHsn, MstHsn>();

            Mapper.CreateMap<MstMatThickness, VMMatThickness>();
            Mapper.CreateMap<VMMatThickness, MstMatThickness>();

            Mapper.CreateMap<MstMatSize, VMMatSize>();
            Mapper.CreateMap<VMMatSize, MstMatSize>();

            Mapper.CreateMap<MstFomDensity, VMFomDensity>();
            Mapper.CreateMap<VMFomDensity, MstFomDensity>();
        }
    }
}