using System;
using System.Collections.Generic;
using System.Text;

namespace ChainStoreTRPZ2Edition.Helpers
{
    public sealed class CustomValidationResult
    {
        public string ErrorMessage { get; private set; }
        public bool IsValid { get; }

        public CustomValidationResult(string errorMessage, bool isValid)
        {
            ErrorMessage = errorMessage;
            IsValid = isValid;
        }

        public void AppendErrorMessageContent(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ErrorMessage += $" {errorMessage}";
            }
        }
    }
}
