using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Routing;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Security;
using System.Linq;
using System.Security.Claims;

namespace WebApi.CustomAttributes
{
    public class ApiAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private string _responseReason = "";
        public string AccessLevel { get; set; }
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            if (!string.IsNullOrEmpty(_responseReason))
                actionContext.Response.ReasonPhrase = _responseReason;
        }
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            List<string> permissionSet = new List<string>();
            var roles = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            if (roles.Contains("admin"))
            {
                permissionSet.Add("All");
            }


            if (!permissionSet.Contains(AccessLevel) && !string.IsNullOrWhiteSpace(AccessLevel))
            {
               // throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                actionContext.Response =new HttpResponseMessage(HttpStatusCode.Unauthorized);
                actionContext.Response.ReasonPhrase = "Permission denied";
                return false;
                
            }
            return base.IsAuthorized(actionContext);
        }

    }
}