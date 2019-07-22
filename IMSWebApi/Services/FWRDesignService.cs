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

namespace IMSWebApi.Services
{
    public class FWRDesignService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public FWRDesignService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMFWRDesignList> getDesigns(int pageSize, int page, string search)
        {
            List<VMFWRDesignList> designListView;
            designListView = repo.MstFWRDesigns.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.designCode.StartsWith(search)
                    || q.designName.StartsWith(search) : true)
                    .Select(d => new VMFWRDesignList
                    {
                        id = d.id,
                        categoryId = d.categoryId,
                        categoryCode = d.MstCategory != null ? d.MstCategory.code : string.Empty,
                        collectionCode = d.MstCollection != null ? d.MstCollection.collectionCode : string.Empty,
                        qualityCode = d.MstQuality != null ? d.MstQuality.qualityCode : string.Empty,
                        designCode = d.designCode,
                        designName = d.designName
                    })
                    .OrderBy(q => q.categoryId).ThenBy(q => q.collectionCode)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMFWRDesignList>
            {
                Data = designListView,
                TotalCount = repo.MstFWRDesigns.Where(q => !string.IsNullOrEmpty(search)
                     ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstQuality.qualityCode.StartsWith(search)
                    || q.designCode.StartsWith(search)
                    || q.designName.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMFWRDesign getDesignById(Int64 id)
        {
            var result = repo.MstFWRDesigns.Where(q => q.id == id).FirstOrDefault();
            var designView = Mapper.Map<MstFWRDesign, VMFWRDesign>(result);
            return designView;
        }

        public List<VMLookUpItem> getDesignLookUpByQuality(Int64 qualityId)
        {
            return repo.MstFWRDesigns.Where(q => q.qualityId == qualityId)
                .OrderBy(s => s.designCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.designCode }).ToList();
        }

        public ResponseMessage postDesign(VMFWRDesign design)
        {
            var designToPost = Mapper.Map<VMFWRDesign, MstFWRDesign>(design);
            designToPost.createdOn = DateTime.Now;
            designToPost.createdBy = _LoggedInuserId;

            repo.MstFWRDesigns.Add(designToPost);
            repo.SaveChanges();
            return new ResponseMessage(designToPost.id, resourceManager.GetString("FWRDesignAdded"), ResponseType.Success);
        }

        public ResponseMessage putDesign(VMFWRDesign design)
        {
            var designToPut = repo.MstFWRDesigns.Where(q => q.id == design.id).FirstOrDefault();

            designToPut.categoryId = design.categoryId;
            designToPut.collectionId = design.collectionId;
            designToPut.qualityId = design.qualityId;
            designToPut.designCode = design.designCode;
            designToPut.designName = design.designName;
            designToPut.description = design.description;

            designToPut.updatedBy = _LoggedInuserId;
            designToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(design.id, resourceManager.GetString("FWRDesignUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteDesign(Int64 id)
        {
            repo.MstFWRDesigns.Remove(repo.MstFWRDesigns.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("FWRDesignDeleted"), ResponseType.Success);
        }

        /// <summary>
        /// uploading design excel sheet
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int UploadDesign(HttpPostedFileBase file)
        {
            DataTable designDatatable = new DataTable();
            designDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateDataTable(designDatatable); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Category *"].SetOrdinal(0);
            validatedDataTable.Columns["Collection *"].SetOrdinal(1);
            validatedDataTable.Columns["Quality* "].SetOrdinal(2);
            validatedDataTable.Columns["Design Code* "].SetOrdinal(3);
            validatedDataTable.Columns["Design Name* "].SetOrdinal(4);
            validatedDataTable.Columns["Description "].SetOrdinal(5);

            validatedDataTable.AcceptChanges();

            SqlParameter param = new SqlParameter("@FWRDesignType", SqlDbType.Structured);
            param.TypeName = "dbo.FWRDesignType";
            param.Value = validatedDataTable;

            using (WebAPIdbEntities db = new WebAPIdbEntities())
            {
                var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFWRDesign @FWRDesignType", param);
            }

            return 0;
        }


        /// <summary>
        /// validate data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateDataTable(DataTable rawTable)
        {
            var model = new VMFWRDesign();

            //datatable for invalid rows
            DataTable InvalidData = new DataTable();
            InvalidData.Columns.Add("Category");
            InvalidData.Columns.Add("Collection");
            InvalidData.Columns.Add("Quality");
            InvalidData.Columns.Add("Design Code");
            InvalidData.Columns.Add("Design Name");
            InvalidData.Columns.Add("Description");
            InvalidData.Columns.Add("Reason");

            //setting column name as its caption name
            foreach (DataColumn col in rawTable.Columns)
            {
                string colname = rawTable.Columns[col.ColumnName].Caption;
                col.ColumnName = colname;
            }
            rawTable.AcceptChanges();

            //first validating without keys
            foreach (DataRow row in rawTable.Rows)
            {
                model.categoryId = model.collectionId = model.qualityId = 1;
                model.designCode = row["Design Code* "].ToString();
                model.designName = row["Design Name* "].ToString();
                model.description = row["Description "].ToString();

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

            //finding distinct values of Design codes
            List<string> Design = new List<string>();
            Design = rawTable.AsEnumerable().Select(c => c.Field<string>("Design Code* ")).Distinct().ToList();

            //formatting as comma separated string
            string designString = (string.Join(",", Design.ToArray()));

            //fetching data for design            
            var designKey = repo.GET_DESIGN_ID(designString).ToList();

            //replacing values of category, collection, quality by their ids         
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = designKey.Where(d => d.MstCategory.code.ToLower().Equals(rawTable.Rows[j]["Category *"].ToString().Trim().ToLower())
                                    && d.MstCollection.collectionName.ToLower().Equals(rawTable.Rows[j]["Collection *"].ToString().Trim().ToLower())
                                    && d.MstQuality.qualityCode.ToLower().Equals(rawTable.Rows[j]["Quality* "].ToString().Trim().ToLower())
                                    && d.designCode.ToLower().Equals(rawTable.Rows[j]["Design Code* "].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null)
                {
                    rawTable.Rows[j]["Category *"] = row.categoryId;
                    rawTable.Rows[j]["Collection *"] = row.collectionId;
                    rawTable.Rows[j]["Quality* "] = row.qualityId;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify category, collection, quality";
                    rawTable.Rows[j].Delete();
                }
            }


            rawTable.AcceptChanges();

            //if contains invalid data then convert to Excel 
            if (InvalidData != null)
            {
                datatable_helper.ConvertToExcel(InvalidData, true);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(rawTable, false);

            return rawTable;
        }
    }
}