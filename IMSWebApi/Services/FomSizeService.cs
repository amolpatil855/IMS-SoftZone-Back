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
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Configuration;

namespace IMSWebApi.Services
{
    public class FomSizeService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        CategoryService _categoryService = null;
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public FomSizeService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMFomSizeList> getFomSizes(int pageSize, int page, string search)
        {
            List<VMFomSizeList> fomSizeListView;
            fomSizeListView= repo.MstFomSizes.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFomDensity.density.ToString().StartsWith(search)
                    || q.MstFomSuggestedMM.suggestedMM.ToString().StartsWith(search)
                    || q.sizeCode.StartsWith(search)
                    || q.itemCode.StartsWith(search): true)
                    .Select(f => new VMFomSizeList
                    {
                        id = f.id,
                        collectionCode = f.MstCollection != null ? f.MstCollection.collectionCode : string.Empty,
                        qualityCode = f.MstQuality != null ? f.MstQuality.qualityCode : string.Empty,
                        density = f.MstFomDensity != null ? f.MstFomDensity.density : string.Empty,
                        suggestedMM = f.MstFomSuggestedMM != null ? f.MstFomSuggestedMM.suggestedMM.ToString() : string.Empty,
                        sizeCode = f.sizeCode,
                        itemCode = f.itemCode
                    })
                    .OrderBy(q => q.collectionCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMFomSizeList>
            {
                Data = fomSizeListView,
                TotalCount = repo.MstFomSizes.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.MstFomDensity.density.ToString().StartsWith(search)
                    || q.MstFomSuggestedMM.suggestedMM.ToString().StartsWith(search)
                    || q.sizeCode.StartsWith(search)
                    || q.itemCode.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFomSize getFomSizeById(Int64 id)
        {
            var result = repo.MstFomSizes.Where(q => q.id == id).FirstOrDefault();
            var qualityView = Mapper.Map<MstFomSize, VMFomSize>(result);
            return qualityView;
        }

        public List<VMLookUpItem> getFomSizeLookUpByCollection(Int64 collectionId)
        {
            return repo.MstFomSizes.Where(m=>m.collectionId == collectionId)
                .OrderBy(m => m.sizeCode)
                .Select(q => new VMLookUpItem { value = q.id, 
                    label = q.itemCode }).ToList();
        }

        public List<VMLookUpItem> getFomSizeLookUpForGRN(Int64 collectionId)
        {
            return repo.TrnPurchaseOrderItems.Where(m => m.collectionId == collectionId
                                           && (m.status.Equals("Approved") || m.status.Equals("PartialCompleted")))
                .OrderBy(m => m.MstFomSize.sizeCode)
                .Select(q => new VMLookUpItem
                {
                    value = q.MstFomSize.id,
                    label = q.MstFomSize.itemCode
                }).Distinct().ToList();
        }

        public List<VMLookUpItem> getFomSizeLookUpByFomSuggestedMM(Int64 fomSuggestedMMId)
        {
            return repo.MstFomSizes.Where(m => m.fomSuggestedMMId == fomSuggestedMMId)
                .OrderBy(m => m.sizeCode)
                .Select(q => new VMLookUpItem
                {
                    value = q.id,
                    label = q.itemCode
                }).ToList();
        }

        public ResponseMessage postFomSize(VMFomSize fomSize)
        {
            MstFomSize fomSizeToPost = Mapper.Map<VMFomSize, MstFomSize>(fomSize);
            fomSizeToPost.categoryId = _categoryService.getFoamCategory().id;
            fomSizeToPost.createdOn = DateTime.Now;
            fomSizeToPost.createdBy = _LoggedInuserId;

            repo.MstFomSizes.Add(fomSizeToPost);
            repo.SaveChanges();
            return new ResponseMessage(fomSizeToPost.id, resourceManager.GetString("FomSizeAdded"), ResponseType.Success);
        }

        public ResponseMessage putFomSize(VMFomSize fomSize)
        {
            var fomSizeToPut = repo.MstFomSizes.Where(q => q.id == fomSize.id).FirstOrDefault();
            //fomSizeToPut.categoryId = fomSize.categoryId;
            fomSizeToPut.collectionId = fomSize.collectionId;
            fomSizeToPut.qualityId = fomSize.qualityId;
            fomSizeToPut.fomDensityId = fomSize.fomDensityId;
            fomSizeToPut.fomSuggestedMMId = fomSize.fomSuggestedMMId;
            fomSizeToPut.width = fomSize.width;
            fomSizeToPut.length = fomSize.length;
            fomSizeToPut.sizeCode = fomSize.sizeCode;
            fomSizeToPut.itemCode = fomSize.itemCode;
            fomSizeToPut.stockReorderLevel = fomSize.stockReorderLevel;

            fomSizeToPut.updatedBy = _LoggedInuserId;
            fomSizeToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(fomSize.id, resourceManager.GetString("FomSizeUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteFomSize(Int64 id)
        {
            repo.MstFomSizes.Remove(repo.MstFomSizes.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("FomSizeDeleted"), ResponseType.Success);
        }

        /// <summary>
        /// uploading design excel sheet
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Tuple<string,int> UploadFoamSize(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable designDatatable = new DataTable();
            designDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable InvalidData = new DataTable();

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateDataTable(designDatatable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Collection *"].SetOrdinal(0);
            validatedDataTable.Columns["Quality  *"].SetOrdinal(1);
            validatedDataTable.Columns["Density *"].SetOrdinal(2);
            validatedDataTable.Columns["Suggested MM *"].SetOrdinal(3);
            validatedDataTable.Columns["Length *"].SetOrdinal(4);
            validatedDataTable.Columns["Width *"].SetOrdinal(5);
            validatedDataTable.Columns["Item Code *"].SetOrdinal(6);
            validatedDataTable.Columns["Stock Reorder Level *"].SetOrdinal(7);

            validatedDataTable.AcceptChanges();

            //SqlParameter param = new SqlParameter("@FoamSizeType", SqlDbType.Structured);
            //param.TypeName = "dbo.FoamSizeType";
            //param.Value = validatedDataTable;

            //using (WebAPIdbEntities db = new WebAPIdbEntities())
            //{
            //    var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFoamSize @FoamSizeType", param);
            //}

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadFoamSize", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@FoamSizeType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Item Code *") == row["itemCode"].ToString()).FirstOrDefault());
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
            if (InvalidData.Rows.Count > 0)
            {
                Invalidfilename = datatable_helper.ConvertToExcel(InvalidData, true);
                Invalidfilename = string.Concat(path, "ExcelUpload\\", Invalidfilename);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return new Tuple<string, int>(Invalidfilename, validatedDataTable.Rows.Count);
        }

        /// <summary>
        /// validate data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateDataTable(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new VMFomSize();

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
                model.collectionId = model.qualityId = model.fomDensityId = model.fomSuggestedMMId = 1;                
                model.length = !string.IsNullOrWhiteSpace(row["Length *"].ToString()) ? Convert.ToDecimal(row["Length *"]) : 0;
                model.width = !string.IsNullOrWhiteSpace(row["Width *"].ToString()) ? Convert.ToDecimal(row["Width *"]) : 0;
                model.itemCode = row["Item Code *"].ToString();                
                model.stockReorderLevel = !string.IsNullOrWhiteSpace(row["Stock Reorder Level *"].ToString()) ? Convert.ToInt32(row["Stock Reorder Level *"]) : 0;

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

            //finding distinct values of quality codes
            List<string> suggestedMM = new List<string>();
            suggestedMM = rawTable.AsEnumerable().Select(c => c.Field<string>("Suggested MM *")).Distinct().ToList();

            //formatting as comma separated string
            string suggestedMMstring = (string.Join(",", suggestedMM.ToArray()));

            //fetching data for quality            
            var sizekeys = repo.GET_SUGGESTED_MM_ID(suggestedMMstring).ToList();

            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = sizekeys.Where(d => d.MstCollection.collectionCode.ToLower().Equals(rawTable.Rows[j]["Collection *"].ToString().Trim().ToLower())
                                    && d.MstQuality.qualityCode.ToLower().Equals(rawTable.Rows[j]["Quality  *"].ToString().Trim().ToLower())
                                    && d.MstFomDensity.density.ToLower().Equals(rawTable.Rows[j]["Density *"].ToString().Trim().ToLower())
                                    && d.suggestedMM.ToString().ToLower().Trim().Equals(rawTable.Rows[j]["Suggested MM *"].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null)
                {
                    rawTable.Rows[j]["Collection *"] = row.collectionId;
                    rawTable.Rows[j]["Quality  *"] = row.qualityId;
                    rawTable.Rows[j]["Density *"] = row.fomDensityId;
                    rawTable.Rows[j]["Suggested MM *"] = row.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify collection, quality, foam density and suggested MM";
                    rawTable.Rows[j].Delete();
                }
            }
            rawTable.AcceptChanges();

            return rawTable;
        }
    }
}