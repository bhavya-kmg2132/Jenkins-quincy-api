using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IFileReaderWriter
    {
        Task<string> GetImportCsvFileType(string filePath);
        Task<DataTable> ReadFileInDataTable(string filePath, char columnSeperator);
        DataTable ToDataTable<T>(List<T> items);
        Task<string[]> ReadCsvFileLine(string path);
        Task<string[]> ReadTopTwoCsvFileLine(string path);
        Task<bool> WriteAllLinesInCsvFile(string path, IEnumerable<string> lines);
        //Task<bool> WriteAllLinesInCsvFile(string path, string[] lines, List<ErrorMessage> errors);
    }
}
