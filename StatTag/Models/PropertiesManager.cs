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
    public class PropertiesManager
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

        public Core.Models.Properties Properties { get; set; }

        public PropertiesManager()
        {
            Properties = new Core.Models.Properties();
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

            key.SetValue(StataLocationKey, Properties.StataLocation, RegistryValueKind.String);
            key.SetValue(LogLocationKey, Properties.LogLocation, RegistryValueKind.String);
            key.SetValue(LogEnabledKey, Properties.EnableLogging, RegistryValueKind.DWord);
            key.SetValue(RunCodeOnOpenKey, Properties.RunCodeOnOpen, RegistryValueKind.DWord);
            key.SetValue(MaxLogFileSize, Properties.GetValueInRange(Properties.MaxLogFileSize,
                Core.Models.Properties.MaxLogFileSizeMin, Core.Models.Properties.MaxLogFileSizeMax,
                Core.Models.Properties.MaxLogFileSizeDefault), RegistryValueKind.QWord);
            key.SetValue(MaxLogFiles, Properties.GetValueInRange(Properties.MaxLogFiles,
                Core.Models.Properties.MaxLogFilesMin, Core.Models.Properties.MaxLogFilesMax,
                Core.Models.Properties.MaxLogFilesDefault), RegistryValueKind.DWord);
            key.SetValue(MissingValuesOption, Properties.RepresentMissingValues, RegistryValueKind.String);
            key.SetValue(MissingValuesCustomValue, Properties.CustomMissingValue, RegistryValueKind.String);
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

            Properties.StataLocation = key.GetValue(StataLocationKey, string.Empty).ToString();
            Properties.LogLocation = key.GetValue(LogLocationKey, string.Empty).ToString();
            Properties.EnableLogging = GetBooleanValue(key, LogEnabledKey);
            Properties.RunCodeOnOpen = GetBooleanValue(key, RunCodeOnOpenKey);
            Properties.MaxLogFileSize = GetULongValue(key, MaxLogFileSize, Core.Models.Properties.MaxLogFileSizeDefault);
            Properties.MaxLogFiles = GetULongValue(key, MaxLogFiles, Core.Models.Properties.MaxLogFilesDefault);
            Properties.RepresentMissingValues =
                key.GetValue(MissingValuesOption, Constants.MissingValueOption.StatPackageDefault).ToString();
            Properties.CustomMissingValue = key.GetValue(MissingValuesCustomValue, string.Empty).ToString();
        }
    }
}
