using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Tests
{
    public static class TestUtil
    {
        public static string GetTestDataFolder(string testDataFolder)
        {
            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            string projectPath = String.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - 3));
            return Path.Combine(projectPath, "Fixtures", testDataFolder);
        }
    }
}
