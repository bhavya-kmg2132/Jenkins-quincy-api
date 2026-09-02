using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    /// <summary>
    /// DEV-ONLY: Tests HybridCache behaviour (L1 / L2 / DB) using isolated
    /// cache keys and a direct DB query in the factory.
    ///
    /// Production key  : Policy|List             tag: Policy
    /// Test key        : CacheTest|Policy|List   tag: CacheTest|Policy
    ///
    /// The factory does a direct Dapper query — PolicyDataAccess is never
    /// called and production Policy|List is never touched.
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CacheTestController : ControllerBase
    {
        private const string TestCacheKey = "CacheTest|Policy|List";
        private const string TestCacheTag = "CacheTest|Policy";

        private readonly HybridCache _hybridCache;
        private readonly IMemoryCache _memoryCache;
        private readonly IConnectionHelper _connectionHelper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CacheTestController> _logger;

        private readonly Dictionary<string, string> _sqlQueries;
        private readonly double _cacheExpiryHours;
        private readonly double _inMemoryCacheExpiryHours;

        public CacheTestController(
            HybridCache hybridCache,
            IMemoryCache memoryCache,
            IConnectionHelper connectionHelper,
            IConfiguration configuration,
            ILogger<CacheTestController> logger)
        {
            _hybridCache = hybridCache;
            _memoryCache = memoryCache;
            _connectionHelper = connectionHelper;
            _configuration = configuration;
            _logger = logger;
            _sqlQueries = _connectionHelper.LoadSqlQueriesXml("Policy");
            _cacheExpiryHours = Convert.ToDouble(configuration["CacheSettings:CacheExpiryHour"]);
            _inMemoryCacheExpiryHours = Convert.ToDouble(configuration["CacheSettings:InMemoryCacheExpiryHour"]);
        }

        // ─────────────────────────────────────────────────────────────
        // STATUS
        // ─────────────────────────────────────────────────────────────

        [HttpGet("status")]
        public IActionResult Status()
        {
            bool testL1Warm = _memoryCache.TryGetValue(TestCacheKey, out _);

            return Ok(new
            {
                Config = new
                {
                    UseInMemoryCache = _configuration["CacheSettings:UseInMemoryCache"],
                    UsePostgreSqlCache = _configuration["CacheSettings:UsePostgreSqlCache"],
                    UseSqlServerCache = _configuration["CacheSettings:UseSqlServerCache"],
                    L1_TTL_Hours = _cacheExpiryHours,
                    L2_TTL_Hours = _inMemoryCacheExpiryHours
                },
                TestCache = new
                {
                    Key = TestCacheKey,
                    Tag = TestCacheTag,
                    L1 = testL1Warm ? "WARM" : "COLD"
                },
                Endpoints = new
                {
                    GetList = "GET    /cache-test/get-list",
                    L1Probe = "GET    /cache-test/l1-probe",
                    ClearL1 = "DELETE /cache-test/clear-l1   → removes test key from L1 only",
                    ClearAll = "DELETE /cache-test/clear-all  → removes test key from L1 + L2",
                    ScenarioWarm = "GET    /cache-test/scenario/warm",
                    ScenarioL2 = "GET    /cache-test/scenario/l2-hit",
                    ScenarioCold = "GET    /cache-test/scenario/cold",
                    Stampede = "GET    /cache-test/scenario/stampede"
                }
            });
        }

        // ─────────────────────────────────────────────────────────────
        // L1 PROBE
        // ─────────────────────────────────────────────────────────────

        [HttpGet("l1-probe")]
        public IActionResult L1Probe()
        {
            bool testExists = _memoryCache.TryGetValue(TestCacheKey, out _);

            return Ok(new
            {
                Key = TestCacheKey,
                Status = testExists ? "L1 WARM" : "L1 COLD"
            });
        }

        // ─────────────────────────────────────────────────────────────
        // TIMED FETCH
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Fetches via test cache key. Factory does a direct DB query —
        /// production Policy|List is never read or written.
        /// Elapsed &lt; 1ms → L1. 1-15ms → L2. &gt; 15ms → DB.
        /// </summary>
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList()
        {
            _logger.LogInformation("CacheTest.GetList - START | key: {Key}", TestCacheKey);

            var sw = Stopwatch.StartNew();
            var (result, dbHit) = await FetchViaCacheTestKeyAsync();
            sw.Stop();

            double ms = sw.Elapsed.TotalMilliseconds;
            string source = InferSource(dbHit, ms);

            _logger.LogInformation("CacheTest.GetList - END | {ElapsedMs}ms | {Source}", ms, source);

            return Ok(new
            {
                CacheKey = TestCacheKey,
                CacheTag = TestCacheTag,
                InferredSource = source,
                DbHit = dbHit,
                ElapsedMs = Math.Round(ms, 3),
                RecordCount = result?.Count ?? 0,
                Timestamp = DateTime.UtcNow,
            });
        }

        // ─────────────────────────────────────────────────────────────
        // CLEAR L1 — test key only
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Removes test key from IMemoryCache (L1) only. L2 remains intact.
        /// Production Policy|List is NOT affected.
        /// </summary>
        [HttpDelete("clear-l1")]
        public IActionResult ClearL1()
        {
            _memoryCache.Remove(TestCacheKey);

            _logger.LogInformation("CacheTest.ClearL1 - Removed '{Key}' from IMemoryCache", TestCacheKey);

            return Ok(new
            {
                Action = $"IMemoryCache.Remove(\"{TestCacheKey}\")",
                L2Intact = true,
                NextStep = "Call GET /cache-test/get-list — elapsed 1-15ms + no 'Cache miss' log = L2 hit confirmed"
            });
        }

        // ─────────────────────────────────────────────────────────────
        // CLEAR ALL — test key only from L1 + L2
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Wipes test cache entry from L1 + L2 via RemoveByTagAsync("CacheTest|Policy").
        /// Production tag "Policy" and key "Policy|List" are NOT affected.
        /// </summary>
        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAll()
        {
            await _hybridCache.RemoveByTagAsync(TestCacheTag);

            _logger.LogInformation("CacheTest.ClearAll - RemoveByTagAsync('{Tag}')", TestCacheTag);

            return Ok(new
            {
                Action = $"RemoveByTagAsync(\"{TestCacheTag}\")",
                EntriesWiped = new[] { TestCacheKey },
                NextStep = "Call GET /cache-test/get-list → expect DB hit and 'CacheTest - Cache miss' in logs"
            });
        }

        // ─────────────────────────────────────────────────────────────
        // SCENARIO RUNNER
        // ─────────────────────────────────────────────────────────────

        [HttpGet("scenario/{name}")]
        public async Task<IActionResult> RunScenario(string name)
        {
            switch (name.ToLowerInvariant())
            {
                // ── warm: fetch as-is ────────────────────────────────
                case "warm":
                    {
                        var sw = Stopwatch.StartNew();
                        var (data, dbHit) = await FetchViaCacheTestKeyAsync();
                        sw.Stop();

                        double ms = sw.Elapsed.TotalMilliseconds;
                        return Ok(new
                        {
                            Scenario = "Warm — fetch as-is",
                            CacheKey = TestCacheKey,
                            ElapsedMs = Math.Round(ms, 3),
                            RecordCount = data?.Count ?? 0,
                            DbHit = dbHit,
                            InferredSource = InferSource(dbHit, ms)
                        });
                    }

                // ── l2-hit: clear test L1 only, fetch → L2 hit ──────
                case "l2-hit":
                    {
                        _memoryCache.Remove(TestCacheKey);
                        _logger.LogInformation("CacheTest.Scenario.L2Hit - Test L1 cleared, fetching...");

                        var sw = Stopwatch.StartNew();
                        var (data, dbHit) = await FetchViaCacheTestKeyAsync();
                        sw.Stop();

                        double ms = sw.Elapsed.TotalMilliseconds;
                        return Ok(new
                        {
                            Scenario = "L2 Hit Test",
                            CacheKey = TestCacheKey,
                            Steps = new[] { $"1. IMemoryCache.Remove({TestCacheKey})", "2. Direct fetch via test key", "3. Infer from elapsed + logs" },
                            ElapsedMs = Math.Round(ms, 3),
                            RecordCount = data?.Count ?? 0,
                            DbHit = dbHit,
                            InferredSource = InferSource(dbHit, ms),
                            ConfirmViaLogs = "No 'CacheTest - Cache miss' log = L2 hit confirmed"
                        });
                    }

                // ── cold: clear test L1+L2, fetch → DB hit ──────────
                case "cold":
                    {
                        await _hybridCache.RemoveByTagAsync(TestCacheTag);
                        _logger.LogInformation("CacheTest.Scenario.Cold - Test cache cleared (L1+L2), fetching...");

                        var sw = Stopwatch.StartNew();
                        var (data, dbHit) = await FetchViaCacheTestKeyAsync();
                        sw.Stop();

                        double ms = sw.Elapsed.TotalMilliseconds;
                        return Ok(new
                        {
                            Scenario = "Cold Cache — full DB hit",
                            CacheKey = TestCacheKey,
                            Steps = new[] { $"1. RemoveByTagAsync({TestCacheTag})", "2. Direct fetch via test key", "3. Factory must have run" },
                            ElapsedMs = Math.Round(ms, 3),
                            RecordCount = data?.Count ?? 0,
                            DbHit = dbHit,
                            InferredSource = InferSource(dbHit, ms),
                            ConfirmViaLogs = "Must see 'CacheTest - Cache miss, fetching from DB directly' in logs"
                        });
                    }

                // ── stampede: 5 parallel cold fetches → factory once ─
                case "stampede":
                    {
                        await _hybridCache.RemoveByTagAsync(TestCacheTag);
                        _logger.LogInformation("CacheTest.Scenario.Stampede - Test cache cleared, firing 5 parallel fetches...");

                        var tasks = new Task<(List<Policy> Data, bool DbHit)>[5];
                        for (int i = 0; i < 5; i++)
                            tasks[i] = FetchViaCacheTestKeyAsync();

                        var sw = Stopwatch.StartNew();
                        await Task.WhenAll(tasks);
                        sw.Stop();

                        int dbHitCount = tasks.Count(t => t.Result.DbHit);

                        return Ok(new
                        {
                            Scenario = "Stampede Protection",
                            CacheKey = TestCacheKey,
                            ParallelCalls = 5,
                            DbHitCount = dbHitCount,
                            StampedeResult = dbHitCount == 1 ? "PASS — factory ran exactly once" : $"FAIL — factory ran {dbHitCount} times",
                            TotalElapsedMs = Math.Round(sw.Elapsed.TotalMilliseconds, 3),
                            ConfirmViaLogs = "'CacheTest - Cache miss' should appear exactly ONCE — not 5 times"
                        });
                    }

                default:
                    return BadRequest(new
                    {
                        Error = $"Unknown scenario '{name}'",
                        Available = new[] { "warm", "l2-hit", "cold", "stampede" }
                    });
            }
        }

        // ─────────────────────────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// All test fetches use this method. The factory runs a direct Dapper
        /// query — PolicyDataAccess is never called, production Policy|List
        /// is never read or written.
        /// Returns the data and a DbHit flag — true only if the factory ran (DB was queried).
        /// </summary>
        private async Task<(List<Policy> Data, bool DbHit)> FetchViaCacheTestKeyAsync()
        {
            bool dbHit = false;

            var data = await _hybridCache.GetOrCreateAsync<List<Policy>>(
                TestCacheKey,
                async _ =>
                {
                    dbHit = true;
                    _logger.LogInformation("CacheTest - Cache miss, fetching from DB directly (PolicyDataAccess bypassed)");

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
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(_cacheExpiryHours),
                    LocalCacheExpiration = TimeSpan.FromHours(_inMemoryCacheExpiryHours)
                },
                tags: new[] { TestCacheTag });

            return (data, dbHit);
        }

        /// <summary>
        /// DbHit=true  → DB (definitive — factory ran).
        /// DbHit=false, ms &lt; 1  → L1 (sub-ms memory read).
        /// DbHit=false, ms &gt;= 1 → L2 (network + deserialization cost).
        /// </summary>
        private static string InferSource(bool dbHit, double ms)
            => dbHit ? "DB — cache miss (factory ran)"
             : ms < 1 ? "L1 — in-memory (sub-ms)"
                      : "L2 — PostgreSQL";
    }
}
