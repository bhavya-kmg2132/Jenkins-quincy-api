using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.TransactionAction.Queries;
using Microsoft.Extensions.Logging;

namespace Application.TransactionAction.Queries.GetTransactionActionMatrix
{
    /// <summary>
    /// Returns the WINS transaction-code -> status -> available-actions matrix,
    /// keyed by transaction code (e.g. "10", "20", "55") so the frontend can index it directly.
    /// </summary>
    public class GetTransactionActionMatrixQuery : IRequest<Dictionary<string, TransactionActionStatusDto>>
    {
    }

    public class GetTransactionActionMatrixQueryHandler : IRequestHandler<GetTransactionActionMatrixQuery, Dictionary<string, TransactionActionStatusDto>>
    {
        private readonly ILogger<GetTransactionActionMatrixQueryHandler> _logger;
        private readonly ITransactionActionDataAccess _transactionActionDataAccess;

        public GetTransactionActionMatrixQueryHandler(ILogger<GetTransactionActionMatrixQueryHandler> logger, ITransactionActionDataAccess transactionActionDataAccess)
        {
            _logger = logger;
            _transactionActionDataAccess = transactionActionDataAccess;
        }

        public async Task<Dictionary<string, TransactionActionStatusDto>> Handle(GetTransactionActionMatrixQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTransactionActionMatrixQuery.Handle - In Process");

            var rows = await _transactionActionDataAccess.GetTransactionActionMatrix();

            var matrix = new Dictionary<string, TransactionActionStatusDto>();

            foreach (var row in rows)
            {
                var key = row.TransactionCode.ToString();

                if (!matrix.TryGetValue(key, out var entry))
                {
                    entry = new TransactionActionStatusDto { TransactionCodeName = row.TransactionCodeName };
                    matrix[key] = entry;
                }

                var actions = row.Status == "PEND" ? entry.Pend : entry.Post;
                actions.Add(new TransactionActionItemDto
                {
                    ActionName = row.ActionName,
                    ActionDisplayName = row.ActionDisplayName
                });
            }

            _logger.LogInformation("GetTransactionActionMatrixQuery.Handle - Completed");

            return matrix;
        }
    }
}
