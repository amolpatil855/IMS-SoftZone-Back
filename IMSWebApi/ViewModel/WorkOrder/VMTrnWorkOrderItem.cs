using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.ViewModel
{
    public class VMTrnWorkOrderItem
    {
        public long id { get; set; }
        public long workOrderId { get; set; }
        public string area { get; set; }
        public string unit { get; set; }
        public Nullable<int> numberOfPanel { get; set; }
        public Nullable<long> patternId { get; set; }
        public Nullable<long> categoryId { get; set; }
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
        public string fabricDirection { get; set; }
        public Nullable<decimal> trackSize { get; set; }
        public Nullable<decimal> unitHeight { get; set; }
        public Nullable<decimal> unitWidth { get; set; }
        public bool isRemote { get; set; }
        public bool isMotor { get; set; }
        public bool isRodAccessory { get; set; }
        public Nullable<decimal> orderQuantity { get; set; }
        public Nullable<decimal> rate { get; set; }
        public Nullable<decimal> labourCharges { get; set; }

        public string categoryName { get; set; }
        public string collectionName { get; set; }
        public string serialno { get; set; }
        public string itemCode { get; set; }
        public string patternName { get; set; }

        public virtual VMAccessory MstAccessory { get; set; }
        public virtual VMCategory MstCategory { get; set; }
        public virtual VMCollection MstCollection { get; set; }
        public virtual VMFWRShade MstFWRShade { get; set; }
        public virtual VMPattern MstPattern { get; set; }
        public virtual VMTrnWorkOrder TrnWorkOrder { get; set; }
    }
}