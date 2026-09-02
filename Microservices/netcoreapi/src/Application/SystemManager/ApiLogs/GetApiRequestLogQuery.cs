using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Application.ApiLog.Queries.GetApiRequestLogQuery
{
    public class GetApiRequestLogQuery : IRequest<ApiRequestLogListVm>
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Path { get; set; }
        public long? MinElapsedMs { get; set; }
        public string Source { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class GetApiRequestLogQueryHandler : IRequestHandler<GetApiRequestLogQuery, ApiRequestLogListVm>
    {
        private const string MsSqlServer = "MsSqlServer";
        private const string PostgreSql = "PostgreSql";

        private readonly IConfiguration _configuration;

        public GetApiRequestLogQueryHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ApiRequestLogListVm> Handle(GetApiRequestLogQuery query, CancellationToken cancellationToken)
        {
            var dbServer = _configuration["Api:SqlDatabaseServer"] ?? MsSqlServer;
            bool isPg = dbServer == PostgreSql;

            // Column quoting helper: PostgreSQL requires quoted identifiers to preserve case
            string Q(string col) => isPg ? $"\"{col}\"" : col;

            var table = isPg ? "app.\"ApiRequestLog\"" : "ApiRequestLog";
            var colList = isPg
                ? "\"Id\", \"CorrelationId\", \"Method\", \"Path\", \"StatusCode\", \"ElapsedMs\", \"Source\", \"CreatedOn\""
                : "Id, CorrelationId, Method, Path, StatusCode, ElapsedMs, Source, CreatedOn";

            var where = new List<string>();
            if (query.From.HasValue) where.Add($"{Q("CreatedOn")} >= @from");
            if (query.To.HasValue) where.Add($"{Q("CreatedOn")} <= @to");
            if (!string.IsNullOrEmpty(query.Path)) where.Add($"{Q("Path")} LIKE @path");
            if (query.MinElapsedMs.HasValue) where.Add($"{Q("ElapsedMs")} >= @minMs");
            if (!string.IsNullOrEmpty(query.Source)) where.Add($"{Q("Source")} = @source");

            var whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : string.Empty;
            var offset = (query.Page - 1) * query.PageSize;

            var pagination = isPg
                ? "LIMIT @pageSize OFFSET @offset"
                : "OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            var sql = $@"
                SELECT {colList} FROM {table}
                {whereClause}
                ORDER BY {Q("CreatedOn")} DESC
                {pagination};

                SELECT COUNT(1) FROM {table} {whereClause};";

            DbConnection conn = isPg
                ? new NpgsqlConnection(_configuration.GetConnectionString("PostgreSqlDBConnection"))
                : new SqlConnection(_configuration.GetConnectionString("SqlDBConnection"));

            await using (conn)
            {
                await conn.OpenAsync(cancellationToken);

                DbCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                void AddParam(string name, object value)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = name;
                    p.Value = value;
                    cmd.Parameters.Add(p);
                }

                if (query.From.HasValue) AddParam("@from", query.From.Value);
                if (query.To.HasValue) AddParam("@to", query.To.Value);
                if (!string.IsNullOrEmpty(query.Path)) AddParam("@path", $"%{query.Path}%");
                if (query.MinElapsedMs.HasValue) AddParam("@minMs", query.MinElapsedMs.Value);
                if (!string.IsNullOrEmpty(query.Source)) AddParam("@source", query.Source);
                AddParam("@offset", offset);
                AddParam("@pageSize", query.PageSize);

                var result = new ApiRequestLogListVm { Items = new List<ApiRequestLogDto>() };

                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                {
                    result.Items.Add(new ApiRequestLogDto
                    {
                        Id = reader.GetInt64(0),
                        CorrelationId = reader.GetString(1),
                        Method = reader.GetString(2),
                        Path = reader.GetString(3),
                        StatusCode = reader.GetInt32(4),
                        ElapsedMs = reader.GetInt64(5),
                        Source = reader.GetString(6),
                        CreatedOn = reader.GetDateTime(7)
                    });
                }

                if (await reader.NextResultAsync(cancellationToken) && await reader.ReadAsync(cancellationToken))
                    result.Total = reader.GetInt32(0);

                return result;
            }
        }
    }
}
