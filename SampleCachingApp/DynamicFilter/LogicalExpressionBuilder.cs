namespace SampleCachingApp.DynamicFilter
{
    public class LogicalExpressionBuilder : ILogicalExpressionBuilder
    {
        public CodeAndParameter BuildEquals(string field, DataType dataType, string value)
        {
            var sqlLiteParam = new InputParameter(GetRandomParameterName(), value, dataType);

            return new CodeAndParameter($"{field} = @{sqlLiteParam.ParameterName}", [sqlLiteParam]);
        }

        public CodeAndParameter BuildNotEquals(string field, DataType dataType, string value)
        {
            var sqlLiteParam = new InputParameter(GetRandomParameterName(), value, dataType);

            return new CodeAndParameter($"{field} <> @{sqlLiteParam.ParameterName}", [sqlLiteParam]);
        }

        public CodeAndParameter BuildContains(string field, DataType dataType, string value)
        {
            var sqlLiteParam = new InputParameter(GetRandomParameterName(), value, dataType);

            return new CodeAndParameter($"({field} LIKE '%' || @{sqlLiteParam.ParameterName} || '%')", [sqlLiteParam]);
        }

        public CodeAndParameter BuildNotContains(string field, DataType dataType, string value)
        {
            var sqlLiteParam = new InputParameter(GetRandomParameterName(), value, dataType);

            return new CodeAndParameter($"(ifnull({field},'') NOT LIKE '%' || @{sqlLiteParam.ParameterName} || '%')", [sqlLiteParam]);
        }

        public CodeAndParameter BuildStartsWith(string field, DataType dataType, string value)
        {
            var sqlLiteParam = new InputParameter(GetRandomParameterName(), value, dataType);

            return new CodeAndParameter($"(ifnull({field},'') LIKE @{sqlLiteParam.ParameterName} || '%')", [sqlLiteParam]);
        }

        public CodeAndParameter BuildEndsWith(string field, DataType dataType, string value)
        {
            var sqlLiteParam = new InputParameter(GetRandomParameterName(), value, dataType);

            return new CodeAndParameter($"(ifnull({field},'') LIKE '%' || @{sqlLiteParam.ParameterName})", [sqlLiteParam]);
        }

        public CodeAndParameter BuildGreaterThan(string field, DataType dataType, string value)
        {
            ValidateNumericInput(field, dataType, ConditionOperator.IsGreaterThan, value);

            var sqlLiteParam = new InputParameter(GetRandomParameterName(), value, dataType == DataType.String ? DataType.Decimal : dataType);

            return new CodeAndParameter($"({CastNumeric(field)} > @{sqlLiteParam.ParameterName})", [sqlLiteParam]);
        }

        public CodeAndParameter BuildGreaterThanOrEqualTo(string field, DataType dataType, string value)
        {
            ValidateNumericInput(field, dataType, ConditionOperator.IsGreaterThanOrEqualTo, value);

            var sqlLiteParam = new InputParameter(GetRandomParameterName(), value, dataType == DataType.String ? DataType.Decimal : dataType);

            return new CodeAndParameter($"({CastNumeric(field)} >= @{sqlLiteParam.ParameterName})", [sqlLiteParam]);
        }

        public CodeAndParameter BuildLessThan(string field, DataType dataType, string value)
        {
            ValidateNumericInput(field, dataType, ConditionOperator.IsLessThan , value);

            var sqlLiteParam = new InputParameter(GetRandomParameterName(), value, dataType == DataType.String ? DataType.Decimal : dataType);

            return new CodeAndParameter($"({CastNumeric(field)} < @{sqlLiteParam.ParameterName})", [sqlLiteParam]);
        }

        public CodeAndParameter BuildLessThanOrEqualTo(string field, DataType dataType, string value)
        {
            ValidateNumericInput(field, dataType, ConditionOperator.IsLessThanOrEqualTo, value);

            var sqlLiteParam = new InputParameter(GetRandomParameterName(), value, dataType == DataType.String ? DataType.Decimal : dataType);

            return new CodeAndParameter($"({CastNumeric(field)} <= @{sqlLiteParam.ParameterName})", [sqlLiteParam]);
        }

        public string BuildOrderBy(string field, bool ascendingSort)
        {
            var direction = ascendingSort ? "ASC" : "DESC";
            return $"ORDER BY {field} {direction}";
        }

        private static string GetRandomParameterName()
        {
            return $"parameter{Guid.NewGuid().ToString("N")}";
        }

        private static void ValidateNumericInput(string field, DataType dataType, ConditionOperator conditionOperator, string value)
        {
            if (dataType is DataType.Boolean)
                throw new InvalidOperationException($"Cannot filter with {Enum.GetName(typeof(ConditionOperator), conditionOperator)} condition, becuase the column {field} is boolean");
            if (!decimal.TryParse(value, out _))
                throw new InvalidOperationException($"Cannot filter with {Enum.GetName(typeof(ConditionOperator), conditionOperator)} condition, becuase the value {value} is not numeric.");
        }

        private static string CastNumeric(string field)
        {
            return $"typeof({field}) IN ('integer', 'real') AND CAST({field} AS REAL)";
        }
    }
}
