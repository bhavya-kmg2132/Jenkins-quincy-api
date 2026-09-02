using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Common.Interfaces;
using Dapper;
using Domain.Entities;
using Domain.Enums;
using Hangfire;
using Hangfire.Storage;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

public class CronJobScheduler : ICronJobScheduler
{
    private ILogger<CronJobScheduler> _logger;
    private IConfiguration _configuration;
    private readonly IDomainEventService _domainEventService;
    private readonly Dictionary<string, string> _sqlQueries;
    private IDbConnection _dapperEventDbConnection { get; set; }
    public IReadOnlyDictionary<string, string> SqlQueries =>
    new ReadOnlyDictionary<string, string>(_sqlQueries);
    private readonly IConnectionHelper _connectionHelper;

    public CronJobScheduler(IConfiguration configuration, ILogger<CronJobScheduler> logger, IDomainEventService domainEventService, IConnectionHelper connectionHelper)
    {
        _logger = logger;
        _configuration = configuration;
        _domainEventService = domainEventService;
        _dapperEventDbConnection = new NpgsqlConnection(_configuration["Hangfire:HangfireTransportConnection:PostGreSQL"]);
        _connectionHelper = connectionHelper;
        _sqlQueries = _connectionHelper.LoadSqlQueriesXml("CronJobRule", DbConfigKeys.EventDb);
    }

    //#region CronJob Scheduling Methods (used to add/update jobs in Hangfire tables) 
    //public async Task UpdateCronJobAsync(CronJobRule oldCronJobRule, CronJobRule newCronJobRule)
    //{
    //    string baseJobId = $"CronJob-{newCronJobRule.Id}";

    //    HashSet<string> Split(string? value) =>
    //        string.IsNullOrWhiteSpace(value)
    //            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    //            : value.Split(';', StringSplitOptions.RemoveEmptyEntries)
    //                   .Select(v => v.Trim())
    //                   .ToHashSet(StringComparer.OrdinalIgnoreCase);

    //    //var oldFrequency = oldCronJobRule.Frequency;
    //    //var newFrequency = newCronJobRule.Frequency;

    //    //var oldDays = oldCronJobRule.ExecutionDay.Split(";", StringSplitOptions.RemoveEmptyEntries);
    //    //var newDays = newCronJobRule.ExecutionDay.Split(";", StringSplitOptions.RemoveEmptyEntries);

    //    var oldDays = Split(oldCronJobRule.ExecutionDay);
    //    var newDays = Split(newCronJobRule.ExecutionDay);
    //    string removed = String.Join(";", oldDays.Except(newDays));

    //    // --------------------
    //    // 1) FREQUENCY CHANGED
    //    // --------------------
    //    if (!oldCronJobRule.Frequency.Equals(newCronJobRule.Frequency, StringComparison.OrdinalIgnoreCase))
    //    {
    //        await RemoveOldCronJobByFrequencyAsync(baseJobId, oldCronJobRule);
    //    }

    //    // ---------------------------------
    //    // 2) SAME FREQUENCY, DAYS CHANGED
    //    // ---------------------------------
    //    else if (!newDays.Equals(oldDays))
    //    {

    //        await RemoveCronJobWithExecutionDays(baseJobId, oldCronJobRule.ExecutionMonth, removed);
    //    }

    //    // -------------------------------------------------------------------
    //    // 2.1) SAME FREQUENCY, EXECUTION MONTH CHANGED (for Yearly frequency)
    //    // -------------------------------------------------------------------
    //    else if (newCronJobRule.ExecutionMonth != oldCronJobRule.ExecutionMonth)
    //    {
    //        // For Yearly frequency, if month changes, we need to remove old jobs and add new ones, since month info is part of job id.
    //        await RemoveCronJobWithExecutionDays(baseJobId, oldCronJobRule.ExecutionMonth, oldCronJobRule.ExecutionDay);
    //    }

    //    // --------------------
    //    // 3) ADD / UPDATE JOBS
    //    // --------------------
    //    await ScheduleCronJobAsync(newCronJobRule);
    //}

    //public async Task RemoveOldCronJobByFrequencyAsync(string baseJobId, CronJobRule rule)
    //{
    //    // Remove daily
    //    RecurringJob.RemoveIfExists(baseJobId);

