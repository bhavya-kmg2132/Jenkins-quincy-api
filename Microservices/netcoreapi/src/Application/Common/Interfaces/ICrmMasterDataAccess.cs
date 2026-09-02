using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Application.Common.Interfaces
{
    public interface ICrmMasterDataAccess
    {
        Task<(string JoinClause, string WhereClause)> BuildWhereClause(JObject filters, string moduleName, string searchText);
        Dictionary<string, string> LoadSqlQueries(string filePath);
    }

}
