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
        private FomSizeService _fomSizeService = null;
        private UnitOfMeasureService _unitOfMeasureService = null;
        private UserService _userService = null;
        private AgentService _agentService = null;
        private CourierService _courierService = null;
        private AccessoryService _accessoryService = null;
        private TrnGoodReceiveNoteService _trnGoodReceiveNoteService = null;

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
            _fomSizeService = new FomSizeService();
            _unitOfMeasureService = new UnitOfMeasureService();
            _userService = new UserService();
            _agentService = new AgentService();
            _courierService = new CourierService();
            _accessoryService = new AccessoryService();
            _trnGoodReceiveNoteService = new TrnGoodReceiveNoteService();
        }

        [HttpGet]
        [Route("api/Common/GetCategoryLookup")]
        public IHttpActionResult GetCategoryLookup()
        {
            var result = _categoryService.getCategoryLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetCategoryWithoutAccessory")]
        public IHttpActionResult GetCategoryWithoutAccessory()
        {
            var result = _categoryService.getCategoryWithoutAccessory();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetCategoryLookupForSO")]
        public IHttpActionResult GetCategoryLookupForSO()
        {
            var result = _categoryService.getCategoryLookupForSO();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetRoleLookupByUserTypeId")]
        public IHttpActionResult GetRoleLookupByUserTypeId(long userTypeId)
        {
            var result = _roleService.getRoleLookupByUserTypeId(userTypeId);
            return Ok(result);
        }

        // Get the type of User
        [HttpGet]
        [Route("api/Common/GetUserTypeLookup")]
        public IHttpActionResult GetUserTypeLookup()
        {
            var result = _userService.getUserTypeLookup();
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
        [Route("api/Common/GetCollectionLookUpForSO")]
        public IHttpActionResult GetCollectionLookUpForSO(long categoryId)
        {
            var result = _collectionService.getCollectionLookUpForSO(categoryId);
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
        [Route("api/Common/GetCollectionForGRNByCategorynSupplierId")]
        public IHttpActionResult GetCollectionForGRNByCategorynSupplierId(long categoryId, long? supplierId)
        {
            var result = _collectionService.getCollectionForGRNByCategorynSupplierId(categoryId,supplierId);
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
        [Route("api/Common/GetSerialNumberLookUpForSO")]
        public IHttpActionResult GetSerialNumberLookUpForSO(long collectionId)
        {
            var result = _shadeService.getSerialNumberLookUpForSO(collectionId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetSerialNumberLookUpForGRN")]
        public IHttpActionResult GetSerialNumberLookUpForGRN(long collectionId)
        {
            var result = _shadeService.getSerialNumberLookUpForGRN(collectionId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetAccessoryLookUp")]
        public IHttpActionResult GetAccessoryLookUp()
        {
            var result = _accessoryService.getAccessoryLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetAccessoryLookUpForGRN")]
        public IHttpActionResult GetAccessoryLookUpForGRN()
        {
            var result = _accessoryService.getAccessoryLookUpForGRN();
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
        [Route("api/Common/GetMatSizeLookUpByCollection")]
        public IHttpActionResult GetMatSizeLookUp(long collectionId)
        {
            var result = _matSizeService.getMatSizeLookUpByCollectionId(collectionId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetMatSizeLookUpForGRN")]
        public IHttpActionResult GetMatSizeLookUpForGRN(long collectionId)
        {
            var result = _matSizeService.getMatSizeLookUpForGRN(collectionId);
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
        [Route("api/Common/GetMatThicknessLookUpForCustomMat")]
        public IHttpActionResult GetMatThicknessLookUpForCustomMat()
        {
            var result = _matThicknessService.getMatThicknessLookUpForCustomMat();
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
        [Route("api/Common/GetSupplierLookupForGRN")]
        public IHttpActionResult GetSupplierLookupForGRN()
        {
            var result = _trnGoodReceiveNoteService.getSupplierForGRN();
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

        [HttpGet]
        [Route("api/Common/GetCompanyLocationById")]
        public IHttpActionResult GetCompanyLocationById(long locationId)
        {
            var result = _companyLocationService.getCompanyLocationById(locationId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetFomSizeLookUpByCollection")]
        public IHttpActionResult GetFomSizeLookUpByCollection(long collectionId)
        {
            var result = _fomSizeService.getFomSizeLookUpByCollection(collectionId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetFomSizeLookUpForGRN")]
        public IHttpActionResult GetFomSizeLookUpForGRN(long collectionId)
        {
            var result = _fomSizeService.getFomSizeLookUpForGRN(collectionId);
            return Ok(result);
        }


        [HttpGet]
        [Route("api/Common/GetUnitOfMeasureLookup")]
        public IHttpActionResult GetUnitOfMeasureLookup()
        {
            var result = _unitOfMeasureService.getUnitOfMeasureLookUp();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetAgentLookup")]
        public IHttpActionResult GetAgentLookup()
        {
            var result = _agentService.getAgentLookup();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetCourierLookup")]
        public IHttpActionResult GetCourierLookup()
        {
            var result = _courierService.getCourierLookup();
            return Ok(result);
        }
    }
}
