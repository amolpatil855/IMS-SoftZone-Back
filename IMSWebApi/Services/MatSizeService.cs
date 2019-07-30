using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;
using IMSWebApi.Enums;
using System.Resources;
using System.Reflection;
using System.Data;
using IMSWebApi.ViewModel.UploadVM;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Configuration;

namespace IMSWebApi.Services
{
    public class MatSizeService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        CategoryService _categoryService;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public MatSizeService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMMatSizeList> getMatSizes(int pageSize, int page, string search)
        {
            List<VMMatSizeList> matSizeListView;
            matSizeListView = repo.MstMatSizes.Where(m => !string.IsNullOrEmpty(search) 
                    ? m.sizeCode.StartsWith(search) 
                    || m.MstCollection.collectionCode.StartsWith(search)
                    || m.MstQuality.qualityCode.StartsWith(search)
                    || m.MstMatThickness.thicknessCode.StartsWith(search)
                    || m.purchaseRate.ToString().StartsWith(search) : true)
                    .Select(d => new VMMatSizeList
                    {
                        id = d.id,
                        collectionCode = d.MstCollection != null ? d.MstCollection.collectionCode : string.Empty,
                        qualityCode = d.MstQuality != null ? d.MstQuality.qualityCode : string.Empty,
                        thicknessCode = d.MstMatThickness != null ? d.MstMatThickness.thicknessCode : string.Empty,
                        sizeCode = d.sizeCode,
                        purchaseRate = d.purchaseRate
                    })
                    .OrderBy(m => m.collectionCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();
            
            return new ListResult<VMMatSizeList>
            {
                Data = matSizeListView,
                TotalCount = repo.MstMatSizes.Where(m => !string.IsNullOrEmpty(search) 
                    ? m.sizeCode.StartsWith(search) 
                    || m.MstCollection.collectionCode.StartsWith(search)
                    || m.MstQuality.qualityCode.StartsWith(search)
                    || m.MstMatThickness.thicknessCode.StartsWith(search)
                    || m.purchaseRate.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMMatSize getMatSizeById(Int64 id)
        {
            var result = repo.MstMatSizes.Where(q => q.id == id).FirstOrDefault();

            var matSizeView = Mapper.Map<MstMatSize, VMMatSize>(result);
            return matSizeView;
        }

        public List<VMLookUpItem> getMatSizeLookUpByCollectionId(Int64 collectionId)
        {
            return repo.MstMatSizes.Where(m=>m.collectionId == collectionId)
                .OrderBy(m=>m.sizeCode)
                .Select(q => new VMLookUpItem { value = q.id, 
                    label = q.sizeCode + " (" + q.MstMatThickness.thicknessCode +"-"+ q.MstQuality.qualityCode+")"}).ToList();
        }

        public List<VMLookUpItem> getMatSizeLookUpForGRN(Int64 collectionId)
        {
            return repo.TrnPurchaseOrderItems.Where(m => m.collectionId == collectionId 
                                                    && m.matSizeId!=null
                                                   && (m.status.Equals("Approved") || m.status.Equals("PartialCompleted")))
                .OrderBy(m => m.MstMatSize.sizeCode)
                .Select(q => new VMLookUpItem
                {
                    value = q.MstMatSize.id,
                    label = q.MstMatSize.sizeCode + " (" + q.MstMatSize.MstMatThickness.thicknessCode + "-" + q.MstMatSize.MstQuality.qualityCode + ")"
                }).Distinct().ToList();
        }

        public List<VMLookUpItem> getMatSizeLookUpByMatThicknessId(Int64 matThicknessId)
        {
            return repo.MstMatSizes.Where(m => m.thicknessId == matThicknessId)
                .OrderBy(m => m.sizeCode)
                .Select(q => new VMLookUpItem
                {
                    value = q.id,
                    label = q.sizeCode + " (" + q.MstMatThickness.thicknessCode + "-" + q.MstQuality.qualityCode + ")"
                }).ToList();
        }

        public ResponseMessage postMatSize(VMMatSize matSize)
        {
            MstMatSize matSizeToPost = Mapper.Map<VMMatSize, MstMatSize>(matSize);
            matSizeToPost.categoryId = _categoryService.getMatressCategory().id;
            matSizeToPost.createdOn = DateTime.Now;
            matSizeToPost.createdBy = _LoggedInuserId;

            repo.MstMatSizes.Add(matSizeToPost);
            repo.SaveChanges();
            return new ResponseMessage(matSizeToPost.id, resourceManager.GetString("MatSizeAdded"), ResponseType.Success);
        }

        public ResponseMessage putMatSize(VMMatSize matSize)
        {
            var matSizeToPut = repo.MstMatSizes.Where(q => q.id == matSize.id).FirstOrDefault();

            //matSizeToPut.categoryId = matSize.categoryId;
            matSizeToPut.collectionId = matSize.collectionId;
            matSizeToPut.qualityId = matSize.qualityId;
            matSizeToPut.thicknessId = matSize.thicknessId;
            matSizeToPut.length = matSize.length;
            matSizeToPut.width = matSize.width;
            matSizeToPut.sizeCode = matSize.sizeCode;
            matSizeToPut.rate = matSize.rate;
            matSizeToPut.purchaseRate = matSize.purchaseRate;
            matSizeToPut.stockReorderLevel = matSize.stockReorderLevel;

            matSizeToPut.updatedBy = _LoggedInuserId;
            matSizeToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(matSize.id, resourceManager.GetString("MatSizeUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteMatSize(Int64 id)
        {
            repo.MstMatSizes.Remove(repo.MstMatSizes.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("MatSizeDeleted"), ResponseType.Success);
        }

        public int UploadMatSize(HttpPostedFileBase file)
        {
            DataTable matSizeDataTable = new DataTable();
            matSizeDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            DataTable InvalidData = new DataTable();
            validatedDataTable = ValidateDataTable(matSizeDataTable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Collection*"].SetOrdinal(0);
            validatedDataTable.Columns["Quality*"].SetOrdinal(1);
            validatedDataTable.Columns["Thickness*"].SetOrdinal(2);
            validatedDataTable.Columns["Length*"].SetOrdinal(3);
            validatedDataTable.Columns["Width*"].SetOrdinal(4);
            validatedDataTable.Columns["Rate*"].SetOrdinal(5);
            validatedDataTable.Columns["Purchase Rate*"].SetOrdinal(6);
            validatedDataTable.Columns["Stock Reorder Level*"].SetOrdinal(7);

            validatedDataTable.AcceptChanges();

            //var shade = repo.UploadFWRShade(validatedDataTable).ToList();

            //SqlParameter param = new SqlParameter("@FWRShadeType", SqlDbType.Structured);
            //param.TypeName = "dbo.FWRShadeType";
            //param.Value = validatedDataTable;

            //using (WebAPIdbEntities db = new WebAPIdbEntities())
            //{
            //    var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFWRShade @FWRShadeType", param);
            //}

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadMattressSize", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@MattressSizeType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Length*") == row["length"].ToString() && r.Field<string>("Width*") == row["width"].ToString()).FirstOrDefault());
                            InvalidData.Rows.Add(row.ItemArray);
                            row.Delete();
                        }
                        dt.AcceptChanges();
                    }
                }
            }

            validatedDataTable.AcceptChanges();
            InvalidData.AcceptChanges();

            //if contains invalid data then convert to Excel 
            if (InvalidData != null)
            {
                datatable_helper.ConvertToExcel(InvalidData, true);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return 0;
        }

        /// <summary>
        /// validate data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateDataTable(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new VMMatSizeUpload();

            //setting column name as its caption name
            foreach (DataColumn col in rawTable.Columns)
            {
                string colname = rawTable.Columns[col.ColumnName].Caption;
                InvalidData.Columns.Add(colname);
                col.ColumnName = colname;
            }
            InvalidData.Columns.Add("Reason");
            rawTable.AcceptChanges();

            //first validating without keys
            foreach (DataRow row in rawTable.Rows)
            {
                model.collectionId = model.qualityId = model.thicknessId = 1;
                model.length = !string.IsNullOrWhiteSpace(row["Length*"].ToString()) ? Convert.ToDecimal(row["Length*"]) : 0; ;
                model.width = !string.IsNullOrWhiteSpace(row["Width*"].ToString()) ? Convert.ToDecimal(row["Width*"]) : 0;
                model.rate = !string.IsNullOrWhiteSpace(row["Rate*"].ToString()) ? Convert.ToDecimal(row["Rate*"]) : 0;
                model.purchaseRate = !string.IsNullOrWhiteSpace(row["Purchase Rate*"].ToString()) ? Convert.ToDecimal(row["Purchase Rate*"]) : 0;
                model.stockReorderLevel = !string.IsNullOrWhiteSpace(row["Stock Reorder Level*"].ToString()) ? Convert.ToInt32(row["Stock Reorder Level*"]) : 0;

                var context = new ValidationContext(model, null, null);
                var result = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(model, context, result, true);

                string errormessage = "";
                if (!isValid)
                {
                    InvalidData.Rows.Add(row.ItemArray);

                    //adding reason of invalid row
                    for (int i = 0; i < result.Count; i++)
                    {
                        errormessage = string.Join(".", string.Join(",", result.ElementAt(i)), errormessage);
                    }
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = errormessage;
                    row.Delete();
                }
            }

            rawTable.AcceptChanges();
            InvalidData.AcceptChanges();

            //finding distinct values of Mattress Thickness
            List<string> MatThickness = new List<string>();
            List<string> MatQuality = new List<string>();

            MatThickness = rawTable.AsEnumerable().Select(c => c.Field<string>("Thickness*")).Distinct().ToList();
            MatQuality = rawTable.AsEnumerable().Select(c => c.Field<string>("Quality*")).Distinct().ToList();

            //formatting as comma separated string
            string matThicknessString = (string.Join(",", MatThickness.ToArray()));
            string matQualityString = (string.Join(",", MatQuality.ToArray()));

            //fetching data for Mattress Thickness            
            var matThicknessKey = repo.GET_MAT_THICKNESS_ID(matThicknessString).ToList();
            var matQualityKey = repo.GET_QUALITY_ID(matQualityString).ToList();

            //replacing values of category, collection, quality and design by their ids
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var matThicknessrow = matThicknessKey.Where(d => d.thicknessCode.ToLower().Equals(rawTable.Rows[j]["Thickness*"].ToString().Trim().ToLower())).FirstOrDefault();
                var matQualityRow = matQualityKey.Where(d => d.qualityCode.ToLower().Equals(rawTable.Rows[j]["Quality*"].ToString().Trim().ToLower()) &&
                                                        d.MstCollection.collectionCode.ToLower().Equals(rawTable.Rows[j]["Collection*"].ToString().Trim().ToLower())).FirstOrDefault();
                if (matThicknessrow != null && matQualityRow != null)
                {
                    rawTable.Rows[j]["Collection*"] = matQualityRow.collectionId;
                    rawTable.Rows[j]["Quality*"] = matQualityRow.id;
                    rawTable.Rows[j]["Thickness*"] = matThicknessrow.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify collection, quality, thickness";
                    rawTable.Rows[j].Delete();
                }
            }

            rawTable.AcceptChanges();

            return rawTable;
        }
    }

}