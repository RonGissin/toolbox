using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using ToolBox.ConfigGeneration.Tool.Hierarchy;
using ToolBox.Safety;

namespace ToolBox.ConfigGeneration.Tool.Generation
{
    public class ConfigurationMapper : IConfigurationMapper
    {
        public object MapFromJson(Type configurationType, JsonObject config, JsonObject combination, ConfigurationHierarchy hierarchy)
        {
            Safe.ThrowIfNull(configurationType);
            Safe.ThrowIfNull(config);
            Safe.ThrowIfNull(combination);
            Safe.ThrowIfNull(hierarchy);
            
            // Get the type and create an instance of the configuration class
            var configInstance = Activator.CreateInstance(configurationType);

            foreach (var property in configurationType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Get the property name
                var propertyName = property.Name;

                // Check if the property exists in the JSON configuration
                if (config.TryGetPropertyValue(propertyName, out var propertyJson))
                {
                    // Drill down through the hierarchy using the combination
                    var currentNode = propertyJson as JsonObject;

                    foreach (var key in hierarchy.Hierarchy)
                    {
                        if (currentNode.TryGetPropertyValue("value", out var defaultNode))
                        {
                            var typedValue = Convert.ChangeType(defaultNode.ToString(), property.PropertyType);
                            property.SetValue(configInstance, typedValue);

                            break;
                        }

                        // Find the value in the current node using the combination key
                        var combinationValue = combination[key]?.ToString();
                        if (combinationValue != null && currentNode.TryGetPropertyValue(combinationValue, out var nextNode))
                        {
                            currentNode = nextNode as JsonObject;
                            continue;
                        }

                        throw new InvalidOperationException("configuration invalid.");
                    }

                    // If a value was found, set it to the property
                    if (currentNode != null && currentNode.TryGetPropertyValue("value", out var finalValue))
                    {
                        var typedValue = Convert.ChangeType(finalValue.ToString(), property.PropertyType);
                        property.SetValue(configInstance, typedValue);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Property {property.Name} in configuration {configurationType.Name} is missing the 'value' property.");
                    }
                }
            }

            return configInstance;
        }
    }
}