    //    if (string.IsNullOrWhiteSpace(rule.ExecutionDay) || rule.ExecutionDay == "Daily")
    //        return;

    //    await RemoveCronJobWithExecutionDays(baseJobId, rule.ExecutionMonth, rule.ExecutionDay);
    //}

    //public async Task RemoveCronJobWithExecutionDays(string baseJobId, int executionMonth, string ExecutionDay)
    //{
    //    foreach (var day in ExecutionDay.Split(';', StringSplitOptions.RemoveEmptyEntries))
    //    {

    //        // Weekly
    //        RecurringJob.RemoveIfExists($"{baseJobId}-{day.Trim()}");

    //        // Monthly
    //        var executionDayNumber = new string((day.Trim()).Where(char.IsDigit).ToArray());
    //        int.TryParse(executionDayNumber, out int dayNumber);

    //        if (dayNumber > 0 && executionMonth == 0)
    //        {
    //            RecurringJob.RemoveIfExists($"{baseJobId}-{dayNumber}");
    //            continue;
    //        }

    //        // Yearly with month info
    //        else if (executionMonth > 0) 
    //        {
    //            RecurringJob.RemoveIfExists($"{baseJobId}-{executionMonth}-{dayNumber}");
    //        }
    //    }
    //}

    //public async Task ScheduleCronJobAsync(CronJobRule rule)
    //{
    //    string baseJobId = $"CronJob-{rule.Id}";

    //    if (rule.Frequency.Equals("Daily", StringComparison.OrdinalIgnoreCase))
    //    {
    //        RecurringJob.AddOrUpdate<CronJob>(
    //            baseJobId,
    //            job => job.RunCronJob(rule.Id, rule.NotificationName, rule.IsNotificationPaused),
    //            Cron.Daily(rule.ExecutionTime.Hours, rule.ExecutionTime.Minutes));
    //    }
    //    else if (rule.Frequency.Equals("Weekly", StringComparison.OrdinalIgnoreCase))
    //    {
    //        foreach (var day in (rule.ExecutionDay ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
    //        {
    //            var cron = Cron.Weekly(
    //                (DayOfWeek)Enum.Parse(typeof(DayOfWeek), day, true),
    //                rule.ExecutionTime.Hours,
    //                rule.ExecutionTime.Minutes);

    //            RecurringJob.AddOrUpdate<CronJob>(
    //                $"{baseJobId}-{day}",
    //                job => job.RunCronJob(rule.Id, rule.NotificationName, rule.IsNotificationPaused),
    //                cron);
    //        }
    //    }
    //    else if (rule.Frequency.Equals("Monthly", StringComparison.OrdinalIgnoreCase))
    //    {
    //        foreach (var day in (rule.ExecutionDay ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
    //        {
    //            int dayNumber = int.Parse(new string(day.Where(char.IsDigit).ToArray()));

    //            var cron = Cron.Monthly(
    //                dayNumber,
    //                rule.ExecutionTime.Hours,
    //                rule.ExecutionTime.Minutes);

    //            RecurringJob.AddOrUpdate<CronJob>(
    //                $"{baseJobId}-{dayNumber}",
    //                job => job.RunCronJob(rule.Id, rule.NotificationName, rule.IsNotificationPaused),
    //                cron);
    //        }
    //    }

    //    else if (rule.Frequency.Equals("Yearly", StringComparison.OrdinalIgnoreCase))
    //    {
    //        foreach (var day in (rule.ExecutionDay).Split(';', StringSplitOptions.RemoveEmptyEntries))
    //        {
    //            int dayNumber = int.Parse(new string(day.Where(char.IsDigit).ToArray()));

    //            var cron = $"{rule.ExecutionTime.Minutes} " +
    //                       $"{rule.ExecutionTime.Hours} " +
    //                       $"{dayNumber} " +
    //                       $"{rule.ExecutionMonth} *";

    //            RecurringJob.AddOrUpdate<CronJob>(
    //                $"{baseJobId}-{rule.ExecutionMonth}-{dayNumber}",
    //                job => job.RunCronJob(rule.Id, rule.NotificationName, rule.IsNotificationPaused),
    //                cron);
    //        }
    //    }
    //}
    //#endregion

