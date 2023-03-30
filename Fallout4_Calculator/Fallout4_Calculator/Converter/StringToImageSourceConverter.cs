using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace Fallout4_Calculator.Converter
{
    public class StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Source = (string)value;
            if (Source == null)
            {
                return null;
            }
            ImageSource imageSource;
            Uri outUri;
            if (Uri.TryCreate(Source, UriKind.Absolute, out outUri) && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps))
            {
                imageSource = ImageSource.FromUri(outUri);
                return imageSource;
            }
            imageSource = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Source)));
            return imageSource;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
