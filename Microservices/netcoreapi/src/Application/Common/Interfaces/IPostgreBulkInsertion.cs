using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IPostgreBulkInsertion
    {
        Task BulkInsertNotificationsAsync(IEnumerable<PostgreNotification> notifications);
        Task BulkInsertZeptoMailRequestsAsync(IEnumerable<ZeptoMail> notifications);
    }
}
