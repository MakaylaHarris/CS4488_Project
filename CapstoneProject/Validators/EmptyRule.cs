using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CapstoneProject.Validators
{
    class EmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
                if (value == null || value.ToString().Length == 0)
                return new ValidationResult(false, $"This field cannot be empty.");
            else
                return ValidationResult.ValidResult;
        }
    }
}
