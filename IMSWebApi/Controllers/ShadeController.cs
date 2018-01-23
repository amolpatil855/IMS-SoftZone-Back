using IMSWebApi.CustomAttributes;
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
    public class ShadeController : ApiController
    {
         private ShadeService _shadeService = null;

         public ShadeController()
        {
            _shadeService = new ShadeService();
        }

         // GET api/Shade   
         [ApiAuthorize(AccessLevel = "shade")]
         [HttpGet]
         public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
         {
             var result = _shadeService.getShade(pageSize, page, search);
             return Ok(result);
         }

         // GET api/Shade/1
         [ApiAuthorize(AccessLevel = "shade")]
         [HttpGet]
         public IHttpActionResult Get(long id)
         {
             var result = _shadeService.getShadeById(id);
             return Ok(result);
         }

         [HttpGet]
         [Route("api/Shade/GetSerialNumberLookUpByDesign")]
         public IHttpActionResult GetSerialNumberLookUpByDesign(long designId)
         {
             var result = _shadeService.getSerialNumberLookUpByDesign(designId);
             return Ok(result);
         }

         [HttpGet]
         [Route("api/Shade/GetSerialNumberLookUpByCollection")]
         public IHttpActionResult GetSerialNumberLookUpByCollection(long collectionId)
         {
             var result = _shadeService.getSerialNumberLookUpByCollection(collectionId);
             return Ok(result);
         }

         // POST api/Shade
         [ApiAuthorize(AccessLevel = "shade")]
         [HttpPost]
         public IHttpActionResult PostShade(VMShade shade)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }
             var result = _shadeService.postShade(shade);
             return Ok(result);
         }

         // PUT api/Shade/1
         [ApiAuthorize(AccessLevel = "shade")]
         [HttpPut]
         public IHttpActionResult PutShade(VMShade shade)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }
             var result = _shadeService.putShade(shade);
             return Ok(result);
         }

         // DELETE api/Shade/1
         [ApiAuthorize(AccessLevel = "shade")]
         [HttpDelete]
         public IHttpActionResult DeleteShade(long id)
         {
             var result = _shadeService.deleteShade(id);
             return Ok(result);
         }
    }
}
