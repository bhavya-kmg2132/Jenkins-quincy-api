using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Newtonsoft.Json.Linq;

namespace Application.Common.Interfaces
{
    public interface IMasterDataAccess
    {
        Task<string> BuildWhereClause(JObject filters, string moduleName, string searchText);
        Task<List<GenericMasterList>> GetFilterGenericMasterList(List<string> TypeList, List<string> GroupList);
        Task<List<GenericMasterList>> GetGenericMaster();
    }
}
