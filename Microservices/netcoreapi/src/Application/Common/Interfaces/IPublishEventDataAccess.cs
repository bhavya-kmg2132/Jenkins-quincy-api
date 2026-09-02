
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.PublishEvent.Queries;

namespace Application.Common.Interfaces
{
    public interface IPublishEventDataAccess
    {
        #region IPublishEventDataAccess

        Task<string> Add(Domain.Common.PublishEventData publishEventData);
        Task<(List<PublishEventDataDto>, int)> GetList(int pageNumber, int pageSize, string orderType, string columnName, string filtersJson, string searchText);
        #endregion
    }
}
