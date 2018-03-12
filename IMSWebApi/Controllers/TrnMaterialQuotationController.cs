using IMSWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    public class TrnMaterialQuotationController : ApiController
    {
        private TrnMaterialQuotationService _trnMaterialQuotationService = null;

        public TrnMaterialQuotationController()
        {
            _trnMaterialQuotationService = new TrnMaterialQuotationService();
        }
    }
}
