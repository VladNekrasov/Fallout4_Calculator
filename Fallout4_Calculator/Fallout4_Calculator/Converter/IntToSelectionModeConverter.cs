using System;
using System.Globalization;
using Xamarin.Forms;

namespace Fallout4_Calculator.Converter
{
    public class IntToSelectionModeConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int x = (int)value;
            if (x==0)
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
