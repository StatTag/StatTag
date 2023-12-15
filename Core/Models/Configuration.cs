using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StatTag.Core.Models
{
    /// <summary>
    /// This manages externally stored configuration options for StatTag.  These are settings
    /// that the user does not customize through the UI, but that are loaded from an external
    /// file to provide some flexibility in system setup.
    /// 
    /// Because of this, we are only worrying about reading in the file, not writing it out.
    /// </summary>
    public class Configuration
    {
        // Define the JSON object keys for retrieval
        private const string JupyterKey = "Jupyter";
        private const string PythonKernelsKey = "PythonKernels";
        private const string RKernelsKey = "RKernels";

        /// <summary>
        /// Define the default Jupyter kernel identifier that we will use for Python.
        /// </summary>
        public const string DefaultPythonKernel = "python3";

        /// <summary>
        /// Define the default Jupyter kernel identifier that we will use for R.
        /// </summary>
        public const string DefaultRKernel = "ir";

        /// <summary>
        /// An instance of Configuration that is fully populated with acceptable default values.
        /// </summary>
        public static readonly Configuration Default = new Configuration();

        public static Configuration Load()
        {
            var config = Configuration.Default;

            // We expect a JSON file containing configuration information.  However, if it doesn't exist, we
            // are equipped to provide default values and will just silently return.
            try
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StatTag-config.json");
                if (!File.Exists(configPath))
                {
                    return config;
                }

                var deserializedObj = JsonConvert.DeserializeObject(File.ReadAllText(configPath));
                if (deserializedObj == null || !(deserializedObj is JObject))
                {
                    return config;
                }

                var configRoot = (JObject) deserializedObj;
                if (!configRoot.HasValues && !configRoot.ContainsKey(JupyterKey))
                {
                    return config;
                }

                var jupyter = (JObject) configRoot[JupyterKey];
                if (jupyter == null || !jupyter.ContainsKey(PythonKernelsKey))
                {
                    return config;
                }

                var pythonKernels = (JArray) jupyter[PythonKernelsKey];
                if (pythonKernels == null || pythonKernels.Count == 0)
                {
                    return config;
                }

                config.PythonKernels = pythonKernels.Select(x => x.ToString()).ToArray();

                var rKernels = (JArray)jupyter[RKernelsKey];
                if (pythonKernels == null || pythonKernels.Count == 0)
                {
                    return config;
                }

                config.RKernels = rKernels.Select(x => x.ToString()).ToArray();
            }
            catch (Exception)
            {
                // Eating the exception - NOM NOM NOM
            }

            return config;
        }

        public string[] PythonKernels { get; private set; }
        public string[] RKernels { get; private set; }

        private Configuration()
        {
            PythonKernels = new string[] { DefaultPythonKernel };
            RKernels = new string[] { DefaultRKernel };
        }
    }
}
