using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Services
{
    public class DashboardService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public DashboardService()
        {  
        }

        public vwDasBoard getDashboardData()
        {
            return repo.vwDasBoards.FirstOrDefault();
        }

    }
}