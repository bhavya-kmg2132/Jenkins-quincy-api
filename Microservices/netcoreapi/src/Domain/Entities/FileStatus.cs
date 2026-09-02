using System;
using System.Collections.Generic;

public class FileStatus
{

    public string Id { get; set; }
    public string input_file_name { get; set; }
    public string output_file_name { get; set; }
    public string errorrecords_file_name { get; set; }
    public string schema_file_name { get; set; }
    public string log_file_name { get; set; }
    public string status { get; set; }
    public string file_schema_id { get; set; }
    public string benchmark_schema_id { get; set; }
    public string benchmark_schema_name { get; set; }
    public List<error_record> error_record { get; set; }
    public List<string> log_record { get; set; }
    public List<Stage> stage { get; set; }
    public string rule_engine_result { get; set; }
    public string rule_engine { get; set; }
    public DateTime modified_date_time { get; set; }
    public DateTime created_date_time { get; set; }
    public string responseTime { get; set; }
    public List<Premium> premium { get; set; }
    public RuleEngineProgress rule_engine_progress { get; set; }

    // Constructor to initialize the lists
    public FileStatus()
    {
        error_record = new List<error_record>();
        log_record = new List<string>();
        premium = new List<Premium>();
        rule_engine_progress = new RuleEngineProgress();
    }
    public class Stage
    {
        public string name { get; set; }
        public string time { get; set; }
    }
    public class Premium
    {
        public double input_premium { get; set; }
        public double output_premium { get; set; }
    }
}

public class error_record
{
    public string record_unique_id { get; set; }
    public string error_description { get; set; }
}


public class RuleEngineProgress
{
    public RuleEngineStatus input { get; set; }
    public RuleEngineStatus output { get; set; }

    public RuleEngineProgress()
    {
        input = new RuleEngineStatus();
        output = new RuleEngineStatus();
    }
}

public class RuleEngineStatus
{
    public int ruleCount { get; set; }
    public string ruleset_name { get; set; }
    public int error_count { get; set; }
    public string result_blob_url { get; set; }
    public List<string> header { get; set; } // stores input/Output headers
    public string status { get; set; }  // Pending, InProgress, Completed
}
