using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ChainStoreTRPZ2Edition.ValidationRules
{
    public sealed class StringValidationRule : ValidationRule
    {
        private int _minimumLength = -1;
        private int _maximumLength = -1;
        private string _parameter;

        public int MinimumLength
        {
            get => _minimumLength;
            set => _minimumLength = value;
        }

        public int MaximumLength
        {
            get => _maximumLength;
            set => _maximumLength = value;
        }

        public string Parameter
        {
            get => _parameter;
            set => _parameter = value;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = new ValidationResult(true, null);
            var inputString = (value ?? string.Empty).ToString();
            if (inputString == string.Empty)
            {
                result = new ValidationResult(false, $"{Parameter} is required.");
            }
            else if (inputString.Length < MinimumLength)
            {
                result = new ValidationResult(false, $"{Parameter} must be at least {MinimumLength} chars long.");
            }
            else if (MaximumLength > 0 && inputString.Length > MaximumLength)
            {
                result = new ValidationResult(false, $"{Parameter} must not exceed {MaximumLength} chars.");
            }

            return result;
        }
    }
}