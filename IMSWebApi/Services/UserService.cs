using AutoMapper;
using IMSWebApi.Common;
using IMSWebApi.Enums;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Resources;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace IMSWebApi.Services
{
    public class UserService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        SendEmail Email = new SendEmail();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        public UserService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public VMUser getLoggedInUserDetails(string username)
        {
            MstUser result = repo.MstUsers.Where(p => p.userName.Equals(username)).FirstOrDefault();
            VMUser userView = new VMUser();
            userView.id = result.id;
            userView.userName = result.userName;
            userView.email = result.email;
            userView.MstCompanyLocation = Mapper.Map<MstCompanyLocation, VMCompanyLocation>(result.MstCompanyLocation);
            userView.MstRole = Mapper.Map<MstRole, VMRole>(result.MstRole);
            if (userView.MstRole != null)
            {
                userView.MstRole.CFGRoleMenus = null;
            }
            return userView;
        }

        public ListResult<VMUser> getUsers(int pageSize, int page, string search)
        {
            List<VMUser> userViews;
            if (pageSize > 0)
            {
                var result = repo.MstUsers.Where(c => !c.MstRole.roleName.Equals("Administrator") && (!string.IsNullOrEmpty(search)
                                    ? c.userName.StartsWith(search) 
                                    || c.MstRole.roleName.StartsWith(search)
                                    || c.phone.StartsWith(search) 
                                    || c.email.StartsWith(search)  : true))
                                    .OrderBy(p => p.id).Skip(page * pageSize)
                                    .Take(pageSize).ToList();
                userViews = Mapper.Map<List<MstUser>, List<VMUser>>(result);
            }
            else
            {
                var result = repo.MstUsers.Where(c => !c.MstRole.roleName.Equals("Administrator") && (!string.IsNullOrEmpty(search)
                                            ? c.userName.StartsWith(search)
                                            || c.MstRole.roleName.StartsWith(search)
                                            || c.phone.StartsWith(search)
                                            || c.email.StartsWith(search) : true)).ToList();
                userViews = Mapper.Map<List<MstUser>, List<VMUser>>(result);
            }

            userViews.ForEach(d => d.MstRole.CFGRoleMenus = null);
            return new ListResult<VMUser>
            {
                Data = userViews,
                TotalCount = repo.MstUsers.Where(c => !c.MstRole.roleName.Equals("Administrator") && (!string.IsNullOrEmpty(search)
                                            ? c.userName.StartsWith(search)
                                            || c.MstRole.roleName.StartsWith(search)
                                            || c.phone.StartsWith(search)
                                            || c.email.StartsWith(search) : true)).Count(),
                Page = page
            };
        }

        public List<string> getUserPermission(string username)
        {
            List<string> permissions = new List<string>();
            var result = from u in repo.MstUsers
                         join cfg in repo.CFGRoleMenus on u.roleId equals cfg.roleId
                         join menu in repo.MstMenus on cfg.menuId equals menu.id
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

        public List<VMLookUpItem> getUserTypeLookup()
        {
            return repo.MstuserTypes.Where(s => !s.userTypeName.Equals("Customer"))
              .OrderBy(s => s.userTypeName)
              .Select(s => new VMLookUpItem { value = s.id, label = s.userTypeName }).ToList();
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
            var originalPassword = createRandomPassword(8);
            userToPost.password = encryption(originalPassword);
            userToPost.isActive = true;
            userToPost.createdOn = DateTime.Now;
            userToPost.createdBy = _LoggedInuserId;
            repo.MstUsers.Add(userToPost);
            repo.SaveChanges();
            sendEmail(userToPost.id, originalPassword, "RegisterUser",false);
            return new ResponseMessage(userToPost.id, resourceManager.GetString("UserAdded"), ResponseType.Success);
        }

        public string createRandomPassword(int passwordLength)
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

        public static string encryption(String password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypt;
            UTF8Encoding encode = new UTF8Encoding();
            //encrypt the given password string into Encrypted data  
            encrypt = md5.ComputeHash(encode.GetBytes(password));
            StringBuilder encryptdata = new StringBuilder();
            //Create a new string by using the encrypted data  
            for (int i = 0; i < encrypt.Length; i++)
            {
                encryptdata.Append(encrypt[i].ToString());
            }
            return encryptdata.ToString();
        }

        public ResponseMessage putUser(VMUser user)
        {
            var userToPut = repo.MstUsers.Where(p => p.id == user.id).FirstOrDefault();
            userToPut.userName = user.userName;
            userToPut.roleId = user.roleId;
            userToPut.userTypeId = user.userTypeId;
            userToPut.locationId = user.locationId;
            userToPut.email = user.email;
            userToPut.phone = user.phone;
            userToPut.updatedOn = DateTime.Now;
            userToPut.updatedBy = _LoggedInuserId;
            repo.SaveChanges();
            return new ResponseMessage(userToPut.id, resourceManager.GetString("UserUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteUser(Int64 id)
        {
            MstUser mstuser = repo.MstUsers.Find(id);
            if (mstuser != null)
            {
                repo.MstUsers.Remove(mstuser);
                repo.SaveChanges();
                return new ResponseMessage(id, resourceManager.GetString("UserDeleted"), ResponseType.Success);
            }
            else
            {
                return new ResponseMessage(id, resourceManager.GetString("UserNotExist"), ResponseType.Error);
            }

        }

        public ResponseMessage changePassword(VMUser user)
        {
            var hashedPassword = encryption(user.oldPassword);
            var userToPut = repo.MstUsers.Where(p => p.id == user.id && p.password == hashedPassword).FirstOrDefault();
            if (userToPut != null)
            {
                userToPut.password = encryption(user.password);
                repo.SaveChanges();
                return new ResponseMessage(userToPut.id, resourceManager.GetString("PasswordUpdated"), ResponseType.Success);
            }
            else
                return new ResponseMessage(user.id, resourceManager.GetString("OldPasswordMismatched"), ResponseType.Error);
        }

        public ResponseMessage forgetPassword(string emailId)
        {
            var user = repo.MstUsers.Where(u => u.email.Equals(emailId)).FirstOrDefault();
            if (user!=null && user.isActive)
            {
                var originalPassword = createRandomPassword(8);
                user.password = encryption(originalPassword);
                repo.SaveChanges();
                sendEmail(user.id, originalPassword, "ForgotPassword",true);
                return new ResponseMessage(user.id, resourceManager.GetString("PasswordReset"), ResponseType.Success);
            }
            else if (user != null && user.isActive == false)
            {
                return new ResponseMessage(user.id, resourceManager.GetString("PasswordResetForDeActiveUser"), ResponseType.Error);
            }
            else
            {
                return new ResponseMessage(0, resourceManager.GetString("PasswordResetFailed"), ResponseType.Error);
            }
        }

        public void sendEmail(Int64 id, string originalPassword, string fileName,bool isReset)
        {
            var result = repo.MstUsers.Where(u => u.id == id).FirstOrDefault();
            Email.email(result, originalPassword, fileName,isReset);
        }

        public MstuserType getCustomerUserType()
        {
            return repo.MstuserTypes.Where(c => c.userTypeName == "Customer").FirstOrDefault();
        }

        public ResponseMessage activateDeActivateUser(Int64 id,bool isActive)
        {
            MstUser user = repo.MstUsers.Where(u => u.id == id).FirstOrDefault();
            user.isActive = isActive;
            user.updatedBy = _LoggedInuserId;
            user.updatedOn = DateTime.Now;

            repo.SaveChanges();
            if (isActive)
            {
                return new ResponseMessage(user.id, resourceManager.GetString("UserActivate"), ResponseType.Success);
            }
            else
            {
                return new ResponseMessage(user.id, resourceManager.GetString("UserDeActivate"), ResponseType.Success);
            }
            
        }
    }
}