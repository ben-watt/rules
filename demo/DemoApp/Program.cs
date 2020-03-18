// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoApp
{
    class Shipment
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    internal static class Program
    {
        public static void Main(string[] args)
        {
            #region And Or Logic
            // Child Rules could imply a AND clause whereas sibling rules could be and OR operator
            // If a parent rule is false then the rule doesn't apply however you can keep
            // going down the tree and if a leaf node and it's siblings are true you should have
            // a valid result. 
            //
            // Example:
            // 
            // Below are two rules each with child rules
            // the first top level rule ends up being false.
            // the last top level rule ends up being true.

            // |- True (Not same as origin)
            //      - True  (Over 200)
            //          - True  (In GBP)
            //              - False (To France)
            // |- True (Not same as origin)
            //      - True (Over 200)
            //          - True (In GBP)
            //              - True (To Spain)
            #endregion

            var workflowRules = new List<WorkflowRules>()
            {
                new WorkflowRules()
                {
                    WorkflowName = "generate-cn22",
                    Rules = new List<Rule>()
                    {
                        new Rule()
                        {
                            RuleName = "From GB Under 100 Pounds",
                            RuleExpressionType = RuleExpressionType.LambdaExpression,
                            Expression = "Origin == \"GB\" && Amount > 100",
                        },
                        new Rule()
                        {
                            RuleName = "Destination is not the same as origin",
                            RuleExpressionType = RuleExpressionType.LambdaExpression,
                            Expression = "Origin != Destination",
                        }
                    }
                }
            };

            var shipment = new Shipment()
            {
                Origin = "GB",
                Destination = "XY",
                Amount = 200,
                Currency = "GBP"
            };

            var engine = new RulesEngine.RulesEngine(workflowRules.ToArray(), null);
            var resultList = engine.ExecuteRule("generate-cn22", shipment);

            var passedRules = resultList.Count(x => x.IsSuccess);
            Console.Write(passedRules);
        }
    }
}
