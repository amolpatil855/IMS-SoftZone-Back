using AutoMapper;
using IMSWebApi.Enums;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;

namespace IMSWebApi.Services
{
    public class CustomerService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
         Int64 _LoggedInuserId;
         public CustomerService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

         public ListResult<VMCustomer> getCustomer(int pageSize, int page, string search)
        {
            List<VMCustomer> customerViews;
            if (pageSize > 0)
            {
                var result = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search) ? c.code.StartsWith(search) || c.phone.StartsWith(search) : true).OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                customerViews = Mapper.Map<List<MstCustomer>, List<VMCustomer>>(result);
            }
            else
            {
                var result = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search) ? c.code.StartsWith(search) || c.phone.StartsWith(search) : true).ToList();
                customerViews = Mapper.Map<List<MstCustomer>, List<VMCustomer>>(result);
            }

            return new ListResult<VMCustomer>
                {
                    Data = customerViews,
                    TotalCount = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search) ? c.code.StartsWith(search) || c.phone.StartsWith(search) : true).Count(),
                    Page = page
                };
        }

        public VMCustomer getCustomerById(Int64 id)
        {
            var result = repo.MstCustomers.Where(c=>c.id == id).FirstOrDefault();
            VMCustomer customerViews = Mapper.Map<MstCustomer,VMCustomer>(result);
            return customerViews;
        }

        public List<VMLookUpItem> getCustomerLookUp()
        {
            return repo.MstCustomers.Select(s => new VMLookUpItem { value = s.id, label = s.name }).ToList();
        }

        public ResponseMessage postCustomer(VMCustomer customer)
        {
            MstCustomer customerToPost = Mapper.Map<VMCustomer, MstCustomer>(customer);
            List<MstCustomerAddress> customerAddress = customerToPost.MstCustomerAddresses.ToList();
            foreach (var caddress in customerAddress)
            {
                caddress.createdOn = DateTime.Now;
                caddress.createdBy = _LoggedInuserId;
            }
            customerToPost.createdOn = DateTime.Now;
            customerToPost.createdBy = _LoggedInuserId;
            repo.MstCustomers.Add(customerToPost);
            repo.SaveChanges();
            return new ResponseMessage(customerToPost.id, "Customer Added Successfully", ResponseType.Success);
        }

        public ResponseMessage putCustomer(VMCustomer customer)
        {
            var customerAddressDetails = Mapper.Map<List<VMCustomerAddress>, List<MstCustomerAddress>>(customer.MstCustomerAddress);
            repo.MstCustomerAddresses.RemoveRange(repo.MstCustomerAddresses.Where(s => s.customerId== customer.id));
            repo.SaveChanges();

            foreach (var caddress in customerAddressDetails)
            {
                caddress.createdOn = DateTime.Now;
                caddress.createdBy = _LoggedInuserId;
            }
            var customerToPut = repo.MstCustomers.Where(s => s.id == customer.id).FirstOrDefault();
            customerToPut.code = customer.code;
            customerToPut.name = customer.name;
            customerToPut.nickName = customer.nickName;
            customerToPut.gstin = customer.gstin;
            customerToPut.email = customer.email;
            customerToPut.phone = customer.phone;
            customerToPut.alternateEmail1 = customer.alternateEmail1;
            customerToPut.alternateEmail2 = customer.alternateEmail2;
            customerToPut.alternatePhone1 = customer.alternatePhone1;
            customerToPut.alternatePhone2 = customer.alternatePhone2;
            customerToPut.pan = customer.pan;
            customerToPut.isWholesaleCustomer = customer.isWholesaleCustomer;
            customerToPut.alternatePhone1 = customer.alternatePhone1;
            customerToPut.accountPersonName = customer.accountPersonName;
            customerToPut.accountPersonEmail = customer.accountPersonEmail;
            customerToPut.accountPersonPhone = customer.accountPersonPhone;
            customerToPut.updatedOn = DateTime.Now;
            customerToPut.updatedBy = _LoggedInuserId;

            customerToPut.MstCustomerAddresses = customerAddressDetails;
            repo.SaveChanges();

            return new ResponseMessage(customer.id, "Customer Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteCustomer(Int64 id)
        {
            repo.MstCustomerAddresses.RemoveRange(repo.MstCustomerAddresses.Where(s => s.customerId== id));
            repo.SaveChanges();
            repo.MstCustomers.Remove(repo.MstCustomers.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Customer Deleted Successfully", ResponseType.Success);
        }

    }
}