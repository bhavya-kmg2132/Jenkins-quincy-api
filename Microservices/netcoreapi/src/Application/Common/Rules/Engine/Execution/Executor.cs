using System;
using System.Collections.Generic;
using Application.Common.Rules.Engine.Interfaces.Execution;

namespace Application.Common.Rules.Engine.Execution
{
    public class Executor<TEntity> : IExecutor<TEntity> where TEntity : class
    {
        private readonly Dictionary<string, IRule<TEntity>> _rules;

        public Executor() => _rules = new Dictionary<string, IRule<TEntity>>();

        public RuleExecutionResult Execute(TEntity entity)
        {
            var validation = new RuleExecutionResult();
            foreach (var rule in _rules)
                if (!rule.Value.Execute(entity))
                    validation.Add(new Error(rule.Key, rule.Value.Message));

            return validation;
        }

        public RuleExecutionResult Execute(TEntity entity, bool throwError)
        {
            var ruleExecutionResult = new RuleExecutionResult();
            foreach (var rule in _rules)
                if (!rule.Value.Execute(entity))
                    ruleExecutionResult.Add(new Error(rule.Key, rule.Value.Message));

            if (throwError)
            {
                if (!ruleExecutionResult.IsValid)
                {
                    List<string> errorList = new List<string>();
                    foreach (var error in ruleExecutionResult.Errors)
                        errorList.Add(error.Message);

                    throw new ApplicationException(string.Join(",", errorList));
                }
            }

            return ruleExecutionResult;
        }

        protected virtual void Add(string name, IRule<TEntity> rule) => _rules.Add(name, rule);

        protected IRule<TEntity> GetRule(string name) => _rules[name];

        protected virtual void Remove(string name) => _rules.Remove(name);
    }
}
