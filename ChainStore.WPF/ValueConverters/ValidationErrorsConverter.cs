using System;
using System.Globalization;
using System.Windows.Data;

namespace ChainStoreTRPZ2Edition.ValueConverters
{
    public sealed class ValidationErrorsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.LongLength > 0)
            {
                foreach (var value in values)
                {
                    if (value is bool b && b)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new ApplicationException();
        }
    }
}