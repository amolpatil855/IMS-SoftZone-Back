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
    public class FomDensityService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        CategoryService _categoryService = null;
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public FomDensityService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMFomDensityList> getFomDensities(int pageSize, int page, string search)
        {
            List<VMFomDensityList> fomDensityListView;
            fomDensityListView = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search)
                    || f.purchaseRatePerMM.ToString().StartsWith(search)
                    || f.purchaseRatePerKG.ToString().StartsWith(search) : true)
                     .Select(f => new VMFomDensityList
                     {
                         id = f.id,
                         collectionCode = f.MstCollection != null ? f.MstCollection.collectionCode : string.Empty,
                         qualityCode = f.MstQuality != null ? f.MstQuality.qualityCode : string.Empty,
                         density = f.density,
                         purchaseRatePerKG = f.purchaseRatePerKG,
                         purchaseRatePerMM = f.purchaseRatePerMM
                     })
                    .OrderBy(f => f.collectionCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMFomDensityList>
            {
                Data = fomDensityListView,
                TotalCount = repo.MstFomDensities.Where(f => !string.IsNullOrEmpty(search)
                    ? f.MstCollection.collectionCode.StartsWith(search)
                    || f.MstQuality.qualityCode.StartsWith(search)
                    || f.density.StartsWith(search)
                    || f.purchaseRatePerMM.ToString().StartsWith(search)
                    || f.purchaseRatePerKG.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFomDensity getFomDensityById(Int64 id)
        {
            var result = repo.MstFomDensities.Where(q => q.id == id).FirstOrDefault();
            var fomDensityView = Mapper.Map<MstFomDensity, VMFomDensity>(result);
            return fomDensityView;
        }

        public List<VMLookUpItem> getFomDensityLookUpByQualityId(Int64 qualityId)
        {
            return repo.MstFomDensities.Where(f => f.qualityId == qualityId)
                .OrderBy(q => q.density)
                .Select(q => new VMLookUpItem { value = q.id, label = q.density })
                .ToList();
        }

        public ResponseMessage postFomDensity(VMFomDensity fomDensity)
        {
            MstFomDensity fomDensityToPost = Mapper.Map<VMFomDensity, MstFomDensity>(fomDensity);
            fomDensityToPost.categoryId = _categoryService.getFoamCategory().id;
            fomDensityToPost.createdOn = DateTime.Now;
            fomDensityToPost.createdBy = _LoggedInuserId;

            repo.MstFomDensities.Add(fomDensityToPost);
            repo.SaveChanges();
            return new ResponseMessage(fomDensityToPost.id, resourceManager.GetString("FomDensityAdded"), ResponseType.Success);
        }

        public ResponseMessage putFomDensity(VMFomDensity fomDensity)
        {
            var fomDensityToPut = repo.MstFomDensities.Where(q => q.id == fomDensity.id).FirstOrDefault();
            //fomDensityToPut.categoryId = fomDensity.categoryId;
            fomDensityToPut.collectionId = fomDensity.collectionId;
            fomDensityToPut.qualityId = fomDensity.qualityId;
            fomDensityToPut.density = fomDensity.density;
            fomDensityToPut.description = fomDensity.description;
            fomDensityToPut.purchaseRatePerMM = fomDensity.purchaseRatePerMM;
            fomDensityToPut.purchaseRatePerKG = fomDensity.purchaseRatePerKG;
            fomDensityToPut.sellingRatePerMM = fomDensity.sellingRatePerMM;
            fomDensityToPut.sellingRatePerKG = fomDensity.sellingRatePerKG;

            fomDensityToPut.updatedBy = _LoggedInuserId;
            fomDensityToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(fomDensity.id, resourceManager.GetString("FomDensityUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteFomDensity(Int64 id)
        {
            repo.MstFomDensities.Remove(repo.MstFomDensities.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("FomDensityDeleted"), ResponseType.Success);
        }

        /// <summary>
        /// uploading design excel sheet
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Tuple<string,int> UploadFoamDensity(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable foamDensityDatatable = new DataTable();
            foamDensityDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable InvalidData = new DataTable();

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateDataTable(foamDensityDatatable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Collection *"].SetOrdinal(0);
            validatedDataTable.Columns["Quality  *"].SetOrdinal(1);
            validatedDataTable.Columns["Density *"].SetOrdinal(2);
            validatedDataTable.Columns["Description"].SetOrdinal(3);
            validatedDataTable.Columns["Purchase Rate/MM *"].SetOrdinal(4);
            validatedDataTable.Columns["Purchase Rate/KG *"].SetOrdinal(5);
            validatedDataTable.Columns["Selling Rate/MM *"].SetOrdinal(6);
            validatedDataTable.Columns["Selling Rate/KG *"].SetOrdinal(7);

            validatedDataTable.AcceptChanges();

            //SqlParameter param = new SqlParameter("@FoamDensityType", SqlDbType.Structured);
            //param.TypeName = "dbo.FoamDensityType";
            //param.Value = validatedDataTable;

            //using (WebAPIdbEntities db = new WebAPIdbEntities())
            //{
            //    var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFoamDensity @FoamDensityType", param);
            //}
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadFoamDensity", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@FoamDensityType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Density *") == row["density"].ToString()).FirstOrDefault());
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
            decimal outputValue = 0.0M;
            var model = new VMFomDensity();

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
                model.collectionId = model.qualityId = 1;
                model.density = row["Density *"].ToString();
                model.description = row["Description"].ToString();

                if (!string.IsNullOrWhiteSpace(row["Purchase Rate/MM *"].ToString()) ? Decimal.TryParse(row["Purchase Rate/MM *"].ToString(), out outputValue) : false)
                {
                    model.purchaseRatePerMM = outputValue;
                    outputValue = 0.0M;
                }

                //model.purchaseRatePerMM = !string.IsNullOrWhiteSpace(row["Purchase Rate/MM *"].ToString()) ? Convert.ToDecimal(row["Purchase Rate/MM *"]) : 0;
                if (!string.IsNullOrWhiteSpace(row["Purchase Rate/KG *"].ToString()) ? Decimal.TryParse(row["Purchase Rate/KG *"].ToString(), out outputValue) : false)
                {
                    model.purchaseRatePerKG = outputValue;
                    outputValue = 0.0M;
                }
                //model.purchaseRatePerKG = !string.IsNullOrWhiteSpace(row["Purchase Rate/KG *"].ToString()) ? Convert.ToDecimal(row["Purchase Rate/KG *"]) : 0;
                if (!string.IsNullOrWhiteSpace(row["Selling Rate/MM *"].ToString()) ? Decimal.TryParse(row["Selling Rate/MM *"].ToString(), out outputValue) : false)
                {
                    model.sellingRatePerMM = outputValue;
                    outputValue = 0.0M;
                }
                //model.sellingRatePerMM = !string.IsNullOrWhiteSpace(row["Selling Rate/MM *"].ToString()) ? Convert.ToDecimal(row["Selling Rate/MM *"]) : 0;
                if (!string.IsNullOrWhiteSpace(row["Selling Rate/KG *"].ToString()) ? Decimal.TryParse(row["Selling Rate/KG *"].ToString(), out outputValue) : false)
                {
                    model.sellingRatePerKG = outputValue;
                    outputValue = 0.0M;
                }
                //model.sellingRatePerKG = !string.IsNullOrWhiteSpace(row["Selling Rate/KG *"].ToString()) ? Convert.ToDecimal(row["Selling Rate/KG *"]) : 0;

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
            List<string> qualitycodes = new List<string>();
            qualitycodes = rawTable.AsEnumerable().Select(c => c.Field<string>("Quality  *")).Distinct().ToList();

            //formatting as comma separated string
            string qualityString = (string.Join(",", qualitycodes.ToArray()));

            //fetching data for quality            
            var densitykeys = repo.GET_QUALITY_ID(qualityString).ToList();

            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = densitykeys.Where(d => d.MstCollection.collectionCode.Trim().ToLower().Equals(rawTable.Rows[j]["Collection *"].ToString().Trim().ToLower())
                        && d.qualityCode.Trim().ToLower().Equals(rawTable.Rows[j]["Quality  *"].ToString().Trim().ToLower())).FirstOrDefault();                        

                if (row != null)
                {
                    rawTable.Rows[j]["Collection *"] = row.collectionId;
                    rawTable.Rows[j]["Quality  *"] = row.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify collection, quality";
                    rawTable.Rows[j].Delete();
                }
            }
            rawTable.AcceptChanges();

            return rawTable;
        }
    }
}
