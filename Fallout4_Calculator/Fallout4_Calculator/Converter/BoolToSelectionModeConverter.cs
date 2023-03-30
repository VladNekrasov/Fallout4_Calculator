using System;
using System.Globalization;
using Xamarin.Forms;

namespace Fallout4_Calculator.Converter
{
    public class BoolToSelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool x = (bool)value;
            if (!x)
                return Xamarin.Forms.SelectionMode.Single;
            else
                return Xamarin.Forms.SelectionMode.Multiple;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
