using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox.ConfigGeneration.Tool.Hierarchy
{
    public interface IHierarchyLoader
    {
        ConfigurationHierarchy LoadHierarchy(string hierarchyFilePath);
    }
}
