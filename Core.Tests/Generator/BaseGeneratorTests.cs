using System;
using System.Security.Cryptography.X509Certificates;
using AnalysisManager.Core.Generator;
using AnalysisManager.Core.Models;
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
            Assert.AreEqual("**>>>AM:", generator.CreateOpenTag(null));
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
            var annotation = new Annotation()
            {
                Id = "id",
                Type = Constants.AnnotationType.Value,
                ValueFormat = new ValueFormat()
            };
            Assert.AreEqual("**>>>AM:Value(Id=\"id\", Type=\"Default\")", generator.CreateOpenTag(annotation));
        }

        [TestMethod]
        public void CreateOpenTag_Figure()
        {
            var generator = new StubGenerator();
            var annotation = new Annotation()
            {
                Id = "id",
                Type = Constants.AnnotationType.Figure,
                FigureFormat = new FigureFormat()
            };
            Assert.AreEqual("**>>>AM:Figure(Id=\"id\")", generator.CreateOpenTag(annotation));
        }

        [TestMethod]
        public void CreateOpenTag_Table()
        {
            var generator = new StubGenerator();
            var annotation = new Annotation()
            {
                Id = "id",
                Type = Constants.AnnotationType.Table
            };
            Assert.AreEqual("**>>>AM:Table(Id=\"id\", Type=\"Default\")", generator.CreateOpenTag(annotation));

            annotation.TableFormat = new TableFormat();
            Assert.AreEqual("**>>>AM:Table(Id=\"id\", ColumnNames=False, RowNames=False, Type=\"Default\")", generator.CreateOpenTag(annotation));
        }

        [TestMethod]
        public void CombineValueAndTableParameters()
        {
            var generator = new StubGenerator();
            var annotation = new Annotation()
            {
                Id = "id",
                Type = Constants.AnnotationType.Table,
                ValueFormat = new ValueFormat(),
                TableFormat = new TableFormat()
            };

            Assert.AreEqual("Id=\"id\", ColumnNames=False, RowNames=False, Type=\"Default\"", generator.CombineValueAndTableParameters(annotation));

            annotation.ValueFormat.FormatType = Constants.ValueFormatType.Numeric;
            annotation.ValueFormat.DecimalPlaces = 2;
            Assert.AreEqual("Id=\"id\", ColumnNames=False, RowNames=False, Type=\"Numeric\", Decimals=2, Thousands=False", generator.CombineValueAndTableParameters(annotation));
        }
    }
}
