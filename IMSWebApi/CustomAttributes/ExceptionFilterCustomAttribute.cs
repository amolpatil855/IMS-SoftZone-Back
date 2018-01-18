using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace IMSWebApi.CustomAttributes
{
    public class ExceptionFilterCustomAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is System.Data.Entity.Infrastructure.DbUpdateException)
            {
                context.Response = new HttpResponseMessage();
                HttpError myCustomError = new HttpError("Duplicate record exist.Record cannot be saved");
                //HttpError myCustomError = new HttpError("My custom error message") { { "CustomErrorCode", 37 } };
                //context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
                throw new HttpResponseException(context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError));
            }
        }
    }
}