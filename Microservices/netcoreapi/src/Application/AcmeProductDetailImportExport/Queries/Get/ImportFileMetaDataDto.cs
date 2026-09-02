using System.Collections.Generic;

namespace Application.AcmeImportExport.Queries.Get
{
    public class AcmeImportFileMetaDataDto
    {
        //Dto Fields
        public AcmeImportFileMetaDataDto()
        {
            CsvField = new List<AcmeCsvFieldDto>();
            CsvField.Add(new AcmeCsvFieldDto());
            CsvFieldMappings = new List<AcmeCsvMappedField>();
        }

        //Import file fields
        public string FileId { get; set; }
        public string FileDisplayName { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public List<AcmeCsvFieldDto> CsvField { get; set; }
        public List<AcmeCsvMappedField> CsvFieldMappings { get; set; }
        public bool IsImportAnyWay { get; set; } = false;
        public string FileSourceShortName { get; set; }
    }

    //Map fields
    public class AcmeCsvMappedField
    {
        public string CsvFieldIndex { get; set; }
        public string DatabaseColumnIndex { get; set; }
        public string CsvFieldName { get; set; }
        public string DatabaseColumnName { get; set; }
    }
}
