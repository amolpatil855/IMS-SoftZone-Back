using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;
using System.Transactions;
using IMSWebApi.Enums;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Configuration;
using IMSWebApi.ViewModel.UploadVM;
using System.IO;
using System.Net.Http;
using System.Net;
using System.ComponentModel;

namespace IMSWebApi.Services
{
    public class TailorService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public TailorService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMTailorList> getTailors(int pageSize, int page, string search)
        {
            List<VMTailorList> tailorListingView;
            tailorListingView = repo.MstTailors.Where(t => !string.IsNullOrEmpty(search)
                    ? t.name.ToString().StartsWith(search)
                    || t.phone.StartsWith(search)
                    || t.email.StartsWith(search)
                    || t.city.StartsWith(search) : true)
                    .Select(c => new VMTailorList
                    {
                        id = c.id,
                        name = c.name,
                        phone = c.phone,
                        email = c.email,
                        city = c.city
                    })
                    .OrderByDescending(o => o.id).Skip(page * pageSize).Take(pageSize).ToList();
            return new ListResult<VMTailorList>
            {
                Data = tailorListingView,
                TotalCount = repo.MstTailors.Where(t => !string.IsNullOrEmpty(search)
                    ? t.name.ToString().StartsWith(search)
                    || t.phone.StartsWith(search)
                    || t.email.StartsWith(search)
                    || t.city.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTailor getTailorById(Int64 id)
        {
            var result = repo.MstTailors.Where(t => t.id == id).FirstOrDefault();
            VMTailor tailorViews = Mapper.Map<MstTailor, VMTailor>(result);
            tailorViews.MstTailorPatternChargeDetails.ForEach(tp => tp.MstTailor = null);
            return tailorViews;
        }

        public List<VMTailor> getAlltailors()
        {
            var tailor = repo.MstTailors.ToList();
            List<VMTailor> VMtailor = Mapper.Map<List<MstTailor>, List<VMTailor>>(tailor);
            VMtailor.ForEach(t => t.MstTailorPatternChargeDetails.ForEach(tpDetails => tpDetails.MstTailor = null));
            return VMtailor;
        }

        public List<VMLookUpItem> getTailorLookup()
        {
            return repo.MstTailors.Select(t => new VMLookUpItem
                                            {
                                                label = t.name,
                                                value = t.id
                                            }).ToList();
        }

        public ResponseMessage postTailor(VMTailor tailor)
        {
            using (var transaction = new TransactionScope())
            {
                MstTailor tailorToPost = Mapper.Map<VMTailor, MstTailor>(tailor);
                List<MstTailorPatternChargeDetail> tailorPatternChargeDetails = tailorToPost.MstTailorPatternChargeDetails.ToList();
                foreach (var tpDetails in tailorPatternChargeDetails)
                {
                    tpDetails.createdOn = DateTime.Now;
                    tpDetails.createdBy = _LoggedInuserId;
                }
                tailorToPost.createdOn = DateTime.Now;
                tailorToPost.createdBy = _LoggedInuserId;
                repo.MstTailors.Add(tailorToPost);
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(tailorToPost.id, resourceManager.GetString("TailorAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putTailor(VMTailor tailor)
        {
            using (var transaction = new TransactionScope())
            {
                MstTailor tailorToPut = repo.MstTailors.Where(c => c.id == tailor.id).FirstOrDefault();
                tailorToPut.name = tailor.name;
                tailorToPut.email = tailor.email;
                tailorToPut.phone = tailor.phone;
                tailorToPut.alternatePhone1 = tailor.alternatePhone1;
                tailorToPut.addressLine1 = tailor.addressLine1;
                tailorToPut.addressLine2 = tailor.addressLine2;
                tailorToPut.city = tailor.city;
                tailorToPut.state = tailor.state;
                tailorToPut.country = tailor.country;
                tailorToPut.pin = tailor.pin;

                tailorToPut.updatedOn = DateTime.Now;
                tailorToPut.updatedBy = _LoggedInuserId;

                updateTailorPatternCharge(tailor);

                repo.SaveChanges();

                transaction.Complete();
                return new ResponseMessage(tailor.id, resourceManager.GetString("TailorUpdated"), ResponseType.Success);
            }
        }

        public void updateTailorPatternCharge(VMTailor tailor)
        {
            MstTailor tailorToPut = repo.MstTailors.Where(c => c.id == tailor.id).FirstOrDefault();

            List<MstTailorPatternChargeDetail> patternToRemove = new List<MstTailorPatternChargeDetail>();

            foreach (var tpDetail in tailorToPut.MstTailorPatternChargeDetails)
            {
                if (tailor.MstTailorPatternChargeDetails.Any(y => y.id == tpDetail.id))
                {
                    continue;
                }
                else
                {
                    patternToRemove.Add(tpDetail);
                }
            }

            repo.MstTailorPatternChargeDetails.RemoveRange(patternToRemove);
            repo.SaveChanges();

            tailor.MstTailorPatternChargeDetails.ForEach(x =>
            {
                if (tailorToPut.MstTailorPatternChargeDetails.Any(y => y.id == x.id))
                {
                    var patternChargeDetailToPut = repo.MstTailorPatternChargeDetails.Where(p => p.id == x.id).FirstOrDefault();

                    patternChargeDetailToPut.patternId = x.patternId;
                    patternChargeDetailToPut.charge = x.charge;
                    patternChargeDetailToPut.updatedOn = DateTime.Now;
                    patternChargeDetailToPut.updatedBy = _LoggedInuserId;
                    repo.SaveChanges();
                }
                else
                {
                    MstTailorPatternChargeDetail patternChargeDetails = Mapper.Map<VMTailorPatternChargeDetail, MstTailorPatternChargeDetail>(x);
                    patternChargeDetails.tailorId = tailor.id;
                    patternChargeDetails.createdBy = _LoggedInuserId;
                    patternChargeDetails.createdOn = DateTime.Now;
                    repo.MstTailorPatternChargeDetails.Add(patternChargeDetails);
                    repo.SaveChanges();
                }
            });
        }

        public ResponseMessage deleteTailor(Int64 id)
        {
            using (var transaction = new TransactionScope())
            {
                MstTailor tailorToDelete = repo.MstTailors.Where(t => t.id == id).FirstOrDefault();
                //if (repo.TrnSaleOrders.Where(so => so.customerId == id).Count() > 0)
                //{
                //    transaction.Complete();
                //    return new ResponseMessage(id, resourceManager.GetString("CustomerNotDeleted"), ResponseType.Error);
                //}
                //repo.MstTailorPatternChargeDetails.RemoveRange(repo.MstTailorPatternChargeDetails.Where(s => s.tailorId == id).ToList());
                repo.MstTailors.Remove(repo.MstTailors.Where(t => t.id == id).FirstOrDefault());
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("TailorDeleted"), ResponseType.Success);
            }
        }

        /// <summary>
        /// uploading design excel sheet
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Tuple<string, int> UploadTailor(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;            

            DataTable tailor = new DataTable();
            tailor = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable InvalidData = new DataTable();

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateDataTable(tailor, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Name*"].SetOrdinal(0);
            validatedDataTable.Columns["Email"].SetOrdinal(1);
            validatedDataTable.Columns["Phone*"].SetOrdinal(2);
            validatedDataTable.Columns["Alternate Phone 1"].SetOrdinal(3);
            validatedDataTable.Columns["Address Line 1*"].SetOrdinal(4);
            validatedDataTable.Columns["Address Line 2"].SetOrdinal(5);
            validatedDataTable.Columns["City*"].SetOrdinal(6);
            validatedDataTable.Columns["Pin Code*"].SetOrdinal(7);
            validatedDataTable.Columns["State*"].SetOrdinal(8);

            validatedDataTable.AcceptChanges();


            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadTailor", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@TailorsType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Phone*") == row["phone"].ToString()).FirstOrDefault());
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
            var model = new VMTailor();

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
                model.name = row["Name*"].ToString();
                model.email = row["Email"].ToString();
                model.phone = row["Phone*"].ToString();
                model.alternatePhone1 = row["Alternate Phone 1"].ToString();
                model.addressLine1 = row["Address Line 1*"].ToString();
                model.addressLine2 = row["Address Line 2"].ToString();
                model.city = row["City*"].ToString();
                model.state = row["State*"].ToString();
                model.pin = row["Pin Code*"].ToString();

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

            return rawTable;
        }

        /// <summary>
        /// uploading design excel sheet
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Tuple<string,int> UploadTailorPatternDetails(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable pattern = new DataTable();
            pattern = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable InvalidData = new DataTable();

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidatePatternDataTable(pattern, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Tailor Name*"].SetOrdinal(0);
            validatedDataTable.Columns["Pattern Name*"].SetOrdinal(1);
            validatedDataTable.Columns["Charge*"].SetOrdinal(2);

            validatedDataTable.AcceptChanges();

            //SqlParameter param = new SqlParameter("@TailorPatternChargesType", SqlDbType.Structured);
            //param.TypeName = "dbo.TailorPatternChargesType";
            //param.Value = validatedDataTable;

            //using (WebAPIdbEntities db = new WebAPIdbEntities())
            //{
            //    var i = db.Database.ExecuteSqlCommand("exec dbo.UploadTailorPatternCharges @TailorPatternChargesType", param);
            //}

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadTailorPatternCharges", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@TailorPatternChargesType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Charge*") == row["charge"].ToString()).FirstOrDefault());
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

        private DataTable ValidatePatternDataTable(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new VMPatternForTailor();

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
                model.tailorId = model.patternId = 1;
                model.charge = !string.IsNullOrWhiteSpace(row["Charge*"].ToString()) ? Convert.ToDecimal(row["Charge*"]) : 0;

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

            //finding distinct values of tailor and pattern
            List<string> tailorname = new List<string>();
            tailorname = rawTable.AsEnumerable().Select(c => c.Field<string>("Tailor Name*")).Distinct().ToList();

            List<string> patternname = new List<string>();
            patternname = rawTable.AsEnumerable().Select(c => c.Field<string>("Pattern Name*")).Distinct().ToList();

            //formatting as comma separated string
            string tailorstring = (string.Join(",", tailorname.ToArray()));
            string patternstring = (string.Join(",", patternname.ToArray()));

            //fetching data for tailor and pattern            
            var tailor = repo.GET_TAILOR_ID(tailorstring).ToList();
            var pattern = repo.GET_PATTERN_ID(patternstring).ToList();

            //replacing values of category, collection, quality by their ids         
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var tailordata = tailor.Where(d => d.name.ToLower().Equals(rawTable.Rows[j]["Tailor Name*"].ToString().Trim().ToLower())).FirstOrDefault();
                var patterndata = pattern.Where(d => d.name.ToLower().Equals(rawTable.Rows[j]["Pattern Name*"].ToString().Trim().ToLower())).FirstOrDefault();

                if (tailordata != null && patterndata != null)
                {
                    rawTable.Rows[j]["Tailor Name*"] = tailordata.id;
                    rawTable.Rows[j]["Pattern Name*"] = patterndata.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify tailor name, pattern name";
                    rawTable.Rows[j].Delete();
                }
            }
            rawTable.AcceptChanges();

            return rawTable;
        }

    }
}