    //#region CronJob Rule methods (CRUD to handle the cron job rule updates from user to JobRule database,related to any type of workflow (ex:Notifications))
    //public async Task<string> InsertCronJobRuleAsync(CronJobRule cronJobRule)
    //{
    //    try
    //    {
    //        //Step 1: Logging Information
    //        this._logger.LogInformation("CronJobScheduler.InsertNewCronJobRuleAsync - In process");

    //        //Step 2: Create SQL Query
    //        var insertQuery = _sqlQueries["CronJobRule.InsertCronJobRule"];

    //        //Step 3: Execute the query using Dapper
    //        var insertedId = await _dapperEventDbConnection.ExecuteScalarAsync<string>(insertQuery, cronJobRule);
    //        cronJobRule.Id = insertedId.ToString();

    //        // Step 4: Call ScheduleNotificationCronJobAsync Api
    //        await ScheduleCronJobAsync(cronJobRule);

    //        //Step 5: Logging Information Completed
    //        _logger.LogInformation("CronJobScheduler.InsertNewCronJobRuleAsync - Completed");

    //        await DispatchEvents(cronJobRule);

    //        //Step 6: Return Id
    //        return cronJobRule.Id;
    //    }
    //    catch (Exception ex)
    //    {
    //        //Logging Information : Error Message.
    //        _logger.LogError($"CronJobScheduler.InsertNewCronJobRuleAsync - {ex.Message}");
    //        throw;
    //    }
    //}

    //public async Task<int> UpdateCronJobRuleAsync(CronJobRule newCronJobRule, CronJobRule oldCronJobRule)
    //{
    //    try
    //    {
    //        //Step 1: Logging Information
    //        this._logger.LogInformation("CronJobScheduler.UpdateCronJobRuleAsync - In process");

    //        //Step 2: Create SQL Query
    //        var updateQuery = _sqlQueries["CronJobRule.UpdateCronJobRule"];

    //        //Step 3: Declare new variable.
    //        int rowsAffected = 0;

    //        ////Step 4: Assign Parameters for Query.
    //        //CronJobRule notificationRule = new()
    //        //{
    //        //    Id = newCronJobRule.Id,
    //        //    NotificationName = newCronJobRule.NotificationName,
    //        //    ExecutionDay = newCronJobRule.ExecutionDay,
    //        //    ExecutionTime = newCronJobRule.ExecutionTime,
    //        //    LastExecutionDate = newCronJobRule.LastExecutionDate,
    //        //    Frequency = newCronJobRule.Frequency,
    //        //    Role = newCronJobRule.Role
    //        //};

    //        //Step 5: Execute the query using Dapper
    //        rowsAffected = await _dapperEventDbConnection.ExecuteAsync(updateQuery, newCronJobRule);

    //        //Step 6: Dispatch Events
    //        await DispatchEvents(newCronJobRule);

    //        // Step 7: Call ScheduleNotificationAndCronJobs Api to Update/Remove the CronJob Schedules,
    //        //         if needed, based on JobId and Frequency/ExecutionDay changes. 

    //        await UpdateCronJobAsync(oldCronJobRule, newCronJobRule);

    //        //Step 8: Logging Information Completed
    //        _logger.LogInformation("CronJobScheduler.UpdateCronJobRuleAsync - Completed");

    //        //Step 9: Return rowsAffected
    //        return rowsAffected;
    //    }
    //    catch (Exception ex)
    //    {
    //        //Logging Information : Error Message.
    //        _logger.LogError($"CronJobScheduler.UpdateCronJobRuleAsync - {ex.Message}");
    //        throw;
    //    }
    //}

    //public async Task<int> DeleteCronJobRuleAsync(CronJobRule cronJobRule)
    //{
    //    try
    //    {
    //        //Step 1: Logging Information
    //        this._logger.LogInformation("CronJobScheduler.DeleteCronJobRuleAsync - In process");

    //        //Step 2: Create SQL Query
    //        var deleteQuery = _sqlQueries["CronJobRule.DeleteCronJobRule"];

    //        //Step 3: Declare new variable
    //        int rowsAffected = 0;

    //        //Step 4: Execute Dapper Query.

