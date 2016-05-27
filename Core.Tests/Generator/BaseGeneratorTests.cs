using System;
using System.Security.Cryptography.X509Certificates;
using StatTag.Core.Generator;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Generator
{
    [TestClass]
    public class BaseGeneratorTests
    {
        class StubGenerator : BaseGenerator
        {
            public override string CommentCharacter
            {
                get { return "*"; }
            }
        }

        [TestMethod]
        public void CreateOpenTagBase()
        {
            var generator = new StubGenerator();
            Assert.AreEqual("**>>>ST:", generator.CreateOpenTag(null));
        }

        [TestMethod]
        public void CreateClosingTag()
        {
            var generator = new StubGenerator();
            Assert.AreEqual("**<<<", generator.CreateClosingTag());
        }

        [TestMethod]
        public void CreateOpenTag_Value()
        {
            var generator = new StubGenerator();
            var tag = new Tag()
            {
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            Assert.AreEqual("**>>>ST:Value(Type=\"Default\")", generator.CreateOpenTag(tag));
        }

        [TestMethod]
        public void CreateOpenTag_Figure()
        {
            var generator = new StubGenerator();
            var tag = new Tag()
            {
                Type = Constants.TagType.Figure,
                FigureFormat = new FigureFormat()
            };
            Assert.AreEqual("**>>>ST:Figure()", generator.CreateOpenTag(tag));
        }

        [TestMethod]
        public void CreateOpenTag_Table()
        {
            var generator = new StubGenerator();
            var tag = new Tag()
            {
                Type = Constants.TagType.Table
            };
            Assert.AreEqual("**>>>ST:Table(Type=\"Default\")", generator.CreateOpenTag(tag));

            tag.TableFormat = new TableFormat();
            Assert.AreEqual("**>>>ST:Table(ColumnNames=False, RowNames=False, Type=\"Default\")", generator.CreateOpenTag(tag));
        }

        [TestMethod]
        public void CombineValueAndTableParameters()
        {
            var generator = new StubGenerator();
            var tag = new Tag()
            {
                Type = Constants.TagType.Table,
                ValueFormat = new ValueFormat(),
                TableFormat = new TableFormat()
            };

            Assert.AreEqual("ColumnNames=False, RowNames=False, Type=\"Default\"", generator.CombineValueAndTableParameters(tag));

            tag.ValueFormat.FormatType = Constants.ValueFormatType.Numeric;
            tag.ValueFormat.DecimalPlaces = 2;
            Assert.AreEqual("ColumnNames=False, RowNames=False, Type=\"Numeric\", Decimals=2, Thousands=False", generator.CombineValueAndTableParameters(tag));
        }
    }
}
