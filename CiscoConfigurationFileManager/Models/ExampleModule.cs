using System.Collections.Generic;
using System.Linq;

namespace CiscoConfigurationFileManager.Models;

// <summary>
/// Represents an example module, which is a derived class of ModuleBase.
/// </summary>
public class ExampleModule : ModuleBase
{
    /// <summary>
    /// Gets or sets the configuration of the example module.
    /// </summary>
    public List<StringWrapperRead>? Configuration { get; set; }

    /// <summary>
    /// Gets a RunningModule instance based on the example module.
    /// </summary>
    /// <returns>A RunningModule instance.</returns>
    public RunningModule GetRunningModule()
    {
        return new RunningModule
        {
            Name = Name,
            Tag = Tag,
            OperationEnum = OperationEnum,
            Configuration = (Configuration ?? new List<StringWrapperRead>())
                .Select(q => new LineWrapper {Line = {Value = q.String ?? string.Empty}}).ToList()
        };
    }
}