using SampleCachingApp.DynamicFilter;

namespace SampleCachingApp
{
    public class EmployeeQueryParameters
    {
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string SortProperty { get; set; }
        public bool AscendingSort { get; set; }
        public Condition? Condition { get; set; }
    }
}
