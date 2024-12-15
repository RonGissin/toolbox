using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox.RuntimeConfiguration.AppSettings
{
    public interface IAppSettingsFileWriter
    {
        public void WriteSettings(string outputDirectory, string fileName, Dictionary<string, string> settings);
    }
}