    //        rowsAffected = await _dapperEventDbConnection.ExecuteScalarAsync<int>(deleteQuery, cronJobRule);

    //        //Step 5: Assign CronJob Base NotificationId
    //        var baseJobId = $"CronJob-{cronJobRule.Id}";

    //        //Step 6: Remove the Hangfire Cron Jobs
    //        await RemoveOldCronJobByFrequencyAsync(baseJobId, cronJobRule);

    //        //Step 7: Dispatch Events
    //        await DispatchEvents(cronJobRule);

    //        //Step 8: Logging Information Completed
    //        _logger.LogInformation("CronJobScheduler.DeleteCronJobRuleAsync - Completed");

    //        //Step 9: Return rowsAffected
    //        return rowsAffected;
    //    }
    //    catch (Exception ex)
    //    {
    //        //Logging Information : Error Message.
    //        _logger.LogError($"CronJobScheduler.DeleteCronJobRuleAsync - {ex.Message}");
    //        throw;
    //    }
    //}

    //public async Task<List<CronJobRule>> GetCronJobRulesAsync()
    //{
    //    try
    //    {
    //        //Step 1: Logging Information
    //        this._logger.LogInformation("CronJobScheduler.GetCronJobRulesAsync - In process");

    //        //Step 2: Create SQL Query
    //        var getQuery = _sqlQueries["CronJobRule.GetCronJobRules"];

    //        //Step 3: New List of CronJobRule
    //        List<CronJobRule> lst = new List<CronJobRule>();

    //        //Step 4: Execute Dapper Query
    //        lst = (await _dapperEventDbConnection.QueryAsync<CronJobRule>(getQuery)).ToList();

    //        //Step 5: Logging Information Completed
    //        _logger.LogInformation("CronJobScheduler.GetCronJobRulesAsync - Completed");

    //        //Step 6: Return list
    //        return lst;
    //    }
    //    catch (Exception ex)
    //    {
    //        //Logging Information : Error Message.
    //        _logger.LogError($"CronJobScheduler.GetCronJobRulesAsync - {ex.Message}");
    //        throw;
    //    }
    //}

    //public async Task<CronJobRule> GetCronJobRuleByIdAsync(string Id)
    //{
    //    try
    //    {
    //        //Step 1: Logging Information
    //        this._logger.LogInformation("CronJobScheduler.GetCronJobRuleByIdAsync - In process");

    //        //Step 2: Create SQL Query
    //        var getQuery = _sqlQueries["CronJobRule.GetCronJobRuleById"];

    //        //Step 3: New List of CronJobRule
    //        CronJobRule rule = new CronJobRule();

    //        //Step 4: Execute Dapper Query
    //        rule = (await _dapperEventDbConnection.QueryAsync<CronJobRule>(getQuery, new { Id = Id })).FirstOrDefault();

    //        //Step 5: Logging Information Completed
    //        _logger.LogInformation("CronJobScheduler.GetCronJobRulesAsync - Completed");

    //        //Step 6: Return CronJobRuleEntity
    //        return rule;
    //    }
    //    catch (Exception ex)
    //    {
    //        //Logging Information : Error Message.
    //        _logger.LogError($"CronJobScheduler.GetCronJobRulesAsync - {ex.Message}");
    //        throw;
    //    }
    //}
    //#endregion

    #region CronJob Scheduling Methods (used to add/update jobs in Hangfire tables) 
    public async Task UpdateCronJobAsync(CronJobRule oldCronJobRule, CronJobRule newCronJobRule)
    {
        string baseJobId = $"CronJob-{newCronJobRule.Id}";

        HashSet<string> Split(string? value) =>
            string.IsNullOrWhiteSpace(value)
                ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                : value.Split(';', StringSplitOptions.RemoveEmptyEntries)
                       .Select(v => v.Trim())
                       .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var oldDays = Split(oldCronJobRule.ExecutionDay);
        var newDays = Split(newCronJobRule.ExecutionDay);

        // --------------------
        // 1) FREQUENCY CHANGED
        // --------------------
        if (!oldCronJobRule.Frequency.Equals(newCronJobRule.Frequency, StringComparison.OrdinalIgnoreCase))
        {
            await RemoveAllJobsForRule(baseJobId);
        }

        // ---------------------------------------------------------
        // 2) MONTH CHANGED (important: must come BEFORE days check)
        // ---------------------------------------------------------
        else if (newCronJobRule.ExecutionMonth != oldCronJobRule.ExecutionMonth)
        {
            await RemoveAllJobsForRule(baseJobId);
        }

        // ---------------------------------
        // 3) SAME FREQUENCY, DAYS CHANGED
        // ---------------------------------
        else if (!newDays.SetEquals(oldDays))
        {
            await RemoveAllJobsForRule(baseJobId);
        }

        // --------------------
        // 4) ADD / UPDATE JOBS
        // --------------------
        await ScheduleCronJobAsync(newCronJobRule);
    }

