using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Utils
{
    public static class SizeUtils
    {
        public enum SizeUnits
        {
            Bytes, KB, MB, GB, TB, PB, EB, ZB, YB
        }

        public static string FormatBytes(this long value, CultureInfo culture)
        {
            SizeUnits unit = SizeUnits.Bytes;
            double size = 0;
            int counter = 0;

            do
            {
                if (counter > 0)
                {
                    unit = unit + 1;
                }
                size = Math.Round((value / Math.Pow(1000, (Int64)unit)), 2);
                counter++;
            }
            while (size > 1024);

            if (size <= 0)
            {
                return null;
            }

            if (size == 1 && unit == SizeUnits.Bytes)
            {
                return "1 Byte";
            }

            string textSize = size % 1 == 0 ? size.ToString(culture) : size.ToString("0.00", culture);            
            return String.Concat(textSize, " ", Enum.GetName(typeof(SizeUnits), unit));
        }

        public static string FormatBytesToUnit(this long bytes, SizeUnits unit, CultureInfo culture)
        {
            return Math.Round((bytes / (Math.Pow(1024, (Int64)unit))), 2).ToString("0.00", culture);
        }

    }
}
