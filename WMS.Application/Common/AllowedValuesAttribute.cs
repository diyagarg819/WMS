using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WMS.Application.Common
{
    public class AllowedValuesAttribute : ValidationAttribute
    {
        private readonly string[] _allowedValues;

        public AllowedValuesAttribute(params string[] allowedValues)
        {
            _allowedValues = allowedValues;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var strValue = value.ToString();
                if (!_allowedValues.Contains(strValue))
                {
                    return new ValidationResult(ErrorMessage ?? $"Invalid value. Allowed values are: {string.Join(", ", _allowedValues)}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
