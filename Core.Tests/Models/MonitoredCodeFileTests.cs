using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Models;

namespace Core.Tests.Models
{
    [TestClass]
    public class MonitoredCodeFileTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidConstructor()
        {
            var file = new MonitoredCodeFile(null);
        }

        [TestMethod]
        public void DetectMultipleFileChanges()
        {
            var codeFile = new CodeFile()
            {
                FilePath = Path.GetTempFileName(),
                StatisticalPackage = Constants.StatisticalPackages.Stata,
                Tags = new List<Tag>()
            };

            var monitoredFile = new MonitoredCodeFile(codeFile, true);
            File.WriteAllText(codeFile.FilePath, "test1");
            Thread.Sleep(100);
            Assert.AreEqual(1, monitoredFile.ChangeHistory.Count);

            // Even when we change the same file again, we only want to count
            // once that there is a pending change for the file.
            File.WriteAllText(codeFile.FilePath, "test2");
            Thread.Sleep(100);
            Assert.AreEqual(1, monitoredFile.ChangeHistory.Count);

            File.Delete(codeFile.FilePath);
        }

        [TestMethod]
        public void DisableChangeTracking()
        {
            var codeFile = new CodeFile()
            {
                FilePath = Path.GetTempFileName(),
                StatisticalPackage = Constants.StatisticalPackages.Stata,
                Tags = new List<Tag>()
            };

            var monitoredFile = new MonitoredCodeFile(codeFile, false);
            Assert.IsFalse(monitoredFile.IsMonitoring());
            File.WriteAllText(codeFile.FilePath, "test1");
            Thread.Sleep(100);
            Assert.AreEqual(0, monitoredFile.ChangeHistory.Count);
            File.WriteAllText(codeFile.FilePath, "test2");
            Thread.Sleep(100);
            Assert.AreEqual(0, monitoredFile.ChangeHistory.Count);

            File.Delete(codeFile.FilePath);
        }

        [TestMethod]
        public void ToggleMonitorStatus()
        {
            var codeFile = new CodeFile()
            {
                FilePath = Path.GetTempFileName(),
                StatisticalPackage = Constants.StatisticalPackages.Stata,
                Tags = new List<Tag>()
            };

            // Monitoring will be on
            var monitoredFile = new MonitoredCodeFile(codeFile);
            Assert.IsTrue(monitoredFile.IsMonitoring());

            // No events, now turn monitoring off
            monitoredFile.StopMonitoring();
            Assert.IsFalse(monitoredFile.IsMonitoring());
            File.WriteAllText(codeFile.FilePath, "test1");
            Thread.Sleep(100);
            Assert.AreEqual(0, monitoredFile.ChangeHistory.Count);
            
            // Turn monitoring on and do another change
            monitoredFile.StartMonitoring();
            Assert.IsTrue(monitoredFile.IsMonitoring());
            File.WriteAllText(codeFile.FilePath, "test2");
            Thread.Sleep(100);
            Assert.AreEqual(1, monitoredFile.ChangeHistory.Count);

            File.Delete(codeFile.FilePath);
        }

        [TestMethod]
        public void ChangeAndRenameSequence()
        {
            var codeFile = new CodeFile()
            {
                FilePath = Path.GetTempFileName(),
                StatisticalPackage = Constants.StatisticalPackages.Stata,
                Tags = new List<Tag>()
            };

            // Monitoring will be on
            var monitoredFile = new MonitoredCodeFile(codeFile);
            File.WriteAllText(codeFile.FilePath, "test1");
            Thread.Sleep(100);
            Assert.AreEqual(1, monitoredFile.ChangeHistory.Count);

            // Rename the file
            File.Move(codeFile.FilePath, codeFile.FilePath + "_1");
            Thread.Sleep(100);
            Assert.AreEqual(2, monitoredFile.ChangeHistory.Count);

            // Write to renamed file.  We will track the rename against the new file name
            // so the change history is incremented.
            File.WriteAllText(codeFile.FilePath, "test2");
            Thread.Sleep(100);
            Assert.AreEqual(3, monitoredFile.ChangeHistory.Count);

            File.Delete(codeFile.FilePath + "_1");
        }

        [TestMethod]
        public void DetectDelete()
        {
            var codeFile = new CodeFile()
            {
                FilePath = Path.GetTempFileName(),
                StatisticalPackage = Constants.StatisticalPackages.Stata,
                Tags = new List<Tag>()
            };

            // Monitoring will be on
            var monitoredFile = new MonitoredCodeFile(codeFile);
            File.Delete(codeFile.FilePath);
            Thread.Sleep(100);
            Assert.AreEqual(1, monitoredFile.ChangeHistory.Count);
        }
    }
}
