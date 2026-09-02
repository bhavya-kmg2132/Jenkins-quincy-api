using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Common.Models
{
    public class Result<T>
    {
        private T _entity;

        public T Entity
        {
            get { return this._entity; }
            set { this._entity = value; }
        }

        public Result()
        {
            Messages = new Dictionary<string, string>();
            SystemLog = new Dictionary<string, string>();
            Errors = new Dictionary<string, string>();
        }

        public string EntityId { get; set; }
        public string Status { get; set; }
        public bool Succeeded { get; set; }
        public Dictionary<string, string> Errors { get; set; }
        public Dictionary<string, string> Messages { get; set; }
        public Dictionary<string, string> SystemLog { get; set; }
        public string CorrelationId { get; set; }
        public string RequestId { get; set; }
        public string JsonObject { get; set; }

        private string _operationSource = string.Empty;
        public string OperationSource
        {
            get
            {
                return this._operationSource;
            }

            set
            {
                // Add your validation checks here
                if ((value != Domain.Common.OperationSource.DAEMON) ||
                    (value != Domain.Common.OperationSource.MOBILE) ||
                    (value != Domain.Common.OperationSource.WEBPAGE))
                {
                    _operationType = value;
                }

                throw new ArgumentOutOfRangeException("OperationType", "Invalid operation type.");
            }
        }

        private string _operationType = string.Empty;
        public string OperationType
        {

            get
            {
                return this._operationType;
            }

            set
            {
                // Add your validation checks here
                if ((value != Domain.Common.OperationType.READ) ||
                    (value != Domain.Common.OperationType.INSERT) ||
                    (value != Domain.Common.OperationType.UPDATE) ||
                    (value != Domain.Common.OperationType.DELETE) ||
                    (value != Domain.Common.OperationType.UPSERT))
                {
                    _operationType = value;
                }

                throw new ArgumentOutOfRangeException("OperationType", "Invalid operation type.");
            }
        }
        public static Result<T> Success()
        {
            return new Result<T>() { Succeeded = true };
        }

        public static Result<T> Failure(List<string> errors)
        {
            // Fix for CS0120 and CS0747:
            // Initialize the Errors dictionary and populate it with the provided errors.
            var result = new Result<T>() { Succeeded = false };
            result.Errors = errors.ToDictionary(error => Guid.NewGuid().ToString(), error => error);
            return result;
        }
    }
}
