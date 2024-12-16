using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ToolBox.ConfigGeneration.Tool.AppSettings
{
    public interface IAppSettingsFileWriter
    {
        public void WriteSettings(string outputDirectory, string fileName, JsonObject settings);
    }
}
