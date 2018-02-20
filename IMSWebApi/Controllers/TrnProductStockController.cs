using IMSWebApi.CustomAttributes;
using IMSWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    [Authorize]
    public class TrnProductStockController : ApiController
    {
        private TrnProductStockService _trnProductStockService = null;

        public TrnProductStockController()
        {
            _trnProductStockService = new TrnProductStockService();
        }

        // GET api/TrnProductStock
        [ApiAuthorize(AccessLevel = "productstock")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnProductStockService.getTrnProductStock(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnProductStock
        [ApiAuthorize(AccessLevel = "productstock")]
        [HttpGet]
        [Route("api/TrnProductStock/GetFWRProductStock")]
        public IHttpActionResult GetFWRProductStock(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnProductStockService.getFWRProductStock(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnProductStock
        [ApiAuthorize(AccessLevel = "productstock")]
        [HttpGet]
        [Route("api/TrnProductStock/GetMatProductStock")]
        public IHttpActionResult GetMatProductStock(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnProductStockService.getMatProductStock(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnProductStock
        [ApiAuthorize(AccessLevel = "productstock")]
        [HttpGet]
        [Route("api/TrnProductStock/GetFomProductStock")]
        public IHttpActionResult GetFomProductStock(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnProductStockService.getFomProductStock(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnProductStock
        [ApiAuthorize(AccessLevel = "productstock")]
        [HttpGet]
        [Route("api/TrnProductStock/GetAccessoryProductStock")]
        public IHttpActionResult GetAccessoryProductStock(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnProductStockService.getAccessoryProductStock(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnProductStock
        //[ApiAuthorize(AccessLevel = "productstock")]
        [HttpGet]
        [Route("api/TrnProductStock/GetProductDetails")]
        public IHttpActionResult GetProductDetails(long categoryId, long? collectionId, long? parameterId, long? qualityId)
        {
            var result = _trnProductStockService.getProductStockAvailablity(categoryId, collectionId, parameterId, qualityId);
            return Ok(result);
        }
    }
}
