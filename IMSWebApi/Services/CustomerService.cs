using AutoMapper;
using IMSWebApi.Enums;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

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

        public List<VMCustomer> getCustomer()
        {
            var result = repo.MstCustomers.ToList();
            List<VMCustomer> customerViews = Mapper.Map<List<MstCustomer>, List<VMCustomer>>(result);
            return customerViews;
        }

        public VMCustomer getCustomerById(Int64 id)
        {
            var result = repo.MstCustomers.Where(c=>c.id == id).FirstOrDefault();
            VMCustomer customerViews = Mapper.Map<MstCustomer,VMCustomer>(result);
            return customerViews;
        }

        public List<VMLookUpItem> getCustomerLookUp()
        {
            return repo.MstCustomers.Select(s => new VMLookUpItem { id = s.id, Name = s.name }).ToList();
        }

        public ResponseMessage postCustomer(VMCustomer customer)
        {
            MstCustomer customerToPost = Mapper.Map<VMCustomer, MstCustomer>(customer);
            List<MstCustomerAddressDetail> customerAddress = customerToPost.MstCustomerAddressDetails.ToList();
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
            var customerAddressDetails = Mapper.Map<List<VMCustomerAddressDetail>, List<MstCustomerAddressDetail>>(customer.MstCustomerAddressDetails);
            repo.MstCustomerAddressDetails.RemoveRange(repo.MstCustomerAddressDetails.Where(s => s.customerId== customer.id));
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
            customerToPut.alternateEamil2 = customer.alternateEamil2;
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

            customerToPut.MstCustomerAddressDetails = customerAddressDetails;
            repo.SaveChanges();

            return new ResponseMessage(customer.id, "Customer Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteCustomer(Int64 id)
        {
            repo.MstCustomerAddressDetails.RemoveRange(repo.MstCustomerAddressDetails.Where(s => s.customerId== id));
            repo.SaveChanges();
            repo.MstCustomers.Remove(repo.MstCustomers.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Customer Deleted Successfully", ResponseType.Success);
        }

    }
}