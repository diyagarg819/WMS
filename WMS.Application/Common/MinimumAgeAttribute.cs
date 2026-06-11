using System;
using System.ComponentModel.DataAnnotations;

namespace WMS.Application.Common
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
            ErrorMessage = $"Employee must be at least {minimumAge} years old.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dob)
            {
                var today = DateTime.Today;
                int age = today.Year - dob.Year;

                if (dob.Date > today.AddYears(-age))
                {
                    age--;
                }

                if (age < _minimumAge)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
