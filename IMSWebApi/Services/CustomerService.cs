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
using System.Resources;
using System.Reflection;
using System.Transactions;
using System.Text;

namespace IMSWebApi.Services
{
    public class CustomerService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        RoleService _roleService = new RoleService();
        UserService _userService = new UserService();
        Int64 _LoggedInuserId;
        bool _IsAdministrator;
        ResourceManager resourceManager = null;
        public CustomerService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _IsAdministrator = HttpContext.Current.User.IsInRole("Administrator");
            _roleService = new RoleService();
            _userService = new UserService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMCustomerList> getCustomers(int pageSize, int page, string search)
        {
            List<VMCustomerList> customerListingView;
            customerListingView = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search)
                    ? c.code.StartsWith(search)
                    || c.name.ToString().StartsWith(search)
                    || c.email.StartsWith(search)
                    || c.phone.StartsWith(search)
                    || c.nickName.StartsWith(search)
                    || c.MstCustomerAddresses.Where(ca => ca.isPrimary == true).FirstOrDefault().gstin.StartsWith(search) : true)
                    .Select(c => new VMCustomerList
                    {
                        id = c.id,
                        code = c.code,
                        name = c.name,
                        email = c.email,
                        phone = c.phone,
                        nickName = c.nickName,
                        isWholesaleCustomer = c.isWholesaleCustomer,
                        gstin = c.MstCustomerAddresses.Where(supp => supp.isPrimary == true).FirstOrDefault().gstin
                    })
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMCustomerList>
            {
                Data = customerListingView,
                TotalCount = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search)
                    ? c.code.StartsWith(search)
                    || c.name.ToString().StartsWith(search)
                    || c.email.StartsWith(search)
                    || c.phone.StartsWith(search)
                    || c.nickName.StartsWith(search)
                    || c.MstCustomerAddresses.Where(ca => ca.isPrimary == true).FirstOrDefault().gstin.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMCustomer getCustomerById(Int64 id)
        {
            var result = repo.MstCustomers.Where(c => c.id == id).FirstOrDefault();
            VMCustomer customerViews = Mapper.Map<MstCustomer, VMCustomer>(result);
            customerViews.userName = customerViews.MstUser != null ? customerViews.MstUser.userName : null;
            return customerViews;
        }

        public VMCustomer getLoggedInCustomerDetails()
        {
            var result = repo.MstCustomers.Where(c => c.userId == _LoggedInuserId).FirstOrDefault();
            VMCustomer customerViews = Mapper.Map<MstCustomer, VMCustomer>(result);
            customerViews.userName = customerViews.MstUser != null ? customerViews.MstUser.userName : null;
            return customerViews;
        }

        public List<VMLookUpItem> getCustomerLookUp()
        {
            return repo.MstCustomers
                .OrderBy(s => s.code)
                .Select(s => new VMLookUpItem { value = s.id, label = s.name + " (" + s.phone + ")"  }).ToList();
        }

        public List<VMLookUpItem> getCustomerLookUpWithoutWholesaleCustomer()
        {
            return repo.MstCustomers.Where(c=>c.isWholesaleCustomer != true)
                .OrderBy(s => s.code)
                .Select(s => new VMLookUpItem { value = s.id, label = s.name + " (" + s.phone + ")" }).ToList();
        }

        public List<VMCustomerAddress> getCustomerAddressByCustomerId(Int64 customerId)
        {
            var result = repo.MstCustomerAddresses.Where(ca => ca.customerId == customerId).ToList();
            List<VMCustomerAddress> customerAddressViews = Mapper.Map<List<MstCustomerAddress>, List<VMCustomerAddress>>(result);
            return customerAddressViews;
        }

        public ResponseMessage postCustomer(VMCustomer customer)
        {
            using (var transaction = new TransactionScope())
            {
                var customerCodeExist = repo.MstCustomers.Where(c => c.code == customer.code).FirstOrDefault();
                if (customerCodeExist != null)
                {
                    int customerCount = repo.MstCustomers.Count();
                    customer.code = string.Empty;
                    StringBuilder customerCode = new StringBuilder();
                    customerCode.Append("SZ");
                    customerCode.Append(customerCount + 1001);
                    customer.code = customerCode.ToString();
                }
                Nullable<long> userId = null;
                if (customer.isWholesaleCustomer == true)
                {
                    userId = CreateUser(customer, userId);
                }
                MstCustomer customerToPost = Mapper.Map<VMCustomer, MstCustomer>(customer);
                customerToPost.userId = userId;
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
                transaction.Complete();
                return new ResponseMessage(customerToPost.id, resourceManager.GetString("CustomerAdded"),
                    ResponseType.Success);
            }
        }

        private long? CreateUser(VMCustomer customer, Nullable<long> userId)
        {
            VMUser user = new VMUser();
            user.userName = customer.userName;
            user.email = customer.email;
            user.phone = customer.phone;
            user.roleId = _roleService.getCustomerRole().id;
            user.userTypeId = _userService.getCustomerUserType().id;
            MstUser userToPost = Mapper.Map<VMUser, MstUser>(user);
            var originalPassword = _userService.createRandomPassword(8);
            userToPost.password = UserService.encryption(originalPassword);
            userToPost.isActive = true;
            userToPost.createdOn = DateTime.Now;
            userToPost.createdBy = _LoggedInuserId;
            repo.MstUsers.Add(userToPost);
            repo.SaveChanges();
            _userService.sendEmail(userToPost.id, originalPassword, "RegisterUser", false);
            userId = userToPost.id;
            return userId;
        }

        public ResponseMessage putCustomer(VMCustomer customer)
        {
            Nullable<long> userId = null;
            using (var transaction = new TransactionScope())
            {
                MstCustomer customerToPut = repo.MstCustomers.Where(c => c.id == customer.id).FirstOrDefault();
                customerToPut.name = customer.name;
                customerToPut.nickName = customer.nickName;
                customerToPut.code = customer.code;
                customerToPut.email = customer.email;
                customerToPut.alternateEmail1 = customer.alternateEmail1;
                customerToPut.alternateEmail2 = customer.alternateEmail2;
                customerToPut.phone = customer.phone;
                customerToPut.type = customer.type;
                customerToPut.alternatePhone1 = customer.alternatePhone1;
                customerToPut.alternatePhone2 = customer.alternatePhone2;
                customerToPut.isWholesaleCustomer = customer.isWholesaleCustomer;
                customerToPut.pan = customer.pan;
                customerToPut.accountPersonName = customer.accountPersonName;
                customerToPut.accountPersonEmail = customer.accountPersonEmail;
                customerToPut.accountPersonPhone = customer.accountPersonPhone;
                customerToPut.creditPeriodDays = customer.creditPeriodDays;
                //customerToPut.MstCustomerAddresses = customerAddressDetails;
                customerToPut.updatedOn = DateTime.Now;
                customerToPut.updatedBy = _LoggedInuserId;

                updateCustomerAddress(customer);
                
                repo.SaveChanges();

                if (customer.userId != null)
                {
                    MstUser user = repo.MstUsers.Where(u => u.id == customer.userId).FirstOrDefault();
                    user.email = customer.email;
                    user.phone = customer.phone;
                }
                if (customer.isWholesaleCustomer == true && _IsAdministrator && customer.userId == null)
                {
                    userId = CreateUser(customer, userId);
                    customerToPut.userId = userId;
                }
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(customer.id, resourceManager.GetString("CustomerUpdated"), ResponseType.Success);
            }
        }

        public void updateCustomerAddress(VMCustomer customer)
        {
            MstCustomer customerToPut = repo.MstCustomers.Where(c => c.id == customer.id).FirstOrDefault();

            List<MstCustomerAddress> addressesToRemove = new List<MstCustomerAddress>();

            foreach (var customerAddress in customerToPut.MstCustomerAddresses)
            {
                if (customer.MstCustomerAddresses.Any(y => y.id == customerAddress.id))
                {
                    continue;
                }
                else
                {
                    addressesToRemove.Add(customerAddress);
                }
            }

            repo.MstCustomerAddresses.RemoveRange(addressesToRemove);
            repo.SaveChanges();

            customer.MstCustomerAddresses.ForEach(x =>
            {
                if (customerToPut.MstCustomerAddresses.Any(y => y.id == x.id))
                {
                    var customerAddressToPut = repo.MstCustomerAddresses.Where(p => p.id == x.id).FirstOrDefault();

                    customerAddressToPut.addressLine1 = x.addressLine1;
                    customerAddressToPut.addressLine2 = x.addressLine2;
                    customerAddressToPut.city = x.city;
                    customerAddressToPut.state = x.state;
                    customerAddressToPut.country = x.country;
                    customerAddressToPut.pin = x.pin;
                    customerAddressToPut.gstin = x.gstin;
                    customerAddressToPut.isPrimary = x.isPrimary;
                    customerAddressToPut.updatedOn = DateTime.Now;
                    customerAddressToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
                else
                {
                    MstCustomerAddress customerAddress = Mapper.Map<VMCustomerAddress, MstCustomerAddress>(x);
                    customerAddress.createdBy = _LoggedInuserId;
                    customerAddress.createdOn = DateTime.Now;
                    repo.MstCustomerAddresses.Add(customerAddress);
                    repo.SaveChanges();
                }
            });
        }

        public ResponseMessage deleteCustomer(Int64 id)
        {
            using (var transaction = new TransactionScope())
            {
                MstCustomer customerToDelete = repo.MstCustomers.Where(c => c.id == id).FirstOrDefault();
                if (customerToDelete.userId != null)
                    repo.MstUsers.Remove(repo.MstUsers.Where(u => u.id == customerToDelete.userId).FirstOrDefault());
                if (repo.TrnSaleOrders.Where(so=>so.customerId == id).Count() > 0)
                {
                    transaction.Complete();
                    return new ResponseMessage(id, resourceManager.GetString("CustomerNotDeleted"), ResponseType.Error);
                }
                repo.MstCustomers.Remove(repo.MstCustomers.Where(s => s.id == id).FirstOrDefault());
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("CustomerDeleted"), ResponseType.Success);
            }
        }

        public bool canCustomerAddressBeDeleted(Int64 id)
        {
            int addressAssociatedWithSOcount = repo.TrnSaleOrders.Where(so => so.shippingAddressId == id).Count();

            return addressAssociatedWithSOcount > 0 ? false : true;
        }

        public string getCustomerCode()
        {
            var result = repo.MstCustomers.Count();
            StringBuilder customerCode = new StringBuilder();
            customerCode.Append("SZ");
            customerCode.Append(result + 1001);
            return customerCode.ToString();
        }
    }
}