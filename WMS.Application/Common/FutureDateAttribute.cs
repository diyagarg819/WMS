using System;
using System.ComponentModel.DataAnnotations;

namespace WMS.Application.Common
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date < DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} cannot be in the past.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
