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
    public class CollectionController : ApiController
    {
        private CollectionService _collectionService = null;
        
        public CollectionController()
        {
            _collectionService = new CollectionService();
        }

        // GET api/Collection
        [ApiAuthorize(AccessLevel = "collection")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _collectionService.getCollection(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Collection/1
        [ApiAuthorize(AccessLevel = "collection")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _collectionService.getCollectionById(id);
            return Ok(result);
        }

        // POST api/Collection
        [ApiAuthorize(AccessLevel = "collection")]
        [HttpPost]
        public IHttpActionResult PostCollection(VMCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _collectionService.postCollection(collection);
            return Ok(result);
        }

        // PUT api/Collection
        [ApiAuthorize(AccessLevel = "collection")]
        [HttpPut]
        public IHttpActionResult PutCollection(VMCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _collectionService.putCollection(collection);
            return Ok(result);
        }

        // DELETE api/Collection/1
        [ApiAuthorize(AccessLevel = "collection")]
        [HttpDelete]
        public IHttpActionResult DeleteCollection(long id)
        {
            var result = _collectionService.deleteCollection(id);
            return Ok(result);
        }
    }
}
