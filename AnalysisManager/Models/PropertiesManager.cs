using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace AnalysisManager.Models
{
    public class PropertiesManager
    {
        private const string ApplicationKey = "Software\\Northwestern University\\AnalysisManager";
        private const string StataLocationKey = "Stata Location";

        public Properties Properties { get; set; }

        public PropertiesManager()
        {
            Properties = new Properties();
        }

        public void Save()
        {
            var key = Registry.CurrentUser.CreateSubKey(ApplicationKey);
            key.SetValue(StataLocationKey, Properties.StataLocation, RegistryValueKind.String);
        }

        public void Load()
        {
            var key = Registry.CurrentUser.OpenSubKey(ApplicationKey);
            if (key == null)
            {
                return;
            }

            Properties.StataLocation = key.GetValue(StataLocationKey, string.Empty).ToString();
        }
    }
}
