using FluentValidation;
using SampleCachingApp.DynamicFilter;
using SampleCachingApp.Helpers;

namespace SampleCachingApp.Validator
{
    public class EmployeeQueryParametersValidator : AbstractValidator<EmployeeQueryParameters>
    {
        public EmployeeQueryParametersValidator() 
        {
            RuleFor(r => r.PageNo).GreaterThan(0).WithMessage($"{nameof(EmployeeQueryParameters.PageNo)} must be greater than 0");
            RuleFor(r => r.PageSize).GreaterThan(0).WithMessage($"{nameof(EmployeeQueryParameters.PageSize)} must be greater than 0");
            RuleFor(x => x.SortProperty)
                .Must(EmployeeHelpers.IsValidProperty)
                .When(x => !string.IsNullOrEmpty(x.SortProperty))
                .WithMessage($"'{nameof(EmployeeQueryParameters.SortProperty)}' must be a valid property of Employee.");

           RuleFor(x => x.Condition)
                .SetValidator(new ConditionValidator())
                .When(x=>x.Condition.Type == ConditionType.Single);
        }
    }
}