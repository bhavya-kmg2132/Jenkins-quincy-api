using System.Data;

namespace NetAuth.Application.Common.Interfaces
{
    internal interface IConnectionHelper
    {
        IDbConnection CreateConnection();
        IDbConnection CreateNetAuthConnection();
        IDbConnection CreateEventConnection();
        Dictionary<string, string> LoadSqlQueriesXml(string xmlFileName);
    }
}
