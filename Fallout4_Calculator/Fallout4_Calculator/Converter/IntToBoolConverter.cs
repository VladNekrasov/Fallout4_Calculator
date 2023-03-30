using System;
using System.Globalization;
using Xamarin.Forms;

namespace Fallout4_Calculator.Converter
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int x = (int)value;
            if (x==0)
                return false;
            else
                return true;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
