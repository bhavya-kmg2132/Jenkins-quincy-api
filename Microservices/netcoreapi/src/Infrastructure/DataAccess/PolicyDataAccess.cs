using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper;
using Domain.Common;
using Domain.Entities;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DataAccess
{
    public class PolicyDataAccess : IPolicyDataAccess
    {
        private readonly ILogger<PolicyDataAccess> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDomainEventService _domainEventService;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;
        private readonly HybridCache _hybridCache;
        private readonly double _cacheExpiryHours;
        private readonly double _inMemoryCacheExpiryHours;
        private readonly string _cachePrefix;
        private readonly string _cacheKeyForPolicyList;
        private readonly string _cacheTagForPolicy;

        public PolicyDataAccess(IConfiguration configuration, ILogger<PolicyDataAccess> logger,
            IDomainEventService domainEventService, IConnectionHelper connectionHelper,
            HybridCache hybridCache, IHostEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _domainEventService = domainEventService;
            _connectionHelper = connectionHelper;
            _sqlQueries = _connectionHelper.LoadSqlQueriesXml("Policy");
            _hybridCache = hybridCache;
            _cacheExpiryHours = Convert.ToDouble(configuration["CacheSettings:CacheExpiryHour"]);
            _inMemoryCacheExpiryHours = Convert.ToDouble(configuration["CacheSettings:InMemoryCacheExpiryHour"]);
            _cachePrefix = $"{env.EnvironmentName}|{configuration["Api:internal_name"]}|";
            _cacheKeyForPolicyList = _cachePrefix + "Policy|GetPolicyList";
            _cacheTagForPolicy = _cachePrefix + "Policy";
        }

        public async Task<string> Add(Policy policy)
        {
            try
            {
                _logger.LogInformation("PolicyDataAccess.Add - In process");

                policy.PolicyNumber = await GeneratePolicyNumberAsync();

                string customFieldsJson = policy.CustomFields != null
                    ? JsonSerializer.Serialize(policy.CustomFields) : null;

                var p = new
                {
                    PolicyNumber = policy.PolicyNumber,
                    PolicyName = policy.PolicyName,
                    LineOfBusinessCode = policy.LineOfBusinessCode,
                    PolicyType = policy.PolicyType,
                    StatusCode = policy.StatusCode,
                    TransactionType = policy.TransactionType,
                    QuoteId = policy.QuoteId,
                    RenewalStatus = policy.RenewalStatus,
                    InsuredId = policy.InsuredId,
                    InsuredName = policy.InsuredName,
                    InsuredAddress = policy.InsuredAddress,
                    EffectiveDate = policy.EffectiveDate,
                    ExpirationDate = policy.ExpirationDate,
                    OriginalEffectiveDate = policy.OriginalEffectiveDate,
                    AccountingDate = policy.AccountingDate,
                    CancellationDate = policy.CancellationDate,
                    CancelReasonDescription = policy.CancelReasonDescription,
                    TotalPremium = policy.TotalPremium,
                    SumInsured = policy.SumInsured,
                    Deductible = policy.Deductible,
                    Currency = policy.Currency,
                    ProducerCode = policy.ProducerCode,
                    ProducerName = policy.ProducerName,
                    UnderwriterId = policy.UnderwriterId,
                    UnderwriterName = policy.UnderwriterName,
                    AgentCode = policy.AgentCode,
                    VesselName = policy.VesselName,
                    VesselType = policy.VesselType,
                    CargoType = policy.CargoType,
                    RouteFrom = policy.RouteFrom,
                    RouteTo = policy.RouteTo,
                    AircraftRegistration = policy.AircraftRegistration,
                    FlightNumber = policy.FlightNumber,
                    RiskDescription = policy.RiskDescription,
                    SurveyorName = policy.SurveyorName,
                    Remarks = policy.Remarks,
                    CreatedBy = policy.CreatedBy,
                    CreatedDateTime = policy.CreatedDateTime,
                    UpdatedDateTime = policy.UpdatedDateTime,
                    CustomFields = customFieldsJson
                };

                using var conn = _connectionHelper.CreateConnection();
                policy.Id = await conn.ExecuteScalarAsync<string>(_sqlQueries["Policy.Add"], p);

                await DispatchEvents(policy);

                // Invalidate list cache — new policy must appear in next GetPolicyList call
                await _hybridCache.RemoveByTagAsync(_cacheTagForPolicy);

                _logger.LogInformation("PolicyDataAccess.Add - Completed");
                return policy.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError("PolicyDataAccess.Add - " + ex.Message);
                throw;
            }
        }

        public async Task<int> Update(Policy policy)
        {
            try
            {
                _logger.LogInformation("PolicyDataAccess.Update - In process");

                string customFieldsJson = policy.CustomFields != null
                    ? JsonSerializer.Serialize(policy.CustomFields) : null;

                var p = new
                {
                    Id = policy.Id,
                    PolicyName = policy.PolicyName,
                    PolicyType = policy.PolicyType,
                    StatusCode = policy.StatusCode,
                    TransactionType = policy.TransactionType,
                    QuoteId = policy.QuoteId,
                    RenewalStatus = policy.RenewalStatus,
                    InsuredId = policy.InsuredId,
                    InsuredName = policy.InsuredName,
                    InsuredAddress = policy.InsuredAddress,
                    EffectiveDate = policy.EffectiveDate,
                    ExpirationDate = policy.ExpirationDate,
                    OriginalEffectiveDate = policy.OriginalEffectiveDate,
                    AccountingDate = policy.AccountingDate,
                    CancellationDate = policy.CancellationDate,
                    CancelReasonDescription = policy.CancelReasonDescription,
                    TotalPremium = policy.TotalPremium,
                    SumInsured = policy.SumInsured,
                    Deductible = policy.Deductible,
                    Currency = policy.Currency,
                    ProducerCode = policy.ProducerCode,
                    ProducerName = policy.ProducerName,
                    UnderwriterId = policy.UnderwriterId,
                    UnderwriterName = policy.UnderwriterName,
                    AgentCode = policy.AgentCode,
                    VesselName = policy.VesselName,
                    VesselType = policy.VesselType,
                    CargoType = policy.CargoType,
                    RouteFrom = policy.RouteFrom,
                    RouteTo = policy.RouteTo,
                    AircraftRegistration = policy.AircraftRegistration,
                    FlightNumber = policy.FlightNumber,
                    RiskDescription = policy.RiskDescription,
                    SurveyorName = policy.SurveyorName,
                    Remarks = policy.Remarks,
                    UpdatedBy = policy.UpdatedBy,
                    UpdatedDateTime = policy.UpdatedDateTime,
                    CustomFields = customFieldsJson
                };

                using var conn = _connectionHelper.CreateConnection();
                int rows = await conn.ExecuteAsync(_sqlQueries["Policy.Update"], p);

                await DispatchEvents(policy);

                // Invalidate list + all per-user GetById cached DTOs for this policy
                await _hybridCache.RemoveByTagAsync(_cacheTagForPolicy);

                _logger.LogInformation("PolicyDataAccess.Update - Completed");
                return rows;
            }
            catch (Exception ex)
            {
                _logger.LogError("PolicyDataAccess.Update - " + ex.Message);
                throw;
            }
        }

        public async Task<int> Delete(Policy policy)
        {
            try
            {
                _logger.LogInformation("PolicyDataAccess.Delete - In process");

                using var conn = _connectionHelper.CreateConnection();
                int rows = await conn.ExecuteAsync(_sqlQueries["Policy.Delete"],
                    new { policy.Id, policy.UpdatedBy, policy.UpdatedDateTime });

                await DispatchEvents(policy);

                // Invalidate list + all per-user GetById cached DTOs for this policy
                await _hybridCache.RemoveByTagAsync(_cacheTagForPolicy);

                _logger.LogInformation("PolicyDataAccess.Delete - Completed");
                return rows;
            }
            catch (Exception ex)
            {
                _logger.LogError("PolicyDataAccess.Delete - " + ex.Message);
                throw;
            }
        }

        public async Task<Policy> GetPolicyById(string id)
        {
            try
            {
                _logger.LogInformation("PolicyDataAccess.GetPolicyById - In process");

                var policies = await GetPolicyList();
                var policy = policies?.FirstOrDefault(p => p.Id == id);

                _logger.LogInformation("PolicyDataAccess.GetPolicyById - Completed");
                return policy;
            }
            catch (Exception ex)
            {
                _logger.LogError("PolicyDataAccess.GetPolicyById - " + ex.Message);
                throw;
            }
        }

        public async Task<List<Policy>> GetPolicyList()
        {
            try
            {
                _logger.LogInformation("PolicyDataAccess.GetPolicyList - In process");

                var entryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(_cacheExpiryHours),
                    LocalCacheExpiration = TimeSpan.FromHours(_inMemoryCacheExpiryHours)
                };

                var result = await _hybridCache.GetOrCreateAsync<List<Policy>>(
                    _cacheKeyForPolicyList,
                    async _ =>
                    {
                        _logger.LogInformation("PolicyDataAccess.GetPolicyList - Cache miss, fetching from DB");
                        using var conn = _connectionHelper.CreateConnection();
                        return (await conn.QueryAsync<Policy>(_sqlQueries["Policy.GetPolicyList"]))
                            .Select(p =>
                            {
                                p.CustomFields = string.IsNullOrWhiteSpace(p.CustomFieldJson)
                                    ? null
                                    : JsonSerializer.Deserialize<List<CustomField>>(p.CustomFieldJson);
                                return p;
                            })
                            .ToList();
                    },
                    entryOptions,
                    tags: new[] { _cacheTagForPolicy });

                _logger.LogInformation("PolicyDataAccess.GetPolicyList - Completed");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("PolicyDataAccess.GetPolicyList - " + ex.Message);
                throw;
            }
        }

        public async Task<int> PermanentDelete(string id)
        {
            try
            {
                _logger.LogInformation("PolicyDataAccess.PermanentDelete - In process");

                using var conn = _connectionHelper.CreateConnection();
                int rows = await conn.ExecuteAsync(_sqlQueries["Policy.PermanentDelete"], new { Id = id });

                await _hybridCache.RemoveByTagAsync(_cacheTagForPolicy);

                _logger.LogInformation("PolicyDataAccess.PermanentDelete - Completed");
                return rows;
            }
            catch (Exception ex)
            {
                _logger.LogError("PolicyDataAccess.PermanentDelete - " + ex.Message);
                throw;
            }
        }

        public async Task<ReferenceCustomFields> GetReferenceCustomFields(string tableName)
        {
            try
            {
                _logger.LogInformation("PolicyDataAccess.GetReferenceCustomFields - In process");

                var queryParams = new { TableName = tableName };

                using var conn = _connectionHelper.CreateConnection();
                string referenceCustomFieldJson = await conn.QueryFirstOrDefaultAsync<string>(
                    _sqlQueries["Policy.GetReferenceCustomFields"], queryParams);

                var referenceCustomFields = new ReferenceCustomFields();
                if (!string.IsNullOrWhiteSpace(referenceCustomFieldJson))
                    referenceCustomFields.CustomFields = JsonSerializer.Deserialize<List<CustomField>>(referenceCustomFieldJson);

                _logger.LogInformation("PolicyDataAccess.GetReferenceCustomFields - Completed");
                return referenceCustomFields;
            }
            catch (Exception ex)
            {
                _logger.LogError("PolicyDataAccess.GetReferenceCustomFields - " + ex.Message);
                throw;
            }
        }

        private async Task<string> GeneratePolicyNumberAsync()
        {
            int year = DateTime.UtcNow.Year;

            using var conn = _connectionHelper.CreateConnection();
            string last = await conn.ExecuteScalarAsync<string>(_sqlQueries["Policy.GeneratePolicyNumber"]);

            int next = 1;
            if (!string.IsNullOrWhiteSpace(last) && last.StartsWith($"MCA-{year}-"))
            {
                var parts = last.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int seq))
                    next = seq + 1;
            }

            return $"MCA-{year}-{next:D6}";
        }

        private async Task DispatchEvents(Policy entity)
        {
            while (true)
            {
                var evt = entity.DomainEvents.FirstOrDefault(e => !e.IsPublished);
                if (evt == null) break;
                evt.IsPublished = true;
                await _domainEventService.Publish(evt);
            }
        }
    }
}
