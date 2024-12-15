using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox.RuntimeConfiguration.Generation
{
    public interface IRuntimeConfigurationGenerator
    {
        public void GenerateConfigurations(RuntimeConfigurationGenerationOptions options);
    }
}
