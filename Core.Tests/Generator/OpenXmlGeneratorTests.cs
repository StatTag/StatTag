using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StatTag.Core.Generator;

namespace Core.Tests.Generator
{
    [TestClass]
    public class OpenXmlGeneratorTests
    {
        private void MockFontAndRange(Mock<Microsoft.Office.Interop.Word.Font> font, Mock<Microsoft.Office.Interop.Word.Range> range)
        {
            font.Setup(f => f.Bold).Returns(0);
            font.Setup(f => f.Italic).Returns(0);
            font.Setup(f => f.AllCaps).Returns(0);
            font.Setup(f => f.SmallCaps).Returns(0);
            font.Setup(f => f.StrikeThrough).Returns(0);
            font.Setup(f => f.DoubleStrikeThrough).Returns(0);
            font.Setup(f => f.Underline).Returns(WdUnderline.wdUnderlineNone);
            font.Setup(f => f.Superscript).Returns(0);
            font.Setup(f => f.Subscript).Returns(0);
            font.Setup(f => f.Color).Returns(0);
            font.Setup(f => f.Size).Returns(10.0f);
            font.Setup(f => f.Name).Returns("Arial");
            range.Setup(r => r.HighlightColorIndex).Returns(WdColorIndex.wdNoHighlight);
            range.Setup(r => r.Font).Returns(font.Object);
            var document = new Mock<Microsoft.Office.Interop.Word.Document>();
            range.Setup(r => r.Document).Returns(document.Object);
        }

        [TestMethod]
        public void GenerateField_NoRange()
        {
            var field = OpenXmlGenerator.GenerateField(null, "test", "1", "2");
            Assert.AreNotEqual(string.Empty, field);
            const string expectedResponse = 
                "<w:p xmlns:w=\"http://schemas.microsoft.com/office/word/2003/wordml\">\r\n" +
                "    <w:r>\r\n" +
                "        <w:rPr>\r\n" +
                "            \r\n" +
                "        </w:rPr>\r\n" +
                "        <w:fldChar w:fldCharType=\"begin\" />\r\n" +
                "        <w:instrText xml:space=\"preserve\"> MacroButton StatTag 1</w:instrText>\r\n" +
                "        <w:fldChar w:fldCharType=\"begin\">\r\n" +
                "            <w:fldData xml:space=\"preserve\">MgA=\r\n" +
                "</w:fldData>\r\n" +
                "        </w:fldChar>\r\n" +
                "        <w:instrText xml:space=\"preserve\"> ADDIN test</w:instrText>\r\n" +
                "        <w:fldChar w:fldCharType=\"end\" />\r\n" +
                "        <w:fldChar w:fldCharType=\"end\" />\r\n" +
                "    </w:r>\r\n" +
                "</w:p>";

            Assert.AreEqual(expectedResponse, field);
        }

        [TestMethod]
        public void GenerateField_MockRange_PlainFont()
        {
            var range = new Mock<Microsoft.Office.Interop.Word.Range>();
            var font = new Mock<Microsoft.Office.Interop.Word.Font>();
            MockFontAndRange(font, range);
            var field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");

            const string expectedResponseFragment1 = @"<w:color w:val=""Black"" /><w:rFonts w:ascii=""Arial"" w:h-ansi=""Arial"" w:cs=""Arial""/>";
            const string expectedResponseFragment2 = @"<w:sz w:val=""20""/>";
        
            Assert.IsTrue(field.Contains(expectedResponseFragment1));
            Assert.IsTrue(field.Contains(expectedResponseFragment2));
        }

        [TestMethod]
        public void GenerateField_MockRange_Bold()
        {
            var range = new Mock<Microsoft.Office.Interop.Word.Range>();
            var font = new Mock<Microsoft.Office.Interop.Word.Font>();
            MockFontAndRange(font, range);
            font.Setup(f => f.Bold).Returns(OpenXmlGenerator.WdTrue);
            var field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:b/>"));
        }

        [TestMethod]
        public void GenerateField_MockRange_Italic()
        {
            var range = new Mock<Microsoft.Office.Interop.Word.Range>();
            var font = new Mock<Microsoft.Office.Interop.Word.Font>();
            MockFontAndRange(font, range);
            font.Setup(f => f.Italic).Returns(OpenXmlGenerator.WdTrue);
            var field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:i/>"));
        }

        [TestMethod]
        public void GenerateField_MockRange_Caps()
        {
            // AllCaps and SmallCaps are mutually exclusive so we test them together
            var range = new Mock<Microsoft.Office.Interop.Word.Range>();
            var font = new Mock<Microsoft.Office.Interop.Word.Font>();
            MockFontAndRange(font, range);
            font.Setup(f => f.AllCaps).Returns(OpenXmlGenerator.WdTrue);
            var field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:caps/>"));
            Assert.IsFalse(field.Contains("<w:smallCaps/>"));

            font.Setup(f => f.SmallCaps).Returns(OpenXmlGenerator.WdTrue);
            field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:caps/>"));
            Assert.IsFalse(field.Contains("<w:smallCaps/>"));

            font.Setup(f => f.AllCaps).Returns(0);
            field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsFalse(field.Contains("<w:caps/>"));
            Assert.IsTrue(field.Contains("<w:smallCaps/>"));
        }

