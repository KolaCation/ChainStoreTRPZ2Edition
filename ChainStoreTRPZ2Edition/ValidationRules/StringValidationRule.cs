using System.Globalization;
using System.Windows.Controls;

namespace ChainStoreTRPZ2Edition.ValidationRules
{
    public sealed class StringValidationRule : ValidationRule
    {
        public int MinimumLength { get; set; } = -1;

        public int MaximumLength { get; set; } = -1;

        public string Parameter { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = new ValidationResult(true, null);
            var inputString = (value ?? string.Empty).ToString();
            if (inputString == string.Empty)
                result = new ValidationResult(false, $"{Parameter} is required.");
            else if (inputString.Length < MinimumLength)
                result = new ValidationResult(false, $"{Parameter} must be at least {MinimumLength} chars long.");
            else if (MaximumLength > 0 && inputString.Length > MaximumLength)
                result = new ValidationResult(false, $"{Parameter} must not exceed {MaximumLength} chars.");

            return result;
        }
    }
}