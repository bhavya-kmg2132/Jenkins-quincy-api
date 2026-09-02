using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.AcmeImportExport.Queries.Get;
using Application.Common.Interfaces;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;

namespace Infrastructure.Files
{
    public class AcmeProductFileLocation : IAcmeProductFileReaderWriter
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Instantiation of FileReaderWriter class
        /// </summary>
        /// <param name="configuration"></param>
        public AcmeProductFileLocation(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Save file in shared location
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public async Task<AcmeImportFileMetaDataDto> SavePostedFileInSharedLocation(IFormFile postedFile, string path)
        {
            AcmeImportFileMetaDataDto fileMetaData = new AcmeImportFileMetaDataDto();
            try
            {
                string csvFileLocation = _configuration.GetSection("ImportProspectFileLoction").GetSection("Path").Value;
                fileMetaData.FileExtension = Path.GetExtension(postedFile.FileName);
                fileMetaData.FileDisplayName = postedFile.FileName;
                fileMetaData.FileId = DateTime.Now.ToString("yyyy-MM-dd") + "_" + Convert.ToString(Guid.NewGuid());

                string filePathWithFileExt = path + fileMetaData.FileId + fileMetaData.FileExtension;

                while (File.Exists(filePathWithFileExt))
                {
                    fileMetaData.FileId = DateTime.Now.ToString("yyyy-MM-dd") + "_" + Guid.NewGuid();
                }

                fileMetaData.FilePath = csvFileLocation;

                if (fileMetaData.FileExtension == ".xlsx" || fileMetaData.FileExtension == ".xls")
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using (Stream fileStream = new FileStream(filePathWithFileExt, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(fileStream);
                    }

                    using (var stream = new FileStream(filePathWithFileExt, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        IExcelDataReader reader = null;
                        if (filePathWithFileExt.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (filePathWithFileExt.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }

                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = false
                            }
                        });

                        //Convert Excel to Csv file
                        var csvContent = string.Empty;
                        int row_no = 0;
                        while (row_no < dataSet.Tables[0].Rows.Count)
                        {
                            var arr = new List<string>();
                            for (int i = 0; i < dataSet.Tables[0].Columns.Count; i++)
                            {
                                arr.Add(dataSet.Tables[0].Rows[row_no][i].ToString());
                            }
                            row_no++;
                            csvContent += string.Join(",", arr) + "\n";
                        }

                        string filePathWithFileExtcsv = path + fileMetaData.FileId + ".csv";
                        using (Stream fileStream = new FileStream(filePathWithFileExtcsv, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(fileStream);
                        }
                        StreamWriter csv = new StreamWriter(filePathWithFileExtcsv);
                        csv.Write(csvContent);
                        csv.Close();

                        fileMetaData.FilePath = csvFileLocation;
                    }
                }
                else
                {
                    using (Stream fileStream = new FileStream(filePathWithFileExt, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(fileStream);
                    }
                    fileMetaData.FilePath = csvFileLocation;
                }
            }
            catch (Exception ex)
            {
                throw ex.GetBaseException();
            }
            return fileMetaData;
        }

        /// <summary>
        /// Read Csv file fields
        /// </summary>
        /// <param name="fileMetaData"></param>
        /// <returns></returns>
        public AcmeImportFileMetaDataDto ReadCsvFields(AcmeImportFileMetaDataDto fileMetaData, string path)
        {
            string filePathWithFileExt = path + ".csv";
            switch (fileMetaData.FileExtension)
            {
                case ".csv":
                    fileMetaData.CsvField = ReadTextDataColumns(filePathWithFileExt, ',');
                    break;
                case ".xls":
                    fileMetaData.CsvField = ReadTextDataColumns(filePathWithFileExt, ',');
                    break;
                case ".xlsx":
                    fileMetaData.CsvField = ReadTextDataColumns(filePathWithFileExt, ',');
                    break;
            }

            return fileMetaData;
        }


        /// <summary>
        /// Read Data from columns
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="columnSeperator"></param>
        /// <returns></returns>
        public List<AcmeCsvFieldDto> ReadTextDataColumns(string filePath, char columnSeperator)
        {
            List<AcmeCsvFieldDto> colList = new List<AcmeCsvFieldDto>();

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
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

            return colList;
        }

        /// <summary>
        /// Read file with DataTable Fields
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="columnSeperator"></param>
        /// <returns></returns>
        public DataTable ReadFileInDataTable(string filePath, char columnSeperator)
        {
            DataTable dt = new DataTable();

            if (File.Exists(filePath))
            {
                dt = GetDataTableFromCSVFile(filePath);
            }

            return dt;
        }

        /// <summary>
        /// convert csv file into dataTable without comma split
        /// </summary>
        /// <param name="csv_file_path"></param>
        /// <returns></returns>
        private static DataTable GetDataTableFromCSVFile(string csv_file_path)
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
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }

                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception)
            {
            }
            return csvData;
        }

    }
}
