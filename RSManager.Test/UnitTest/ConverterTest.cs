using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSManager.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RSManager.Test.UnitTest
{
    [TestClass]
    public class ConverterTest
    {
        readonly CultureInfo culture;
        public ConverterTest()
        {
            culture = CultureInfo.GetCultureInfo("en-US");
        }

        [TestMethod]
        public void Boolean_To_Visibility_Value_Converter_Test()
        {
            BooleanToVisibilityValueConverter converter = new BooleanToVisibilityValueConverter();

            object result_1 = converter.Convert(true, null, null, culture);
            object result_2 = converter.Convert(false, null, null, culture);
            object result_3 = converter.Convert(null, null, null, culture);

            Assert.AreEqual(Visibility.Visible, result_1);
            Assert.AreEqual(Visibility.Collapsed, result_3);
            Assert.AreEqual(Visibility.Collapsed, result_2);
        }

        [TestMethod]
        public void IsNull_Or_Empty_Value_Converter_Test()
        {
            IsNullOrEmptyValueConverter converter = new IsNullOrEmptyValueConverter();

            Assert.AreEqual(true, converter.Convert(null, null, null, culture));
            Assert.AreEqual(true, converter.Convert("", null, null, culture));
            Assert.AreEqual(false, converter.Convert(2, null, null, culture));
            Assert.AreEqual(false, converter.Convert("test", null, null, culture));
        }

        [TestMethod]
        public void String_To_Path_Value_Converter_Test()
        {
            StringToPathValueConverter converter = new StringToPathValueConverter();
            string path = @"M 10,100 C 10,300 300,-200 300,100";
            Assert.AreEqual(Geometry.Parse(path).ToString(), converter.Convert(path, null, null, culture).ToString());
        }

        [TestMethod]
        public void Text_Mask_Value_Converter_Test()
        {
            TextMaskValueConverter converter = new TextMaskValueConverter();

            Assert.AreEqual("***", converter.Convert("123", null, "*", culture));
            Assert.AreEqual("??", converter.Convert("AA", null, "?", culture));
        }

        [TestMethod]
        public void Unit_Size_Value_Converter_Test()
        {
            UnitSizeValueConverter converter = new UnitSizeValueConverter();

            Assert.AreEqual("1.40 KB", converter.Convert(1400L, null, null, culture));
        }

    }
}
