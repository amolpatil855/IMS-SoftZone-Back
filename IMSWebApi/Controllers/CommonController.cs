using IMSWebApi.Services;
using IMSWebApi.ServicesDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    [Authorize]
    public class CommonController : ApiController
    {
        private CollectionService _collectionService = null;
        private CustomerService _customerService = null;
        private FWRDesignService _designService = null;
        private FWRShadeService _shadeService = null;
        private HsnService _hsnService = null;
        private MatSizeService _matSizeService = null;
        private MatThicknessService _matThicknessService = null;
        private SupplierService _supplierService = null;
        private QualityService _qualityService = null;
        private CategoryService _categoryService = null;
        private FomDensityService _fomDensityService = null;
        private FomSuggestedMMService _fomSuggestedMMService = null;
        private CompanyLocationService _companyLocationService = null;
        private RoleService _roleService = null;

        public CommonController()
        {
            _collectionService = new CollectionService();
            _customerService = new CustomerService();
            _designService = new FWRDesignService();
            _shadeService = new FWRShadeService();
            _hsnService = new HsnService();
            _matSizeService = new MatSizeService();
            _matThicknessService = new MatThicknessService();
            _supplierService = new SupplierService();
            _qualityService = new QualityService();
            _categoryService = new CategoryService();
            _fomDensityService = new FomDensityService();
            _fomSuggestedMMService = new FomSuggestedMMService();
            _companyLocationService = new CompanyLocationService();
            _roleService = new RoleService();
        }

        [HttpGet]
        [Route("api/Common/GetCategoryLookup")]
        public IHttpActionResult GetCategoryLookup()
        {
            var result = _categoryService.getCategoryLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetRoleLookup")]
        public IHttpActionResult GetRoleLookup()
        {
            var result = _roleService.getRoleLookUp();
            return Ok(result);
        }


        [HttpGet]
        [Route("api/Common/GetFWRCategoryLookup")]
        public IHttpActionResult GetFWRCategoryLookup()
        {
            var result = _categoryService.getFWRCategoryLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetCollectionLookUpByCategoryId")]
        public IHttpActionResult GetCollectionLookUpByCategoryId(long categoryId)
        {
            var result = _collectionService.getCollectionLookUpByCategoryId(categoryId);
            return Ok(result);
        }
                
        [HttpGet]
        [Route("api/Common/GetMatCollectionLookUp")]
        public IHttpActionResult GetMatCollectionLookUp()
        {
            var result = _collectionService.getMatCollectionLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetFomCollectionLookUp")]
        public IHttpActionResult GetFomCollectionLookUp()
        {
            var result = _collectionService.getFomCollectionLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetCustomerLookUp")]
        public IHttpActionResult GetCustomerLookUp()
        {
            var result = _customerService.getCustomerLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetDesignLookupByQuality")]
        public IHttpActionResult GetDesignLookupByQuality(long qualityid)
        {
            var result = _designService.getDesignLookUpByQuality(qualityid);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetSerialNumberLookUpByDesign")]
        public IHttpActionResult GetSerialNumberLookUpByDesign(long designId)
        {
            var result = _shadeService.getSerialNumberLookUpByDesign(designId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetSerialNumberLookUpByCollection")]
        public IHttpActionResult GetSerialNumberLookUpByCollection(long collectionId)
        {
            var result = _shadeService.getSerialNumberLookUpByCollection(collectionId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetHsnLookUp")]
        public IHttpActionResult GetHsnLookUp()
        {
            var result = _hsnService.getHsnLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetMatSizeLookUp")]
        public IHttpActionResult GetMatSizeLookUp()
        {
            var result = _matSizeService.getMatSizeLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetMatThicknessLookUp")]
        public IHttpActionResult GetMatThicknessLookUp()
        {
            var result = _matThicknessService.getMatThicknessLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetSupplierLookUp")]
        public IHttpActionResult GetSupplierLookUp()
        {
            var result = _supplierService.getSupplierLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetQualityLookUpByCollection")]
        public IHttpActionResult GetQualityLookUpByCollection(long collectionId)
        {
            var result = _qualityService.getQualityLookUpByCollection(collectionId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetFomDensityLookUpByQuality")]
        public IHttpActionResult GetFomDensityLookUpByQuality(long qualityId)
        {
            var result = _fomDensityService.getFomDensityLookUpByQualityId(qualityId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetFomSuggestedMMLookUpByFomDensity")]
        public IHttpActionResult GetFomSuggestedMMLookUpByFomDensity(long fomDensityId)
        {
            var result = _fomSuggestedMMService.getFomSuggestedMMLookUpByFomDensity(fomDensityId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetCompanyLocationLookUp")]
        public IHttpActionResult GetCompanyLocationLookUp()
        {
            var result = _companyLocationService.getCompanyLocationLookUp();
            return Ok(result);
        }

    }
}
