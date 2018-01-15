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
       public bool RegisterUser(User userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.username
            };

           repo.Users.Add(userModel);
           repo.SaveChanges();
           return true;
        }

        public User FindUser(string userName, string password)
        {
            User user = repo.Users.FirstOrDefault(p => p.username == userName && p.password == password);
            return user;
        }

    }
}