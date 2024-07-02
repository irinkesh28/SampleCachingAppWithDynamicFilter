namespace SampleCachingApp.DynamicFilter
{
    public interface ILogicalExpressionBuilder
    {
        public CodeAndParameter BuildEquals(string field, DataType dataType, string value);
        public CodeAndParameter BuildNotEquals(string field, DataType dataType, string value);
        public CodeAndParameter BuildContains(string field, DataType dataType, string value);
        public CodeAndParameter BuildNotContains(string field, DataType dataType, string value);
        public CodeAndParameter BuildStartsWith(string field, DataType dataType, string value);
        public CodeAndParameter BuildEndsWith(string field, DataType dataType, string value);
        public CodeAndParameter BuildGreaterThan(string field, DataType dataType, string value);
        public CodeAndParameter BuildGreaterThanOrEqualTo(string field, DataType dataType, string value);
        public CodeAndParameter BuildLessThan(string field, DataType dataType, string value);
        public CodeAndParameter BuildLessThanOrEqualTo(string field, DataType dataType, string value);
        public string BuildOrderBy(string field, bool ascendingSort);
    }
}
