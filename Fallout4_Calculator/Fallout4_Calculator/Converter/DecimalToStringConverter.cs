using System;
using System.Globalization;
using Xamarin.Forms;

namespace Fallout4_Calculator.Converter
{
    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal x = (decimal)value;
            return x.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
