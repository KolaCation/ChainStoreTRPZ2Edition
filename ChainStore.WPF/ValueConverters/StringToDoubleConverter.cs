﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace ChainStoreTRPZ2Edition.ValueConverters
{
    public sealed class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value == null ? string.Empty : value.ToString();
            if (double.TryParse(stringValue, out var result))
            {
                return result;
            }

            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value == null ? string.Empty : value.ToString();
            if (double.TryParse(stringValue, out var result))
            {
                return result;
            }

            return 0.0;
        }
    }
}