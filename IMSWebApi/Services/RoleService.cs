using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
namespace IMSWebApi.Services
{
    public class RoleService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        public List<VMRole> getRole()
        {
            var result = repo.MstRoles.ToList();
            List<VMRole> roleViews = Mapper.Map<List<MstRole>, List<VMRole>>(result);
            return roleViews;
        }

        public VMRole getRoleById(Int64 id)
        {
            var result = repo.MstRoles.Where(p=>p.id==id).FirstOrDefault();
            VMRole roleView = Mapper.Map<MstRole, VMRole>(result);
            return roleView;
        }

        public List<VMCFGRoleMenu> getRoleMenu(Int64 id)
        {
            var result= repo.CFGRoleMenus.Where(p => p.roleId == id).ToList();
            List<VMCFGRoleMenu> roleViews = Mapper.Map<List<CFGRoleMenu>, List<VMCFGRoleMenu>>(result);
            return roleViews;
        }
    }
}