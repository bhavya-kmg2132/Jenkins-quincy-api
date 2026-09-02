using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Dto;
using RulesEngine.Actions;
using RulesEngine.Models;

namespace Application.Common.RuleEngine
{
    public class RuleEngine : IRuleEngine
    {
        public async Task<List<RuleEngineResult>> Run(object input, string jsonFile, string workflowName)
        {
            try
            {
                var settings = new ReSettings
                {
                    CustomActions = new Dictionary<string, Func<ActionBase>>
                    {
                        { "CustomAction", () => new CustomAction() }
                    }
                };

                //string json = File.ReadAllText(jsonFile);
                //string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jsonFile));
                string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, jsonFile));



                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    PropertyNameCaseInsensitive = true
                };

                var workflows = JsonSerializer.Deserialize<List<Workflow>>(json, options);
                var engine = new RulesEngine.RulesEngine(workflows.ToArray(), settings);

                List<RuleResultTree> ruleresult;

                // If caller passed an object[] we need to expand that array as inputs to the rules engine.
                if (input is object[] inputsArray)
                {
                    ruleresult = await engine.ExecuteAllRulesAsync(workflowName, inputsArray);
                }
                else
                {
                    ruleresult = await engine.ExecuteAllRulesAsync(workflowName, input);
                }

                var _result = Transform(ruleresult);
                return _result;
            }

            catch (Exception)
            {
                // Preserve original exception for diagnostics
                throw;
            }
        }

        private static RuleEngineResult Transform(RuleResultTree ruleResultTree)
        {
            if (ruleResultTree == null) return null;

            var message = ruleResultTree.ActionResult?.Output as string
                          ?? (!ruleResultTree.IsSuccess ? ruleResultTree.Rule.ErrorMessage : ruleResultTree.Rule.SuccessEvent);

            var model = new RuleEngineResult
            {
                RuleName = ruleResultTree.Rule.RuleName,
                IsSuccess = ruleResultTree.IsSuccess,
                Outcome = message,
                ChildResult = new List<RuleEngineResult>()
            };

            if (ruleResultTree.ChildResults != null && ruleResultTree.ChildResults.Any())
            {
                foreach (var child in ruleResultTree.ChildResults)
                {
                    model.ChildResult.Add(Transform(child));
                }
            }

            return model;
        }

        private static List<RuleEngineResult> Transform(List<RuleResultTree> ruleResultTrees)
        {
            var models = new List<RuleEngineResult>();

            foreach (var ruleResultTree in ruleResultTrees)
            {
                models.Add(Transform(ruleResultTree));
            }

            return models;
        }
    }
}
