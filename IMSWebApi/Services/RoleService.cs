using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using IMSWebApi.Enums;
namespace IMSWebApi.Services
{
    public class RoleService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        public List<VMRole> getRole()
        {
            var result = repo.MstRoles.Where(p => !p.roleName.Equals("Administrator")).ToList();
            List<VMRole> roleViews = Mapper.Map<List<MstRole>, List<VMRole>>(result);
            roleViews.ForEach(d => d.CFGRoleMenus = null);
            return roleViews;
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
            MstRole roleObj = Mapper.Map<VMRole, MstRole>(role);
            List<CFGRoleMenu> lstCfg = roleObj.CFGRoleMenus.ToList();
            roleObj.createdOn = DateTime.Now;
            roleObj.createdBy = 1;
            roleObj.CFGRoleMenus = null;
            repo.MstRoles.Add(roleObj);
            repo.SaveChanges();
            foreach (var item in lstCfg)
            {
                item.MstRole = null;
                item.roleId = roleObj.id;
                item.createdOn = DateTime.Now;
                item.createdBy = 1;
                repo.CFGRoleMenus.Add(item);
            }
            repo.SaveChanges();
            return new ResponseMessage(roleObj.id, "Role Added Successfully", ResponseType.Success);
        }


        public ResponseMessage updateRole(VMRole role)
        {

            List<CFGRoleMenu> lstCfg = Mapper.Map<List<VMCFGRoleMenu>, List<CFGRoleMenu>>(role.CFGRoleMenus);
            MstRole recordToUpdate = repo.MstRoles.FirstOrDefault(p => p.id == role.id);
            recordToUpdate.updatedOn = DateTime.Now;
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
                item.createdBy = 1;
                repo.CFGRoleMenus.Add(item);
            }
            repo.SaveChanges();
            return new ResponseMessage(role.id, "Role Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteRole(Int64 roleId)
        {
            MstRole recordToRemove = repo.MstRoles.FirstOrDefault(p => p.id == roleId);
            if (recordToRemove.MstUsers.Count() > 0)
            {
                return new ResponseMessage(roleId, "Role already assigned to other user. cannot be deleted",ResponseType.Error);
            }
            repo.MstRoles.Remove(recordToRemove);
            var savedCfgRoleMenu = repo.CFGRoleMenus.Where(p => p.roleId == recordToRemove.id).ToList();
            foreach (var item in savedCfgRoleMenu)
            {
                repo.CFGRoleMenus.Remove(item);
            }
            repo.SaveChanges();
            return new ResponseMessage(roleId, "Role Deleted Successfully", ResponseType.Success);
        }



    }
}