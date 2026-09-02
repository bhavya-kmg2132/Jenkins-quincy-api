using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Common;

namespace Application.Common.Interfaces
{
    public interface IPolicyDataAccess
    {
        Task<string> Add(Domain.Entities.Policy policy);
        Task<int> Update(Domain.Entities.Policy policy);
        Task<int> Delete(Domain.Entities.Policy policy);
        Task<int> PermanentDelete(string id);
        Task<Domain.Entities.Policy> GetPolicyById(string id);
        Task<List<Domain.Entities.Policy>> GetPolicyList();
        Task<ReferenceCustomFields> GetReferenceCustomFields(string tableName);
    }
}
