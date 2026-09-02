using System.Data;
using System.Threading.Tasks;
using Application.AcmeImportExport.Queries.Get;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces
{
    public interface IAcmeProductFileReaderWriter
    {

        Task<AcmeImportFileMetaDataDto> SavePostedFileInSharedLocation(IFormFile postedFile, string path);
        //Task<GetAcmeProductDetailsDto> SavePostedFileInSharedLocation(IFormFile postedFile, string path);

        AcmeImportFileMetaDataDto ReadCsvFields(AcmeImportFileMetaDataDto fileMetaData, string path);

        //List<CsvFieldDto> ReadTextDataColumns(string filePath, char columnSeperator);

        DataTable ReadFileInDataTable(string filePath, char columnSeperator);
    }
}
