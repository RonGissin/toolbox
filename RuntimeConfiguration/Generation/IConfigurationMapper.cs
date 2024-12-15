using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ToolBox.RuntimeConfiguration.Hierarchy;

namespace ToolBox.RuntimeConfiguration.Generation
{
    public interface IConfigurationMapper
    {
        public object MapFromJson(Type configurationType, JsonObject config, JsonObject combination, ConfigurationHierarchy hierarchy);
    }
}
