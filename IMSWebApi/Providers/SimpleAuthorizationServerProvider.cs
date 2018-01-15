using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using IMSWebApi.Models;

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
                User user = _repo.Users.FirstOrDefault(p => p.username.Equals(context.UserName) && p.password.Equals(context.Password));

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            if (context.UserName.Equals("amol"))
                identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
            else
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            context.Validated(identity);

        }
    }
}