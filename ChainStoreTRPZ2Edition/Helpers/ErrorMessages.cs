using System;
using System.Collections.Generic;
using System.Text;

namespace ChainStoreTRPZ2Edition.Helpers
{
    public static class ErrorMessages
    {
        public static string Required(string parameter)
        {
            return $"{parameter} is required.";
        }

        public static string StringMinLength(string parameter, int length)
        {
            return $"{parameter} must be at least {length} chars long.";
        }

        public static string StringMaxLength(string parameter, int length)
        {
            return $"{parameter} must not exceed {length} chars.";
        }

        public static string MinValue(string parameter, int value)
        {
            return $"{parameter} minimum value is {value}";
        }

        public static string MaxValue(string parameter, int value)
        {
            return $"{parameter} maximum value is {value}";
        }
    }
}