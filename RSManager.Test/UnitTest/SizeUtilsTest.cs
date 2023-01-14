using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSManager.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Test.UnitTest
{
    [TestClass]
    public class SizeUtilsTest
    {
        readonly CultureInfo culture;
        public SizeUtilsTest()
        {
            culture = CultureInfo.GetCultureInfo("en-US");
        }

        [TestMethod]
        public void Format_Bytes_To_Kilobytes()
        {
            Assert.AreEqual("0.59", SizeUtils.FormatBytesToUnit(600, SizeUtils.SizeUnits.KB, culture));
            Assert.AreEqual("1.37", SizeUtils.FormatBytesToUnit(1400, SizeUtils.SizeUnits.KB, culture));
            Assert.AreEqual("6.01", SizeUtils.FormatBytesToUnit(6154, SizeUtils.SizeUnits.KB, culture));
        }

        [TestMethod]
        public void Format_Bytes_To_Megabytes()
        {
            Assert.AreEqual("2.86", SizeUtils.FormatBytesToUnit(3000000, SizeUtils.SizeUnits.MB, culture));
            Assert.AreEqual("0.04", SizeUtils.FormatBytesToUnit(40000, SizeUtils.SizeUnits.MB, culture));
            Assert.AreEqual("1.53", SizeUtils.FormatBytesToUnit(1600000, SizeUtils.SizeUnits.MB, culture));
        }

        [TestMethod]
        public void Format_Bytes_To_Near_Unit()
        {
            Assert.AreEqual("1 Byte", SizeUtils.FormatBytes(1, culture));
            Assert.AreEqual("600 Bytes", SizeUtils.FormatBytes(600, culture));
            Assert.AreEqual("3 KB", SizeUtils.FormatBytes(3000, culture));
            Assert.AreEqual("40 GB", SizeUtils.FormatBytes(40000000000, culture));
        }

    }
}
