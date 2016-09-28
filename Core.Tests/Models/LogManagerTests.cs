using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;

namespace Core.Tests.Models
{
    [TestClass]
    public class LogManagerTests
    {
        [TestMethod]
        public void IsValidLogPath_Invalid()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.OpenWrite(It.IsAny<string>())).Throws(new Exception("Invalid"));
            var manager = new LogManager(mock.Object);
            Assert.IsFalse(manager.IsValidLogPath("Test.log"));
            Assert.IsFalse(manager.IsValidLogPath(""));
        }

        [TestMethod]
        public void IsValidLogPath_Valid()
        {
            var mock = new Mock<IFileHandler>();
            FileStream nullStream = null;
            mock.Setup(file => file.OpenWrite(It.IsAny<string>())).Returns(nullStream);
            var manager = new LogManager(mock.Object);
            Assert.IsTrue(manager.IsValidLogPath("Test.log"));
        }

        [TestMethod]
        public void UpdateSettings()
        {
            var manager = new LogManager();
            Assert.IsFalse(manager.Enabled);
            Assert.IsNull(manager.LogFilePath);

            var properties = new Properties() {EnableLogging = true, LogLocation = "Test.log"};
            manager.UpdateSettings(properties);
            Assert.IsTrue(manager.Enabled);
            Assert.AreEqual("Test.log", manager.LogFilePath);
        }

        [TestMethod]
        public void WriteMessage_Disabled()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.AppendAllText(It.IsAny<string>(), It.IsAny<string>()));
            var manager = new LogManager(mock.Object);
            manager.UpdateSettings(false, null);
            manager.WriteMessage("Test");
            // Never called if disabled
            mock.Verify(m => m.AppendAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void WriteMessage_Enabled()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.AppendAllText(It.IsAny<string>(), It.IsAny<string>()));
            var manager = new LogManager(mock.Object);
            manager.UpdateSettings(true, "Test.log");
            manager.WriteMessage("Test");
            mock.Verify(m => m.AppendAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void WriteException()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.AppendAllText(It.IsAny<string>(), It.IsAny<string>()));
            var manager = new LogManager(mock.Object);
            manager.UpdateSettings(true, "Test.log");
            manager.WriteException(new Exception("Test exception"));;
            mock.Verify(m => m.AppendAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void WriteException_Nested()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.AppendAllText(It.IsAny<string>(), It.IsAny<string>()));
            var manager = new LogManager(mock.Object);
            manager.UpdateSettings(true, "Test.log");
            manager.WriteException(new Exception("Test exception", new Exception("Inner")));
            mock.Verify(m => m.AppendAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
