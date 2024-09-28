using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Models;
using System;

namespace Core.Tests.Models
{
    [TestClass]
    public class UserSettingsTests
    {
        [TestMethod]
        public void RSettingsChanged_Null()
        {
            var newSettings = new UserSettings();
            Assert.IsTrue(newSettings.RSettingsChanged(null));
        }

        [TestMethod]
        public void RSettingsChanged_BothEmpty()
        {
            var oldSettings = new UserSettings();
            var newSettings = new UserSettings();
            Assert.IsFalse(newSettings.RSettingsChanged(oldSettings));
        }

        [TestMethod]
        public void RSettingsChanged_Unchanged()
        {
            var oldSettings = new UserSettings()
            {
                RDetection = Constants.RDetectionOption.System,
                RLocation = "C:\\test\\R"
            };
            var newSettings = new UserSettings()
            {
                RDetection = oldSettings.RDetection,
                RLocation = oldSettings.RLocation
            };
            Assert.IsFalse(newSettings.RSettingsChanged(oldSettings));
        }

        [TestMethod]
        public void RSettingsChanged_DetectionChanged()
        {
            var oldSettings = new UserSettings()
            {
                RDetection = Constants.RDetectionOption.System,
                RLocation = "C:\\test\\R"
            };
            var newSettings = new UserSettings()
            {
                RDetection = Constants.RDetectionOption.Selected,
                RLocation = oldSettings.RLocation
            };
            Assert.IsTrue(newSettings.RSettingsChanged(oldSettings));
        }

        [TestMethod]
        public void RSettingsChanged_DetectionChanged_Null()
        {
            // Make sure changing to null doesn't crash
            var oldSettings = new UserSettings()
            {
                RDetection = Constants.RDetectionOption.System,
                RLocation = "C:\\test\\R"
            };
            var newSettings = new UserSettings()
            {
                RDetection = null,
                RLocation = oldSettings.RLocation
            };
            Assert.IsTrue(newSettings.RSettingsChanged(oldSettings));
        }

        [TestMethod]
        public void RSettingsChanged_LocationChanged()
        {
            var oldSettings = new UserSettings()
            {
                RDetection = Constants.RDetectionOption.Selected,
                RLocation = "C:\\test\\R"
            };
            var newSettings = new UserSettings()
            {
                RDetection = oldSettings.RDetection,
                RLocation = "C:\\test\\newpath\\R"
            };
            Assert.IsTrue(newSettings.RSettingsChanged(oldSettings));
        }

        [TestMethod]
        public void RSettingsChanged_LocationChanged_Null()
        {
            // Make sure a value of null doesn't crash
            var oldSettings = new UserSettings()
            {
                RDetection = Constants.RDetectionOption.Selected,
                RLocation = null
            };
            var newSettings = new UserSettings()
            {
                RDetection = oldSettings.RDetection,
                RLocation = "C:\\test\\newpath\\R"
            };
            Assert.IsTrue(newSettings.RSettingsChanged(oldSettings));
        }

        [TestMethod]
        public void RSettingsChanged_CustomPathChanged()
        {
            var oldSettings = new UserSettings()
            {
                RDetection = Constants.RDetectionOption.Selected,
                RLocation = "C:\\test\\R",
                RCustomPath = "C:\\custom\\R"
            };
            var newSettings = new UserSettings()
            {
                RDetection = oldSettings.RDetection,
                RLocation = oldSettings.RLocation,
                RCustomPath = "C:\\new-custom\\R"
            };

            // Because the custom path is independent of the user's selection
            // for detecting R, it doesn't count as a change.
            Assert.IsFalse(newSettings.RSettingsChanged(oldSettings));
        }

        [TestMethod]
        public void Clone()
        {
            var settings = new UserSettings()
            {
                CustomMissingValue = "a",
                EnableLogging = true,
                LogLocation = "b",
                MaxLogFiles = 1,
                MaxLogFileSize = 2,
                RCustomPath = "c",
                RDetection = "d",
                RepresentMissingValues = "e",
                RLocation = "f",
                RunCodeOnOpen = false,
                StataLocation = "g"
            };

            var cloneSettings = (UserSettings)settings.Clone();
            cloneSettings.CustomMissingValue = "h";
            Assert.AreNotEqual<string>(settings.CustomMissingValue, cloneSettings.CustomMissingValue);
            cloneSettings.EnableLogging = !cloneSettings.EnableLogging;
            Assert.AreNotEqual<bool>(settings.EnableLogging, cloneSettings.EnableLogging);
            cloneSettings.LogLocation = "i";
            Assert.AreNotEqual<string>(settings.LogLocation, cloneSettings.LogLocation);
            cloneSettings.MaxLogFiles = 3;
            Assert.AreNotEqual<ulong?>(settings.MaxLogFiles, cloneSettings.MaxLogFiles);
            cloneSettings.MaxLogFileSize = 4;
            Assert.AreNotEqual<ulong?>(settings.MaxLogFileSize, cloneSettings.MaxLogFileSize);
            cloneSettings.RCustomPath = "j";
            Assert.AreNotEqual<string>(settings.RCustomPath, cloneSettings.RCustomPath);
            cloneSettings.RDetection = "k";
            Assert.AreNotEqual<string>(settings.RDetection, cloneSettings.RDetection);
            cloneSettings.RepresentMissingValues = "l";
            Assert.AreNotEqual<string>(settings.RepresentMissingValues, cloneSettings.RepresentMissingValues);
            cloneSettings.RLocation = "m";
            Assert.AreNotEqual<string>(settings.RLocation, cloneSettings.RLocation);
            cloneSettings.RunCodeOnOpen = !cloneSettings.RunCodeOnOpen;
            Assert.AreNotEqual<bool>(settings.RunCodeOnOpen, cloneSettings.RunCodeOnOpen);
            cloneSettings.StataLocation = "n";
            Assert.AreNotEqual<string>(settings.StataLocation, cloneSettings.StataLocation);
        }
    }
}
