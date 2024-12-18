using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolBox.ConfigGeneration.Attributes;

namespace ToolBox.RuntimeConfigurationSample.src.configuration
{
    [RuntimeConfiguration]
    public class MyRuntimeConfiguration
    {
        public string FirstProperty { get; set; }

        public string SecondProperty { get; set; }
    }
}
