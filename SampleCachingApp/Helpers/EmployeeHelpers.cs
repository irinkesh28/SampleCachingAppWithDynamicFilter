using SampleCachingApp.DynamicFilter;

namespace SampleCachingApp.Helpers
{
    public static class EmployeeHelpers
    {
        public static bool IsValidProperty(string propertyName)
        {
            return typeof(Employee).GetProperties().Any(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static List<string> Columns()
        {
            return typeof(Employee).GetProperties().Select(p => p.Name).ToList();
        }

        public static DataType GetPropertyType(string propertyName)
        {
            var property = typeof(Employee).GetProperties()
                .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            if (property == null)
            {
                throw new InvalidOperationException($"Property '{propertyName}' not found on {nameof(Employee)}.");
            }

            return GetMappedType(property.PropertyType);
        }

        private static DataType GetMappedType(Type type)
        {
            if (type == typeof(string)) return DataType.String;
            if (type == typeof(int)) return DataType.Int;
            if (type == typeof(decimal)) return DataType.Decimal;
            if (type == typeof(bool)) return DataType.Boolean;
            throw new NotSupportedException($"Type '{type.Name}' is not supported.");
        }
    }
}
