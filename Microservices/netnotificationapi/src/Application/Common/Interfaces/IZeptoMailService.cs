using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IZeptoMailService
    {
        Task<Domain.Entities.ZeptoMail> SendEmailAsync(Domain.Entities.ZeptoMail zeptoMail);
        System.Threading.Tasks.Task DispatchEvents(Domain.Entities.ZeptoMail entity);
        Task<List<Domain.Entities.ZeptoMail>> SendBatchEmailAsync(List<Domain.Entities.ZeptoMail> zeptoMails);
    }

}
