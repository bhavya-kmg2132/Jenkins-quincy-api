using System;
using System.Text.Json.Serialization;

namespace Application.AcmeImportExport.Queries.Get
{
    public class CsvMachTemplateDto
    {
        public string FieldName { get; set; }
        public int FieldIndex { get; set; }

        [JsonIgnore]
        public Type FieldDataType { get; set; }
    }
}

