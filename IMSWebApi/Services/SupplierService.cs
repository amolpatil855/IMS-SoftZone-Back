﻿using AutoMapper;
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
    public class SupplierService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        public SupplierService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
        }

        public List<VMSupplier> getSupplier()
        {
            var rusult = repo.MstSuppliers.ToList();
            var suppliers = Mapper.Map<List<MstSupplier>, List<VMSupplier>>(rusult);
            return suppliers;
        }

        public VMSupplier getSupplierById(Int64 id)
        {
            var result = repo.MstSuppliers.Where(s => s.id == id).FirstOrDefault();
            var supplier = Mapper.Map<MstSupplier, VMSupplier>(result);
            return supplier;
        }

        public List<VMLookUpItem> getSupplierLookUp()
        {
            return repo.MstSuppliers.Select(s => new VMLookUpItem { id = s.id, Name = s.code }).ToList();
        }

        public ResponseMessage postSupplier(VMSupplier supplier)
        {
            MstSupplier supplierToPost = Mapper.Map<VMSupplier, MstSupplier>(supplier);
            var supplierAddresses = supplierToPost.MstSupplierAddressDetails.ToList();
            foreach (var saddress in supplierAddresses)
            {
                saddress.createdOn = DateTime.Now;
                saddress.createdBy = _LoggedInuserId;
            }
            supplierToPost.createdOn = DateTime.Now;
            supplierToPost.createdBy = _LoggedInuserId;

            repo.MstSuppliers.Add(supplierToPost);
            repo.SaveChanges();

            //List<MstSupplierAddressDetail> supplierAddresDetailsToPost = Mapper.Map<List<VMSupplierAddressDetail>, List<MstSupplierAddressDetail>>(supplier.SupplierAddressDetails);
            //foreach(var supplierAddresDetail in supplierAddresDetailsToPost )
            //{
            //    supplierAddresDetail.supplierId = supplierToPost.id;
            //    supplierAddresDetail.createdOn = DateTime.Now;
            //    repo.MstSupplierAddressDetails.Add(supplierAddresDetail);
            //}
            //repo.SaveChanges();
            return new ResponseMessage(supplierToPost.id, "Supplier Added Successfully", ResponseType.Success);

        }

        public ResponseMessage putSupplier(VMSupplier supplier)
        {
            var supplierAddressDetails = Mapper.Map<List<VMSupplierAddressDetail>, List<MstSupplierAddressDetail>>(supplier.MstSupplierAddressDetails);
            repo.MstSupplierAddressDetails.RemoveRange(repo.MstSupplierAddressDetails.Where(s => s.supplierId == supplier.id));
            repo.SaveChanges();

            foreach (var saddress in supplierAddressDetails)
            {
                saddress.createdOn = DateTime.Now;
                saddress.createdBy = _LoggedInuserId;
            }
            var supplierToPut = repo.MstSuppliers.Where(s => s.id == supplier.id).FirstOrDefault();
            supplierToPut.code = supplier.code;
            supplierToPut.name = supplier.name;
            supplierToPut.firmName = supplier.firmName;
            supplierToPut.description = supplier.description;
            supplierToPut.gstin = supplier.gstin;
            supplierToPut.email = supplier.email;
            supplierToPut.phone = supplier.phone;
            supplierToPut.accountPersonName = supplier.accountPersonName;
            supplierToPut.accountPersonEmail = supplier.accountPersonEmail;
            supplierToPut.accountPersonPhone = supplier.accountPersonPhone;
            supplierToPut.warehousePersonName = supplier.warehousePersonName;
            supplierToPut.warehousePersonEmail = supplier.warehousePersonEmail;
            supplierToPut.warehousePersonPhone = supplier.warehousePersonPhone;
            supplierToPut.dispatchPersonName = supplier.dispatchPersonName;
            supplierToPut.dispatchPersonEmail = supplier.dispatchPersonEmail;
            supplierToPut.dispatchPersonPhone = supplier.dispatchPersonPhone;
            supplierToPut.updatedOn = DateTime.Now;
            supplierToPut.updatedBy = _LoggedInuserId;
            supplierToPut.MstSupplierAddressDetails = supplierAddressDetails;
            repo.SaveChanges();

            return new ResponseMessage(supplier.id, "Supplier Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteSupplier(Int64 id)
        {
            repo.MstSupplierAddressDetails.RemoveRange(repo.MstSupplierAddressDetails.Where(s => s.supplierId == id));
            repo.SaveChanges();
            repo.MstSuppliers.Remove(repo.MstSuppliers.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Supplier Deleted Successfully", ResponseType.Success);
        }

    }
}