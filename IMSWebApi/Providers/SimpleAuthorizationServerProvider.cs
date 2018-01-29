using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using IMSWebApi.Models;
using IMSWebApi.Services;

namespace IMSWebApi.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            using (WebAPIdbEntities _repo = new WebAPIdbEntities())
            {
                var passWord = UserService.encryption(context.Password);
                MstUser user = _repo.MstUsers.FirstOrDefault(p => p.userName.Equals(context.UserName) && p.password.Equals(passWord));

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.userName));
                identity.AddClaim(new Claim(ClaimTypes.Role, user.MstRole.roleName));
                context.Validated(identity);
            }

        }
    }
}