    private Task RemoveAllJobsForRule(string baseJobId)
    {
        var connection = JobStorage.Current.GetConnection();
        var jobs = connection.GetRecurringJobs();

        foreach (var job in jobs)
        {
            if (job.Id.StartsWith(baseJobId))
            {
                RecurringJob.RemoveIfExists(job.Id);
            }
        }

        return Task.CompletedTask;
    }

    public async Task RemoveOldCronJobByFrequencyAsync(string baseJobId, CronJobRule rule)
    {
        // Not used anymore (kept for compatibility)
        await RemoveAllJobsForRule(baseJobId);
    }


    public async Task ScheduleCronJobAsync(CronJobRule rule)
    {
        string baseJobId = $"CronJob-{rule.Id}";

        if (rule.Frequency.Equals("Daily", StringComparison.OrdinalIgnoreCase))
        {
            RecurringJob.AddOrUpdate<CronJob>(
                baseJobId,
                job => job.RunCronJob(rule.Id, rule.NotificationName, rule.IsNotificationPaused),
                Cron.Daily(rule.ExecutionTime.Hours, rule.ExecutionTime.Minutes));
        }
        else if (rule.Frequency.Equals("Weekly", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var day in (rule.ExecutionDay ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!Enum.TryParse<DayOfWeek>(day.Trim(), true, out var parsedDay))
                    continue; // ✅ safe parsing

                var cron = Cron.Weekly(
                    parsedDay,
                    rule.ExecutionTime.Hours,
                    rule.ExecutionTime.Minutes);

                RecurringJob.AddOrUpdate<CronJob>(
                    $"{baseJobId}-{parsedDay}",
                    job => job.RunCronJob(rule.Id, rule.NotificationName, rule.IsNotificationPaused),
                    cron);
            }
        }
        else if (rule.Frequency.Equals("Monthly", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var day in (rule.ExecutionDay ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var numericPart = new string(day.Where(char.IsDigit).ToArray());
                if (!int.TryParse(numericPart, out int dayNumber))
                    continue; // ✅ safe parsing

                var cron = Cron.Monthly(
                    dayNumber,
                    rule.ExecutionTime.Hours,
                    rule.ExecutionTime.Minutes);

                RecurringJob.AddOrUpdate<CronJob>(
                    $"{baseJobId}-{dayNumber}",
                    job => job.RunCronJob(rule.Id, rule.NotificationName, rule.IsNotificationPaused),
                    cron);
            }
        }
        else if (rule.Frequency.Equals("Yearly", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var day in (rule.ExecutionDay ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var numericPart = new string(day.Where(char.IsDigit).ToArray());
                if (!int.TryParse(numericPart, out int dayNumber))
                    continue; // ✅ safe parsing

                var cron = $"{rule.ExecutionTime.Minutes} " +
                           $"{rule.ExecutionTime.Hours} " +
                           $"{dayNumber} " +
                           $"{rule.ExecutionMonth} *";

                RecurringJob.AddOrUpdate<CronJob>(
                    $"{baseJobId}-{rule.ExecutionMonth}-{dayNumber}",
                    job => job.RunCronJob(rule.Id, rule.NotificationName, rule.IsNotificationPaused),
                    cron);
            }
        }

        await Task.CompletedTask;
    }
    #endregion

    #region CronJob Rule methods (CRUD to handle the cron job rule updates from user to JobRule database,related to any type of workflow (ex:Notifications))
    public async Task<string> InsertCronJobRuleAsync(CronJobRule cronJobRule)
    {
        try
        {
            this._logger.LogInformation("CronJobScheduler.InsertNewCronJobRuleAsync - In process");

            var insertQuery = _sqlQueries["CronJobRule.InsertCronJobRule"];

            var insertedId = await _dapperEventDbConnection.ExecuteScalarAsync<string>(insertQuery, cronJobRule);
            cronJobRule.Id = insertedId.ToString();

            await ScheduleCronJobAsync(cronJobRule);

            _logger.LogInformation("CronJobScheduler.InsertNewCronJobRuleAsync - Completed");

            await DispatchEvents(cronJobRule);

            return cronJobRule.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError($"CronJobScheduler.InsertNewCronJobRuleAsync - {ex.Message}");
            throw;
        }
    }

    public async Task<int> UpdateCronJobRuleAsync(CronJobRule newCronJobRule, CronJobRule oldCronJobRule)
    {
        try
        {
            this._logger.LogInformation("CronJobScheduler.UpdateCronJobRuleAsync - In process");

            var updateQuery = _sqlQueries["CronJobRule.UpdateCronJobRule"];

            int rowsAffected = await _dapperEventDbConnection.ExecuteAsync(updateQuery, newCronJobRule);

            await DispatchEvents(newCronJobRule);

            await UpdateCronJobAsync(oldCronJobRule, newCronJobRule);

            _logger.LogInformation("CronJobScheduler.UpdateCronJobRuleAsync - Completed");

            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError($"CronJobScheduler.UpdateCronJobRuleAsync - {ex.Message}");
            throw;
        }
    }

    public async Task<int> DeleteCronJobRuleAsync(CronJobRule cronJobRule)
    {
        try
        {
            this._logger.LogInformation("CronJobScheduler.DeleteCronJobRuleAsync - In process");

            var deleteQuery = _sqlQueries["CronJobRule.DeleteCronJobRule"];

            int rowsAffected = await _dapperEventDbConnection.ExecuteScalarAsync<int>(deleteQuery, cronJobRule);

            var baseJobId = $"CronJob-{cronJobRule.Id}";

            await RemoveAllJobsForRule(baseJobId); // ✅ FIXED

            await DispatchEvents(cronJobRule);

            _logger.LogInformation("CronJobScheduler.DeleteCronJobRuleAsync - Completed");

            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError($"CronJobScheduler.DeleteCronJobRuleAsync - {ex.Message}");
            throw;
        }
    }

    public async Task<List<CronJobRule>> GetCronJobRulesAsync()
    {
        try
        {
            this._logger.LogInformation("CronJobScheduler.GetCronJobRulesAsync - In process");

            var getQuery = _sqlQueries["CronJobRule.GetCronJobRules"];

            var lst = (await _dapperEventDbConnection.QueryAsync<CronJobRule>(getQuery)).ToList();

            _logger.LogInformation("CronJobScheduler.GetCronJobRulesAsync - Completed");

            return lst;
        }
        catch (Exception ex)
        {
            _logger.LogError($"CronJobScheduler.GetCronJobRulesAsync - {ex.Message}");
            throw;
        }
    }

    public async Task<CronJobRule> GetCronJobRuleByIdAsync(string Id)
    {
        try
        {
            this._logger.LogInformation("CronJobScheduler.GetCronJobRuleByIdAsync - In process");

            var getQuery = _sqlQueries["CronJobRule.GetCronJobRuleById"];

            var rule = (await _dapperEventDbConnection.QueryAsync<CronJobRule>(getQuery, new { Id })).FirstOrDefault();

            _logger.LogInformation("CronJobScheduler.GetCronJobRulesAsync - Completed");

            return rule;
        }
        catch (Exception ex)
        {
            _logger.LogError($"CronJobScheduler.GetCronJobRulesAsync - {ex.Message}");
            throw;
        }
    }
    #endregion

    #region NotificationUserSubscription
    public async Task<int> UpsertNotificationUserSubscriptionAsync(List<NotificationUserSubscription> notificationUserSubscription, string userEmail)
    {
        string sqlQuery = _sqlQueries["CronJobRule.UpdateNotificationUserSubscriptionByUserId"];
        var rowsAffected = 0;

        // Flatten the list for bulk parameterized update
        var updateData = notificationUserSubscription
            .SelectMany(us => us.SubscriptionDetails.Select(sd => new
            {
                UserId = us.UserId,
                NotificationId = sd.NotificationId,
                OptOut = sd.OptOut,
                CreatedBy = userEmail,
                UpdatedBy = userEmail
            }
            ))
            .ToList();

        var parameters = new
        {
            updateData,
            CratedBy = userEmail
        };

        using (var connection = new NpgsqlConnection(this._configuration["ConnectionStrings:AzureEventDBPostgreSqlDBConnection"]))
        {
            await connection.OpenAsync();

            // Execute the update for all records
            rowsAffected = await connection.ExecuteAsync(sqlQuery, updateData);
        }

        return rowsAffected;
    }

    public async Task<List<NotificationUserSubscription>> GetNotificationUserSubscriptionsAsync(List<string> userIds)
    {
        try
        {
            //Step 1: Logging Information
            this._logger.LogInformation("CronJobScheduler.GetNotificationUserSubscriptionsAsync - In process");

            //Step 2: Create SQL Query
            var getQuery = _sqlQueries["CronJobRule.GetNotificationUserSubscriptions"];

            //Step 3: New List of NotificationRule
            List<NotificationUserSubscription> lst = new List<NotificationUserSubscription>();

            //Step 4: Execute Dapper Query
            using (var connection = new NpgsqlConnection(this._configuration["ConnectionStrings:AzureEventDBPostgreSqlDBConnection"]))
            {
                foreach (var userId in userIds)
                {
                    var result = await connection.QueryAsync(
                        getQuery, new { UserId = userId });

                    if (result.Any())
                    {
                        // Map records into the entity
                        var userSubscriptions = new NotificationUserSubscription
                        {
                            UserId = userId,
                            SubscriptionDetails = result.Select(r => new NotificationSubscriptionDetail
                            {
                                NotificationId = r.NotificationId,
                                OptOut = r.OptOut
                            }).ToList()
                        };

                        lst.Add(userSubscriptions);
                    }
                    else
                    {
                        // Add a blank entity for the user ID
                        lst.Add(new NotificationUserSubscription
                        {
                            UserId = userId,
                            SubscriptionDetails = new List<NotificationSubscriptionDetail>()
                        });
                    }
                }


                //Step 5: Dispatch Events
                //await DispatchEvents(Assignment);

                //Step 6: Logging Information Completed
                _logger.LogInformation("CronJobScheduler.GetNotificationUserSubscriptionsAsync - Completed");

                //Step 10: Return list
                return lst;
            }
        }
        catch (Exception ex)
        {
            //Logging Information : Error Message.
            _logger.LogError($"CronJobScheduler.GetNotificationUserSubscriptionsAsync - {ex.Message}");
            throw;
        }
    }

    public async Task<int> InsertNotificationUserSubscriptionAsync(List<NotificationUserSubscription> notificationUserSubscription)
    {
        string insertQuery = _sqlQueries["CronJobRule.InsertNotificationUserSubscriptionByUserId"];
        var rowsAffected = 0;

        // Flatten the list for bulk parameterized update
        var updateData = notificationUserSubscription
            .SelectMany(us => us.SubscriptionDetails.Select(sd => new
            {
                UserId = us.UserId,
                NotificationId = sd.NotificationId,
                OptOut = sd.OptOut
            }))
            .ToList();

        using (var connection = new SqlConnection(this._configuration["ConnectionStrings:SqlDBConnection"]))
        {
            await connection.OpenAsync();

            // Execute the update for all records
            rowsAffected = await connection.ExecuteAsync(insertQuery, updateData);

        }

        return rowsAffected;
    }
    #endregion

    public async Task DispatchEvents(Domain.Entities.CronJobRule entity)
    {
        while (true)
        {
            var domainEventEntity = entity.DomainEvents
                .Where(domainEvent => !domainEvent.IsPublished)
                .FirstOrDefault();
            if (domainEventEntity == null) break;

            domainEventEntity.IsPublished = true;
            await _domainEventService.Publish(domainEventEntity);
        }
    }

}
