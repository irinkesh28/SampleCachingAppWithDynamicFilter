namespace SampleCachingApp.DynamicFilter
{
    public interface IConditionToCodeBuilder
    {
        public CodeAndParameter BuildLogicalExpressionCodeAndParameter(Condition condition);
        public string BuildOrderBy(string field, bool ascendingSort);
    }
}
