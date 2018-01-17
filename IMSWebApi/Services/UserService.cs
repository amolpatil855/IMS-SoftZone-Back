using AutoMapper;
using IMSWebApi.Common;
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

        public long postUser(VMUser user)
        {
            MstUser userToPost = Mapper.Map<VMUser, MstUser>(user);
            userToPost.password = createRandomPassword(8);
            userToPost.createdOn = DateTime.Now;
            repo.MstUsers.Add(userToPost);
            repo.SaveChanges();
            sendEmail(userToPost.id, "RegisterUser");
            return userToPost.id;
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

        public long putUser(VMUser user)
        {
            if (user != null)
            {
                var userToPut = repo.MstUsers.Where(p => p.id == user.id).FirstOrDefault();
                if (userToPut != null)
                {
                    userToPut.userName = user.userName;
                    userToPut.roleId = user.roleId;
                    userToPut.userTypeId = user.userTypeId;
                    userToPut.email = user.email;
                    userToPut.phone = user.phone;
                    userToPut.updatedOn = DateTime.Now;
                    repo.SaveChanges();
                    return userToPut.id;
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        public bool deleteUser(Int64 id)
        {
            MstUser mstuser = repo.MstUsers.Find(id);
            if (mstuser != null)
            {
                repo.MstUsers.Remove(mstuser);
                repo.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public long changePassword(VMUser user)
        {
            if (user.id != null)
            {
                var userToPut = repo.MstUsers.Where(p => p.id == user.id && p.password == user.oldPassword).FirstOrDefault();
                if (userToPut != null)
                {
                    userToPut.password = user.password;
                    repo.SaveChanges();
                    return userToPut.id;
                }
                else
                    return 0;
            }
            else
                return 0;

        }

        public void sendEmail(Int64 id, string fileName)
        {
            var result = repo.MstUsers.Where(u => u.id == id).FirstOrDefault();
            Email.email(result, fileName);
        }

    }
}