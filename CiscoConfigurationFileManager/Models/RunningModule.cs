using System.Collections.Generic;

namespace CiscoConfigurationFileManager.Models;

/// <summary>
/// Represents a module with a name, tag, and a list of configuration lines.
/// </summary>
public class RunningModule : ModuleBase
{
    public List<LineWrapper>? Configuration { get; set; }
}