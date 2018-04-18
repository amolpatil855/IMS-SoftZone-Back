using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using System.Globalization;
using IMSWebApi.Enums;

namespace IMSWebApi.Services
{
    public class LabourJobService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;

        public LabourJobService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMvwLabourJob> getLabourJobs(int pageSize, int page, string isLabourChargePaid, Int64? tailorId, DateTime? startDate, DateTime? endDate)
        {
            List<VMvwLabourJob> labourJobView;
            //DateTime startDateValue;
            //bool isStartDate = DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDateValue);
            //DateTime endDateValue;
            //bool isEndDate = DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDateValue);
            var result = repo.vwLabourJobs.Where(wo => (!string.IsNullOrEmpty(isLabourChargePaid) ? (isLabourChargePaid.ToLower().Equals("yes") ? wo.isLabourChargesPaid : !(wo.isLabourChargesPaid)) : !(wo.isLabourChargesPaid))
                                                     && (tailorId != null ? wo.tailorId == tailorId : true)
                                                     && ((startDate != null && endDate != null) ? ((wo.workOrderDate <= startDate && endDate >= wo.workOrderDate) ? true : false) : true))
                    .Select(wo => new VMvwLabourJob
                    {  
                        workOrderId = wo.workOrderId,
                        workOrderDate = wo.workOrderDate,
                        workOrderNumber = wo.workOrderNumber,
                        curtainQuotationNumber = wo.curtainQuotationNumber,
                        tailorId = wo.tailorId,
                        tailorName = wo.tailorName,
                        labourcharges = wo.labourcharges,
                        isLabourChargesPaid = wo.isLabourChargesPaid
                    })
                    .OrderByDescending(p => p.workOrderId).Skip(page * pageSize).Take(pageSize).ToList();
            labourJobView = result;

            return new ListResult<VMvwLabourJob>
            {
                Data = labourJobView,
                //TotalCount = repo.vwLabourJobs.Where(wo => (!wo.isLabourChargesPaid && wo.tailorId != null)
                //                                     && (!string.IsNullOrEmpty(isPaid) ? (isPaid.ToLower().Equals("yes") ? wo.isLabourChargesPaid : !(wo.isLabourChargesPaid)) : true)
                //                                     && (tailorId != null ? wo.tailorId == tailorId : true)
                //                                     && ((isStartDate && isEndDate) ? ((wo.workOrderDate <= startDateValue && endDateValue >= wo.workOrderDate) ? true : false) : true)).Count(),
                TotalCount = repo.vwLabourJobs.Where(wo => (!string.IsNullOrEmpty(isLabourChargePaid) ? (isLabourChargePaid.ToLower().Equals("yes") ? wo.isLabourChargesPaid : !(wo.isLabourChargesPaid)) : !(wo.isLabourChargesPaid))
                                                     && (tailorId != null ? wo.tailorId == tailorId : true)
                                                     && ((startDate != null && endDate != null) ? ((wo.workOrderDate <= startDate && endDate >= wo.workOrderDate) ? true : false) : true)).Count(),
                Page = page
            };
        }

        public ResponseMessage paidLabourCharge(Int64 id)
        {
            TrnWorkOrder workOrder = repo.TrnWorkOrders.Where(u => u.id == id).FirstOrDefault();
            workOrder.isLabourChargesPaid = true;
            workOrder.updatedBy = _LoggedInuserId;
            workOrder.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(workOrder.id, resourceManager.GetString("LabourChargePaid"), ResponseType.Success);
        }
    }
}