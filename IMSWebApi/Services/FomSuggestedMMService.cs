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
    public class FomSuggestedMMService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        CategoryService _categoryService = null;
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public FomSuggestedMMService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMFomSuggestedMMList> getFomSuggestedMMs(int pageSize, int page, string search)
        {
            List<VMFomSuggestedMMList> fomSuggestedMMListView;
            fomSuggestedMMListView= repo.MstFomSuggestedMMs.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstFomDensity.density.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search) 
                    || q.suggestedMM.ToString().StartsWith(search): true)
                     .Select(f => new VMFomSuggestedMMList
                     {
                         id = f.id,
                         collectionCode = f.MstCollection != null ? f.MstCollection.collectionCode : string.Empty,
                         qualityCode = f.MstQuality != null ? f.MstQuality.qualityCode : string.Empty,
                         density = f.MstFomDensity != null ? f.MstFomDensity.density : string.Empty,
                         suggestedMM = f.suggestedMM
                     })
                    .OrderBy(q => q.collectionCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMFomSuggestedMMList>
            {
                Data = fomSuggestedMMListView,
                TotalCount = repo.MstFomSuggestedMMs.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstFomDensity.density.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.suggestedMM.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFomSuggestedMM getFomSuggestedMMById(Int64 id)
        {
            var result = repo.MstFomSuggestedMMs.Where(q => q.id == id).FirstOrDefault();
            var fomSuggestedMMView = Mapper.Map<MstFomSuggestedMM, VMFomSuggestedMM>(result);
            return fomSuggestedMMView;
        }

        public List<VMLookUpItem> getFomSuggestedMMLookUpByFomDensity(Int64 fomDensityId)
        {
            return repo.MstFomSuggestedMMs.Where(q => q.fomDensityId == fomDensityId)
                .OrderBy(q => q.suggestedMM)
                .Select(q => new VMLookUpItem { value = q.id, label = q.suggestedMM.ToString() })
                .ToList();
        }

        public ResponseMessage postFomSuggestedMM(VMFomSuggestedMM fomSuggestedMM)
        {
            MstFomSuggestedMM fomSuggestedMMToPost = Mapper.Map<VMFomSuggestedMM, MstFomSuggestedMM>(fomSuggestedMM);
            fomSuggestedMMToPost.categoryId = _categoryService.getFoamCategory().id;
            fomSuggestedMMToPost.createdOn = DateTime.Now;
            fomSuggestedMMToPost.createdBy = _LoggedInuserId;

            repo.MstFomSuggestedMMs.Add(fomSuggestedMMToPost);
            repo.SaveChanges();
            return new ResponseMessage(fomSuggestedMMToPost.id, resourceManager.GetString("FomSuggestedMMAdded"), ResponseType.Success);
        }

        public ResponseMessage putFomSuggestedMM(VMFomSuggestedMM fomSuggestedMM)
        {
            var fomSuggestedMMToPut = repo.MstFomSuggestedMMs.Where(q => q.id == fomSuggestedMM.id).FirstOrDefault();

            //fomSuggestedMMToPut.categoryId = fomSuggestedMM.categoryId;
            fomSuggestedMMToPut.collectionId = fomSuggestedMM.collectionId;
            fomSuggestedMMToPut.qualityId = fomSuggestedMM.qualityId;
            fomSuggestedMMToPut.fomDensityId = fomSuggestedMM.fomDensityId;
            fomSuggestedMMToPut.suggestedMM = fomSuggestedMM.suggestedMM;

            fomSuggestedMMToPut.updatedBy = _LoggedInuserId;
            fomSuggestedMMToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(fomSuggestedMMToPut.id, resourceManager.GetString("FomSuggestedMMUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteFomSuggestedMM(Int64 id)
        {
            repo.MstFomSuggestedMMs.Remove(repo.MstFomSuggestedMMs.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("FomSuggestedMMDeleted"), ResponseType.Success);
        }

         /// <summary>
        /// uploading design excel sheet
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Tuple<string,int> UploadFoamSuggestedMM(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable suggestedMMdt = new DataTable();
            suggestedMMdt = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable InvalidData = new DataTable();

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateDataTable(suggestedMMdt, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Collection *"].SetOrdinal(0);
            validatedDataTable.Columns["Quality  *"].SetOrdinal(1);
            validatedDataTable.Columns["Foam Density *"].SetOrdinal(2);
            validatedDataTable.Columns["Suggested MM *"].SetOrdinal(3);

            validatedDataTable.AcceptChanges();

            //SqlParameter param = new SqlParameter("@FoamSuggestedMMType", SqlDbType.Structured);
            //param.TypeName = "dbo.FoamSuggestedMMType";
            //param.Value = validatedDataTable;

            //using (WebAPIdbEntities db = new WebAPIdbEntities())
            //{
            //    var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFoamSuggestedMM @FoamSuggestedMMType", param);
            //}

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadFoamSuggestedMM", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@FoamSuggestedMMType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Suggested MM *") == row["suggestedMM"].ToString()).FirstOrDefault());
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
            var model = new VMFomSuggestedMM();

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
                model.collectionId = model.qualityId = model.fomDensityId = 1;
                model.suggestedMM = !string.IsNullOrWhiteSpace(row["Suggested MM *"].ToString()) ? Convert.ToInt32(row["Suggested MM *"]) : 0;
               
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

            //finding distinct values of density
            List<string> foamDensity = new List<string>();
            foamDensity = rawTable.AsEnumerable().Select(c => c.Field<string>("Foam Density *")).Distinct().ToList();

            //formatting as comma separated string
            string densityvalues = (string.Join(",", foamDensity.ToArray()));

            //fetching data for density            
            var desnitykeys = repo.GET_DENSITY_ID(densityvalues).ToList();

            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = desnitykeys.Where(d => d.MstCollection.collectionCode.ToLower().Equals(rawTable.Rows[j]["Collection *"].ToString().Trim().ToLower())
                                    && d.MstQuality.qualityCode.ToLower().Equals(rawTable.Rows[j]["Quality  *"].ToString().Trim().ToLower())
                                    && d.density.ToString().ToLower().Trim().Equals(rawTable.Rows[j]["Foam Density *"].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null)
                {
                    rawTable.Rows[j]["Collection *"] = row.collectionId;
                    rawTable.Rows[j]["Quality  *"] = row.qualityId;
                    rawTable.Rows[j]["Foam Density *"] = row.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify collection, quality and foam density";
                    rawTable.Rows[j].Delete();
                }
            }
            rawTable.AcceptChanges();

            return rawTable;
        }
    }
}