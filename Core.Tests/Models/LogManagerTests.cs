using System;
using System.IO;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using LogManager = StatTag.Core.Models.LogManager;

namespace Core.Tests.Models
{
    [TestClass]
    public class LogManagerTests
    {
        [TestMethod]
        public void IsValidLogPath_Invalid()
        {
            var mockHandler = new Mock<IFileHandler>();
            var mockLog = new Mock<ILog>();
            mockHandler.Setup(file => file.OpenWrite(It.IsAny<string>())).Throws(new Exception("Invalid"));
            var manager = new LogManager(mockHandler.Object, mockLog.Object);
            Assert.IsFalse(manager.IsValidLogPath("Test.log"));
            Assert.IsFalse(manager.IsValidLogPath(""));
        }

        [TestMethod]
        public void IsValidLogPath_Valid()
        {
            var mockHandler = new Mock<IFileHandler>();
            var mockLog = new Mock<ILog>();
            FileStream nullStream = null;
            mockHandler.Setup(file => file.OpenWrite(It.IsAny<string>())).Returns(nullStream);
            var manager = new LogManager(mockHandler.Object, mockLog.Object);
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
            var mockHandler = new Mock<IFileHandler>();
            var mockLog = new Mock<ILog>();
            mockLog.Setup(log => log.Info(It.IsAny<string>()));
            var manager = new LogManager(mockHandler.Object, mockLog.Object);
            manager.UpdateSettings(false, "Test.log", 1L, 1L);
            manager.WriteMessage("Test");
            // Never called if disabled
            mockLog.Verify(m => m.Info(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void WriteMessage_Enabled()
        {
            var mockHandler = new Mock<IFileHandler>();
            var mockLog = new Mock<ILog>();
            mockLog.Setup(log => log.Info(It.IsAny<string>()));
            var manager = new LogManager(mockHandler.Object, mockLog.Object);
            manager.UpdateSettings(true, "Test.log", 1L, 1L);
            manager.WriteMessage("Test");
            mockLog.Verify(m => m.Info(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void WriteException()
        {
            var mockHandler = new Mock<IFileHandler>();
            var mockLog = new Mock<ILog>();
            mockLog.Setup(log => log.ErrorFormat(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>()));
            var manager = new LogManager(mockHandler.Object, mockLog.Object);
            manager.UpdateSettings(true, "Test.log", 1L, 1L);
            manager.WriteException(new Exception("Test exception")); ;
            mockLog.Verify(m => m.ErrorFormat(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>()), Times.Once);
        }

        [TestMethod]
        public void WriteException_Nested()
        {
            var mockHandler = new Mock<IFileHandler>();
            var mockLog = new Mock<ILog>();
            mockLog.Setup(log => log.ErrorFormat(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>()));
            var manager = new LogManager(mockHandler.Object, mockLog.Object);
            manager.UpdateSettings(true, "Test.log", 1L, 1L);
            manager.WriteException(new Exception("Test exception", new Exception("Inner")));
            mockLog.Verify(m => m.ErrorFormat(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>()), Times.Exactly(2));
        }
    }
}
