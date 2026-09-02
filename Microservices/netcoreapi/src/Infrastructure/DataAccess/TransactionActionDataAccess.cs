using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DataAccess
{
    /// <summary>
    /// Data Access layer for the WINS transaction code -> action matrix reference data.
    /// </summary>
    public class TransactionActionDataAccess : ITransactionActionDataAccess
    {
        private readonly ILogger<TransactionActionDataAccess> _logger;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;

        public TransactionActionDataAccess(ILogger<TransactionActionDataAccess> logger, IConnectionHelper connectionHelper)
        {
            _logger = logger;
            _connectionHelper = connectionHelper;
            _sqlQueries = _connectionHelper.LoadSqlQueriesXml("TransactionAction");
        }

        /// <summary>
        /// GetTransactionActionMatrix
        /// </summary>
        /// <returns>List<TransactionActionMatrix></returns>
        public async Task<List<TransactionActionMatrix>> GetTransactionActionMatrix()
        {
            try
            {
                _logger.LogInformation("TransactionActionDataAccess.GetTransactionActionMatrix - In process");

                var getTransactionActionMatrixQuery = _sqlQueries["TransactionAction.GetTransactionActionMatrix"];

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    var transactionActionMatrix = (await _dapperDbConnection.QueryAsync<TransactionActionMatrix>(getTransactionActionMatrixQuery)).AsList();

                    _logger.LogInformation("TransactionActionDataAccess.GetTransactionActionMatrix - Completed");

                    return transactionActionMatrix;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TransactionActionDataAccess.GetTransactionActionMatrix - " + ex.Message);
                throw;
            }
        }
    }
}
