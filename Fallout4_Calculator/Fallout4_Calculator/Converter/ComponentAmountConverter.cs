﻿using System;
using Fallout4_Calculator.Models;
using Xamarin.Forms;

namespace Fallout4_Calculator.Converter
{
    public class ComponentAmountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] != null && values[1] != null && values.Length == 2)
            {
                var t = values[0] as Components_A;
                var c = values[1] as Entry;
                return new ComponentWithEntry { components=t, entry=c} ;
            }
            return null;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
