
namespace SampleCachingApp.DynamicFilter
{
    public class ConditionToCodeBuilder : IConditionToCodeBuilder
    {
        private readonly ILogicalExpressionBuilder _logicalExpressionBuilder;
        public ConditionToCodeBuilder(ILogicalExpressionBuilder logicalExpressionBuilder) 
        {
            _logicalExpressionBuilder = logicalExpressionBuilder;
        }

        public CodeAndParameter BuildLogicalExpressionCodeAndParameter(Condition condition)
        {
            var sqlCodeAndParameters = BuildConditionCodeAndParameter(condition);
            var sqlText = BuildOr([sqlCodeAndParameters.LogicalExpressionCode]);
            var sqlCode = BuildAnd([sqlText]);
            
            return new CodeAndParameter(sqlCode, sqlCodeAndParameters.InputParameters);
        }

        public string BuildOrderBy(string field, bool ascendingSort)
        {
            return _logicalExpressionBuilder.BuildOrderBy(field, ascendingSort);
        }

        private CodeAndParameter BuildConditionCodeAndParameter(Condition condition)
        {
            if (condition.Type == ConditionType.Single)
            {
                return BuildSingleConditionToCode(condition);
            }
            var nestedCondition = condition.Conditions.Select(r => BuildConditionCodeAndParameter(r)).ToList();
            var codeForGroup = BuildGroupConditionsCode(condition, nestedCondition);
            return new CodeAndParameter(codeForGroup,
                nestedCondition.SelectMany(r => r.InputParameters).ToList()); ;
        }

        private string BuildGroupConditionsCode(Condition condition, List<CodeAndParameter> codeAndParameters)
        {
            if (condition.LogicalOperator == null)
                throw new InvalidOperationException("Could not find the logical operator");
            var groupCode = condition.LogicalOperator == LogicalOperator.And ?
                BuildAnd(codeAndParameters.Select(r => r.LogicalExpressionCode).ToList()) :
                BuildOr(codeAndParameters.Select(r => r.LogicalExpressionCode).ToList());
            return groupCode;
        }

        private static string BuildAnd(IList<string> terms)
        {
            if (!terms.Any() || terms.All(t => string.IsNullOrEmpty(t)))
                return string.Empty;
            if (terms.Count == 1)
                return terms.First();
            if (terms.Count(t => !string.IsNullOrEmpty(t)) == 1)
                return terms.FirstOrDefault(t => !string.IsNullOrEmpty(t));
            return $"({string.Join(" AND ", terms.Where(t=>!string.IsNullOrEmpty(t)))})";
        }

        private static string BuildOr(IList<string> terms)
        {
            if (!terms.Any() || terms.All(t => string.IsNullOrEmpty(t)))
                return string.Empty;
            if (terms.Count == 1)
                return terms.First();
            if (terms.Count(t => !string.IsNullOrEmpty(t)) == 1)
                return terms.FirstOrDefault(t => !string.IsNullOrEmpty(t));
            return $"({string.Join(" OR ", terms.Where(t => !string.IsNullOrEmpty(t)))})";
        }

        private CodeAndParameter BuildSingleConditionToCode(Condition condition)
        {
            return condition.ConditionOperator switch
            {
                ConditionOperator.Equals => _logicalExpressionBuilder.BuildEquals(condition.Column.Name, condition.Column.DataType, condition.Values[0]),
                ConditionOperator.NotEquals => _logicalExpressionBuilder.BuildNotEquals(condition.Column.Name, condition.Column.DataType, condition.Values[0]),
                ConditionOperator.Contains => _logicalExpressionBuilder.BuildContains(condition.Column.Name, condition.Column.DataType, condition.Values[0]),
                ConditionOperator.NotContains => _logicalExpressionBuilder.BuildNotContains(condition.Column.Name, condition.Column.DataType, condition.Values[0]),
                ConditionOperator.StartsWith => _logicalExpressionBuilder.BuildStartsWith(condition.Column.Name, condition.Column.DataType, condition.Values[0]),
                ConditionOperator.EndsWith => _logicalExpressionBuilder.BuildEndsWith(condition.Column.Name, condition.Column.DataType, condition.Values[0]),
                ConditionOperator.IsGreaterThan => _logicalExpressionBuilder.BuildGreaterThan(condition.Column.Name, condition.Column.DataType, condition.Values[0]),
                ConditionOperator.IsGreaterThanOrEqualTo => _logicalExpressionBuilder.BuildGreaterThanOrEqualTo(condition.Column.Name, condition.Column.DataType, condition.Values[0]),
                ConditionOperator.IsLessThan => _logicalExpressionBuilder.BuildLessThan(condition.Column.Name, condition.Column.DataType, condition.Values[0]),
                ConditionOperator.IsLessThanOrEqualTo => _logicalExpressionBuilder.BuildLessThanOrEqualTo(condition.Column.Name, condition.Column.DataType, condition.Values[0]),
                _ => throw new InvalidOperationException("Could not find the operator")
            };
        }
    }
}
