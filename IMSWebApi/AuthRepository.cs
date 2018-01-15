using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IMSWebApi.Models;

namespace IMSWebApi
{
    public class AuthRepository 
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
       public bool RegisterUser(MstUser userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.userName
            };

            repo.MstUsers.Add(userModel);
           repo.SaveChanges();
           return true;
        }

       public MstUser FindUser(string userName, string password)
        {
            MstUser user = repo.MstUsers.FirstOrDefault(p => p.userName == userName && p.password == password);
            return user;
        }

    }
}