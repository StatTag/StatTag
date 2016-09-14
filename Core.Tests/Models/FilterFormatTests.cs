using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Models;

namespace Core.Tests.Models
{
    [TestClass]
    public class FilterFormatTests
    {
        [TestMethod]
        public void Equals()
        {
            var filter1 = new FilterFormat("A") { Enabled = true, Type = "1", Value = "1"};
            var filter2 = new FilterFormat("A") { Enabled = false, Type = "2", Value = "2" };
            var filter3 = new FilterFormat("B") { Enabled = true, Type = "1", Value = "1" };
            var filter4 = new FilterFormat("A") { Enabled = true, Type = "1", Value = "1" };

            Assert.AreEqual(filter1, filter1);
            Assert.AreNotEqual(filter1, filter2);
            Assert.AreNotEqual(null, filter1);
            Assert.AreNotEqual(filter1, null);
            Assert.AreNotEqual(filter1, filter3);
            Assert.AreEqual(filter1, filter4);
        }

        [TestMethod]
        public void ExpandValue_Empty()
        {
            var format = new FilterFormat("Test") {Value = null};
            Assert.IsNull(format.ExpandValue());

            format.Value = string.Empty;
            Assert.IsNull(format.ExpandValue());

            format.Value = "     ";
            Assert.IsNull(format.ExpandValue());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ExpandValue_NonNumeric()
        {
            var format = new FilterFormat("Test") {Value = "A"};
            format.ExpandValue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ExpandValue_UnfinishedRange()
        {
            var format = new FilterFormat("Test") {Value = "1-"};
            format.ExpandValue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ExpandValue_NegativeValues()
        {
            var format = new FilterFormat("Test") {Value = "-2-5"};
            format.ExpandValue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ExpandValue_BlankComponent()
        {
            var format = new FilterFormat("Test") {Value = ",2-5"};
            format.ExpandValue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ExpandValue_ZeroValue()
        {
            var format = new FilterFormat("Test") {Value = "0"};
            format.ExpandValue();
        }

        [TestMethod]
        public void ExpandValue_SingleValues()
        {
            var format = new FilterFormat("Test") { Value = "1" };
            var values = format.ExpandValue();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual(1, values[0]);

            // Throwing in some extra spaces for fun
            format.Value = "  1,3, 5 ";
            values = format.ExpandValue();
            Assert.AreEqual(3, values.Length);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(3, values[1]);
            Assert.AreEqual(5, values[2]);

            // Make sure it will sort the values
            format.Value = "5,1,3";
            values = format.ExpandValue();
            Assert.AreEqual(3, values.Length);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(3, values[1]);
            Assert.AreEqual(5, values[2]);

            // Make sure we remove duplicates
            format.Value = "5,1,3,5,1,3,3,3";
            values = format.ExpandValue();
            Assert.AreEqual(3, values.Length);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(3, values[1]);
            Assert.AreEqual(5, values[2]);
        }

        [TestMethod]
        public void ExpandValue_Ranges()
        {
            var format = new FilterFormat("Test") {Value = "3-5"};
            var values = format.ExpandValue();
            Assert.AreEqual(3, values.Length);
            Assert.AreEqual(3, values[0]);
            Assert.AreEqual(4, values[1]);
            Assert.AreEqual(5, values[2]);
        
            // Flipped range
            format.Value = "5-3";
            values = format.ExpandValue();
            Assert.AreEqual(3, values.Length);
            Assert.AreEqual(3, values[0]);
            Assert.AreEqual(4, values[1]);
            Assert.AreEqual(5, values[2]);

            // Single value range (we will allow this)
            format.Value = "3-3";
            values = format.ExpandValue();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual(3, values[0]);

            // Multiple ranges, no overlap
            format.Value = "3-4,6-7";
            values = format.ExpandValue();
            Assert.AreEqual(4, values.Length);
            Assert.AreEqual(3, values[0]);
            Assert.AreEqual(4, values[1]);
            Assert.AreEqual(6, values[2]);
            Assert.AreEqual(7, values[3]);

            // Multiple ranges, with overlap
            format.Value = "3-6,4-7";
            values = format.ExpandValue();
            Assert.AreEqual(5, values.Length);
            Assert.AreEqual(3, values[0]);
            Assert.AreEqual(4, values[1]);
            Assert.AreEqual(5, values[2]);
            Assert.AreEqual(6, values[3]);
            Assert.AreEqual(7, values[4]);
        }

        [TestMethod]
        public void ExpandValue_ValuesAndRanges()
        {
            var format = new FilterFormat("Test") {Value = "1,3-5"};
            var values = format.ExpandValue();
            Assert.AreEqual(4, values.Length);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(3, values[1]);
            Assert.AreEqual(4, values[2]);
            Assert.AreEqual(5, values[3]);

            // Overlap
            format.Value = "3-5, 4, 5-5, 6-3";
            values = format.ExpandValue();
            Assert.AreEqual(4, values.Length);
            Assert.AreEqual(3, values[0]);
            Assert.AreEqual(4, values[1]);
            Assert.AreEqual(5, values[2]);
            Assert.AreEqual(6, values[3]);
        }
    }
}
