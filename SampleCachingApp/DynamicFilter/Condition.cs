using Microsoft.Data.Sqlite;
using System.Data;

namespace SampleCachingApp.DynamicFilter
{
    public class Condition
    {
        public ConditionType Type { get; set; }
        public LogicalOperator? LogicalOperator { get; set; }
        public Column? Column { get; set; }
        public ConditionOperator ConditionOperator { get; set; }
        public List<string> Values { get; set; } = [];
        public List<Condition> Conditions { get; set; } = [];
    }

    public class Column
    {
        public required string Name { get; set; }
        public DataType DataType { get; set; }
    }

    public enum ConditionType
    {
        Group,
        Single
    }

    public enum LogicalOperator
    {
        And,
        Or
    }

    public enum DataType
    {
        String,
        Int,
        Decimal,
        Boolean
    }

    public enum ConditionOperator
    {
        Equals,
        NotEquals,
        Contains,
        NotContains,
        StartsWith,
        EndsWith,
        IsGreaterThan,
        IsGreaterThanOrEqualTo,
        IsLessThan,
        IsLessThanOrEqualTo
    }

    public class CodeAndParameter(string logicalExpressionCode, IList<InputParameter> inputParameters)
    {
        public string LogicalExpressionCode { get; set; } = logicalExpressionCode;
        public IList<InputParameter> InputParameters { get; set; } = inputParameters;
    }

    public class InputParameter(string parameterName, string value, DataType dataType)
    {
        public string ParameterName { get; set; } = parameterName;
        public string Value { get; set; } = value;
        public DataType DataType { get; set; } = dataType;
    }
}
