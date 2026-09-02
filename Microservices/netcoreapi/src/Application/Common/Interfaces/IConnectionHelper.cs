using System.Collections.Generic;
using System.Data;
using Domain.Enums;

namespace Application.Common.Interfaces
{
    public interface IConnectionHelper
    {
        IDbConnection CreateConnection();
        IDbConnection CreateNetAuthConnection();
        IDbConnection CreateEventConnection();
        Dictionary<string, string> LoadSqlQueriesXml(string xmlFileName, DbConfigKeys dbKey = DbConfigKeys.MainDb);
    }
}
