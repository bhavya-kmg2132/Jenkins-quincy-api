using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ITransactionActionDataAccess
    {
        Task<List<Domain.Entities.TransactionActionMatrix>> GetTransactionActionMatrix();
    }
}
