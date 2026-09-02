using System;
using System.Collections.Generic;

namespace Application.Common.Models
{
    public class ValidateProspectResponseModel
    {
        public ValidateProspectResponseModel()
        {
            ValidProspects = new List<object>();
            InValidProspects = new List<object>();
        }
        public List<Object> ValidProspects { get; set; }
        public List<Object> InValidProspects { get; set; }
        public int TotalRecord { get; set; }
        public int MappedRecord { get; set; }
        public bool IsMappedInDatabase { get; set; } = false;
        public string FileId { get; set; }
        public bool IsSuccess { get; set; } = false;
        public List<string> Messages { get; set; }
    }
}
