using System.Collections.Generic;
using System.Linq;

namespace Application.Common.Rules.Engine.Execution
{
    public class RuleExecutionResult
    {
        public string Message { get; set; }
        public bool IsValid { get => !Errors.Any(); }
        public IEnumerable<Error> Errors { get; private set; }

        public RuleExecutionResult()
        {
            Errors = new Error[] { };
        }

        public void Add(Error error)
        {
            var list = new List<Error>(Errors) { error };
            SetErrors(list);
        }

        public void Add(params RuleExecutionResult[] validationResults)
        {
            var list = new List<Error>(Errors);
            foreach (var validation in validationResults)
                list.AddRange(validation.Errors);

            SetErrors(list);
        }

        public void Remove(Error error)
        {
            var list = new List<Error>(Errors);
            list.Remove(error);
            SetErrors(list);
        }

        private void SetErrors(List<Error> errors)
        {
            Errors = errors;

            if (!IsValid)
                Message = errors[0].Message;
        }
    }
}
