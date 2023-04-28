using System.Collections.Generic;
using System.Linq;

namespace CiscoConfigurationFileManager.Services.Configurations;

/// <summary>
/// A static class that replaces a given parameter in a list of strings with a replacement value.
/// </summary>
public static class ParameterReplacer
{
    /// <summary>
    /// Replaces a given parameter in a list of strings with a replacement value.
    /// </summary>
    /// <param name="commands">The list of strings to search for the parameter.</param>
    /// <param name="identifier">The parameter to search for.</param>
    /// <param name="replacementValue">The value to replace the parameter with.</param>
    /// <returns>A new list of strings with the parameter replaced with the replacement value.</returns>
    public static List<string> ReplaceParameter(List<string> commands, string identifier, string? replacementValue)
    {
        return commands.Select(q => q.Replace(identifier, replacementValue)).ToList();
    }
}