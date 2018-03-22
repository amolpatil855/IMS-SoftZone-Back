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
    public class ProductListController : ApiController
    {
        private ProductListService _productListService = null;

        public ProductListController()
        {
            _productListService = new ProductListService();
        }

        // GET api/ProductList   
        [ApiAuthorize(AccessLevel = "customerLogin")]
        [HttpGet]
        [Route("api/ProductList/GetFabricProducts")]
        public IHttpActionResult GetFabricProducts(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _productListService.getFabricProducts(pageSize, page, search);
            return Ok(result);
        }

        // GET api/ProductList   
        [ApiAuthorize(AccessLevel = "customerLogin")]
        [HttpGet]
        [Route("api/ProductList/GetRugProducts")]
        public IHttpActionResult GetRugProducts(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _productListService.getRugProducts(pageSize, page, search);
            return Ok(result);
        }

        // GET api/ProductList   
        [ApiAuthorize(AccessLevel = "customerLogin")]
        [HttpGet]
        [Route("api/ProductList/GetWallpaperProducts")]
        public IHttpActionResult GetWallpaperProducts(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _productListService.getWallpaperProducts(pageSize, page, search);
            return Ok(result);
        }

        // GET api/ProductList   
        [ApiAuthorize(AccessLevel = "customerLogin")]
        [HttpGet]
        [Route("api/ProductList/GetFoamProducts")]
        public IHttpActionResult GetFoamProducts(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _productListService.getFoamProducts(pageSize, page, search);
            return Ok(result);
        }

        // GET api/ProductList   
        [ApiAuthorize(AccessLevel = "customerLogin")]
        [HttpGet]
        [Route("api/ProductList/GetAccessoryProducts")]
        public IHttpActionResult GetAccessoryProducts(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _productListService.getAccessoryProducts(pageSize, page, search);
            return Ok(result);
        }

        // GET api/ProductList   
        [ApiAuthorize(AccessLevel = "customerLogin")]
        [HttpGet]
        [Route("api/ProductList/GetMattressProducts")]
        public IHttpActionResult GetMattressProducts(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _productListService.getMattressProducts(pageSize, page, search);
            return Ok(result);
        }
    }
}
