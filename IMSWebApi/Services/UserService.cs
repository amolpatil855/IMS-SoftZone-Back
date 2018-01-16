using AutoMapper;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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



    }
}