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
    public class TrnMaterialSelectionController : ApiController
    {
        private TrnMaterialSelectionService _trnMaterialSelectionService = null;

        public TrnMaterialSelectionController()
        {
            _trnMaterialSelectionService = new TrnMaterialSelectionService();
        }
    }
}
