using System;

namespace Application.AcmeImportExport.Queries.Get
{
    public class AcmeCsvFieldDto
    {
        //Csv fields
        public string FieldName { get; set; }
        public int FieldIndex { get; set; }

        public Type FieldDataType { get; set; }
    }
}
