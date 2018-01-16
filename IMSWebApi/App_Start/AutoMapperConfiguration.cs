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
            Mapper.CreateMap<MstRole, VMRole>();
            Mapper.CreateMap<MstMenu, VMMenu>();
            Mapper.CreateMap<MstUser, VMUser>();
        }
    }
}