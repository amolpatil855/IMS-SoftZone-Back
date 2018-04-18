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
using AutoMapper;
using System.Transactions;
using IMSWebApi.Enums;

namespace IMSWebApi.Services
{
    public class TailorService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        
        public TailorService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMTailorList> getTailors(int pageSize, int page, string search)
        {
            List<VMTailorList> tailorListingView;
            tailorListingView = repo.MstTailors.Where(t => !string.IsNullOrEmpty(search)
                    ? t.name.ToString().StartsWith(search)
                    || t.phone.StartsWith(search)
                    || t.email.StartsWith(search) 
                    || t.city.StartsWith(search) : true)
                    .Select(c => new VMTailorList
                    {
                        id = c.id,
                        name = c.name,
                        phone = c.phone,
                        email = c.email,
                        city = c.city
                    })
                    .OrderByDescending(o => o.id).Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMTailorList>
            {
                Data = tailorListingView,
                TotalCount = repo.MstTailors.Where(t => !string.IsNullOrEmpty(search)
                    ? t.name.ToString().StartsWith(search)
                    || t.phone.StartsWith(search)
                    || t.email.StartsWith(search)
                    || t.city.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTailor getTailorById(Int64 id)
        {
            var result = repo.MstTailors.Where(t => t.id == id).FirstOrDefault();
            VMTailor tailorViews = Mapper.Map<MstTailor, VMTailor>(result);
            tailorViews.MstTailorPatternChargeDetails.ForEach(tp => tp.MstTailor = null);
            return tailorViews;
        }

        public List<VMTailor> getAlltailors()
        {
            var tailor = repo.MstTailors.ToList();
            List<VMTailor> VMtailor = Mapper.Map<List<MstTailor>, List<VMTailor>>(tailor);
            VMtailor.ForEach(t => t.MstTailorPatternChargeDetails.ForEach(tpDetails => tpDetails.MstTailor = null));
            return VMtailor;
        }

        public List<VMLookUpItem> getTailorLookup()
        {
            return repo.MstTailors.Select(t => new VMLookUpItem
                                            {
                                                label = t.name,
                                                value = t.id
                                            }).ToList();
        }

        public ResponseMessage postTailor(VMTailor tailor)
        {
            using (var transaction = new TransactionScope())
            {   
                MstTailor tailorToPost = Mapper.Map<VMTailor, MstTailor>(tailor);
                List<MstTailorPatternChargeDetail> tailorPatternChargeDetails = tailorToPost.MstTailorPatternChargeDetails.ToList();
                foreach (var tpDetails in tailorPatternChargeDetails)
                {
                    tpDetails.createdOn = DateTime.Now;
                    tpDetails.createdBy = _LoggedInuserId;
                }
                tailorToPost.createdOn = DateTime.Now;
                tailorToPost.createdBy = _LoggedInuserId;
                repo.MstTailors.Add(tailorToPost);
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(tailorToPost.id, resourceManager.GetString("TailorAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putTailor(VMTailor tailor)
        {
            using (var transaction = new TransactionScope())
            {
                MstTailor tailorToPut = repo.MstTailors.Where(c => c.id == tailor.id).FirstOrDefault();
                tailorToPut.name = tailor.name;
                tailorToPut.email = tailor.email;
                tailorToPut.phone = tailor.phone;
                tailorToPut.alternatePhone1 = tailor.alternatePhone1;
                tailorToPut.addressLine1 = tailor.addressLine1;
                tailorToPut.addressLine2 = tailor.addressLine2;
                tailorToPut.city = tailor.city;
                tailorToPut.state = tailor.state;
                tailorToPut.country = tailor.country;
                tailorToPut.pin = tailor.pin;

                tailorToPut.updatedOn = DateTime.Now;
                tailorToPut.updatedBy = _LoggedInuserId;

                updateTailorPatternCharge(tailor);

                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(tailor.id, resourceManager.GetString("TailorUpdated"), ResponseType.Success);
            }
        }

        public void updateTailorPatternCharge(VMTailor tailor)
        {
            MstTailor tailorToPut = repo.MstTailors.Where(c => c.id == tailor.id).FirstOrDefault();

            List<MstTailorPatternChargeDetail> patternToRemove = new List<MstTailorPatternChargeDetail>();

            foreach (var tpDetail in tailorToPut.MstTailorPatternChargeDetails)
            {
                if (tailor.MstTailorPatternChargeDetails.Any(y => y.id == tpDetail.id))
                {
                    continue;
                }
                else
                {
                    patternToRemove.Add(tpDetail);
                }
            }

            repo.MstTailorPatternChargeDetails.RemoveRange(patternToRemove);
            repo.SaveChanges();

            tailor.MstTailorPatternChargeDetails.ForEach(x =>
            {
                if (tailorToPut.MstTailorPatternChargeDetails.Any(y => y.id == x.id))
                {
                    var patternChargeDetailToPut = repo.MstTailorPatternChargeDetails.Where(p => p.id == x.id).FirstOrDefault();

                    patternChargeDetailToPut.patternId = x.patternId;
                    patternChargeDetailToPut.charge = x.charge;
                    patternChargeDetailToPut.updatedOn = DateTime.Now;
                    patternChargeDetailToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
                else
                {
                    MstTailorPatternChargeDetail patternChargeDetails = Mapper.Map<VMTailorPatternChargeDetail, MstTailorPatternChargeDetail>(x);
                    patternChargeDetails.tailorId = tailor.id;
                    patternChargeDetails.createdBy = _LoggedInuserId;
                    patternChargeDetails.createdOn = DateTime.Now;
                    repo.MstTailorPatternChargeDetails.Add(patternChargeDetails);
                    repo.SaveChanges();
                }
            });
        }

        public ResponseMessage deleteTailor(Int64 id)
        {
            using (var transaction = new TransactionScope())
            {
                MstTailor tailorToDelete = repo.MstTailors.Where(t => t.id == id).FirstOrDefault();
                //if (repo.TrnSaleOrders.Where(so => so.customerId == id).Count() > 0)
                //{
                //    transaction.Complete();
                //    return new ResponseMessage(id, resourceManager.GetString("CustomerNotDeleted"), ResponseType.Error);
                //}
                //repo.MstTailorPatternChargeDetails.RemoveRange(repo.MstTailorPatternChargeDetails.Where(s => s.tailorId == id).ToList());
                repo.MstTailors.Remove(repo.MstTailors.Where(t => t.id == id).FirstOrDefault());
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("TailorDeleted"), ResponseType.Success);
            }
        }
    }
}