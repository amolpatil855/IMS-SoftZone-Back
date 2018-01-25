using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Services
{
    public class CompanyLocationService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public List<VMLookUpItem> getCompanyLocationLookUp()
        {
            return repo.MstCompanyLocations
                .OrderBy(s => s.locationCode)
                .Select(s => new VMLookUpItem { value = s.id, label = s.locationCode }).ToList();
        }
    }
}