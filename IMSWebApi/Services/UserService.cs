using AutoMapper;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace IMSWebApi.Services
{
    public class UserService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public List<VMUser> getUser()
        {
            var result = repo.MstUsers.ToList();
            List<VMUser> userViews = Mapper.Map<List<MstUser>, List<VMUser>>(result);
            return userViews;
        }

        public VMUser getUserById(Int64 id)
        {
            var result = repo.MstUsers.Where(p => p.id == id).FirstOrDefault();
            VMUser userView = Mapper.Map<MstUser, VMUser>(result);
            return userView;
        }

        public long postUser(VMUser user)
        {
           MstUser userToPost = Mapper.Map<VMUser,MstUser>(user);

            //MstUser userToPost = new MstUser();
            //userToPost.userName = user.userName;
            //userToPost.email = user.email;
            //userToPost.phone = user.phone != string.Empty ? user.phone : null;
            //userToPost.isActive = user.isActive;
            //userToPost.createdBy = user.createdBy;
            //userToPost.roleId = user.roleId;
            //userToPost.password = "espl@123";
            //userToPost.userTypeId = 1;
            userToPost.createdOn = DateTime.Now;
           repo.MstUsers.Add(userToPost);
            
            repo.SaveChanges();

            return userToPost.id;

        }

    }
}