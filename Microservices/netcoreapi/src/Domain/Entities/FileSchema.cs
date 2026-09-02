using System.Collections.Generic;

namespace Domain.Entities
{
    public class Rules
    {
        public string rule_key { get; set; }
        public string rule_type { get; set; }
        public string output_column { get; set; }
        public string input_column { get; set; }
        public string default_value { get; set; }
        public double confidence_score { get; set; }
        public int mapping_score { get; set; }  //matching_score
        public int matching_score { get; set; }  //matching_score
        public int data_score { get; set; }  //data_score
        public int model_score { get; set; }
        public bool error_override_with_default_value { get; set; }
        public int rule_section { get; set; }
        public int rule_sequence { get; set; }
        public bool is_active { get; set; }
        public bool is_required { get; set; }
        public string description { get; set; }

    }
    public class FileSchema
    {
        public string id { get; set; }
        public string benchmark_schema_id { get; set; }
        public string mapping_approval_status { get; set; }
        public Dictionary<string, string> fields { get; set; }
        public List<Rules> rules { get; set; }
        public int mapping_count { get; set; }
        public double avg_confidence_score { get; set; }
    }
}
