using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using StatTag.Core.Models;

namespace StatTag.Models
{
    /// <summary>
    /// Manages loading and saving user settings and preferences.
    /// </summary>
    public class SettingsManager
    {
        private const string ApplicationKey = "Software\\Northwestern University\\StatTag";
        private const string StataLocationKey = "Stata Location";
        private const string LogLocationKey = "Log Location";
        private const string LogEnabledKey = "Logging Enabled";
        private const string RunCodeOnOpenKey = "Autorun Code";
        private const string MaxLogFileSize = "Max Log File Size";
        private const string MaxLogFiles = "Max Num Log Files";
        private const string MissingValuesOption = "Missing Values";
        private const string MissingValuesCustomValue = "Custom Missing Value String";
        private const string RDetectionKey = "R Detection";
        private const string RLocationKey = "R Location";
        private const string RCustomPathKey = "R Custom Path";

        public Core.Models.UserSettings Settings { get; set; }

        public SettingsManager()
        {
            Settings = new Core.Models.UserSettings();
        }

        /// <summary>
        /// Save the properties to the user's registry.
        /// </summary>
        public void Save()
        {
            var key = Registry.CurrentUser.CreateSubKey(ApplicationKey);
            if (key == null)
            {
                return;
            }

            key.SetValue(StataLocationKey, Settings.StataLocation, RegistryValueKind.String);
            key.SetValue(LogLocationKey, Settings.LogLocation, RegistryValueKind.String);
            key.SetValue(LogEnabledKey, Settings.EnableLogging, RegistryValueKind.DWord);
            key.SetValue(RunCodeOnOpenKey, Settings.RunCodeOnOpen, RegistryValueKind.DWord);
            key.SetValue(MaxLogFileSize, Settings.GetValueInRange(Settings.MaxLogFileSize,
                Core.Models.UserSettings.MaxLogFileSizeMin, Core.Models.UserSettings.MaxLogFileSizeMax,
                Core.Models.UserSettings.MaxLogFileSizeDefault), RegistryValueKind.QWord);
            key.SetValue(MaxLogFiles, Settings.GetValueInRange(Settings.MaxLogFiles,
                Core.Models.UserSettings.MaxLogFilesMin, Core.Models.UserSettings.MaxLogFilesMax,
                Core.Models.UserSettings.MaxLogFilesDefault), RegistryValueKind.DWord);
            key.SetValue(MissingValuesOption, Settings.RepresentMissingValues, RegistryValueKind.String);
            key.SetValue(MissingValuesCustomValue, Settings.CustomMissingValue, RegistryValueKind.String);
            key.SetValue(RDetectionKey, Settings.RDetection, RegistryValueKind.String);
            key.SetValue(RCustomPathKey, Settings.RCustomPath, RegistryValueKind.String);
            key.SetValue(RLocationKey, Settings.RLocation, RegistryValueKind.String);
        }

        /// <summary>
        /// Helper function to decipher a boolean registry value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="registryKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private bool GetBooleanValue(RegistryKey key, string registryKey, bool defaultValue = false)
        {
            var value = key.GetValue(registryKey, defaultValue);
            if (value == null)
            {
                return defaultValue;
            }

            if (value is int)
            {
                return ((int) value) == 1;
            }

            if (value is bool)
            {
                return ((bool) value);
            }

            return defaultValue;
        }

        /// <summary>
        /// Helper function to decipher a ulong registry value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="registryKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private ulong GetULongValue(RegistryKey key, string registryKey, ulong defaultValue)
        {
            var value = key.GetValue(registryKey, defaultValue);
            if (value == null)
            {
                return defaultValue;
            }

            if (value is long)
            {
                return ((ulong) (long) value);
            }
            else if (value is int)
            {
                return ((ulong) (int) value);
            }

            return defaultValue;
        }

        /// <summary>
        /// Load the properties from the user's registry.
        /// </summary>
        public void Load()
        {
            var key = Registry.CurrentUser.OpenSubKey(ApplicationKey);
            if (key == null)
            {
                return;
            }

            Settings.StataLocation = key.GetValue(StataLocationKey, string.Empty).ToString();
            Settings.LogLocation = key.GetValue(LogLocationKey, string.Empty).ToString();
            Settings.EnableLogging = GetBooleanValue(key, LogEnabledKey);
            Settings.RunCodeOnOpen = GetBooleanValue(key, RunCodeOnOpenKey);
            Settings.MaxLogFileSize = GetULongValue(key, MaxLogFileSize, Core.Models.UserSettings.MaxLogFileSizeDefault);
            Settings.MaxLogFiles = GetULongValue(key, MaxLogFiles, Core.Models.UserSettings.MaxLogFilesDefault);
            Settings.RepresentMissingValues =
                key.GetValue(MissingValuesOption, Constants.MissingValueOption.BlankString).ToString();
            Settings.CustomMissingValue = key.GetValue(MissingValuesCustomValue, string.Empty).ToString();
            Settings.RDetection = key.GetValue(RDetectionKey, Constants.RDetectionOption.System).ToString();
            Settings.RLocation = key.GetValue(RLocationKey, string.Empty).ToString();
            Settings.RCustomPath = key.GetValue(RCustomPathKey, string.Empty).ToString();
        }
    }
}
