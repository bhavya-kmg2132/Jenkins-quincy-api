
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Application.AcmeImportExport.Queries.Get;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;

namespace Infrastructure.Files
{
    public class FileReaderWriter : IFileReaderWriter
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Instantiation of FileReaderWriter class
        /// </summary>
        /// <param name="configuration"></param>
        public FileReaderWriter(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Read csv file in dataTable
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public async Task<DataTable> ReadFileInDataTableFromFile(IFormFile postedFile)
        {
            DataTable dt = new DataTable();

            using (var reader = new StreamReader(postedFile.OpenReadStream()))
            {
                if (reader != null && reader.Peek() > 0)
                {
                    var data = reader.ReadLineAsync();
                    var columns = data.Result.Split(',').ToList();

                    foreach (var c in columns)
                    {
                        dt.Columns.Add(c);
                    }

                    while (reader.Peek() >= 0)
                    {
                        var row = reader.ReadLineAsync().Result.Split(',').Select(a => string.IsNullOrEmpty(a) ? null : a).ToArray();
                        dt.Rows.Add(row);
                    }
                }
            }
            return await Task.FromResult<DataTable>(dt);
        }

        /// <summary>
        /// convert csv file into dataTable without comma split
        /// </summary>
        /// <param name="csv_file_path"></param>
        /// <returns></returns>
        public async Task<DataTable> GetDataTableFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();

            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        if (csvData.Columns.Contains(column) == false && column != "")
                        {
                            DataColumn datecolumn = new DataColumn(column);
                            datecolumn.AllowDBNull = true;

                            csvData.Columns.Add(datecolumn);
                        }
                    }

                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        string[] fieldData1 = new string[csvData.Columns.Count];
                        //Making empty value as null
                        for (int i = 0; i <= csvData.Columns.Count - 1; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                            else
                            {
                                fieldData1[i] = fieldData[i];
                            }
                        }
                        if (fieldData1.All(y => y == null) == false)
                            csvData.Rows.Add(fieldData1);
                    }
                }
            }
            catch (Exception)
            {
            }
            return await Task.FromResult<DataTable>(csvData);
        }


        /// <summary>
        /// Read Data from columns
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="columnSeperator"></param>
        /// <returns></returns>
        public async Task<List<AcmeCsvFieldDto>> ReadTextDataColumns(string filePath, char columnSeperator)
        {
            List<AcmeCsvFieldDto> colList = new List<AcmeCsvFieldDto>();
            List<string> lineList = new List<string>();
            if (File.Exists(filePath))
            {
                var lines = await ReadTopTwoCsvFileLine(filePath);
                foreach (var line in lines)
                {
                    var word = line.Split(',')
                                  .Select(x => x.Replace("\"", "").Trim())
                                  .Where(x => !string.IsNullOrWhiteSpace(x))
                                  .ToList();
                    lineList.Add(String.Join(",", word));
                }

                lines = lineList.ToArray();

                if (lines != null && lines.Count() > 0)
                {
                    var fileColumnList = lines[0].Split(columnSeperator).ToList();
                    if (fileColumnList != null && fileColumnList.Count > 0)
                    {
                        for (int i = 0; i <= fileColumnList.Count - 1; i++)
                        {
                            colList.Add(new AcmeCsvFieldDto() { FieldIndex = i, FieldName = fileColumnList[i] });
                        }
                    }
                }
            }

            return await Task.FromResult<List<AcmeCsvFieldDto>>(colList);
        }

        /// <summary>
        /// Read file with DataTable Fields
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="columnSeperator"></param>
        /// <returns></returns>
        public async Task<DataTable> ReadFileInDataTable(string filePath, char columnSeperator)
        {
            DataTable dt = new DataTable();

            if (File.Exists(filePath))
            {
                dt = await GetDataTableFromCSVFile(filePath);
            }

            return dt;
        }

        /// <summary>
        /// Get type of import file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<string> GetImportCsvFileType(string filePath)
        {
            DataTable prospectRequiredFields = new DataTable();
            string FileType = "";
            if (File.Exists(filePath))
            {
                prospectRequiredFields = await GetDataTableFromCSVFile(filePath);
                DataColumn dcProspectId = new DataColumn();
                dcProspectId.ColumnName = "ProspectId";
                DataColumn dcProspectGuidId = new DataColumn();
                dcProspectGuidId.ColumnName = "ProspectGuidId";
                DataColumn fileTypeImport = new DataColumn();
                fileTypeImport.ColumnName = "Import";
                DataColumn fileTypeExport = new DataColumn();
                fileTypeExport.ColumnName = "Export";

                if (prospectRequiredFields.Columns.Contains(dcProspectId.ColumnName) == false && prospectRequiredFields.Columns.Contains(dcProspectGuidId.ColumnName) == false)
                {
                    FileType = fileTypeImport.ColumnName;
                }
                else if (prospectRequiredFields.Columns.Contains(dcProspectId.ColumnName) == true && prospectRequiredFields.Columns.Contains(dcProspectGuidId.ColumnName) == true)
                {
                    var BlankValueRowsProspectId = from csvRowsValues in prospectRequiredFields.AsEnumerable()
                                                   where
                                                   Convert.ToString(csvRowsValues[dcProspectId.ColumnName]) == "0"
                                                   || string.IsNullOrEmpty(Convert.ToString(csvRowsValues[dcProspectId.ColumnName]))
                                                   select csvRowsValues;

                    var BlankValueRowsProspectGuid = from csvRowsValue in prospectRequiredFields.AsEnumerable()
                                                     where Convert.ToString(csvRowsValue[dcProspectGuidId.ColumnName]) == "0"
                                                     || string.IsNullOrEmpty(Convert.ToString(csvRowsValue[dcProspectGuidId.ColumnName]))
                                                     select csvRowsValue;

                    if (BlankValueRowsProspectId.Count() == prospectRequiredFields.Rows.Count && BlankValueRowsProspectGuid.Count() == prospectRequiredFields.Rows.Count)
                    {
                        FileType = fileTypeImport.ColumnName;
                    }
                    else
                        FileType = fileTypeExport.ColumnName;
                }
                else if (prospectRequiredFields.Columns.Contains(dcProspectId.ColumnName.ToLower()) == true && prospectRequiredFields.Columns.Contains(dcProspectGuidId.ColumnName.ToLower()) == true)
                {
                    var BlankValueRowsProspectId = from csvRowsValues in prospectRequiredFields.AsEnumerable()
                                                   where
                                                   Convert.ToString(csvRowsValues[dcProspectId.ColumnName.ToLower()]) == "0"
                                                   || string.IsNullOrEmpty(Convert.ToString(csvRowsValues[dcProspectId.ColumnName]))
                                                   select csvRowsValues;

                    var BlankValueRowsProspectGuid = from csvRowsValues in prospectRequiredFields.AsEnumerable()
                                                     where Convert.ToString(csvRowsValues[dcProspectGuidId.ColumnName.ToLower()]) == "0"
                                                     || string.IsNullOrEmpty(Convert.ToString(csvRowsValues[dcProspectGuidId.ColumnName.ToLower()]))
                                                     select csvRowsValues;

                    if (BlankValueRowsProspectId.Count() == prospectRequiredFields.Rows.Count && BlankValueRowsProspectGuid.Count() == prospectRequiredFields.Rows.Count)
                    {
                        FileType = fileTypeImport.ColumnName;
                    }
                    else
                        FileType = fileTypeExport.ColumnName;
                }
                else if (prospectRequiredFields.Columns.Contains(dcProspectId.ColumnName) == true && prospectRequiredFields.Columns.Contains(dcProspectGuidId.ColumnName) == false)
                {
                    FileType = fileTypeImport.ColumnName;
                }
                else if (prospectRequiredFields.Columns.Contains(dcProspectId.ColumnName) == false && prospectRequiredFields.Columns.Contains(dcProspectGuidId.ColumnName) == true)
                {
                    FileType = fileTypeImport.ColumnName;
                }
                else
                    FileType = fileTypeExport.ColumnName;
            }
            return FileType;
        }

        /// <summary>
        /// Convert list to datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        /// <summary>
        /// Read Csv file line's
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<string[]> ReadCsvFileLine(string path)
        {
            var lines = File.ReadAllLines(path);
            List<string> lineList = new List<string>();
            foreach (var line in lines)
            {
                var word = line.Split(',')
                              .Select(x => x.Replace("\"", "").Trim())
                              //.Where(x => !string.IsNullOrWhiteSpace(x))
                              .ToList();
                if (word.All(y => y == null || y == "") == false)
                    lineList.Add(String.Join(",", word));
            }

            lines = lineList.ToArray();


            //return File.ReadAllLines(path);
            return await Task.FromResult<string[]>(lines);
        }



        /// <summary>
        /// Read Csv file line's for only 2 rows
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<string[]> ReadTopTwoCsvFileLine(string path)
        {
            var lines = File.ReadAllLines(path);
            List<string> lineList = new List<string>();

            for (var i = 0; i < 2; i += 1)
            {
                var word = lines[i].Split(',')
                              .Select(x => x.Replace("\"", "").Trim())
                              //.Where(x => !string.IsNullOrWhiteSpace(x))
                              .ToList();
                if (word.All(y => y == null || y == "") == false)
                    lineList.Add(String.Join(",", word));
                // Process line
            }


            //foreach (var line in lines)
            //{
            //    var word = line.Split(',')
            //                  .Select(x => x.Replace("\"", "").Trim())
            //                  //.Where(x => !string.IsNullOrWhiteSpace(x))
            //                  .ToList();
            //    if (word.All(y => y == null || y == "") == false)
            //        lineList.Add(String.Join(",", word));
            //}

            lines = lineList.ToArray();


            //return File.ReadAllLines(path);
            return await Task.FromResult<string[]>(lines);
        }

        /// <summary>
        /// Write line's in Csv file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public async Task<bool> WriteAllLinesInCsvFile(string path, IEnumerable<string> lines)
        {
            try
            {
                File.WriteAllLines(path, lines);
                return await Task.FromResult<bool>(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Convert DataTable to csv
        /// </summary>
        /// <param name="dtDataTable"></param>
        /// <param name="strFilePath"></param>
        public static void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers    
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}
