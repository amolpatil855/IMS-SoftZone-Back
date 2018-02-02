using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using IMSWebApi.Enums;
using IMSWebApi.Common;
using Microsoft.AspNet.Identity;
using System.Resources;
using System.Reflection;
using System.Transactions;

namespace IMSWebApi.Services
{
    public class RoleService
    {
        ResourceManager resourceManager = null;
        Int64 _LoggedInuserId;

        public RoleService()
        {
            _LoggedInuserId=Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        WebAPIdbEntities repo = new WebAPIdbEntities();
        public ListResult<VMRole> getRole(int pageSize, int page,string search)
        {
            List<VMRole> roleViews;
            if (pageSize > 0)
            {
                var result = repo.MstRoles.Where(p => !p.roleName.Equals("Administrator") &&(!string.IsNullOrEmpty(search)
                    ? p.roleName.StartsWith(search):true)).OrderBy(p=>p.id).Skip(page * pageSize).Take(pageSize).ToList();
                roleViews = Mapper.Map<List<MstRole>, List<VMRole>>(result);
            }
            else
            {
                var result = repo.MstRoles.Where(p => !p.roleName.Equals("Administrator")).ToList();
                roleViews = Mapper.Map<List<MstRole>, List<VMRole>>(result);
            }

            roleViews.ForEach(d => d.CFGRoleMenus = null);
            return new ListResult<VMRole>
            {
                Data = roleViews,
                TotalCount = repo.MstRoles.Where(p => !p.roleName.Equals("Administrator") && (!string.IsNullOrEmpty(search)
                    ? p.roleName.StartsWith(search) : true)).Count(),
                Page = page
            };
        }

        public VMRole getRoleById(Int64 id)
        {
            var result = repo.MstRoles.Where(p => p.id == id).FirstOrDefault();
            VMRole roleView = Mapper.Map<MstRole, VMRole>(result);
            roleView.CFGRoleMenus = null;
            return roleView;
        }

        public List<VMCFGRoleMenu> getRoleMenu(Int64 id)
        {
            var result = repo.CFGRoleMenus.Where(p => p.roleId == id).ToList();
            List<VMCFGRoleMenu> roleViews = Mapper.Map<List<CFGRoleMenu>, List<VMCFGRoleMenu>>(result);
            return roleViews;
        }

        public ResponseMessage insertRole(VMRole role)
        {
            using (var transaction = new TransactionScope())
            {  
                    MstRole roleObj = Mapper.Map<VMRole, MstRole>(role);
                    List<CFGRoleMenu> lstCfg = roleObj.CFGRoleMenus.ToList();
                    roleObj.createdOn = DateTime.Now;
                    roleObj.createdBy = _LoggedInuserId;
                    roleObj.CFGRoleMenus = null;
                    repo.MstRoles.Add(roleObj);
                    repo.SaveChanges();
                    foreach (var item in lstCfg)
                    {
                        item.MstRole = null;
                        item.roleId = roleObj.id;
                        item.createdOn = DateTime.Now;
                        item.createdBy = _LoggedInuserId;
                        repo.CFGRoleMenus.Add(item);
                    }
                    repo.SaveChanges();
                    transaction.Complete();
                    return new ResponseMessage(roleObj.id, resourceManager.GetString("RoleAdded"), ResponseType.Success);
            }       
        }


        public ResponseMessage updateRole(VMRole role)
        {
            using (var transaction = new TransactionScope())
            {
                List<CFGRoleMenu> lstCfg = Mapper.Map<List<VMCFGRoleMenu>, List<CFGRoleMenu>>(role.CFGRoleMenus);
                MstRole recordToUpdate = repo.MstRoles.FirstOrDefault(p => p.id == role.id);
                recordToUpdate.updatedOn = DateTime.Now;
                recordToUpdate.updatedBy = _LoggedInuserId;
                recordToUpdate.roleName = role.roleName;
                recordToUpdate.roleDescription = role.roleDescription;
                var savedCfgRoleMenu = repo.CFGRoleMenus.Where(p => p.roleId == role.id).ToList();
                foreach (var item in savedCfgRoleMenu)
                {
                    repo.CFGRoleMenus.Remove(item);
                }
                repo.SaveChanges();
                foreach (var item in lstCfg)
                {
                    item.MstRole = null;
                    item.roleId = role.id;
                    item.createdOn = DateTime.Now;
                    item.createdBy = _LoggedInuserId;
                    repo.CFGRoleMenus.Add(item);
                }
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(role.id, resourceManager.GetString("RoleUpdated"), ResponseType.Success);
            }
        }

        public ResponseMessage deleteRole(Int64 roleId)
        {
            using (var transaction = new TransactionScope())
            {
                MstRole recordToRemove = repo.MstRoles.FirstOrDefault(p => p.id == roleId);
                if (recordToRemove.MstUsers.Count() > 0)
                {
                    return new ResponseMessage(roleId, resourceManager.GetString("RoleCannotBeDeleted"), ResponseType.Error);
                }
                repo.MstRoles.Remove(recordToRemove);
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(roleId, resourceManager.GetString("RoleDeleted"), ResponseType.Success);
            }
        }

        public MstRole getCustomerRole()
        {
            return repo.MstRoles.Where(c => c.roleName == "Customer").FirstOrDefault();
        }

        public List<VMLookUpItem> getRoleLookUp()
        {
            return repo.MstRoles.Where(s=>!s.roleName.Equals("Customer"))
                .OrderBy(s => s.roleName)
                .Select(s => new VMLookUpItem { value = s.id, label = s.roleName }).ToList();
        }

    }
}