using System;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Models
{
    [TestClass]
    public class ValueFormatTests
    {
        [TestMethod]
        public void Format_Empty()
        {
            var format = new ValueFormat();
            Assert.AreEqual(string.Empty, format.Format(null));
            Assert.AreEqual(string.Empty, format.Format(""));
            Assert.AreEqual(string.Empty, format.Format("    \t \r\n"));
        }

        [TestMethod]
        public void Format_Default()
        {
            var format = new ValueFormat() {FormatType = Constants.ValueFormatType.Default};
            Assert.AreEqual("My string", format.Format("My string"));
            Assert.AreEqual("1234.56789\r\n", format.Format("1234.56789\r\n"));
        }

        [TestMethod]
        public void Format_Numeric()
        {
            var format = new ValueFormat() { FormatType = Constants.ValueFormatType.Numeric };
            Assert.AreEqual(string.Empty, format.Format("Not a number"));
            Assert.AreEqual("0", format.Format("0.0"));
            Assert.AreEqual("1", format.Format("1.11"));  // Default numeric format
            Assert.AreEqual("1235", format.Format("1234.56789"));  // Rounds up
            format.DecimalPlaces = 2;
            Assert.AreEqual("1234.57", format.Format("1234.56789"));  // Rounds up with decimal places
            format.DecimalPlaces = 10;
            Assert.AreEqual("1234.5678900000", format.Format("1234.56789"));  // Rounds up with decimal places

            format.DecimalPlaces = 0;
            format.UseThousands = true;
            Assert.AreEqual("1,234", format.Format("1234"));
            format.DecimalPlaces = 2;
            Assert.AreEqual("1,234.57", format.Format("1234.567"));
            format.DecimalPlaces = 1;
            Assert.AreEqual("1.0", format.Format("1"));
            format.DecimalPlaces = 0;
            Assert.AreEqual("1,234,567,890", format.Format("1234567890"));

            format.AllowInvalidTypes = true;
            Assert.AreEqual("test", format.Format("test"));
        }

        [TestMethod]
        public void Format_Percentage()
        {
            var format = new ValueFormat() { FormatType = Constants.ValueFormatType.Percentage };
            Assert.AreEqual(string.Empty, format.Format("Not a number"));
            Assert.AreEqual("111%", format.Format("1.11"));
            Assert.AreEqual("12%", format.Format("0.1234"));
            format.DecimalPlaces = 2;
            Assert.AreEqual("12.35%", format.Format("0.123456789"));  // Rounds up with decimal places
            format.DecimalPlaces = 10;
            Assert.AreEqual("12.3400000000%", format.Format("0.1234"));  // Rounds up with decimal places

            format.AllowInvalidTypes = true;
            Assert.AreEqual("test", format.Format("test"));
        }

        [TestMethod]
        public void Format_DateTime()
        {
            // Date only
            var format = new ValueFormat() {FormatType = Constants.ValueFormatType.DateTime};
            Assert.AreEqual(string.Empty, format.Format("Not a date"));
            format.DateFormat = Constants.DateFormats.MMDDYYYY;
            Assert.AreEqual("03/11/2012", format.Format("3/11/2012"));
            Assert.AreEqual("11/11/2011", format.Format("11/11/11 11:11"));
            format.DateFormat = Constants.DateFormats.MonthDDYYYY;
            Assert.AreEqual("March 11, 2012", format.Format("3/11/2012"));
            Assert.AreEqual("November 11, 2011", format.Format("11/11/11 11:11:11"));

            // Time only
            format.DateFormat = string.Empty;
            format.TimeFormat = Constants.TimeFormats.HHMM;
            Assert.AreEqual("11:30", format.Format("11:30:50"));
            format.TimeFormat = Constants.TimeFormats.HHMMSS;
            Assert.AreEqual("15:30:50", format.Format("11/11/11 15:30:50"));

            // Date and time
            format.DateFormat = Constants.DateFormats.MMDDYYYY;
            format.TimeFormat = Constants.TimeFormats.HHMMSS;
            Assert.AreEqual("03/11/2012 11:30:00", format.Format("3/11/2012 11:30"));

            format.AllowInvalidTypes = true;
            Assert.AreEqual("test", format.Format("test"));
        }

        [TestMethod]
        public void Repeat()
        {
            Assert.AreEqual("aaaaa", ValueFormat.Repeat("a", 5));
            Assert.AreEqual("aAaAaA", ValueFormat.Repeat("aA", 3));
            Assert.AreEqual(string.Empty, ValueFormat.Repeat(string.Empty, 5));
            Assert.AreEqual(string.Empty, ValueFormat.Repeat("test", 0));
            Assert.AreEqual(string.Empty, ValueFormat.Repeat(null, 5));
        }

        [TestMethod]
        public void Equals()
        {
            var firstObject = new ValueFormat()
            {
                DateFormat = "DateTest",
                DecimalPlaces = 1,
                FormatType = "FormatTest",
                TimeFormat = "TimeTest",
                UseThousands = true
            };
            var secondObject = new ValueFormat()
            {
                DateFormat = "DateTest",
                DecimalPlaces = 1,
                FormatType = "FormatTest",
                TimeFormat = "TimeTest",
                UseThousands = true
            };
            Assert.IsTrue(firstObject.Equals(secondObject));
            Assert.IsTrue(secondObject.Equals(firstObject));
            Assert.AreEqual(firstObject, secondObject);
            Assert.AreEqual(secondObject, firstObject);
            Assert.IsTrue(firstObject == secondObject);
            Assert.IsTrue(secondObject == firstObject);

            secondObject.DateFormat += "1";
            Assert.IsFalse(firstObject.Equals(secondObject));
            Assert.IsFalse(secondObject.Equals(firstObject));
            Assert.AreNotEqual(firstObject, secondObject);
            Assert.AreNotEqual(secondObject, firstObject);
            Assert.IsFalse(firstObject == secondObject);
            Assert.IsFalse(secondObject == firstObject);
        }
    }
}
