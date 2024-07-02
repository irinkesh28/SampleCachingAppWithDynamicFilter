using FluentValidation;
using SampleCachingApp.DynamicFilter;
using SampleCachingApp.Helpers;

namespace SampleCachingApp.Validator
{
    public class ConditionValidator : AbstractValidator<Condition>
    {
        public ConditionValidator()
        {
            RuleFor(x => x.Column.Name)
                .Must(EmployeeHelpers.IsValidProperty)
                .When(x => x.Column != null && !string.IsNullOrEmpty(x.Column.Name))
                .WithMessage("Column name must be a valid.");

            RuleFor(x => x.Column.DataType)
                .Must((condition, dataType) =>
                    condition.Column == null ||
                    string.IsNullOrEmpty(condition.Column.Name) ||
                    dataType == EmployeeHelpers.GetPropertyType(condition.Column.Name))
                .WithMessage("Column data type must be a valid data type.");
        }
    }
}
