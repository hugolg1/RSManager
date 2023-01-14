using RSManager.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static RSManager.Utils.SizeUtils;

namespace RSManager.Converters
{
    public class UnitSizeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is long size))
            {
                return null;
            }
            if(size <= 0)
            {
                return null;
            }

            if (parameter is string)
            {
                Enum.TryParse(parameter.ToString(), true, out SizeUnits unit);
                return SizeUtils.FormatBytesToUnit(size, unit, culture); 
            }

            return SizeUtils.FormatBytes(size, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
