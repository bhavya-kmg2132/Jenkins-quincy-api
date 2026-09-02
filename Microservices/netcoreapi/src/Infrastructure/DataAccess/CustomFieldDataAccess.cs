using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper;
using Domain.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.DataAccess
{
    public class CustomFieldDataAccess : ICustomFieldDataAccess

    {
        private ILogger<CustomFieldDataAccess> _logger;
        private IConfiguration _configuration;
        private readonly IDomainEventService _domainEventService;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;
        public CustomFieldDataAccess(IConfiguration configuration, ILogger<CustomFieldDataAccess> logger, IDomainEventService domainEventService, IConnectionHelper connectionHelper)
        {
            _logger = logger;
            _configuration = configuration;
            _domainEventService = domainEventService;
            _connectionHelper = connectionHelper;
            _sqlQueries = _connectionHelper.LoadSqlQueriesXml("CustomFields");
        }


        /// <summary>
        /// Update : Update CustomField.
        /// </summary>
        /// <param names="CustomField"></param>
        /// <returns>int</returns>
        public async Task<string> AddCustomFields(CustomField customField, string entity)
        {
            try
            {
                _logger.LogInformation("AddCustomFields - In process");

                var existing = await GetCustomFieldByEntity(entity);

                if (existing.Any(c => c.field_name == customField.field_name))
                    throw new ApplicationException("Custom Field Already Exists");

                existing.Add(customField);

                var json = JsonConvert.SerializeObject(existing);

                var query = _sqlQueries["CustomFields.AddCustomFields"];

                using (var connection = _connectionHelper.CreateConnection())
                {
                    await connection.ExecuteAsync(query,
                        new { CustomFields = json, Entity = entity });
                }

                return "Inserted CustomField";
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddCustomFields - {ex.Message}");
                throw;
            }
        }

        public async Task<List<CustomField>> GetCustomFieldByEntity(string name)
        {
            try
            {
                _logger.LogInformation("GetCustomFieldByEntity - In process");

                var query = _sqlQueries["CustomFields.GetCustomFieldByEntity"];

                using (var connection = _connectionHelper.CreateConnection())
                {
                    var json = await connection.QueryFirstOrDefaultAsync<string>(
                        query,
                        new { TableName = name });

                    if (string.IsNullOrEmpty(json))
                        return new List<CustomField>();

                    return JsonConvert.DeserializeObject<List<CustomField>>(json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCustomFieldByEntity - {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetEntityNameFromReferenceCustomField()
        {
            try
            {
                _logger.LogInformation("GetEntityNameFromReferenceCustomField - In process");

                var query = _sqlQueries["CustomFields.GetEntityNameFromReferenceCustomField"];

                using (var connection = _connectionHelper.CreateConnection())
                {
                    var result = await connection.QueryAsync<string>(query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetEntityNameFromReferenceCustomField - {ex.Message}");
                throw;
            }
        }

        public async Task<string> DeleteCustomFieldFromEntity(string entityName, string fieldName)
        {
            try
            {
                _logger.LogInformation("DeleteCustomFieldFromEntity - In process");

                var existing = await GetCustomFieldByEntity(entityName);

                if (!existing.Any(f => f.field_name == fieldName))
                    return "Field Not Found";

                existing.RemoveAll(f => f.field_name == fieldName);

                var json = JsonConvert.SerializeObject(existing);

                var query = _sqlQueries["CustomFields.DeleteCustomFieldFromEntity"];

                using (var connection = _connectionHelper.CreateConnection())
                {
                    await connection.ExecuteAsync(query,
                        new { CustomFields = json, Entity = entityName });
                }

                return "Custom Field Deleted";
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteCustomFieldFromEntity - {ex.Message}");
                throw;
            }
        }

    }
}
