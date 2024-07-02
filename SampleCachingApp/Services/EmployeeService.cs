using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SampleCachingApp.DynamicFilter;
using SampleCachingApp.Helpers;

namespace SampleCachingApp.Services
{
    public class EmployeeService
    {
        private readonly SampleCachingAppContext _sampleCachingAppContext;
        private readonly IConditionToCodeBuilder _conditionToCodeBuilder;
        private readonly IMemoryCache _cache;
        private static readonly Dictionary<DataType, SqliteType> DataTypeToSqliteType = new()
        {
            { DataType.String, SqliteType.Text},
            { DataType.Boolean, SqliteType.Integer},
            { DataType.Int, SqliteType.Integer },
            { DataType.Decimal, SqliteType.Real}
        };

        public EmployeeService(SampleCachingAppContext sampleCachingAppContext, IConditionToCodeBuilder conditionToCodeBuilder,
            IMemoryCache cache)
        {
            _sampleCachingAppContext = sampleCachingAppContext;
            _conditionToCodeBuilder = conditionToCodeBuilder;
            _cache = cache;
        }

        public List<Employee> GetEmployees(Dictionary<string, string> filters, int pageNo, int pageSize, string sortProperty, bool ascendingSort)
        {
            // Get employees from database based on provided parameters
            var employeesQueryable = _sampleCachingAppContext.Employee.AsQueryable();

            // Apply filters
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "name":
                        employeesQueryable = employeesQueryable.Where(x => x.Name.Contains(filter.Value));
                        break;
                    case "designation":
                        employeesQueryable = employeesQueryable.Where(x => x.Designation.Contains(filter.Value));
                        break;
                        // Add more cases for other filterable properties as needed
                }
            }

            // Apply sorting
            switch (sortProperty.ToLower())
            {
                case "name":
                    employeesQueryable = ascendingSort ? employeesQueryable.OrderBy(x => x.Name) : employeesQueryable.OrderByDescending(x => x.Name);
                    break;
                case "designation":
                    employeesQueryable = ascendingSort ? employeesQueryable.OrderBy(x => x.Designation) : employeesQueryable.OrderByDescending(x => x.Designation);
                    break;
                case "id":
                default:
                    employeesQueryable = ascendingSort ? employeesQueryable.OrderBy(x => x.Id) : employeesQueryable.OrderByDescending(x => x.Id);
                    break;
            }

            // Apply paging
            employeesQueryable = employeesQueryable.Skip(pageNo * pageSize).Take(pageSize);

            return employeesQueryable.ToList();
        }

        public List<Employee> FilterEmployees(EmployeeQueryParameters employeeQueryParameters)
        {
            string filterString = JsonConvert.SerializeObject(employeeQueryParameters);
            List<Employee> cachedEmployees = _cache.Get<List<Employee>>(filterString);

            if (cachedEmployees is not null)
                return cachedEmployees;

            var parameters = new List<SqliteParameter>();
            var logicalExpressionCode = string.Empty;
            var orderBy = string.Empty;

            if (employeeQueryParameters.Condition is not null && employeeQueryParameters.Condition.Conditions.Count > 0)
            {
                ValidateColumns(employeeQueryParameters.Condition.Conditions);
                var queryCode = _conditionToCodeBuilder.BuildLogicalExpressionCodeAndParameter(employeeQueryParameters.Condition);
                logicalExpressionCode = !string.IsNullOrEmpty(queryCode.LogicalExpressionCode) ? $" WHERE {queryCode.LogicalExpressionCode}" : string.Empty;
                parameters = queryCode.InputParameters.Select(r =>
                new SqliteParameter(r.ParameterName, DataTypeToSqliteType[r.DataType]) { Value = r.Value })
                    .ToList();
            }

            if (!string.IsNullOrEmpty(employeeQueryParameters.SortProperty))
                orderBy = _conditionToCodeBuilder.BuildOrderBy(employeeQueryParameters.SortProperty, employeeQueryParameters.AscendingSort);


            parameters.Add(new SqliteParameter
            {
                ParameterName = "skip",
                Value = ((employeeQueryParameters.PageNo - 1) * employeeQueryParameters.PageSize).ToString(),
                SqliteType = SqliteType.Integer
            });
            parameters.Add(new SqliteParameter
            {
                ParameterName = "take",
                Value = employeeQueryParameters.PageSize.ToString(),
                SqliteType = SqliteType.Integer
            });

            var rawQuery = $"Select * from Employee {logicalExpressionCode} {orderBy} LIMIT @take OFFSET @skip";

            var sqlLiteQuery = _sampleCachingAppContext.Employee.FromSqlRaw(rawQuery, parameters.ToArray());

            var employees = sqlLiteQuery.AsNoTracking().ToList();

            _cache.Set(filterString, employees, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            });
            GlobalCacheService.globalCacheService.Keys.Add(filterString);

            return employees;
        }

        public static void ValidateColumns(List<Condition> conditions)
        {
            var validColumns = EmployeeHelpers.Columns();

            bool IsValidCondition(Condition condition) =>
                (condition.Column == null || validColumns.Contains(condition.Column.Name, StringComparer.InvariantCultureIgnoreCase))
                && condition.Conditions.All(IsValidCondition);

            if (conditions.Any(condition => !IsValidCondition(condition)))
            {
                throw new InvalidOperationException("Any of the given columns is not a valid column.");
            }
        }
    }
}
