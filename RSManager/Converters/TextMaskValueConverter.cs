using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RSManager.Converters
{
    public class TextMaskValueConverter : IValueConverter
    {
        private string originalText = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text && parameter is string mask)
            {
                originalText = text;
                return string.Concat(Enumerable.Repeat(mask, text.Length));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                if(originalText == null)
                {
                    return value;
                }

                if (text.Length > originalText.Length)
                {
                    return String.Concat(originalText, text.Substring(originalText.Length));
                }
                else
                {
                    return originalText.Substring(0, text.Length);
                }
            }

            return null;
        }
    }
}
