using AutoMapper;
using IMSWebApi.Common;
using IMSWebApi.Enums;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IMSWebApi.Services
{
    public class UserService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        SendEmail Email = new SendEmail();

        public VMUser getLoggedInUserDetails(string username)
        { 
            MstUser result = repo.MstUsers.Where(p => p.userName.Equals(username)).FirstOrDefault();
            VMUser userView = new VMUser();
            userView.id = result.id;
            userView.userName = result.userName;
            userView.email = result.email;
            userView.MstRole = Mapper.Map<MstRole, VMRole>(result.MstRole);
            if (userView.MstRole != null)
            {
                userView.MstRole.CFGRoleMenus = null;
            }
            return userView;
        }

        public List<VMUser> getUser()
        {
            var result = repo.MstUsers.ToList();
            List<VMUser> userViews = Mapper.Map<List<MstUser>, List<VMUser>>(result);
            userViews.ForEach(d => d.MstRole.CFGRoleMenus = null);
            return userViews;
        }

        public List<string> getUserPermission(string username)
        {
            List<string> permissions = new List<string>();
            var result = from u in repo.MstUsers
                         join cfg in repo.CFGRoleMenus on u.roleId equals cfg.roleId
                         join menu in repo.MstMenus on cfg.id equals menu.id
                         where u.userName.Equals(username)
                         select new
                    {
                        Menu = menu.menuName
                    };

            foreach (var item in result)
            {
                permissions.Add(item.Menu.Replace(" ", string.Empty).ToLower());
            }
            return permissions;
        }

        public List<VMUserType> getUserType()
        {
            var result = repo.MstuserTypes.ToList();
            List<VMUserType> userTypeViews = Mapper.Map<List<MstuserType>, List<VMUserType>>(result);
            return userTypeViews;
        }

        public VMUser getUserById(Int64 id)
        {
            var result = repo.MstUsers.Where(p => p.id == id).FirstOrDefault();
            VMUser userView = Mapper.Map<MstUser, VMUser>(result);
            if (userView.MstRole != null)
            {
                userView.MstRole.CFGRoleMenus = null;
            }
            return userView;
        }

        public ResponseMessage postUser(VMUser user)
        {
            MstUser userToPost = Mapper.Map<VMUser, MstUser>(user);
            userToPost.password = createRandomPassword(8);
            userToPost.createdOn = DateTime.Now;
            repo.MstUsers.Add(userToPost);
            repo.SaveChanges();
            sendEmail(userToPost.id, "RegisterUser");
            return new ResponseMessage(userToPost.id, "User Added Successfully", ResponseType.Success);
        }

        private static string createRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[passwordLength];
            Random rd = new Random();
            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }

        public ResponseMessage putUser(VMUser user)
        {
            var userToPut = repo.MstUsers.Where(p => p.id == user.id).FirstOrDefault();
            userToPut.userName = user.userName;
            userToPut.roleId = user.roleId;
            userToPut.userTypeId = user.userTypeId;
            userToPut.email = user.email;
            userToPut.phone = user.phone;
            userToPut.updatedOn = DateTime.Now;
            repo.SaveChanges();
            return new ResponseMessage(userToPut.id, "User Details Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteUser(Int64 id)
        {
            MstUser mstuser = repo.MstUsers.Find(id);
            if (mstuser != null)
            {
                repo.MstUsers.Remove(mstuser);
                repo.SaveChanges();
                return new ResponseMessage(id, "User Deleted Successfully", ResponseType.Success);
            }
            else
            {
                return new ResponseMessage(id, "User Does not exist", ResponseType.Error);
            }

        }

        public ResponseMessage changePassword(VMUser user)
        {  
                var userToPut = repo.MstUsers.Where(p => p.id == user.id && p.password == user.oldPassword).FirstOrDefault();
                if (userToPut != null)
                {
                    userToPut.password = user.password;
                    repo.SaveChanges();
                    return new ResponseMessage(userToPut.id, "Password Updated Successfully", ResponseType.Success);
                }
                else
                    return new ResponseMessage(user.id, "Old Password didn't Matched", ResponseType.Error);
        }

        public void sendEmail(Int64 id, string fileName)
        {
            var result = repo.MstUsers.Where(u => u.id == id).FirstOrDefault();
            Email.email(result, fileName);
        }

    }
}