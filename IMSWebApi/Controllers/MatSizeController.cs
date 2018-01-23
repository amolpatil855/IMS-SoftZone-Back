using IMSWebApi.CustomAttributes;
using IMSWebApi.Services;
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
    public class MatSizeController : ApiController
    {
        private MatSizeService _matSizeService = null;
        private CategoryService _categoryService = null;

        public MatSizeController()
        {
            _matSizeService = new MatSizeService();
            _categoryService = new CategoryService();
        }

        // GET api/MatSize   
        [ApiAuthorize(AccessLevel = "mattresssize")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _matSizeService.getMatSize(pageSize, page, search);
            return Ok(result);
        }

        // GET api/MatSize/1
        [ApiAuthorize(AccessLevel = "mattresssize")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _matSizeService.getMatSizeById(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/MatSize/GetMatSizeLookUp")]
        public IHttpActionResult GetMatSizeLookUp()
        {
            var result = _matSizeService.getMatSizeLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/MatSize/GetCategoryLookup")]
        public IHttpActionResult GetCategoryLookup()
        {
            var result = _categoryService.getCategoryLookUp();
            return Ok(result);
        }

        // POST api/MatSize
        [ApiAuthorize(AccessLevel = "mattresssize")]
        [HttpPost]
        public IHttpActionResult PostMatSize(VMMatSize matSize)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _matSizeService.postMatSize(matSize);
            return Ok(result);
        }

        // PUT api/MatSize
        [ApiAuthorize(AccessLevel = "mattresssize")]
        [HttpPut]
        public IHttpActionResult PutMatSize(VMMatSize matSize)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _matSizeService.putMatSize(matSize);
            return Ok(result);
        }

        // DELETE api/MatSize/1
        [ApiAuthorize(AccessLevel = "mattresssize")]
        [HttpDelete]
        public IHttpActionResult DeleteMatSize(long id)
        {
            var result = _matSizeService.deleteMatSize(id);
            return Ok(result);
        }
    }
}
