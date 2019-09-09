using Bytescout.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace IMSWebApi.Common
{
    public class DataTableHelper
    {
        string TimeStamp = string.Empty;
        string fileExtension = string.Empty;
        /// <summary>
        /// preparing data table from excel file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public DataTable PrepareDataTable(HttpPostedFileBase file)
        {
            TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            fileExtension = Path.GetExtension(file.FileName);

            var fileName = string.Concat(
            Path.GetFileNameWithoutExtension(file.FileName),
            TimeStamp,
            Path.GetExtension(file.FileName)
            );

            var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/ExcelUpload"), fileName);

            DataTable dataTable = new DataTable();

            Spreadsheet document = new Spreadsheet();

            //string saveFolder = @"D:\uploads"; //Pick a folder on your machine to store the uploaded files

            //string filePath1 = Path.Combine(filePath, fileName);

            file.SaveAs(filePath);

            var temp = filePath;

            //var tempDirectory = filePath;
            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                document.LoadFromFile(filePath);
                dataTable = document.ExportToDataTable(0, true);
            }

            removeEmptyColumnsAndRows(ref dataTable);

            return dataTable;
        }

        /// <summary>
        /// Converting invalid data to excel sheet
        /// </summary>
        /// <param name="invalidData"></param>
        /// <returns></returns>
        public string ConvertToExcel(DataTable dataTable, bool isInvalid)
        {
            //Spreadsheet scc = new Spreadsheet();

            //scc.Workbook.Worksheets.Add("Logs");
            var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/ExcelUpload"));
            //string saveLocation = @"D:\uploads\";
            //string fileName = "excel.xls";


            //scc.ImportFromDataTable(dataTable, scc.Workbook.Worksheets.IndexOf("Logs"));
            //scc.SaveAs(saveLocation+ "excel.xls");

            var InvalidfileName = string.Concat(
            "Invalidexcel_",
            TimeStamp, fileExtension
            );

            var ValidfileName = string.Concat(
            "Validexcel_",
            TimeStamp, fileExtension
            );

            var lines = new List<string>();
            string[] columnNames = dataTable.Columns.Cast<DataColumn>().
                                  Select(column => column.ColumnName).
                                  ToArray();

            var header = string.Join(",", columnNames);
            lines.Add(header);

            var valueLines = dataTable.AsEnumerable()
                               .Select(row => string.Join(",", row.ItemArray));
            lines.AddRange(valueLines);

            string filewritpath = string.Concat(filePath, "\\", InvalidfileName);
            string filewritpathforValid = string.Concat(filePath, "\\", ValidfileName);

            if (isInvalid)
            {
                //File.OpenWrite(filewritpath);
                //File.WriteAllLines(filePath + InvalidfileName, lines);
                // File.WriteAllLines(filewritpath, lines);

                using (StreamWriter writer = new StreamWriter(filewritpath, true))
                {
                    foreach (var line in lines)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            else
            {
                //File.WriteAllLines(filePath + ValidfileName, lines);
                using (StreamWriter writer = new StreamWriter(filewritpathforValid, true))
                {
                    foreach (var line in lines)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            return InvalidfileName;
        }

        public void removeEmptyColumnsAndRows(ref DataTable datatable)
        {
            // Remove Empty Columns
            foreach (DataColumn col in datatable.Columns.Cast<DataColumn>().ToArray())
            {
                if (string.IsNullOrWhiteSpace(datatable.Columns[col.ColumnName].Caption))
                {
                    datatable.Columns.Remove(col);
                }
            }
            datatable.AcceptChanges();

            //Remove Empty Rows
            for (int rowNum = 0; rowNum < datatable.Rows.Count; rowNum++)
            {
                if (datatable.Rows[rowNum].IsNull(0))
                {
                    datatable.Rows[rowNum].Delete();
                }
            }
            datatable.AcceptChanges();
        }
    }
}