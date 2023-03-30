using System;
using System.Globalization;
using Xamarin.Forms;

namespace Fallout4_Calculator.Converter
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int x = (int)value;
            return x.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
