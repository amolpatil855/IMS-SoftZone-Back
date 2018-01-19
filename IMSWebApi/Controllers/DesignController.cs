using IMSWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    public class DesignController : ApiController
    {
        private DesignService _designService = null;

        public DesignController()
        {
            _designService = new DesignService();
        }

    }
}
