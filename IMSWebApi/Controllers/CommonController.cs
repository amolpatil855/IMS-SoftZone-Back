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
        private TrnMaterialQuotationService _trnMaterialQuotationService = null;
        private PatternService _patternService = null;
        private TrnCurtainSelectionService _trnCurtainSelectionService = null;
        private TrnCurtainQuotationService _trnCurtainQuotationService = null;
        private TailorService _tailorService = null;

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
            _trnMaterialQuotationService = new TrnMaterialQuotationService();
            _patternService = new PatternService();
            _trnCurtainSelectionService = new TrnCurtainSelectionService();
            _trnCurtainQuotationService = new TrnCurtainQuotationService();
            _tailorService = new TailorService();
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
        [Route("api/Common/GetCustomerLookUpWithoutWholesaleCustomer")]
        public IHttpActionResult GetCustomerLookUpWithoutWholesaleCustomer()
        {
            var result = _customerService.getCustomerLookUpWithoutWholesaleCustomer();
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
        [Route("api/Common/GetAccessoryLookUpBySupplierId")]
        public IHttpActionResult getAccessoryLookUpBySupplierId(long supplierId)
        {
            var result = _accessoryService.getAccessoryLookUpBySupplierId(supplierId);
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
        [Route("api/Common/GetMatSizeLookUpByMatThicknessId")]
        public IHttpActionResult GetMatSizeLookUpByMatThicknessId(long matThicknessId)
        {
            var result = _matSizeService.getMatSizeLookUpByMatThicknessId(matThicknessId);
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
        [Route("api/Common/GetQualityLookUpForSO")]
        public IHttpActionResult GetQualityLookUpForSO(long collectionId)
        {
            var result = _qualityService.getQualityLookUpForSO(collectionId);
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
        [Route("api/Common/GetFomSizeLookUpByFomSuggestedMMId")]
        public IHttpActionResult GetFomSizeLookUpByFomSuggestedMMId(long fomSuggestedMMId)
        {
            var result = _fomSizeService.getFomSizeLookUpByFomSuggestedMM(fomSuggestedMMId);
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

        [HttpGet]
        [Route("api/Common/GetCategoryLookupBySelectionType")]
        public IHttpActionResult GetCategoryLookupBySelectionType(string selectionType)
        {
            var result = _categoryService.getCategoryLookUpBySelectionType(selectionType);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetMaterialQuotationLookup")]
        public IHttpActionResult GetMaterialQuotationLookup()
        {
            var result = _trnMaterialQuotationService.getMaterialQuotationLookup();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetCustomerLookupByMaterialQuotationId")]
        public IHttpActionResult GetCustomerLookupByMaterialQuotationId(long materialQuotationId)
        {
            var result = _trnMaterialQuotationService.getCustomerLookupByMaterialQuotationId(materialQuotationId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetPatternLookup")]
        public IHttpActionResult GetPatternLookup()
        {
            var result = _patternService.getPatternLookup();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetAllPatterns")]
        public IHttpActionResult GetAllPatterns()
        {
            var result = _patternService.getAllPatterns();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetSerialNumberForCS")]
        public IHttpActionResult GetSerialNumberForCS(long collectionId)
        {
            var result = _trnCurtainSelectionService.getSerialNumberForCS(collectionId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetAccessoryItemCodeForCS")]
        public IHttpActionResult GetAccessoryItemCodeForCS()
        {
            var result = _trnCurtainSelectionService.getAccessoryItemCodeForCS();
            return Ok(result);
        }

        /*
         Added by Dakshineshwar Swain
         Date :- 10th April 2018
         */ 
        [HttpGet]
        [Route("api/Common/GetPatternDetailsForTailor")]
        public IHttpActionResult GetPatternDetailsForTailor()
        {
            var result = _patternService.getPatternDetailsForTailor();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetRodAccessoryItemCodeForCQ")]
        public IHttpActionResult GetRodAccessoryItemCodeForCQ()
        {
            var result = _trnCurtainQuotationService.getRodAccessoryForCQ();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetRodAccessoriesItemCodeForCQ")]
        public IHttpActionResult GetRodAccessoriesItemCodeForCQ()
        {
            var result = _trnCurtainQuotationService.getRodAccessoriesForCQ();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetTrackAccessoryItemCodeForCQ")]
        public IHttpActionResult GetTrackAccessoryItemCodeForCQ()
        {
            var result = _trnCurtainQuotationService.getTrackAccessoryForCQ();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetCurtainQuotationLookup")]
        public IHttpActionResult GetCurtainQuotationLookup()
        {
            var result = _trnCurtainQuotationService.getCurtainQuotationLookup();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetRemoteAccessoryItemCodeForCQ")]
        public IHttpActionResult GetRemoteAccessoryItemCodeForCQ()
        {
            var result = _trnCurtainQuotationService.getRemoteAccessoryForCQ();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetMotorAccessoryItemCodeForCQ")]
        public IHttpActionResult GetMotorAccessoryItemCodeForCQ()
        {
            var result = _trnCurtainQuotationService.getMotorAccessoryForCQ();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetAllTailors")]
        public IHttpActionResult GetAllTailors()
        {
            var result = _tailorService.getAlltailors();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Common/GetTailorLookup")]
        public IHttpActionResult GetTailorLookup()
        {
            var result = _tailorService.getTailorLookup();
            return Ok(result);
        }
    }
}