        [TestMethod]
        public void GenerateField_MockRange_StrikeThrough()
        {
            // StrikeThrough and DoubleStrikeThrough are mutually exclusive so we test them together
            var range = new Mock<Microsoft.Office.Interop.Word.Range>();
            var font = new Mock<Microsoft.Office.Interop.Word.Font>();
            MockFontAndRange(font, range);
            font.Setup(f => f.StrikeThrough).Returns(OpenXmlGenerator.WdTrue);
            var field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:strike/>"));
            Assert.IsFalse(field.Contains("<w:dstrike/>"));

            font.Setup(f => f.DoubleStrikeThrough).Returns(OpenXmlGenerator.WdTrue);
            field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:strike/>"));
            Assert.IsFalse(field.Contains("<w:dstrike/>"));

            font.Setup(f => f.StrikeThrough).Returns(0);
            field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsFalse(field.Contains("<w:strike/>"));
            Assert.IsTrue(field.Contains("<w:dstrike/>"));
        }

        [TestMethod]
        public void GenerateField_MockRange_Underline()
        {
            var range = new Mock<Microsoft.Office.Interop.Word.Range>();
            var font = new Mock<Microsoft.Office.Interop.Word.Font>();
            MockFontAndRange(font, range);
            font.Setup(f => f.Underline).Returns(WdUnderline.wdUnderlineDashHeavy);
            var field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:u w:color=\"auto\" w:val=\"dashedHeavy\"/>"));
        }

        [TestMethod]
        public void GenerateField_MockRange_Script()
        {
            // Superscript and Subscript are mutually exclusive so we test them together
            var range = new Mock<Microsoft.Office.Interop.Word.Range>();
            var font = new Mock<Microsoft.Office.Interop.Word.Font>();
            MockFontAndRange(font, range);
            font.Setup(f => f.Superscript).Returns(OpenXmlGenerator.WdTrue);
            var field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:vertAlign w:val=\"superscript\"/>"));
            Assert.IsFalse(field.Contains("<w:vertAlign w:val=\"subscript\"/>"));

            font.Setup(f => f.Subscript).Returns(OpenXmlGenerator.WdTrue);
            field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:vertAlign w:val=\"superscript\"/>"));
            Assert.IsFalse(field.Contains("<w:vertAlign w:val=\"subscript\"/>"));

            font.Setup(f => f.Superscript).Returns(0);
            field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsFalse(field.Contains("<w:vertAlign w:val=\"superscript\"/>"));
            Assert.IsTrue(field.Contains("<w:vertAlign w:val=\"subscript\"/>"));
        }

        [TestMethod]
        public void GenerateField_MockRange_HighlightColor()
        {
            var range = new Mock<Microsoft.Office.Interop.Word.Range>();
            var font = new Mock<Microsoft.Office.Interop.Word.Font>();
            MockFontAndRange(font, range);
            range.Setup(r => r.HighlightColorIndex).Returns(WdColorIndex.wdBlue);
            var field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:highlight w:val=\"blue\"/>"));
        }

        [TestMethod]
        public void GenerateField_MockRange_Color()
        {
            var range = new Mock<Microsoft.Office.Interop.Word.Range>();
            var font = new Mock<Microsoft.Office.Interop.Word.Font>();
            MockFontAndRange(font, range);
            font.Setup(f => f.Color).Returns(WdColor.wdColorRed);
            var field = OpenXmlGenerator.GenerateField(range.Object, "test", "1", "2");
            Assert.IsTrue(field.Contains("<w:color w:val=\"Red\" />"));
        }

        [TestMethod]
        public void Base64EncodeFieldData_Empty()
        {
            Assert.AreEqual(string.Empty, OpenXmlGenerator.Base64EncodeFieldData(null));
            Assert.AreEqual(string.Empty, OpenXmlGenerator.Base64EncodeFieldData(string.Empty));
            // String with spaces is different from an empty string
            Assert.AreNotEqual(string.Empty, OpenXmlGenerator.Base64EncodeFieldData("   "));
        }

        [TestMethod]
        public void Base64EncodeFieldData_WithData()
        {
            const string testString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var encoded = OpenXmlGenerator.Base64EncodeFieldData(testString);
            Assert.AreEqual("QQBCAEMARABFAEYARwBIAEkASgBLAEwATQBOAE8AUABRAFIAUwBUAFUAVgBXAFgAWQBaADEAMgAz\r\nADQANQA2ADcAOAA5ADAA\r\n", encoded);

            var longString =
                Enumerable.Repeat(testString, 1000).Aggregate(new StringBuilder(), (sb, s) => sb.Append(s)).ToString();
            var longEncoded = OpenXmlGenerator.Base64EncodeFieldData(longString);
            Assert.AreNotEqual(string.Empty, longEncoded);
            Assert.IsTrue(longEncoded.Length > encoded.Length);
        }
    }
}
