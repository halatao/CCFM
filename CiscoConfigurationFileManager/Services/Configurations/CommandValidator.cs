using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CiscoConfigurationFileManager.Services.Configurations;

/// <summary>
/// Contains methods for validating Cisco configuration commands and IP addresses.
/// </summary>
public class CommandValidator
{
    /// <summary>
    /// Lazily loaded list of regular expressions used for command validation.
    /// </summary>
    private static readonly Lazy<List<Regex>> Regexs = new(LoadRegexes);

    /// <summary>
    /// Loads regular expressions from a file.
    /// </summary>
    /// <returns>A list of regular expressions.</returns>
    private static List<Regex> LoadRegexes()
    {
        var regexs = new List<Regex>();
        using var sr = new StreamReader(Paths.Regex);
        while (sr.ReadLine() is { } line)
        {
            regexs.Add(new Regex(line));
        }

        return regexs;
    }

    /// <summary>
    /// Validates a single line of Cisco configuration.
    /// </summary>
    /// <param name="line">The line to validate.</param>
    /// <returns>True if the line is valid, otherwise false.</returns>
    public static bool ValidateLine(string line)
    {
        foreach (var r in Regexs.Value)
        {
            if (!r.IsMatch(line)) continue;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if the given string is a valid IP address.
    /// </summary>
    /// <param name="ip">The IP address to check.</param>
    /// <returns>True if the string is a valid IP address, otherwise false.</returns>
    public static bool IsIpAddress(string ip)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(ip,
            @"^^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    }

    /// <summary>
    /// Checks if the given string is a valid IP address with a subnet mask.
    /// </summary>
    /// <param name="ip">The IP address with subnet mask to check.</param>
    /// <returns>True if the string is a valid IP address with a subnet mask, otherwise false.</returns>
    public static bool IsIpWithMask(string ip)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(ip,
            @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\s+((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    }
}