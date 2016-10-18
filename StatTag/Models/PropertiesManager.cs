using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

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
        }
    }
}
