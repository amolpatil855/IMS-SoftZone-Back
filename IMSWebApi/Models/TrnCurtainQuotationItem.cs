//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class TrnCurtainQuotationItem
    {
        public long id { get; set; }
        public long curtainQuotationId { get; set; }
        public string area { get; set; }
        public string unit { get; set; }
        public Nullable<int> numberOfPanel { get; set; }
        public Nullable<long> patternId { get; set; }
        public long categoryId { get; set; }
        public Nullable<long> collectionId { get; set; }
        public Nullable<long> shadeId { get; set; }
        public Nullable<long> accessoryId { get; set; }
        public bool isPatch { get; set; }
        public bool isVerticalPatch { get; set; }
        public Nullable<int> noOfVerticalPatch { get; set; }
        public Nullable<decimal> verticalPatchWidth { get; set; }
        public Nullable<decimal> verticalPatchQuantity { get; set; }
        public bool isHorizontalPatch { get; set; }
        public Nullable<int> noOfHorizontalPatch { get; set; }
        public Nullable<decimal> horizontalPatchHeight { get; set; }
        public Nullable<decimal> horizontalPatchQuantity { get; set; }
        public bool isLining { get; set; }
        public bool isTrack { get; set; }
        public bool isRod { get; set; }
        public Nullable<decimal> unitHeight { get; set; }
        public Nullable<decimal> unitWidth { get; set; }
        public bool isRemote { get; set; }
        public bool isMotor { get; set; }
        public Nullable<decimal> orderQuantity { get; set; }
        public Nullable<decimal> balanceQuantity { get; set; }
        public Nullable<decimal> deliverQuantity { get; set; }
        public string orderType { get; set; }
        public Nullable<decimal> discount { get; set; }
        public Nullable<decimal> rate { get; set; }
        public Nullable<decimal> rateWithGST { get; set; }
        public Nullable<long> amount { get; set; }
        public Nullable<long> amountWithGST { get; set; }
        public Nullable<decimal> laborCharges { get; set; }
        public Nullable<int> gst { get; set; }
        public string status { get; set; }
        public System.DateTime createdOn { get; set; }
        public long createdBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public Nullable<long> updatedBy { get; set; }
    
        public virtual MstAccessory MstAccessory { get; set; }
        public virtual MstCategory MstCategory { get; set; }
        public virtual MstCollection MstCollection { get; set; }
        public virtual MstFWRShade MstFWRShade { get; set; }
        public virtual MstPattern MstPattern { get; set; }
        public virtual TrnCurtainQuotation TrnCurtainQuotation { get; set; }
    }
}
