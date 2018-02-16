﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMSWebApi.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WebAPIdbEntities : DbContext
    {
        public WebAPIdbEntities()
            : base("name=WebAPIdbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CFGRoleMenu> CFGRoleMenus { get; set; }
        public virtual DbSet<MstAccessory> MstAccessories { get; set; }
        public virtual DbSet<MstAgent> MstAgents { get; set; }
        public virtual DbSet<MstCategory> MstCategories { get; set; }
        public virtual DbSet<MstCollection> MstCollections { get; set; }
        public virtual DbSet<MstCompanyInfo> MstCompanyInfoes { get; set; }
        public virtual DbSet<MstCompanyLocation> MstCompanyLocations { get; set; }
        public virtual DbSet<MstCourier> MstCouriers { get; set; }
        public virtual DbSet<MstCustomerAddress> MstCustomerAddresses { get; set; }
        public virtual DbSet<MstFinancialYear> MstFinancialYears { get; set; }
        public virtual DbSet<MstFomDensity> MstFomDensities { get; set; }
        public virtual DbSet<MstFomSuggestedMM> MstFomSuggestedMMs { get; set; }
        public virtual DbSet<MstFWRDesign> MstFWRDesigns { get; set; }
        public virtual DbSet<MstFWRShade> MstFWRShades { get; set; }
        public virtual DbSet<MstHsn> MstHsns { get; set; }
        public virtual DbSet<MstMatSize> MstMatSizes { get; set; }
        public virtual DbSet<MstMatThickness> MstMatThicknesses { get; set; }
        public virtual DbSet<MstMenu> MstMenus { get; set; }
        public virtual DbSet<MstQuality> MstQualities { get; set; }
        public virtual DbSet<MstRole> MstRoles { get; set; }
        public virtual DbSet<MstSupplierAddress> MstSupplierAddresses { get; set; }
        public virtual DbSet<MstUnitOfMeasure> MstUnitOfMeasures { get; set; }
        public virtual DbSet<MstUser> MstUsers { get; set; }
        public virtual DbSet<MstuserType> MstuserTypes { get; set; }
        public virtual DbSet<TrnGoodReceiveNote> TrnGoodReceiveNotes { get; set; }
        public virtual DbSet<TrnGoodReceiveNoteItem> TrnGoodReceiveNoteItems { get; set; }
        public virtual DbSet<TrnProductStock> TrnProductStocks { get; set; }
        public virtual DbSet<TrnProductStockDetail> TrnProductStockDetails { get; set; }
        public virtual DbSet<TrnPurchaseOrder> TrnPurchaseOrders { get; set; }
        public virtual DbSet<TrnPurchaseOrderItem> TrnPurchaseOrderItems { get; set; }
        public virtual DbSet<TrnSaleOrder> TrnSaleOrders { get; set; }
        public virtual DbSet<TrnSaleOrderItem> TrnSaleOrderItems { get; set; }
        public virtual DbSet<MstFomSize> MstFomSizes { get; set; }
        public virtual DbSet<MstCustomer> MstCustomers { get; set; }
        public virtual DbSet<MstSupplier> MstSuppliers { get; set; }
    }
}
