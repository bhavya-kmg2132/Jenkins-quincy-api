using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Common;

namespace Application.Common.Interfaces
{
    public interface ICustomFieldDataAccess
    {
        Task<string> AddCustomFields(CustomField customField, string entity);
        Task<List<CustomField>> GetCustomFieldByEntity(string Name);
        Task<string> DeleteCustomFieldFromEntity(string EntityName, string FieldName);
        Task<List<string>> GetEntityNameFromReferenceCustomField();
    }
}
