using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using IMSWebApi.ViewModel.SalesInvoice;

namespace IMSWebApi.App_Start
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<CFGRoleMenu, VMCFGRoleMenu>();
            Mapper.CreateMap<VMCFGRoleMenu, CFGRoleMenu>();
            
            Mapper.CreateMap<VMMenu, MstMenu>();
            Mapper.CreateMap<MstMenu, VMMenu>();

            Mapper.CreateMap<VMRole, MstRole>();
            Mapper.CreateMap<MstRole, VMRole>();
            
            Mapper.CreateMap<MstUser, VMUser>();
            Mapper.CreateMap<VMUser, MstUser>();
            
            Mapper.CreateMap<MstuserType, VMUserType>();
            Mapper.CreateMap<VMUserType, MstuserType>();

            Mapper.CreateMap<MstCompanyInfo, VMCompanyInfo>();
            Mapper.CreateMap<VMCompanyInfo, MstCompanyInfo>();
            
            Mapper.CreateMap<MstCustomer, VMCustomer>();
            Mapper.CreateMap<VMCustomer, MstCustomer>();
            
            Mapper.CreateMap<MstCustomerAddress, VMCustomerAddress>();
            Mapper.CreateMap<VMCustomerAddress, MstCustomerAddress>();
            
            Mapper.CreateMap<MstSupplier, VMSupplier>();
            Mapper.CreateMap<VMSupplier, MstSupplier>();
            
            Mapper.CreateMap<MstSupplierAddress, VMSupplierAddress>();
            Mapper.CreateMap<VMSupplierAddress, MstSupplierAddress>();

            Mapper.CreateMap<MstCollection, VMCollection>();
            Mapper.CreateMap<VMCollection, MstCollection>();

            Mapper.CreateMap<MstCategory, VMCategory>();
            Mapper.CreateMap<VMCategory, MstCategory>();

            Mapper.CreateMap<MstQuality, VMQuality>();
            Mapper.CreateMap<VMQuality, MstQuality>();

            Mapper.CreateMap<MstFWRDesign, VMFWRDesign>();
            Mapper.CreateMap<VMFWRDesign, MstFWRDesign>();

            Mapper.CreateMap<MstFWRShade, VMFWRShade>();
            Mapper.CreateMap<VMFWRShade, MstFWRShade>();

            Mapper.CreateMap<MstHsn, VMHsn>();
            Mapper.CreateMap<VMHsn, MstHsn>();

            Mapper.CreateMap<MstMatThickness, VMMatThickness>();
            Mapper.CreateMap<VMMatThickness, MstMatThickness>();

            Mapper.CreateMap<MstMatSize, VMMatSize>();
            Mapper.CreateMap<VMMatSize, MstMatSize>();

            Mapper.CreateMap<MstFomDensity, VMFomDensity>();
            Mapper.CreateMap<VMFomDensity, MstFomDensity>();

            Mapper.CreateMap<MstFomSuggestedMM, VMFomSuggestedMM>();
            Mapper.CreateMap<VMFomSuggestedMM, MstFomSuggestedMM>();


            Mapper.CreateMap<MstFomSize, VMFomSize>();
            Mapper.CreateMap<VMFomSize, MstFomSize>();

            Mapper.CreateMap<TrnProductStock, VMTrnProductStock>();
            Mapper.CreateMap<VMTrnProductStock, TrnProductStock>();

            Mapper.CreateMap<TrnProductStockDetail, VMTrnProductStockDetail>();
            Mapper.CreateMap<VMTrnProductStockDetail, TrnProductStockDetail>();

            Mapper.CreateMap<MstCompanyLocation, VMCompanyLocation>();
            Mapper.CreateMap<VMCompanyLocation, MstCompanyLocation>();

            Mapper.CreateMap<MstAgent, VMAgent>();
            Mapper.CreateMap<VMAgent, MstAgent>();

            Mapper.CreateMap<MstCourier, VMCourier>();
            Mapper.CreateMap<VMCourier, MstCourier>();

            Mapper.CreateMap<MstFinancialYear, VMFinancialYear>();
            Mapper.CreateMap<VMFinancialYear, MstFinancialYear>();

            Mapper.CreateMap<MstAccessory, VMAccessory>();
            Mapper.CreateMap<VMAccessory, MstAccessory>();

            Mapper.CreateMap<MstUnitOfMeasure, VMUnitOfMeasure>();
            Mapper.CreateMap<VMUnitOfMeasure, MstUnitOfMeasure>();

            Mapper.CreateMap<TrnPurchaseOrder, VMTrnPurchaseOrder>();
            Mapper.CreateMap<VMTrnPurchaseOrder, TrnPurchaseOrder>();

            Mapper.CreateMap<TrnPurchaseOrderItem, VMTrnPurchaseOrderItem>();
            Mapper.CreateMap<VMTrnPurchaseOrderItem, TrnPurchaseOrderItem>();

            Mapper.CreateMap<TrnSaleOrder, VMTrnSaleOrder>();
            Mapper.CreateMap<VMTrnSaleOrder, TrnSaleOrder>();

            Mapper.CreateMap<TrnSaleOrderItem, VMTrnSaleOrderItem>();
            Mapper.CreateMap<VMTrnSaleOrderItem, TrnSaleOrderItem>();

            Mapper.CreateMap<TrnGoodReceiveNote, VMTrnGoodReceiveNote>();
            Mapper.CreateMap<VMTrnGoodReceiveNote, TrnGoodReceiveNote>();

            Mapper.CreateMap<TrnGoodReceiveNoteItem, VMTrnGoodReceiveNoteItem>();
            Mapper.CreateMap<VMTrnGoodReceiveNoteItem, TrnGoodReceiveNoteItem>();

            Mapper.CreateMap<TrnGoodIssueNote, VMTrnGoodIssueNote>();
            Mapper.CreateMap<VMTrnGoodIssueNote, TrnGoodIssueNote>();

            Mapper.CreateMap<TrnGoodIssueNoteItem, VMTrnGoodIssueNoteItem>();
            Mapper.CreateMap<VMTrnGoodIssueNoteItem, TrnGoodIssueNoteItem>();

            Mapper.CreateMap<TrnSalesInvoice, VMTrnSalesInvoice>();
            Mapper.CreateMap<VMTrnSalesInvoice, TrnSalesInvoice>();

            Mapper.CreateMap<TrnSalesInvoiceItem, VMTrnSalesInvoiceItem>();
            Mapper.CreateMap<VMTrnSalesInvoiceItem, TrnSalesInvoiceItem>();

            Mapper.CreateMap<TrnMaterialSelection, VMTrnMaterialSelection>();
            Mapper.CreateMap<VMTrnMaterialSelection, TrnMaterialSelection>();

            Mapper.CreateMap<TrnMaterialSelectionItem, VMTrnMaterialSelectionItem>();
            Mapper.CreateMap<VMTrnMaterialSelectionItem, TrnMaterialSelectionItem>();

            Mapper.CreateMap<TrnMaterialQuotation, VMTrnMaterialQuotation>();
            Mapper.CreateMap<VMTrnMaterialQuotation, TrnMaterialQuotation>();

            Mapper.CreateMap<TrnMaterialQuotationItem, VMTrnMaterialQuotationItem>();
            Mapper.CreateMap<VMTrnMaterialQuotationItem, TrnMaterialQuotationItem>();

            Mapper.CreateMap<TrnAdvancePayment, VMTrnAdvancePayment>();
            Mapper.CreateMap<VMTrnAdvancePayment, TrnAdvancePayment>();

            Mapper.CreateMap<vwAccessory, VMvwAccessory>();
            Mapper.CreateMap<VMvwAccessory, vwAccessory>();
            
            Mapper.CreateMap<vwFoam, VMvwFoam>();
            Mapper.CreateMap<VMvwFoam, vwFoam>();

            Mapper.CreateMap<vwFWR, VMvwFWR>();
            Mapper.CreateMap<VMvwFWR, vwFWR>();

            Mapper.CreateMap<vwMattress, VMvwMattress>();
            Mapper.CreateMap<VMvwMattress, vwMattress>();

            Mapper.CreateMap<vwDasBoard, VMvwDashboard>();
            Mapper.CreateMap<VMvwDashboard, vwDasBoard>();

            Mapper.CreateMap<MstPattern, VMPattern>();
            Mapper.CreateMap<VMPattern, MstPattern>();
        }
    }
}