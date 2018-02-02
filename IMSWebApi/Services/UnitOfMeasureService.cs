using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Services
{
    public class UnitOfMeasureService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public List<VMLookUpItem> getUnitOfMeasureLookUp()
        {
            return repo.MstUnitOfMeasures
                .OrderBy(s => s.uomCode)
                .Select(s => new VMLookUpItem { value = s.id, label = s.uomCode }).ToList();
        }
    }
}