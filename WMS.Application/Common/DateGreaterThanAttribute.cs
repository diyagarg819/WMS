using System;
using System.ComponentModel.DataAnnotations;

namespace WMS.Application.Common
{
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                return new ValidationResult($"Property {_comparisonProperty} not found.");

            var comparisonValue = property.GetValue(validationContext.ObjectInstance);

            if (value is DateTime currentDate && comparisonValue is DateTime comparisonDate)
            {
                if (currentDate.Date < comparisonDate.Date)
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} cannot be earlier than {_comparisonProperty}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
