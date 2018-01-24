using IMSWebApi.CustomAttributes;
using IMSWebApi.Services;
using IMSWebApi.ServicesDesign;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    [Authorize]
    public class FWRShadeController : ApiController
    {
         private FWRShadeService _shadeService = null;
         
         public FWRShadeController()
        {
            _shadeService = new FWRShadeService();
        }

         // GET api/FWRShade   
         [ApiAuthorize(AccessLevel = "shade")]
         [HttpGet]
         public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
         {
             var result = _shadeService.getShade(pageSize, page, search);
             return Ok(result);
         }

         // GET api/FWRShade/1
         [ApiAuthorize(AccessLevel = "shade")]
         [HttpGet]
         public IHttpActionResult Get(long id)
         {
             var result = _shadeService.getShadeById(id);
             return Ok(result);
         }

         // POST api/FWRShade
         [ApiAuthorize(AccessLevel = "shade")]
         [HttpPost]
         public IHttpActionResult PostShade(VMFWRShade shade)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }
             var result = _shadeService.postShade(shade);
             return Ok(result);
         }

         // PUT api/FWRShade/1
         [ApiAuthorize(AccessLevel = "shade")]
         [HttpPut]
         public IHttpActionResult PutShade(VMFWRShade shade)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }
             var result = _shadeService.putShade(shade);
             return Ok(result);
         }

         // DELETE api/FWRShade/1
         [ApiAuthorize(AccessLevel = "shade")]
         [HttpDelete]
         public IHttpActionResult DeleteShade(long id)
         {
             var result = _shadeService.deleteShade(id);
             return Ok(result);
         }
    }
}
